using Newtonsoft.Json.Linq;
using StardewValley;

namespace stardew_access.Features
{
    public class StaticTiles
    {
        private JObject? data;

        public StaticTiles()
        {
            using (StreamReader file = new StreamReader("static-tiles.json"))
            {
                string json = file.ReadToEnd();
                data = JObject.Parse(json);

            }
        }

        public bool isAvailable(string locationName)
        {
            if (data != null)
            {
                foreach (var location in data)
                {
                    if (locationName.ToLower().Equals(location.Key))
                        return true;
                }
            }

            return false;
        }

        public string? getStaticTileAt(int x, int y)
        {
            if (data == null)
                return null;

            foreach (var location in data)
            {
                if (!Game1.currentLocation.Name.ToLower().Equals(location.Key))
                    continue;

                if (location.Value != null)
                    foreach (var tile in ((JObject)location.Value))
                    {
                        if (tile.Value == null)
                            continue;

                        JToken? tileX = tile.Value["x"];
                        JToken? tileY = tile.Value["y"];

                        if (tileX == null || tileY == null)
                            continue;

                        if (short.Parse(tileX.ToString()) == x && short.Parse(tileY.ToString()) == y)
                            return tile.Key;
                    }
            }

            return null;
        }
    }
}