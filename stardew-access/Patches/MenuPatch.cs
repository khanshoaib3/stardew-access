using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using static StardewValley.Menus.LoadGameMenu;

namespace stardew_access.Patches
{
    internal class MenuPatch
    {
        private static int saveGameIndex = -1, currentChatMessageIndex = 0;
        private static bool isRunning = false, isChatRunning = false;
        private static string currentLetterText = " ";
        private static string currentDailyQuestText = " ";
        private static string currentLevelUpTitle = " ";
        private const int MAX_COMPONENTS = 20;

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

        internal static void ChatBoxPatch(ChatBox __instance, List<ChatMessage> ___messages)
        {
            try
            {
                string toSpeak = " ";

                if (__instance.chatBox.Selected)
                {
                    bool isPrevArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.PageUp);
                    bool isNextArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.PageDown);

                    if (___messages.Count > 0)
                    {
                        #region To narrate previous and next chat messages
                        if (isNextArrowPressed && !isChatRunning)
                        {
                            _ = CycleThroughChatMessages(true, ___messages);
                        }
                        else if (isPrevArrowPressed && !isChatRunning)
                        {
                            _ = CycleThroughChatMessages(false, ___messages);
                        }
                        #endregion
                    }
                }
                else if (___messages.Count > 0)
                {
                    #region To narrate latest chat message
                    ___messages[___messages.Count - 1].message.ForEach(message =>
                    {
                        toSpeak += $"{message.message}, ";
                    });
                    if (toSpeak != " ")
                        ScreenReader.sayWithChatChecker(toSpeak, false);
                    #endregion
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        private static async Task CycleThroughChatMessages(bool increase, List<ChatMessage> ___messages)
        {
            isChatRunning = true;
            await Task.Delay(200);
            string toSpeak = " ";
            if (increase)
            {
                ++currentChatMessageIndex;
                if (currentChatMessageIndex > ___messages.Count - 1)
                {
                    currentChatMessageIndex = ___messages.Count - 1;
                }
            }
            else
            {
                --currentChatMessageIndex;
                if (currentChatMessageIndex < 0)
                {
                    currentChatMessageIndex = 0;
                }
            }
            ___messages[currentChatMessageIndex].message.ForEach(message =>
            {
                toSpeak += $"{message.message}, ";
            });

            ScreenReader.say(toSpeak, true);
            isChatRunning = false;
        }

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

        internal static void OptionsPagePatch(OptionsPage __instance)
        {
            try
            {
                int currentItemIndex = Math.Max(0, Math.Min(__instance.options.Count - 7, __instance.currentItemIndex));
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y;
                for (int i = 0; i < __instance.optionSlots.Count; i++)
                {
                    if (__instance.optionSlots[i].bounds.Contains(x, y) && currentItemIndex + i < __instance.options.Count && __instance.options[currentItemIndex + i].bounds.Contains(x - __instance.optionSlots[i].bounds.X, y - __instance.optionSlots[i].bounds.Y))
                    {
                        OptionsElement optionsElement = __instance.options[currentItemIndex + i];
                        string toSpeak = optionsElement.label;

                        if (optionsElement is OptionsButton)
                            toSpeak = $" {toSpeak} Button";
                        else if (optionsElement is OptionsCheckbox)
                            toSpeak = ((optionsElement as OptionsCheckbox).isChecked ? "Enabled" : "Disabled") + $" {toSpeak} Checkbox";
                        else if (optionsElement is OptionsDropDown)
                            toSpeak = $"{toSpeak} Dropdown, option {(optionsElement as OptionsDropDown).dropDownDisplayOptions[(optionsElement as OptionsDropDown).selectedOption]} selected";
                        else if (optionsElement is OptionsSlider)
                            toSpeak = $"{(optionsElement as OptionsSlider).value}% {toSpeak} Slider";
                        else if (optionsElement is OptionsPlusMinus)
                            toSpeak = $"{(optionsElement as OptionsPlusMinus).displayOptions[(optionsElement as OptionsPlusMinus).selected]} selected of {toSpeak}";
                        else if (optionsElement is OptionsInputListener)
                        {
                            string buttons = "";
                            (optionsElement as OptionsInputListener).buttonNames.ForEach(name => { buttons += $", {name}"; });
                            toSpeak = $"{toSpeak} is bound to {buttons}. Left click to change.";
                        }
                        else
                        {
                            if (toSpeak.Contains(":"))
                                toSpeak = toSpeak.Replace(":", "");

                            toSpeak = $"{toSpeak} Options:";
                        }

                        ScreenReader.sayWithChecker(toSpeak, true);
                        break;
                    }
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
                        if(___cabinLayoutButtons.Count <= 0)
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
            currentLevelUpTitle = " ";
        }
    }
}
