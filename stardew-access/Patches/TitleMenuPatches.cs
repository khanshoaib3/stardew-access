using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using static StardewValley.Menus.LoadGameMenu;

namespace stardew_access.Patches
{
    internal class TitleMenuPatches
    {
        private static int saveGameIndex = -1;
        private static bool isRunning = false;
        private const int MAX_COMPONENTS = 20;

        internal static void CoopMenuPatch(CoopMenu __instance, CoopMenu.Tab ___currentTab)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y;
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
                    MainClass.screenReader.SayWithChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
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
                    if (component.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                    {
                        string name = component.name;
                        string label = component.label;
                        toSpeak = $"{name} {label} Button";
                    }
                });

                if (__instance.muteMusicButton.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    toSpeak = "Mute Music Button";
                }

                if (__instance.aboutButton.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    toSpeak = "About Button";
                }

                if (__instance.languageButton.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    toSpeak = "Language Button";
                }

                if (__instance.windowedButton.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    toSpeak = "Fullscreen toggle Button";
                }

                if (TitleMenu.subMenu != null && __instance.backButton.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    string text = "Back Button";
                    MainClass.screenReader.SayWithChecker(text, true);
                }

                if (TitleMenu.subMenu == null && toSpeak != "")
                    MainClass.screenReader.SayWithChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void LoadGameMenuPatch(SaveFileSlot __instance, LoadGameMenu ___menu, int i)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y;
                if (___menu.slotButtons[i].containsPoint(x, y))
                {
                    if (__instance.Farmer != null)
                    {
                        #region Farms
                        if (___menu.deleteButtons.Count > 0 && ___menu.deleteButtons[i].containsPoint(x, y))
                        {
                            MainClass.screenReader.SayWithChecker($"Delete {__instance.Farmer.farmName} Farm", true);
                            return;
                        }

                        if (___menu.deleteConfirmationScreen)
                        {
                            // Used diff. functions to narrate to prevent it from speaking the message again on selecting another button.
                            string message = "Really delete farm?";

                            MainClass.screenReader.SayWithChecker(message, true);
                            if (___menu.okDeleteButton.containsPoint(x, y))
                            {
                                MainClass.screenReader.SayWithMenuChecker("Ok Button", false);
                            }
                            else if (___menu.cancelDeleteButton.containsPoint(x, y))
                            {
                                MainClass.screenReader.SayWithMenuChecker("Cancel Button", false);
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

                        MainClass.screenReader.SayWithChecker(toSpeak, true);
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void NewGameMenuPatch(CharacterCustomization __instance, bool ___skipIntro)
        {
            try
            {
                bool isNextArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right);
                bool isPrevArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left);

                if (isNextArrowPressed && !isRunning)
                {
                    _ = CycleThroughItems(true, __instance, ___skipIntro);
                }
                else if (isPrevArrowPressed && !isRunning)
                {
                    _ = CycleThroughItems(false, __instance, ___skipIntro);
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        private static async Task CycleThroughItems(bool increase, CharacterCustomization __instance, bool ___skipIntro)
        {
            isRunning = true;
            string toSpeak = " ";

            if (increase)
            {
                saveGameIndex++;
                if (saveGameIndex > MAX_COMPONENTS)
                    saveGameIndex = 1;
            }
            else
            {
                saveGameIndex--;
                if (saveGameIndex < 1)
                    saveGameIndex = MAX_COMPONENTS;
            }


            switch (saveGameIndex)
            {
                case 1:
                    {
                        #region Skip if button is not available
                        if (!__instance.nameBoxCC.visible)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 2;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case MAX_COMPONENTS;
                            }
                        }
                        #endregion

                        __instance.nameBoxCC.snapMouseCursorToCenter();
                        toSpeak = "Enter Farmer's Name";
                    }
                    break;

                case 2:
                    {
                        #region Skip if button is not available
                        if (!__instance.farmnameBoxCC.visible)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 3;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 1;
                            }
                        }
                        #endregion

                        __instance.farmnameBoxCC.snapMouseCursorToCenter();
                        toSpeak = "Enter Farm's Name";
                    }
                    break;
                case 3:
                    {
                        #region Skip if button is not available
                        if (!__instance.favThingBoxCC.visible)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 4;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 2;
                            }
                        }
                        #endregion

                        __instance.favThingBoxCC.snapMouseCursorToCenter();
                        toSpeak = "Enter Favourite Thing";
                    }
                    break;
                case 4:
                    {
                        #region Skip if button is not available
                        if (!__instance.skipIntroButton.visible)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 5;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 3;
                            }
                        }
                        #endregion

                        __instance.skipIntroButton.snapMouseCursor();
                        toSpeak =  (___skipIntro?"Enabled":"Disabled") + " Skip Intro Button";
                    }
                    break;
                case 5:
                    {
                        #region Skip if button is not available
                        if (!__instance.randomButton.visible)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 6;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 5;
                            }
                        }
                        #endregion

                        __instance.randomButton.snapMouseCursor();
                        toSpeak = "Random Skin Button";
                        break;
                    }
                case 6:
                    {
                        #region Skip if button is not available
                        if (__instance.genderButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 8;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 6;
                            }
                        }
                        #endregion

                        __instance.genderButtons[0].snapMouseCursor();
                        toSpeak = "Gender Male Button";
                        break;
                    }
                case 7:
                    {
                        #region Skip if button is not available
                        if (__instance.genderButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 8;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 6;
                            }
                        }
                        #endregion

                        __instance.genderButtons[1].snapMouseCursor();
                        toSpeak = "Gender Female Button";
                        break;
                    }
                case 8:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 9;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 7;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[0].snapMouseCursor();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[0]);
                        break;
                    }
                case 9:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 10;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 8;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[1].snapMouseCursor();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[1]);
                        break;
                    }
                case 10:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 11;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 9;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[2].snapMouseCursor();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[2]);
                        break;
                    }
                case 11:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 12;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 10;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[3].snapMouseCursor();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[3]);
                        break;
                    }
                case 12:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 13;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 11;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[4].snapMouseCursor();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[4]);
                        break;
                    }
                case 13:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 14;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 12;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[5].snapMouseCursor();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[5]);
                        break;
                    }
                case 14:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 15;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 13;
                            }
                        }
                        #endregion

                        __instance.farmTypeButtons[6].snapMouseCursor();
                        toSpeak = getFarmHoverText(__instance.farmTypeButtons[6]);
                        break;
                    }
                case 15:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypeNextPageButton == null)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 16;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 14;
                            }
                        }
                        #endregion

                        __instance.farmTypeNextPageButton.snapMouseCursor();
                        toSpeak = "Next Farm Type Page Button";
                        break;
                    }
                case 16:
                    {
                        #region Skip if button is not available
                        if (__instance.farmTypePreviousPageButton == null)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 17;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 15;
                            }
                        }
                        #endregion

                        __instance.farmTypePreviousPageButton.snapMouseCursor();
                        toSpeak = "Previous Farm Type Page Button";
                        break;
                    }
                case 17:
                    {
                        #region Skip if button is not available
                        if (__instance.cabinLayoutButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 18;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 16;
                            }
                        }
                        #endregion

                        __instance.cabinLayoutButtons[0].snapMouseCursor();
                        toSpeak = "Cabin layout nearby";
                        break;
                    }
                case 18:
                    {
                        #region Skip if button is not available
                        if (__instance.cabinLayoutButtons.Count <= 0)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 19;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 17;
                            }
                        }
                        #endregion

                        __instance.cabinLayoutButtons[1].snapMouseCursor();
                        toSpeak = "Cabin layout separate";
                        break;
                    }
                case 19:
                    {
                        #region Skip if button is not available
                        if (!__instance.okButton.visible)
                        {
                            if (increase)
                            {
                                ++saveGameIndex;
                                goto case 18;
                            }
                            else
                            {
                                --saveGameIndex;
                                goto case 20;
                            }
                        }
                        #endregion

                        __instance.okButton.snapMouseCursor();
                        toSpeak = "Ok Button";
                    }
                    break;
                case 20:
                    {
                        #region Exit if button is not available
                        if (!__instance.backButton.visible)
                        {
                            break;
                        }
                        #endregion

                        __instance.backButton.snapMouseCursor();
                        toSpeak = "Back Button";
                    }
                    break;
            }

            if(toSpeak!=" ")
            {
                MainClass.screenReader.Say(toSpeak, true);
            }

            await Task.Delay(200);
            isRunning = false;
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
