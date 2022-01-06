
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class GameMenuPatches
    {
        internal static string hoveredItemQueryKey = "";
        internal static string geodeMenuQueryKey = "";
        internal static string itemGrabMenuQueryKey = "";
        internal static string craftingPageQueryKey = "";
        internal static string inventoryPageQueryKey = "";

        internal static void GeodeMenuPatch(GeodeMenu __instance)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position

                #region Narrate the treasure recieved on breaking the geode
                if (__instance.geodeTreasure != null)
                {
                    string name = __instance.geodeTreasure.DisplayName;
                    int stack = __instance.geodeTreasure.Stack;

                    string toSpeak = $"Recieved {stack} {name}";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        ScreenReader.say(toSpeak, true);
                    }
                    return;
                } 
                #endregion

                #region Narrate hovered buttons in the menu
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
                        Game1.playSound("sa_drop_item");
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
                #endregion

                #region Narrate hovered item
                if(narrateHoveredItemInInventory(__instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                    geodeMenuQueryKey = "";
                #endregion
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
                bool isIPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.I);
                bool isLeftShiftPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

                if(isLeftShiftPressed && isIPressed && __instance.inventory != null)
                {
                    __instance.inventory.inventory[0].snapMouseCursorToCenter();
                }else if(!isLeftShiftPressed && isIPressed && __instance.ItemsToGrabMenu != null)
                {
                    __instance.ItemsToGrabMenu.inventory[0].snapMouseCursorToCenter();
                }

                #region Narrate buttons in the menu
                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    string toSpeak = "Ok Button";
                    if(itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        ScreenReader.say(toSpeak, true);
                    }
                    return;
                }
                if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
                {
                    string toSpeak = "Trash Can";
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        ScreenReader.say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
                {
                    string toSpeak = "Drop Item";
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        ScreenReader.say(toSpeak, true);
                        Game1.playSound("sa_drop_item");
                    }
                    return;
                }
                #endregion

                #region Narrate the last shipped item if in the shipping bin
                if (__instance.shippingBin && Game1.getFarm().lastItemShipped != null && __instance.lastShippedHolder.containsPoint(x, y))
                {
                    Item lastShippedItem = Game1.getFarm().lastItemShipped;
                    string name = lastShippedItem.DisplayName;
                    int count = lastShippedItem.Stack;

                    string toSpeak = $"Last Shipped: {count} {name}";

                    ScreenReader.sayWithMenuChecker(toSpeak, true);
                    return;
                } 
                #endregion

                #region Narrate hovered item
                if(narrateHoveredItemInInventory(__instance.inventory.inventory, __instance.inventory.actualInventory, x, y, true))
                    return;

                if(narrateHoveredItemInInventory(__instance.ItemsToGrabMenu.inventory, __instance.ItemsToGrabMenu.actualInventory, x, y, true))
                    return;

                #endregion
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

        internal static void InventoryPagePatch(InventoryPage __instance)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position

                if (__instance.inventory.dropItemInvisibleButton != null && __instance.inventory.dropItemInvisibleButton.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker("Drop Item", true);
                    return;
                }

                #region Narrate hovered item
                if(narrateHoveredItemInInventory(__instance.inventory.inventory, __instance.inventory.actualInventory, x, y, true))
                    inventoryPageQueryKey = "";
                #endregion
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

        internal static bool narrateHoveredItemInInventory(List<ClickableComponent> inventory, IList<Item> actualInventory, int x, int y, bool giveExtraDetails = false)
        {
            #region Narrate hovered item
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].containsPoint(x, y))
                {
                    string toSpeak = "";
                    if ((i + 1) <= actualInventory.Count)
                    {
                        if (actualInventory[i] != null)
                        {
                            string name = actualInventory[i].DisplayName;
                            int stack = actualInventory[i].Stack;
                            string quality = "";
                            string healthNStamine = "";
                            string buffs = "";
                            string description = "";

                            #region Add quality of item
                            if (actualInventory[i] is StardewValley.Object && (actualInventory[i] as StardewValley.Object).quality > 0)
                            {
                                int qualityIndex = (actualInventory[i] as StardewValley.Object).quality;
                                if (qualityIndex == 1)
                                {
                                    quality = "Silver quality";
                                }
                                else if (qualityIndex == 2 || qualityIndex == 3)
                                {
                                    quality = "Gold quality";
                                }
                                else if (qualityIndex >= 4)
                                {
                                    quality = "Iridium quality";
                                }
                            }
                            #endregion

                            if (giveExtraDetails)
                            {
                                description = actualInventory[i].getDescription();
                                #region Add health & stamina provided by the item
                                if (actualInventory[i] is StardewValley.Object && (actualInventory[i] as StardewValley.Object).Edibility != -300)
                                {
                                    int stamina_recovery = (actualInventory[i] as StardewValley.Object).staminaRecoveredOnConsumption();
                                    healthNStamine += $"{stamina_recovery} Energy";
                                    if (stamina_recovery >= 0)
                                    {
                                        int health_recovery = (actualInventory[i] as StardewValley.Object).healthRecoveredOnConsumption();
                                        healthNStamine += $"\n\t{health_recovery} Health";
                                    }
                                }
                                #endregion

                                #region Add buff items (effects like +1 walking speed)
                                // These variables are taken from the game's code itself (IClickableMenu.cs -> 1016 line)
                                bool edibleItem = actualInventory[i] != null && actualInventory[i] is StardewValley.Object && (int)(actualInventory[i] as StardewValley.Object).edibility != -300;
                                string[] buffIconsToDisplay = (edibleItem && Game1.objectInformation[(actualInventory[i] as StardewValley.Object).parentSheetIndex].Split('/').Length > 7) ? actualInventory[i].ModifyItemBuffs(Game1.objectInformation[(actualInventory[i] as StardewValley.Object).parentSheetIndex].Split('/')[7].Split(' ')) : null;
                                if (buffIconsToDisplay != null)
                                {
                                    for (int j = 0; j < buffIconsToDisplay.Length; j++)
                                    {
                                        string buffName = ((Convert.ToInt32(buffIconsToDisplay[j]) > 0) ? "+" : "") + buffIconsToDisplay[j] + " ";
                                        if (j <= 11)
                                        {
                                            buffName = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + j, buffName);
                                        }
                                        try
                                        {
                                            int count = int.Parse(buffName.Substring(0, buffName.IndexOf(' ')));
                                            if (count != 0)
                                                buffs += $"{buffName}\n";
                                        }
                                        catch (Exception) { }
                                    }
                                }
                                #endregion 
                            }

                            if (giveExtraDetails)
                            {
                                if (stack > 1)
                                    toSpeak = $"{stack} {name} {quality}, \n{description}, \n{healthNStamine}, \n{buffs}";
                                else
                                    toSpeak = $"{name} {quality}, \n{description}, \n{healthNStamine}, \n{buffs}"; 
                            }
                            else
                            {
                                if (stack > 1)
                                    toSpeak = $"{stack} {name} {quality}";
                                else
                                    toSpeak = $"{name} {quality}";
                            }
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

                    if (hoveredItemQueryKey != $"{toSpeak}:{i}")
                    {
                        hoveredItemQueryKey = $"{toSpeak}:{i}";
                        ScreenReader.say(toSpeak, true);
                    }
                    return true;
                }
            }
            #endregion
            return false;
        }
    }
}
