using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace stardew_access.Patches
{
    internal class MenuPatch
    {
        private static string currentLetterText = " ";
        private static string currentDailyQuestText = " ";
        private static string currentLevelUpTitle = " ";

        internal static void LanguageSelectionMenuPatch(LanguageSelectionMenu __instance)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position

                if(__instance.nextPageButton != null && __instance.nextPageButton.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker($"Next Page Button", true);
                    return;
                }

                if (__instance.previousPageButton != null && __instance.previousPageButton.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker($"Previous Page Button", true);
                    return;
                }

                for(int i=0; i<__instance.languages.Count; i++)
                {
                    if(__instance.languages[i].containsPoint(x, y))
                    {
                        ScreenReader.sayWithMenuChecker($"{__instance.languageList[i]} Button", true);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void MineElevatorMenuPatch(List<ClickableComponent> ___elevators)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position
                for (int i=0; i<___elevators.Count; i++)
                {
                    if(___elevators[i].containsPoint(x, y))
                    {
                        ScreenReader.sayWithMenuChecker($"{___elevators[i].name} level", true);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void NamingMenuPatch(NamingMenu __instance, string title, TextBox ___textBox)
        {
            try
            {
                __instance.textBoxCC.snapMouseCursor();
                ___textBox.SelectMe();
                string toSpeak = $"{title}";

                ScreenReader.sayWithChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void ConfirmationDialogPatch(ConfirmationDialog __instance, string ___message)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y;

                ScreenReader.sayWithMenuChecker(___message, true);
                if(__instance.okButton.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker("Ok Button", false);
                } else if (__instance.cancelButton.containsPoint(x, y))
                {
                    ScreenReader.sayWithMenuChecker("Cancel Button", false);
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void LevelUpMenuPatch(LevelUpMenu __instance, List<int> ___professionsToChoose, List<string> ___leftProfessionDescription, List<string> ___rightProfessionDescription, List<string> ___extraInfoForLevel, List<CraftingRecipe> ___newCraftingRecipes, string ___title)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y;
                string leftProfession = " ", rightProfession = " ", extraInfo = " ", newCraftingRecipe = " ", toSpeak = " ";

                if (!__instance.informationUp)
                {
                    return;
                }
                if (__instance.isProfessionChooser)
                {
                    if (___professionsToChoose.Count() == 0)
                    {
                        return;
                    }
                    for (int j = 0; j < ___leftProfessionDescription.Count; j++)
                    {
                        leftProfession += ___leftProfessionDescription[j] + ", ";
                    }
                    for (int i = 0; i < ___rightProfessionDescription.Count; i++)
                    {
                        rightProfession += ___rightProfessionDescription[i] + ", ";
                    }

                    if (__instance.leftProfession.containsPoint(x, y))
                        toSpeak = $"Selected: {leftProfession} Left click to choose.";

                    if (__instance.rightProfession.containsPoint(x, y))
                        toSpeak = $"Selected: {rightProfession} Left click to choose.";
                }
                else
                {
                    foreach (string s2 in ___extraInfoForLevel)
                    {
                        extraInfo += s2 + ", ";
                    }
                    foreach (CraftingRecipe s in ___newCraftingRecipes)
                    {
                        string cookingOrCrafting = Game1.content.LoadString("Strings\\UI:LearnedRecipe_" + (s.isCookingRecipe ? "cooking" : "crafting"));
                        string message = Game1.content.LoadString("Strings\\UI:LevelUp_NewRecipe", cookingOrCrafting, s.DisplayName);

                        newCraftingRecipe += $"{message}, ";
                    }

                    if (__instance.okButton.containsPoint(x, y))
                    {
                        toSpeak = $"{___title} {extraInfo} {newCraftingRecipe}. Left click to close.";
                    }
                }

                if (toSpeak != " ")
                    ScreenReader.sayWithMenuChecker(toSpeak, true);
                else if (__instance.isProfessionChooser && currentLevelUpTitle != $"{___title}. Select a new profession.")
                {
                    ScreenReader.sayWithMenuChecker($"{___title}. Select a new profession.", true);
                    currentLevelUpTitle = $"{___title}. Select a new profession.";
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void ShippingMenuPatch(ShippingMenu __instance, List<int> ___categoryTotals)
        {
            try
            {
                if (__instance.currentPage == -1)
                {
                    int total = ___categoryTotals[5];
                    string toSpeak;
                    if (__instance.okButton.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                    {
                        toSpeak = $"{total}g in total. Press left mouse button to save.";
                        ScreenReader.sayWithChecker(toSpeak, true);
                    }
                    for (int i = 0; i < __instance.categories.Count; i++)
                    {
                        if (__instance.categories[i].containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                        {
                            toSpeak = $"Money recieved from {__instance.getCategoryName(i)}: {___categoryTotals[i]}g.";
                            ScreenReader.sayWithChecker(toSpeak, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void BillboardPatch(Billboard __instance, bool ___dailyQuestBoard)
        {
            try
            {
                if (!___dailyQuestBoard)
                {
                    #region Callender
                    for (int i = 0; i < __instance.calendarDays.Count; i++)
                    {
                        if (__instance.calendarDays[i].containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                        {
                            string toSpeak = $"Day {i + 1}";

                            if (__instance.calendarDays[i].name.Length > 0)
                            {
                                toSpeak += $", {__instance.calendarDays[i].name}";
                            }
                            if (__instance.calendarDays[i].hoverText.Length > 0)
                            {
                                toSpeak += $", {__instance.calendarDays[i].hoverText}";
                            }

                            if (Game1.dayOfMonth == i + 1)
                                toSpeak += $", Current";

                            ScreenReader.sayWithChecker(toSpeak, true);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Daily Quest Board
                    if (Game1.questOfTheDay == null || Game1.questOfTheDay.currentObjective == null || Game1.questOfTheDay.currentObjective.Length == 0)
                    {
                        // No quests
                        string toSpeak = "No quests for today!";
                        if (currentDailyQuestText != toSpeak)
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
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
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

        internal static void resetGlobalVars()
        {
            currentLetterText = " ";
            currentDailyQuestText = " ";
            currentLevelUpTitle = " ";
        }
    }
}
