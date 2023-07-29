using StardewValley;
using StardewValley.Menus;
using static StardewValley.Menus.LoadGameMenu;

namespace stardew_access.Patches
{
    internal class LoadGameMenuPatch
    {
        private static string loadGameMenuQueryKey = "";
        private static bool firstTimeInMenu = true;

        internal static void DrawPatch(SaveFileSlot __instance, LoadGameMenu ___menu, int i)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                string toSpeak = "";

                if (!___menu.slotButtons[i].containsPoint(x, y)) return;
                if (__instance.Farmer == null) return;

                if (___menu.deleteButtons.Count > 0 && ___menu.deleteButtons[i].containsPoint(x, y))
                {
                    toSpeak = $"Delete {__instance.Farmer.farmName.Value} Farm";
                    if (loadGameMenuQueryKey != toSpeak)
                    {
                        loadGameMenuQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (___menu.deleteConfirmationScreen)
                {
                    if (firstTimeInMenu)
                    {
                        firstTimeInMenu = false;
                        toSpeak = "Really delete farm?";
                    }

                    if (___menu.okDeleteButton.containsPoint(x, y))
                    {
                        toSpeak = $"{toSpeak} Ok button";
                    }
                    else if (___menu.cancelDeleteButton.containsPoint(x, y))
                    {
                        toSpeak = $"{toSpeak} Cancel button";
                    }

                    if (loadGameMenuQueryKey != toSpeak)
                    {
                        loadGameMenuQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }

                String farmerName = __instance.Farmer.displayName;
                String farmName = __instance.Farmer.farmName.Value;
                String money = __instance.Farmer.Money.ToString();
                String hoursPlayed = Utility.getHoursMinutesStringFromMilliseconds(__instance.Farmer.millisecondsPlayed);
                string dateStringForSaveGame = ((!__instance.Farmer.dayOfMonthForSaveGame.HasValue ||
                    !__instance.Farmer.seasonForSaveGame.HasValue ||
                    !__instance.Farmer.yearForSaveGame.HasValue) ? __instance.Farmer.dateStringForSaveGame : Utility.getDateStringFor(__instance.Farmer.dayOfMonthForSaveGame.Value, __instance.Farmer.seasonForSaveGame.Value, __instance.Farmer.yearForSaveGame.Value));

                toSpeak = $"{farmName} Farm Selected, \t\n Farmer: {farmerName}, \t\nMoney: {money}, \t\nHours Played: {hoursPlayed}, \t\nDate: {dateStringForSaveGame}";

                if (loadGameMenuQueryKey != toSpeak)
                {
                    loadGameMenuQueryKey = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occured in load game menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup()
        {
            loadGameMenuQueryKey = "";
            firstTimeInMenu = true;
        }
    }
}
