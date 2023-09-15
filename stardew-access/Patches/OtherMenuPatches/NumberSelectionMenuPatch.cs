using StardewValley;
using StardewValley.Menus;
using stardew_access.Translation;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

namespace stardew_access.Patches
{
    internal class NumberSelectionMenuPatch : IPatch
    {
        private static bool firstTimeInMenu = true;
        private static string previousValueNPriceText = "";
        private static string previousHoveredButton = "";

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NumberSelectionMenu), nameof(NumberSelectionMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(NumberSelectionMenuPatch), nameof(NumberSelectionMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(
            NumberSelectionMenu __instance,
            string ___message,
            int ___currentValue,
            int ___price,
            TextBox ___numberSelectedBox
        )
        {
            try
            {
                string toSpeak = "", hoveredButton = "";
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                    hoveredButton = Translator.Instance.Translate("common-ui-ok_button", TranslationCategory.Menu);
                else if (__instance.cancelButton != null && __instance.cancelButton.containsPoint(x, y))
                    hoveredButton = Translator.Instance.Translate("common-ui-cancel_button", TranslationCategory.Menu);
                else if (__instance.leftButton != null && __instance.leftButton.containsPoint(x, y))
                    hoveredButton = Translator.Instance.Translate("menu-number_selection-button-left_button", TranslationCategory.Menu);
                else if (__instance.rightButton != null && __instance.rightButton.containsPoint(x, y))
                    hoveredButton = Translator.Instance.Translate("menu-number_selection-button-right_button", TranslationCategory.Menu);
                else
                    return; // Skips if no button is hovered, this usually happens when the menu is transitioning or fading in.

                int totalPrice = (___price <= 0) ? 0 : ___price * ___currentValue;
                string valueNPriceText = Translator.Instance.Translate("menu-number_selection-value_and_price_info",
                    new { value = ___currentValue, price = totalPrice }, TranslationCategory.Menu
                );

                if (firstTimeInMenu)
                {
                    ___numberSelectedBox.Selected = false;
                    firstTimeInMenu = false;
                    toSpeak = ___message;
                }

                if (valueNPriceText != previousValueNPriceText)
                {
                    previousValueNPriceText = valueNPriceText;
                    toSpeak = $"{toSpeak} {valueNPriceText}";
                }

                if (hoveredButton != previousHoveredButton)
                {
                    previousHoveredButton = hoveredButton;
                    toSpeak = $"{toSpeak} {hoveredButton}";
                }

                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in number selection menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup()
        {
            firstTimeInMenu = true;
            previousValueNPriceText = "";
            previousHoveredButton = "";
        }
    }
}
