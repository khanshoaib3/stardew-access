using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class GameMenuPatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(GameMenu), "draw"),
            postfix: new HarmonyMethod(typeof(GameMenuPatch), nameof(GameMenuPatch.DrawPatch))
        );
    }

    private static void DrawPatch(GameMenu __instance)
    {
        try
        {
            // Skip if in map page
            if (__instance.currentTab == 3)
                return;

            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

            for (int i = 0; i < __instance.tabs.Count; i++)
            {
                if (!__instance.tabs[i].containsPoint(x, y))
                    continue;
                string name = GameMenu.getLabelOfTabFromIndex(i);
                if (string.IsNullOrWhiteSpace(name)) name = Game1.content.LoadString("Strings\\1_6_Strings:GameMenu_Animals");

                MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-game_menu-tab_names", true, new
                {
                    tab_name = name,
                    is_active = (i == __instance.currentTab) ? 1 : 0
                });
                return;
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in game menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
