using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class BillboardPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Billboard), nameof(Billboard.draw),
                    new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(BillboardPatch), nameof(BillboardPatch.DrawPatch))
            );
        }

        private static void DrawPatch(Billboard __instance, bool ___dailyQuestBoard)
        {
            try
            {
                if (___dailyQuestBoard)
                {
                    NarrateDailyQuestBoard(__instance);
                }
                else
                {
                    NarrateCalendar(__instance);
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in billboard menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void NarrateCalendar(Billboard __instance)
        {
            for (int i = 0; i < __instance.calendarDays.Count; i++)
            {
                if (!__instance.calendarDays[i].containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                    continue;

                object? translationTokens = new
                {
                    day = i + 1,
                    is_current = (Game1.dayOfMonth == i + 1) ? 1 : 0,
                    season = Game1.CurrentSeasonDisplayName,
                    year = Game1.year,
                    day_name = string.IsNullOrEmpty(__instance.calendarDays[i].name)
                        ? "null"
                        : __instance.calendarDays[i].name,
                    extra_info = string.IsNullOrEmpty(__instance.calendarDays[i].hoverText)
                        ? "null"
                        : __instance.calendarDays[i].hoverText
                };

                MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-billboard-calendar-day_info", true,
                    translationTokens);
                return;
            }
        }

        private static void NarrateDailyQuestBoard(Billboard __instance)
        {
            if (Game1.questOfTheDay == null || Game1.questOfTheDay.currentObjective == null ||
                Game1.questOfTheDay.currentObjective.Length == 0)
            {
                // No quests
                string toSpeak = Game1.content.LoadString("Strings\\UI:Billboard_NothingPosted");
                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            }
            else
            {
                // Snap to accept quest button
                if (__instance.acceptQuestButton.visible)
                {
                    MainClass.ScreenReader.MenuSuffixText =
                        Translator.Instance.Translate("menu-billboard-daily_quest-accept_quest-suffix",
                            TranslationCategory.Menu);
                    __instance.acceptQuestButton.snapMouseCursorToCenter();
                }

                MainClass.ScreenReader.SayWithMenuChecker(Game1.questOfTheDay.questDescription, true);
            }
        }
    }
}