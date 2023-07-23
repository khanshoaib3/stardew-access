using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stardew_access.Utils
{
    public class SpecialObject
    {
        public string name;
        public Vector2 TileLocation;
        public Vector2? PathfindingOverride;
        public NPC? character;
        public bool reachable = true;
        public string? unreachable_reason;

        public SpecialObject(string name, Vector2 location)
        {
            this.name = name;
            this.TileLocation = location;
        }

        public SpecialObject(string name, Vector2 location, Vector2 path_override)
        {
            this.name = name;
            this.TileLocation = location;
            this.PathfindingOverride = path_override;
        }
    }
}
