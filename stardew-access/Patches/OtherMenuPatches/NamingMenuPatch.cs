using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class NamingMenuPatch
    {
        internal static bool firstTimeInNamingMenu = true;

        internal static void DrawPatch(NamingMenu __instance, TextBox ___textBox, string ___title)
        {
            try
            {
                if (firstTimeInNamingMenu)
                {
                    firstTimeInNamingMenu = false;
                    ___textBox.Selected = false;
                }

                if (TextBoxPatch.IsAnyTextBoxActive) return;

                string toSpeak = "";
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                bool isEscPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape); // For escaping/unselecting from the animal name text box

                if (__instance.textBoxCC != null && __instance.textBoxCC.containsPoint(x, y))
                    toSpeak = $"{___title} text box";
                else if (__instance.doneNamingButton != null && __instance.doneNamingButton.containsPoint(x, y))
                    toSpeak = $"Done naming button";
                else if (__instance.randomButton != null && __instance.randomButton.containsPoint(x, y))
                    toSpeak = $"Random button";

                if (toSpeak != "")
                    MainClass.ScreenReader.SayWithChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
