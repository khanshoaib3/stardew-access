using Microsoft.Xna.Framework;
using stardew_access.Features;
using StardewModdingAPI;
using StardewValley;

namespace stardew_access.Patches
{
    internal class Game1Patch
    {
        private static Vector2? prevTile = null;

        internal static void ExitActiveMenuPatch()
        {
            try
            {
                IClickableMenuPatch.Cleanup(Game1.activeClickableMenu);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static bool PlaySoundPatch(string cueName)
        {
            try
            {
                if (!Context.IsPlayerFree)
                    return true;

                if (!Game1.player.isMoving())
                    return true;

                if (cueName == "grassyStep" || cueName == "sandyStep" || cueName == "snowyStep" || cueName == "stoneStep" || cueName == "thudStep" || cueName == "woodyStep")
                {
                    Vector2 nextTile = CurrentPlayer.FacingTile;
                    if (TileInfo.IsCollidingAtTile(Game1.currentLocation, (int)nextTile.X, (int)nextTile.Y))
                    {
                        if (prevTile != nextTile)
                        {
                            prevTile = nextTile;
                            //Game1.playSound("colliding");
                        }
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }

            return true;
        }
    }
}
