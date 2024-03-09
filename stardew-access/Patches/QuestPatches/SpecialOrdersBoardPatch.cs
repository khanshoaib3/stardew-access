using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;
using StardewValley.SpecialOrders;

namespace stardew_access.Patches
{
    internal class SpecialOrdersBoardPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                    original: AccessTools.Method(typeof(SpecialOrdersBoard), nameof(SpecialOrdersBoard.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(SpecialOrdersBoardPatch), nameof(SpecialOrdersBoardPatch.DrawPatch))
            );
        }

        private static void DrawPatch(SpecialOrdersBoard __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (__instance.acceptLeftQuestButton.visible && __instance.acceptLeftQuestButton.containsPoint(x, y))
                {
                    MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-special_orders_board-accept_button", true, new
                    {
                        is_left_quest = 1,
                        quest_details = GetQuestDetails(__instance.leftOrder)
                    });
                    return;
                }

                if (__instance.acceptRightQuestButton.visible && __instance.acceptRightQuestButton.containsPoint(x, y))
                {
                    MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-special_orders_board-accept_button", true, new
                    {
                        is_left_quest = 0,
                        quest_details = GetQuestDetails(__instance.rightOrder)
                    });
                    return;
                }

                if (Game1.player.team.acceptedSpecialOrderTypes.Contains(__instance.GetOrderType()))
                {
                    if (__instance.leftOrder.questState.Value == SpecialOrderStatus.InProgress)
                    {
                        MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-special_orders_board-quest_in_progress", true, new
                        {
                            quest_details = GetQuestDetails((IsInProgress(__instance.leftOrder, __instance)) ? __instance.leftOrder : __instance.rightOrder)
                        });
                    }
                    return;
                }

                // FIXME Does not indicate completed status with this logic
                /*
                if (Game1.player.team.completedSpecialOrders.ContainsKey(__instance.GetOrderType()))
                {
                    if (__instance.leftOrder.questState.Value == SpecialOrder.QuestState.Complete || __instance.rightOrder.questState.Value == SpecialOrder.QuestState.Complete)
                    {
                        MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-special_orders_board-quest_completed", true, new
                        {
                            name = (__instance.leftOrder.questState.Value == SpecialOrder.QuestState.Complete) ? __instance.leftOrder.GetName() : __instance.rightOrder.GetName()
                        });
                    }
                }
                */
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in special orders board patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static string GetQuestDetails(SpecialOrder order) => Translator.Instance.Translate(
            "menu-special_orders_board-quest_details", new
            {
                name = order.GetName(),
                description = order.GetDescription(),
                objectives_list = string.Join(", ", order.GetObjectiveDescriptions()),
                is_timed = order.IsTimedQuest() ? 1 : 0,
                days = order.GetDaysLeft(),
                has_money_reward = order.HasMoneyReward() ? 1 : 0,
                money = order.GetMoneyReward()
            },
            TranslationCategory.Menu);

        private static bool IsInProgress(SpecialOrder order, SpecialOrdersBoard __instance)
        {
            bool flag1 = false;
            bool flag2 = false;
            foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
            {
                if (specialOrder.questState.Value == SpecialOrderStatus.InProgress)
                {
                    foreach (SpecialOrder availableSpecialOrder in Game1.player.team.availableSpecialOrders)
                    {
                        if (!(availableSpecialOrder.orderType.Value != __instance.GetOrderType()) &&
                            specialOrder.questKey.Value == availableSpecialOrder.questKey.Value)
                        {
                            if (order.questKey != specialOrder.questKey)
                                flag1 = true;
                            flag2 = true;
                            break;
                        }
                    }

                    if (flag2)
                        break;
                }
            }

            if (!flag2 && Game1.player.team.acceptedSpecialOrderTypes.Contains(__instance.GetOrderType()))
                flag1 = true;

            return !flag1;
        }
    }
}
