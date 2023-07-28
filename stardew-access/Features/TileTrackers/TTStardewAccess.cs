using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stardew_access.Features.Tracker
{
    internal class TTStardewAccess : TileTrackerBase
    {

        private readonly string[] ignored_categories = { "animal" };

        public TTStardewAccess(object? arg = null) : base(arg) {
            
        }

        public override void FindObjects(object? arg) {

            Dictionary<Vector2, (string name, string category)> scannedTiles = Radar.SearchLocation();

            /* Categorise the scanned tiles into groups
             *
             * This method uses breadth first search so the first item is the closest item, no need to reorder or check for closest item
             */
            foreach (var tile in scannedTiles) {

                string category = tile.Value.category;

                if (ignored_categories.Contains(category)) continue;

                AddFocusableObject(tile.Value.category, tile.Value.name, tile.Key);
            }

            base.FindObjects(arg);
        }

    }
}
