using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace stardew_access.Patches
{
    internal class Game1Patch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), nameof(Game1.closeTextEntry)),
                    prefix: new HarmonyMethod(typeof(Game1Patch), nameof(Game1Patch.CloseTextEntryPatch))
            );

            harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), nameof(Game1.exitActiveMenu)),
                    prefix: new HarmonyMethod(typeof(Game1Patch), nameof(Game1Patch.ExitActiveMenuPatch))
            );

            harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), nameof(Game1.playSound)),
                    prefix: new HarmonyMethod(typeof(Game1Patch), nameof(Game1Patch.PlaySoundPatch))
            );
        }

        private static void ExitActiveMenuPatch()
        {
            try
            {
                Log.Debug($"Closing {Game1.activeClickableMenu.GetType()} menu, performing cleanup...");
                IClickableMenuPatch.Cleanup(Game1.activeClickableMenu);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in exit active menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void CloseTextEntryPatch()
        {
            TextBoxPatch.activeTextBoxes = "";
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
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in play sound patch:\n{e.Message}\n{e.StackTrace}");
            }

            return true;
        }
    }
}
