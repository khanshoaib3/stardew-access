using Newtonsoft.Json.Linq;
using StardewValley;
using System.Linq;

namespace stardew_access.Features
{
    public class StaticTiles
    {
        private static JObject? staticTilesData = null;
        private static JObject? customTilesData = null;
        private static Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>?>? staticTilesDataDict = null;
        private static Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>?>? customTilesDataDict = null;
        
        public StaticTiles()
        {
            if (MainClass.ModHelper is  null)
                return;

            if (staticTilesData is null) LoadTilesFiles();
            this.SetupTilesDicts();
        }

        public static void LoadTilesFiles()
        {
            try
            {
                using (StreamReader file = new(Path.Combine(MainClass.ModHelper.DirectoryPath, "assets", "static-tiles.json")))
                {
                    string json = file.ReadToEnd();
                    staticTilesData = JObject.Parse(json);
                }
                if (staticTilesData is not null)
                {
                }

                MainClass.InfoLog($"Loaded static-tile.json");
            }
            catch (System.Exception)
            {
                MainClass.ErrorLog($"static-tiles.json file not found or an error occured while initializing static-tiles.json\nThe path of the file should be:\n\t{Path.Combine(MainClass.ModHelper.DirectoryPath, "assets", "static-tiles.json")}");
            }

            try
            {
                using (StreamReader file = new(Path.Combine(MainClass.ModHelper.DirectoryPath, "assets", "custom-tiles.json")))
                {
                    string json = file.ReadToEnd();
                    customTilesData = JObject.Parse(json);
                }
                if (customTilesData is not null)
                {
                }

                MainClass.InfoLog($"Loaded custom-tile.json");
            }
            catch (System.Exception)
            {
                MainClass.InfoLog($"custom-tiles.json file not found or an error occured while initializing custom-tiles.json\nThe path of the file should be:\n\t{Path.Combine(MainClass.ModHelper.DirectoryPath, "assets", "custom-tiles.json")}");
            }
        }
        public static bool IsAvailable(string locationName)
        {
            List<JObject> allData = new();

            if (customTilesData != null) allData.Add(customTilesData);
            if (staticTilesData != null) allData.Add(staticTilesData);

            foreach (JObject data in allData)
            {
                foreach (KeyValuePair<string, JToken?> location in data)
                {
                    if (location.Key.Contains("||") && MainClass.ModHelper != null)
                    {
                        string uniqueModID = location.Key[(location.Key.LastIndexOf("||") + 2)..];
                        string locationNameInJson = location.Key.Remove(location.Key.LastIndexOf("||"));
                        bool isLoaded = MainClass.ModHelper.ModRegistry.IsLoaded(uniqueModID);

                        if (!isLoaded) continue; // Skip if the specified mod is not loaded
                        if (locationName.Equals(locationNameInJson, StringComparison.OrdinalIgnoreCase)) return true;
                    }
                    else if (locationName.Equals(location.Key, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        public static (string? name, CATEGORY category) GetTileFromDict(int x, int y)
        {
            if (staticTilesDataDict is not null && staticTilesDataDict.TryGetValue(Game1.currentLocation.Name, out var locationDict))
            {
                if (locationDict is not null && locationDict.TryGetValue(((short)x, (short)y), out var tile))
                {
                    //MainClass.DebugLog($"Tile ({x}, {y}) is in the dict as {tile.name}.");
                    return tile;
                }
            } /*else if (locationDict is null) {
                //MainClass.DebugLog($"Skipping null entry for location {Game1.currentLocation.Name}.");
            }
            else {
                MainClass.InfoLog($"Location {Game1.currentLocation.Name} not found in static tiles.");
            }*/
            return (null, CATEGORY.Others);
        }

        private static Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>?>? BuildTilesDict(JObject? data)
        {
            if (data is null) return null;
            //MainClass.DebugLog("Loading dict data");
            var comparer = StringComparer.OrdinalIgnoreCase;
            Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>?> tilesDict = new(comparer);
            foreach (KeyValuePair<string, JToken?> location in data)
            {
                try
                {
                    //MainClass.DebugLog($"Entering loop for location {location}.");
                    if (location.Value is null) continue;
                    string locationName = location.Key;
                    if (locationName.Contains("||") && MainClass.ModHelper is not null)
                    {
                        /*                      Mod Specific Tiles
                         * We can add tiles that only get detected when the specified mod is loaded.
                         * Syntax: <location name>||<Mod's unique id, look into the mod's manifest.json for unique id>
                         * Example: The following tile will only be detected if Stardew Valley Expanded mod is installed
                         *              {
                         *                  .
                         *                  .
                         *                  .
                         *                  "Town||FlashShifter.StardewValleyExpandedCP":{
                         *                      "<Tile Name>":{
                         *                          "x": [<x location(s)>],
                         *                          "y": [<y location(s)>],
                         *                          "type": "<Category name>"
                         *                      }
                         *                  },
                         *                  .
                         *                  .
                         *                  .
                         *              }
                        */
                        string uniqueModID = locationName[(locationName.LastIndexOf("||") + 2)..];
                        locationName = locationName.Remove(locationName.LastIndexOf("||"));
                        bool isLoaded = MainClass.ModHelper.ModRegistry.IsLoaded(uniqueModID);

                        if (!isLoaded) continue; // Skip if the specified mod is not loaded
                    }
                    //MainClass.DebugLog($"Loading tiles for {locationName}.");
                    if (location.Value.Type == JTokenType.Null)
                    {
                        tilesDict.Add(location.Key, null);
                        //MainClass.DebugLog($"Created null entry for location {location.Key}.");
                        //MainClass.DebugLog("SPAM!!!");
                        continue;
                    }
                    

                    Dictionary<(short x, short y), (string name, CATEGORY category)>? locationDict = new();
                    //MainClass.DebugLog($"Entering tiles loop for {locationName}.");
                    foreach (var tileInfo in ((JObject)location.Value))
                    {
                        if (tileInfo.Value == null) continue;
                        string key = tileInfo.Key;
                        var tile = tileInfo.Value;
                        if (tile.Type == JTokenType.Object )
                        {
                            JToken? tileXArray = tile["x"];
                            JToken? tileYArray = tile["y"];
                            JToken? tileType = tile["type"];

                            if (tileXArray is null || tileYArray is null || tileType is null)
                                continue;

                            //MainClass.DebugLog($"Adding tile {key} to location {locationName}.");
                            if (key.Contains('[') && key.Contains(']'))
                            {
                                int i1 = key.IndexOf('[');
                                int i2 = key.LastIndexOf(']');

                                if (i1 < i2)
                                {
                                    key = key.Remove(i1, ++i2 - i1);
                                }
                            }
                            (string key, CATEGORY category) tileData = (key.Trim(), CATEGORY.FromString(tileType.ToString().ToLower()));

                            foreach (var item_x in tileXArray)
                            {
                                short x = short.Parse(item_x.ToString());
                                foreach (var item_y in tileYArray)
                                {
                                    short y = short.Parse(item_y.ToString());
                                    (short x, short y) coords = (x, y);
                                    try
                                    {
                                        locationDict.Add(coords, tileData);            
                                    }
                                    catch (System.Exception e)
                                    {
                                        MainClass.ErrorLog($"Failed setting tile {key} for location {locationName}. Reason:\n\t{e}");
                                    }
                                }
                            }
                        }
                    }
                    //MainClass.DebugLog($"Location Dict has {locationDict.Count} members.");
                    if (locationDict.Count > 0)
                    {
                        //MainClass.DebugLog($"Adding locationDict for {locationName}");
                        tilesDict.Add(locationName, locationDict);
                        //MainClass.DebugLog($"Added locationDict for {locationName}");
                    }
                } catch (System.Exception e) {
                    if (location.Value is null || location.Value.Type == JTokenType.Null)
                    {
                        tilesDict.Add(location.Key, null);
                        //MainClass.DebugLog($"Created null entry for location {location.Key}.");
                    } else {
                        MainClass.ErrorLog($"Unable to build tiles dict; failed on location {location.Key} with value ({location.Value.GetType()}){location.Value}. Reason:\n\t{e}");
                        throw;
                    }
                }
            }

            if (tilesDict.Count > 0)
            {
                //MainClass.DebugLog("Dict loaded, returning.");
                return tilesDict;
            } else {
                //MainClass.DebugLog("Dict not loaded, returning null");
                return null;
            }
        }

        public void SetupTilesDicts()
        {
            //MainClass.DebugLog("Attempting to set dicts");
            try
            {
                staticTilesDataDict = BuildTilesDict(staticTilesData);
                if (staticTilesDataDict is not null)
                {
                    //MainClass.DebugLog($"staticTilesDataDict has {staticTilesDataDict.Count} entries.");
                    //MainClass.DebugLog($"Keys: {staticTilesDataDict.Keys}");
                } else {
                    //MainClass.DebugLog("Static tiles not loaded.");
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Failed to set static tiles dict. Reason: \n\t{e}");
            }
            try
            {
                customTilesDataDict = BuildTilesDict(customTilesData);
                if (customTilesDataDict is not null)
                {
                    //MainClass.DebugLog($"customTilesDataDict has {customTilesDataDict.Count} entries.");
                } else {
                    //MainClass.DebugLog("Custom tiles not loaded.");
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Faild to set custom tiles dict. Reason:\n\t{e}");
            }
            //MainClass.DebugLog("Successfully created tiles dicts.");
        }

        public static string? GetStaticTileInfoAt(int x, int y)
        {
            return GetStaticTileInfoAtWithCategory(x, y).name;
        }

        public static (string? name, CATEGORY category) GetStaticTileInfoAtWithCategory(int x, int y)
        {
            if (customTilesDataDict is not null) return GetTileFromDict(x, y);
            if (staticTilesDataDict is not null) return GetTileFromDict(x, y);

            return (null, CATEGORY.Others);
        }

        private static int GetFarmTypeIndex(string farmType)
        {
            return farmType.ToLower() switch
            {
                "default" => 0,
                "riverlands" => 1,
                "forest" => 2,
                "mountains" => 3,
                "combat" => 4,
                "fourcorners" => 5,
                "beach" => 6,
                _ => 7,
            };
        }
    }
}
