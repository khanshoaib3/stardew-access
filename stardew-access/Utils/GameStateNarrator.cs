using StardewValley;
using System.Text.RegularExpressions;
using stardew_access.Translation;

namespace stardew_access.Utils
{
    internal class GameStateNarrator
    {
        private static Item? currentSlotItem;
        private static Item? previousSlotItem;

        private static GameLocation? currentLocation;
        private static GameLocation? previousLocation;

        /// <summary>
        /// Narrates the currently selected slot item when changing the selected slot.
        /// </summary>
        public static void NarrateCurrentSlot()
        {
            try
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
            catch (Exception e)
            {
                Log.Error($"An error occurred in narrating the current slot item:\n{e.Message}\n{e.StackTrace}");
            }
        }


        /// <summary>
        /// Narrates the current location name when moving to a new location.
        /// </summary>
        public static void NarrateCurrentLocation()
        {
            try
            {
                currentLocation = Game1.currentLocation;

                if (currentLocation == null)
                    return;

                if (previousLocation == currentLocation)
                    return;

                previousLocation = currentLocation;
                MainClass.ScreenReader.Say(
                    Translator.Instance.Translate( "feature-speak_location_name", new { location_name = currentLocation.Name }),
                    true
                );
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in narrating the current location:\n{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Narrates the HUD messages.
        /// </summary>
        public static void NarrateHudMessages()
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

                        searchQuery = (Regex.Replace(toSpeak, @"[\d+]", string.Empty)).Trim();

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
                Log.Error($"An error occurred in narrating the hud messages:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
