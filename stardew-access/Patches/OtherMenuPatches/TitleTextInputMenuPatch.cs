using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class TitleTextInputMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(TitleTextInputMenu), nameof(TitleTextInputMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(TitleTextInputMenuPatch), nameof(TitleTextInputMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(TitleTextInputMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (__instance.pasteButton == null || !__instance.pasteButton.containsPoint(x, y))
                    return;

                MainClass.ScreenReader.TranslateAndSayWithChecker($"menu-title_text_input-paste_button", true);
            }
            catch (System.Exception e)
            {
                Log.Error($"An error occurred in title text input menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
