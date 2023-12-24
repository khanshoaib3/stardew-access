using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace stardew_access.Patches
{
    // a.k.a. Journal Menu
    internal class QuestLogPatch : IPatch
    {
        internal static bool isNarratingQuestInfo = false;
        internal static bool firstTimeInIndividualQuest = true;

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(QuestLog), nameof(QuestLog.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(QuestLogPatch), nameof(QuestLogPatch.DrawPatch))
            );
        }

        private static void DrawPatch(QuestLog __instance, int ___questPage, List<List<IQuest>> ___pages, int ___currentPage, IQuest ____shownQuest, List<string> ____objectiveText)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (___questPage == -1)
                {
                    NarrateQuestList(__instance, ___pages, ___currentPage, x, y);
                }
                else
                {
                    NarrateIndividualQuest(__instance, ___currentPage, ____shownQuest, ____objectiveText, x, y);
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in quest log menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void NarrateQuestList(QuestLog __instance, List<List<IQuest>> ___pages, int ___currentPage, int x, int y)
        {
            string translationKey = "";
            object? translationTokens = null;

            if (!firstTimeInIndividualQuest) firstTimeInIndividualQuest = true;

            if (__instance.backButton != null && __instance.backButton.visible && __instance.backButton.containsPoint(x, y))
                translationKey = "common-ui-previous_page_button";
            else if (__instance.forwardButton != null && __instance.forwardButton.visible && __instance.forwardButton.containsPoint(x, y))
                translationKey = "common-ui-next_page_button";
            else if (__instance.upperRightCloseButton != null && __instance.upperRightCloseButton.visible && __instance.upperRightCloseButton.containsPoint(x, y))
                translationKey = "common-ui-close_menu_button";
            else
            {
                for (int i = 0; i < __instance.questLogButtons.Count; i++)
                {
                    if (___pages.Count <= 0 || ___pages[___currentPage].Count <= i)
                        continue;

                    if (!__instance.questLogButtons[i].containsPoint(x, y))
                        continue;

                    translationTokens = new
                    {
                        name = ___pages[___currentPage][i].GetName(),
                        days_left = ___pages[___currentPage][i].GetDaysLeft(),
                        is_completed = ___pages[___currentPage][i].ShouldDisplayAsComplete() ? 1 : 0
                    };

                    translationKey = "menu-quest_log-quest_brief";
                    break;
                }
            }

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
        }

        private static void NarrateIndividualQuest(QuestLog __instance, int ___currentPage, IQuest ____shownQuest, List<string> ____objectiveText, int x, int y)
        {
            if (____shownQuest == null)  return;

            bool isPrimaryInfoKeyPressed = MainClass.Config.PrimaryInfoKey.JustPressed();
            bool containsReward = __instance.HasReward() || __instance.HasMoneyReward();
            string description = ____shownQuest.GetDescription();
            string translationKey = "";

            if (firstTimeInIndividualQuest || (isPrimaryInfoKeyPressed && !isNarratingQuestInfo))
            {
                firstTimeInIndividualQuest = false;

                List<string> objectivesList = [];
                for (int j = 0; !____shownQuest.ShouldDisplayAsComplete() && j < ____objectiveText.Count; j++)
                {
                    string objective_info = ____objectiveText[j];
                    if (____shownQuest is SpecialOrder order)
                    {
                        OrderObjective order_objective = order.objectives[j];
                        if (order_objective.GetMaxCount() > 1 && order_objective.ShouldShowProgress())
                            objective_info = $"{order_objective.GetCount()}/{order_objective.GetMaxCount()} {objective_info}";
                    }

                    objectivesList.Add($"{j + 1}: {objective_info}");
                }

                object translationTokens = new
                {
                    is_completed = ____shownQuest.ShouldDisplayAsComplete() ? 1 : 0,
                    name = ____shownQuest.GetName(),
                    description = ____shownQuest.GetDescription(),
                    objectives_list = string.Join(", ", objectivesList),
                    days_left = ____shownQuest.GetDaysLeft(),
                    has_received_money = __instance.HasMoneyReward() ? 1 : 0,
                    received_money = ____shownQuest.GetMoneyReward(),
                };

                MainClass.ScreenReader.MenuPrefixNoQueryText = $"{Translator.Instance.Translate("menu-quest_log-quest_detail", translationTokens, TranslationCategory.Menu)}\n";
                MainClass.ScreenReader.PrevMenuQueryText = "";
                isNarratingQuestInfo = true;
                Task.Delay(200).ContinueWith(_ => { isNarratingQuestInfo = false; });
            }

            if (__instance.backButton != null && __instance.backButton.visible && __instance.backButton.containsPoint(x, y))
                translationKey = (___currentPage > 0) ? "common-ui-previous_page_button" : "common-ui-back_button";
            else if (__instance.forwardButton != null && __instance.forwardButton.visible && __instance.forwardButton.containsPoint(x, y))
                translationKey = "common-ui-next_page_button";
            else if (__instance.cancelQuestButton != null && __instance.cancelQuestButton.visible && __instance.cancelQuestButton.containsPoint(x, y))
                translationKey = "menu-quest_log-cancel_quest_button";
            else if (__instance.upperRightCloseButton != null && __instance.upperRightCloseButton.visible && __instance.upperRightCloseButton.containsPoint(x, y))
                translationKey = "common-ui-close_menu_button";
            else if (containsReward && __instance.rewardBox.containsPoint(x, y))
                translationKey = "menu-quest_log-reward_button";

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true);
        }

        internal static void Cleanup()
        {
            isNarratingQuestInfo = false;
            firstTimeInIndividualQuest = true;
        }
    }
}
