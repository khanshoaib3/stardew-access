using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ChooseFromListMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(ChooseFromListMenu), nameof(ChooseFromListMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ChooseFromListMenuPatch), nameof(ChooseFromListMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(ChooseFromListMenu __instance, List<string> ___options, int ___index, bool ___isJukebox)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string translationKey = "";
                object? translationTokens = null;

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    translationTokens = new
                    { 
                        option = ___isJukebox
                        ? Utility.getSongTitleFromCueName(___options[___index])
                        : ___options[___index] 
                    };
                    translationKey = "menu-choose_from_list-ok_button";
                }
                else if (__instance.cancelButton != null && __instance.cancelButton.containsPoint(x, y))
                {
                    translationKey = "common-ui-cancel_button";
                }
                else if (__instance.backButton != null && __instance.backButton.containsPoint(x, y))
                {
                    translationTokens = new
                    {
                        option = ___isJukebox
                        ? Utility.getSongTitleFromCueName(___options[Math.Max(0, ___index - 1)])
                        : ___options[Math.Max(0, ___index - 1)]
                    };
                    translationKey = "menu-choose_from_list-previous_button";
                }
                else if (__instance.forwardButton != null && __instance.forwardButton.containsPoint(x, y))
                {
                    translationTokens = new
                    {
                        option = ___isJukebox
                        ? Utility.getSongTitleFromCueName(___options[Math.Min(___options.Count, ___index + 1)])
                        : ___options[Math.Min(___options.Count, ___index + 1)]
                    };
                    translationKey = "menu-choose_from_list-next_button";
                }

                MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
            }
            catch (System.Exception e)
            {
                Log.Error($"An error occurred in choose from list menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
