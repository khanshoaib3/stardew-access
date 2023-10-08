namespace stardew_access.Features;

using Utils;
using Translation;
using StardewModdingAPI.Events;

/// <summary>
/// Warns the player when their health or stamina/energy is low. Also warns when its past midnight.
/// </summary>
public class Warnings : FeatureBase
{
    // Store the previously checked value
    private int _prevStamina;
    private int _prevHealth;
    private int _prevHour;

    private static Warnings? instance;
    public new static Warnings Instance
    {
        get
        {
            instance ??= new Warnings();
            return instance;
        }
    }

    public Warnings()
    {
        _prevStamina = 100;
        _prevHealth = 100;
        _prevHour = 6;
    }

    public override void Update(object? sender, UpdateTickedEventArgs e)
    {
        if (!MainClass.Config.Warning) return;
            
        this.CheckForHealth();
        this.CheckForStamina();
        this.CheckForTimeOfDay();
    }

    /// <summary>
    /// Warns when its past 12:00 am and 1:00 am
    /// </summary>
    private void CheckForTimeOfDay()
    {
        if (MainClass.ModHelper == null)
            return;

        int hours = StardewValley.Game1.timeOfDay / 100;
        var timeOfDay = CurrentPlayer.TimeOfDay;

        if (hours < 1 && _prevHour > 2 || hours >= 1 && _prevHour < 1)
        {
            MainClass.ScreenReader.Say(
                Translator.Instance.Translate("feature-warnings-time", new { value = timeOfDay }),
                true
            );
            // Pause the read tile feature to prevent interruption in warning message
            ReadTile.Instance.PauseUntil();
        }

        _prevHour = hours;
    }

    /// <summary>
    /// Warns when stamina reaches below 50, 25 and 10.
    /// </summary>
    public void CheckForStamina()
    {
        if (MainClass.ModHelper == null)
            return;

        int stamina = CurrentPlayer.PercentStamina;

        if (stamina == _prevStamina)
            return; // Return early if stamina hasn't changed

        if ((stamina <= 50 && _prevStamina > 50) || (stamina <= 25 && _prevStamina > 25) || (stamina <= 10 && _prevStamina > 10))
        {
            MainClass.ScreenReader.Say(
                Translator.Instance.Translate("feature-warnings-stamina", new { value = stamina }),
                true
            );
            // Pause the read tile feature to prevent interruption in warning message
            ReadTile.Instance.PauseUntil();
        }

        _prevStamina = stamina;
    }

    /// <summary>
    /// Warns when health reaches below 50, 25 and 10.
    /// </summary>
    public void CheckForHealth()
    {
        if (MainClass.ModHelper == null)
            return;

        int health = CurrentPlayer.PercentHealth;

        if (health == _prevHealth)
            return; // Return early if health hasn't changed

        if ((health <= 50 && _prevHealth > 50) || (health <= 25 && _prevHealth > 25) || (health <= 10 && _prevHealth > 10))
        {
            MainClass.ScreenReader.Say(
                Translator.Instance.Translate("feature-warnings-health", new { value = health }),
                true
            );
            // Pause the read tile feature to prevent interruption in warning message
            ReadTile.Instance.PauseUntil();
        }

        _prevHealth = health;
    }
}