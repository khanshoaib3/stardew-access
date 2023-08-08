using Microsoft.Xna.Framework;
using stardew_access.Features;
using stardew_access.Utils;

namespace stardew_access
{
    public class API
    {
        // Note to future self, don't make these static, it won't give errors in sv access but it will in other mods if they try to use the stardew access api.
        public API() { }

        public string PrevMenuQueryText
        {
            get => MainClass.ScreenReader.PrevMenuQueryText;
            set => MainClass.ScreenReader.PrevMenuQueryText = value;
        }

        public string MenuPrefixText
        {
            get => MainClass.ScreenReader.MenuPrefixText;
            set => MainClass.ScreenReader.MenuPrefixText = value;
        }

        public string MenuSuffixText
        {
            get => MainClass.ScreenReader.MenuSuffixText;
            set => MainClass.ScreenReader.MenuSuffixText = value;
        }

        public string MenuPrefixNoQueryText
        {
            get => MainClass.ScreenReader.MenuPrefixNoQueryText;
            set => MainClass.ScreenReader.MenuPrefixNoQueryText = value;
        }

        public string MenuSuffixNoQueryText
        {
            get => MainClass.ScreenReader.MenuSuffixNoQueryText;
            set => MainClass.ScreenReader.MenuSuffixNoQueryText = value;
        }

        /// <summary>
        /// Search the area using Breadth First Search algorithm(BFS).
        /// </summary>
        /// <param name="center">The starting point.</param>
        /// <param name="limit">The limiting factor or simply radius of the search area.</param>
        /// <returns>A dictionary with all the detected tiles along with the name of the object on it and it's category.</returns>
        public Dictionary<Vector2, (string name, string category)> SearchNearbyTiles(
            Vector2 center,
            int limit
        )
        {
            /*
            * How to use the Dictionary to get the name and category of a tile:-
            *
            * string tileName = detectedTiles.GetValueOrDefault(tilePosition).name;
            * string tileCategory = detectedTiles.GetValueOrDefault(tilePosition).category;
            *
            * Here detectedTiles is the Dictionary returned by this method
            */

            return new Radar().SearchNearbyTiles(center, limit, false);
        }

        /// <summary>
        /// Search the entire location using Breadth First Search algorithm(BFS).
        /// </summary>
        /// <returns>A dictionary with all the detected tiles along with the name of the object on it and it's category.</returns>
        public Dictionary<Vector2, (string name, string category)> SearchLocation()
        {
            /*
            * How to use the Dictionary to get the name and category of a tile:-
            *
            * string tileName = detectedTiles.GetValueOrDefault(tilePosition).name;
            * string tileCategory = detectedTiles.GetValueOrDefault(tilePosition).category;
            *
            * Here detectedTiles is the Dictionary returned by this method
            */

            return Radar.SearchLocation();
        }

        /// <summary>
        /// Check the tile for any object
        /// </summary>
        /// <param name="tile">The tile where we want to check the name and category of object if any</param>
        /// <returns>Name of the object as the first item (name) and category as the second item (category). Returns null if no object found.</returns>
        public (string? name, string? category) GetNameWithCategoryNameAtTile(Vector2 tile)
        {
            return TileInfo.GetNameWithCategoryNameAtTile(tile, null);
        }

        /// <summary>
        /// Check the tile for any object
        /// </summary>
        /// <param name="tile">The tile where we want to check the name and category of object if any</param>
        /// <returns>Name of the object. Returns null if no object found.</returns>
        public string? GetNameAtTile(Vector2 tile)
        {
            return TileInfo.GetNameAtTile(tile, null);
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        /// <returns>true if the text was spoken otherwise false.</returns>
        public bool Say(String text, Boolean interrupt)
        {
            if (MainClass.ScreenReader == null)
                return false;

            return MainClass.ScreenReader.Say(text, interrupt);
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        /// <param name="customQuery">If set, uses this instead of <paramref name="text"/> as query to check whether to speak the text or not.</param>
        /// <returns>true if the text was spoken otherwise false.</returns>
        public bool SayWithChecker(String text, Boolean interrupt, String? customQuery = null)
        {
            if (MainClass.ScreenReader == null)
                return false;

            return MainClass.ScreenReader.SayWithChecker(text, interrupt, customQuery);
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
        /// <br/><br/>Use this when narrating hovered component in menus to avoid interference.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        /// <param name="customQuery">If set, uses this instead of <paramref name="text"/> as query to check whether to speak the text or not.</param>
        /// <returns>true if the text was spoken otherwise false.</returns>
        public bool SayWithMenuChecker(String text, Boolean interrupt, String? customQuery = null)
        {
            if (MainClass.ScreenReader == null)
                return false;

            return MainClass.ScreenReader.SayWithMenuChecker(text, interrupt, customQuery);
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
        /// <br/><br/>Use this when narrating chat messages to avoid interference.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        /// <returns>true if the text was spoken otherwise false.</returns>
        public bool SayWithChatChecker(String text, Boolean interrupt)
        {
            if (MainClass.ScreenReader == null)
                return false;

            return MainClass.ScreenReader.SayWithChatChecker(text, interrupt);
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
        /// <br/><br/>Use this when narrating texts based on tile position to avoid interference.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="x">The X location of tile.</param>
        /// <param name="y">The Y location of tile.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        /// <returns>true if the text was spoken otherwise false.</returns>
        public bool SayWithTileQuery(String text, int x, int y, Boolean interrupt)
        {
            if (MainClass.ScreenReader == null)
                return false;

            return MainClass.ScreenReader.SayWithTileQuery(text, x, y, interrupt);
        }
    }
}
