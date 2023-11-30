using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace stardew_access.Features;

using Microsoft.Xna.Framework;
using Tracker;
using Utils;
using Translation;
using static Utils.MiscUtils;
using static Utils.InputUtils;
using static Utils.MovementHelpers;
using static Utils.NPCUtils;
using StardewValley;

internal class ObjectTracker : FeatureBase
{
    private  bool sortByProximity;
    private  TrackedObjects? trackedObjects;
    private  Pathfinder? pathfinder;
    internal string? SelectedCategory;
    internal string? SelectedObject;
    private readonly int[] objectCounts = new int[6] { 0, 0, 0, 0, 0, 0 };
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
        updateActions = new List<Action>
        {
            () => UpdateAndRunIfChanged(ref objectCounts[0], Game1.currentLocation.debris.Count, () => { Log.Debug("Debris count has changed."); countHasChanged = true; }),
            () => UpdateAndRunIfChanged(ref objectCounts[1], Game1.currentLocation.objects.Count(), () => { Log.Debug("Objects count has changed."); countHasChanged = true; }),
            () => UpdateAndRunIfChanged(ref objectCounts[2], Game1.currentLocation.furniture.Count, () => { Log.Debug("Furniture count has changed."); countHasChanged = true; }),
            () => UpdateAndRunIfChanged(ref objectCounts[3], Game1.currentLocation.resourceClumps.Count, () => { Log.Debug("ResourceClumps count has changed."); countHasChanged = true; }),
            () => UpdateAndRunIfChanged(ref objectCounts[4], Game1.currentLocation.terrainFeatures.Count(), () => { Log.Debug("TerrainFeatures count has changed."); countHasChanged = true; }),
            () => UpdateAndRunIfChanged(ref objectCounts[5], Game1.currentLocation.largeTerrainFeatures.Count, () => { Log.Debug("LargeTerrainFeatures count has changed."); countHasChanged = true; }),
        };
    }

    public override void Update(object? sender, UpdateTickedEventArgs e)
    {
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
                MainClass.ModHelper.Input.Suppress(e.Button);
                return true;
            }
            else if (IsAnyMovementKeyPressed())
            {
                #if DEBUG
                Log.Verbose("ObjectTracker->HandleKeys: movement key pressed, canceling auto walking for object tracker.");
                #endif
                pathfinder.StopPathfinding();
                MainClass.ModHelper.Input.Suppress(e.Button);
                return true;
            }
            else if (IsUseToolKeyActive())
            {
                #if DEBUG
                Log.Verbose("ObjectTracker->HandleKeys: use tool button pressed, canceling auto walking for object tracker.");
                #endif
                pathfinder.StopPathfinding();
                MainClass.ModHelper.Input.Suppress(e.Button);
                Game1.pressUseToolButton();
                return true;
            }
            else if (IsDoActionKeyActive())
            {
                #if DEBUG
                Log.Verbose("ObjectTracker->HandleKeys: action button pressed, canceling auto walking for object tracker.");
                #endif
                pathfinder.StopPathfinding();
                MainClass.ModHelper.Input.Suppress(e.Button);
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
                object? translationTokens = new
                {
                    object_name = SelectedObject ??
                                  Translator.Instance.Translate("feature-object_tracker-no_selected_object"),
                    only_tile = readTileOnly ? 1 : 0,
                    object_x = (int)sObjectTile.Value.X,
                    object_y = (int)sObjectTile.Value.Y,
                    player_x = (int)playerTile.X,
                    player_y = (int)playerTile.Y,
                    direction = direction,
                    distance = distance
                };
                MainClass.ScreenReader.TranslateAndSay("feature-object_tracker-read_selected_object", true,
                    translationTokens: translationTokens);
            }
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
            string[] categories = objects?.Keys.ToArray() ?? Array.Empty<string>();
            CycleHelper(ref SelectedCategory, categories);
            SetFocusToFirstObject(false);
        }
        else
        {
            string[] objectKeys = SelectedCategory != null && objects?.ContainsKey(SelectedCategory) == true
                ? objects[SelectedCategory].Keys.ToArray()
                : Array.Empty<string>();
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
}