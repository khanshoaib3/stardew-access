
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class GameMenuPatches
    {
        internal static string geodeMenuQueryKey = "";

        internal static void GeodeMenuPatch(GeodeMenu __instance)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position

                if (__instance.geodeSpot != null && __instance.geodeSpot.containsPoint(x, y))
                {
                    string toSpeak = "Place geode here";
                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        ScreenReader.say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
                {
                    string toSpeak = "Drop item here";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        ScreenReader.say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
                {
                    string toSpeak = "Trash can";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        ScreenReader.say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    string toSpeak = "Ok button";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        ScreenReader.say(toSpeak, true);
                    }
                    return;
                }

                for (int i = 0; i < __instance.inventory.inventory.Count; i++)
                {
                    if(__instance.inventory.inventory[i].containsPoint(x, y))
                    {
                        string toSpeak = "";
                        if ((i + 1) <= __instance.inventory.actualInventory.Count)
                        {
                            if (__instance.inventory.actualInventory[i] != null)
                            {
                                string name = __instance.inventory.actualInventory[i].DisplayName;
                                int stack = __instance.inventory.actualInventory[i].Stack;

                                if(stack>1)
                                    toSpeak = $"{stack} {name}";
                                else
                                    toSpeak = $"{name}";

                            }
                            else
                            {
                                // For empty slot
                                toSpeak = "Empty Slot";
                            }
                        }
                        else
                        {
                            // For empty slot
                            toSpeak = "Empty Slot";
                        }

                        if (geodeMenuQueryKey != $"{toSpeak}:{i}")
                        {
                            geodeMenuQueryKey = $"{toSpeak}:{i}";
                            ScreenReader.say(toSpeak, true);
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void ItemGrabMenuPatch(ItemGrabMenu __instance)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker("Ok Button", true);
                    return;
                }
                if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker("Trash Can", true);
                    return;
                }

                if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker("Drop Item", true);
                    return;
                }
                
                if (__instance.shippingBin && Game1.getFarm().lastItemShipped != null && __instance.lastShippedHolder.containsPoint(x, y))
                {
                    Item lastShippedItem = Game1.getFarm().lastItemShipped;
                    string name = lastShippedItem.DisplayName;
                    int count = lastShippedItem.Stack;

                    string toSpeak = $"Last Shipped: {count} {name}";

                    ScreenReader.sayWithMenuChecker(toSpeak, true);
                    return;
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void CraftingPagePatch(CraftingPage __instance)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position

                if(__instance.upButton != null && __instance.upButton.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker("Previous Recipe List", true);
                    return;
                }
                
                if (__instance.downButton != null && __instance.downButton.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker("Next Recipe List", true);
                    return;
                }

                if(__instance.trashCan.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker("Trash Can", true);
                    return;
                }

                if(__instance.dropItemInvisibleButton.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker("Drop Item", true);
                    return;
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void InventoryMenuPatch(InventoryMenu __instance)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position

                if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker("Drop Item", true);
                    return;
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void OptionsPagePatch(OptionsPage __instance)
        {
            try
            {
                int currentItemIndex = Math.Max(0, Math.Min(__instance.options.Count - 7, __instance.currentItemIndex));
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y;
                for (int i = 0; i < __instance.optionSlots.Count; i++)
                {
                    if (__instance.optionSlots[i].bounds.Contains(x, y) && currentItemIndex + i < __instance.options.Count && __instance.options[currentItemIndex + i].bounds.Contains(x - __instance.optionSlots[i].bounds.X, y - __instance.optionSlots[i].bounds.Y))
                    {
                        OptionsElement optionsElement = __instance.options[currentItemIndex + i];
                        string toSpeak = optionsElement.label;

                        if (optionsElement is OptionsButton)
                            toSpeak = $" {toSpeak} Button";
                        else if (optionsElement is OptionsCheckbox)
                            toSpeak = ((optionsElement as OptionsCheckbox).isChecked ? "Enabled" : "Disabled") + $" {toSpeak} Checkbox";
                        else if (optionsElement is OptionsDropDown)
                            toSpeak = $"{toSpeak} Dropdown, option {(optionsElement as OptionsDropDown).dropDownDisplayOptions[(optionsElement as OptionsDropDown).selectedOption]} selected";
                        else if (optionsElement is OptionsSlider)
                            toSpeak = $"{(optionsElement as OptionsSlider).value}% {toSpeak} Slider";
                        else if (optionsElement is OptionsPlusMinus)
                            toSpeak = $"{(optionsElement as OptionsPlusMinus).displayOptions[(optionsElement as OptionsPlusMinus).selected]} selected of {toSpeak}";
                        else if (optionsElement is OptionsInputListener)
                        {
                            string buttons = "";
                            (optionsElement as OptionsInputListener).buttonNames.ForEach(name => { buttons += $", {name}"; });
                            toSpeak = $"{toSpeak} is bound to {buttons}. Left click to change.";
                        }
                        else
                        {
                            if (toSpeak.Contains(":"))
                                toSpeak = toSpeak.Replace(":", "");

                            toSpeak = $"{toSpeak} Options:";
                        }

                        ScreenReader.sayWithChecker(toSpeak, true);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void ExitPagePatch(ExitPage __instance)
        {
            try
            {
                if (__instance.exitToTitle.visible &&
                        __instance.exitToTitle.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    ScreenReader.sayWithChecker("Exit to Title Button", true);
                }
                if (__instance.exitToDesktop.visible &&
                    __instance.exitToDesktop.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    ScreenReader.sayWithChecker("Exit to Desktop Button", true);
                }
            }
            catch (Exception e)
            {

                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }
    }
}
