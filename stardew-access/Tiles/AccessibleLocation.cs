using Microsoft.Xna.Framework;
using stardew_access.Utils;
using System.Collections.Generic;

namespace stardew_access.Tiles
{
    public class AccessibleLocation
    {
        // Dictionary to map coordinate vectors to tiles
        public Dictionary<Vector2, AccessibleTile> Tiles { get; set; } = new();

        // Dictionary to map categories to HashSets of tiles
        public Dictionary<string, HashSet<AccessibleTile>> CategoryTileMap { get; set; } = new();

        public void AddTile(AccessibleTile tile)
        {
            if (Tiles.TryAdd(tile.Coordinates, tile))
            {
                if (!CategoryTileMap.ContainsKey(tile.Category.ToString()))
                {
                    CategoryTileMap[tile.Category.ToString()] = new HashSet<AccessibleTile>();
                }
                CategoryTileMap[tile.Category.ToString()].Add(tile);
            }
        }

        public void RemoveTile(AccessibleTile tile)
        {
            // Remove tile from the Tiles dictionary
            Tiles.Remove(tile.Coordinates);

            // Remove tile from the CategoryTileMap
            if (CategoryTileMap.ContainsKey(tile.Category.ToString()))
            {
                CategoryTileMap[tile.Category.ToString()].Remove(tile);
                if (CategoryTileMap[tile.Category.ToString()].Count == 0)
                {
                    CategoryTileMap.Remove(tile.Category.ToString());
                }
            }
        }

        // Indexer 1: Retrieve by Vector2
        public AccessibleTile? this[Vector2 coordinates]
        {
            get
            {
                Tiles.TryGetValue(coordinates, out AccessibleTile? tile);
                return tile;
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
            get
            {
                if (CategoryTileMap.TryGetValue(category, out HashSet<AccessibleTile>? tiles))
                {
                    return tiles!;
                }
                return null;
            }
        }

        // Indexer 5: Retrieve by CATEGORY
        public HashSet<AccessibleTile>? this[CATEGORY categoryInstance]
        {
            get => this[categoryInstance.ToString()];
        }

    }
}
