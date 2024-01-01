using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace stardew_access.Features;
using System.Timers;

using Tracker;
using Utils;
using Translation;
using static Utils.MiscUtils;
using static Utils.InputUtils;
using static Utils.MovementHelpers;
using static Utils.NPCUtils;

internal class ObjectTracker : FeatureBase
{
    private  bool sortByProximity;
    private  TrackedObjects? trackedObjects;
    private  Pathfinder? pathfinder;
    internal string? SelectedCategory;
    internal string? SelectedObject;
    private Dictionary<string, Dictionary<int, (string? name, string? category)>> favorites = [];
    private const int PressInterval = 500; // Milliseconds
    private readonly Timer lastPressTimer = new(PressInterval);
    private readonly Timer navigationTimer = new(PressInterval);
    private int lastFavoritePressed = 0;
    private int sameFavoritePressed = 0;
    private int navigateToFavorite = -1;
    private int _favoriteStack;
    public int FavoriteStack
    {
        get { return _favoriteStack; }
        set { _favoriteStack = Math.Max(0, value); }
    }

    private readonly int[] objectCounts = [0, 0, 0, 0, 0, 0];
    private readonly List<Action> updateActions;
    private int currentActionIndex = 0;
    private bool countHasChanged = false;

    private static ObjectTracker? instance;
    public new static ObjectTracker Instance
    {
        get
        {
            instance ??= new ObjectTracker();
            return instance;
        }
    }

    public ObjectTracker()
    {
        sortByProximity = MainClass.Config.OTSortByProximity;
        lastPressTimer.Elapsed += OnLastPressTimerElapsed;
        lastPressTimer.AutoReset = false; // So it only triggers once per start
        navigationTimer.Elapsed += OnNavigationTimerElapsed;
        navigationTimer.AutoReset = false;
        updateActions =
        [
            () => UpdateAndRunIfChanged(ref objectCounts[0], Game1.currentLocation.debris.Count, () => { Log.Debug("Debris count has changed."); countHasChanged = true; }),
            () => UpdateAndRunIfChanged(ref objectCounts[1], Game1.currentLocation.objects.Count(), () => { Log.Debug("Objects count has changed."); countHasChanged = true; }),
            () => UpdateAndRunIfChanged(ref objectCounts[2], Game1.currentLocation.furniture.Count, () => { Log.Debug("Furniture count has changed."); countHasChanged = true; }),
            () => UpdateAndRunIfChanged(ref objectCounts[3], Game1.currentLocation.resourceClumps.Count, () => { Log.Debug("ResourceClumps count has changed."); countHasChanged = true; }),
            () => UpdateAndRunIfChanged(ref objectCounts[4], Game1.currentLocation.terrainFeatures.Count(), () => { Log.Debug("TerrainFeatures count has changed."); countHasChanged = true; }),
            () => UpdateAndRunIfChanged(ref objectCounts[5], Game1.currentLocation.largeTerrainFeatures.Count, () => { Log.Debug("LargeTerrainFeatures count has changed."); countHasChanged = true; }),
        ];
        LoadFavorites();
    }

    private void OnLastPressTimerElapsed(object? sender, ElapsedEventArgs? e) => FavoriteKeysReset();

    private void OnNavigationTimerElapsed(object? sender, ElapsedEventArgs? e)
    {
        if (navigateToFavorite > 0)
        {
            SetFromFavorites(navigateToFavorite);
            navigateToFavorite = -1;
            MoveToCurrentlySelectedObject();
        }
    }

    public override void Update(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu != null && pathfinder != null && pathfinder.IsActive)
        {
            #if DEBUG
            Log.Verbose(
                "ObjectTracker->Update: a menu has opened, canceling auto walking.");
            #endif
            pathfinder.StopPathfinding();
            return;
        }

        if (!e.IsMultipleOf(15) || !MainClass.Config.OTAutoRefreshing) return;

        Tick();
    }

    public override bool OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        base.OnButtonPressed(sender, e);
        bool cancelAutoWalkingPressed = MainClass.Config.OTCancelAutoWalking.JustPressed();
        
        if (pathfinder != null && pathfinder.IsActive)
        {
            if (cancelAutoWalkingPressed)
            {
                #if DEBUG
                Log.Verbose("ObjectTracker->HandleKeys: cancel auto walking pressed, canceling auto walking for object tracker.");
                #endif
                pathfinder.StopPathfinding();
                MainClass.ModHelper!.Input.Suppress(e.Button);
                return true;
            }
            else if (IsAnyMovementKeyPressed())
            {
                #if DEBUG
                Log.Verbose("ObjectTracker->HandleKeys: movement key pressed, canceling auto walking for object tracker.");
                #endif
                pathfinder.StopPathfinding();
                MainClass.ModHelper!.Input.Suppress(e.Button);
                return true;
            }
            else if (IsUseToolKeyActive())
            {
                #if DEBUG
                Log.Verbose("ObjectTracker->HandleKeys: use tool button pressed, canceling auto walking for object tracker.");
                #endif
                pathfinder.StopPathfinding();
                MainClass.ModHelper!.Input.Suppress(e.Button);
                Game1.pressUseToolButton();
                return true;
            }
            else if (IsDoActionKeyActive())
            {
                #if DEBUG
                Log.Verbose("ObjectTracker->HandleKeys: action button pressed, canceling auto walking for object tracker.");
                #endif
                pathfinder.StopPathfinding();
                MainClass.ModHelper!.Input.Suppress(e.Button);
                Game1.pressActionButton(Game1.input.GetKeyboardState(), Game1.input.GetMouseState(),
                    Game1.input.GetGamePadState());
                return true;
            }
        }
        return false;
    }

    public override void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        base.OnButtonsChanged(sender, e);
        
        if (!Context.IsPlayerFree)
            return;
        Instance.HandleKeys(sender, e);
    }

    public override void OnPlayerWarped(object? sender, WarpedEventArgs e)
    {
        // reset the objects being tracked
        GetLocationObjects(resetFocus: true);
        // reset favorites stack to the first stack for new location.
        FavoriteStack = 0;
    }

    public void Tick()
    {
        if (MainClass.Config.OTAutoRefreshing || Game1.currentLocation == null) return;

        if (updateActions.Count == 0)
        {
            Log.Error("No update actions to run.");
            return;
        }

        // Cycle through the actions without wrapping around
        var (action, edgeOfList) = MiscUtils.Cycle(updateActions, ref currentActionIndex, wrapAround: false);

        // If we've reached the end of the cycle
        if (edgeOfList)
        {
            // If a change was detected, refresh the objects
            if (countHasChanged)
            {
                Log.Debug("Refreshing ObjectTracker; changes detected.");
                GetLocationObjects(resetFocus: false);
                countHasChanged = false;  // Reset the flag for the next cycle
            }

            // Manually reset the index to 0 for the next iteration
            currentActionIndex = 0;

            // Return without running the action
            return;
        }

        // Run the selected action
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Log.Error($"Error in RunUpdateAction: {ex.Message}");
        }
    }

    private bool RetryPathfinding(int attemptNumber, int maxRetries, Vector2? lastTargetedTile)
    {
        if (IsFocusValid() && attemptNumber < maxRetries)
        {
            if (trackedObjects != null && trackedObjects.GetObjects().TryGetValue("characters", out var characters))
            {
                foreach (var kvp in characters)
                {
                    NPC? character = kvp.Value.character;
                    GhostNPC(character, sameTile: true);
                }
            }
            return true;
        }
        GetLocationObjects(resetFocus: true);
        return false;
    }

    private void StopPathfinding(Vector2? lastTargetedTile)
    {
        FixCharacterMovement();
        if (lastTargetedTile != null) FacePlayerToTargetTile(lastTargetedTile.Value);
        ReadCurrentlySelectedObject();
        pathfinder?.Dispose();
    }

    private bool IsFocusValid()
    {
        if (SelectedCategory != null && SelectedObject != null)
            return trackedObjects?.GetObjects().ContainsKey(SelectedCategory) == true && trackedObjects.GetObjects()[SelectedCategory].ContainsKey(SelectedObject);
        return false;
    }

    private bool IsValidSelection()
    {
        return trackedObjects != null && SelectedCategory != null && SelectedObject != null;
    }

    private SpecialObject? GetCurrentlySelectedObject()
    {
        return SelectedCategory != null && SelectedObject != null && trackedObjects?.GetObjects()?.TryGetValue(SelectedCategory, out var categoryObjects) == true
            ? categoryObjects.TryGetValue(SelectedObject, out var selectedObject) ? selectedObject : null
            : null;
    }

    private void ReadCurrentlySelectedObject(bool readTileOnly = false)
    {
        if (!IsValidSelection())
            return;

        Farmer player = Game1.player;
        SpecialObject? sObject = GetCurrentlySelectedObject();

        if (sObject != null)
        {
            Vector2 playerTile = player.getTileLocation();
            Vector2? sObjectTile = sObject?.TileLocation;
            if (sObjectTile != null)
            {
                string direction = GetDirection(playerTile, sObjectTile.Value);
                string distance = GetDistance(playerTile, sObjectTile).ToString();
                Log.Info($"ASDF: SelecetedObject is {SelectedObject}");
                object? translationTokens = new
                {
                    object_name = SelectedObject ??
                                  Translator.Instance.Translate("feature-object_tracker-no_selected_object"),
                    only_tile = readTileOnly ? 1 : 0,
                    object_x = (int)sObjectTile.Value.X,
                    object_y = (int)sObjectTile.Value.Y,
                    player_x = (int)playerTile.X,
                    player_y = (int)playerTile.Y,
                    direction,
                    distance
                };
                MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-read_selected_object", true,
                    translationTokens: translationTokens);
            } else{
                Log.Info("WTF2??!?!?!?!!");
            }
        } else {
            Log.Info("WTF1??!?!?!?!!");
        }
    }

    private void SetFocusToFirstObject(bool resetCategory = false)
    {
        var objects = trackedObjects?.GetObjects();
        if (objects == null || !objects.Any())
        {
            MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-no_objects_found", true);
            return;
        }

        if (!objects.Keys.Any())
        {
            MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-no_categories_found", true);
            return;
        }

        // If SelectedCategory is unset or resetCategory is true, set SelectedCategory to the first available category
        if (string.IsNullOrEmpty(SelectedCategory) || resetCategory || !objects.ContainsKey(SelectedCategory))
        {
            SelectedCategory = objects.Keys.First();
            SelectedObject = null;
        }

        // Set SelectedObject to the first item in the current category
        if (SelectedCategory != null && objects.TryGetValue(SelectedCategory, out var catObjects) && catObjects.Any())
        {
            SelectedObject = catObjects.Keys.First();
        }
        else
        {
            SelectedObject = null;
        }

        string outputCategory = SelectedCategory ?? "No Category";
        string outputObject = SelectedObject ?? "No Object";
        Log.Debug($"Category: {outputCategory} | Object: {outputObject}");
    }

    internal void GetLocationObjects(bool resetFocus = true)
    {
        TrackedObjects tObjects = new();
        tObjects.FindObjectsInArea(sortByProximity);
        trackedObjects = tObjects;

        var objects = trackedObjects.GetObjects();

        if (!resetFocus && SelectedCategory != null && !objects.ContainsKey(SelectedCategory))
        {
            resetFocus = true;
        }

        if (resetFocus)
        {
            SetFocusToFirstObject();
        }

        if (trackedObjects == null || SelectedCategory == null || SelectedObject == null)
        {
            return;
        }

        if (!objects.TryGetValue(SelectedCategory, out var categoryObjects) || !categoryObjects.ContainsKey(SelectedObject))
        {
            SetFocusToFirstObject(false);
        }
    }

    private void Cycle(bool cycleCategories, bool back = false, bool wrapAround = false)
    {
        if (!IsValidSelection())
            return;

        var objects = trackedObjects?.GetObjects();
        string suffixText = string.Empty;
        string endOfList = Translator.Instance.Translate("feature-object_tracker-end_of_list");
        string startOfList = Translator.Instance.Translate("feature-object_tracker-start_of_list");
        string noObject = Translator.Instance.Translate("feature-object_tracker-no_object");
        string noCategory = Translator.Instance.Translate("feature-object_tracker-no_category");

        void CycleHelper(ref string? selectedItem, string[] items)
        {
            if (selectedItem is not null)
            {
                int index = Array.IndexOf(items, selectedItem);
                if (index == -1)
                {
                    SetFocusToFirstObject(cycleCategories);
                    index = 0; // Reset index to 0 after setting focus to first object
                }

                var (selected, edgeOfList) = MiscUtils.Cycle(items, ref index, back, wrapAround);
                selectedItem = selected;
                suffixText = edgeOfList ? (wrapAround ? (back ? endOfList : startOfList) : (back ? startOfList : endOfList)) : string.Empty;
            }
        }

        if (cycleCategories)
        {
            string[] categories = objects?.Keys.ToArray() ?? [];
            CycleHelper(ref SelectedCategory, categories);
            SetFocusToFirstObject(false);
        }
        else
        {
            string[] objectKeys = SelectedCategory != null && objects?.ContainsKey(SelectedCategory) == true
                ? objects[SelectedCategory].Keys.ToArray()
                : [];
            CycleHelper(ref SelectedObject, objectKeys);
        }

        suffixText = suffixText.Length > 0 ? ", " + suffixText : string.Empty;
        string spokenText = cycleCategories
            ? $"{SelectedCategory ?? noCategory}, {SelectedObject ?? noObject}" + suffixText
            : $"{SelectedObject ?? noObject}" + suffixText;

        MainClass.ScreenReader.Say(spokenText, true);
    }

    internal void HandleKeys(object? sender, ButtonsChangedEventArgs e)
    {
        bool cycleUpCategoryPressed = MainClass.Config.OTCycleUpCategory.JustPressed();
        bool cycleDownCategoryPressed = MainClass.Config.OTCycleDownCategory.JustPressed();
        bool cycleUpObjectPressed = MainClass.Config.OTCycleUpObject.JustPressed();
        bool cycleDownObjectPressed = MainClass.Config.OTCycleDownObject.JustPressed();
        bool readSelectedObjectPressed = MainClass.Config.OTReadSelectedObject.JustPressed();
        bool switchSortingModePressed = MainClass.Config.OTSwitchSortingMode.JustPressed();
        bool moveToSelectedObjectPressed = MainClass.Config.OTMoveToSelectedObject.JustPressed();
        bool readSelectedObjectTileLocationPressed = MainClass.Config.OTReadSelectedObjectTileLocation.JustPressed();
        
        int favoriteKeyJustPressed = 0;

        if (MainClass.Config.OTFavorite1.JustPressed()) favoriteKeyJustPressed = 1;
        else if (MainClass.Config.OTFavorite2.JustPressed()) favoriteKeyJustPressed = 2;
        else if (MainClass.Config.OTFavorite3.JustPressed()) favoriteKeyJustPressed = 3;
        else if (MainClass.Config.OTFavorite4.JustPressed()) favoriteKeyJustPressed = 4;
        else if (MainClass.Config.OTFavorite5.JustPressed()) favoriteKeyJustPressed = 5;
        else if (MainClass.Config.OTFavorite6.JustPressed()) favoriteKeyJustPressed = 6;
        else if (MainClass.Config.OTFavorite7.JustPressed()) favoriteKeyJustPressed = 7;
        else if (MainClass.Config.OTFavorite8.JustPressed()) favoriteKeyJustPressed = 8;
        else if (MainClass.Config.OTFavorite9.JustPressed()) favoriteKeyJustPressed = 9;
        else if (MainClass.Config.OTFavorite10.JustPressed()) favoriteKeyJustPressed = 10;
        else if (MainClass.Config.OTFavoriteDecreaseStack.JustPressed()) favoriteKeyJustPressed = 11;
        else if (MainClass.Config.OTFavoriteIncreaseStack.JustPressed()) favoriteKeyJustPressed = 12;

        if (favoriteKeyJustPressed > 0)
        {
            HandleFavorite(favoriteKeyJustPressed);
            foreach(var button in e.Pressed)
                MainClass.ModHelper!.Input.Suppress(button);
        }
        else 
        {
            if (e.Pressed.Any()) FavoriteKeysReset();
            if (cycleUpCategoryPressed)
            {
                Cycle(cycleCategories: true, back: true, wrapAround: MainClass.Config?.OTWrapLists ?? false);
            }
            else if (cycleDownCategoryPressed)
            {
                Cycle(cycleCategories: true, wrapAround: MainClass.Config?.OTWrapLists ?? false);
            }
            else if (cycleUpObjectPressed)
            {
                Cycle(cycleCategories: false, back: true, wrapAround: MainClass.Config?.OTWrapLists ?? false);
            }
            else if (cycleDownObjectPressed)
            {
                Cycle(cycleCategories: false, wrapAround: MainClass.Config?.OTWrapLists ?? false);
            }

            if (readSelectedObjectPressed || moveToSelectedObjectPressed || readSelectedObjectTileLocationPressed || switchSortingModePressed)
            {
                if (switchSortingModePressed)
                {
                    sortByProximity = !sortByProximity;
                    MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-sort_by_proximity", true,
                        translationTokens: new { is_enabled = sortByProximity ? 1 : 0 });
                }
                GetLocationObjects(resetFocus: false);
                if (readSelectedObjectPressed)
                {
                    ReadCurrentlySelectedObject();
                }
                if (moveToSelectedObjectPressed)
                {
                    MoveToCurrentlySelectedObject();
                }
                if (readSelectedObjectTileLocationPressed)
                {
                    ReadCurrentlySelectedObject(readTileOnly: true);
                }
            }
        }
    }

    private void MoveToCurrentlySelectedObject()
    {
        Log.Debug("Attempt pathfinding.");
        if (IsFocusValid())
        {
            ReadCurrentlySelectedObject();
        }

        Farmer player = Game1.player;
        SpecialObject? sObject = GetCurrentlySelectedObject();

        Vector2 playerTile = player.getTileLocation();
        Vector2? sObjectTile = (sObject != null) ? sObject.TileLocation : (Vector2?)null;

        Vector2? closestTile = sObject is not null ? (sObject.PathfindingOverride != null ? GetClosestTilePath((Vector2)sObject.PathfindingOverride) : GetClosestTilePath(sObjectTile)) : null;

        if (closestTile != null)
        {
            MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-moving_to", true,
                translationTokens: new
                {
                    object_x = (int)closestTile.Value.X,
                    object_y = (int)closestTile.Value.Y
                });
            pathfinder?.Dispose();
            pathfinder = new(RetryPathfinding, StopPathfinding);
            pathfinder.StartPathfinding(player, Game1.currentLocation, closestTile.Value.ToPoint());
        }
        else
        {
            MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-could_not_find_path", true);
        }
    }

    public void SaveToFavorites(int hotkey)
    {
        string location = Game1.currentLocation.NameOrUniqueName;
        if (!favorites.ContainsKey(location))
        {
            favorites[location] = [];
        }

        favorites[location][hotkey] = (SelectedObject, SelectedCategory);
        SaveFavorites();
    }

    public (string?, string?) GetFromFavorites(int hotkey)
    {
        string location = Game1.currentLocation.NameOrUniqueName;
        if (favorites.TryGetValue(location, out var locationFavorites) && locationFavorites.TryGetValue(hotkey, out var value))
        {
            return value;
        }

        return (null, null);
    }

    public void SetFromFavorites(int hotkey)
    {
        var (obj, category) = GetFromFavorites(hotkey);
        if (category != null && obj != null)
        {
            SelectedCategory = category;
            SelectedObject = obj;
        }
    }

    public void DeleteFavorite(int favoriteNumber)
    {
        string currentLocation = Game1.currentLocation.NameOrUniqueName;

        // Try to get the sub-dictionary for the current location
        if (favorites.TryGetValue(currentLocation, out var locationFavorites))
        {
            // Remove the favorite entry if it exists
            locationFavorites.Remove(favoriteNumber);
            if (locationFavorites.Count == 0)
            {
                // If empty, remove the location from the favorites
                favorites.Remove(currentLocation);
            }
            SaveFavorites();
        }
    }

    private void FavoriteKeysReset()
    {
        lastFavoritePressed = 0;
        sameFavoritePressed = -1;
        lastPressTimer.Stop();
    }

    private void HandleFavorite(int favKeyNum)
    {
        if (favKeyNum > 10)
        {
            switch (favKeyNum)
            {
                case 11:
                    FavoriteStack--;
                    break;
                case 12:
                    FavoriteStack++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(favKeyNum), favKeyNum, "The favorite key number cannot be greater than 12.");
            }
            MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-read_favorite_stack", true,
                new {
                    stack_number = FavoriteStack + 1
                }
            );
        }
        else
        {

            if (lastFavoritePressed == favKeyNum)
            {
                sameFavoritePressed++;
            }
            else
            {
                sameFavoritePressed = 1;
                lastFavoritePressed = favKeyNum;
                lastPressTimer.Stop();
                lastPressTimer.Start();
            }

            int favorite_number = favKeyNum + (FavoriteStack * 10);
            string? targetObject, targetCategory;
            (targetObject, targetCategory) = GetFromFavorites(favorite_number);
            bool isFavoriteSet = targetObject != null && targetCategory != null;
            // Logic for handling single, double, or triple presses
            if (sameFavoritePressed == 1)
            {
                // Handle single press
                if (isFavoriteSet)
                {
                    MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-read_favorite", true,
                        new
                        {
                            favorite_number,
                            target_object = targetObject,
                            target_category = targetCategory
                        }
                    );
                }
                else
                {
                    MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-favorite_unset", true,
                        new
                        {
                            favorite_number
                        }
                    );
                }
                lastPressTimer.Start();
            }
            else if (sameFavoritePressed == 2)
            {
                if (isFavoriteSet)
                {
                    // start a timer that will begin movement in `PressInterval` ms, allowing time for 3rd press to cancel it.
                    navigateToFavorite = favorite_number;
                    navigationTimer.Start();
                }
                else
                {
                    // this slot is unset; save current tracker target here
                    // Only if `SelectedObject` and `SelectedCategory` are both not null
                    if (SelectedObject != null && SelectedCategory != null)
                    {
                        SaveToFavorites(favorite_number);
                        MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-favorite_save", true,
                            new
                            {
                                selected_object = SelectedObject,
                                selected_category = SelectedCategory,
                                location_name = Game1.currentLocation!.NameOrUniqueName,
                                favorite_number
                            }
                        );
                    }
                    else
                    {
                        MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-no_destination_selected", true);
                    }
                }
            }
            else if (sameFavoritePressed >= 3)
            {
                navigationTimer.Stop();
                navigateToFavorite = -1;
                DeleteFavorite(favorite_number);
                MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-favorite_cleared", true,
                    new
                    {
                        location_name = Game1.currentLocation!.NameOrUniqueName,
                        favorite_number
                    }
                );
            }
        }
    }

    internal void LoadFavorites()
    {
        if (JsonLoader.TryLoadJsonFile("favorites.json", out JToken? jsonToken, "assets/TileData") && jsonToken is not null)
        {
            favorites = jsonToken.ToObject<Dictionary<string, Dictionary<int, (string?, string?)>>>() ?? [];
        }
        else
        {
            // Handle the case where the file couldn't be loaded or the JToken is null
            Log.Warn("Could not load favorites.json or the file is empty.");
            favorites = [];
        }
    }

    internal void SaveFavorites()
    {
        #if DEBUG
        Log.Verbose("Saving favorites");
        #endif
        JsonLoader.SaveJsonFile("favorites.json", favorites, "assets/TileData");
    }
}