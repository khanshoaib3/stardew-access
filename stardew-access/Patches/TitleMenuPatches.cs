using StardewValley;
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
                    MainClass.GetScreenReader().SayWithChecker(toSpeak, true);
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
                    MainClass.GetScreenReader().SayWithChecker(text, true);
                }

                if (TitleMenu.subMenu == null && toSpeak != "")
                    MainClass.GetScreenReader().SayWithChecker(toSpeak, true);
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
                            MainClass.GetScreenReader().SayWithChecker($"Delete {__instance.Farmer.farmName} Farm", true);
                            return;
                        }

                        if (___menu.deleteConfirmationScreen)
                        {
                            // Used diff. functions to narrate to prevent it from speaking the message again on selecting another button.
                            string message = "Really delete farm?";

                            MainClass.GetScreenReader().SayWithChecker(message, true);
                            if (___menu.okDeleteButton.containsPoint(x, y))
                            {
                                MainClass.GetScreenReader().SayWithMenuChecker("Ok Button", false);
                            }
                            else if (___menu.cancelDeleteButton.containsPoint(x, y))
                            {
                                MainClass.GetScreenReader().SayWithMenuChecker("Cancel Button", false);
                            }
                            return;
                        }

                        String farmerName = __instance.Farmer.displayName;
                        String farmName = __instance.Farmer.farmName;
                        String money = __instance.Farmer.Money.ToString();
                        String hoursPlayed = Utility.getHoursMinutesStringFromMilliseconds(__instance.Farmer.millisecondsPlayed);
                        string dateStringForSaveGame = ((!__instance.Farmer.dayOfMonthForSaveGame.HasValue ||
                            !__instance.Farmer.seasonForSaveGame.HasValue ||
                            !__instance.Farmer.yearForSaveGame.HasValue) ? __instance.Farmer.dateStringForSaveGame : Utility.getDateStringFor(__instance.Farmer.dayOfMonthForSaveGame.Value, __instance.Farmer.seasonForSaveGame.Value, __instance.Farmer.yearForSaveGame.Value));

                        string toSpeak = $"{farmName} Farm Selected, \t\n Farmer:{farmerName}, \t\nMoney:{money}, \t\nHours Played:{hoursPlayed}, \t\nDate:{dateStringForSaveGame}";

                        MainClass.GetScreenReader().SayWithChecker(toSpeak, true);
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
        ClickableComponent ___startingCabinsLabel, ClickableComponent ___difficultyModifierLabel)
        {
            try
            {
                bool isNextArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right);
                bool isPrevArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left);

                if (__instance.backButton.containsPoint != null && __instance.backButton.visible && __instance.backButton.containsPoint((int)Game1.getMouseX(true), (int)Game1.getMouseY(true)))
                {
                    // Perform Left Click
                    if (MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed())
                    {
                        Game1.activeClickableMenu.receiveLeftClick(Game1.getMouseX(true), Game1.getMouseY(true));
                    }
                }

                if (isNextArrowPressed && !isRunning)
                {
                    isRunning = true;
                    CycleThroughItems(true, __instance, ___skipIntro, ___startingCabinsLabel, ___difficultyModifierLabel);
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
                }
                else if (isPrevArrowPressed && !isRunning)
                {
                    isRunning = true;
                    CycleThroughItems(false, __instance, ___skipIntro, ___startingCabinsLabel, ___difficultyModifierLabel);
                    Task.Delay(200).ContinueWith(_ => { isRunning = false; });
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
                buttons.Add(__instance.nameBoxCC, "Enter Farmer's Name");

            if (__instance.farmnameBoxCC != null && __instance.farmnameBoxCC.visible)
                buttons.Add(__instance.farmnameBoxCC, "Enter Farm's Name");

            if (__instance.favThingBoxCC != null && __instance.favThingBoxCC.visible)
                buttons.Add(__instance.favThingBoxCC, "Enter Favourite Thing");

            if (__instance.petPortraitBox.HasValue) // Cannot get petButtons like with others
            {
                ClickableComponent petPrev = __instance.getComponentWithID(511);
                buttons.Add(petPrev, "Previous pet: " + getPetName(-1, __instance.isModifyingExistingPet));

                ClickableComponent petNext = __instance.getComponentWithID(510);
                buttons.Add(petNext, "Next pet: " + getPetName(+1, __instance.isModifyingExistingPet));
            }

            if (__instance.randomButton != null && __instance.randomButton.visible)
                buttons.Add(__instance.randomButton, "Random Skin Button");

            if (__instance.genderButtons.Count > 0)
            {
                buttons.Add(__instance.genderButtons[0], "Gender: Male Button");
                buttons.Add(__instance.genderButtons[1], "Gender: Female Button");
            }
            #endregion

            #region Farm layout related
            if (__instance.farmTypeButtons.Count > 0)
            {
                buttons.Add(__instance.farmTypeButtons[0], getFarmHoverText(__instance.farmTypeButtons[0]));
                buttons.Add(__instance.farmTypeButtons[1], getFarmHoverText(__instance.farmTypeButtons[1]));
                buttons.Add(__instance.farmTypeButtons[2], getFarmHoverText(__instance.farmTypeButtons[2]));
                buttons.Add(__instance.farmTypeButtons[3], getFarmHoverText(__instance.farmTypeButtons[3]));
                buttons.Add(__instance.farmTypeButtons[4], getFarmHoverText(__instance.farmTypeButtons[4]));
                buttons.Add(__instance.farmTypeButtons[5], getFarmHoverText(__instance.farmTypeButtons[5]));
                buttons.Add(__instance.farmTypeButtons[6], getFarmHoverText(__instance.farmTypeButtons[6]));
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
                MainClass.GetScreenReader().Say(toSpeak, true);
            }
        }

        private static string getPetName(int change, bool isModifyingExistingPet)
        {
            Game1.player.whichPetBreed += change;
            if (Game1.player.whichPetBreed >= 3)
            {
                Game1.player.whichPetBreed = 0;
                if (!isModifyingExistingPet)
                {
                    Game1.player.catPerson = !Game1.player.catPerson;
                }
            }
            else if (Game1.player.whichPetBreed < 0)
            {
                Game1.player.whichPetBreed = 2;
                if (!isModifyingExistingPet)
                {
                    Game1.player.catPerson = !Game1.player.catPerson;
                }
            }

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
