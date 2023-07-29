using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ConfirmationDialogMenuPatch
    {
        internal static void DrawPatch(ConfirmationDialog __instance, string ___message)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                string toSpeak = ___message;

                if (__instance.okButton.containsPoint(x, y))
                {
                    toSpeak += "\n\tOk Button";
                }
                else if (__instance.cancelButton.containsPoint(x, y))
                {
                    toSpeak += "\n\tCancel Button";
                }

                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
