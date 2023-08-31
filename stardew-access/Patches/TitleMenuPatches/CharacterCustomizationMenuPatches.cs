using StardewValley;
using StardewValley.Menus;
using System.Text.Json;
using static stardew_access.Utils.JsonLoader;
using static stardew_access.Utils.ColorMatcher;
using stardew_access.Translation;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

namespace stardew_access.Patches
{
    internal class CharacterCustomizationMenuPatch : IPatch
    {
        private static bool isRunning = false;
        private static int saveGameIndex = -1;
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

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(CharacterCustomizationMenuPatch), nameof(CharacterCustomizationMenuPatch.DrawPatch))
            );
        }

        private static Dictionary<string, Dictionary<int, string>> Descriptions
        {
            get
            {
                _descriptions ??= LoadDescriptionJson();
                return _descriptions;
            }
        }
        private static Dictionary<string, Dictionary<int, string>>? _descriptions;

        private static Dictionary<string, Dictionary<int, string>> LoadDescriptionJson()
        {
            Log.Debug("Attempting to load json");
            bool loaded = TryLoadJsonFile("new-character-appearance-descriptions.json", out JsonElement jsonElement);

            if (!loaded || jsonElement.ValueKind == JsonValueKind.Undefined)
            {
                return new Dictionary<string, Dictionary<int, string>>();
            }

            Dictionary<string, Dictionary<int, string>> result = new();

            foreach (JsonProperty category in jsonElement.EnumerateObject())
            {
                Dictionary<int, string> innerDictionary = new();

                foreach (JsonProperty item in category.Value.EnumerateObject())
                {
                    int index = int.Parse(item.Name);
                    innerDictionary[index] = item.Value.GetString() ?? "";
                }

                result[category.Name] = innerDictionary;
                Log.Info($"Loaded key '{category.Name}' with {innerDictionary.Count} entries in the sub dictionary.");
            }

            return result;
        }

        internal static void DrawPatch(CharacterCustomization __instance, bool ___skipIntro,
        ClickableComponent ___startingCabinsLabel, ClickableComponent ___difficultyModifierLabel, TextBox ___nameBox,
        TextBox ___farmnameBox, TextBox ___favThingBox)
        {
            try
            {
                if (TextBoxPatch.IsAnyTextBoxActive) return;

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

                changesToSpeak = GetChangesToSpeak(__instance);
                if (changesToSpeak != "")
                    toSpeak = $"{toSpeak} \n {changesToSpeak}";

                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in character customization menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static string GetChangesToSpeak(CharacterCustomization __instance)
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

                if (prevEyeColor != currentEyeColor)
                {
                    if (currentComponent != null && (currentComponent.myID == 507 || (currentComponent.myID >= 522 || currentComponent.myID <= 524)))
                    {
                        prevEyeColor = currentEyeColor;
                        if (currentEyeColor != "")
                            toSpeak = $"{toSpeak} \n {currentEyeColor}";
                    }
                }

                if (prevEyeColorHue != currentEyeColorHue)
                {
                    if (currentComponent != null && currentComponent.myID == 522)
                    {
                        prevEyeColorHue = currentEyeColorHue;
                        if (currentEyeColorHue != "")
                            toSpeak = $"{toSpeak} \n {currentEyeColorHue}%";
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
                            toSpeak = $"{toSpeak} \n {currentEyeColorSaturation}%";
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
                            toSpeak = $"{toSpeak} \n {currentEyeColorValue}%";
                    }
                    else
                    {
                        prevEyeColorValue = "";
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

                if (prevHairColorHue != currentHairColorHue)
                {
                    if (currentComponent != null && currentComponent.myID == 525)
                    {
                        prevHairColorHue = currentHairColorHue;
                        if (currentHairColorHue != "")
                            toSpeak = $"{toSpeak} \n {currentHairColorHue}%";
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
                            toSpeak = $"{toSpeak} \n {currentHairColorSaturation}%";
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
                            toSpeak = $"{toSpeak} \n {currentHairColorValue}%";
                    }
                    else
                    {
                        prevHairColorValue = "";
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

                if (prevPantsColorHue != currentPantsColorHue)
                {
                    if (currentComponent != null && currentComponent.myID == 528)
                    {
                        prevPantsColorHue = currentPantsColorHue;
                        if (currentPantsColorHue != "")
                            toSpeak = $"{toSpeak} \n {currentPantsColorHue}%";
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
                            toSpeak = $"{toSpeak} \n {currentPantsColorSaturation}%";
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
                            toSpeak = $"{toSpeak} \n {currentPantsColorValue}%";
                    }
                    else
                    {
                        prevPantsColorValue = "";
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
            Dictionary<ClickableComponent, (string translationKey, object? translationTokens)> buttons = new();

            #region Add buttons with their names IF they are available

            #region Character related
            if (__instance.nameBoxCC != null && __instance.nameBoxCC.visible)
            {
                buttons.Add(__instance.nameBoxCC, ("menu-character_creation-farmer_name_text_box", new
                {
                    value = string.IsNullOrEmpty(___nameBox.Text) ? "null" : ___nameBox.Text
                }));
            }

            if (__instance.farmnameBoxCC != null && __instance.farmnameBoxCC.visible)
            {
                buttons.Add(__instance.farmnameBoxCC, ("menu-character_creation-farm_name_text_box", new
                {
                    value = string.IsNullOrEmpty(___farmnameBox.Text) ? "null" : ___farmnameBox.Text
                }));
            }

            if (__instance.favThingBoxCC != null && __instance.favThingBoxCC.visible)
            {
                buttons.Add(__instance.favThingBoxCC, ("menu-character_creation-favorite_thing_text_box", new
                {
                    value = string.IsNullOrEmpty(___favThingBox.Text) ? "null" : ___favThingBox.Text
                }));
            }

            if (__instance.petPortraitBox.HasValue) // Cannot get petButtons like with others
            {
                ClickableComponent petPrev = __instance.getComponentWithID(511);
                buttons.Add(petPrev, ("menu-character_creation-previous_pet_button", null));

                ClickableComponent petNext = __instance.getComponentWithID(510);
                buttons.Add(petNext, ("menu-character_creation-next_pet_button", null));
            }

            if (__instance.randomButton != null && __instance.randomButton.visible)
                buttons.Add(__instance.randomButton, ("menu-character_creation-random_skin_button", null));

            // Controls to rotate the farmer (Potentially useful for low vision players) are first if they're available.
            // They also appear above the gender buttons, so we handle them separately here.
            if (characterDesignToggle && new[] { __instance.leftSelectionButtons.Count, __instance.rightSelectionButtons.Count }.All(c => c >= 0)) // both have Count > 0
            {
                if (new[] { __instance.leftSelectionButtons[DesignControlsIndex].visible, __instance.rightSelectionButtons[DesignControlsIndex].visible }.All(v => v == true) // both visible
                    && new[] { __instance.leftSelectionButtons[DesignControlsIndex].name, __instance.rightSelectionButtons[DesignControlsIndex].name }.All(n => n == "Direction")) // both named "Direction"
                {
                    buttons.Add(__instance.leftSelectionButtons[DesignControlsIndex], ("menu-character_creation-rotate_left_button", null));
                    buttons.Add(__instance.rightSelectionButtons[DesignControlsIndex], ("menu-character_creation-rotate_right_button", null));
                    ++DesignControlsIndex;
                }
            }

            if (__instance.genderButtons.Count > 0)
            {
                buttons.Add(__instance.genderButtons[0], ("menu-character_creation-gender_button", new
                {
                    is_selected = Game1.player.IsMale ? 1 : 0,
                    is_male = 1
                }));
                buttons.Add(__instance.genderButtons[1], ("menu-character_creation-gender_button", new
                {
                    is_selected = !Game1.player.IsMale ? 1 : 0,
                    is_male = 0
                }));
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
                        // FIXME Translate these!!
                        buttons.Add(left, ($"Previous {name} button", null));
                        buttons.Add(right, ($"Next {name} button", null));
                    }
                    //MainClass.ScreenReader.Say($"Left {DesignControlsIndex}: {__instance.leftSelectionButtons[DesignControlsIndex]} {__instance.leftSelectionButtons[DesignControlsIndex].name}\n", true);
                    //MainClass.ScreenReader.Say($"Right {DesignControlsIndex}: {__instance.rightSelectionButtons[DesignControlsIndex]} {__instance.rightSelectionButtons[DesignControlsIndex].name}\n", true);
                    ++DesignControlsIndex;
                }

                ClickableComponent eyeColorHue = __instance.getComponentWithID(522);
                if (eyeColorHue != null && eyeColorHue.visible)
                    buttons.Add(eyeColorHue, ("menu-character_creation-eye_color_hue_slider", null));

                ClickableComponent eyeColorSaturation = __instance.getComponentWithID(523);
                if (eyeColorSaturation != null && eyeColorSaturation.visible)
                    buttons.Add(eyeColorSaturation, ("menu-character_creation-eye_color_saturation_slider", null));

                ClickableComponent eyeColorValue = __instance.getComponentWithID(524);
                if (eyeColorValue != null && eyeColorValue.visible)
                    buttons.Add(eyeColorValue, ("menu-character_creation-eye_color_value_slider", null));

                ClickableComponent hairColorHue = __instance.getComponentWithID(525);
                if (hairColorHue != null && hairColorHue.visible)
                    buttons.Add(hairColorHue, ("menu-character_creation-hair_color_hue_slider", null));

                ClickableComponent hairColorSaturation = __instance.getComponentWithID(526);
                if (hairColorSaturation != null && hairColorSaturation.visible)
                    buttons.Add(hairColorSaturation, ("menu-character_creation-hair_color_saturation_slider", null));

                ClickableComponent hairColorValue = __instance.getComponentWithID(527);
                if (hairColorValue != null && hairColorValue.visible)
                    buttons.Add(hairColorValue, ("menu-character_creation-hair_color_value_slider", null));

                ClickableComponent pantsColorHue = __instance.getComponentWithID(528);
                if (pantsColorHue != null && pantsColorHue.visible)
                    buttons.Add(pantsColorHue, ("menu-character_creation-pants_color_hue_slider", null));

                ClickableComponent pantsColorSaturation = __instance.getComponentWithID(529);
                if (pantsColorSaturation != null && pantsColorSaturation.visible)
                    buttons.Add(pantsColorSaturation, ("menu-character_creation-pants_color_saturation_slider", null));

                ClickableComponent pantsColorValue = __instance.getComponentWithID(530);
                if (pantsColorValue != null && pantsColorValue.visible)
                    buttons.Add(pantsColorValue, ("menu-character_creation-pants_color_value_slider", null));
            }

            #endregion

            #region Farm layout related
            if (__instance.farmTypeButtons.Count > 0)
            {
                for (int i = 0; i < __instance.farmTypeButtons.Count; i++)
                {
                    buttons.Add(__instance.farmTypeButtons[i], ("menu-character_creation-farm_type_buttons", new
                    {
                        is_selected = (i == Game1.whichFarm) ? 1 : 0,
                        value = GetFarmHoverText(__instance.farmTypeButtons[i])
                    }));
                }
            }

            if (__instance.farmTypeNextPageButton != null && __instance.farmTypeNextPageButton.visible)
                buttons.Add(__instance.farmTypeNextPageButton, ("menu-character_creation-next_farm_type_page_button", null));

            if (__instance.farmTypePreviousPageButton != null && __instance.farmTypePreviousPageButton.visible)
                buttons.Add(__instance.farmTypePreviousPageButton, ("menu-character_creation-previous_farm_type_page_button", null));
            #endregion

            #region Co-op related
            if (__instance.source == CharacterCustomization.Source.HostNewFarm)
            {
                ClickableComponent cabinLeft = __instance.getComponentWithID(621);
                if (Game1.startingCabins > 0)
                    buttons.Add(cabinLeft, ("menu-character_creation-decrease_starting_cabins_button", null));

                buttons.Add(___startingCabinsLabel, ("menu-character_creation-starting_cabins_label", new
                {
                    value = Game1.startingCabins
                }));

                ClickableComponent cabinRight = __instance.getComponentWithID(622);
                if (Game1.startingCabins < 3)
                    buttons.Add(cabinRight, ("menu-character_creation-increase_starting_cabins_button", null));

                if (Game1.startingCabins > 0)
                {
                    buttons.Add(__instance.cabinLayoutButtons[0], ("menu-character_creation-cabin_layout_nearby_button", null));
                    buttons.Add(__instance.cabinLayoutButtons[1], ("menu-character_creation-cabin_layout_separate_button", null));
                }

                ClickableComponent difficultyLeft = __instance.getComponentWithID(627);
                buttons.Add(difficultyLeft, ("menu-character_creation-increase_profit_margin_button", null));
                buttons.Add(___difficultyModifierLabel, ("menu-character_creation-profit_margin_label", new
                {
                    value = ((Game1.player.difficultyModifier * 100) == 100f) ? "normal" : Game1.player.difficultyModifier.ToString()
                }));
                ClickableComponent difficultyRight = __instance.getComponentWithID(628);
                buttons.Add(difficultyRight, ("menu-character_creation-decrease_profit_margin_button", null));

                ClickableComponent walletLeft = __instance.getComponentWithID(631);
                buttons.Add(walletLeft, ("menu-character_creation-money_style_separate_wallets_button", new
                {
                    separate_wallets = !Game1.player.team.useSeparateWallets.Value ? 1 : 0
                }));
            }
            #endregion

            if (__instance.skipIntroButton != null && __instance.skipIntroButton.visible)
                buttons.Add(__instance.skipIntroButton, ("menu-character_creation-skip_intro_button", new
                {
                    is_enabled = ___skipIntro ? 1 : 0
                }));

            if (__instance.advancedOptionsButton != null && __instance.advancedOptionsButton.visible)
                buttons.Add(__instance.advancedOptionsButton, ("menu-character_creation-advanced_options_button", null));

            if (__instance.okButton != null && __instance.okButton.visible)
                buttons.Add(__instance.okButton, ("common-ui-ok_button", null));

            if (__instance.backButton != null && __instance.backButton.visible)
                buttons.Add(__instance.backButton, ("common-ui-back_button", null));
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

            (string translationKey, object? translationTokens) = buttons.ElementAt(saveGameIndex).Value;
            toSpeak = Translator.Instance.Translate(translationKey, translationTokens);

            return toSpeak.Trim();
        }

        private static string GetFarmHoverText(ClickableTextureComponent farm)
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

        private static SliderBar? GetCurrentSliderBar(int id, CharacterCustomization __instance)
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
                ColorPicker
                        // 525-527 == hair color
                        cp = whichColorPicker switch
                        {
                            1 => __instance.hairColorPicker,// 525-527 == hair color
                            2 => __instance.pantsColorPicker,// 528-530 == pants color
                            _ => __instance.eyeColorPicker,// 522-524 == eye color
                        };
                SliderBar
                        // 523, 526, 529 == saturation slider
                        sb = whichSliderBar switch
                        {
                            1 => cp.saturationBar,// 523, 526, 529 == saturation slider
                            2 => cp.valueBar,// 524, 527, 530 == value slider
                            _ => cp.hueBar,// 522, 525, 528 == hue slider
                        };
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
                SliderBar sb = GetCurrentSliderBar(currentComponent.myID, __instance)!;
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
                    if (Descriptions.TryGetValue(petType, out var innerDict) && innerDict.TryGetValue(whichPetBreed, out var description))
                    {
                        return description;
                    }
                    else
                    {
                        Log.Error($"Warning: Description for {petType} with index {whichPetBreed} not found in the dictionary.");
                    }
                }

                return $"{(Game1.player.catPerson ? "Cat" : "Dog")} #{whichPetBreed + 1}";
            }
            return "";
        }

        private static string GetCurrentAttributeValue(string componentName, Func<int> getValue, bool lessInfo = false)
        {
            if (currentComponent != null && (currentComponent.myID == 507 || (!string.IsNullOrEmpty(currentComponent.name) && componentName.StartsWith(currentComponent.name, StringComparison.OrdinalIgnoreCase))))
            {
                int index = getValue();

                if (!lessInfo)
                {
                    if (Descriptions.TryGetValue(componentName, out var innerDict))
                    {
                        if (innerDict.TryGetValue(index, out var description))
                        {
                            return description;
                        }
                        else
                        {
                            Log.Error($"Warning: Description for {componentName} with index {index} not found in the inner dictionary.");
                        }
                    }
                    else
                    {
                        Log.Error($"Warning: Description for {componentName} not found in the outer dictionary.");
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

        private static string GetCurrentEyeColor()
        {
            return GetCurrentColorAttributeValue("Eye color", 522, 524, () => GetNearestColorName(Game1.player.newEyeColor.R, Game1.player.newEyeColor.G, Game1.player.newEyeColor.B));
        }

        private static string GetCurrentEyeColorHue(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Hue", 522, 524, () => (GetCurrentSliderBar(522, __instance)!.value!.ToString()));
        private static string GetCurrentEyeColorSaturation(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Saturation", 522, 524, () => (GetCurrentSliderBar(523, __instance)!.value!.ToString()));
        private static string GetCurrentEyeColorValue(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Value", 522, 524, () => (GetCurrentSliderBar(524, __instance)!.value!.ToString()));

        private static string GetCurrentHairColor()
        {
            return GetCurrentColorAttributeValue("Hair color", 525, 527, () => GetNearestColorName(Game1.player.hairstyleColor.R, Game1.player.hairstyleColor.G, Game1.player.hairstyleColor.B));
        }

        private static string GetCurrentHairColorHue(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Hue", 525, 527, () => (GetCurrentSliderBar(525, __instance)!.value!.ToString()));
        private static string GetCurrentHairColorSaturation(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Saturation", 525, 527, () => (GetCurrentSliderBar(526, __instance)!.value!.ToString()));
        private static string GetCurrentHairColorValue(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Value", 525, 527, () => (GetCurrentSliderBar(527, __instance)!.value!.ToString()));

        private static string GetCurrentPantsColor()
        {
            return GetCurrentColorAttributeValue("Pants color", 528, 530, () => GetNearestColorName(Game1.player.pantsColor.R, Game1.player.pantsColor.G, Game1.player.pantsColor.B));
        }

        private static string GetCurrentPantsColorHue(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Hue", 528, 530, () => (GetCurrentSliderBar(528, __instance)!.value!.ToString()));
        private static string GetCurrentPantsColorSaturation(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Saturation", 528, 530, () => (GetCurrentSliderBar(529, __instance)!.value!.ToString()));
        private static string GetCurrentPantsColorValue(CharacterCustomization __instance) => GetCurrentColorAttributeValue("Value", 528, 530, () => (GetCurrentSliderBar(530, __instance)!.value!.ToString()));
    }
}
