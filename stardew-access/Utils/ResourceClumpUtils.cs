using System.Collections.Generic;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace stardew_access.Utils
{
    public static class ResourceClumpUtils
    {
        public static Dictionary<(int x, int y), ResourceClump>? GetResourceClumpsAtLocation(GameLocation? location = null)
        {
            // If location is null, set it to the current location
            location ??= Game1.currentLocation;

            // Create a dictionary to hold the resource clumps keyed by their coordinates
            var resourceClumpsByCoordinate = new Dictionary<(int x, int y), ResourceClump>();

            // Add all the regular resource clumps
            foreach (var resourceClump in location.resourceClumps)
            {
                AddResourceClump(resourceClumpsByCoordinate, resourceClump);
            }

            // If the location is Woods, add the stumps too since they are also ResourceClump instances
            if (location is Woods woods)
            {
                foreach (var stump in woods.stumps)
                {
                    AddResourceClump(resourceClumpsByCoordinate, stump);
                }
            }

            return resourceClumpsByCoordinate;
        }

		private static void AddResourceClump(Dictionary<(int x, int y), ResourceClump> dict, ResourceClump resourceClump)
		{
			// Loop over all tiles occupied by this resource clump
			for (int x = (int)resourceClump.tile.X; x < resourceClump.tile.X + resourceClump.width.Value; x++)
			{
				for (int y = (int)resourceClump.tile.Y; y < resourceClump.tile.Y + resourceClump.height.Value; y++)
				{
					// Use TryAdd and log if there's already an entry at this position
					if (!dict.TryAdd((x, y), resourceClump))
					{
						// Log the error or handle it in some way
						Log.Warn($"Warning: Overlapping resource clumps detected at ({x}, {y})");
					}
				}
			}
		}
    }
}