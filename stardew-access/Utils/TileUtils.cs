using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace stardew_access.Utils
{
    public static class TileUtils
    {
        private static readonly Dictionary<Map, TileMapData> SortedTiles = new();
        private static readonly Dictionary<TileArray, Layer> layersByArray = new();
        public static readonly Dictionary<Map, string> MapNames = new();

        /// <summary>
        /// Updates tile information in the SortedTiles dictionary.
        /// </summary>
        /// <param name="tileCoords">The x, y coordinates of the tile.</param>
        /// <param name="value">The tile information.</param>
        /// <param name="__instance">The TileArray instance containing the tile.</param>
        internal static void UpdateTile((int x, int y) tileCoords, Tile value, TileArray __instance)
        {
            #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            if (!layersByArray.TryGetValue(__instance, out Layer layer))
            #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            {
                layer = MainClass.ModHelper!.Reflection.GetField<Layer>(__instance, "m_layer").GetValue() ?? throw new Exception("Failed to get layer instance");
                layersByArray[__instance] = layer;
            }

            #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            if (!SortedTiles.TryGetValue(layer.Map, out TileMapData tileMapData))
            #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            {
                tileMapData = new TileMapData();
                SortedTiles[layer.Map] = tileMapData;
            }

            tileMapData.UpdateTile(tileCoords, value, layer.Id);
        }

        internal static void CleanupMaps(params GameLocation[] locations)
        {
            // Step 1: Create a set from the keys in SortedTiles dictionary
            HashSet<Map> mapsToRemove = new(SortedTiles.Keys);

            // Step 2: Create a set from the .map properties of GameLocation instances, active mines, buildings, and the provided locations
            HashSet<Map> validMaps = new(
                Game1.locations
                .Concat(MineShaft.activeMines)
                .Concat(Game1.getFarm().buildings
                    .Where(building => building != null /*&& building.indoors != null*/ && building.indoors.Value != null)
                    .Select(building => building.indoors.Value))
                .Concat(locations)
                    .Select(location => location.map)
            );


            // Step 3: Perform set difference to get the maps to be removed
            mapsToRemove.ExceptWith(validMaps);

            // Step 4: Remove those map instances from the SortedTiles dictionary
            Log.Trace($"TileUtils.CleanupMaps: Removing {mapsToRemove.Count}stale maps.");
            foreach (Map map in mapsToRemove)
            {
                SortedTiles.Remove(map);
                if (MapNames.ContainsKey(map))
                {
                    #if DEBUG
                    Log.Verbose($"Removed map {map}, \"{MapNames[map]}\"");
                    #endif
                    MapNames.Remove(map);
                }
            }
        }

        /// <summary>
        /// Finds the GameLocation associated with a given map instance.
        /// </summary>
        /// <param name="mapInstance">The Map instance.</param>
        /// <returns>The GameLocation that holds the map, or null if not found.</returns>
        public static GameLocation? GetLocationByMap(Map mapInstance)
        {
            return Game1.locations.FirstOrDefault(location => location.map == mapInstance);
        }

        /// <summary>
        /// Retrieves the layers and tile indexes for the specified coordinates.
        /// </summary>
        /// <param name="coords">The x, y coordinates of the tile.</param>
        /// <param name="location">The GameLocation, defaults to current location if null.</param>
        /// <returns>A Dictionary containing layer IDs and their respective tile indexes.</returns>
        public static Dictionary<string, int>? GetTileLayers((int x, int y) coords, GameLocation? location = null)
        {
            GameLocation currentLocation = location ?? Game1.currentLocation;

            // Try to get the TileMapData corresponding to the current location's map
            if (!SortedTiles.TryGetValue(currentLocation.map, out var tileMapData))
            {
                return default;
            }

            // Try to get the layer dictionary for the given coordinates
            if (!tileMapData.TryGetValue(coords, out var layerDict))
            {
                return default;
            }

            return new Dictionary<string, int>(layerDict!);
        }

        /// <summary>
        /// Retrieves the tile index for a specific layer at the specified coordinates.
        /// </summary>
        /// <param name="coords">The x, y coordinates of the tile.</param>
        /// <param name="layerName">The name of the layer.</param>
        /// <param name="location">The GameLocation, defaults to current location if null.</param>
        /// <returns>The tile index if it exists, null otherwise.</returns>
        public static int? GetTileIndexForLayer((int x, int y) coords, string layerName, GameLocation? location = null)
        {
            GameLocation currentLocation = location ?? Game1.currentLocation;

            if (!SortedTiles.TryGetValue(currentLocation.map, out var tileMapData))
            {
                return null;
            }

            if (tileMapData.TryGetValue(coords, layerName, out int? tileIndex))
            {
                return tileIndex;
            }

            return null;
        }

        /// <summary>
        /// Retrieves all tiles in a specified layer, sorted by their tile indexes.
        /// </summary>
        /// <param name="layerName">The name of the layer.</param>
        /// <param name="location">The GameLocation, defaults to current location if null.</param>
        /// <returns>A Dictionary of tile indexes and their coordinates.</returns>
        public static Dictionary<int, HashSet<(int x, int y)>>? GetTilesByLayer(string layerName, GameLocation? location = null)
        {
            GameLocation currentLocation = location ?? Game1.currentLocation;

            if (!SortedTiles.TryGetValue(currentLocation.map, out var tileMapData))
            {
                return null;
            }

            if (tileMapData.TryGetValue(layerName, out var result))
            {
                return new Dictionary<int, HashSet<(int x, int y)>>(result!);
            }

            return null;
        }

        /// <summary>
        /// Retrieves all tile coordinates in a specified layer for a specific tile index.
        /// </summary>
        /// <param name="layerName">The name of the layer.</param>
        /// <param name="tileIndex">The tile index.</param>
        /// <param name="location">The GameLocation, defaults to current location if null.</param>
        /// <returns>A HashSet containing the coordinates of all matching tiles.</returns>
        public static HashSet<(int x, int y)>? GetTilesByLayerAndIndex(string layerName, int tileIndex, GameLocation? location = null)
        {
            GameLocation currentLocation = location ?? Game1.currentLocation;

            if (!SortedTiles.TryGetValue(currentLocation.map, out var tileMapData))
            {
                return null;
            }

            if (tileMapData.TryGetValue(layerName, tileIndex, out var result))
            {
                return new HashSet<(int x, int y)>(result!);
            }

            return null;
        }

        /// <summary>
        /// Represents the tile data for a given map, optimizing for quick lookups by layer and index, as well as by tile coordinates.
        /// </summary>
        private class TileMapData
        {
            /// <summary>
            /// Dictionary that stores tile data indexed first by layer name, then by tile index.
            /// Each tile index maps to a HashSet of tile coordinates where that tile index appears in the corresponding layer.
            /// </summary>
            private readonly Dictionary<string, Dictionary<int, HashSet<(int x, int y)>>> byLayerAndIndex;

            /// <summary>
            /// Dictionary that stores tile data indexed by tile coordinates (x, y).
            /// Each tile coordinate maps to a Dictionary, which further maps layer names to tile indexes for that coordinate.
            /// </summary>
            private readonly Dictionary<(int x, int y), Dictionary<string, int>> byTileCoords;

            /// <summary>
            /// Initializes a new instance of the <see cref="TileMapData"/> class.
            /// </summary>
            public TileMapData()
            {
                this.byLayerAndIndex = new Dictionary<string, Dictionary<int, HashSet<(int x, int y)>>>();
                this.byTileCoords = new Dictionary<(int x, int y), Dictionary<string, int>>();
            }

            /// <summary>
            /// Updates the tile data for a given tile, storing the tile index and coordinates in optimized dictionaries.
            /// </summary>
            /// <param name="tileCoords">A tuple containing the X and Y coordinates of the tile.</param>
            /// <param name="value">The tile object which contains the new tile index.</param>
            /// <param name="layer">The layer name where the tile exists.</param>
            internal void UpdateTile((int x, int y) tileCoords, Tile value, string layer)
            {
                if (value == null) return;
                // Try to get the dictionary of layers for the given tile coordinates.
                // Create a new dictionary and add it if it doesn't exist.
                if (!byTileCoords.TryGetValue(tileCoords, out var layerDict))
                {
                    layerDict = new Dictionary<string, int>();
                    byTileCoords[tileCoords] = layerDict;
                }

                // Try to get the old tile index for the given layer at the given tile coordinates.
                // Remove the tile coordinates from the hash set for the old tile index if it exists.
                if (layerDict.TryGetValue(layer, out var oldIndex))
                {
                    byLayerAndIndex[layer][oldIndex].Remove(tileCoords);
                }

                // Update the tile index for the given layer at the given tile coordinates.
                int newIndex = value.TileIndex;
                layerDict[layer] = newIndex;

                // Try to get the dictionary of tile indexes for the given layer.
                // Create a new dictionary and add it if it doesn't exist.
                if (!byLayerAndIndex.TryGetValue(layer, out var tileIndexDict))
                {
                    tileIndexDict = new Dictionary<int, HashSet<(int x, int y)>>();
                    byLayerAndIndex[layer] = tileIndexDict;
                }

                // Try to get the hash set for the new tile index.
                // Create a new hash set and add it if it doesn't exist.
                if (!tileIndexDict.TryGetValue(newIndex, out var tileSet))
                {
                    tileSet = new HashSet<(int x, int y)>();
                    tileIndexDict[newIndex] = tileSet;
                }

                // Add the new tile coordinates to the hash set for the new tile index.
                tileSet.Add(tileCoords);
            }

            /// <summary>
            /// Tries to get the dictionary of layers and tile indexes for a given tile coordinate.
            /// </summary>
            /// <param name="coords">The (X, Y) coordinates of the tile.</param>
            /// <param name="result">The output dictionary containing layers and their respective tile indexes.</param>
            /// <returns>True if the dictionary exists, false otherwise.</returns>
            public bool TryGetValue((int x, int y) coords, out Dictionary<string, int>? result) => byTileCoords.TryGetValue(coords, out result);

            /// <summary>
            /// Tries to get the tile index for a specific layer at a given tile coordinate.
            /// </summary>
            /// <param name="coords">The (X, Y) coordinates of the tile.</param>
            /// <param name="layerName">The name of the layer.</param>
            /// <param name="tileIndex">The output tile index.</param>
            /// <returns>True if the tile index exists, false otherwise.</returns>
            public bool TryGetValue((int x, int y) coords, string layerName, out int? tileIndex)
            {
                if (TryGetValue(coords, out var layerDict))
                {
                    if (layerDict!.TryGetValue(layerName, out int tmpTileIndex))
                    {
                        tileIndex = tmpTileIndex;
                        return true;
                    }
                }
                tileIndex = null;
                return false;
            }

            /// <summary>
            /// Tries to get the dictionary of tile indexes and their coordinates for a given layer.
            /// </summary>
            /// <param name="layer">The name of the layer.</param>
            /// <param name="result">The output dictionary containing tile indexes and their coordinates.</param>
            /// <returns>True if the dictionary exists, false otherwise.</returns>
            public bool TryGetValue(string layer, out Dictionary<int, HashSet<(int x, int y)>>? result) => byLayerAndIndex.TryGetValue(layer, out result);

            /// <summary>
            /// Tries to get the HashSet of coordinates for a specific tile index in a given layer.
            /// </summary>
            /// <param name="layer">The name of the layer.</param>
            /// <param name="tileIndex">The tile index.</param>
            /// <param name="result">The output HashSet containing the coordinates.</param>
            /// <returns>True if the HashSet exists, false otherwise.</returns>
            public bool TryGetValue(string layer, int tileIndex, out HashSet<(int x, int y)>? result)
            {
                if (TryGetValue(layer, out var tileIndexDict))
                {
                    return tileIndexDict!.TryGetValue(tileIndex, out result);
                }
                result = null; ;
                return false;
            }
        }
    }
}