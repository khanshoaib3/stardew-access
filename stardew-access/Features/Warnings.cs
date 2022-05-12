namespace stardew_access.Features
{
    public class Warnings
    {
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
                MainClass.ReadTileFeature.pause();
            }

            prevHour = hours;
        }

        public void checkForStamina()
        {
            if (MainClass.ModHelper == null)
                return;

            int stamina = CurrentPlayer.Stamina;
            string toSpeak = MainClass.ModHelper.Translation.Get("warnings.stamina", new { value = stamina });

            if ((stamina <= 50 && prevStamina > 50) || (stamina <= 25 && prevStamina > 25) || (stamina <= 10 && prevStamina > 10))
            {
                MainClass.ScreenReader.Say(toSpeak, true);
                // Pause the read tile feature to prevent interruption in warning message
                MainClass.ReadTileFeature.pause();
            }

            prevStamina = stamina;
        }

        public void checkForHealth()
        {
            if (MainClass.ModHelper == null)
                return;

            int health = CurrentPlayer.Health;
            string toSpeak = MainClass.ModHelper.Translation.Get("warnings.health", new { value = health });

            if ((health <= 50 && prevHealth > 50) || (health <= 25 && prevHealth > 25) || (health <= 10 && prevHealth > 10))
            {
                MainClass.ScreenReader.Say(toSpeak, true);
                // Pause the read tile feature to prevent interruption in warning message
                MainClass.ReadTileFeature.pause();
            }

            prevHealth = health;
        }
    }
}