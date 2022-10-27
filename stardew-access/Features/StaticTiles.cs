using Newtonsoft.Json.Linq;
using StardewValley;

namespace stardew_access.Features
{
    public class StaticTiles
    {
        private JObject? staticTilesData = null;
        private JObject? customTilesData = null;

        public StaticTiles()
        {
            if (MainClass.ModHelper == null)
                return;

            try
            {
                using (StreamReader file = new StreamReader(Path.Combine(MainClass.ModHelper.DirectoryPath, "assets", "static-tiles.json")))
                {
                    string json = file.ReadToEnd();
                    staticTilesData = JObject.Parse(json);
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

                MainClass.InfoLog($"Loaded custom-tile.json");
            }
            catch (System.Exception)
            {
                MainClass.ErrorLog($"custom-tiles.json file not found or an error occured while initializing custom-tiles.json\nThe path of the file should be:\n\t{Path.Combine(MainClass.ModHelper.DirectoryPath, "assets", "custom-tiles.json")}");
            }
        }

        public bool isAvailable(string locationName)
        {
            List<JObject> allData = new List<JObject>();

            if (staticTilesData != null) allData.Add(staticTilesData);
            if (customTilesData != null) allData.Add(customTilesData);

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
                        if (locationName.ToLower().Equals(locationNameInJson.ToLower())) return true;
                    }
                    else if (locationName.ToLower().Equals(location.Key.ToLower()))
                        return true;
                }
            }

            return false;
        }

        public string? getStaticTileInfoAt(int x, int y)
        {
            return getStaticTileInfoAtWithCategory(x, y).name;
        }

        public (string? name, CATEGORY category) getStaticTileInfoAtWithCategory(int x, int y)
        {
            List<JObject> allData = new List<JObject>();

            if (staticTilesData != null) allData.Add(staticTilesData);
            if (customTilesData != null) allData.Add(customTilesData);

            foreach (JObject data in allData)
            {
                foreach (KeyValuePair<string, JToken?> location in data)
                {
                    if (location.Key.Contains("||") && MainClass.ModHelper != null)
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
                        string uniqueModID = location.Key.Substring(location.Key.LastIndexOf("||") + 2);
                        string locationName = location.Key.Remove(location.Key.LastIndexOf("||"));
                        bool isLoaded = MainClass.ModHelper.ModRegistry.IsLoaded(uniqueModID);

                        if (!isLoaded) continue; // Skip if the specified mod is not loaded
                        if (!Game1.currentLocation.Name.ToLower().Equals(locationName.ToLower())) continue;
                    }
                    else if (!Game1.currentLocation.Name.ToLower().Equals(location.Key.ToLower())) continue;

                    if (location.Value != null)
                        foreach (var tile in ((JObject)location.Value))
                        {
                            if (tile.Value == null)
                                continue;

                            JToken? tileXArray = tile.Value["x"];
                            JToken? tileYArray = tile.Value["y"];
                            JToken? tileType = tile.Value["type"];

                            if (tileXArray == null || tileYArray == null || tileType == null)
                                continue;

                            bool isXPresent = false;
                            bool isYPresent = false;

                            foreach (var item in tileXArray)
                            {
                                if (short.Parse(item.ToString()) == x)
                                {
                                    isXPresent = true;
                                    break;
                                }
                            }

                            foreach (var item in tileYArray)
                            {
                                if (short.Parse(item.ToString()) == y)
                                {
                                    isYPresent = true;
                                    break;
                                }
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
            }
            return (null, CATEGORY.Others);
        }
    }
}