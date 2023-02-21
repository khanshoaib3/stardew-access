namespace stardew_access.Patches
{
    internal class TextBoxPatch
    {
        internal static string textBoxQuery = " ";

        internal static void DrawPatch(StardewValley.Menus.TextBox __instance)
        {
            try
            {
                bool isEscPressed = StardewValley.Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape); // For escaping/unselecting from the animal name text box
                string toSpeak = " ";
                if (__instance.Selected)
                {
                    MainClass.isAnyTextBoxActive = true;
                    toSpeak = __instance.Text;

                    if (isEscPressed)
                    {
                        __instance.Selected = false;
                    }
                }
                else
                {
                    MainClass.isAnyTextBoxActive = false;
                }

                if (textBoxQuery != toSpeak)
                {
                    textBoxQuery = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"An error occured in DrawPatch() in TextBoxPatch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
