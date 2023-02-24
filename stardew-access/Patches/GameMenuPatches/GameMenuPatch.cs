using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class GameMenuPatch
    {
        internal static string gameMenuQueryKey = "";

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
                    if (gameMenuQueryKey != toSpeak)
                    {
                        gameMenuQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }

                // If not hovering on any tab button
                Cleanup();
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup()
        {
            gameMenuQueryKey = "";
        }
    }
}
