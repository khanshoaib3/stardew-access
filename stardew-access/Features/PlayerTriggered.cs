using stardew_access.Translation;
using stardew_access.Utils;
using StardewModdingAPI.Events;
using StardewValley;

namespace stardew_access.Features;

public class PlayerTriggered : FeatureBase
{
    private static PlayerTriggered? instance;
    public new static PlayerTriggered Instance
    {
        get
        {
            instance ??= new PlayerTriggered();
            return instance;
        }
    }

    public override void Update(object? sender, UpdateTickedEventArgs e)
    { }

    public override bool OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        // Exit if in a menu
        if (Game1.activeClickableMenu != null)
        {
            #if DEBUG
            Log.Verbose("OnButtonPressed: returning due to 'Game1.activeClickableMenu' not being null AKA in a menu");
            #endif
            return false;
        }

        // TODO i18n: Add missing translations

        // Narrate Current Location
        if (MainClass.Config.LocationKey.JustPressed())
        {
            MainClass.ScreenReader.Say(Game1.currentLocation.Name, true);
            return true;
        }

        // Narrate Position
        if (MainClass.Config.PositionKey.JustPressed())
        {
            string toSpeak = MainClass.Config.VerboseCoordinates
                ? $"X: {CurrentPlayer.PositionX}, Y: {CurrentPlayer.PositionY}"
                : $"{CurrentPlayer.PositionX}, {CurrentPlayer.PositionY}";
            MainClass.ScreenReader.Say(toSpeak, true);
            return true;
        }

        // Narrate health and stamina
        if (MainClass.Config.HealthNStaminaKey.JustPressed())
        {
            if (MainClass.ModHelper == null)
                return true;

            // TODO unify translation keys
            string toSpeak = MainClass.Config.HealthNStaminaInPercentage
                ? Translator.Instance.Translate(
                    "feature-speak_health_n_stamina-in_percentage_format",
                    new
                    {
                        health = CurrentPlayer.PercentHealth,
                        stamina = CurrentPlayer.PercentStamina
                    }
                )
                : Translator.Instance.Translate(
                    "feature-speak_health_n_stamina-in_normal_format",
                    new
                    {
                        health = CurrentPlayer.CurrentHealth,
                        stamina = CurrentPlayer.CurrentStamina
                    }
                );

            MainClass.ScreenReader.Say(toSpeak, true);
            return true;
        }

        // Narrate money at hand
        if (MainClass.Config.MoneyKey.JustPressed())
        {
            MainClass.ScreenReader.Say($"You have {CurrentPlayer.Money}g", true);
            return true;
        }

        // Narrate time and season
        if (MainClass.Config.TimeNSeasonKey.JustPressed())
        {
            MainClass.ScreenReader.Say(
                $"Time is {CurrentPlayer.TimeOfDay} and it is {CurrentPlayer.Day} {CurrentPlayer.Date} of {CurrentPlayer.Season}",
                true);
            return true;
        }

        return false;
    }
    
}