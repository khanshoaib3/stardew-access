using StardewValley;
using StardewValley.Tools;
using Microsoft.Xna.Framework;
using stardew_access.Translation;

namespace stardew_access.Utils
{
    public static class MovementHelpers
    {
        private static readonly List<Vector2>[] Stages = new List<Vector2>[]
        {
            new List<Vector2>
            { // directly adjacent
                new Vector2(0, -1), // top
                new Vector2(1, 0), // right
                new Vector2(0, 1), // bottom
                new Vector2(-1, 0), // left
            },
            new List<Vector2>
            { // diagonally adjacent
                new Vector2(-1, -1), // top left
                new Vector2(1, -1), // top right
                new Vector2(1, 1), // bottom right
                new Vector2(-1, 1), // bottom left
            }
        };

        internal static void CenterPlayer()
        {
            Game1.player.Position = Vector2.Divide(Game1.player.Position, Game1.tileSize) * Game1.tileSize;
        }

        internal static void FacePlayerToTargetTile(Vector2 targetTile)
        {
            var player = Game1.player;
            string faceDirection = GetDirectionTranslationKey(player.getTileLocation(), targetTile);
            switch (faceDirection)
            {
                case "direction-north":
                    player.faceDirection(0);
                    break;
                case "direction-east":
                    player.faceDirection(1);
                    break;
                case "direction-south":
                    player.faceDirection(2);
                    break;
                case "direction-west":
                    player.faceDirection(3);
                    break;
            }
        }

        internal static void FixCharacterMovement()
        {
            //ripped from the debug cm command
            Game1.player.isEating = false;
            Game1.player.CanMove = true;
            Game1.player.UsingTool = false;
            Game1.player.usingSlingshot = false;
            Game1.player.FarmerSprite.PauseForSingleAnimation = false;
            if (Game1.player.CurrentTool is FishingRod fishingRod)
                fishingRod.isFishing = false;
            Game1.player.mount?.dismount();
        }

        private static Vector2? GetClosestNavigableTile(List<Vector2> tiles, Vector2? tilePosition, Vector2 playerLocation)
        {
            if (tilePosition == null) return null;
            Vector2? closestTile = null;
            double? closestTileDistance = null;

            foreach (var tile in tiles)
            {
                Vector2 tileLocation = tilePosition.Value + tile;
                PathFindController controller = new(Game1.player, Game1.currentLocation, tileLocation.ToPoint(), -1, eraseOldPathController: true);

                if (controller.pathToEndPoint != null)
                {
                    int tileDistance = controller.pathToEndPoint.Count;
                    double distanceToObject = GetDistance(tileLocation, playerLocation);

                    if (closestTileDistance == null || tileDistance <= closestTileDistance && distanceToObject <= closestTileDistance)
                    {
                        closestTile = tileLocation;
                        closestTileDistance = tileDistance;
                    }
                }
            }

            return closestTile;
        }

        internal static Vector2? GetClosestTilePath(Vector2? tilePosition)
        {
            if (tilePosition == null) return null;

            Vector2 playerLocation = Game1.player.getTileLocation();

            foreach (var stage in Stages)
            {
                Vector2? closestTile = GetClosestNavigableTile(stage, tilePosition, playerLocation);
                if (closestTile != null)
                {
                    return closestTile;
                }
            }

            return null;
        }
        
        internal static string GetDirection(Vector2 start, Vector2 end)
        {
            return Translator.Instance.Translate(GetDirectionTranslationKey(start, end));
        }

        internal static string GetDirectionTranslationKey(Vector2 start, Vector2 end)
        {
            double tan_Pi_div_8 = Math.Sqrt(2.0) - 1.0;
            double dx = end.X - start.X;
            double dy = start.Y - end.Y;

            if (Math.Abs(dx) > Math.Abs(dy)) {
                if (Math.Abs(dy / dx) <= tan_Pi_div_8) {
                    return dx > 0 ? "direction-east" : "direction-west";
                } else if (dx > 0) {
                    return dy > 0 ? "direction-north_east" : "direction-south_east";
                } else {
                    return dy > 0 ? "direction-north_west" : "direction-south_west";
                }
            } else if (Math.Abs(dy) > 0) {
                if (Math.Abs(dx / dy) <= tan_Pi_div_8) {
                    return dy > 0 ? "direction-north" : "direction-south";
                } else if (dy > 0) {
                    return dx > 0 ? "direction-north_east" : "direction-north_west";
                } else {
                    return dx > 0 ? "direction-south_east" : "direction-south_west";
                }
            } else {
                return "direction-current_tile";
            }
        }

        internal static double GetDistance(Vector2? player, Vector2? point)
        {
            if (player == null)
            {
                string message = point == null ? "Both player and point must not be null." : "Player must not be null.";
                throw new ArgumentNullException(nameof(player), message);
            }
            else if (point == null)
            {
                throw new ArgumentNullException(nameof(point), "Point must not be null.");
            }

            double value = Math.Sqrt(Math.Pow((point.Value.X - player.Value.X), 2) + Math.Pow((point.Value.Y - player.Value.Y), 2));
            return Math.Round(value);
        }

        // Add other methods related to player movement here
    }
}