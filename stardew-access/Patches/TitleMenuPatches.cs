using StardewValley;
using StardewValley.Menus;
using static StardewValley.Menus.LoadGameMenu;

namespace stardew_access.Patches
{
    internal class TitleMenuPatches
    {
        public static string advancedGameOptionsQueryKey = " ";

        internal static void AdvancedGameOptionsPatch(AdvancedGameOptions __instance)
        {
            try
            {
                int currentItemIndex = Math.Max(0, Math.Min(__instance.options.Count - 7, __instance.currentItemIndex));
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    string toSpeak = "OK Button";
                    if (advancedGameOptionsQueryKey != toSpeak)
                    {
                        advancedGameOptionsQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }

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
                        else if (optionsElement is OptionsTextEntry)
                        {
                            toSpeak = $"Seed text box";
                        }
                        else
                        {
                            if (toSpeak.Contains(":"))
                                toSpeak = toSpeak.Replace(":", "");

                            toSpeak = $"{toSpeak} Options:";
                        }

                        if (advancedGameOptionsQueryKey != toSpeak)
                        {
                            advancedGameOptionsQueryKey = toSpeak;
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

        internal static void CoopMenuPatch(CoopMenu __instance, CoopMenu.Tab ___currentTab)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                string toSpeak = " ";

                #region Join/Host Button (Important! This should be checked before checking other buttons)
                if (__instance.slotButtons[0].containsPoint(x, y))
                {
                    if (___currentTab == CoopMenu.Tab.JOIN_TAB)
                        toSpeak = "Join lan game";
                    if (___currentTab == CoopMenu.Tab.HOST_TAB)
                        toSpeak = "Host new farm";
                }
                #endregion

                #region Other Buttons
                if (__instance.joinTab.containsPoint(x, y))
                {
                    toSpeak = "Join Tab Button";
                }
                else if (__instance.hostTab.containsPoint(x, y))
                {
                    toSpeak = "Host Tab Button";
                }
                else if (__instance.refreshButton.containsPoint(x, y))
                {
                    toSpeak = "Refresh Button";
                }
                #endregion

                if (toSpeak != " ")
                    MainClass.ScreenReader.SayWithChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void TitleMenuPatch(TitleMenu __instance, bool ___isTransitioningButtons)
        {
            try
            {
                if (___isTransitioningButtons)
                    return;

                string toSpeak = "";

                __instance.buttons.ForEach(component =>
                {
                    if (component.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                    {
                        string name = component.name;
                        string label = component.label;
                        toSpeak = $"{name} {label} Button";
                    }
                });

                if (__instance.muteMusicButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    toSpeak = "Mute Music Button";
                }

                if (__instance.aboutButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    toSpeak = "About Button";
                }

                if (__instance.languageButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    toSpeak = "Language Button";
                }

                if (__instance.windowedButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    toSpeak = "Fullscreen: " + ((Game1.isFullscreen) ? "enabled" : "disabled");
                }

                if (TitleMenu.subMenu != null && __instance.backButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    string text = "Back Button";
                    MainClass.ScreenReader.SayWithChecker(text, true);
                }

                // Fix for back button not working using keyboard
                if (TitleMenu.subMenu is CharacterCustomization && ((CharacterCustomization)TitleMenu.subMenu).backButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    // Perform Left Click
                    if (MainClass.Config.LeftClickMainKey.JustPressed())
                    {
                        __instance.backButtonPressed();
                    }
                }

                if (TitleMenu.subMenu == null && toSpeak != "")
                    MainClass.ScreenReader.SayWithChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void LoadGameMenuPatch(SaveFileSlot __instance, LoadGameMenu ___menu, int i)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                if (___menu.slotButtons[i].containsPoint(x, y))
                {
                    if (__instance.Farmer != null)
                    {
                        #region Farms
                        if (___menu.deleteButtons.Count > 0 && ___menu.deleteButtons[i].containsPoint(x, y))
                        {
                            MainClass.ScreenReader.SayWithChecker($"Delete {__instance.Farmer.farmName.Value} Farm", true);
                            return;
                        }

                        if (___menu.deleteConfirmationScreen)
                        {
                            // Used diff. functions to narrate to prevent it from speaking the message again on selecting another button.
                            string message = "Really delete farm?";

                            MainClass.ScreenReader.SayWithChecker(message, true);
                            if (___menu.okDeleteButton.containsPoint(x, y))
                            {
                                MainClass.ScreenReader.SayWithMenuChecker("Ok Button", false);
                            }
                            else if (___menu.cancelDeleteButton.containsPoint(x, y))
                            {
                                MainClass.ScreenReader.SayWithMenuChecker("Cancel Button", false);
                            }
                            return;
                        }

                        String farmerName = __instance.Farmer.displayName;
                        String farmName = __instance.Farmer.farmName.Value;
                        String money = __instance.Farmer.Money.ToString();
                        String hoursPlayed = Utility.getHoursMinutesStringFromMilliseconds(__instance.Farmer.millisecondsPlayed);
                        string dateStringForSaveGame = ((!__instance.Farmer.dayOfMonthForSaveGame.HasValue ||
                            !__instance.Farmer.seasonForSaveGame.HasValue ||
                            !__instance.Farmer.yearForSaveGame.HasValue) ? __instance.Farmer.dateStringForSaveGame : Utility.getDateStringFor(__instance.Farmer.dayOfMonthForSaveGame.Value, __instance.Farmer.seasonForSaveGame.Value, __instance.Farmer.yearForSaveGame.Value));

                        string toSpeak = $"{farmName} Farm Selected, \t\n Farmer: {farmerName}, \t\nMoney: {money}, \t\nHours Played: {hoursPlayed}, \t\nDate: {dateStringForSaveGame}";

                        MainClass.ScreenReader.SayWithChecker(toSpeak, true);
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
