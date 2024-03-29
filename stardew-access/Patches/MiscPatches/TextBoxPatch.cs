using HarmonyLib;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class TextBoxPatch : IPatch
    {
        internal static string textBoxQuery = "";
        internal static string activeTextBoxes = "";
        internal static bool IsAnyTextBoxActive => activeTextBoxes != "";

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                    original: AccessTools.Method(typeof(TextBox), nameof(TextBox.Draw)),
                    prefix: new HarmonyMethod(typeof(TextBoxPatch), nameof(TextBoxPatch.DrawPatch))
                );
        }

        internal static void DrawPatch(StardewValley.Menus.TextBox __instance)
        {
            try
            {
                string uniqueIdentifier = $"{__instance.X}:{__instance.Y}:{__instance.Height}:{__instance.Width}";
                if (!__instance.Selected)
                {
                    if (activeTextBoxes.Contains(uniqueIdentifier)) activeTextBoxes = activeTextBoxes.Replace(uniqueIdentifier, "");
                    return;
                }

                if (!activeTextBoxes.Contains(uniqueIdentifier)) activeTextBoxes += uniqueIdentifier;

                bool isEscPressed = StardewValley.Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape);
                string toSpeak = __instance.Text;

                if (isEscPressed)
                {
                    if (activeTextBoxes.Contains(uniqueIdentifier)) activeTextBoxes = activeTextBoxes.Replace(uniqueIdentifier, "");
                    __instance.Selected = false;
                }

                if (textBoxQuery != toSpeak)
                {
                    textBoxQuery = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occured in DrawPatch() in TextBoxPatch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
