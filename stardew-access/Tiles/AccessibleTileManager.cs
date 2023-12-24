using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using stardew_access.Utils;
using StardewValley;

namespace stardew_access.Tiles;

public class AccessibleTileManager
{
    // The loaded json data
    private JObject? tilesJson;
    private JObject? userTilesJson;

    private const string TileDataPath = "assets/TileData";
    private const string TileFileName = "tiles.json";
    private const string UserTileFileName = "tiles_user.json";

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
        LoadTileData();
    }

    public void LoadTileData()
    {
        Locations.Clear();
        // Load tile data
        if (!JsonLoader.TryLoadJsonFile(TileFileName, out JToken? tileJson, TileDataPath) || tileJson == null)
        {
            throw new InvalidOperationException("Tile data file not found or invalid.");
        }
        tilesJson = (tileJson! as JObject)!;

        // Load user tile data
        if (JsonLoader.TryLoadJsonFile(UserTileFileName, out JToken? userJson, TileDataPath) && userJson != null)
        {
            userTilesJson = (userJson! as JObject)!;
        }
        else
        {
            Log.Warn("User tile data file not found or invalid. Loading with default tiles only.");
            userTilesJson = []; // Create an empty JObject if user file is not found
        }
    }

    private static void ProcessCustomTileEntry(List<string> path, JToken token, ref Dictionary<string, List<object>> tiles)
    {
        string location = path[0];
        string NameOrTranslationKey = path[1]; // Assuming second element in path is the key

        if (!tiles.TryGetValue(location, out List<object>? entries) || entries == null)
        {
            entries = [];
            tiles[location] = entries;
        }

        var xArray = token["x"]?.ToObject<int[]>() ?? [];
        var yArray = token["y"]?.ToObject<int[]>() ?? [];
        string category = token["type"]?.ToObject<string>() ?? "Other";

        var tileEntry = new { NameOrTranslationKey, X = xArray, Y = yArray, Category = category };
        entries.Add(tileEntry);
    }

    internal static bool ConvertOldCustomTilesFormat()
    {
        string newLocation = Path.Combine(MainClass.ModHelper!.DirectoryPath, "assets", "TileData", "custom-tiles.json");
        string oldLocation = Path.Combine(MainClass.ModHelper!.DirectoryPath, "assets", "custom-tiles.json");
        string userFileLocation = Path.Combine(MainClass.ModHelper!.DirectoryPath, "assets", "TileData", "tiles_user.json");

        string? subdir = null;
        string? fileToProcess = null;

        // Check for file in new location
        if (File.Exists(newLocation))
        {
            subdir = "assets/TileData";
            fileToProcess = newLocation;
        }
        // If not found, check for file in old location
        else if (File.Exists(oldLocation))
        {
            subdir = "assets";
            fileToProcess = oldLocation;
        }

        if (subdir != null && fileToProcess != null)
        {
            Dictionary<string, List<object>> tiles = [];
            if (JsonLoader.TryLoadNestedJson("custom-tiles.json", ProcessCustomTileEntry, ref tiles, 2, subdir))
            {
                // Serialize the dictionary
                string serializedTiles = JsonConvert.SerializeObject(tiles, Formatting.Indented);
                
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
        string locationName = gameLocation.NameOrUniqueName;
        Log.Trace($"AccessibleTileManager.CreateLocation: Creating new AccessibleLocation {locationName}");

        JArray? jsonData = null;
        JArray? userJsonData = null;

        // Check if location data exists in the JSON objects
        if (tilesJson != null && tilesJson.TryGetValue(locationName, out JToken? locationJsonToken) && locationJsonToken is JArray array)
        {
            jsonData = array;
        }

        if (userTilesJson != null && userTilesJson.TryGetValue(locationName, out JToken? userLocationJsonToken) && userLocationJsonToken is JArray array1)
        {
            userJsonData = array1;
        }

        AccessibleLocation location;

        try
        {
            location = AccessibleLocation.FromJArray(gameLocation, jsonData!, userJsonData!);
        }
        catch (ArgumentNullException)
        {
            // Both jsonData and userJsonData are null
            location = new AccessibleLocation(gameLocation); // Creating an AccessibleLocation with empty tile dictionary
        }

        // Add the location to the Locations dictionary
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

    public (string nameOrTranslationKey, CATEGORY category)? GetNameAndCategoryAt(Vector2 coordinates, string? layerName = null, string? locationName = null)
    {
        locationName ??= Game1.currentLocation.NameOrUniqueName;
        return GetLocation(locationName)?.GetNameAndCategoryAt(coordinates, layerName);
    }

    public (string nameOrTranslationKey, CATEGORY category)? GetNameAndCategoryAt(Vector2 coordinates, string? layerName = null) => GetNameAndCategoryAt(coordinates, layerName, Game1.currentLocation.NameOrUniqueName);
    public (string nameOrTranslationKey, CATEGORY category)? GetNameAndCategoryAt(Vector2 coordinates, string? layerName = null, GameLocation? location = null) => GetNameAndCategoryAt(coordinates, layerName, location?.NameOrUniqueName);
    public (string nameOrTranslationKey, CATEGORY category)? GetNameAndCategoryAt((int x, int y) coordinates, string? layerName = null, string? locationName = null) => GetNameAndCategoryAt(new Vector2(coordinates.x, coordinates.y), layerName, locationName);
    public (string nameOrTranslationKey, CATEGORY category)? GetNameAndCategoryAt((int x, int y) coordinates, string? layerName = null, GameLocation? location = null) => GetNameAndCategoryAt(new Vector2(coordinates.x, coordinates.y), layerName, location?.NameOrUniqueName);

    public HashSet<AccessibleTile> GetTilesByCategory(CATEGORY category, string? layerName = null) => GetLocation()?.GetTilesByCategory(category, layerName) ?? [];
    public HashSet<AccessibleTile> GetTilesByCategory(CATEGORY category, string? layerName = null, string? locationName = null) => GetLocation(locationName)?.GetTilesByCategory(category, layerName) ?? [];
    public HashSet<AccessibleTile> GetTilesByCategory(CATEGORY category, string? layerName = null, GameLocation? location = null) => GetLocation(location)?.GetTilesByCategory(category, layerName) ?? [];

}