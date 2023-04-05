using System.Text.Json;
using StardewValley;
using StardewValley.Menus;
using static stardew_access.Features.Utils;

namespace stardew_access.Patches
{
    internal class CharacterCustomizationMenuPatch
    {
        private static bool isRunning = false;
        private static int saveGameIndex = -1;
        private static string characterCreationMenuQueryKey = " ";
        private static string prevPants = " ";
        private static string prevShirt = " ";
        private static string prevHair = " ";
        private static string prevAccessory = " ";
        private static string prevSkin = " ";
        private static string prevEyeColor = " ";
        private static string prevEyeColorHue = " ";
        private static string prevEyeColorSaturation = " ";
        private static string prevEyeColorValue = " ";
        private static string prevHairColor = " ";
        private static string prevHairColorHue = " ";
        private static string prevHairColorSaturation = " ";
        private static string prevHairColorValue = " ";
        private static string prevPantsColor = " ";
        private static string prevPantsColorHue = " ";
        private static string prevPantsColorSaturation = " ";
        private static string prevPantsColorValue = " ";
        private static string prevPet = " ";
        private static bool characterDesignToggle = false;
        private static bool characterDesignToggleShouldSpeak = true;
        private static ClickableComponent? currentComponent = null;
        private static Dictionary<string, Dictionary<int, string>> descriptions
        {
            get
            {
                if (_descriptions == null)
                {
                    _descriptions = LoadDescriptionJson();
                }
                return _descriptions;
            }
        }
        private static Dictionary<string, Dictionary<int, string>>? _descriptions;

        private static Dictionary<string, Dictionary<int, string>> LoadDescriptionJson()
        {
            MainClass.DebugLog("Attempting to load json");
            JsonElement jsonElement = LoadJsonFile("new-character-appearance-descriptions.json");

            if (jsonElement.ValueKind == JsonValueKind.Undefined)
            {
                return new Dictionary<string, Dictionary<int, string>>();
            }

            Dictionary<string, Dictionary<int, string>> result = new Dictionary<string, Dictionary<int, string>>();

            foreach (JsonProperty category in jsonElement.EnumerateObject())
            {
                Dictionary<int, string> innerDictionary = new Dictionary<int, string>();

                foreach (JsonProperty item in category.Value.EnumerateObject())
                {
                    int index = int.Parse(item.Name);
                    innerDictionary[index] = item.Value.GetString() ?? "";
                }

                result[category.Name] = innerDictionary;
                MainClass.InfoLog($"Loaded key '{category.Name}' with {innerDictionary.Count} entries in the sub dictionary.");
            }

            return result;
        }

        internal static void DrawPatch(CharacterCustomization __instance, bool ___skipIntro,
        ClickableComponent ___startingCabinsLabel, ClickableComponent ___difficultyModifierLabel, TextBox ___nameBox,
        TextBox ___farmnameBox, TextBox ___favThingBox)
        {
            try
            {
                if (TextBoxPatch.isAnyTextBoxActive) return;

                bool isEscPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape); // For escaping/unselecting from the animal name text box
                string toSpeak = "";
                if (characterDesignToggleShouldSpeak)
                {
                    toSpeak = "Press left control + space to toggle character appearance controls";
                    characterDesignToggleShouldSpeak = false;
                }
                string itemsToSpeak = "";
                string changesToSpeak = "";

                if (MainClass.Config.CharacterCreationMenuNextKey.JustPressed() && !isRunning)
                {
                    isRunning = true;
                    itemsToSpeak = CycleThroughItems(true, __instance, ___skipIntro, ___startingCabinsLabel, ___difficultyModifierLabel, ___nameBox, ___farmnameBox, ___favThingBox);
                    if (itemsToSpeak != "")
                        toSpeak = $"{itemsToSpeak} \n {toSpeak}";
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
                }
                else if (MainClass.Config.CharacterCreationMenuPreviousKey.JustPressed() && !isRunning)
                {
                    isRunning = true;
                    toSpeak = CycleThroughItems(false, __instance, ___skipIntro, ___startingCabinsLabel, ___difficultyModifierLabel, ___nameBox, ___farmnameBox, ___favThingBox);
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
                    }
                    else
                    {
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
                MainClass.ErrorLog($"An error occured in character customization menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static string getChangesToSpeak(CharacterCustomization __instance)
        {
            string toSpeak = "";
            string currentPet = GetCurrentPet();
            string currentSkin = GetCurrentSkin();
            string currentHair = GetCurrentHair();
            string currentShirt = GetCurrentShirt();
            string currentPants = GetCurrentPants();
            string currentAccessory = GetCurrentAccessory();
            string currentEyeColor = GetCurrentEyeColor();
            string currentEyeColorHue = GetCurrentEyeColorHue(__instance);
            string currentEyeColorSaturation = GetCurrentEyeColorSaturation(__instance);
            string currentEyeColorValue = GetCurrentEyeColorValue(__instance);
            string currentHairColor = GetCurrentHairColor();
            string currentHairColorHue = GetCurrentHairColorHue(__instance);
            string currentHairColorSaturation = GetCurrentHairColorSaturation(__instance);
            string currentHairColorValue = GetCurrentHairColorValue(__instance);
            string currentPantsColor = GetCurrentPantsColor();
            string currentPantsColorHue = GetCurrentPantsColorHue(__instance);
            string currentPantsColorSaturation = GetCurrentPantsColorSaturation(__instance);
            string currentPantsColorValue = GetCurrentPantsColorValue(__instance);

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
                    }
                    else
                    {
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
                    }
                    else
                    {
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
                    }
                    else
                    {
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
                    }
                    else
                    {
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
                    }
                    else
                    {
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
                    }
                    else
                    {
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
                    }
                    else
                    {
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
                    }
                    else
                    {
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
                    }
                    else
                    {
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

            if (prevPet != currentPet)
            {
                prevPet = currentPet;
                if (currentPet != "")
                    toSpeak = $"{toSpeak} \n Current Pet: {currentPet}";
            }
            return toSpeak.Trim();
        }

        private static string CycleThroughItems(bool increase, CharacterCustomization __instance, bool ___skipIntro,
         ClickableComponent ___startingCabinsLabel, ClickableComponent ___difficultyModifierLabel, TextBox ___nameBox,
         TextBox ___farmnameBox, TextBox ___favThingBox)
        {
            string toSpeak = " ";
            int DesignControlsIndex = 0;
            Dictionary<ClickableComponent, string> buttons = new();

            #region Add buttons with their names IF they are available

            #region Character related
            string postText = "";
            if (__instance.nameBoxCC != null && __instance.nameBoxCC.visible)
            {
                if (___nameBox.Text != "")
                {
                    postText = $": {___nameBox.Text}";
                }
                else
                {
                    postText = " Text Box";
                }
                buttons.Add(__instance.nameBoxCC, $"Farmer's Name{postText}");
            }

            if (__instance.farmnameBoxCC != null && __instance.farmnameBoxCC.visible)
            {
                if (___farmnameBox.Text != "")
                {
                    postText = $": {___farmnameBox.Text}";
                }
                else
                {
                    postText = " Text Box";
                }
                buttons.Add(__instance.farmnameBoxCC, $"Farm's Name{postText}");
            }

            if (__instance.favThingBoxCC != null && __instance.favThingBoxCC.visible)
            {
                if (___favThingBox.Text != "")
                {
                    postText = $": {___favThingBox.Text}";
                }
                else
                {
                    postText = " Text Box";
                }
                buttons.Add(__instance.favThingBoxCC, $"Favourite Thing{postText}");
            }

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
            if (characterDesignToggle && new[] { __instance.leftSelectionButtons.Count, __instance.rightSelectionButtons.Count }.All(c => c >= 0)) // both have Count > 0
            {
                if (new[] { __instance.leftSelectionButtons[DesignControlsIndex].visible, __instance.rightSelectionButtons[DesignControlsIndex].visible }.All(v => v == true) // both visible
                    && new[] { __instance.leftSelectionButtons[DesignControlsIndex].name, __instance.rightSelectionButtons[DesignControlsIndex].name }.All(n => n == "Direction")) // both named "Direction"
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

            if (characterDesignToggle && new[] { __instance.leftSelectionButtons.Count, __instance.rightSelectionButtons.Count }.All(c => c >= DesignControlsIndex) && new[] { __instance.leftSelectionButtons[DesignControlsIndex].visible, __instance.rightSelectionButtons[DesignControlsIndex].visible }.All(v => v == true))
            {
                while (DesignControlsIndex < __instance.leftSelectionButtons.Count)
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
            if (__instance.source == CharacterCustomization.Source.HostNewFarm)
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
            }
            else
            {
                return null;
            }
        }

        private static void AdjustCurrentSlider(bool increase, CharacterCustomization __instance, int amount = 1)
        {
            if (currentComponent != null && currentComponent.myID >= 522 && currentComponent.myID <= 530)
            {
                SliderBar sb = getCurrentSliderBar(currentComponent.myID, __instance)!;
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
                    }
                    else
                    {
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
        private static string GetCurrentPet(bool lessInfo = false)
        {
            if (currentComponent != null && currentComponent.name == "Pet")
            {
                int whichPetBreed = Game1.player.whichPetBreed + 1;

                if (!lessInfo)
                {
                    string petType = Game1.player.catPerson ? "Cat" : "Dog";
                    if (descriptions.TryGetValue(petType, out var innerDict) && innerDict.TryGetValue(whichPetBreed, out var description))
                    {
                        return description;
                    }
                    else
                    {
                        MainClass.ErrorLog($"Warning: Description for {petType} with index {whichPetBreed} not found in the dictionary.");
                    }
                }

                return $"{(Game1.player.catPerson ? "Cat" : "Dog")} #{whichPetBreed + 1}";
            }
            return "";
        }

        private static string GetCurrentAttributeValue(string componentName, Func<int> getValue, bool lessInfo = false)
        {
            if (currentComponent != null && (currentComponent.myID == 507 || currentComponent.name == componentName))
            {
                int index = getValue();

                if (!lessInfo)
                {
                    if (descriptions.TryGetValue(componentName, out var innerDict))
                    {
                        if (innerDict.TryGetValue(index, out var description))
                        {
                            return description;
                        }
                        else
                        {
                            MainClass.ErrorLog($"Warning: Description for {componentName} with index {index} not found in the inner dictionary.");
                        }
                    }
                    else
                    {
                        MainClass.ErrorLog($"Warning: Description for {componentName} not found in the outer dictionary.");
                    }
                }
                return $"{componentName}: {index}";
            }
            return "";
        }

        private static string GetCurrentSkin(bool lessInfo = false) => GetCurrentAttributeValue("Skin", () => Game1.player.skin.Value + 1, lessInfo);

        private static string GetCurrentHair(bool lessInfo = false) => GetCurrentAttributeValue("Hair", () => Game1.player.hair.Value + 1, lessInfo);

        private static string GetCurrentShirt(bool lessInfo = false) => GetCurrentAttributeValue("Shirt", () => Game1.player.shirt.Value + 1, lessInfo);

        private static string GetCurrentPants(bool lessInfo = false) => GetCurrentAttributeValue("Pants Style", () => Game1.player.pants.Value + 1, lessInfo);

        private static string GetCurrentAccessory(bool lessInfo = false) => GetCurrentAttributeValue("Accessory", () => Game1.player.accessory.Value + 2, lessInfo);

        private static string GetCurrentColorAttributeValue(string componentName, int minID, int maxID, Func<string> getValue)
        {
            if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= minID && currentComponent.myID <= maxID)))
            {
                return $"{componentName}: {getValue()}";
            }
            return "";
        }

        private static string GetCurrentEyeColor() => GetCurrentColorAttributeValue("Eye color", 522, 524, () => $"{Game1.player.newEyeColor.R}, {Game1.player.newEyeColor.G}, {Game1.player.newEyeColor.B}");

        private static string GetCurrentEyeColorHue(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Eye color hue", 522, 524, () => (getCurrentSliderBar(522, __instance)!.value!.ToString()));

        private static string GetCurrentEyeColorSaturation(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Eye color saturation", 522, 524, () => (getCurrentSliderBar(523, __instance)!.value!.ToString()));

        private static string GetCurrentEyeColorValue(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Eye color value", 522, 524, () => (getCurrentSliderBar(524, __instance)!.value!.ToString()));

        private static string GetCurrentHairColor() => GetCurrentColorAttributeValue("Hair color", 525, 527, () => $"{Game1.player.hairstyleColor.R}, {Game1.player.hairstyleColor.G}, {Game1.player.hairstyleColor.B}");

        private static string GetCurrentHairColorHue(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Hair color hue", 525, 527, () => (getCurrentSliderBar(525, __instance)!.value!.ToString()));

        private static string GetCurrentHairColorSaturation(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Hair color saturation", 525, 527, () => (getCurrentSliderBar(526, __instance)!.value!.ToString()));

        private static string GetCurrentHairColorValue(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Hair color value", 525, 527, () => (getCurrentSliderBar(527, __instance)!.value!.ToString()));
        private static string GetCurrentPantsColor() => GetCurrentColorAttributeValue("Pants color", 528, 530, () => $"{Game1.player.pantsColor.R}, {Game1.player.pantsColor.G}, {Game1.player.pantsColor.B}");

        private static string GetCurrentPantsColorHue(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Pants color hue", 528, 530, () => (getCurrentSliderBar(528, __instance)!.value!.ToString()));

        private static string GetCurrentPantsColorSaturation(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Pants color saturation", 528, 530, () => (getCurrentSliderBar(529, __instance)!.value!.ToString()));

        private static string GetCurrentPantsColorValue(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Pants color value", 528, 530, () => (getCurrentSliderBar(530, __instance)!.value!.ToString()));
    }
}
