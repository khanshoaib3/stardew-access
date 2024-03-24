namespace stardew_access.Features;

using StardewModdingAPI.Events;
using System.Text.RegularExpressions;
using Translation;
using StardewValley;

internal class GameStateNarrator : FeatureBase
{
    private static Item? currentSlotItem;
    private static Item? previousSlotItem;

    private static GameLocation? currentLocation;
    private static GameLocation? previousLocation;

    private static string hudMessageQueryKey = "";
    private static bool isNarratingHudMessage = false;

    private static GameStateNarrator? instance;

    /// <summary>
    /// Stores the last 9 spoken hud messages.
    /// </summary>
    public static List<string> HudMessagesBuffer = new();

    public new static GameStateNarrator Instance
    {
        get
        {
            instance ??= new GameStateNarrator();
            return instance;
        }
    }

    public override void Update(object? sender, UpdateTickedEventArgs e)
    {
        NarrateCurrentSlot();
        NarrateCurrentLocation();
        RunHudMessageNarration();

        static async void RunHudMessageNarration()
        {
            if (!isNarratingHudMessage)
            {
                isNarratingHudMessage = true;
                NarrateHudMessages();
                await Task.Delay(300);
                isNarratingHudMessage = false;
            }
        }
    }

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
                Translator.Instance.Translate("feature-speak_selected_slot_item_name",
                    new { slot_item_name = currentSlotItem.DisplayName }),
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
                Translator.Instance.Translate("feature-speak_location_name",
                    new { location_name = currentLocation.GetParentLocation() is Farm  ? currentLocation.Name : currentLocation.DisplayName }),
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
                    string toSpeak = lastMessage.message;
                    var searchQuery = (Regex.Replace(toSpeak, @"[\d+]", string.Empty)).Trim();

                    if (hudMessageQueryKey != searchQuery)
                    {
                        hudMessageQueryKey = searchQuery;
                        MainClass.ScreenReader.Say(toSpeak, true);
                        HudMessagesBuffer.Add(toSpeak);
                        if (HudMessagesBuffer.Count > 9) HudMessagesBuffer.RemoveAt(0);
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
