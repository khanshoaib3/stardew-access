using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class BillboardPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                 original: AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new Type[] { typeof(SpriteBatch) }),
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
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void NarrateCalendar(Billboard __instance)
        {
            for (int i = 0; i < __instance.calendarDays.Count; i++)
            {
                if (!__instance.calendarDays[i].containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                    continue;
                
                string toSpeak = $"Day {i + 1}";
                string currentYearNMonth = $"of {Game1.CurrentSeasonDisplayName}, {Game1.content.LoadString("Strings\\UI:Billboard_Year", Game1.year)}";

                if (__instance.calendarDays[i].name.Length > 0)
                {
                    toSpeak += $", {__instance.calendarDays[i].name}";
                }
                if (__instance.calendarDays[i].hoverText.Length > 0)
                {
                    toSpeak += $", {__instance.calendarDays[i].hoverText}";
                }

                if (Game1.dayOfMonth == i + 1)
                    toSpeak = $"Current {toSpeak}";

                if (i == 0) toSpeak = $"{toSpeak} {currentYearNMonth}";
                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);

                return;
            }
        }

        private static void NarrateDailyQuestBoard(Billboard __instance)
        {
            if (Game1.questOfTheDay == null || Game1.questOfTheDay.currentObjective == null || Game1.questOfTheDay.currentObjective.Length == 0)
            {
                // No quests
                string toSpeak = "No quests for today!";
                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            }
            else
            {
                SpriteFont font = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? Game1.smallFont : Game1.dialogueFont);
                string description = Game1.parseText(Game1.questOfTheDay.questDescription, font, 640);
                string toSpeak = description;

                // Snap to accept quest button
                if (__instance.acceptQuestButton.visible)
                {
                    toSpeak += "\t\n Left click to accept quest.";
                    __instance.acceptQuestButton.snapMouseCursorToCenter();
                }

                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            }
        }
    }
}
