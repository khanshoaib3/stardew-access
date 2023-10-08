using stardew_access.Utils;
using stardew_access.Translation;
using StardewModdingAPI.Events;

namespace stardew_access.Features
{
    /// <summary>
    /// Warns the player when their health or stamina/energy is low. Also warns when its past midnight.
    /// </summary>
    public class Warnings : FeatureBase
    {
        // Store the previously checked value
        private int prevStamina;
        private int prevHealth;
        private int prevHour;

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
            prevStamina = 100;
            prevHealth = 100;
            prevHour = 6;
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

            if (hours < 1 && prevHour > 2 || hours >= 1 && prevHour < 1)
            {
                MainClass.ScreenReader.Say(
                    Translator.Instance.Translate("feature-warnings-time", new { value = timeOfDay }),
                    true
                );
                // Pause the read tile feature to prevent interruption in warning message
                ReadTile.Instance.PauseUntil();
            }

            prevHour = hours;
        }

        /// <summary>
        /// Warns when stamina reaches below 50, 25 and 10.
        /// </summary>
        public void CheckForStamina()
        {
            if (MainClass.ModHelper == null)
                return;

            int stamina = CurrentPlayer.PercentStamina;

            if (stamina == prevStamina)
                return; // Return early if stamina hasn't changed

            if ((stamina <= 50 && prevStamina > 50) || (stamina <= 25 && prevStamina > 25) || (stamina <= 10 && prevStamina > 10))
            {
                MainClass.ScreenReader.Say(
                    Translator.Instance.Translate("feature-warnings-stamina", new { value = stamina }),
                    true
                );
                // Pause the read tile feature to prevent interruption in warning message
                ReadTile.Instance.PauseUntil();
            }

            prevStamina = stamina;
        }

        /// <summary>
        /// Warns when health reaches below 50, 25 and 10.
        /// </summary>
        public void CheckForHealth()
        {
            if (MainClass.ModHelper == null)
                return;

            int health = CurrentPlayer.PercentHealth;

            if (health == prevHealth)
                return; // Return early if health hasn't changed

            if ((health <= 50 && prevHealth > 50) || (health <= 25 && prevHealth > 25) || (health <= 10 && prevHealth > 10))
            {
                MainClass.ScreenReader.Say(
                    Translator.Instance.Translate("feature-warnings-health", new { value = health }),
                    true
                );
                // Pause the read tile feature to prevent interruption in warning message
                ReadTile.Instance.PauseUntil();
            }

            prevHealth = health;
        }
    }
}
