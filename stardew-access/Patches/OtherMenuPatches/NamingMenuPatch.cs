using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class NamingMenuPatch : IPatch
    {
        internal static bool firstTimeInNamingMenu = true;

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NamingMenu), nameof(NamingMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(NamingMenuPatch), nameof(NamingMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(NamingMenu __instance, TextBox ___textBox, string ___title)
        {
            // TODO Check if there is any bug in this menu
            try
            {
                if (firstTimeInNamingMenu)
                {
                    firstTimeInNamingMenu = false;
                    ___textBox.Selected = false;
                }

                if (TextBoxPatch.IsAnyTextBoxActive) return;

                string translationKey = "";
                object? translationTokens = null;
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                bool isEscPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape); // For escaping/unselecting from the animal name text box

                if (__instance.textBoxCC != null && __instance.textBoxCC.containsPoint(x, y))
                {
                    translationKey = $"menu-options_page-text_box_info";
                    translationTokens = new
                    {
                        label = ___title
                    };
                }
                else if (__instance.doneNamingButton != null && __instance.doneNamingButton.containsPoint(x, y))
                {
                    translationKey = "menu-naming-done_naming_button";
                }
                else if (__instance.randomButton != null && __instance.randomButton.containsPoint(x, y))
                {
                    translationKey = "menu-naming-random_button";
                }

                MainClass.ScreenReader.TranslateAndSayWithChecker(translationKey, true, translationTokens);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in naming menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
