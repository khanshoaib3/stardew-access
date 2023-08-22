using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Utils
{
    internal static class OptionsElementUtils
    {
        internal static bool NarrateOptionsElementSlots(List<ClickableComponent> optionSlots, List<OptionsElement> options, int currentItemIndex)
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
            for (int i = 0; i < optionSlots.Count; i++)
            {
                if (!optionSlots[i].bounds.Contains(x, y) || currentItemIndex + i >= options.Count || !options[currentItemIndex + i].bounds.Contains(x - optionSlots[i].bounds.X, y - optionSlots[i].bounds.Y))
                    continue;

                MainClass.ScreenReader.SayWithMenuChecker(GetNameOfElement(options[currentItemIndex + i]), true);
                return true;
            }

            return false;
        }

        internal static string GetNameOfElement(OptionsElement optionsElement)
        {
            string translationKey;
            string label = optionsElement.label;
            object? tokens = new { label };

            switch (optionsElement)
            {
                case OptionsButton:
                    translationKey = "options_element-button_info";
                    break;
                case OptionsCheckbox checkbox:
                    translationKey = "options_element-checkbox_info";
                    tokens = new
                    {
                        label,
                        is_checked = checkbox.isChecked ? 1 : 0
                    };
                    break;
                case OptionsDropDown dropdown:
                    translationKey = "options_element-dropdown_info";
                    tokens = new
                    {
                        label,
                        selected_option = dropdown.dropDownDisplayOptions[dropdown.selectedOption]
                    };
                    break;
                case OptionsSlider slider:
                    translationKey = "options_element-slider_info";
                    tokens = new
                    {
                        label,
                        slider_value = slider.value
                    };
                    break;
                case OptionsPlusMinus plusMinus:
                    translationKey = "options_element-plus_minus_button_info";
                    tokens = new
                    {
                        label,
                        selected_option = plusMinus.displayOptions[plusMinus.selected]
                    };
                    break;
                case OptionsInputListener listener:
                    string buttons = string.Join(", ", listener.buttonNames);
                    translationKey = "options_element-input_listener_info";
                    tokens = new
                    {
                        label,
                        buttons_list = buttons
                    };
                    break;
                case OptionsTextEntry textEntry:
                    Log.Debug(textEntry.textBox.TitleText);
                    translationKey = "options_element-text_box_info";
                    tokens = new
                    {
                        label,
                        value = string.IsNullOrEmpty(textEntry.textBox.Text) ? "null" : textEntry.textBox.Text,
                    };
                    break;
                default:
                    return label;
            }

            return Translator.Instance.Translate(translationKey, tokens, TranslationCategory.Menu);
        }
    }
}
