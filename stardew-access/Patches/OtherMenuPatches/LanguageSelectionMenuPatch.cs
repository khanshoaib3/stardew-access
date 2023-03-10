using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class LanguageSelectionMenuPatch
    {
        internal static void DrawPatch(LanguageSelectionMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (__instance.nextPageButton != null && __instance.nextPageButton.containsPoint(x, y))
                {
                    MainClass.ScreenReader.SayWithMenuChecker($"Next Page Button", true);
                    return;
                }

                if (__instance.previousPageButton != null && __instance.previousPageButton.containsPoint(x, y))
                {
                    MainClass.ScreenReader.SayWithMenuChecker($"Previous Page Button", true);
                    return;
                }

                for (int i = 0; i < __instance.languages.Count; i++)
                {
                    if (__instance.languages[i].containsPoint(x, y))
                    {
                        MainClass.ScreenReader.SayWithMenuChecker($"{__instance.languageList[i]} Button", true);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
