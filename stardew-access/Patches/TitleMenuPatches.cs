﻿using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using static StardewValley.Menus.CharacterCustomization;
using static StardewValley.Menus.LoadGameMenu;

namespace stardew_access.Patches
{
    internal class TitleMenuPatches
    {
        private static int saveGameIndex = -1;
        private static bool isRunning = false;
        public static string characterCreationMenuQueryKey = " ";
        public static string advancedGameOptionsQueryKey = " ";
        public static string prevPetName = " ";

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

                        string toSpeak = $"{farmName} Farm Selected, \t\n Farmer:{farmerName}, \t\nMoney:{money}, \t\nHours Played:{hoursPlayed}, \t\nDate:{dateStringForSaveGame}";

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

        internal static void CharacterCustomizationMenuPatch(CharacterCustomization __instance, bool ___skipIntro,
        ClickableComponent ___startingCabinsLabel, ClickableComponent ___difficultyModifierLabel, TextBox ___nameBox,
        TextBox ___farmnameBox, TextBox ___favThingBox)
        {
            try
            {
                bool isEscPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape); // For escaping/unselecting from the animal name text box
                string toSpeak = " ";
                string currentPetName = getCurrentPetName();

                if (___nameBox.Selected)
                {
                    toSpeak = ___nameBox.Text;

                    if (isEscPressed)
                    {
                        ___nameBox.Selected = false;
                    }
                }
                else if (___farmnameBox.Selected)
                {
                    toSpeak = ___farmnameBox.Text;

                    if (isEscPressed)
                    {
                        ___farmnameBox.Selected = false;
                    }
                }
                else if (___favThingBox.Selected)
                {
                    toSpeak = ___favThingBox.Text;

                    if (isEscPressed)
                    {
                        ___favThingBox.Selected = false;
                    }
                }
                else if (MainClass.Config.CharacterCreationMenuNextKey.JustPressed() && !isRunning)
                {
                    isRunning = true;
                    CycleThroughItems(true, __instance, ___skipIntro, ___startingCabinsLabel, ___difficultyModifierLabel);
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
                }
                else if (MainClass.Config.CharacterCreationMenuPreviousKey.JustPressed() && !isRunning)
                {
                    isRunning = true;
                    CycleThroughItems(false, __instance, ___skipIntro, ___startingCabinsLabel, ___difficultyModifierLabel);
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
                }

                if (prevPetName != currentPetName)
                {
                    prevPetName = currentPetName;
                    toSpeak = $"Current Pet: {currentPetName} \n {toSpeak}";
                }

                if (characterCreationMenuQueryKey != toSpeak && toSpeak != " ")
                {
                    characterCreationMenuQueryKey = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void CycleThroughItems(bool increase, CharacterCustomization __instance, bool ___skipIntro,
         ClickableComponent ___startingCabinsLabel, ClickableComponent ___difficultyModifierLabel)
        {
            string toSpeak = " ";
            Dictionary<ClickableComponent, string> buttons = new();

            #region Add buttons with their names IF they are available

            #region Character related
            if (__instance.nameBoxCC != null && __instance.nameBoxCC.visible)
                buttons.Add(__instance.nameBoxCC, "Farmer's Name Text box");

            if (__instance.farmnameBoxCC != null && __instance.farmnameBoxCC.visible)
                buttons.Add(__instance.farmnameBoxCC, "Farm's Name Text box");

            if (__instance.favThingBoxCC != null && __instance.favThingBoxCC.visible)
                buttons.Add(__instance.favThingBoxCC, "Favourite Thing Text box");

            if (__instance.petPortraitBox.HasValue) // Cannot get petButtons like with others
            {
                ClickableComponent petPrev = __instance.getComponentWithID(511);
                buttons.Add(petPrev, "Previous pet button");

                ClickableComponent petNext = __instance.getComponentWithID(510);
                buttons.Add(petNext, "Next pet button");
            }

            if (__instance.randomButton != null && __instance.randomButton.visible)
                buttons.Add(__instance.randomButton, "Random Skin Button");

            if (__instance.genderButtons.Count > 0)
            {
                buttons.Add(__instance.genderButtons[0], ((Game1.player.IsMale) ? "Selected " : "") + "Gender: Male Button");
                buttons.Add(__instance.genderButtons[1], ((!Game1.player.IsMale) ? "Selected " : "") + "Gender: Female Button");
            }
            #endregion

            #region Farm layout related
            if (__instance.farmTypeButtons.Count > 0)
            {
                for (int i = 0; i < __instance.farmTypeButtons.Count; i++)
                {
                    buttons.Add(__instance.farmTypeButtons[i], ((i == Game1.whichFarm) ? "Selected " : "") + getFarmHoverText(__instance.farmTypeButtons[i]));
                }
            }

            if (__instance.farmTypeNextPageButton != null && __instance.farmTypeNextPageButton.visible)
                buttons.Add(__instance.farmTypeNextPageButton, "Next Farm Type Page Button");

            if (__instance.farmTypePreviousPageButton != null && __instance.farmTypePreviousPageButton.visible)
                buttons.Add(__instance.farmTypePreviousPageButton, "Previous Farm Type Page Button");
            #endregion

            #region Co-op related
            if (__instance.source == Source.HostNewFarm)
            {
                ClickableComponent cabinLeft = __instance.getComponentWithID(621);
                if (Game1.startingCabins > 0)
                    buttons.Add(cabinLeft, "Decrease starting cabins button");

                buttons.Add(___startingCabinsLabel, $"Starting cabins: {Game1.startingCabins}");

                ClickableComponent cabinRight = __instance.getComponentWithID(622);
                if (Game1.startingCabins < 3)
                    buttons.Add(cabinRight, "Increase starting cabins button");

                if (Game1.startingCabins > 0)
                {
                    buttons.Add(__instance.cabinLayoutButtons[0], "Cabin layout to nearby Button");
                    buttons.Add(__instance.cabinLayoutButtons[1], "Cabin layout to separate Button");
                }

                ClickableComponent difficultyLeft = __instance.getComponentWithID(627);
                buttons.Add(difficultyLeft, "Increase profit margin button");
                buttons.Add(___difficultyModifierLabel, "Profit Margin: " + (((Game1.player.difficultyModifier * 100) == 100f) ? "normal" : Game1.player.difficultyModifier.ToString()));
                ClickableComponent difficultyRight = __instance.getComponentWithID(628);
                buttons.Add(difficultyRight, "Decrease profit margin button");

                ClickableComponent walletLeft = __instance.getComponentWithID(631);
                buttons.Add(walletLeft, "Money style to " + ((!Game1.player.team.useSeparateWallets.Value) ? "separate wallets" : "shared wallets") + " button");
            }
            #endregion

            if (__instance.skipIntroButton != null && __instance.skipIntroButton.visible)
                buttons.Add(__instance.skipIntroButton, (___skipIntro ? "Enabled" : "Disabled") + " Skip Intro Button");

            if (__instance.advancedOptionsButton != null && __instance.advancedOptionsButton.visible)
                buttons.Add(__instance.advancedOptionsButton, "Advanced Options Button");

            if (__instance.okButton != null && __instance.okButton.visible)
                buttons.Add(__instance.okButton, "OK Button");

            if (__instance.backButton != null && __instance.backButton.visible)
                buttons.Add(__instance.backButton, "Back Button");
            #endregion

            int size = buttons.Count - 1;

            if (increase)
            {
                saveGameIndex++;
                if (saveGameIndex > size)
                    saveGameIndex = 0;
            }
            else
            {
                saveGameIndex--;
                if (saveGameIndex < 0)
                    saveGameIndex = size;
            }

            buttons.ElementAt(saveGameIndex).Key.snapMouseCursor();
            toSpeak = buttons.ElementAt(saveGameIndex).Value;

            if (toSpeak != " ")
            {
                MainClass.ScreenReader.Say(toSpeak, true);
            }
        }

        private static string getCurrentPetName()
        {
            return ((Game1.player.catPerson) ? "Cat" : "Dog") + " Breed: " + Game1.player.whichPetBreed;
        }

        private static string getFarmHoverText(ClickableTextureComponent farm)
        {
            string hoverTitle = " ", hoverText = " ";
            if (!farm.name.Contains("Gray"))
            {
                if (farm.hoverText.Contains('_'))
                {
                    hoverTitle = farm.hoverText.Split('_')[0];
                    hoverText = farm.hoverText.Split('_')[1];
                }
                else
                {
                    hoverTitle = " ";
                    hoverText = farm.hoverText;
                }
            }
            else
            {
                if (farm.name.Contains("Gray"))
                {
                    hoverText = "Reach level 10 " + Game1.content.LoadString("Strings\\UI:Character_" + farm.name.Split('_')[1]) + " to unlock.";
                }
            }

            return $"{hoverTitle}: {hoverText}";
        }
    }
}
