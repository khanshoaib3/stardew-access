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
        public static string prevPants = " ";
        public static string prevShirt = " ";
        public static string prevHair = " ";
        public static string prevAccessory = " ";
        public static string prevSkin = " ";
        public static string prevEyeColor = " ";
        public static string prevEyeColorHue = " ";
        public static string prevEyeColorSaturation = " ";
        public static string prevEyeColorValue = " ";
        public static string prevHairColor = " ";
        public static string prevHairColorHue = " ";
        public static string prevHairColorSaturation = " ";
        public static string prevHairColorValue = " ";
        public static string prevPantsColor = " ";
        public static string prevPantsColorHue = " ";
        public static string prevPantsColorSaturation = " ";
        public static string prevPantsColorValue = " ";
        public static string prevPetName = " ";
        public static bool characterDesignToggle = false;
        public static bool characterDesignToggleShouldSpeak = true;
        public static ClickableComponent? currentComponent = null;

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

        internal static void CharacterCustomizationMenuPatch(CharacterCustomization __instance, bool ___skipIntro,
        ClickableComponent ___startingCabinsLabel, ClickableComponent ___difficultyModifierLabel, TextBox ___nameBox,
        TextBox ___farmnameBox, TextBox ___favThingBox)
        {
            try
            {
                bool isEscPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape); // For escaping/unselecting from the animal name text box
                string toSpeak = "";
                if (characterDesignToggleShouldSpeak)
                {
                    toSpeak = "Press left control + space to toggle character appearance controls";
                    characterDesignToggleShouldSpeak = false;
                }
                string itemsToSpeak = "";
                string changesToSpeak = "";

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
                    itemsToSpeak =CycleThroughItems(true, __instance, ___skipIntro, ___startingCabinsLabel, ___difficultyModifierLabel);
                    if (itemsToSpeak != "")
                        toSpeak = $"{itemsToSpeak} \n {toSpeak}";
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
                }
                else if (MainClass.Config.CharacterCreationMenuPreviousKey.JustPressed() && !isRunning)
                {
                    isRunning = true;
                    toSpeak = CycleThroughItems(false, __instance, ___skipIntro, ___startingCabinsLabel, ___difficultyModifierLabel);
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
                }

                else if (characterDesignToggle && MainClass.Config.CharacterCreationMenuSliderIncreaseKey.JustPressed() && !isRunning)
                {
                    isRunning = true;
                    AdjustCurrentSlider(true, __instance);
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
                }

                else if (characterDesignToggle && MainClass.Config.CharacterCreationMenuSliderLargeIncreaseKey.JustPressed() && !isRunning)
                {
                    isRunning = true;
                    AdjustCurrentSlider(true, __instance, 10);
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
                }

                else if (characterDesignToggle && MainClass.Config.CharacterCreationMenuSliderDecreaseKey.JustPressed() && !isRunning)
                {
                    isRunning = true;
                    AdjustCurrentSlider(false, __instance);
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
                }

                else if (characterDesignToggle && MainClass.Config.CharacterCreationMenuSliderLargeDecreaseKey.JustPressed() && !isRunning)
                {
                    isRunning = true;
                    AdjustCurrentSlider(false, __instance, 10);
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
                }

                else if (Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && MainClass.Config.CharacterCreationMenuDesignToggleKey.JustPressed() && !isRunning)
                {
                    string displayState = "";
                    characterDesignToggle = !characterDesignToggle;
                    saveGameIndex = Math.Min(saveGameIndex, 5); // move to random skin button if focus was beyond that point
                    if (characterDesignToggle)
                    {
                        displayState = "shown";
                    } else {
                        displayState = "hidden";
                    }
                    toSpeak = $"Character design controls {displayState}. \n {toSpeak}";
                }

                changesToSpeak = getChangesToSpeak(__instance);
                if (changesToSpeak != "")
                    toSpeak = $"{toSpeak} \n {changesToSpeak}";

                if (characterCreationMenuQueryKey != toSpeak && toSpeak.Trim() != "")
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

        private static string getChangesToSpeak(CharacterCustomization __instance)
        {
            string toSpeak = "";
            string currentPetName = getCurrentPetName();
            string currentSkin = getCurrentSkin();
            string currentHair = getCurrentHair();
            string currentShirt = getCurrentShirt();
            string currentPants = getCurrentPants();
            string currentAccessory = getCurrentAccessory();
            string currentEyeColor = getCurrentEyeColor();
            string currentEyeColorHue = getCurrentEyeColorHue(__instance);
            string currentEyeColorSaturation = getCurrentEyeColorSaturation(__instance);
            string currentEyeColorValue = getCurrentEyeColorValue(__instance);
            string currentHairColor = getCurrentHairColor();
            string currentHairColorHue = getCurrentHairColorHue(__instance);
            string currentHairColorSaturation = getCurrentHairColorSaturation(__instance);
            string currentHairColorValue = getCurrentHairColorValue(__instance);
            string currentPantsColor = getCurrentPantsColor();
            string currentPantsColorHue = getCurrentPantsColorHue(__instance);
            string currentPantsColorSaturation = getCurrentPantsColorSaturation(__instance);
            string currentPantsColorValue = getCurrentPantsColorValue(__instance);

            if (characterDesignToggle)
            {
                if (prevSkin != currentSkin)
                {
                    prevSkin = currentSkin;
                    if (currentSkin != "")
                        toSpeak = $"{toSpeak} \n {currentSkin}";
                }

                if (prevHair != currentHair)
                {
                    prevHair = currentHair;
                    if (currentHair != "")
                        toSpeak = $"{toSpeak} \n {currentHair}";
                }

                if (prevShirt != currentShirt)
                {
                    prevShirt = currentShirt;
                    if (currentShirt != "")
                        toSpeak = $"{toSpeak} \n {currentShirt}";
                }

                if (prevPants != currentPants)
                {
                    prevPants = currentPants;
                    if (currentPants != "")
                        toSpeak = $"{toSpeak} \n {currentPants}";
                }

                if (prevAccessory != currentAccessory)
                {
                    prevAccessory = currentAccessory;
                    if (currentAccessory != "")
                        toSpeak = $"{toSpeak} \n {currentAccessory}";
                }

                if (prevEyeColorHue != currentEyeColorHue)
                {
                    if (currentComponent != null && currentComponent.myID == 522)
                    {
                        prevEyeColorHue = currentEyeColorHue;
                        if (currentEyeColorHue != "")
                            toSpeak = $"{toSpeak} \n Hue: {currentEyeColorHue}";
                    } else {
                        prevEyeColorHue = "";
                    }
                }

                if (prevEyeColorSaturation != currentEyeColorSaturation)
                {
                    if (currentComponent != null && currentComponent.myID == 523)
                    {
                        prevEyeColorSaturation = currentEyeColorSaturation;
                        if (currentEyeColorSaturation != "")
                            toSpeak = $"{toSpeak} \n Saturation: {currentEyeColorSaturation}";
                    } else {
                        prevEyeColorSaturation = "";
                    }
                }

                if (prevEyeColorValue != currentEyeColorValue)
                {
                    if (currentComponent != null && currentComponent.myID == 524)
                    {
                        prevEyeColorValue = currentEyeColorValue;
                        if (currentEyeColorValue != "")
                            toSpeak = $"{toSpeak} \n Value: {currentEyeColorValue}";
                    } else {
                        prevEyeColorValue = "";
                    }
                }

                if (prevEyeColor != currentEyeColor)
                {
                    if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 522 || currentComponent.myID <= 524)))
                    {
                        prevEyeColor = currentEyeColor;
                        if (currentEyeColor != "")
                            toSpeak = $"{toSpeak} \n {currentEyeColor}";
                    }
                }

                if (prevHairColorHue != currentHairColorHue)
                {
                    if (currentComponent != null && currentComponent.myID == 525)
                    {
                        prevHairColorHue = currentHairColorHue;
                        if (currentHairColorHue != "")
                            toSpeak = $"{toSpeak} \n Hue: {currentHairColorHue}";
                    } else {
                        prevHairColorHue = "";
                    }
                }

                if (prevHairColorSaturation != currentHairColorSaturation)
                {
                    if (currentComponent != null && currentComponent.myID == 526)
                    {
                        prevHairColorSaturation = currentHairColorSaturation;
                        if (currentHairColorSaturation != "")
                            toSpeak = $"{toSpeak} \n Saturation: {currentHairColorSaturation}";
                    } else {
                        prevHairColorSaturation = "";
                    }
                }

                if (prevHairColorValue != currentHairColorValue)
                {
                    if (currentComponent != null && currentComponent.myID == 527)
                    {
                        prevHairColorValue = currentHairColorValue;
                        if (currentHairColorValue != "")
                            toSpeak = $"{toSpeak} \n Value: {currentHairColorValue}";
                    } else {
                        prevHairColorValue = "";
                    }
                }

                if (prevHairColor != currentHairColor)
                {
                    if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 525 || currentComponent.myID <= 527)))
                    {
                        prevHairColor = currentHairColor;
                        if (currentHairColor != "")
                            toSpeak = $"{toSpeak} \n {currentHairColor}";
                    }
                }

                if (prevPantsColorHue != currentPantsColorHue)
                {
                    if (currentComponent != null && currentComponent.myID == 528)
                    {
                        prevPantsColorHue = currentPantsColorHue;
                        if (currentPantsColorHue != "")
                            toSpeak = $"{toSpeak} \n Hue: {currentPantsColorHue}";
                    } else {
                        prevPantsColorHue = "";
                    }
                }

                if (prevPantsColorSaturation != currentPantsColorSaturation)
                {
                    if (currentComponent != null && currentComponent.myID == 529)
                    {
                        prevPantsColorSaturation = currentPantsColorSaturation;
                        if (currentPantsColorSaturation != "")
                            toSpeak = $"{toSpeak} \n Saturation: {currentPantsColorSaturation}";
                    } else {
                        prevPantsColorSaturation = "";
                    }
                }

                if (prevPantsColorValue != currentPantsColorValue)
                {
                    if (currentComponent != null && currentComponent.myID == 530)
                    {
                        prevPantsColorValue = currentPantsColorValue;
                        if (currentPantsColorValue != "")
                            toSpeak = $"{toSpeak} \n Value: {currentPantsColorValue}";
                    } else {
                        prevPantsColorValue = "";
                    }
                }

                if (prevPantsColor != currentPantsColor)
                {
                    if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 528 || currentComponent.myID <= 530)))
                    {
                        prevPantsColor = currentPantsColor;
                        if (currentPantsColor != "")
                            toSpeak = $"{toSpeak} \n {currentPantsColor}";
                    }
                }
            }

            if (prevPetName != currentPetName)
            {
                prevPetName = currentPetName;
                if (currentPetName  != "")
                    toSpeak = $"{toSpeak} \n Current Pet: {currentPetName}";
            }
            return toSpeak.Trim();
        }


        private static string CycleThroughItems(bool increase, CharacterCustomization __instance, bool ___skipIntro,
         ClickableComponent ___startingCabinsLabel, ClickableComponent ___difficultyModifierLabel)
        {
            string toSpeak = " ";
            int DesignControlsIndex = 0;
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

            // Controls to rotate the farmer (Potentially useful for low vision players) are first if they're available.
            // They also appear above the gender buttons, so we handle them separately here.
            if (characterDesignToggle && new[] {__instance.leftSelectionButtons.Count, __instance.rightSelectionButtons.Count }.All(c => c >= 0)) // both have Count > 0
            {
                if (new[] {__instance.leftSelectionButtons[DesignControlsIndex].visible, __instance.rightSelectionButtons[DesignControlsIndex].visible }.All(v => v == true) // both visible
                    && new[] {__instance.leftSelectionButtons[DesignControlsIndex].name, __instance.rightSelectionButtons[DesignControlsIndex].name }.All(n => n == "Direction")) // both named "Direction"
                {
                    buttons.Add(__instance.leftSelectionButtons[DesignControlsIndex], "Rotate Left Button");
                    buttons.Add(__instance.rightSelectionButtons[DesignControlsIndex], "Rotate Right Button");
                    ++DesignControlsIndex;
                }
            }

            if (__instance.genderButtons.Count > 0)
            {
                buttons.Add(__instance.genderButtons[0], ((Game1.player.IsMale) ? "Selected " : "") + "Gender: Male Button");
                buttons.Add(__instance.genderButtons[1], ((!Game1.player.IsMale) ? "Selected " : "") + "Gender: Female Button");
            }

            if (characterDesignToggle&& new[] {__instance.leftSelectionButtons.Count, __instance.rightSelectionButtons.Count }.All(c => c >= DesignControlsIndex) && new[] {__instance.leftSelectionButtons[DesignControlsIndex].visible, __instance.rightSelectionButtons[DesignControlsIndex].visible }.All(v => v == true))
            {
                while(DesignControlsIndex < __instance.leftSelectionButtons.Count)
                {
                    ClickableComponent left = __instance.leftSelectionButtons[DesignControlsIndex];
                    ClickableComponent right = __instance.rightSelectionButtons[DesignControlsIndex];
                    string name = left.name;
                    // minor cleanup on names to be slightly more descriptive
                    switch (name)
                    {
                        case "Skin":
                            name += " Tone";
                            break;
                        case "Hair":
                            name += " Style";
                            break;
                        case "Acc":
                            name = "Accessory";
                            break;
                        default:
                            break;
                    }
                    if (!buttons.ContainsKey(left) || !buttons.ContainsKey(right))
                    {
                        buttons.Add(left, $"Previous {name} button");
                        buttons.Add(right, $"Next {name} button");
                    }
                    //MainClass.ScreenReader.Say($"Left {DesignControlsIndex}: {__instance.leftSelectionButtons[DesignControlsIndex]} {__instance.leftSelectionButtons[DesignControlsIndex].name}\n", true);
                    //MainClass.ScreenReader.Say($"Right {DesignControlsIndex}: {__instance.rightSelectionButtons[DesignControlsIndex]} {__instance.rightSelectionButtons[DesignControlsIndex].name}\n", true);
                    ++DesignControlsIndex;
                }

                ClickableComponent eyeColorHue = __instance.getComponentWithID(522);
                if (eyeColorHue != null && eyeColorHue.visible)
                    buttons.Add(eyeColorHue, "eye color hue slider");

                ClickableComponent eyeColorSaturation = __instance.getComponentWithID(523);
                if (eyeColorSaturation != null && eyeColorSaturation.visible)
                    buttons.Add(eyeColorSaturation, "eye color saturation slider");

                ClickableComponent eyeColorValue = __instance.getComponentWithID(524);
                if (eyeColorValue != null && eyeColorValue.visible)
                    buttons.Add(eyeColorValue, "eye color Value slider");

                ClickableComponent hairColorHue = __instance.getComponentWithID(525);
                if (hairColorHue != null && hairColorHue.visible)
                    buttons.Add(hairColorHue, "hair color hue slider");

                ClickableComponent hairColorSaturation = __instance.getComponentWithID(526);
                if (hairColorSaturation != null && hairColorSaturation.visible)
                    buttons.Add(hairColorSaturation, "hair color saturation slider");

                ClickableComponent hairColorValue = __instance.getComponentWithID(527);
                if (hairColorValue != null && hairColorValue.visible)
                    buttons.Add(hairColorValue, "hair color Value slider");

                ClickableComponent pantsColorHue = __instance.getComponentWithID(528);
                if (pantsColorHue != null && pantsColorHue.visible)
                    buttons.Add(pantsColorHue, "pants color hue slider");

                ClickableComponent pantsColorSaturation = __instance.getComponentWithID(529);
                if (pantsColorSaturation != null && pantsColorSaturation.visible)
                    buttons.Add(pantsColorSaturation, "pants color saturation slider");

                ClickableComponent pantsColorValue = __instance.getComponentWithID(530);
                if (pantsColorValue != null && pantsColorValue.visible)
                    buttons.Add(pantsColorValue, "pants color Value slider");
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

            currentComponent = buttons.ElementAt(saveGameIndex).Key;
            currentComponent!.snapMouseCursor(); 
            __instance.setCurrentlySnappedComponentTo(currentComponent!.myID);

            toSpeak = buttons.ElementAt(saveGameIndex).Value;

            return toSpeak.Trim();
        }

        private static SliderBar? getCurrentSliderBar(int id, CharacterCustomization __instance)
        {
            if (id >= 522 && id <= 530)
            {
                // Three ColorPickers with 3 SliderBars each.
                // First group ids by ColorPicker.
                // Maps 522-524 -> 0, 525-527 -> 1, 528-530 -> 2
                int whichColorPicker = (int)Math.Floor(((float)id - 522f) / 3f);
                // Next group ids by slider type.
                // Maps [522,525,528] -> 0, [523,526,529] -> 1, [524,527,530] -> 2
                int whichSliderBar = (int)Math.Floor((float)id % 3f);
                ColorPicker cp;
                switch (whichColorPicker)
                {
                    default:
                    case 0:
                        // 522-524 == eye color
                        cp = __instance.eyeColorPicker;
                        break;
                    case 1:
                        // 525-527 == hair color
                        cp = __instance.hairColorPicker;
                        break;
                    case 2:
                        // 528-530 == pants color
                        cp = __instance.pantsColorPicker;
                        break;
                }
                SliderBar sb;
                switch (whichSliderBar)
                {
                    default:
                    case 0:
                        // 522, 525, 528 == hue slider
                        sb = cp.hueBar;
                        break;
                    case 1:
                        // 523, 526, 529 == saturation slider
                        sb = cp.saturationBar;
                        break;
                    case 2:
                        // 524, 527, 530 == value slider
                        sb = cp.valueBar;
                        break;
                }
                return sb;
            } else {
                return null;
            }
        }

        private static  void AdjustCurrentSlider(bool increase, CharacterCustomization __instance, int amount=1) 
        {
            if (currentComponent != null && currentComponent.myID >= 522 && currentComponent.myID <= 530)
            {
                SliderBar sb = getCurrentSliderBar(currentComponent.myID, __instance) !;
                if (sb != null)
                {
                    double step = ((double)sb.bounds.Width / 100d); // size of 1% change in slider value
                    double value = (double)sb.value;
                    double x = 0d;
                    int y = currentComponent.bounds.Center.Y;
                    if (increase)
                    {
                        value = Math.Min(value + amount, 99d);
                        x = Math.Min(Math.Ceiling((value * step)), (double)sb.bounds.Width);
                    } else {
                        value = Math.Max(value - amount, 0d);
                        x = Math.Max(Math.Ceiling((value * step)), 0d);
                    }
                    x += (double)currentComponent.bounds.Left;
                    Game1.setMousePosition((int)x, y);
                    Game1.activeClickableMenu.receiveLeftClick((int)x, y);
                }
            }
        }

        // Most values (exception noted below) are 0 indexed internally but visually start from 1. Thus we increment before returning.
        private static string getCurrentSkin()
        {
            if (currentComponent != null && (currentComponent.myID == 507 || currentComponent.name == "Skin"))
                return $"Skin tone: {Game1.player.skin.Value + 1}";
            return "";
        }

        private static string getCurrentHair()
        {
            if (currentComponent != null && (currentComponent.myID == 507 || currentComponent.name == "Hair"))
                return $"hair style: {Game1.player.hair.Value + 1}";
            return "";
        }

        private static string getCurrentShirt()
        {
            if (currentComponent != null && (currentComponent.myID == 507 || currentComponent.name == "Shirt"))
                return $"Shirt: {Game1.player.shirt.Value + 1}";
            return "";
        }

        private static string getCurrentPants()
        {
            if (currentComponent != null && (currentComponent.myID == 507 || currentComponent.name == "Pants Style"))
                return $"Pants: {Game1.player.pants.Value + 1}";
            return "";
        }

        private static string getCurrentAccessory()
        {
            // Internally accessory starts from -1 while displaying +1 on screen.
            if (currentComponent != null && (currentComponent.myID == 507 || currentComponent.name == "Acc"))
                return $"accessory: {Game1.player.accessory.Value + 2}";
            return "";
        }

        private static string getCurrentEyeColor()
        {
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 522 && currentComponent.myID <= 524)))
                return $"Eye color: {Game1.player.newEyeColor.R}, {Game1.player.newEyeColor.G}, {Game1.player.newEyeColor.B}";
            return "";
        }

        private static string getCurrentEyeColorHue(CharacterCustomization __instance)
        {
            SliderBar sb = getCurrentSliderBar(522, __instance)!;
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 522 && currentComponent.myID <= 524)))
                return sb.value!.ToString();
            return "";
        }

        private static string getCurrentEyeColorSaturation(CharacterCustomization __instance)
        {
            SliderBar sb = getCurrentSliderBar(523, __instance)!;
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 522 && currentComponent.myID <= 524)))
                return sb.value!.ToString();
            return "";
        }

        private static string getCurrentEyeColorValue(CharacterCustomization __instance)
        {
            SliderBar sb = getCurrentSliderBar(524, __instance)!;
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 522 && currentComponent.myID <= 524)))
                return sb.value!.ToString();
            return "";
        }

        private static string getCurrentHairColor()
        {
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 525 && currentComponent.myID <= 527)))
                return $"Hair color: {Game1.player.hairstyleColor.R}, {Game1.player.hairstyleColor.G}, {Game1.player.hairstyleColor.B}";
            return "";
        }

        private static string getCurrentHairColorHue(CharacterCustomization __instance)
        {
            SliderBar sb = getCurrentSliderBar(525, __instance)!;
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 525 && currentComponent.myID <= 527)))
                return sb.value!.ToString();
            return "";
        }

        private static string getCurrentHairColorSaturation(CharacterCustomization __instance)
        {
            SliderBar sb = getCurrentSliderBar(526, __instance)!;
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 525 && currentComponent.myID <= 527)))
                return sb.value!.ToString();
            return "";
        }

        private static string getCurrentHairColorValue(CharacterCustomization __instance)
        {
            SliderBar sb = getCurrentSliderBar(527, __instance)!;
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 525 && currentComponent.myID <= 527)))
                return sb.value!.ToString();
            return "";
        }

        private static string getCurrentPantsColor()
        {
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 528 && currentComponent.myID <= 530)))
                return $"Pants color: {Game1.player.pantsColor.R}, {Game1.player.pantsColor.G}, {Game1.player.pantsColor.B}";
            return "";
        }

        private static string getCurrentPantsColorHue(CharacterCustomization __instance)
        {
            SliderBar sb = getCurrentSliderBar(528, __instance)!;
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 528 && currentComponent.myID <= 530)))
                return sb.value!.ToString();
            return "";
        }

        private static string getCurrentPantsColorSaturation(CharacterCustomization __instance)
        {
            SliderBar sb = getCurrentSliderBar(529, __instance)!;
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 528 && currentComponent.myID <= 530)))
                return sb.value!.ToString();
            return "";
        }

        private static string getCurrentPantsColorValue(CharacterCustomization __instance)
        {
            SliderBar sb = getCurrentSliderBar(530, __instance)!;
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 528 && currentComponent.myID <= 530)))
                return sb.value!.ToString();
            return "";
        }

        private static string getCurrentPetName()
        {
            if (currentComponent != null && currentComponent.name == "Pet")
            {
                return ((Game1.player.catPerson) ? "Cat" : "Dog") + " Breed: " + Game1.player.whichPetBreed;
            } else {
                return "";
            }
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
