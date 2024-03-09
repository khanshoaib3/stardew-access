using HarmonyLib;
using stardew_access.Utils;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Audio;

namespace stardew_access.Patches;

internal class SoundsHelperPatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
                original: AccessTools.Method(typeof(SoundsHelper), "PlayLocal"),
                prefix: new HarmonyMethod(typeof(SoundsHelperPatch), nameof(SoundsHelperPatch.PlaySoundPatch))
        );
    }

    /// <summary>
    /// Stops the footstep sounds if the player is not moving.
    /// </summary>
    private static bool PlaySoundPatch(string cueName)
    {
        try
        {
            if (!Context.IsPlayerFree)
                return true;

            if (!Game1.player.isMoving())
                return true;

            if (cueName is "grassyStep" or "sandyStep" or "snowyStep" or "stoneStep" or "thudStep" or "woodyStep"
                    && TileInfo.IsCollidingAtTile(Game1.currentLocation, (int)CurrentPlayer.FacingTile.X, (int)CurrentPlayer.FacingTile.Y))
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in play sound patch:\n{e.Message}\n{e.StackTrace}");
        }

        return true;
    }
}
