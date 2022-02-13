using Microsoft.Xna.Framework;
using stardew_access.Features;

namespace stardew_access.ScreenReader
{
    public class API
    {

        public API()
        {
        }

        /// <summary>
        /// Search the area using Breadth First Search algorithm(BFS).
        /// </summary>
        /// <param name="center">The starting point.</param>
        /// <param name="limit">The limiting factor or simply radius of the search area.</param>
        /// <returns>A dictionary with all the detected tiles along with the name of the object on it and it's category.</returns>
        public Dictionary<Vector2, (string, string)> SearchNearbyTiles(Vector2 center, int limit)
        {
            /*
            * How to use the Dictionary to get the name and category of a tile:-
            * 
            * string? objectName = detectedTiles.GetValueOrDefault(center).Item1;
            * string? objectCategory = detectedTiles.GetValueOrDefault(center).Item2;
            *
            * Here detectedTiles is the Dictionary returned by this method
            */

            return new Radar().SearchNearbyTiles(center, limit, false);
        }

        /// <summary>
        /// Check the tile for any object
        /// </summary>
        /// <param name="tile">The tile where we want to check the name and category of object if any</param>
        /// <returns>Name of the object as the first item (Item1) and category as the second item (Item2). Returns null if no object found.</returns>
        public (string?, string?) GetNameWithCategoryNameAtTile(Vector2 tile)
        {
            return ReadTile.getNameWithCategoryNameAtTile(tile);
        }

        /// <summary>
        /// Check the tile for any object
        /// </summary>
        /// <param name="tile">The tile where we want to check the name and category of object if any</param>
        /// <returns>Name of the object. Returns null if no object found.</returns>
        public string? GetNameAtTile(Vector2 tile)
        {
            return ReadTile.getNameAtTile(tile);
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        public void Say(String text, Boolean interrupt)
        {
            if (MainClass.GetScreenReader() == null)
                return;

            MainClass.GetScreenReader().Say(text, interrupt);
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        public void SayWithChecker(String text, Boolean interrupt)
        {
            if (MainClass.GetScreenReader() == null)
                return;

            MainClass.GetScreenReader().SayWithChecker(text, interrupt);
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
        /// <br/><br/>Use this when narrating hovered component in menus to avoid interference.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        public void SayWithMenuChecker(String text, Boolean interrupt)
        {
            if (MainClass.GetScreenReader() == null)
                return;

            MainClass.GetScreenReader().SayWithMenuChecker(text, interrupt);
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
        /// <br/><br/>Use this when narrating chat messages to avoid interference.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        public void SayWithChatChecker(String text, Boolean interrupt)
        {
            if (MainClass.GetScreenReader() == null)
                return;

            MainClass.GetScreenReader().SayWithChatChecker(text, interrupt);
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
        /// <br/><br/>Use this when narrating texts based on tile position to avoid interference.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="x">The X location of tile.</param>
        /// <param name="y">The Y location of tile.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        public void SayWithTileQuery(String text, int x, int y, Boolean interrupt)
        {
            if (MainClass.GetScreenReader() == null)
                return;

            MainClass.GetScreenReader().SayWithTileQuery(text, x, y, interrupt);
        }

    }
}