using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class AdvancedGameOptionsPatch
    {
        internal static void DrawPatch(AdvancedGameOptions __instance)
        {
            try
            {
                int currentItemIndex = Math.Max(0, Math.Min(__instance.options.Count - 7, __instance.currentItemIndex));
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    string toSpeak = "OK Button";
                    MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
                    return;
                }

                for (int i = 0; i < __instance.optionSlots.Count; i++)
                {
                    if (!__instance.optionSlots[i].bounds.Contains(x, y)
                            || currentItemIndex + i >= __instance.options.Count
                            || !__instance.options[currentItemIndex + i].bounds.Contains(x - __instance.optionSlots[i].bounds.X, y - __instance.optionSlots[i].bounds.Y))
                        continue;

                    OptionsElement optionsElement = __instance.options[currentItemIndex + i];
                    string toSpeak = optionsElement.label;

                    switch (optionsElement)
                    {
                        case OptionsButton _:
                            toSpeak = $" {toSpeak} Button";
                            break;
                        case OptionsCheckbox checkbox:
                            toSpeak = $"{(checkbox.isChecked ? "Enabled" : "Disabled")} {toSpeak} Checkbox";
                            break;
                        case OptionsDropDown dropdown:
                            toSpeak = $"{toSpeak} Dropdown, option {dropdown.dropDownDisplayOptions[dropdown.selectedOption]} selected";
                            break;
                        case OptionsSlider slider:
                            toSpeak = $"{slider.value}% {toSpeak} Slider";
                            break;
                        case OptionsPlusMinus plusMinus:
                            toSpeak = $"{plusMinus.displayOptions[plusMinus.selected]} selected of {toSpeak}";
                            break;
                        case OptionsInputListener inputListener:
                            string buttons = "";
                            inputListener.buttonNames.ForEach(name => { buttons += $", {name}"; });
                            toSpeak = $"{toSpeak} is bound to {buttons}. Left click to change.";
                            break;
                        case OptionsTextEntry _:
                            toSpeak = $"Seed text box";
                            break;
                        default:
                            if (toSpeak.Contains(":"))
                                toSpeak = toSpeak.Replace(":", "");
                            toSpeak = $"{toSpeak} Options:";
                            break;
                    }

                    MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
                    return;
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"An error occured in advanced game menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
