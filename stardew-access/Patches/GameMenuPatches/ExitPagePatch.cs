using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ExitPagePatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(ExitPage), nameof(ExitPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ExitPagePatch), nameof(ExitPagePatch.DrawPatch))
            );
        }

        private static void DrawPatch(ExitPage __instance)
        {
            try
            {
                if (__instance.exitToTitle.visible &&
                        __instance.exitToTitle.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    MainClass.ScreenReader.SayWithMenuChecker(Translator.Instance.Translate("menu-exit_page-exit_to_title_button"), true);
                    return;
                }
                if (__instance.exitToDesktop.visible &&
                    __instance.exitToDesktop.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    MainClass.ScreenReader.SayWithMenuChecker(Translator.Instance.Translate("menu-exit_page-exit_to_desktop_button"), true);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in exit page patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
