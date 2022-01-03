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
                    ScreenReader.sayWithChecker(toSpeak, true);
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
                    ScreenReader.sayWithChecker(text, true);
                }

                if (TitleMenu.subMenu == null && toSpeak != "")
                    ScreenReader.sayWithChecker(toSpeak, true);
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
                            ScreenReader.sayWithChecker($"Delete {__instance.Farmer.farmName} Farm", true);
                            return;
                        }

                        if (___menu.deleteConfirmationScreen)
                        {
                            // Used diff. functions to narrate to prevent it from speaking the message again on selecting another button.
                            string message = "Really delete farm?";

                            ScreenReader.sayWithChecker(message, true);
                            if (___menu.okDeleteButton.containsPoint(x, y))
                            {
                                ScreenReader.sayWithMenuChecker("Ok Button", false);
                            }
                            else if (___menu.cancelDeleteButton.containsPoint(x, y))
                            {
                                ScreenReader.sayWithMenuChecker("Cancel Button", false);
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

                        ScreenReader.sayWithChecker(toSpeak, true);
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void NewGameMenuPatch(CharacterCustomization __instance, TextBox ___nameBox, TextBox ___farmnameBox,
            TextBox ___favThingBox, ClickableTextureComponent ___skipIntroButton, ClickableTextureComponent ___okButton,
            ClickableComponent ___backButton, ClickableTextureComponent ___randomButton, List<ClickableComponent> ___genderButtons,
            List<ClickableTextureComponent> ___farmTypeButtons, ClickableTextureComponent ___farmTypeNextPageButton, ClickableTextureComponent ___farmTypePreviousPageButton,
            List<ClickableTextureComponent> ___cabinLayoutButtons)
        {
            try
            {
                if (__instance.source != CharacterCustomization.Source.NewGame && __instance.source != CharacterCustomization.Source.HostNewFarm)
                    return;


                bool isNextArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right);
                bool isPrevArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left);

                if (isNextArrowPressed && !isRunning)
                {
                    _ = CycleThroughItems(true, ___nameBox, ___farmnameBox, ___favThingBox,
                        ___skipIntroButton, ___okButton, ___backButton,
                        ___randomButton, ___genderButtons, ___farmTypeButtons,
                        ___farmTypeNextPageButton, ___farmTypePreviousPageButton, ___cabinLayoutButtons);
                }
                else if (isPrevArrowPressed && !isRunning)
                {
                    _ = CycleThroughItems(false, ___nameBox, ___farmnameBox, ___favThingBox,
                        ___skipIntroButton, ___okButton, ___backButton, ___randomButton,
                        ___genderButtons, ___farmTypeButtons,
                        ___farmTypeNextPageButton, ___farmTypePreviousPageButton, ___cabinLayoutButtons);
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        private static async Task CycleThroughItems(bool increase, TextBox ___nameBox, TextBox ___farmnameBox,
            TextBox ___favThingBox, ClickableTextureComponent ___skipIntroButton, ClickableTextureComponent ___okButton,
            ClickableComponent ___backButton, ClickableTextureComponent ___randomButton, List<ClickableComponent> ___genderButtons,
            List<ClickableTextureComponent> ___farmTypeButtons, ClickableTextureComponent ___farmTypeNextPageButton, ClickableTextureComponent ___farmTypePreviousPageButton,
            List<ClickableTextureComponent> ___cabinLayoutButtons)
        {
            isRunning = true;
            if (increase)
            {
                saveGameIndex++;
                if (saveGameIndex > MAX_COMPONENTS)
                    saveGameIndex = 0;
            }
            else
            {
                saveGameIndex--;
                if (saveGameIndex < 0)
                    saveGameIndex = MAX_COMPONENTS;
            }

            await Task.Delay(200);

            switch (saveGameIndex)
            {
                case 0:
                    {
                        Rectangle bounds = new Rectangle(___nameBox.X, ___nameBox.Y, ___nameBox.Width, ___nameBox.Height);
                        Game1.input.SetMousePosition(bounds.Center.X, bounds.Center.Y);
                        ScreenReader.say("Enter Farmer's Name", false);
                    }
                    break;

                case 1:
                    {
                        Rectangle bounds = new Rectangle(___farmnameBox.X, ___farmnameBox.Y, ___farmnameBox.Width, ___farmnameBox.Height);
                        Game1.input.SetMousePosition(bounds.Center.X, bounds.Center.Y);
                        ScreenReader.say("Enter Farm's Name", false);
                    }
                    break;
                case 3:
                    {
                        Rectangle bounds = new Rectangle(___favThingBox.X, ___favThingBox.Y, ___favThingBox.Width, ___favThingBox.Height);
                        Game1.input.SetMousePosition(bounds.Center.X, bounds.Center.Y);
                        ScreenReader.say("Enter Favourite Thing", false);
                    }
                    break;
                case 4:
                    {
                        ___skipIntroButton.snapMouseCursor();
                        ScreenReader.say("Skip Intro Button", false);
                    }
                    break;
                case 5:
                    {
                        ___randomButton.snapMouseCursor();
                        ScreenReader.say("Random Skin Button", false);
                        break;
                    }
                case 6:
                    {
                        ___genderButtons[0].snapMouseCursor();
                        ScreenReader.say("Gender Male Button", false);
                        break;
                    }
                case 7:
                    {
                        ___genderButtons[1].snapMouseCursor();
                        ScreenReader.say("Gender Female Button", false);
                        break;
                    }
                case 8:
                    {
                        ___farmTypeButtons[0].snapMouseCursor();
                        ScreenReader.say(getFarmHoverText(___farmTypeButtons[0]), false);
                        break;
                    }
                case 9:
                    {
                        ___farmTypeButtons[1].snapMouseCursor();
                        ScreenReader.say(getFarmHoverText(___farmTypeButtons[1]), false);
                        break;
                    }
                case 10:
                    {
                        ___farmTypeButtons[2].snapMouseCursor();
                        ScreenReader.say(getFarmHoverText(___farmTypeButtons[2]), false);
                        break;
                    }
                case 11:
                    {
                        ___farmTypeButtons[3].snapMouseCursor();
                        ScreenReader.say(getFarmHoverText(___farmTypeButtons[3]), false);
                        break;
                    }
                case 12:
                    {
                        ___farmTypeButtons[4].snapMouseCursor();
                        ScreenReader.say(getFarmHoverText(___farmTypeButtons[4]), false);
                        break;
                    }
                case 13:
                    {
                        ___farmTypeButtons[5].snapMouseCursor();
                        ScreenReader.say(getFarmHoverText(___farmTypeButtons[5]), false);
                        break;
                    }
                case 14:
                    {
                        ___farmTypeButtons[6].snapMouseCursor();
                        ScreenReader.say(getFarmHoverText(___farmTypeButtons[6]), false);
                        break;
                    }
                case 15:
                    {
                        if (___farmTypeNextPageButton == null)
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

                        ___farmTypeNextPageButton.snapMouseCursor();
                        ScreenReader.say("Next Farm Type Page Button", false);
                        break;
                    }
                case 16:
                    {
                        if (___farmTypePreviousPageButton == null)
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

                        ___farmTypePreviousPageButton.snapMouseCursor();
                        ScreenReader.say("Previous Farm Type Page Button", false);
                        break;
                    }
                case 17:
                    {
                        if (___cabinLayoutButtons.Count <= 0)
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

                        ___cabinLayoutButtons[0].snapMouseCursor();
                        ScreenReader.say("Cabin layout nearby", false);
                        break;
                    }
                case 18:
                    {
                        if (___cabinLayoutButtons.Count <= 0)
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

                        ___cabinLayoutButtons[1].snapMouseCursor();
                        ScreenReader.say("Cabin layout separate", false);
                        break;
                    }
                case 19:
                    {
                        ___okButton.snapMouseCursor();
                        ScreenReader.say("Ok Button", false);
                    }
                    break;
                case 20:
                    {
                        ___backButton.snapMouseCursor();
                        ScreenReader.say("Back Button", false);
                    }
                    break;
            }

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
                    hoverTitle = null;
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
