using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace stardew_access.Patches
{
    // a.k.a. Journal Menu
    internal class QuestLogPatch
    {
        internal static string questLogQuery = "";
        internal static bool isNarratingQuestInfo = false;
        internal static bool firstTimeInIndividualQuest = true;

        internal static void DrawPatch(QuestLog __instance, int ___questPage, List<List<IQuest>> ___pages, int ___currentPage, IQuest ____shownQuest, List<string> ____objectiveText)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (___questPage == -1)
                {
                    narrateQuestList(__instance, ___pages, ___currentPage, x, y);
                }
                else
                {
                    narrateIndividualQuest(__instance, ___currentPage, ____shownQuest, ____objectiveText, x, y);
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void narrateQuestList(QuestLog __instance, List<List<IQuest>> ___pages, int ___currentPage, int x, int y)
        {
            string toSpeak = "";

            if (!firstTimeInIndividualQuest) firstTimeInIndividualQuest = true;

            if (__instance.backButton != null && __instance.backButton.visible && __instance.backButton.containsPoint(x, y))
                toSpeak = "Previous page button";
            else if (__instance.forwardButton != null && __instance.forwardButton.visible && __instance.forwardButton.containsPoint(x, y))
                toSpeak = "Next page button";
            else if (__instance.upperRightCloseButton != null && __instance.upperRightCloseButton.visible && __instance.upperRightCloseButton.containsPoint(x, y))
                toSpeak = "Close menu button";
            else
            {
                for (int i = 0; i < __instance.questLogButtons.Count; i++)
                {
                    if (___pages.Count() <= 0 || ___pages[___currentPage].Count() <= i)
                        continue;

                    if (!__instance.questLogButtons[i].containsPoint(x, y))
                        continue;

                    string name = ___pages[___currentPage][i].GetName();
                    int daysLeft = ___pages[___currentPage][i].GetDaysLeft();
                    toSpeak = $"{name}";

                    if (daysLeft > 0 && ___pages[___currentPage][i].ShouldDisplayAsComplete())
                        toSpeak += $"\t\n {daysLeft} days left";

                    toSpeak += ___pages[___currentPage][i].ShouldDisplayAsComplete() ? " completed!" : "";
                    break;
                }
            }

            if (questLogQuery != toSpeak)
            {
                questLogQuery = toSpeak;
                MainClass.ScreenReader.Say(toSpeak, true);
            }
        }

        private static void narrateIndividualQuest(QuestLog __instance, int ___currentPage, IQuest ____shownQuest, List<string> ____objectiveText, int x, int y)
        {
            bool isPrimaryInfoKeyPressed = MainClass.Config.PrimaryInfoKey.JustPressed();
            bool containsReward = __instance.HasReward() || __instance.HasMoneyReward();
            string description = Game1.parseText(____shownQuest.GetDescription(), Game1.dialogueFont, __instance.width - 128);
            string title = ____shownQuest.GetName();
            string toSpeak = "";
            string extra = "";

            if (firstTimeInIndividualQuest || (isPrimaryInfoKeyPressed && !isNarratingQuestInfo))
            {
                if (firstTimeInIndividualQuest)
                    toSpeak = "Back button";

                if (____shownQuest.ShouldDisplayAsComplete())
                {
                    #region Quest completed menu

                    extra = $"Quest: {title} Completed!";

                    if (__instance.HasMoneyReward())
                        extra += $"you recieved {____shownQuest.GetMoneyReward()}g";

                    #endregion
                }
                else
                {
                    #region Quest in-complete menu
                    extra = $"Title: {title}. \t\n Description: {description}";

                    for (int j = 0; j < ____objectiveText.Count; j++)
                    {
                        string parsed_text = Game1.parseText(____objectiveText[j], width: __instance.width - 192, whichFont: Game1.dialogueFont);
                        if (____shownQuest != null && ____shownQuest is SpecialOrder)
                        {
                            OrderObjective order_objective = ((SpecialOrder)____shownQuest).objectives[j];
                            if (order_objective.GetMaxCount() > 1 && order_objective.ShouldShowProgress())
                                parsed_text += "\n\t" + order_objective.GetCount() + " of " + order_objective.GetMaxCount() + " completed";
                        }

                        extra += $"\t\nOrder {j + 1}: {parsed_text} \t\n";
                    }

                    if (____shownQuest != null)
                    {
                        int daysLeft = ____shownQuest.GetDaysLeft();

                        if (daysLeft > 0)
                            extra += $"\t\n{daysLeft} days left.";
                    }
                    #endregion
                }

                isNarratingQuestInfo = true;
                Task.Delay(200).ContinueWith(_ => { isNarratingQuestInfo = false; });
                questLogQuery = "";
            }

            if (!firstTimeInIndividualQuest)
            {
                if (__instance.backButton != null && __instance.backButton.visible && __instance.backButton.containsPoint(x, y))
                    toSpeak = (___currentPage > 0) ? "Previous page button" : "Back button";
                else if (__instance.forwardButton != null && __instance.forwardButton.visible && __instance.forwardButton.containsPoint(x, y))
                    toSpeak = "Next page button";
                else if (__instance.cancelQuestButton != null && __instance.cancelQuestButton.visible && __instance.cancelQuestButton.containsPoint(x, y))
                    toSpeak = "Cancel quest button";
                else if (__instance.upperRightCloseButton != null && __instance.upperRightCloseButton.visible && __instance.upperRightCloseButton.containsPoint(x, y))
                    toSpeak = "Close menu button";
                else if (containsReward && __instance.rewardBox.containsPoint(x, y))
                    toSpeak = "Left click to collect reward";
            }

            if (firstTimeInIndividualQuest || (questLogQuery != toSpeak))
            {
                questLogQuery = toSpeak;
                MainClass.ScreenReader.Say(extra + " \n\t" + toSpeak, true);

                if (firstTimeInIndividualQuest) firstTimeInIndividualQuest = false;
            }
        }

        internal static void Cleaup()
        {
            questLogQuery = "";
            isNarratingQuestInfo = false;
            firstTimeInIndividualQuest = true;
        }
    }
}
