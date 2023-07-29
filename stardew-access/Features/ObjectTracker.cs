using Microsoft.Xna.Framework;
using stardew_access.Features.Tracker;
using stardew_access.Utils;
using static stardew_access.Utils.MiscUtils;
using static stardew_access.Utils.MovementHelpers;
using static stardew_access.Utils.NPCUtils;
using static stardew_access.Utils.StringUtils;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace stardew_access.Features
{
    internal class ObjectTracker
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
            ReadCurrentlySelectedObject();
            FixCharacterMovement();
            if (lastTargetedTile != null) FacePlayerToTargetTile  (lastTargetedTile.Value);
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
                    MainClass.ScreenReader.Say((readTileOnly ? MainClass.Config.OTReadSelectedObjectTileText : MainClass.Config.OTReadSelectedObjectText).FormatWith(new Dictionary<string, object>
                    {
                        { "object", SelectedObject ?? "No selected object"},
                        { "objectX", sObjectTile?.X.ToString() ?? "0" },
                        { "objectY", sObjectTile?.Y.ToString() ?? "0" },
                        { "playerX", playerTile.X.ToString() },
                        { "playerY", playerTile.Y.ToString() },
                        { "direction", direction },
                        { "distance", distance }
                    }), true);
                }
            }
        }

        private void SetFocusToFirstObject(bool resetCategory = false)
        {
            var objects = trackedObjects?.GetObjects();
            if (objects == null || !objects.Any())
            {
                MainClass.ScreenReader.Say("No objects found.", true);
                return;
            }

            if (!objects.Keys.Any())
            {
                MainClass.ScreenReader.Say("No categories found.", true);
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
                    suffixText = edgeOfList ? (wrapAround ? (back ? "End of list." : "Start of list.") : (back ? "Start of list." : "End of list.")) : string.Empty;
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
            MainClass.ScreenReader.Say($"{SelectedCategory ?? "No Category"}, {SelectedObject ?? "No Object"}" + suffixText, true);
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
                    MainClass.ScreenReader.Say("Sort By Proximity: " + (sortByProximity ? "Enabled" : "Disabled"), true);
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

            if (pathfinder != null && pathfinder.IsActive)
            {
                if (MiscUtils.IsAnyMovementKeyActive())
                {
                    pathfinder.StopPathfinding();
                }
                else if (MiscUtils.IsUseToolKeyActive())
                {
                    pathfinder.StopPathfinding();
                    Game1.pressUseToolButton();
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
                MainClass.ScreenReader.Say($"Moving to {closestTile.Value.X}-{closestTile.Value.Y}.", true);
                pathfinder = new(RetryPathfinding, StopPathfinding);
                pathfinder.StartPathfinding(player, Game1.currentLocation, closestTile.Value.ToPoint());
            }
            else
            {
                MainClass.ScreenReader.Say("Could not find path to object.", true);
            }
        }
    }
}
