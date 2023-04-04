using System.IO;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using StardewValley;
using static stardew_access.Features.Utils;

namespace stardew_access.Features
{
    public class StaticTiles
    {
        // Static instance for the singleton pattern
        private static StaticTiles? _instance;

        /// <summary>
        /// The singleton instance of the <see cref="StaticTiles"/> class.
        /// </summary>
        public static StaticTiles Instance
        {
            get
            {
                _instance ??= new StaticTiles();
                return _instance;
            }
        }

        /// <summary>
        /// A nullable JsonElement containing static tile data.
        /// </summary>
        private static JsonElement? staticTilesData;

        /// <summary>
        /// A nullable JsonElement containing custom tile data.
        /// </summary>
        private static JsonElement? customTilesData;

        /// <summary>
        /// A dictionary that maps location names to tile data dictionaries for static tiles.
        /// Each tile data dictionary maps tile coordinates (x, y) to a tuple containing the object name and category.
        /// </summary>
        private static Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>> staticTilesDataDict = new();

        /// <summary>
        /// A dictionary that maps location names to tile data dictionaries for custom tiles.
        /// Each tile data dictionary maps tile coordinates (x, y) to a tuple containing the object name and category.
        /// </summary>
        private static Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>> customTilesDataDict = new();

        /// <summary>
        /// The file name of the JSON file containing static tile data.
        /// </summary>
        private const string StaticTilesFileName = "static-tiles.json";

        /// <summary>
        /// The file name of the JSON file containing custom tile data.
        /// </summary>
        private const string CustomTilesFileName = "custom-tiles.json";

        /// <summary>
        /// A dictionary that contains conditional lambda functions for checking specific game conditions.
        /// Each lambda function takes two arguments: a conditionType (string) and a uniqueModId (string) and returns a boolean value.
        /// </summary>
        /// <remarks>
        /// The following lambda functions are currently supported:
        /// <list type="bullet">
        /// <item>
        /// <description>"Farm": Checks if the current in-game farm type matches the given farm type (conditionType).</description>
        /// </item>
        /// <item>
        /// <description>"JojaMember": Checks if the player has the "JojaMember" mail. The input arguments are ignored.</description>
        /// </item>
        /// </list>
        /// Additional lambda functions can be added as needed.
        /// </remarks>
        private static readonly Dictionary<string, Func<string, string, bool>> conditionals = new()
		{
            ["Farm"] = (conditionType, uniqueModId) =>
            {
                if (string.IsNullOrEmpty(uniqueModId))
                {
                    // Branch for vanilla locations
                    // Calculate farmTypeIndex using the switch expression
                    int farmTypeIndex = conditionType.ToLower() switch
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

                    // Return true if the farmTypeIndex matches the current in-game farm type, otherwise false
                    return farmTypeIndex == Game1.whichFarm;
                }
                else
                {
                    // Branch for mod locations
                    // Log an error message and return false, as mod locations are not yet supported for the Farm conditional
                    MainClass.ErrorLog("Mod locations are not yet supported for the Farm conditional.");
                    return false;
                }
            },
            ["JojaMember"] = (conditionType, uniqueModId) =>
            {
                // Return true if the player has the "JojaMember" mail, otherwise false
                return Game1.MasterPlayer.mailReceived.Contains("JojaMember");
            }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticTiles"/> class.
        /// Loads the tile files and sets up the tile dictionaries.
        /// </summary>
        private StaticTiles()
        {
            LoadTilesFiles();
            SetupTilesDicts();
        }

        /// <summary>
        /// Loads the static and custom tile files.
        /// </summary>
        public static void LoadTilesFiles()
        {
            if (MainClass.ModHelper is null) return;

            staticTilesData = LoadJsonFile(StaticTilesFileName);
            customTilesData = LoadJsonFile(CustomTilesFileName);
        }

        /// <summary>
        /// Adds a conditional lambda function to the conditionals dictionary at runtime.
        /// </summary>
        /// <param name="conditionName">The name of the condition to be added.</param>
        /// <param name="conditionLambda">The lambda function to be added. It should accept two strings (conditionName and uniqueModID) and return a bool.</param>
        /// <returns>Returns true if the lambda was added successfully, and false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown if the conditionName or conditionLambda is null or empty.</exception>
        public static bool AddConditionalLambda(string conditionName, Func<string, string, bool> conditionLambda)
        {
            // Check if the conditionName is not null or empty
            if (string.IsNullOrEmpty(conditionName))
            {
                throw new ArgumentException("Condition name cannot be null or empty.", nameof(conditionName));
            }

            // Check if the conditionLambda is not null
            if (conditionLambda == null)
            {
                throw new ArgumentException("Condition lambda cannot be null.", nameof(conditionLambda));
            }

            // Check if the conditionName already exists in the dictionary
            if (conditionals.ContainsKey(conditionName))
            {
                MainClass.ErrorLog($"A conditional with the name '{conditionName}' already exists.");
                return false;
            }

            // Add the lambda to the dictionary
            conditionals.Add(conditionName, conditionLambda);
            return true;

        }

        /// <summary>
        /// Creates a location tile dictionary based on the given JSON dictionary.
        /// </summary>
        /// <param name="jsonDict">The JSON dictionary containing location tile data.</param>
        /// <returns>A dictionary mapping tile coordinates to tile names and categories.</returns>
        public static Dictionary<(short x, short y), (string name, CATEGORY category)> CreateLocationTileDict(JsonElement locationJson)
        {
            var jsonDict = locationJson.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
            var locationData = new Dictionary<(short x, short y), (string name, CATEGORY category)>(jsonDict.Count);

            // Iterate over the JSON dictionary
            foreach (var item in jsonDict)
            {
                var name = item.Key;

                // Error handling: Check if "x" and "y" properties exist in the JSON object
                if (!item.Value.TryGetProperty("x", out var xElement) || !item.Value.TryGetProperty("y", out var yElement))
                {
                    MainClass.ErrorLog($"Missing x or y property for {name}");
                    continue;
                }

                var xValues = xElement.EnumerateArray().Select(x => x.GetInt16()).ToArray();
                var yValues = yElement.EnumerateArray().Select(y => y.GetInt16()).ToArray();

                // Error handling: Ensure that x and y arrays are not empty
                if (xValues.Length == 0 || yValues.Length == 0)
                {
                    MainClass.ErrorLog($"Empty x or y array for {name}");
                    continue;
                }

                // Get the "type" property if it exists, otherwise use the default value "Others"
                var type = item.Value.TryGetProperty("type", out var typeElement) ? typeElement.GetString() : "Others";

                // Obtain the category instance
                var category = CATEGORY.FromString(type!);

                // Iterate over y and x values, adding entries to the locationData dictionary
                for (int j = 0; j < yValues.Length; j++)
                {
                    var y = yValues[j];
                    for (int i = 0; i < xValues.Length; i++)
                    {
                        var x = xValues[i];
                        locationData.TryAdd((x, y), (name, category));
                    }
                }
            }

            return locationData;
        }

        /// <summary>
                /// Represents the different categories of locations.
        /// </summary>
        public enum LocationCategory
        {
            /// <summary>
            /// Represents mod locations with conditional requirements.
            /// </summary>
            ModConditional,

            /// <summary>
            /// Represents mod locations without conditional requirements.
            /// </summary>
            Mod,

            /// <summary>
            /// Represents vanilla locations with conditional requirements.
            /// </summary>
            VanillaConditional,

            /// <summary>
            /// Represents vanilla locations without conditional requirements.
            /// </summary>
            Vanilla
        }

        /// <summary>
        /// Determines the location category based on the given location name.
        /// </summary>
        /// <param name="name">The location name.</param>
        /// <returns>The location category.</returns>
        public static LocationCategory GetLocationCategory(string name)
        {
            bool hasDoubleUnderscore = name.Contains("__");
            bool hasDoubleVerticalBar = name.Contains("||");

            if (hasDoubleUnderscore && hasDoubleVerticalBar)
                return LocationCategory.ModConditional;
            if (hasDoubleVerticalBar)
                return LocationCategory.Mod;
            if (hasDoubleUnderscore)
                return LocationCategory.VanillaConditional;

            return LocationCategory.Vanilla;
        }

        /// <summary>
        /// Sorts location data from a JsonElement into four dictionaries based on their type (mod conditional, mod, vanilla conditional, or vanilla).
        /// </summary>
        /// <param name="json">A JsonElement containing location data.</param>
        /// <returns>
        /// A tuple containing four dictionaries:
        /// - modConditionalLocations: A dictionary of mod locations with conditionals.
        /// - modLocations: A dictionary of mod locations without conditionals.
        /// - vanillaConditionalLocations: A dictionary of vanilla locations with conditionals.
        /// - vanillaLocations: A dictionary of vanilla locations without conditionals.
        /// Each dictionary maps a location name to another dictionary, which maps tile coordinates (x, y) to a tuple containing the object name and category.
        /// </returns>
        /// <remarks>
        /// This function iterates over the properties of the input JsonElement and categorizes each location based on the naming conventions.
        /// If a location has a conditional, the function checks if the condition is met before adding it to the respective dictionary.
        /// If a mod location is specified, the function checks if the mod is loaded before adding it to the respective dictionary.
        /// </remarks>
        public static (
            Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>> modConditionalLocations,
            Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>> modLocations,
            Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>> vanillaConditionalLocations,
            Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>> vanillaLocations
        ) SortLocationsByType(JsonElement json)
        {
            var modConditionalLocations = new Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>>();
            var modLocations = new Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>>();
            var vanillaConditionalLocations = new Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>>();
            var vanillaLocations = new Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>>();

            var categoryDicts = new Dictionary<LocationCategory, Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>>>
            {
                { LocationCategory.ModConditional, modConditionalLocations },
                { LocationCategory.Mod, modLocations },
                { LocationCategory.VanillaConditional, vanillaConditionalLocations },
                { LocationCategory.Vanilla, vanillaLocations }
            };

            foreach (var property in json.EnumerateObject())
            {
                if (property.Value.ValueKind != JsonValueKind.Object)
                {
                    MainClass.ErrorLog($"Invalid value type for {property.Name}");
                    continue;
                }

                string propertyName = property.Name;
                string uniqueModId = "";

                var splitModId = propertyName.Split("||", StringSplitOptions.RemoveEmptyEntries);
                if (splitModId.Length == 2)
                {
                    propertyName = splitModId[0];
                    uniqueModId = splitModId[1];

                    if (MainClass.ModHelper == null || !MainClass.ModHelper.ModRegistry.IsLoaded(uniqueModId))
                    {
                        continue;
                    }
                }

                var category = GetLocationCategory(propertyName);

                if (category == LocationCategory.VanillaConditional || category == LocationCategory.ModConditional)
                {
                    var splitPropertyName = propertyName.Split("__", StringSplitOptions.RemoveEmptyEntries);
                    if (splitPropertyName.Length == 2)
                    {
                        propertyName = splitPropertyName[0];
                        string conditionalName = splitPropertyName[1];

                        if (conditionals.TryGetValue(conditionalName, out var conditionalFunc))
                        {
                            if (!conditionalFunc(conditionalName, uniqueModId))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            MainClass.ErrorLog($"Unknown conditional name: {conditionalName}");
                            continue;
                        }
                    }
                }

                var locationDict = CreateLocationTileDict(property.Value);

                if (categoryDicts.TryGetValue(category, out var targetDict))
                {
                    targetDict.Add(propertyName, locationDict);
                }
                else
                {
                    MainClass.ErrorLog($"Unknown location category for {propertyName}");
                }
            }

            return (modConditionalLocations, modLocations, vanillaConditionalLocations, vanillaLocations);
        }

        /// <summary>
        /// Merges the contents of the source dictionary into the destination dictionary.
        /// If a key exists in both dictionaries and the associated values are dictionaries, the function merges them recursively.
        /// If the values are not dictionaries, the value from the source dictionary overwrites the value in the destination dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionaries.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionaries.</typeparam>
        /// <param name="destinationDictionary">The destination dictionary to merge the source dictionary into.</param>
        /// <param name="sourceDictionary">The source dictionary containing the data to merge into the destination dictionary.</param>
        private static void MergeDicts<TKey, TValue>(
            Dictionary<TKey, TValue> destinationDictionary,
            Dictionary<TKey, TValue> sourceDictionary) where TKey : notnull
        {
            if (destinationDictionary == null || sourceDictionary == null)
            {
                // Log a warning or throw an exception if either dictionary is null
                return;
            }

            foreach (var sourceEntry in sourceDictionary)
            {
                // Try to get the existing value from the destination dictionary
                if (destinationDictionary.TryGetValue(sourceEntry.Key, out var existingValue))
                {
                    // If both existing value and the source value are dictionaries,
                    // merge them recursively
                    if (existingValue is Dictionary<TKey, TValue> existingDictionary
                        && sourceEntry.Value is Dictionary<TKey, TValue> sourceSubDictionary)
                    {
                        MergeDicts(existingDictionary, sourceSubDictionary);
                    }
                    else
                    {
                        // Overwrite the existing value if it's not a dictionary
                        destinationDictionary[sourceEntry.Key] = sourceEntry.Value;
                    }
                }
                else
                {
                    // Add a new entry if the key doesn't exist in the destination dictionary
                    destinationDictionary[sourceEntry.Key] = sourceEntry.Value;
                }
            }
        }

        /// <summary>
        /// Builds a dictionary containing location data and tile information based on the provided JsonElement.
        /// </summary>
        /// <param name="json">A JsonElement containing the location and tile data.</param>
        /// <returns>A dictionary containing location data and tile information.</returns>
        public static Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>> BuildTilesDict(JsonElement json)
        {
            // Sort the locations by their types (modConditional, mod, vanillaConditional, vanilla)
            var (modConditionalLocations, modLocations, vanillaConditionalLocations, vanillaLocations) = SortLocationsByType(json);

            // Create a merged dictionary to store all the location dictionaries
            var mergedDict = new Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>>();

            // Merge each category-specific dictionary into the merged dictionary. Prioritize conditional locations whose conditions are true and mod locations where the corresponding mod is loaded. Overwrite their default and vanilla versions, respectively.
            MergeDicts(mergedDict, modConditionalLocations);
            MergeDicts(mergedDict, modLocations);
            MergeDicts(mergedDict, vanillaConditionalLocations);
            MergeDicts(mergedDict, vanillaLocations);

            return mergedDict;
        }

        /// <summary>
        /// Sets up the tile dictionaries (staticTilesDataDict and customTilesDataDict) using the data from the loaded JsonElements.
        /// </summary>
        public static void SetupTilesDicts()
        {
            if (staticTilesData.HasValue)
            {
                staticTilesDataDict = BuildTilesDict(staticTilesData.Value);
            }
            else
            {
                staticTilesDataDict = new Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>>();
            }
            
            if (customTilesData.HasValue)
            {
                customTilesDataDict = BuildTilesDict(customTilesData.Value);
            }
            else
            {
                customTilesDataDict = new Dictionary<string, Dictionary<(short x, short y), (string name, CATEGORY category)>>();
            }
        }

        /// <summary>
        /// Retrieves the tile information (name and optionally category) from the dictionaries based on the specified location and coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="currentLocationName">The name of the current location. Defaults to Game1.currentLocation.Name.</param>
        /// <param name="includeCategory">Specifies whether to include the tile's category in the returned tuple.</param>
        /// <returns>A tuple containing the tile's name and optionally its category. If the tile is not found, the name will be null and the category will be CATEGORY.Others if requested.</returns>
        private static (string? name, CATEGORY? category) GetTileInfoAt(int x, int y, string? currentLocationName = null, bool includeCategory = false)
        {
            currentLocationName ??= Game1.currentLocation.Name;

            if (customTilesDataDict != null && customTilesDataDict.TryGetValue(currentLocationName, out var customLocationDict))
            {
                if (customLocationDict != null && customLocationDict.TryGetValue(((short)x, (short)y), out var customTile))
                {
                    return (customTile.name, includeCategory ? customTile.category : (CATEGORY?)null);
                }
            }

            if (staticTilesDataDict != null && staticTilesDataDict.TryGetValue(currentLocationName, out var staticLocationDict))
            {
                if (staticLocationDict != null && staticLocationDict.TryGetValue(((short)x, (short)y), out var staticTile))
                {
                    return (staticTile.name, includeCategory ? staticTile.category : (CATEGORY?)null);
                }
            }

            return (null, includeCategory ? CATEGORY.Others : (CATEGORY?)null);
        }

        /// <summary>
        /// Retrieves the tile name from the dictionaries based on the specified location and coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="currentLocationName">The name of the current location. Defaults to Game1.currentLocation.Name.</param>
        /// <returns>The name of the tile if found, or null if not found.</returns>
        public static string GetStaticTileNameAt(int x, int y, string? currentLocationName = null)
        {
            var (name, _) = GetTileInfoAt(x, y, currentLocationName, includeCategory: false);
            return name ?? "";
        }

        /// <summary>
        /// Retrieves the tile information (name and category) from the dictionaries based on the specified location and coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="currentLocationName">The name of the current location. Defaults to Game1.currentLocation.Name.</param>
        /// <returns>A tuple containing the tile's name and category. If the tile is not found, the name will be null and the category will be CATEGORY.Others.</returns>
        public static (string? name, CATEGORY category) GetStaticTileInfoAtWithCategory(int x, int y, string? currentLocationName = null)
        {
            var (name, category) = GetTileInfoAt(x, y, currentLocationName, includeCategory: true);
            return (name, category ?? CATEGORY.Others);
        }
    }
}