using StardewValley;
using System.Text.RegularExpressions;

namespace stardew_access.Features
{
    internal class Other
    {
        private static Item? currentSlotItem;
        private static Item? previousSlotItem;

        private static GameLocation? currentLocation;
        private static GameLocation? previousLocation;

        /// <summary>
        /// Narrates the currently selected slot item when changing the selected slot.
        /// </summary>
        public static void narrateCurrentSlot()
        {
            currentSlotItem = Game1.player.CurrentItem;

            if (currentSlotItem == null)
                return;

            if (previousSlotItem == currentSlotItem)
                return;

            previousSlotItem = currentSlotItem;
            MainClass.ScreenReader.Say(
                Translator.Instance.Translate( "feature-speak_selected_slot_item_name", new { slot_item_name = currentSlotItem.DisplayName }),
                true
            );
        }


        /// <summary>
        /// Narrates the current location name when moving to a new location.
        /// </summary>
        public static void narrateCurrentLocation()
        {
            currentLocation = Game1.currentLocation;

            if (currentLocation == null)
                return;

            if (previousLocation == currentLocation)
                return;

            previousLocation = currentLocation;
            MainClass.ScreenReader.Say($"{currentLocation.Name} Entered", true);
            MainClass.ScreenReader.Say(
                Translator.Instance.Translate( "feature-speak_location_name", new { currentLocation.Name }),
                true
            );
        }

        /// <summary>
        /// Narrates the HUD messages.
        /// </summary>
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
