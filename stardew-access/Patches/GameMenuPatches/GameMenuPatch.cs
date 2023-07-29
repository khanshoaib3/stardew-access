using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class GameMenuPatch
    {
        internal static void DrawPatch(GameMenu __instance)
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

                    string toSpeak = $"{GameMenu.getLabelOfTabFromIndex(i)} Tab" + ((i == __instance.currentTab) ? " Active" : "");
                    MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
