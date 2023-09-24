using Microsoft.Xna.Framework;
using stardew_access.Utils;
using StardewValley;
using System.Text.Json;

namespace stardew_access.Tiles
{
    public class AccessibleTileManager
    {
        // Dictionary of location specific settings
        private readonly Dictionary<string, (string[] withMods, string[] conditions, bool isEvent)> locationSettings = new();

        // Dictionary to map location names to Accessiblelocations
        private Dictionary<string, AccessibleLocation> Locations { get; set; } = new();

        // Dictionary to hold parsed and validated json data for static tiles
        private readonly Dictionary<string, List<(string? nameOrTranslationKey, string? dynamicNameOrTranslationKey, int[] xArray, int[] yArray, string category, string[] withMods, string[] conditions, bool isEvent)>> staticTileData = new();

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
            Initialize();
        }

        private void Initialize()
        {
            Log.Trace("Initializing     AccessibleTileManager");
            if (JsonLoader.TryLoadNestedJson(
                "tiles.json", 
                TileDataProcessor, 
                out Dictionary<string, AccessibleLocation> tileData, 
                2, 
                "assets/TileData"))
            {
                // Assign the loaded data to the class-level variable
                Locations = tileData;
                Log.Info("Successfully initialized tile data.");
            }
            else
            {
                // Log an error if the load fails
                Log.Error("Failed to initialize tile data.");
            }
        }

        private void TileDataProcessor(List<string> path, JsonElement element, ref Dictionary<string, AccessibleLocation> result)
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
                    TileDataProcessor(newPath, element, ref result);
                }
                return;
            }

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
                // Add settings to the locationSettings dict if no entry exists
                if (!locationSettings.TryAdd(locationName, (withMods, conditions, isEvent)))
                {
                    // Settings already exist; merge new settings
                    var (existingWithMods, existingConditions, _) = locationSettings[locationName];
                    // Append the new array to the old one, excluding duplicate entries
                    var updatedWithMods = existingWithMods.Concat(withMods.Except(existingWithMods)).ToArray();
                    var updatedConditions = existingConditions.Concat(conditions.Except(existingConditions)).ToArray();
                    // Just use new isEvent value; it's either the same or it isn't.
                    locationSettings[locationName] = (updatedWithMods, updatedConditions, isEvent);
                }
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
                category = categoryElement.GetString() ?? "Other";

            // Logic to determine the layer to add tiles to
            
            if (hasStaticCoordinates)
            {
                if (!staticTileData.ContainsKey(locationName))
                {
                    staticTileData[locationName] = new List<(string? nameOrTranslationKey, string? dynamicNameOrTranslationKey, int[] xArray, int[] yArray, string category, string[] withMods, string[] conditions, bool isEvent)>();
                }
                (string? nameOrTranslationKey, string? dynamicNameOrTranslationKey, int[] xArray, int[] yArray, string category, string[] withMods, string[] conditions, bool isEvent) staticData = (nameOrTranslationKey, dynamicNameOrTranslationKey, xArray!, yArray!, category, withMods, conditions, isEvent);
                staticTileData[locationName].Add(staticData);
            }
        }

        // Create a new AccessibleLocation
        public AccessibleLocation CreateLocation(GameLocation gameLocation)
        {
            // Create new AccessibleLocation and initialize with base layer "Static"
            AccessibleLocation location = new(gameLocation);

            string locationName = gameLocation.NameOrUniqueName;

            // Add it to the Locations dictionary
            Locations.Add(locationName, location);

            // Check if there's corresponding data in the staticTileData dictionary
            if (staticTileData.TryGetValue(locationName, out List<(string? NameOrTranslationKey, string? dynamicNameOrTranslationKey, int[] XArray, int[] YArray, string Category, string[] WithMods, string[] Conditions, bool IsEvent)>? tileDataList))
            {
                location.LoadStaticTiles(tileDataList);
            }

            return location;
        }

        // Get an AccessibleLocation by name
        public AccessibleLocation? GetLocation(string locationName)
        {
            if (string.IsNullOrEmpty(locationName))
            {
                throw new ArgumentException("Location name cannot be null or empty", nameof(locationName));
            }

            EnsureLocationLoaded(Game1.currentLocation);

            if (Locations.TryGetValue(locationName, out AccessibleLocation? location))
            {
                return location;
            }

            return null;
        }

        public void EnsureLocationLoaded(GameLocation gameLocation)
        {
            if (gameLocation == null) return;

            string locationName = gameLocation.NameOrUniqueName;

                        // Use negated TryGetValue to create an AccessibleLocation instance if it doesn't exist
            if (!Locations.TryGetValue(locationName, out _))
            {
                CreateLocation(gameLocation);
                Log.Trace($"Created AccessibleLocation for {locationName}.");
            }
        }

        // Any other methods for refreshing data, querying across locations, etc.
    }
}
