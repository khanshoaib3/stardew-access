using Microsoft.Xna.Framework;
using stardew_access.Utils;
using System.Text.Json;

namespace stardew_access.Tiles
{
    public class AccessibleTileManager
    {
        // Dictionary to map location names to AccessibleLocations
        private Dictionary<string, AccessibleLocation> LocationMap { get; set; } = new();

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
                LocationMap = tileData;
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
            string locationName = path[0]; // The location name should be at index 0
            #if DEBUG
            string arrayIndex = path[1]; // The array index as a string should be at index 1
            Log.Verbose($"Attempting to add tile at index {arrayIndex} to location {locationName}");
            #endif

            if (!result.ContainsKey(locationName))
            {
                // Create a new AccessibleLocation if it doesn't already exist for this location
                #if DEBUG
                Log.Verbose($"AccessibleTileManager: creating new AccessibleLocation instance \"{locationName}\"");
                #endif
                result[locationName] = new AccessibleLocation();
            }

            // Get the AccessibleLocation instance for this location
            AccessibleLocation location = result[locationName];

            // Parse individual elements from JSON
            string? nameOrTranslationKey = element.GetProperty("NameOrTranslationKey").GetString();
            JsonElement? xElement = element.GetProperty("X");
            JsonElement? yElement = element.GetProperty("Y");
            string category = element.GetProperty("Category").GetString() ?? "Other";

            // Convert the JSON arrays to int arrays
            int[] xArray = xElement != null ? xElement.Value.EnumerateArray().Select(x => x.GetInt32()).ToArray() : Array.Empty<int>();
            int[] yArray = yElement != null ? yElement.Value.EnumerateArray().Select(y => y.GetInt32()).ToArray() : Array.Empty<int>();

            // Create AccessibleTile instances for each combination of X and Y coordinates
            foreach (int y in yArray)
            {
                foreach (int x in xArray)
                {
                    AccessibleTile tile = new(nameOrTranslationKey, new Vector2(x, y), CATEGORY.FromString(category));
                    location.AddTile(tile);
                }
            }
        }

        // Method to get an AccessibleLocation by name
        public AccessibleLocation? GetLocation(string locationName)
        {
            if (LocationMap.TryGetValue(locationName, out AccessibleLocation? location))
            {
                return location;
            }
            return null;
        }

        // Any other methods for refreshing data, querying across locations, etc.
    }
}
