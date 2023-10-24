using Microsoft.Xna.Framework;
using stardew_access.Features;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley.Menus;

namespace stardew_access
{
    public class API
    {
        // Note to future self, don't make these static, it won't give errors in sv access but it will in other mods if they try to use the stardew access api.
        //Setting Pragma to disable warning CA1822 prompting to make fields static.
        public API()
        {
        }
#pragma warning disable CA1822 // Mark members as static
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

        /// <summary>
        /// Speaks the hovered inventory slot from the provided <see cref="InventoryMenu"/>.
        /// In case there is nothing in a slot, then it will speak "Empty Slot".
        /// Also plays a sound if the slot is grayed out, like tools in <see cref="GeodeMenu">geode menu</see>.
        /// </summary>
        /// <param name="inventoryMenu">The object of <see cref="InventoryMenu"/> whose inventory is to be spoken.</param>
        /// <param name="giveExtraDetails">(Optional) Whether to speak extra details about the item in slot or not. Default to null in which case it uses <see cref="ModConfig.DisableInventoryVerbosity"/> to get whether to speak extra details or not.</param>
        /// <param name="hoverPrice">(Optional) The price of the hovered item, generally used in <see cref="ShopMenu"/>.</param>
        /// <param name="extraItemToShowIndex">(Optional) The index (probably parentSheetIndex) of the extra item which is generally a requirement for the hovered item in certain menus.</param>
        /// <param name="extraItemToShowAmount">(Optional) The amount or quantity of the extra item which is generally a requirement for the hovered item in certain menus.</param>
        /// <param name="highlightedItemPrefix">(Optional) The prefix to add to the spoken hovered item's details if it is highlighted i.e., not grayed out.</param>
        /// <param name="highlightedItemSuffix">(Optional) The suffix to add to the spoken hovered item's details if it is highlighted i.e., not grayed out.</param>
        /// <param name="hoverX">(Optional) The X position on screen to check. Default to null, in which case it uses the mouse's X position.</param>
        /// <param name="hoverY">(Optional) The Y position on screen to check. Default to null, in which case it uses the mouse's Y position.</param>
        /// <returns>true if any inventory slot was hovered or found at the <paramref name="hoverX"/> and <paramref name="hoverY"/>.</returns>
        public bool SpeakHoveredInventorySlot(InventoryMenu? inventoryMenu,
            bool? giveExtraDetails = null,
            int hoverPrice = -1,
            int extraItemToShowIndex = -1,
            int extraItemToShowAmount = -1,
            string highlightedItemPrefix = "",
            string highlightedItemSuffix = "",
            int? hoverX = null,
            int? hoverY = null) =>
            InventoryUtils.NarrateHoveredSlot(inventoryMenu,
                giveExtraDetails,
                hoverPrice,
                extraItemToShowIndex,
                extraItemToShowAmount,
                highlightedItemPrefix,
                highlightedItemSuffix,
                hoverX,
                hoverY);

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

        /// <summary>
        /// Registers a language helper to be used for a specific locale.
        /// </summary>
        /// <param name="locale">The locale for which the helper should be used (e.g., "en", "fr", "es-es").</param>
        /// <param name="helper">An instance of the language helper class implementing <see cref="ILanguageHelper"/>.</param>
        /// <remarks>
        /// The provided helper class should ideally derive from <see cref="LanguageHelperBase"/> for optimal compatibility, though this is not strictly required as long as it implements <see cref="ILanguageHelper"/>.
        /// </remarks>
        public void RegisterLanguageHelper(string locale, Type helperType)
        {
#if DEBUG
            Log.Trace($"Registered language helper for locale '{locale}': Type: {helperType.Name}");
#endif
            CustomFluentFunctions.RegisterLanguageHelper(locale, helperType);
        }
#pragma warning restore CA1822 // Mark members as static
    }
}