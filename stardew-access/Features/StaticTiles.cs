using Newtonsoft.Json.Linq;
using StardewValley;

namespace stardew_access.Features
{
    public class StaticTiles
    {
        private JObject? staticTilesData = null;
        private JObject? customTilesData = null;
        HashSet<KeyValuePair<string, JToken?>>? staticTilesDataSet = null;
        HashSet<KeyValuePair<string, JToken?>>? customTilesDataSet = null;

        public StaticTiles()
        {
            if (MainClass.ModHelper is  null)
                return;

            try
            {
                using (StreamReader file = new StreamReader(Path.Combine(MainClass.ModHelper.DirectoryPath, "assets", "static-tiles.json")))
                {
                    string json = file.ReadToEnd();
                    staticTilesData = JObject.Parse(json);
                }
                if (staticTilesData is not null)
                {
                    staticTilesDataSet = new HashSet<KeyValuePair<string, JToken>>(staticTilesData);
                }

                MainClass.InfoLog($"Loaded static-tile.json");
            }
            catch (System.Exception)
            {
                MainClass.ErrorLog($"static-tiles.json file not found or an error occured while initializing static-tiles.json\nThe path of the file should be:\n\t{Path.Combine(MainClass.ModHelper.DirectoryPath, "assets", "static-tiles.json")}");
            }

            try
            {
                using (StreamReader file = new StreamReader(Path.Combine(MainClass.ModHelper.DirectoryPath, "assets", "custom-tiles.json")))
                {
                    string json = file.ReadToEnd();
                    customTilesData = JObject.Parse(json);
                }
                if (customTilesData is not null)
                {
                    customTilesDataSet = new HashSet<KeyValuePair<string, JToken>>(customTilesData);
                }

                MainClass.InfoLog($"Loaded custom-tile.json");
            }
            catch (System.Exception)
            {
                MainClass.InfoLog($"custom-tiles.json file not found or an error occured while initializing custom-tiles.json\nThe path of the file should be:\n\t{Path.Combine(MainClass.ModHelper.DirectoryPath, "assets", "custom-tiles.json")}");
            }
        }

        public bool isAvailable(string locationName)
        {
            List<JObject> allData = new List<JObject>();

            if (customTilesData != null) allData.Add(customTilesData);
            if (staticTilesData != null) allData.Add(staticTilesData);

            foreach (JObject data in allData)
            {
                foreach (KeyValuePair<string, JToken?> location in data)
                {
                    if (location.Key.Contains("||") && MainClass.ModHelper != null)
                    {
                        string uniqueModID = location.Key.Substring(location.Key.LastIndexOf("||") + 2);
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

        public (string? name, CATEGORY category) GetTileFromSet(int x, int y, GameLocation currentLocation, HashSet<KeyValuePair<string, JToken?>> data)
        {
            foreach (KeyValuePair<string, JToken?> location in data)
            {
                string locationName = location.Key;
                if (locationName.Contains("||") && MainClass.ModHelper is not null)
                {
                    //                      Mod Specific Tiles
                    // We can add tiles that only get detected when the specified mod is loaded.
                    // Syntax: <location name>||<Mod's unique id, look into the mod's manifest.json for unique id>
                    // Example: THe following tile will only be detected if Stardew Valley Expanded mod is installed
                    //              {
                    //                  .
                    //                  .
                    //                  .
                    //                  "Town||FlashShifter.StardewValleyExpandedCP":{
                    //                      "<Tile Name>":{
                    //                          "x": [<x location(s)>],
                    //                          "y": [<y location(s)>],
                    //                          "type": "<Category name>"
                    //                      }
                    //                  },
                    //                  .
                    //                  .
                    //                  .
                    //              }
                    string uniqueModID = locationName.Substring(locationName.LastIndexOf("||") + 2);
                    locationName = locationName.Remove(locationName.LastIndexOf("||"));
                    bool isLoaded = MainClass.ModHelper.ModRegistry.IsLoaded(uniqueModID);

                    if (!isLoaded) continue; // Skip if the specified mod is not loaded
                }

                if (locationName.StartsWith("farm_", StringComparison.OrdinalIgnoreCase))
                {
                    string farmType = locationName.Substring(locationName.LastIndexOf("_") + 1);
                    int farmTypeIndex = getFarmTypeIndex(farmType);
                    locationName = locationName.Remove(locationName.LastIndexOf("_"));

                    if (farmTypeIndex != Game1.whichFarm) continue; // Skip if current farm type does not matches
                    // if (Game1.whichModFarm is not null) MainClass.DebugLog($"{farmType} {Game1.whichModFarm.MapName}");
                    if (farmTypeIndex != 7 || Game1.whichModFarm is null || !farmType.Equals(Game1.whichModFarm.MapName, StringComparison.OrdinalIgnoreCase)) continue; // Not tested but should work
                }

                if (locationName.Equals("town_joja", StringComparison.OrdinalIgnoreCase) && Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
                {
                    locationName = "town";
                }

                if (!currentLocation.Name.Equals(locationName, StringComparison.OrdinalIgnoreCase)) continue;
                if (location.Value is null) continue;

                foreach (var tile in ((JObject)location.Value))
                {
                    if (tile.Value is null) continue;

                    JToken? tileXArray = tile.Value["x"];
                    JToken? tileYArray = tile.Value["y"];
                    JToken? tileType = tile.Value["type"];

                    if (tileXArray is null || tileYArray is null || tileType is null)
                        continue;

                    bool isXPresent = false;
                    bool isYPresent = false;

                    foreach (var item in tileXArray)
                    {
                        if (short.Parse(item.ToString()) != x)
                            continue;

                        isXPresent = true;
                        break;
                    }

                    foreach (var item in tileYArray)
                    {
                        if (short.Parse(item.ToString()) != y)
                            continue;

                        isYPresent = true;
                        break;
                    }

                    if (isXPresent && isYPresent)
                    {
                        string key = tile.Key;
                        if (key.Contains('[') && key.Contains(']'))
                        {
                            int i1 = key.IndexOf('[');
                            int i2 = key.LastIndexOf(']');

                            if (i1 < i2)
                            {
                                key = key.Remove(i1, ++i2 - i1);
                            }
                        }

                        return (key.Trim(), CATEGORY.FromString(tileType.ToString().ToLower()));
                    }
                }
            }

            return (null, CATEGORY.Others);
        }

        public string? getStaticTileInfoAt(int x, int y)
        {
            return getStaticTileInfoAtWithCategory(x, y).name;
        }

        public (string? name, CATEGORY category) getStaticTileInfoAtWithCategory(int x, int y)
        {
            GameLocation currentLocation = Game1.currentLocation;
            if (customTilesDataSet is not null) return GetTileFromSet(x, y, currentLocation, customTilesDataSet);
            if (staticTilesDataSet is not null) return GetTileFromSet(x, y, currentLocation, staticTilesDataSet);

            return (null, CATEGORY.Others);
        }

        private int getFarmTypeIndex(string farmType)
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
