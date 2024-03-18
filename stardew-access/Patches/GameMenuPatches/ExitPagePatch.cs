using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class ExitPagePatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(ExitPage), "draw"),
            postfix: new HarmonyMethod(typeof(ExitPagePatch), nameof(ExitPagePatch.DrawPatch))
        );
    }

    private static void DrawPatch(ExitPage __instance)
    {
        try
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true);

            if (__instance.exitToTitle.visible &&
                __instance.exitToTitle.containsPoint(x, y))
            {
                MainClass.ScreenReader.TranslateAndSayWithMenuChecker(
                    "menu-exit_page-exit_to_title_button", true);
                return;
            }

            if (__instance.exitToDesktop.visible &&
                __instance.exitToDesktop.containsPoint(x, y))
            {
                MainClass.ScreenReader.TranslateAndSayWithMenuChecker(
                    "menu-exit_page-exit_to_desktop_button", true);
                return;
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in exit page patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
