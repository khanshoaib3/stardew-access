using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class BillboardPatch
    {
        private static string billboardQueryKey = "";

        internal static void DrawPatch(Billboard __instance, bool ___dailyQuestBoard)
        {
            try
            {
                if (___dailyQuestBoard)
                {
                    narrateDailyQuestBoard(__instance);
                }
                else
                {
                    narrateCallendar(__instance);
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void narrateCallendar(Billboard __instance)
        {
            for (int i = 0; i < __instance.calendarDays.Count; i++)
            {
                if (!__instance.calendarDays[i].containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                    continue;
                
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

                if (billboardQueryKey != toSpeak)
                {
                    billboardQueryKey = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
        }

        private static void narrateDailyQuestBoard(Billboard __instance)
        {
            if (Game1.questOfTheDay == null || Game1.questOfTheDay.currentObjective == null || Game1.questOfTheDay.currentObjective.Length == 0)
            {
                // No quests
                string toSpeak = "No quests for today!";
                if (billboardQueryKey != toSpeak)
                {
                    billboardQueryKey = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
            else
            {
                SpriteFont font = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? Game1.smallFont : Game1.dialogueFont);
                string description = Game1.parseText(Game1.questOfTheDay.questDescription, font, 640);
                string toSpeak = description;

                if (billboardQueryKey != toSpeak)
                {
                    billboardQueryKey = toSpeak;

                    // Snap to accept quest button
                    if (__instance.acceptQuestButton.visible)
                    {
                        toSpeak += "\t\n Left click to accept quest.";
                        __instance.acceptQuestButton.snapMouseCursorToCenter();
                    }

                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
        }

        internal static void Cleanup()
        {
            billboardQueryKey = "";
        }
    }
}
