using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ChooseFromListMenuPatch
    {
        internal static void DrawPatch(ChooseFromListMenu __instance, List<string> ___options, int ___index, bool ___isJukebox)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string toSpeak = "";

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                    toSpeak = "Select " + (___isJukebox ? Utility.getSongTitleFromCueName(___options[___index]) : ___options[___index]) + " button";
                else if (__instance.cancelButton != null && __instance.cancelButton.containsPoint(x, y))
                    toSpeak = "Cancel button";
                else if (__instance.backButton != null && __instance.backButton.containsPoint(x, y))
                    toSpeak = "Previous option: " + (___isJukebox ? Utility.getSongTitleFromCueName(___options[Math.Max(0, ___index - 1)]) : ___options[Math.Max(0, ___index - 1)]) + " button";
                else if (__instance.forwardButton != null && __instance.forwardButton.containsPoint(x, y))
                    toSpeak = "Next option: " + (___isJukebox ? Utility.getSongTitleFromCueName(___options[Math.Min(___options.Count, ___index + 1)]) : ___options[Math.Min(___options.Count, ___index + 1)]) + " button";

                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
