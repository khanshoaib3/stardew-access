using StardewValley;
using Microsoft.Xna.Framework;

namespace stardew_access.Utils
{
    internal class CurrentPlayer
    {

        /// <summary>
        /// Returns the percentage health remaining of player.
        /// </summary>
        public static int PercentHealth
        {
            get
            {
                if (Game1.player == null)
                    return 0;

                return (CurrentHealth * 100) / Game1.player.maxHealth; ;
            }
        }

        /// <summary>
        /// Returns the total health player has currently
        /// </summary>
        public static int CurrentHealth
        {
            get
            {
                if (Game1.player == null)
                    return 0;

                return Game1.player.health;
            }
        }

        /// <summary>
        /// Returns the percentage stamine/energy remaining of player.
        /// </summary>
        public static int PercentStamina
        {
            get
            {
                if (Game1.player == null)
                    return 0;

                return (CurrentStamina * 100) / Game1.player.maxStamina.Value;
            }
        }

        /// <summary>
        /// Returns the total stamina player has currently
        /// </summary>
        public static int CurrentStamina
        {
            get
            {
                if (Game1.player == null)
                    return 0;

                return (int)Game1.player.stamina;
            }
        }

        /// <summary>
        /// Returns the tile location of the player
        /// </summary>
        public static Vector2 Position
        {
            get
            {
                if (Game1.player == null)
                    return Vector2.Zero;

                return Game1.player.Tile;
            }
        }

        /// <summary>
        /// Returns the X coordinate of the player
        /// </summary>
        public static int PositionX
        {
            get
            {
                if (Game1.player == null)
                    return 0;

                return (int)Position.X;
            }
        }

        /// <summary>
        /// Returns the Y coordinate of the player
        /// </summary>
        public static int PositionY
        {
            get
            {
                if (Game1.player == null)
                    return 0;

                return (int)Position.Y;
            }
        }

        /// <summary>
        /// Returns the time in the 12 hours format
        /// </summary>
        public static string TimeOfDay
        {
            get
            {
                int timeOfDay = Game1.timeOfDay;

                int minutes = timeOfDay % 100;
                int hours = timeOfDay / 100;
                string amOrpm = hours / 12 == 1 ? "PM" : "AM";
                hours %= 12;
                if (hours == 0) hours = 12;
                return $"{hours}:{minutes:00} {amOrpm}";
            }
        }

        /// <summary>
        /// Returns the current season
        /// </summary>
        public static string Season => Game1.CurrentSeasonDisplayName;

        /// <summary>
        /// Returns the current date of month
        /// </summary>
        public static int Date => Game1.dayOfMonth;

        /// <summary>
        /// Returns the current day of week
        /// </summary>
        /// <returns></returns>
        public static string Day => Game1.Date.DayOfWeek.ToString();

        /// <summary>
        /// Returns the amount of money the player has currently
        /// </summary>
        public static int Money
        {
            get
            {
                if (Game1.player == null)
                    return -1;

                return Game1.player.Money;
            }
        }

        /// <summary>
        /// Returns the tile position of the tile the player is facing
        /// </summary>
        /// <value></value>
        public static Vector2 FacingTile
        {
            get
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
}
