using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using stardew_access.Tiles;
using StardewValley;

namespace stardew_access.Utils;

public class UserTilesUtils
{
    public static bool TryAndGetTileDataAt(out AccessibleTile.JsonSerializerFormat? tileData, int x, int y, GameLocation? location = null)
    {
        location ??= Game1.currentLocation;

        tileData = AccessibleTileManager.Instance.GetLocation(location)?.GetAccessibleTileAt(new Vector2(x, y), "user", false)
            ?.SerializableFormat;

        return tileData != null;
    }

    public static void AddTileData(AccessibleTile.JsonSerializerFormat jsonDataForTile)
    {
        JObject root;
        if (File.Exists(JsonLoader.GetFilePath("tiles_user.json", "assets/TileData")))
        {
            Log.Trace("Loading existing tiles_user.json");
            string fileContent = File.ReadAllText(JsonLoader.GetFilePath("tiles_user.json", "assets/TileData"));
            try
            {
                root = JObject.Parse(fileContent);
            }
            catch (JsonReaderException)
            {
                MainClass.ScreenReader.Say("Unable to parse tiles_user.json", true);
                return;
            }
        }
        else
        {
            Log.Trace("tiles_user.json not found, creating a new one...");
            root = [];
        }

        if (!root.TryGetValue(Game1.currentLocation.NameOrUniqueName, out JToken? locationToken))
        {
            // Creates the location property if not exists
            Log.Trace($"Entry for location {Game1.currentLocation.NameOrUniqueName} not found, adding one...");
            locationToken = new JProperty(Game1.currentLocation.NameOrUniqueName, new JArray());
            root.Add(locationToken);
        }

        JArray locationValueArray = locationToken.Type == JTokenType.Property
            ? (JArray)locationToken.Value<JProperty>()!.Value
            : locationToken.Value<JArray>()!;

        locationValueArray.Add(JObject.FromObject(jsonDataForTile));

        JsonLoader.SaveJsonFile("tiles_user.json", root,
            "assets/TileData");
    }

    public static void RemoveTileDataAt(int x, int y, string locationName)
    {
        JObject root;
        Log.Trace("Loading tiles_user.json");
        string fileContent = File.ReadAllText(JsonLoader.GetFilePath("tiles_user.json", "assets/TileData"));
        try
        {
            root = JObject.Parse(fileContent);
        }
        catch (JsonReaderException)
        {
            MainClass.ScreenReader.Say("Unable to parse tiles_user.json", true);
            return;
        }

        if (!root.TryGetValue(Game1.currentLocation.NameOrUniqueName, out JToken? locationToken))
        {
            // Creates the location property if not exists
            Log.Trace($"Cannot find location data with name: {locationName}");
            return;
        }

        if (locationToken.Type == JTokenType.Property)
        {
            Log.Trace($"Location with name {locationName} has no tile data.");
            return;
        }

        JArray locationValueArray = locationToken.Value<JArray>()!;
        JToken? toRemove = null;
        foreach (var tile in locationValueArray)
        {
            bool hasX = false, hasY = false;
            foreach (var val in ((JObject)tile).GetValue("X")!.Value<JArray>()!)
            {
                if (val.Value<int>() != x) continue;
                hasX = true;
                break;
            }
            foreach (var val in ((JObject)tile).GetValue("Y")!.Value<JArray>()!)
            {
                if (val.Value<int>() != y) continue;
                hasY = true;
                break;
            }

            if (!hasX || !hasY) continue;

            Log.Trace($"Found tile data at {x}x {y}y, removing...");
            toRemove = tile;
        }

        if (toRemove == null)
        {
            Log.Trace($"Tile data at {x}x {y}y not found!");
            return;
        }

        locationValueArray.Remove(toRemove);
        
        JsonLoader.SaveJsonFile("tiles_user.json", root,
            "assets/TileData");
    }
}