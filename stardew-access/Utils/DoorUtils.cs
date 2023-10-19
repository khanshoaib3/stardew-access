using stardew_access.Translation;
using StardewValley;
using System.Text.Json;
using System.Text.RegularExpressions; 

namespace stardew_access.Utils
{ 
    /// <summary>
    /// Provides utility functions for handling door-like objects in game locations.
    /// </summary>
    /// <remarks>
    /// The DoorUtils class offers a variety of static methods to interact with, identify, and categorize door-like objects in game locations.
    /// These door-like objects include static doors, warps, regular doors, interior doors, and doors identified by tile index.
    ///
    /// The class reads tile index data for doors from a JSON configuration file. The JSON keys for locations can be comma-separated lists, allowing 
    /// multiple location names to share the same set of door tile indexes.
    /// 
    /// It also recognizes entries from StaticTiles of the Doors category, and will prefer names defined there over internally generated names.
    /// </remarks>
    public static class DoorUtils
    {
        private static Dictionary<(int x, int y), string>? staticDoorsCache = null;
        private static Dictionary<(int x, int y), string>? warpsCache = null;
        private static Dictionary<(int x, int y), string>? doorsCache = null;
        private static GameLocation? lastLocation = null;
        private static Dictionary<string, Dictionary<int, string>> DoorTileIndexes = new(StringComparer.OrdinalIgnoreCase);

        static DoorUtils()
        {
            LoadDoorTileIndexes();
        }

        /// <summary>
        /// Loads door tile indexes from a JSON file into the DoorTileIndexes dictionary.
        /// </summary>
        /// <remarks>
        /// This method uses the JsonLoader utility class to read the "DoorTileIndexes.json" file.
        /// It then populates the DoorTileIndexes dictionary, which maps location names to another dictionary.
        /// The inner dictionary maps tile indexes to their default names, like "Elevator" or "Up Ladder".
        /// 
        /// Note: The keys for location names in the JSON may consist of multiple location names separated by commas.
        /// For example, "Mine,UndergroundMine" would be split into two keys: "Mine" and "UndergroundMine".
        /// </remarks>
        private static void LoadDoorTileIndexes()
        {
            if (JsonLoader.TryLoadNestedJson<string, Dictionary<int, string>>(
                "DoorTileIndexes.json",
                ProcessNestedItem,
                out DoorTileIndexes, // directly populate DoorTileIndexes
                1,
                subdir: "assets/TileData"
            ))
            {
                // No need to loop through loadedIndexes, as it is already handled in ProcessNestedItem
            }
            else
            {
                Log.Warn("Could not load DoorTileIndexes.json");
            }
            static void ProcessNestedItem(List<string> path, JsonElement element, ref Dictionary<string, Dictionary<int, string>> res)
            {
                if (path.Count > 0)
                {
                    var keys = path[0].Split(",");
                    if (element.ValueKind == JsonValueKind.Object)
                    {
                        var value = JsonSerializer.Deserialize<Dictionary<int, string>>(element.GetRawText());
                        if (value != null)
                        {
                            foreach (var key in keys)
                            {
                                res[key.Trim()] = value;  // Trim to remove any accidental spaces around the keys
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invalidates the internal cache for door data if the current location has changed.
        /// </summary>
        /// <param name="currentLocation">The current game location to check against.</param>
        /// <remarks>
        /// This method checks whether the current location has changed since the last time
        /// it was checked. If it has, the method invalidates the internal caches for static doors,
        /// warps, and other doors so that new data can be fetched.
        /// </remarks>
        private static void CheckAndInvalidateCache(GameLocation currentLocation)
        {
            if (currentLocation == lastLocation) return; // location hasn't changed; do nothing.
            staticDoorsCache = null;
            warpsCache = null;
            doorsCache = null;
            lastLocation = currentLocation;
        }

        /// <summary>
        /// Retrieves a dictionary of static doors in a given location.
        /// </summary>
        /// <param name="location">The game location to fetch static doors from. Defaults to the current location if null.</param>
        /// <param name="lessInfo">Whether to provide less information in the output. Defaults to false.</param>
        /// <param name="suffix">An optional suffix to append to the static door names. Defaults to the category of Doors.</param>
        /// <returns>A dictionary mapping coordinates to static door names.</returns>
        /// <remarks>
        /// This method fetches static doors based on a given category and location. It uses an internal
        /// cache to speed up subsequent requests. The cache is invalidated if the game location changes.
        /// </remarks>
        public static Dictionary<(int x, int y), string> GetStaticDoors(GameLocation? location = null, bool lessInfo = false, string? suffix = null)
        {
            location ??= Game1.currentLocation;
            suffix ??= CATEGORY.Doors.ToString();
            CheckAndInvalidateCache(location);
            staticDoorsCache ??= MainClass.TileManager.GetTilesByCategory(CATEGORY.Doors, "Static").ToDictionary(tile => ((int)tile.Coordinates.X, (int)tile.Coordinates.Y), tile => $"{Translator.Instance.Translate(tile.NameOrTranslationKey, TranslationCategory.StaticTiles)} {suffix}");
            return staticDoorsCache;
        }

        /// <summary>
        /// Checks if a Warp object exists at the specified coordinates in the given location.
        /// </summary>
        /// <param name="coords">The x, y coordinates to check for a Warp object.</param>
        /// <param name="currentLocation">The game location to check. Defaults to the current location if null.</param>
        /// <returns>True if a Warp object exists at the specified coordinates; otherwise, false.</returns>
        /// <remarks>
        /// This method iterates through all the Warp objects in the given location and checks
        /// if any match the specified x, y coordinates.
        /// </remarks>
        public static bool IsWarpAtTile((int x, int y) coords, GameLocation? currentLocation = null)
        {
            // Use the current location if one is not provided
            currentLocation ??= Game1.currentLocation;

            // Iterate through each Warp object and check if it matches the given coordinates
            foreach (var warp in currentLocation.warps)
            {
                if (warp.X == coords.x && warp.Y == coords.y)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves a dictionary of Warp objects in a given location.
        /// </summary>
        /// <param name="location">The game location to fetch Warps from. Defaults to the current location if null.</param>
        /// <param name="lessInfo">Whether to provide less information in the output. Defaults to false.</param>
        /// <returns>A dictionary mapping coordinates to Warp names.</returns>
        /// <remarks>
        /// This method fetches Warp objects based on the given location. It uses an internal
        /// cache to speed up subsequent requests. The cache is invalidated if the game location changes.
        ///
        /// This method also takes into consideration any static warp names that may have been defined.
        /// </remarks>
        public static Dictionary<(int x, int y), string> GetWarps(GameLocation? location = null, bool lessInfo = false)
        {
            location ??= Game1.currentLocation;
            CheckAndInvalidateCache(location);
            return warpsCache ?? LoadWarps(location, lessInfo);

            static Dictionary<(int x, int y), string> LoadWarps(GameLocation location, bool lessInfo)
            {
                Dictionary<(int x, int y), string> warpDict = new();
                var staticDoors = GetStaticDoors(location, lessInfo, Translator.Instance.Translate("tile-entrance"));

                foreach (Warp warp in location.warps)
                {
                    (int x, int y) coords = (warp.X, warp.Y);
                    if (!staticDoors.TryGetValue(coords, out string? warpName))
                        warpName = lessInfo ? warp.TargetName : $"{warp.TargetName} {Translator.Instance.Translate("tile-entrance")}";
                    warpDict.TryAdd(coords, warpName);
                }

                warpsCache = warpDict;
                return warpDict;
            }
        }

        /// <summary>
        /// Retrieves a dictionary of door objects in a given location.
        /// </summary>
        /// <param name="location">The game location to fetch doors from. Defaults to the current location if null.</param>
        /// <param name="lessInfo">Whether to provide less information in the output. Defaults to false.</param>
        /// <returns>A dictionary mapping coordinates to door names.</returns>
        /// <remarks>
        /// This method fetches door objects based on the given location. It uses an internal
        /// cache to speed up subsequent requests. The cache is invalidated if the game location changes.
        ///
        /// If a door has a custom name provided, it will use that. Otherwise, it generates a numeric identifier
        /// for unnamed doors. Adjacent unnamed doors are given the same name.
        ///
        /// This method also takes into consideration any static door names that may have been defined.
        /// </remarks>
        public static Dictionary<(int x, int y), string> GetDoors(GameLocation? location = null, bool lessInfo = false)
        {
            location ??= Game1.currentLocation;
            CheckAndInvalidateCache(location);
            return doorsCache ?? LoadDoors(location, lessInfo);

            static Dictionary<(int x, int y), string> LoadDoors(GameLocation location, bool lessInfo)
            {
                Dictionary<(int x, int y), string> doorDict = new();
                var staticDoors = GetStaticDoors(location, lessInfo);
                int unnamedDoorCount = 0;
                (int x, int y)? lastUnnamedDoorCoords = null;
                string? lastUnnamedDoorName = null;

                foreach (var door in location.doors.Pairs)
                {
                    int x = door.Key.X;
                    int y = door.Key.Y;
                    (int x, int y) coords = (x, y);
                    // if custom door name provided, use that (the out variable when if clause fails)
                    if (!staticDoors.TryGetValue(coords, out string? doorName))
                    {
                        // If door has a name provided, use that instead of custom name.
                        if (door.Value != null)
                        {
                            doorName = lessInfo ? door.Value : $"{door.Value} {Translator.Instance.Translate("tile-door")}";
                            lastUnnamedDoorCoords = null;  // Reset since this door has a name
                            lastUnnamedDoorName = null; // Reset the last unnamed door name
                        }
                        else // numericly generate unique name
                        {
                            bool isAdjacentToLastUnnamedDoor = lastUnnamedDoorCoords.HasValue &&
                                (Math.Abs(lastUnnamedDoorCoords.Value.x - x) + Math.Abs(lastUnnamedDoorCoords.Value.y - y) == 1);

                            if (!isAdjacentToLastUnnamedDoor)
                            {
                                lastUnnamedDoorName = $"{Translator.Instance.Translate("tile-door")}{++unnamedDoorCount}"; // Update the last unnamed door name
                                lastUnnamedDoorCoords = coords;  // Update last coordinates
                            }
                        }
                    }
                    else // has static name
                    {
                        lastUnnamedDoorCoords = null;  // Reset since this door has a name
                        lastUnnamedDoorName = null; // Reset the last unnamed door name
                    }

                    doorDict.TryAdd(coords, lastUnnamedDoorName ?? doorName!);
                }

                doorsCache = doorDict;
                return doorDict;
            }
        }

        /// <summary>
        /// Retrieves a dictionary of interior door objects for a given location.
        /// </summary>
        /// <param name="location">The game location to fetch interior doors from. Defaults to the current location if null.</param>
        /// <param name="lessInfo">Flag indicating whether to provide less detailed information in the output. Defaults to false.</param>
        /// <returns>A dictionary mapping coordinates to interior door names.</returns>
        /// <remarks>
        /// This function builds a dictionary of interior doors based on the location provided. Otherwise, the function generates a numeric name for unnamed doors.
        ///
        /// Adjacent unnamed doors will be given the same numeric identifier. The function also provides the state of the door (opened/closed)
        /// unless the <paramref name="lessInfo"/> flag is set to true.
        ///
        /// The function also takes into account any static door names that may have been defined.
        /// </remarks>
        public static Dictionary<(int x, int y), string> GetInteriorDoors(GameLocation? location = null, bool lessInfo = false)
        {
            location ??= Game1.currentLocation;

            Dictionary<(int x, int y), string> interiorDoorDict = new();
            var staticDoors = GetStaticDoors(location, lessInfo);
            int unnamedDoorCount = 0;
            (int x, int y)? lastUnnamedDoorCoords = null;
            string? lastUnnamedDoorName = null;

            foreach (var pair in location.interiorDoors.Pairs)
            {
                int x = pair.Key.X;
                int y = pair.Key.Y;
                bool isOpen = pair.Value;
                (int x, int y) coords = (x, y);

                // if custom door name provided, use that
                if (!staticDoors.TryGetValue(coords, out string? doorName))
                {
                    bool isAdjacentToLastUnnamedDoor = lastUnnamedDoorCoords.HasValue &&
                        (Math.Abs(lastUnnamedDoorCoords.Value.x - x) + Math.Abs(lastUnnamedDoorCoords.Value.y - y) == 1);

                    if (!isAdjacentToLastUnnamedDoor)
                    {
                        lastUnnamedDoorName = $"{Translator.Instance.Translate("tile-interior_door")} {++unnamedDoorCount}";
                        lastUnnamedDoorCoords = (x, y);
                    }
                }
                else // has static name
                {
                    // Reset lastUnnamedDoor... since this door has a name
                    lastUnnamedDoorCoords = null;
                    lastUnnamedDoorName = null;
                }

                string doorState = Translator.Instance.Translate("tile-door_state-" + (isOpen ? "opened" : "closed"));
                interiorDoorDict.TryAdd(coords, $"{(lastUnnamedDoorName ?? doorName!)}{(lessInfo ? string.Empty : $": {doorState} ")}");
            }

            return interiorDoorDict;
        }

        /// <summary>
        /// Retrieves a dictionary of door-like objects based on Buildings-layer tile index for a given location.
        /// </summary>
        /// <param name="location">The game location to fetch doors from. Defaults to the current location if null.</param>
        /// <returns>A dictionary mapping coordinates to door names.</returns>
        /// <remarks>
        /// This function builds a dictionary of doors based on the tile indexes in the location provided. It uses a predefined set of tile indexes 
        /// that correspond to "door-like" objects (e.g., elevators, ladders, etc.). The function first strips any trailing digits from the 
        /// location name and tries to find a match in the DoorTileIndexes dictionary, which is loaded from json.
        /// 
        /// If a static name for the door exists, that name is used; otherwise, a default name based on the tile index is used. For unnamed doors, 
        /// a numeric identifier is generated to create unique names, E.G. "Down Ladder 2".
        /// dictionary.
        /// </remarks>
        public static Dictionary<(int x, int y), string> GetDoorsByTileIndex(GameLocation? location = null)
        {
            location ??= Game1.currentLocation;
            Dictionary<(int x, int y), string> doorIndexes     = new();
            
            // Strip trailing digits from location name
            string strippedName = Regex.Replace(location.NameOrUniqueName, @"\d+$", "");

            // Look for the location in the DoorTileIndexes
            DoorTileIndexes.TryGetValue(strippedName, out var locationIndexes);

            if (locationIndexes == null)
            {
                // If not found, exit the method
                return doorIndexes    ;
            }

            var buildingTiles = TileUtils.GetTilesByLayer("Buildings", location);
            Dictionary<string, HashSet<(int x, int y)>> nameLocations = new();

            // Loop over potential door indexes based on locationIndexes
            foreach (var doorIndex in locationIndexes.Keys)
            {
                if (buildingTiles?.TryGetValue(doorIndex, out HashSet<(int x, int y)>? tileCoords) == true)
                {
                    foreach (var coords in tileCoords)
                    {
                        // Use the default name
                        string? defaultName = locationIndexes[doorIndex];
                        if (!nameLocations.ContainsKey(defaultName))
                        {
                            nameLocations[defaultName] = new HashSet<(int x, int y)>();
                        }
                        nameLocations[defaultName].Add(coords);
                    }
                }
            }

            // Check for static names and remove them from the default set if they exist
            var staticDoors = GetStaticDoors(location);
            foreach (var staticDoor in staticDoors)
            {
                doorIndexes[staticDoor.Key] = staticDoor.Value;
                
                foreach (var entry in nameLocations)
                {
                    entry.Value.Remove(staticDoor.Key);
                }
            }
            
            // Update names based on counts
            foreach (var entry in nameLocations)
            {
                string baseName = entry.Key;
                var locations = entry.Value;

                if (locations.Count == 1)
                {
                    doorIndexes[locations.First()] = baseName;
                }
                else
                {
                    int count = 1;
                    foreach (var coords in locations)
                    {
                        doorIndexes[coords] = $"{baseName} {count++}";
                    }
                }
            }

            return doorIndexes;
        }

        /// <summary>
        /// Retrieves a comprehensive dictionary of all door-like objects in a specified game location.
        /// </summary>
        /// <param name="location">The game location to search for doors. If null, defaults to the current game location.</param>
        /// <param name="lessInfo">A flag indicating whether to retrieve less detailed information. Defaults to false.</param>
        /// <returns>A dictionary mapping tile coordinates to door names, combining various types of doors (static, warps, regular doors, interior doors, and doors by tile index).</returns>
        /// <remarks>
        /// This function consolidates various types of door-like objects into a single dictionary. It includes:
        /// - Static doors: Defined doors with fixed names and positions.
        /// - Warps: Teleportation points to other locations.
        /// - Regular Doors: Doors in the location, E.G. into and out of buildings.
        /// - Interior Doors: Doors within a building or structure.
        /// - Tile Index Doors: Doors identified by their tile index based on the location's building layer.
        ///
        /// The function makes use of other utility functions to fetch each type of door and then combines them into a single dictionary.
        /// </remarks>
        public static Dictionary<(int x, int y), string> GetAllDoors(GameLocation? location = null, bool lessInfo = false)
        {
            location ??= Game1.currentLocation;
            
            Dictionary<(int x, int y), string> allDoors = new();

            // Add staticDoors, warps, doors, and interior doors
            var staticDoors = GetStaticDoors(location, lessInfo);
            var warps = GetWarps(location, lessInfo);
            var doors = GetDoors(location, lessInfo);
            var interiorDoors = GetInteriorDoors(location, lessInfo);
            var tileIndexDoors = GetDoorsByTileIndex(location);

            foreach (var entry in staticDoors)
            {
                allDoors[entry.Key] = entry.Value;
            }

            foreach (var entry in warps)
            {
                allDoors[entry.Key] = entry.Value;
            }

            foreach (var entry in doors)
            {
                allDoors[entry.Key] = entry.Value;
            }

            foreach (var entry in interiorDoors)
            {
                allDoors[entry.Key] = entry.Value;
            }

            foreach (var entry in tileIndexDoors)
            {
                allDoors[entry.Key] = entry.Value;
            }

            return allDoors;
        }
    }
}