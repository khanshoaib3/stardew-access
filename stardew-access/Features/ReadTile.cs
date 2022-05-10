using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace stardew_access.Features
{
    public class ReadTile
    {
        public static bool isReadingTile = false;
        public static Vector2 prevTile;

        public ReadTile()
        {
            isReadingTile = false;
        }

        public static void run(bool manuallyTriggered = false, bool playersPosition = false)
        {
            try
            {
                Vector2 tile;

                #region Get Tile
                int x, y;
                if (!playersPosition)
                {
                    // Grab tile
                    tile = CurrentPlayer.getNextTile();
                }
                else
                {
                    // Player's standing tile
                    tile = CurrentPlayer.getPosition();
                }
                x = (int)tile.X;
                y = (int)tile.Y;
                #endregion

                if (Context.IsPlayerFree)
                {
                    if (!manuallyTriggered && prevTile != tile)
                    {
                        if (MainClass.ScreenReader != null)
                            MainClass.ScreenReader.PrevTextTile = " ";
                    }

                    bool isColliding = TileInfo.isCollidingAtTile(x, y);

                    (string? name, string? category) info = TileInfo.getNameWithCategoryNameAtTile(tile);

                    #region Narrate toSpeak
                    if (info.name != null)
                        if (MainClass.ScreenReader != null)
                            if (manuallyTriggered)
                                MainClass.ScreenReader.Say($"{info.name}, Category: {info.category}", true);
                            else
                                MainClass.ScreenReader.SayWithTileQuery(info.name, x, y, true);
                    #endregion

                    #region Play colliding sound effect
                    if (isColliding && prevTile != tile)
                    {
                        Game1.playSound("colliding");
                    }
                    #endregion

                    if (!manuallyTriggered)
                        prevTile = tile;
                }

            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Error in Read Tile:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
