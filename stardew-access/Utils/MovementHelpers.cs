using StardewValley;
using Microsoft.Xna.Framework;

namespace stardew_access.Utils
{
    public static class MovementHelpers
    {
        public static void CenterPlayer()
        {
            Game1.player.Position = Vector2.Divide(Game1.player.Position, Game1.tileSize) * Game1.tileSize;
        }

        // Add other methods related to player movement here
    }
}