using Microsoft.Xna.Framework;
using static stardew_access.Utils.MachineUtils;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Text;
using System.Text.Json;

namespace stardew_access.Utils
{
    public class TileInfo
    {
        private static readonly string[] trackable_machines;
        private static readonly Dictionary<int, string> ResourceClumpNameTranslationKeys;
        private static readonly Dictionary<int, (string category, string itemName)> ParentSheetIndexes;
        private static readonly Dictionary<string, Dictionary<(int, int), string>> BundleLocations;

        static TileInfo()
        {
            JsonLoader.TryLoadJsonArray("trackable_machines.json", out trackable_machines, subdir: "assets/TileData");
            JsonLoader.TryLoadJsonDictionary("resource_clump_name_translation_keys.json", out ResourceClumpNameTranslationKeys, subdir: "assets/TileData");
            JsonLoader.TryLoadNestedJson<int, (string, string)>(
                "ParentSheetIndexes.json", 
                ProcessParentSheetIndex,
                out ParentSheetIndexes,
                2,
                subdir: "assets/TileData"
            );
            JsonLoader.TryLoadNestedJson<string, Dictionary<(int, int), string>>(
                "BundleLocations.json", 
                ProcessBundleLocation,
                out BundleLocations,
                2,
                subdir: "assets/TileData"
            );

        }

        private static void ProcessParentSheetIndex(List<string> path, JsonElement element, ref Dictionary<int, (string, string)> result)
        {
            string category = path[0];
            string itemName = path[1];

            foreach (JsonElement indexElement in element.EnumerateArray())
            {
                int index = indexElement.GetInt32();
                result[index] = (category, itemName);
            }
        }

        private static void ProcessBundleLocation(List<string> path, JsonElement element, ref Dictionary<string, Dictionary<(int x, int y), string>> bundleLocations)
        {
            string locationName = path[0];
            string bundleName = path[1];
            int x = element[0].GetInt32();
            int y = element[1].GetInt32();

            if (!bundleLocations.ContainsKey(locationName))
            {
                bundleLocations[locationName] = new Dictionary<(int x, int y), string>();
            }
            
            bundleLocations[locationName][(x, y)] = bundleName;
        }

        ///<summary>Returns the name of the object at tile alongwith it's category's name</summary>
        public static (string? name, string? categoryName) GetNameWithCategoryNameAtTile(Vector2 tile, GameLocation? currentLocation)
        {
            (string? name, CATEGORY? category) = GetNameWithCategoryAtTile(tile, currentLocation);

            category ??= CATEGORY.Others;

            return (name, category.ToString());
        }

        ///<summary>Returns the name of the object at tile</summary>
        public static string? GetNameAtTile(Vector2 tile, GameLocation? currentLocation = null)
        {
            currentLocation ??= Game1.currentLocation;
            return GetNameWithCategoryAtTile(tile, currentLocation).name;
        }

        ///<summary>Returns the name of the object at tile alongwith it's category</summary>
        public static (string? name, CATEGORY? category) GetNameWithCategoryAtTile(Vector2 tile, GameLocation? currentLocation, bool lessInfo = false)
        {
            var (name, category) = GetTranslationKeyWithCategoryAtTile(tile, currentLocation, lessInfo);
            if (name == null)
                return (null, CATEGORY.Others);

            return (Translator.Instance.Translate(name, disableWarning: true), category);
        }

        public static (string? name, CATEGORY? category) GetTranslationKeyWithCategoryAtTile(Vector2 tile, GameLocation? currentLocation, bool lessInfo = false)
        {
            currentLocation ??= Game1.currentLocation;
            int x = (int)tile.X;
            int y = (int)tile.Y;

            var terrainFeature = currentLocation.terrainFeatures.FieldDict;

            if (currentLocation.isCharacterAtTile(tile) is NPC npc)
            {
                CATEGORY category = npc.isVillager() || npc.CanSocialize ? CATEGORY.Farmers : CATEGORY.NPCs;
                return (npc.displayName, category);
            }

            string? farmAnimal = GetFarmAnimalAt(currentLocation, x, y);
            if (farmAnimal is not null)
            {
                return (farmAnimal, CATEGORY.FarmAnimals);
            }

            string? door = GetDoorAtTile(currentLocation, x, y);
            if (door != null)
            {
                return (door, CATEGORY.Doors);
            }

            (string? name, CATEGORY category) staticTile = StaticTiles.GetStaticTileInfoAtWithCategory(x, y, currentLocation.Name);
            if (staticTile.name != null)
            {
                return (staticTile.name, staticTile.category);
            }

            (string? name, CATEGORY? category) dynamicTile = DynamicTiles.GetDynamicTileAt(currentLocation, x, y, lessInfo);
            if (dynamicTile.name != null)
            {
                return (dynamicTile.name, dynamicTile.category);
            }

            if (currentLocation.isObjectAtTile(x, y))
            {
                (string? name, CATEGORY? category) = GetObjectAtTile(currentLocation, x, y, lessInfo);
                return (name, category);
            }

            if (currentLocation.isWaterTile(x, y) && !lessInfo && IsCollidingAtTile(currentLocation, x, y))
            {
                return ("tile-water-name", CATEGORY.WaterTiles);
            }

            string? resourceClump = GetResourceClumpAtTile(currentLocation, x, y, lessInfo);
            if (resourceClump != null)
            {
                return (resourceClump, CATEGORY.ResourceClumps);
            }

            if (terrainFeature.TryGetValue(tile, out var tf))
            {
                (string? name, CATEGORY? category) = GetTerrainFeatureAtTile(tf);
                if (name != null)
                {
                    return (name, category);
                }
            }

            string? bush = GetBushAtTile(currentLocation, x, y, lessInfo);
            if (bush != null)
            {
                return (bush, CATEGORY.Bush);
            }

            string? junimoBundle = GetJunimoBundleAt(currentLocation, x, y);
            if (junimoBundle != null)
            {
                return (junimoBundle, CATEGORY.JunimoBundle);
            }

            // Track dropped items
            if (MainClass.Config.TrackDroppedItems)
            {
                try
                {
                    foreach (var item in currentLocation.debris)
                    {
                        int xPos = ((int)item.Chunks[0].position.Value.X / Game1.tileSize) + 1;
                        int yPos = ((int)item.Chunks[0].position.Value.Y / Game1.tileSize) + 1;
                        if (xPos != x || yPos != y || item.item == null) continue;

                        string name = item.item.DisplayName;
                        int count = item.item.Stack;
                        return (Translator.Instance.Translate("item-dropped_item-info", new {item_count = count, item_name = name}), CATEGORY.DroppedItems);
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"An error occurred while detecting dropped items:\n{e.Message}");
                }
            }

            return (null, CATEGORY.Others);
        }

        /// <summary>
        /// Gets the bush at the specified tile coordinates in the provided GameLocation.
        /// </summary>
        /// <param name="currentLocation">The GameLocation instance to search for bushes.</param>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <param name="lessInfo">Whether to return less information about the bush.</param>
        /// <returns>A string describing the bush if one is found at the specified coordinates, otherwise null.</returns>
        public static string? GetBushAtTile(GameLocation currentLocation, int x, int y, bool lessInfo = false)
        {
            Bush? bush = (Bush)currentLocation.getLargeTerrainFeatureAt(x, y);

            if (bush is null || (lessInfo && (bush.tilePosition.Value.X != x || bush.tilePosition.Value.Y != y)))
                return null;

            return TerrainUtils.GetBushInfoString(bush);
        }

        /// <summary>
        /// Determines if there is a Junimo bundle at the specified tile coordinates in the provided GameLocation.
        /// </summary>
        /// <param name="currentLocation">The GameLocation instance to search for Junimo bundles.</param>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <returns>The name of the Junimo bundle if one is found at the specified coordinates, otherwise null.</returns>
        public static string? GetJunimoBundleAt(GameLocation currentLocation, int x, int y)
        {
            string locationName = currentLocation.NameOrUniqueName;
            
            if (BundleLocations.TryGetValue(locationName, out Dictionary<(int, int), string>? bundleCoords))
            {
                if (bundleCoords.TryGetValue((x, y), out string? bundleName))
                {
                    if (currentLocation is CommunityCenter communityCenter)
                    {
                        if (communityCenter.shouldNoteAppearInArea(CommunityCenter.getAreaNumberFromName(bundleName)))
                        {
                            return $"{bundleName} bundle";
                        }
                    }
                    else if (currentLocation is AbandonedJojaMart)
                    {
                        return $"{bundleName} bundle";
                    }
                }
            }
            
            return null;  // No bundle was found
        }

        /// <summary>
        /// Determines if there is a collision at the specified tile coordinates in the provided GameLocation.
        /// </summary>
        /// <param name="currentLocation">The GameLocation instance to search for collisions.</param>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <returns>True if a collision is detected at the specified tile coordinates, otherwise False.</returns>
        public static bool IsCollidingAtTile(GameLocation currentLocation, int x, int y, bool lessInfo = false)
        {
            // This function highly optimized over readability because `currentLocation.isCollidingPosition` takes ~30ms on the Farm map, more on larger maps I.E. Forest.
            // Return the result of the logical comparison directly, inlining operations
            // Check if the tile is NOT a warp point and if it collides with an object or terrain feature
            // OR if the tile has stumps in a Woods location
            return !DoorUtils.IsWarpAtTile((x, y), currentLocation) &&
                   (currentLocation.isCollidingPosition(new Rectangle(x * 64 + 1, y * 64 + 1, 62, 62), Game1.viewport, true, 0, glider: false, Game1.player, pathfinding: true));
        }

        /// <summary>
        /// Gets the farm animal at the specified tile coordinates in the given location.
        /// </summary>
        /// <param name="location">The location where the farm animal might be found. Must be either a Farm or an AnimalHouse (coop, barn, etc).</param>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <returns>
        /// A string containing the farm animal's name, type, and age if a farm animal is found at the specified tile;
        /// null if no farm animal is found or if the location is not a Farm or an AnimalHouse.
        /// </returns>
        public static string? GetFarmAnimalAt(GameLocation location, int x, int y)
        {
            Dictionary<(int x, int y), FarmAnimal>? animalsByCoordinate = AnimalUtils.GetAnimalsByLocation(location);
            
            if (animalsByCoordinate == null || !animalsByCoordinate.TryGetValue((x, y), out FarmAnimal? foundAnimal))
                return null;

            string name = foundAnimal.displayName;
            int age = foundAnimal.age.Value;
            string type = foundAnimal.displayType;

            // TODO: Better age info
            return $"{name}, {type}, age {age}";
        }

        /// <summary>
        /// Retrieves the name and category of the terrain feature at the given tile.
        /// </summary>
        /// <param name="terrain">A reference to the terrain feature to be checked.</param>
        /// <returns>A tuple containing the name and category of the terrain feature at the tile.</returns>
        public static (string? name, CATEGORY? category) GetTerrainFeatureAtTile(Netcode.NetRef<TerrainFeature> terrain)
        {
            // Get the terrain feature from the reference
            var terrainFeature = terrain.Get();
            return TerrainUtils.GetTerrainFeatureInfoAndCategory(terrainFeature);
        }
        #region Objects
        /// <summary>
        /// Retrieves the name and category of an object at a specific tile in the game location.
        /// </summary>
        /// <param name="currentLocation">The current game location.</param>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        /// <param name="lessInfo">An optional parameter to display less information, set to false by default.</param>
        /// <returns>A tuple containing the object's name and category.</returns>
        public static (string? name, CATEGORY category) GetObjectAtTile(GameLocation currentLocation, int x, int y, bool lessInfo = false)
        {
            (string? name, CATEGORY category) toReturn = (null, CATEGORY.Others);

            // Get the object at the specified tile
            StardewValley.Object obj = currentLocation.getObjectAtTile(x, y);
            if (obj == null) return toReturn;

            int index = obj.ParentSheetIndex;
            toReturn.name = obj.DisplayName;

            // Get object names and categories based on index
            (string? name, CATEGORY category) correctNameAndCategory = GetCorrectNameAndCategoryFromIndex(index);

            // Check the object type and assign the appropriate name and category
            if (obj is Chest chest)
            {
                DiscreteColorPicker dummyColorPicker = new(0, 0);
                int colorIndex = dummyColorPicker.getSelectionFromColor(chest.playerChoiceColor.Get());
                string chestColor = colorIndex == 0
                    ? ""
                    : Translator.Instance.Translate("menu-item_grab-chest_colors",
                        new { index = colorIndex, is_selected = 0 }, TranslationCategory.Menu);
                toReturn = ($"{chestColor} {chest.DisplayName}", CATEGORY.Containers);
            }
            else if (obj is IndoorPot indoorPot)
            {
                toReturn.name = $"{obj.DisplayName}, {TerrainUtils.GetDirtInfoString(indoorPot.hoeDirt.Value, true)}";
            }
            else if (obj is Sign sign && sign.displayItem.Value != null)
            {
                toReturn.name = $"{sign.DisplayName}, {sign.displayItem.Value.DisplayName}";
            }
            else if (obj is Furniture furniture)
            {
                if (lessInfo && (furniture.TileLocation.X != x || furniture.TileLocation.Y != y))
                {
                    toReturn.category = CATEGORY.Others;
                    toReturn.name = null;
                }
                else
                {
                    toReturn.category = CATEGORY.Furnitures;
                }
            }
            else if (obj.IsSprinkler() && obj.heldObject.Value != null) // Detect the upgrade attached to the sprinkler
            {
                string heldObjectName = obj.heldObject.Value.Name;
                if (MainClass.ModHelper is not null)
                {
                    if (heldObjectName.Contains("pressure nozzle", StringComparison.OrdinalIgnoreCase))
                    {
                        toReturn.name = Translator.Instance.Translate("tile-sprinkler-pressure_nozzle-prefix", new { content = toReturn.name });
                    }
                    else if (heldObjectName.Contains("enricher", StringComparison.OrdinalIgnoreCase))
                    {
                        toReturn.name = Translator.Instance.Translate("tile-sprinkler-enricher-prefix", new { content = toReturn.name });
                    }
                    else
                    {
                        toReturn.name = $"{obj.heldObject.Value.DisplayName} {toReturn.name}";
                    }
                }
            }
            else if ((obj.Type == "Crafting" && obj.bigCraftable.Value) || obj.Name.Equals("crab pot", StringComparison.OrdinalIgnoreCase))
            {
                foreach (string machine in trackable_machines)
                {
                    if (obj.Name.Contains(machine, StringComparison.OrdinalIgnoreCase))
                    {
                        toReturn.name = obj.DisplayName;
                        toReturn.category = CATEGORY.Machines;
                    }
                }
            }
            else if (correctNameAndCategory.name != null)
            {
                toReturn = correctNameAndCategory;
            }
            else if (obj.name.Equals("stone", StringComparison.OrdinalIgnoreCase))
                toReturn.category = CATEGORY.Debris;
            else if (obj.name.Equals("twig", StringComparison.OrdinalIgnoreCase))
                toReturn.category = CATEGORY.Debris;
            else if (obj.name.Contains("quartz", StringComparison.OrdinalIgnoreCase))
                toReturn.category = CATEGORY.MineItems;
            else if (obj.name.Contains("earth crystal", StringComparison.OrdinalIgnoreCase))
                toReturn.category = CATEGORY.MineItems;
            else if (obj.name.Contains("frozen tear", StringComparison.OrdinalIgnoreCase))
                toReturn.category = CATEGORY.MineItems;

            if (toReturn.category == CATEGORY.Machines) // Fix for `Harvestable table` and `Busy nodes`
            {
                MachineState machineState = GetMachineState(obj);
                if (machineState == MachineState.Ready)
                    toReturn.name = $"Harvestable {toReturn.name}";
                else if (machineState == MachineState.Busy)
                    toReturn.name = $"Busy {toReturn.name}";
            }

            return toReturn;
        }

        /// <summary>
        /// Retrieves the correct name and category for an object based on its index and name.
        /// </summary>
        /// <param name="index">The object's index value.</param>
        /// <param name="objName">The object's name.</param>
        /// <returns>A tuple containing the object's correct name and category.</returns>
        private static (string? name, CATEGORY category) GetCorrectNameAndCategoryFromIndex(int index)
        {
            // Use the ParentSheetIndexes dictionary for fast lookups.
            if (ParentSheetIndexes.TryGetValue(index, out var info))
            {
                return (info.itemName, CATEGORY.FromString(info.category));
            }

            // If the index is not found in the ParentSheetIndexes dictionary, return the Others category.
            return (null, CATEGORY.Others);
        }
        #endregion  

        /// <summary>
        /// Gets the door information at the specified tile coordinates in the given location.
        /// </summary>
        /// <param name="currentLocation">The GameLocation where the door might be found.</param>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <returns>A string containing the door information if a door is found at the specified tile; null if no door is found.</returns>
        public static string? GetDoorAtTile(GameLocation currentLocation, int x, int y, bool lessInfo = false)
        {
            if (DoorUtils.GetAllDoors(Game1.currentLocation, lessInfo).TryGetValue((x, y), out var doorName))
            {
                return doorName!;
            }
            return null;
            /*// Create a Point object from the given tile coordinates
            Point tilePoint = new(x, y);

            // Access the doorList in the current location
            StardewValley.Network.NetPointDictionary<string, Netcode.NetString> doorList = currentLocation.doors;

            // Check if the doorList contains the given tile point
            if (doorList.TryGetValue(tilePoint, out string? doorName))
            {
                // Return the door information with the name if available, otherwise use "door"
                return doorName != null ? $"{doorName} door" : "door";
            }

            // No matching door found
            return null;*/
        }

        /// <summary>
        /// Gets the resource clump information at the specified tile coordinates in the given location.
        /// </summary>
        /// <param name="currentLocation">The GameLocation where the resource clump might be found.</param>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
        /// <returns>A string containing the resource clump information if a resource clump is found at the specified tile; null if no resource clump is found.</returns>
        public static string? GetResourceClumpAtTile(GameLocation currentLocation, int x, int y, bool lessInfo = false)
        {
            // Get the dictionary of resource clumps (this includes stumps in woods)
            Dictionary<(int x, int y), ResourceClump>? resourceClumpsByCoordinate = ResourceClumpUtils.GetResourceClumpsAtLocation(currentLocation);

            // Check if there's a resource clump at the given coordinates
            if (resourceClumpsByCoordinate?.TryGetValue((x, y), out ResourceClump? resourceClump) == true)
            {
                // Check if lessInfo condition is met
                if (!lessInfo || (resourceClump.tile.X == x && resourceClump.tile.Y == y))
                {
                    // Return the name of the resource clump or "Unknown" if not available
                    if (ResourceClumpNameTranslationKeys.TryGetValue(resourceClump.parentSheetIndex.Value, out string? translationKey))
                    {
                        return translationKey;
                    }
                    else
                    {
                        // Log the missing translation key and some info about the clump
                        Log.Warn($"Missing translation key for resource clump with parentSheetIndex {resourceClump.parentSheetIndex.Value}.", true);
                        return "common-unknown";
                    }
                }
            }

            // No matching resource clump found
            return null;
        }
    }
}
