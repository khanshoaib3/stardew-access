using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace stardew_access.Patches
{
    // Menus in the game menu i.e., the menu which opens when we press `e`
    internal class GameMenuPatches
    {
        internal static string hoveredItemQueryKey = "";
        internal static string gameMenuQueryKey = "";
        internal static string inventoryPageQueryKey = "";
        internal static string exitPageQueryKey = "";
        internal static string optionsPageQueryKey = "";
        internal static string profilePageQuery = "";

        internal static void GameMenuPatch(GameMenu __instance)
        {
            try
            {
                // Continue if only in the Inventory Page or Crafting Page
                if (__instance.currentTab != 0 && __instance.currentTab != 4 && __instance.currentTab != 6 && __instance.currentTab != 7)
                    return;

                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                for (int i = 0; i < __instance.tabs.Count; i++)
                {
                    if (__instance.tabs[i].containsPoint(x, y))
                    {
                        string toSpeak = $"{GameMenu.getLabelOfTabFromIndex(i)} Tab";
                        if (gameMenuQueryKey != toSpeak)
                        {
                            MainClass.DebugLog("here");
                            gameMenuQueryKey = toSpeak;
                            MainClass.ScreenReader.Say(toSpeak, true);
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void InventoryPagePatch(InventoryPage __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                #region Narrate buttons in the menu
                if (__instance.inventory.dropItemInvisibleButton != null && __instance.inventory.dropItemInvisibleButton.containsPoint(x, y))
                {
                    string toSpeak = "Drop Item";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.ScreenReader.Say(toSpeak, true);
                        Game1.playSound("drop_item");
                    }
                }

                if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
                {
                    string toSpeak = "Organize Inventory Button";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                }

                if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
                {
                    string toSpeak = "Trash Can";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                }

                if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
                {
                    string toSpeak = "Organize Button";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                }

                if (__instance.junimoNoteIcon != null && __instance.junimoNoteIcon.containsPoint(x, y))
                {

                    string toSpeak = "Community Center Button";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                }
                #endregion

                #region Narrate equipment slots
                for (int i = 0; i < __instance.equipmentIcons.Count; i++)
                {
                    if (__instance.equipmentIcons[i].containsPoint(x, y))
                    {
                        string toSpeak = "";

                        #region Get name and description of the item
                        switch (__instance.equipmentIcons[i].name)
                        {
                            case "Hat":
                                {
                                    if (Game1.player.hat.Value != null)
                                    {
                                        toSpeak = $"{Game1.player.hat.Value.DisplayName}, {Game1.player.hat.Value.getDescription()}";
                                    }
                                    else
                                    {
                                        toSpeak = "Hat slot";
                                    }
                                }
                                break;
                            case "Left Ring":
                                {
                                    if (Game1.player.leftRing.Value != null)
                                    {
                                        toSpeak = $"{Game1.player.leftRing.Value.DisplayName}, {Game1.player.leftRing.Value.getDescription()}";
                                    }
                                    else
                                    {
                                        toSpeak = "Left Ring slot";
                                    }
                                }
                                break;
                            case "Right Ring":
                                {
                                    if (Game1.player.rightRing.Value != null)
                                    {
                                        toSpeak = $"{Game1.player.rightRing.Value.DisplayName}, {Game1.player.rightRing.Value.getDescription()}";
                                    }
                                    else
                                    {
                                        toSpeak = "Right ring slot";
                                    }
                                }
                                break;
                            case "Boots":
                                {
                                    if (Game1.player.boots.Value != null)
                                    {
                                        toSpeak = $"{Game1.player.boots.Value.DisplayName}, {Game1.player.boots.Value.getDescription()}";
                                    }
                                    else
                                    {
                                        toSpeak = "Boots slot";
                                    }
                                }
                                break;
                            case "Shirt":
                                {
                                    if (Game1.player.shirtItem.Value != null)
                                    {
                                        toSpeak = $"{Game1.player.shirtItem.Value.DisplayName}, {Game1.player.shirtItem.Value.getDescription()}";
                                    }
                                    else
                                    {
                                        toSpeak = "Shirt slot";
                                    }
                                }
                                break;
                            case "Pants":
                                {
                                    if (Game1.player.pantsItem.Value != null)
                                    {
                                        toSpeak = $"{Game1.player.pantsItem.Value.DisplayName}, {Game1.player.pantsItem.Value.getDescription()}";
                                    }
                                    else
                                    {
                                        toSpeak = "Pants slot";
                                    }
                                }
                                break;
                        }
                        #endregion

                        if (inventoryPageQueryKey != toSpeak)
                        {
                            inventoryPageQueryKey = toSpeak;
                            gameMenuQueryKey = "";
                            hoveredItemQueryKey = "";
                            MainClass.ScreenReader.Say(toSpeak, true);
                        }
                    }
                }
                #endregion

                #region Narrate hovered item
                if (InventoryUtils.narrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y, true))
                {
                    gameMenuQueryKey = "";
                    inventoryPageQueryKey = "";
                }
                #endregion

                if (MainClass.Config.MoneyKey.JustPressed())
                {
                    string farmName = Game1.content.LoadString("Strings\\UI:Inventory_FarmName", Game1.player.farmName.Value);
                    string currentFunds = Game1.content.LoadString("Strings\\UI:Inventory_CurrentFunds" + (Game1.player.useSeparateWallets ? "_Separate" : ""), Utility.getNumberWithCommas(Game1.player.Money));
                    string totalEarnings = Game1.content.LoadString("Strings\\UI:Inventory_TotalEarnings" + (Game1.player.useSeparateWallets ? "_Separate" : ""), Utility.getNumberWithCommas((int)Game1.player.totalMoneyEarned));
                    int festivalScore = Game1.player.festivalScore;
                    int walnut = Game1.netWorldState.Value.GoldenWalnuts.Value;
                    int qiGems = Game1.player.QiGems;
                    int qiCoins = Game1.player.clubCoins;

                    string toSpeak = $"{farmName}\n{currentFunds}\n{totalEarnings}";

                    if (festivalScore > 0)
                        toSpeak = $"{toSpeak}\nFestival Score: {festivalScore}";

                    if (walnut > 0)
                        toSpeak = $"{toSpeak}\nGolden Walnut: {walnut}";

                    if (qiGems > 0)
                        toSpeak = $"{toSpeak}\nQi Gems: {qiGems}";

                    if (qiCoins > 0)
                        toSpeak = $"{toSpeak}\nQi Club Coins: {qiCoins}";

                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void OptionsPagePatch(OptionsPage __instance)
        {
            try
            {
                int currentItemIndex = Math.Max(0, Math.Min(__instance.options.Count - 7, __instance.currentItemIndex));
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                for (int i = 0; i < __instance.optionSlots.Count; i++)
                {
                    if (__instance.optionSlots[i].bounds.Contains(x, y) && currentItemIndex + i < __instance.options.Count && __instance.options[currentItemIndex + i].bounds.Contains(x - __instance.optionSlots[i].bounds.X, y - __instance.optionSlots[i].bounds.Y))
                    {
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
                            gameMenuQueryKey = "";
                            optionsPageQueryKey = toSpeak;
                            MainClass.ScreenReader.Say(toSpeak, true);
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void ExitPagePatch(ExitPage __instance)
        {
            try
            {
                if (__instance.exitToTitle.visible &&
                        __instance.exitToTitle.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    string toSpeak = "Exit to Title Button";
                    if (exitPageQueryKey != toSpeak)
                    {
                        gameMenuQueryKey = "";
                        exitPageQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }
                if (__instance.exitToDesktop.visible &&
                    __instance.exitToDesktop.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    string toSpeak = "Exit to Desktop Button";
                    if (exitPageQueryKey != toSpeak)
                    {
                        gameMenuQueryKey = "";
                        exitPageQueryKey = toSpeak;
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
    }
}
