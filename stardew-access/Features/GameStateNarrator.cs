using StardewValley;
using System.Text.RegularExpressions;

namespace stardew_access.Features
{
    internal class GameStateNarrator
    {
        private static Item? currentSlotItem;
        private static Item? previousSlotItem;

        private static GameLocation? currentLocation;
        private static GameLocation? previousLocation;

        // Narrates current slected slot name
        public static void narrateCurrentSlot()
        {
            currentSlotItem = Game1.player.CurrentItem;

            if (currentSlotItem == null)
                return;

            if (previousSlotItem == currentSlotItem)
                return;

            previousSlotItem = currentSlotItem;
            MainClass.ScreenReader.Say($"{currentSlotItem.DisplayName} Selected", true);
        }

        // Narrates current location's name
        public static void narrateCurrentLocation()
        {
            currentLocation = Game1.currentLocation;

            if (currentLocation == null)
                return;

            if (previousLocation == currentLocation)
                return;

            previousLocation = currentLocation;
            MainClass.ScreenReader.Say($"{currentLocation.Name} Entered", true);
        }

        public static void narrateHudMessages()
        {
            try
            {
                if (Game1.hudMessages.Count > 0)
                {
                    int lastIndex = Game1.hudMessages.Count - 1;
                    HUDMessage lastMessage = Game1.hudMessages[lastIndex];
                    if (!lastMessage.noIcon)
                    {
                        string toSpeak = lastMessage.Message;
                        string searchQuery = toSpeak;

                        searchQuery = Regex.Replace(toSpeak, @"[\d+]", string.Empty);
                        searchQuery.Trim();


                        if (MainClass.hudMessageQueryKey != searchQuery)
                        {
                            MainClass.hudMessageQueryKey = searchQuery;

                            MainClass.ScreenReader.Say(toSpeak, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate hud messages:\n{e.Message}\n{e.StackTrace}");
            }

        }
    }
}
