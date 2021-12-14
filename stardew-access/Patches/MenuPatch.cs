using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace stardew_access.Patches
{
    internal class MenuPatch
    {
        private static int saveGameIndex = -1;
        private static bool isRunning = false;
        private static string currentLetterText = " ";
        private static string currentDailyQuestText = " ";

        internal static void BillboardPatch(Billboard __instance, bool ___dailyQuestBoard)
        {
            if (!___dailyQuestBoard)
            {
                #region Callender
                /*b.DrawString(Game1.dialogueFont, Utility.getSeasonNameFromNumber(Utility.getSeasonNumber(Game1.currentSeason)), new Vector2(xPositionOnScreen + 160, yPositionOnScreen + 80), Game1.textColor);
                        b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:Billboard_Year", Game1.year), new Vector2(xPositionOnScreen + 448, yPositionOnScreen + 80), Game1.textColor);
                        for (int i = 0; i < calendarDays.Count; i++)
                        {
                            if (calendarDays[i].name.Length > 0)
                            {
                                if (calendarDays[i].name.Equals(nightMarketLocalized))
                                {
                                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(calendarDays[i].bounds.X + 12, (float)(calendarDays[i].bounds.Y + 60) - Game1.dialogueButtonScale / 2f), new Rectangle(346, 392, 8, 8), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 1f);
                                }
                                else
                                {
                                    Utility.drawWithShadow(b, billboardTexture, new Vector2(calendarDays[i].bounds.X + 40, (float)(calendarDays[i].bounds.Y + 56) - Game1.dialogueButtonScale / 2f), new Rectangle(1 + (int)(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600.0 / 100.0) * 14, 398, 14, 12), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 1f);
                                }
                            }
                            if (calendarDays[i].hoverText.Length > 0)
                            {
                                b.Draw(calendarDays[i].texture, new Vector2(calendarDays[i].bounds.X + 48, calendarDays[i].bounds.Y + 28), calendarDays[i].sourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                            }
                            if (_upcomingWeddings.ContainsKey(calendarDays[i]))
                            {
                                foreach (string item in _upcomingWeddings[calendarDays[i]])
                                {
                                    _ = item;
                                    b.Draw(Game1.mouseCursors2, new Vector2(calendarDays[i].bounds.Right - 56, calendarDays[i].bounds.Top - 12), new Rectangle(112, 32, 16, 14), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                                }
                            }
                            if (Game1.dayOfMonth > i + 1)
                            {
                                b.Draw(Game1.staminaRect, calendarDays[i].bounds, Color.Gray * 0.25f);
                            }
                            else if (Game1.dayOfMonth == i + 1)
                            {
                                int offset = (int)(4f * Game1.dialogueButtonScale / 8f);
                                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(379, 357, 3, 3), calendarDays[i].bounds.X - offset, calendarDays[i].bounds.Y - offset, calendarDays[i].bounds.Width + offset * 2, calendarDays[i].bounds.Height + offset * 2, Color.Blue, 4f, drawShadow: false);
                            }
                        }*/ 
                #endregion
            }
            else
            {
                #region Daily Quest Board
                if (Game1.questOfTheDay == null || Game1.questOfTheDay.currentObjective == null || Game1.questOfTheDay.currentObjective.Length == 0)
                {
                    // No quests
                    string toSpeak = "No quests for today!";
                    if(currentDailyQuestText != toSpeak)
                    {
                        currentDailyQuestText = toSpeak;
                        ScreenReader.say(toSpeak, true);
                    }
                }
                else
                {
                    SpriteFont font = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? Game1.smallFont : Game1.dialogueFont);
                    string description = Game1.parseText(Game1.questOfTheDay.questDescription, font, 640);
                    string toSpeak = description;

                    if (currentDailyQuestText != toSpeak)
                    {
                        currentDailyQuestText = toSpeak;

                        // Snap to accept quest button
                        if (__instance.acceptQuestButton.visible)
                        {
                            toSpeak += "\t\n Left click to accept quest.";
                            __instance.acceptQuestButton.snapMouseCursorToCenter();
                        }

                        ScreenReader.say(toSpeak, true);
                    }
                } 
                #endregion
            }
        }

        internal static void QuestLogPatch(QuestLog __instance, int ___questPage, List<List<IQuest>> ___pages, int ___currentPage, IQuest ____shownQuest, List<string> ____objectiveText)
        {
            try
            {
                bool snapMouseToRewardBox = false;

                if (___questPage == -1)
                {
                    #region Quest Lists
                    for (int i = 0; i < __instance.questLogButtons.Count; i++)
                    {
                        if (___pages.Count() > 0 && ___pages[___currentPage].Count() > i)
                        {
                            string name = ___pages[___currentPage][i].GetName();
                            int daysLeft = ___pages[___currentPage][i].GetDaysLeft();
                            string toSpeak = $"{name} quest";

                            if (daysLeft > 0 && ___pages[___currentPage][i].ShouldDisplayAsComplete())
                                toSpeak += $"\t\n {daysLeft} days left";

                            toSpeak += ___pages[___currentPage][i].ShouldDisplayAsComplete() ? " completed!" : "";
                            if (__instance.questLogButtons[i].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
                            {
                                ScreenReader.sayWithChecker(toSpeak, true);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Individual quest
                    string description = Game1.parseText(____shownQuest.GetDescription(), Game1.dialogueFont, __instance.width - 128);
                    string title = ____shownQuest.GetName();
                    string toSpeak = " ";
                    if (____shownQuest.ShouldDisplayAsComplete())
                    {
                        #region Quest completed menu

                        toSpeak = $"Quest: {title} Completed!";

                        if (__instance.HasReward())
                        {
                            snapMouseToRewardBox = true;
                            if (__instance.HasMoneyReward())
                            {
                                toSpeak += $"you recieved {____shownQuest.GetMoneyReward()}g";
                            }

                            toSpeak += "... left click to collect reward";
                        }

                        #endregion
                    }
                    else
                    {
                        #region Quest in-complete menu
                        toSpeak = $"Title: {title}. \t\n Description: {description}";

                        for (int j = 0; j < ____objectiveText.Count; j++)
                        {
                            if (____shownQuest != null)
                            {
                                _ = ____shownQuest is SpecialOrder;
                            }
                            string parsed_text = Game1.parseText(____objectiveText[j], width: __instance.width - 192, whichFont: Game1.dialogueFont);

                            toSpeak += $"\t\nOrder {j + 1}: {parsed_text} \t\n";
                        }

                        int daysLeft = ____shownQuest.GetDaysLeft();

                        if (daysLeft > 0)
                            toSpeak += $"\t\n{daysLeft} days left.";
                        #endregion
                    }

                    // Move mouse to reward button
                    if (snapMouseToRewardBox)
                        __instance.rewardBox.snapMouseCursorToCenter();

                    ScreenReader.sayWithChecker(toSpeak, true);
                    #endregion
                }
            }
            catch (Exception e)
            {

                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void LetterViewerMenuPatch(LetterViewerMenu __instance)
        {
            try
            {
                if (!__instance.IsActive())
                    return;

                #region Texts in the letter
                string title = __instance.mailTitle;
                string message = __instance.mailMessage[__instance.page];

                string toSpeak = $"{title} \t\n\t {message}.";

                if (__instance.ShouldShowInteractable())
                {
                    if (__instance.moneyIncluded > 0)
                    {
                        string moneyText = Game1.content.LoadString("Strings\\UI:LetterViewer_MoneyIncluded", __instance.moneyIncluded);
                        toSpeak += $"\t\n\t ,Included money: {moneyText}";
                    }
                    else if (__instance.learnedRecipe != null && __instance.learnedRecipe.Length > 0)
                    {
                        string recipeText = Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipe", __instance.cookingOrCrafting);
                        toSpeak += $"\t\n\t ,Learned Recipe: {recipeText}";
                    }
                }

                if (currentLetterText != toSpeak)
                {
                    currentLetterText = toSpeak;

                    // snap mouse to accept quest button
                    if (__instance.acceptQuestButton.visible)
                    {
                        toSpeak += "\t\n Left click to accept quest.";
                        __instance.acceptQuestButton.snapMouseCursorToCenter();
                    }
                    ScreenReader.say(toSpeak, false);
                }
                #endregion

                #region Narrate items given in the mail
                if (__instance.ShouldShowInteractable())
                {
                    foreach (ClickableComponent c in __instance.itemsToGrab)
                    {
                        string name = c.name;
                        string label = c.label;

                        if (c.containsPoint(Game1.getMousePosition().X, Game1.getMousePosition().Y))
                            ScreenReader.sayWithChecker($"Grab: {name} \t\n {label}", false);
                    }
                }
                #endregion
            }
            catch (Exception e)
            {

                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void TitleMenuPatch(TitleMenu __instance)
        {
            try
            {
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

        internal static void LoadGameMenuPatch(LoadGameMenu.SaveFileSlot __instance, LoadGameMenu ___menu, int i)
        {
            try
            {
                if (___menu.slotButtons[i].containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    if (__instance.Farmer == null)
                        return;

                    if (___menu.deleteButtons[i].containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                    {
                        // Fix for delete button hover text not narrating
                        ScreenReader.sayWithChecker($"Delete {__instance.Farmer.farmName} Farm", true);
                        return;
                    }

                    String farmerName = __instance.Farmer.Name;
                    String farmName = __instance.Farmer.farmName;
                    String money = __instance.Farmer.Money.ToString();
                    String hoursPlayed = Utility.getHoursMinutesStringFromMilliseconds(__instance.Farmer.millisecondsPlayed);
                    string dateStringForSaveGame = ((!__instance.Farmer.dayOfMonthForSaveGame.HasValue ||
                        !__instance.Farmer.seasonForSaveGame.HasValue ||
                        !__instance.Farmer.yearForSaveGame.HasValue) ? __instance.Farmer.dateStringForSaveGame : Utility.getDateStringFor(__instance.Farmer.dayOfMonthForSaveGame.Value, __instance.Farmer.seasonForSaveGame.Value, __instance.Farmer.yearForSaveGame.Value));

                    string toSpeak = $"{farmName} Farm Selected, \t\n Farmer:{farmerName}, \t\nMoney:{money}, \t\nHours Played:{hoursPlayed}, \t\nDate:{dateStringForSaveGame}";

                    ScreenReader.sayWithChecker(toSpeak, true);
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void NewGameMenuPatch(CharacterCustomization __instance, TextBox ___nameBox, TextBox ___farmnameBox, TextBox ___favThingBox, ClickableTextureComponent ___skipIntroButton, ClickableTextureComponent ___okButton, ClickableComponent ___backButton)
        {
            try
            {
                if (__instance.source != CharacterCustomization.Source.NewGame)
                    return;


                bool isNextArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right);
                bool isPrevArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left);
                if (isNextArrowPressed && !isRunning)
                {
                    _ = CycleThroughItems(true, ___nameBox, ___farmnameBox, ___favThingBox, ___skipIntroButton, ___okButton, ___backButton);
                }
                else if (isPrevArrowPressed && !isRunning)
                {
                    _ = CycleThroughItems(false, ___nameBox, ___farmnameBox, ___favThingBox, ___skipIntroButton, ___okButton, ___backButton);
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        private static async Task CycleThroughItems(bool increase, TextBox ___nameBox, TextBox ___farmnameBox, TextBox ___favThingBox, ClickableTextureComponent ___skipIntroButton, ClickableTextureComponent ___okButton, ClickableComponent ___backButton)
        {
            isRunning = true;
            if (increase)
            {
                saveGameIndex++;
                if (saveGameIndex > 6)
                    saveGameIndex = 0;
            }
            else
            {
                saveGameIndex--;
                if (saveGameIndex < 0)
                    saveGameIndex = 6;
            }

            await Task.Delay(200);

            switch (saveGameIndex)
            {
                case 0:
                    {
                        Rectangle bounds = new Rectangle(___nameBox.X, ___nameBox.Y, ___nameBox.Width, ___nameBox.Height);
                        Game1.input.SetMousePosition(bounds.Center.X, bounds.Center.Y);
                        ScreenReader.say("Enter Farmer's Name", true);
                    }
                    break;

                case 1:
                    {
                        Rectangle bounds = new Rectangle(___farmnameBox.X, ___farmnameBox.Y, ___farmnameBox.Width, ___farmnameBox.Height);
                        Game1.input.SetMousePosition(bounds.Center.X, bounds.Center.Y);
                        ScreenReader.say("Enter Farm's Name", true);
                    }
                    break;
                case 3:
                    {
                        Rectangle bounds = new Rectangle(___favThingBox.X, ___favThingBox.Y, ___favThingBox.Width, ___favThingBox.Height);
                        Game1.input.SetMousePosition(bounds.Center.X, bounds.Center.Y);
                        ScreenReader.say("Enter Favourite Thing", true);
                    }
                    break;
                case 4:
                    {
                        ___skipIntroButton.snapMouseCursor();
                        ScreenReader.say("Skip Intro Button", true);
                    }
                    break;
                case 5:
                    {
                        ___okButton.snapMouseCursor();
                        ScreenReader.say("Ok Button", true);
                    }
                    break;
                case 6:
                    {
                        ___backButton.snapMouseCursor();
                        ScreenReader.say("Back Button", true);
                    }
                    break;
            }

            isRunning = false;
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

        internal static void resetGlobalVars()
        {
            currentLetterText = " ";
            currentDailyQuestText = " ";
        }
    }
}
