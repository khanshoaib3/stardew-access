using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ConfirmationDialogMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(ConfirmationDialog), nameof(ConfirmationDialog.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ConfirmationDialogMenuPatch), nameof(ConfirmationDialogMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(ConfirmationDialog __instance, string ___message)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                string translationKey = "";

                if (__instance.okButton.containsPoint(x, y))
                {
                    translationKey = "menu-confirmation_dialogue-ok_button";
                }
                else if (__instance.cancelButton.containsPoint(x, y))
                {
                    translationKey = "menu-confirmation_dialogue-cancel_button";
                }

                if (string.IsNullOrEmpty(translationKey))
                    MainClass.ScreenReader.SayWithMenuChecker(___message, true);
                else
                    MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, new { dialogue_message = ___message });
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in confirmation dialogue menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
