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
                int x, y;
                #region Get Tile
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
                #endregion
                x = (int)tile.X;
                y = (int)tile.Y;

                if (Context.IsPlayerFree)
                {
                    if (!manuallyTriggered && prevTile != tile)
                    {
                        if (MainClass.ScreenReader != null)
                            MainClass.ScreenReader.PrevTextTile = " ";
                    }

                    bool isColliding = TileInfo.isCollidingAtTile(x, y);

                    string? toSpeak = TileInfo.getNameAtTile(tile);

                    #region Narrate toSpeak
                    if (toSpeak != null)
                        if (MainClass.ScreenReader != null)
                            if (manuallyTriggered)
                                MainClass.ScreenReader.Say(toSpeak, true);
                            else
                                MainClass.ScreenReader.SayWithTileQuery(toSpeak, x, y, true);
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
