using Microsoft.Xna.Framework;
using stardew_access.Utils;
using StardewValley;
using System.Text.Json;

namespace stardew_access.Tiles
{
    public class AccessibleTileManager
    {
        // Dictionary to hold parsed and validated json data for static tiles
        private Dictionary<string, AccessibleLocationData> LocationData = new(StringComparer.OrdinalIgnoreCase);

        // Dictionary to map location names to Accessiblelocations
        private Dictionary<string, AccessibleLocation> Locations { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        // Private instance variable
        private static AccessibleTileManager? _instance;

        // Public property to access the instance
        public static AccessibleTileManager Instance
        {
            get
            {
                _instance ??= new AccessibleTileManager();
                return _instance;
            }
        }

        // Private constructor to prevent multiple instances
        private AccessibleTileManager()
        {
            // Call the Initialize method to load data
            //Initialize();
        }

        internal void Initialize()
        {
            Log.Trace("Initializing     AccessibleTileManager");
            // First try converting the old custom-tiles.json
            if (ConvertOldCustomTilesFormat())
                Log.Alert($"Your custom-tiles.json file was updated to the new format,. You can find the new file under assets/TileData/tiles_user.json. Your original file was renamed to custom-tiles.old.json.");
            if (JsonLoader.TryLoadNestedJsonWithUserFile(
                "tiles.json", 
                TileDataProcessor, 
                out LocationData, 
                2, 
                "assets/TileData",
                true
            ))
            {
                Log.Info("Successfully initialized tile data.");
            }
            else
            {
                // Log an error if the load fails
                Log.Error("Failed to initialize tile data.");
            }
        }

        private void TileDataProcessor(List<string> path, JsonElement element, ref Dictionary<string, AccessibleLocationData> result, bool isUserFile)
        {
            string locationName = path[0];
            string arrayIndex = path[1];
            #if DEBUG
            Log.Verbose($"Attempting to add tile at index {arrayIndex} to location {locationName}");
            #endif

            // Check if `locationName` is actually multiple location names.
            if (locationName.Contains(','))
            {
                // Split the locationName by commas and remove any whitespace.
                var locationNames = locationName.Split(',')
                                                .Select(name => name.Trim())
                                                .ToList();

                // Recursively call the function for each individual location name.
                foreach (var singleLocationName in locationNames)
                {
                    // Create a new `path` for the recursive calls
                    var newPath = new List<string>(path)
                    {
                        [0] = singleLocationName // but update the locationName 
                    };
                    TileDataProcessor(newPath, element, ref result, isUserFile);
                }
                return;
            }

            // Initialize an AccessibleLocationData instance

            if (!result.TryGetValue(locationName, out AccessibleLocationData locationData))
            {
                locationData = new AccessibleLocationData();
                result[locationName] = locationData;
            }

            // Populate the AccessibleLocationData instance with JSON data

            // Initialize variables to store values from JSON
            string? nameOrTranslationKey = null, dynamicNameOrTranslationKey = null, dynamicCoordinates = null, category = "Other";
            int[] xArray = Array.Empty<int>(), yArray = Array.Empty<int>();
            bool isEvent = false;
            string[] withMods = Array.Empty<string>(), conditions = Array.Empty<string>();

            // Try to get each property

            // Shared properties
            // Used by both AccessibleLocation and AccessibleTile
            if (element.TryGetProperty("WithMods", out JsonElement withModsElement) && withModsElement.ValueKind != JsonValueKind.Null)
                withMods = withModsElement.EnumerateArray().Select(m => m.GetString()).Where(m => m != null).Select(m => m!).ToArray();

            if (	element.TryGetProperty("Conditions", out JsonElement conditionsElement) && conditionsElement.ValueKind != JsonValueKind.Null)
                conditions = conditionsElement.EnumerateArray().Select(c => c.GetString()).Where(c => c != null).Select(c => c!).ToArray();

            if (element.TryGetProperty("Event", out JsonElement eventElement) && eventElement.ValueKind != JsonValueKind.Null)
                isEvent = eventElement.GetBoolean();

            // Check if this is the locationSettings entry
            if (element.TryGetProperty("IsLocationSettings", out JsonElement isLocationSettingsElement) && isLocationSettingsElement.GetBoolean())
            {
                // update the withMods and conditions lists with new entries
                locationData.WithMods = locationData.WithMods.Concat(withMods.Except(locationData.WithMods)).ToArray();
                locationData.Conditions = locationData.Conditions.Concat(conditions.Except(locationData.Conditions)).ToArray();
                locationData.IsEvent = isEvent;
                // Settings added
                return;
            }

            // Handle Tile specific properties

            // Load name and/or name generating function
            bool hasStaticName = element.TryGetProperty("NameOrTranslationKey", out JsonElement nameElement) && nameElement.ValueKind != JsonValueKind.Null;
            if (hasStaticName)
                nameOrTranslationKey = nameElement.GetString();

            bool hasDynamicName = element.TryGetProperty("DynamicNameOrTranslationKey", out JsonElement dynamicNameElement) && dynamicNameElement.ValueKind != JsonValueKind.Null;
            if (hasDynamicName)
                dynamicNameOrTranslationKey = dynamicNameElement.GetString();

            // Validate at least one of them is set; both is ok.
            if (!(hasStaticName || hasDynamicName))
                throw new InvalidOperationException("Either NameOrTranslationKey or DynamicNameOrTranslationKey must be set.");

            // Load coordinates or coordinates generating function
            bool hasStaticXCoordinates = element.TryGetProperty("X", out JsonElement xElement) && xElement.ValueKind != JsonValueKind.Null;
            if (hasStaticXCoordinates)
                xArray = xElement.EnumerateArray().Select(x => x.GetInt32()).ToArray() ?? Array.Empty<int>();

            bool hasStaticYCoordinates = element.TryGetProperty("Y", out JsonElement yElement) && yElement.ValueKind != JsonValueKind.Null;
            if (hasStaticYCoordinates)
                yArray = yElement.EnumerateArray().Select(y => y.GetInt32()).ToArray() ?? Array.Empty<int>();

            bool hasStaticCoordinates = hasStaticXCoordinates && hasStaticYCoordinates && xArray?.Length > 0 && yArray?.Length > 0;

            bool hasDynamicCoordinates = element.TryGetProperty("DynamicCoordinates", out JsonElement dynamicCoordinatesElement) && dynamicCoordinatesElement.ValueKind != JsonValueKind.Null;
            if (hasDynamicCoordinates)
                dynamicCoordinates = dynamicCoordinatesElement.GetString();

            // Validate coordinates; one and only one must be set
            if (!(hasStaticCoordinates ^ hasDynamicCoordinates))
                throw new InvalidOperationException("exactly one of (X and Y arrays) or DynamicCoordinates must be set.");

            if (element.TryGetProperty("Category", out JsonElement categoryElement) && categoryElement.ValueKind != JsonValueKind.Null)
                category = categoryElement.GetString() ?? CATEGORY.Others.ToString();

            // Logic to determine the layer to add tiles to
            
            if (hasStaticCoordinates)
            {
                string layerName = isUserFile ? "user" : "stardew-access";
                foreach (int y in yArray!)
                {
                    foreach (int x in xArray!)
                    {
                        Vector2 position = new(x, y);
                        AccessibleTile tile = new(nameOrTranslationKey!, position, CATEGORY.FromString(category), conditions: conditions, withMods: withMods);
                        Log.Debug($"AccessibleTileManager: Adding tile {tile} to layer {layerName}.");
                        locationData.Tiles.Add(position, tile, layerName);
                    }
                }
            }
        }

        private static void ProcessCustomTileEntry(List<string> path, JsonElement element, ref Dictionary<string, List<object>> tiles)
        {
            string location = path[0];
            string NameOrTranslationKey = path[1]; // Assuming second element in path is the key
            if (!tiles.TryGetValue(location, out List<object>? entries) || entries == null)
            {
                entries = new List<object>();
                tiles[location] = entries;
            }

            int[] X = Array.Empty<int>(), Y = Array.Empty<int>();
            if (element.TryGetProperty("x", out JsonElement xElement) && xElement.ValueKind == JsonValueKind.Array)
            {
                X = xElement.EnumerateArray().Select(x => x.GetInt32()).ToArray();
            }

            if (element.TryGetProperty("y", out JsonElement yElement) && yElement.ValueKind == JsonValueKind.Array)
            {
                Y = yElement.EnumerateArray().Select(y => y.GetInt32()).ToArray();
            }

            string Category = element.TryGetProperty("type", out JsonElement typeElement) && typeElement.ValueKind != JsonValueKind.Null
                ? typeElement.GetString() ?? "Other"
                : "Other";

            var tileEntry = new { NameOrTranslationKey, X, Y, Category };
            entries.Add(tileEntry);
                    }

        internal static bool ConvertOldCustomTilesFormat()
        {
            string newLocation = Path.Combine(MainClass.ModHelper!.DirectoryPath, "assets", "TileData", "custom-tiles.json");
            string oldLocation = Path.Combine(MainClass.ModHelper!.DirectoryPath, "assets", "custom-tiles.json");
            string userFileLocation = Path.Combine(MainClass.ModHelper!.DirectoryPath, "assets", "TileData", "tiles_user.json");

            string? subdir = null;
            string? fileToProcess = null;  // Add this line
            
            // Check for file in new location
            if (File.Exists(newLocation))
            {
                subdir = "assets/TileData";
                fileToProcess = newLocation;  // Set fileToProcess
            }
            // If not found, check for file in old location
            else if (File.Exists(oldLocation))
            {
                subdir = "assets";
                fileToProcess = oldLocation;  // Set fileToProcess
            }

            if (subdir != null && fileToProcess != null)
            {
                Dictionary<string, List<object>> tiles = new();
                if (JsonLoader.TryLoadNestedJson("custom-tiles.json", ProcessCustomTileEntry, out tiles, 2, subdir))
                {
                    // Serialize the dictionary
                    string serializedTiles = JsonSerializer.Serialize(tiles, new JsonSerializerOptions { WriteIndented = true });
                    
                    // Save it to tiles_user.json in new location
                    File.WriteAllText(userFileLocation, serializedTiles);
                    
                    // Rename the processed file to custom-tiles.old.json
                    string oldFileName = Path.Combine(subdir, "custom-tiles.json");
                    string newFileName = Path.Combine(subdir, "custom-tiles.old.json");
                    File.Move(fileToProcess, Path.Combine(MainClass.ModHelper!.DirectoryPath, newFileName));
                }
                return true;
            }
            return false;
        }

        // Create a new AccessibleLocation
        public AccessibleLocation CreateLocation(GameLocation gameLocation)
        {
            // Create new AccessibleLocation and initialize with base layer "Static"
            string locationName = gameLocation.NameOrUniqueName;
            Log.Trace($"AccessibleTileManager.CreateLocation: Creating new AccessibleLocation {locationName}");
            // Check if there's corresponding data in the AccessibleLocationData dictionary
            if (LocationData.TryGetValue(locationName, out var locationData))
            {
                Log.Trace($"AccessibleTileManager: found static data for location {locationName}");
            } 
            #if DEBUG
            else
            {
                Log.Debug($"AccessibleTileManager.CreateLocation: Unable to find static data for \"{locationName}\". Keys are:\n\t{string.Join(", ", LocationData.Keys)}", true);
            }
            #endif

            AccessibleLocation location = new(gameLocation, locationData.Tiles);

            // Add it to the Locations dictionary
            Locations.Add(locationName, location);

            return location;
        }

        // Get an AccessibleLocation by name
        public AccessibleLocation? GetLocation(string? locationName)
        {
            if (string.IsNullOrEmpty(locationName))
            {
                locationName = Game1.currentLocation.NameOrUniqueName;
            }

            EnsureLocationLoaded(Game1.currentLocation);

            if (Locations.TryGetValue(locationName, out AccessibleLocation? location))
            {
                return location;
            }

            return null;
        }

        // Get an AccessibleLocation by GameLocation
        public AccessibleLocation? GetLocation(GameLocation? location = null)
        {
            location ??= Game1.currentLocation;
            return GetLocation(location!.NameOrUniqueName);
        }

        public void EnsureLocationLoaded(GameLocation gameLocation)
        {
            if (gameLocation == null) return;

            string locationName = gameLocation.NameOrUniqueName;

                        // Use negated TryGetValue to create an AccessibleLocation instance if it doesn't exist
            if (!Locations.TryGetValue(locationName, out _))
            {
                CreateLocation(gameLocation);
                Log.Trace($"AccessibleTileManager.EnsureLocationLoaded: Created AccessibleLocation for {locationName}.");
            }
        }

        public (string? nameOrTranslationKey, CATEGORY? category) GetNameAndCategoryAt(Vector2 coordinates, string? layerName = null, string? locationName = null)
        {
            locationName ??= Game1.currentLocation.NameOrUniqueName;
            return GetLocation(locationName)?.GetNameAndCategoryAt(coordinates, layerName) ?? (null, null);
        }

        public (string? nameOrTranslationKey, CATEGORY? category) GetNameAndCategoryAt(Vector2 coordinates, string? layerName = null) => GetNameAndCategoryAt(coordinates, layerName, Game1.currentLocation.NameOrUniqueName);
        public (string? nameOrTranslationKey, CATEGORY? category) GetNameAndCategoryAt(Vector2 coordinates, string? layerName = null, GameLocation? location = null) => GetNameAndCategoryAt(coordinates, layerName, location?.NameOrUniqueName);
        public (string? nameOrTranslationKey, CATEGORY? category) GetNameAndCategoryAt((int x, int y) coordinates, string? layerName = null, string? locationName = null) => GetNameAndCategoryAt(new Vector2(coordinates.x, coordinates.y), layerName, locationName);
        public (string? nameOrTranslationKey, CATEGORY? category) GetNameAndCategoryAt((int x, int y) coordinates, string? layerName = null, GameLocation? location = null) => GetNameAndCategoryAt(new Vector2(coordinates.x, coordinates.y), layerName, location?.NameOrUniqueName);

        public HashSet<AccessibleTile> GetTilesByCategory(CATEGORY category, string? layerName = null) => GetLocation()?.GetTilesByCategory(category, layerName) ?? new();
        public HashSet<AccessibleTile> GetTilesByCategory(CATEGORY category, string? layerName = null, string? locationName = null) => GetLocation(locationName)?.GetTilesByCategory(category, layerName) ?? new();
        public HashSet<AccessibleTile> GetTilesByCategory(CATEGORY category, string? layerName = null, GameLocation? location = null) => GetLocation(location)?.GetTilesByCategory(category, layerName) ?? new();

    }
}
