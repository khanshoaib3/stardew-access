using StardewValley;
using StardewModdingAPI;

namespace stardew_access.Game
{
    internal class CurrentPlayer
    {
        private static Farmer? player = null;

        CurrentPlayer()
        {
        }

        private static void initPlayer()
        {
            player = Game1.player;
        }

        public static int getHealth()
        {
            if(player == null)
                initPlayer();

            int maxHealth = player.maxHealth;
            int currentHealth = player.health;

            int healthPercentage = (int) (currentHealth * 100)/maxHealth;
            return healthPercentage;
        }

        public static int getStamina()
        {
            if (player == null)
                initPlayer();

            int maxStamina = player.maxStamina;
            int currentStamine = (int)player.stamina;

            int staminaPercentage = (int)(currentStamine * 100) / maxStamina;

            return staminaPercentage;
        }

        public static int getPositionX()
        {
            if (player == null)
                initPlayer();

            int x = (int)player.getTileLocation().X;
            return x;
        }

        public static int getPositionY()
        {
            if (player == null)
                initPlayer();

            int y = (int)player.getTileLocation().Y;
            return y;
        }
    }
}
