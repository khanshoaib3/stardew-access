using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class SpecialOrdersBoardPatch
    {
        private static string specialOrdersBoardQueryKey = "";

        internal static void DrawPatch(SpecialOrdersBoard __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (__instance.acceptLeftQuestButton.visible && __instance.acceptLeftQuestButton.containsPoint(x, y))
                {
                    string toSpeak = GetSpecialOrderDetails(__instance.leftOrder);

                    toSpeak = $"Left Quest:\n\t{toSpeak}\n\tPress left click to accept this quest.";

                    Speak(toSpeak);
                    return;
                }

                if (__instance.acceptRightQuestButton.visible && __instance.acceptRightQuestButton.containsPoint(x, y))
                {
                    string toSpeak = GetSpecialOrderDetails(__instance.rightOrder);

                    toSpeak = $"Right Quest:\n\t{toSpeak}\n\tPress left click to accept this quest.";

                    Speak(toSpeak);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static string GetSpecialOrderDetails(SpecialOrder order)
        {
            int daysLeft = order.GetDaysLeft();
            string description = order.GetDescription();
            string objectiveDescription = "";
            string name = order.GetName();
            int moneyReward = order.GetMoneyReward();

            // Get each objectives
            for (int i = 0; i < order.GetObjectiveDescriptions().Count; i++)
            {
                objectiveDescription += order.GetObjectiveDescriptions()[i] + ", \n";
            }

            string toReturn = $"{name}\n\tDescription:{description}\n\tObjectives: {objectiveDescription}";

            if (order.IsTimedQuest())
            {
                toReturn = $"{toReturn}\n\tTime: {daysLeft} days";
            }

            if (order.HasMoneyReward())
            {
                toReturn = $"{toReturn}\n\tReward: {moneyReward}g";
            }

            return toReturn;
        }

        private static void Speak(string toSpeak)
        {
            if (specialOrdersBoardQueryKey == toSpeak) return;

            specialOrdersBoardQueryKey = toSpeak;
            MainClass.ScreenReader.Say(toSpeak, true);
        }

        internal static void Cleanup()
        {
            specialOrdersBoardQueryKey = "";
        }
    }
}
