using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class OptionsPagePatch
    {
        internal static string optionsPageQueryKey = "";

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

                    if (optionsElement is OptionsButton)
                        toSpeak = $" {toSpeak} Button";
                    else if (optionsElement is OptionsCheckbox)
                        toSpeak = (((OptionsCheckbox)optionsElement).isChecked ? "Enabled" : "Disabled") + $" {toSpeak} Checkbox";
                    else if (optionsElement is OptionsDropDown)
                        toSpeak = $"{toSpeak} Dropdown, option {((OptionsDropDown)optionsElement).dropDownDisplayOptions[((OptionsDropDown)optionsElement).selectedOption]} selected";
                    else if (optionsElement is OptionsSlider)
                        toSpeak = $"{((OptionsSlider)optionsElement).value}% {toSpeak} Slider";
                    else if (optionsElement is OptionsPlusMinus)
                        toSpeak = $"{((OptionsPlusMinus)optionsElement).displayOptions[((OptionsPlusMinus)optionsElement).selected]} selected of {toSpeak}";
                    else if (optionsElement is OptionsInputListener)
                    {
                        string buttons = "";
                        ((OptionsInputListener)optionsElement).buttonNames.ForEach(name => { buttons += $", {name}"; });
                        toSpeak = $"{toSpeak} is bound to {buttons}. Left click to change.";
                    }
                    else
                    {
                        if (toSpeak.Contains(":"))
                            toSpeak = toSpeak.Replace(":", "");

                        toSpeak = $"{toSpeak} Options:";
                    }

                    if (optionsPageQueryKey != toSpeak)
                    {
                        optionsPageQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup()
        {
            optionsPageQueryKey = "";
        }
    }
}
