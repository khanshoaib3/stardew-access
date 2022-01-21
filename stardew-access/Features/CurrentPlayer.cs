using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework;

namespace stardew_access.Game
{
    internal class CurrentPlayer
    {

        public static int getHealth()
        {
            if(Game1.player == null)
                return 0;

            int maxHealth = Game1.player.maxHealth;
            int currentHealth = Game1.player.health;

            int healthPercentage = (int) (currentHealth * 100)/maxHealth;
            return healthPercentage;
        }

        public static int getStamina()
        {
            if (Game1.player == null)
                return 0;

            int maxStamina = Game1.player.maxStamina;
            int currentStamine = (int)Game1.player.stamina;

            int staminaPercentage = (int)(currentStamine * 100) / maxStamina;

            return staminaPercentage;
        }

        public static int getPositionX()
        {
            if (Game1.player == null)
                return 0;

            int x = (int)Game1.player.getTileLocation().X;
            return x;
        }

        public static int getPositionY()
        {
            if (Game1.player == null)
                return 0;

            int y = (int)Game1.player.getTileLocation().Y;
            return y;
        }

        public static string getTimeOfDay()
        {
            int timeOfDay = Game1.timeOfDay;

            int minutes = timeOfDay % 100;
            int hours = timeOfDay / 100;
            string amOrpm = "A M";
            if(hours>=12)
            {
                amOrpm = "P M";
                if (hours > 12)
                    hours -= 12;
            }

            return $"{hours}:{minutes} {amOrpm}";
        }

        public static string getSeason()
        {
            return Game1.CurrentSeasonDisplayName;
        }

        public static int getDate()
        {
            return Game1.dayOfMonth;
        }

        public static string getDay()
        {
            return Game1.Date.DayOfWeek.ToString();
        }

        public static int getMoney()
        {
            if(Game1.player == null)
                return -1;

            return Game1.player.Money;
        }

        public static Vector2 getNextTile()
        {
            int x = Game1.player.GetBoundingBox().Center.X;
            int y = Game1.player.GetBoundingBox().Center.Y;

            int offset = 64;

            switch (Game1.player.FacingDirection)
            {
                case 0:
                    y -= offset;
                    break;
                case 1:
                    x += offset;
                    break;
                case 2:
                    y += offset;
                    break;
                case 3:
                    x -= offset;
                    break;
            }

            x /= Game1.tileSize;
            y /= Game1.tileSize;
            return new Vector2(x, y);
        }
    }
}
