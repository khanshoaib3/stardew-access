using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;
using System.Text;

namespace stardew_access.Patches
{
    internal class OptionsPagePatch :IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                    original: AccessTools.Method(typeof(OptionsPage), nameof(OptionsPage.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(OptionsPagePatch), nameof(OptionsPagePatch.DrawPatch))
            );
        }

        private static void DrawPatch(OptionsPage __instance)
        {
            try
            {
                int currentItemIndex = Math.Max(0, Math.Min(__instance.options.Count - 7, __instance.currentItemIndex));
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                for (int i = 0; i < __instance.optionSlots.Count; i++)
                {
                    if (!__instance.optionSlots[i].bounds.Contains(x, y) || currentItemIndex + i >= __instance.options.Count || !__instance.options[currentItemIndex + i].bounds.Contains(x - __instance.optionSlots[i].bounds.X, y - __instance.optionSlots[i].bounds.Y))
                        continue;

                    OptionsElement optionsElement = __instance.options[currentItemIndex + i];
                    string translationKey = optionsElement.label;
                    string label = optionsElement.label;
                    if (label.Contains(":"))
                        label = label.Replace(":", "");
                    object? tokens = new {label = label};

                    switch (optionsElement)
                    {
                        case OptionsButton:
                            translationKey = "menu-options_page-button_info";
                            break;
                        case OptionsCheckbox checkbox:
                            translationKey = "menu-options_page-checkbox_info";
                            tokens = new
                            {
                                label = label,
                                is_checked = checkbox.isChecked ? 1 : 0
                            };
                            break;
                        case OptionsDropDown dropdown:
                            translationKey = "menu-options_page-dropdown_info";
                            tokens = new
                            {
                                label = label,
                                selected_option = dropdown.dropDownDisplayOptions[dropdown.selectedOption]
                            };
                            break;
                        case OptionsSlider slider:
                            translationKey = "menu-options_page-slider_info";
                            tokens = new
                            {
                                label = label,
                                slider_value = slider.value
                            };
                            break;
                        case OptionsPlusMinus plusMinus:
                            translationKey = "menu-options_page-plus_minus_button_info";
                            tokens = new
                            {
                                label = label,
                                selected_option = plusMinus.displayOptions[plusMinus.selected]
                            };
                            break;
                        case OptionsInputListener listener:
                            string buttons = string.Join(", ", listener.buttonNames);
                            translationKey = "menu-options_page-input_listener_info";
                            tokens = new
                            {
                                label = label,
                                buttons_list = buttons
                            };
                            break;
                        default:
                            translationKey = "menu-options_page-heading_info";
                            break;
                    }

                    MainClass.ScreenReader.SayWithMenuChecker(Translator.Instance.Translate(translationKey, tokens), true);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in options page patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
