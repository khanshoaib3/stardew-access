using StardewValley;
using StardewValley.Menus;
using System.Text;

namespace stardew_access.Patches
{
    internal class OptionsPagePatch
    {
        internal static void DrawPatch(OptionsPage __instance)
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
                    string toSpeak = optionsElement.label;

                    switch (optionsElement)
                    {
                        case OptionsButton:
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
                        case OptionsInputListener listener:
                            var buttons = new StringBuilder();
                            listener.buttonNames.ForEach(name => buttons.Append($", {name}"));
                            toSpeak = $"{toSpeak} is bound to {buttons}. Left click to change.";
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
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
