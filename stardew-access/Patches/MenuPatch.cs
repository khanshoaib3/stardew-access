using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class MenuPatch
    {

        public static void TitleMenuPatch(TitleMenu __instance)
        {
            try
            {
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

                if(__instance.muteMusicButton.containsPoint(Game1.getMousePosition(true).X,Game1.getMousePosition(true).Y))
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

        public static void LoadGameMenuPatch(LoadGameMenu.SaveFileSlot __instance, LoadGameMenu ___menu, int i)
        {
            try
            {
                if (___menu.slotButtons[i].containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    if (__instance.Farmer == null)
                        return;

                    if (___menu.deleteButtons[i].containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                    {
                        // Fix for delete button hover text not narrating
                        ScreenReader.sayWithChecker($"Delete {__instance.Farmer.farmName} Farm", true);
                        return;
                    }

                    String farmerName = __instance.Farmer.Name;
                    String farmName = __instance.Farmer.farmName;
                    String money = __instance.Farmer.Money.ToString();
                    String hoursPlayed = Utility.getHoursMinutesStringFromMilliseconds(__instance.Farmer.millisecondsPlayed);
                    string dateStringForSaveGame = ((!__instance.Farmer.dayOfMonthForSaveGame.HasValue || 
                        !__instance.Farmer.seasonForSaveGame.HasValue || 
                        !__instance.Farmer.yearForSaveGame.HasValue) ? __instance.Farmer.dateStringForSaveGame : Utility.getDateStringFor(__instance.Farmer.dayOfMonthForSaveGame.Value, __instance.Farmer.seasonForSaveGame.Value, __instance.Farmer.yearForSaveGame.Value));

                    string toSpeak = $"{farmName} Farm, \t\n Farmer:{farmerName}, \t\nMoney:{money}, \t\nHours Played:{hoursPlayed}, \t\nDate:{dateStringForSaveGame}";

                    ScreenReader.sayWithChecker(toSpeak, true);
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void ExitPagePatch(ExitPage __instance)
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
    }
}
