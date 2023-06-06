using stardew_access.Utils;

namespace stardew_access.Features
{
    /// <summary>
    /// Warns the player when their health or stamina/energy is low. Also warns when its past midnight.
    /// </summary>
    public class Warnings
    {
        // Store the previously checked value
        private int prevStamina;
        private int prevHealth;
        private int prevHour;

        public Warnings()
        {
            prevStamina = 100;
            prevHealth = 100;
            prevHour = 6;
        }

        public void update()
        {
            this.checkForHealth();
            this.checkForStamina();
            this.checkForTimeOfDay();
        }

        /// <summary>
        /// Warns when its past 12:00 am and 1:00 am
        /// </summary>
        private void checkForTimeOfDay()
        {
            if (MainClass.ModHelper == null)
                return;

            int hours = StardewValley.Game1.timeOfDay / 100;
            string toSpeak = MainClass.ModHelper.Translation.Get("warnings.time", new { value = CurrentPlayer.TimeOfDay });

            if (hours < 1 && prevHour > 2 || hours >= 1 && prevHour < 1)
            {
                MainClass.ScreenReader.Say(toSpeak, true);
                // Pause the read tile feature to prevent interruption in warning message
                MainClass.ReadTileFeature.pauseUntil();
            }

            prevHour = hours;
        }

        /// <summary>
        /// Warns when stamina reaches below 50, 25 and 10.
        /// </summary>
        public void checkForStamina()
        {
            if (MainClass.ModHelper == null)
                return;

            int stamina = CurrentPlayer.PercentStamina;
            string toSpeak = MainClass.ModHelper.Translation.Get("warnings.stamina", new { value = stamina });

            if ((stamina <= 50 && prevStamina > 50) || (stamina <= 25 && prevStamina > 25) || (stamina <= 10 && prevStamina > 10))
            {
                MainClass.ScreenReader.Say(toSpeak, true);
                // Pause the read tile feature to prevent interruption in warning message
                MainClass.ReadTileFeature.pauseUntil();
            }

            prevStamina = stamina;
        }

        /// <summary>
        /// Warns when health reaches below 50, 25 and 10.
        /// </summary>
        public void checkForHealth()
        {
            if (MainClass.ModHelper == null)
                return;

            int health = CurrentPlayer.PercentHealth;
            string toSpeak = MainClass.ModHelper.Translation.Get("warnings.health", new { value = health });

            if ((health <= 50 && prevHealth > 50) || (health <= 25 && prevHealth > 25) || (health <= 10 && prevHealth > 10))
            {
                MainClass.ScreenReader.Say(toSpeak, true);
                // Pause the read tile feature to prevent interruption in warning message
                MainClass.ReadTileFeature.pauseUntil();
            }

            prevHealth = health;
        }
    }
}