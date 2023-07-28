using StardewValley;
using StardewValley.Menus;
using stardew_access.Translation;

namespace stardew_access.Patches
{
    internal class NumberSelectionMenuPatch
    {
        private static bool firstTimeInMenu = true;
        private static string previousValueNPriceText = "";
        private static string previousHoveredButton = "";

        internal static void DrawPatch(
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
                    hoveredButton = Translator.Instance.Translate("common-ui-ok_button");
                else if ( __instance.cancelButton != null && __instance.cancelButton.containsPoint(x, y))
                    hoveredButton = Translator.Instance.Translate("common-ui-cancel_button");
                else if (__instance.leftButton != null && __instance.leftButton.containsPoint(x, y))
                    hoveredButton = Translator.Instance.Translate( "menu-number_selection-button-left_button");
                else if ( __instance.rightButton != null && __instance.rightButton.containsPoint(x, y))
                    hoveredButton = Translator.Instance.Translate( "menu-number_selection-button-right_button");
                else
                    return; // Skips if no button is hovered, this usually happens when the menu is transitioning or fading in.

                int totalPrice = (___price <= 0) ? 0 : ___price * ___currentValue;
                string valueNPriceText = Translator.Instance.Translate( "menu-number_selection-value_and_price_info",
                    new { value = ___currentValue, price = totalPrice }
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
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
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
