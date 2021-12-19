using StardewValley;
using StardewModdingAPI;

namespace stardew_access.Game
{
    internal class CurrentPlayer
    {

        internal static int getHealth()
        {
            if(Game1.player == null)
                return 0;

            int maxHealth = Game1.player.maxHealth;
            int currentHealth = Game1.player.health;

            int healthPercentage = (int) (currentHealth * 100)/maxHealth;
            return healthPercentage;
        }

        internal static int getStamina()
        {
            if (Game1.player == null)
                return 0;

            int maxStamina = Game1.player.maxStamina;
            int currentStamine = (int)Game1.player.stamina;

            int staminaPercentage = (int)(currentStamine * 100) / maxStamina;

            return staminaPercentage;
        }

        internal static int getPositionX()
        {
            if (Game1.player == null)
                return 0;

            int x = (int)Game1.player.getTileLocation().X;
            return x;
        }

        internal static int getPositionY()
        {
            if (Game1.player == null)
                return 0;

            int y = (int)Game1.player.getTileLocation().Y;
            return y;
        }
    }
}
