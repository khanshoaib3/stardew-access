using Microsoft.Xna.Framework;
using stardew_access.Utils;
using StardewValley;

namespace stardew_access.Tiles
{
    public struct AccessibleLocationData
    {
        public string[] WithMods { get; set; }
        public string[] Conditions { get; set; }
        public bool IsEvent { get; set; }
        public OverlayedDictionary<Vector2, AccessibleTile> Tiles { get; set; }

        public AccessibleLocationData()
        {
            this.WithMods = Array.Empty<string>();
            this.Conditions = Array.Empty<string>();
            this.IsEvent = false;

            this.Tiles = new OverlayedDictionary<Vector2, AccessibleTile>(
                new List<IDictionary<Vector2, AccessibleTile>>
                {
                    new Dictionary<Vector2, AccessibleTile>(),
                    new Dictionary<Vector2, AccessibleTile>()
                },
                new List<string?> { "user", "stardew-access" }
            );
        }
    }

    public class AccessibleLocation
    {
        // Name of the default layer
        private const string DefaultLayerName = "stardew-access";
        // OverlayedDictionary to map coordinate vectors to tiles
        internal readonly OverlayedDictionary<Vector2, AccessibleTile> Tiles;

        // Dictionary to map categories to HashSets of tiles
        internal readonly Dictionary<CATEGORY, Dictionary<string, HashSet<AccessibleTile>>> CategoryTileMap = new();

        // The GameLocation this instance corresponds to
        internal readonly GameLocation Location;

        public AccessibleLocation(GameLocation location, OverlayedDictionary<Vector2, AccessibleTile>? staticTiles = null)
        {
            Location = location;
            if (staticTiles != null)
            {
                Tiles = staticTiles;
                foreach (IDictionary<Vector2, AccessibleTile> layer in staticTiles.GetAllLayers())
                {
                    string layerName = staticTiles.GetLayerName(layer)!;
                    foreach (AccessibleTile tile in layer.Values)
                    {
                        AddTileToCategoryMap(tile, layerName);
                    }
                }
            } else {
                Tiles = new OverlayedDictionary<Vector2, AccessibleTile>(DefaultLayerName);
            }
            #if DEBUG
            Log.Verbose($"AccessibleLocation: initialized \"{location.NameOrUniqueName}\"");
            #endif
        }

        public void AddLayer(string layerName, IDictionary<Vector2, AccessibleTile>? newLayer = null)
        {
            Tiles.AddLayer(newLayer ?? new Dictionary<Vector2, AccessibleTile>(), layerName);
        }

        public IDictionary<Vector2, AccessibleTile>? GetLayer(string layerName)
        {
            return Tiles.GetLayer(layerName);
        }

        public void RemoveLayer(string layerName)
        {
            Tiles.RemoveLayer(layerName);
        }

        public void AddTile(AccessibleTile tile, string? layerName = null)
        {
            bool result = false;
            if (layerName != null)
            {
                result = Tiles.TryAdd(tile.Coordinates, tile, layerName);
                #if DEBUG
                Log.Verbose($"Adding tile {tile} to layer {layerName} of location {Location.NameOrUniqueName}");
                #endif
            } else {
                result = Tiles.TryAdd(tile.Coordinates, tile);
                #if DEBUG
                Log.Verbose($"Adding tile {tile} to location {Location.NameOrUniqueName}");
                #endif
            }
            if (result) AddTileToCategoryMap(tile, layerName ?? "");
        }

        private void AddTileToCategoryMap(AccessibleTile tile, string layerName)
        {
            if (this[tile.Coordinates] != tile) return;
            if (!CategoryTileMap.ContainsKey(tile.Category))
            {
                CategoryTileMap[tile.Category] = new Dictionary<string, HashSet<AccessibleTile>>(StringComparer.OrdinalIgnoreCase);
            }
            if (!CategoryTileMap[tile.Category].ContainsKey(layerName))
            {
                CategoryTileMap[tile.Category][layerName] = new HashSet<AccessibleTile>();
            }
            
            CategoryTileMap[tile.Category][layerName].Add(tile);
        }

        public void RemoveTile(AccessibleTile tile)
        {
            // Remove tile from the Tiles dictionary
            Tiles.Remove(tile.Coordinates, true);

            // Remove tile from the CategoryTileMap
            if (CategoryTileMap.ContainsKey(tile.Category))
            {
                foreach(var pair in CategoryTileMap[tile.Category])
                {
                    pair.Value.Remove(tile);
                    if (pair.Value.Count == 0)
                    {
                        CategoryTileMap[tile.Category].Remove(pair.Key);
                    }
                }
                if (CategoryTileMap[tile.Category].Count == 0)
                {
                    CategoryTileMap.Remove(tile.Category);
                }
            }
        }

        public (string? nameOrTranslationKey, CATEGORY? category) GetNameAndCategoryAt(Vector2 coordinates, string? layerName = null)
            => GetAccessibleTileAt(coordinates, layerName)?.NameAndCategory ?? (null, null)!;

        public AccessibleTile? GetAccessibleTileAt(Vector2 coordinates, string? layerName = null, bool getOnlyVisible = true)
        {
            IDictionary<Vector2, AccessibleTile> tiles = (layerName == null ? Tiles : Tiles.GetLayer(layerName!))!;
            if (tiles.TryGetValue(coordinates, out AccessibleTile? tile) 
                && (!getOnlyVisible || (getOnlyVisible && tile.Visible)))
                return tile;

            return null;
        }

        public HashSet<AccessibleTile> GetTilesByCategory(CATEGORY category, string? layerName = null)
        {
            // Create an empty HashSet to store the result
            HashSet<AccessibleTile> resultSet = new();

            // Check if the category exists in the CategoryTileMap
            if (CategoryTileMap.TryGetValue(category, out var layerDict))
            {
                if (layerName != null)
                {
                    // If layerName is specified, try to get that specific HashSet
                    if (layerDict.TryGetValue(layerName, out var tileSet))
                    {
                        return new HashSet<AccessibleTile>(tileSet.Where(tile => tile.Visible));
                    }
                }
                else
                {
                    // If layerName is null, merge all HashSets together
                    foreach (var tileSet in layerDict.Values)
                    {
                        resultSet.UnionWith(tileSet);
                    }
                    return new HashSet<AccessibleTile>(resultSet.Where(tile => tile.Visible));
                }
            }

            // If we get here, either the category or the layerName doesn't exist, so return an empty HashSet
            return resultSet;
        }

        // Indexer 1: Retrieve by Vector2
        public AccessibleTile? this[Vector2 coordinates]
        {
            get
            {
                if (Tiles.TryGetValue(coordinates, out AccessibleTile? tile) && tile != null && tile.Visible
                    )
                    return tile;
                return null;
            }
        }

        // Indexer 2: Retrieve by x and y (as separate arguments)
        public AccessibleTile? this[int x, int y]
        {
            get => this[new Vector2(x, y)];
        }

        // Indexer 3: Retrieve by (x, y) as a tuple
        public AccessibleTile? this[(int x, int y) coordinates]
        {
            get => this[new Vector2(coordinates.x, coordinates.y)];
        }

        // Indexer 4: Retrieve by category string
        public HashSet<AccessibleTile>? this[string category]
        {
            get => this[CATEGORY.FromString(category)];
        }

        // Indexer 5: Retrieve by CATEGORY
        public HashSet<AccessibleTile>? this[CATEGORY categoryInstance]
        {
            get
            {
                return GetTilesByCategory(categoryInstance);
            }
        }

    }
}
