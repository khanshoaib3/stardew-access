using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class CoopMenuPatch
    {
        private static string coopMenuQueryKey = "";

        internal static void DrawPatch(CoopMenu __instance, CoopMenu.Tab ___currentTab)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                string toSpeak = "";

                #region Join/Host Button (Important! This should be checked before checking other buttons)
                if (__instance.slotButtons[0].containsPoint(x, y))
                {
                    if (___currentTab == CoopMenu.Tab.JOIN_TAB)
                        toSpeak = "Join lan game";
                    if (___currentTab == CoopMenu.Tab.HOST_TAB)
                        toSpeak = "Host new farm";
                }
                #endregion

                #region Other Buttons
                if (__instance.joinTab.containsPoint(x, y))
                {
                    toSpeak = "Join Tab Button";
                }
                else if (__instance.hostTab.containsPoint(x, y))
                {
                    toSpeak = "Host Tab Button";
                }
                else if (__instance.refreshButton.containsPoint(x, y))
                {
                    toSpeak = "Refresh Button";
                }
                #endregion

                if (coopMenuQueryKey != toSpeak)
                {
                    coopMenuQueryKey = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occured in co-op menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup()
        {
            coopMenuQueryKey = "";
        }
    }
}
