using StardewModdingAPI.Utilities;

namespace stardew_access
{
    internal class ModConfig
    {
        // https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Input#SButton button key codes

        #region Simulate mouse clicks
        public KeybindList LeftClickMainKey { get; set; } = KeybindList.Parse("LeftControl + Enter");
        public KeybindList RightClickMainKey { get; set; } = KeybindList.Parse("LeftShift + Enter");
        public KeybindList LeftClickAlternateKey { get; set; } = KeybindList.Parse("OemOpenBrackets");
        public KeybindList RightClickAlternateKey { get; set; } = KeybindList.Parse("OemCloseBrackets");
        #endregion

        #region Chat menu
        public KeybindList ChatMenuNextKey { get; set; } = KeybindList.Parse("PageUp");
        public KeybindList ChatMenuPreviousKey { get; set; } = KeybindList.Parse("PageDown");
        #endregion

        #region Read tile
        public Boolean ReadTile { get; set; } = true;
        public KeybindList ReadTileKey { get; set; } = KeybindList.Parse("J");
        public KeybindList ReadStandingTileKey { get; set; } = KeybindList.Parse("LeftAlt + J");
        public Boolean ReadFlooring { get; set; } = false;
        #endregion

        #region Tile viewer
        public KeybindList TileCursorUpKey { get; set; } = KeybindList.Parse("Up");
        public KeybindList TileCursorRightKey { get; set; } = KeybindList.Parse("Right");
        public KeybindList TileCursorDownKey { get; set; } = KeybindList.Parse("Down");
        public KeybindList TileCursorLeftKey { get; set; } = KeybindList.Parse("Left");
        public KeybindList TileCursorPreciseUpKey { get; set; } = KeybindList.Parse("LeftShift + Up");
        public KeybindList TileCursorPreciseRightKey { get; set; } = KeybindList.Parse("LeftShift + Right");
        public KeybindList TileCursorPreciseDownKey { get; set; } = KeybindList.Parse("LeftShift + Down");
        public KeybindList TileCursorPreciseLeftKey { get; set; } = KeybindList.Parse("LeftShift + Left");
        public KeybindList ToggleRelativeCursorLockKey { get; set; } = KeybindList.Parse("L");
        public KeybindList AutoWalkToTileKey { get; set; } = KeybindList.Parse("LeftControl + Enter");
        public bool LimitTileCursorToScreen { get; set; } = false;
        public int TileCursorPreciseMovementDistance { get; set; } = 8;
        #endregion

        #region Radar
        public Boolean Radar { get; set; } = false;
        public Boolean RadarStereoSound { get; set; } = true;
        #endregion

        #region Menu Keys
        public KeybindList PrimaryInfoKey { get; set; } = KeybindList.Parse("C");

        // Charachter Creatinon menu (new game menu) keys
        public KeybindList CharacterCreationMenuNextKey { get; set; } = KeybindList.Parse("Right");
        public KeybindList CharacterCreationMenuPreviousKey { get; set; } = KeybindList.Parse("Left");

        // Bundle menu keys
        public KeybindList BundleMenuIngredientsKey { get; set; } = KeybindList.Parse("I");
        public KeybindList BundleMenuInventoryItemsKey { get; set; } = KeybindList.Parse("C");
        public KeybindList BundleMenuPurchaseButtonKey { get; set; } = KeybindList.Parse("P");
        public KeybindList BundleMenuIngredientsInputSlotKey { get; set; } = KeybindList.Parse("V");
        public KeybindList BundleMenuBackButtonKey { get; set; } = KeybindList.Parse("Back");

        // Menus with secondary inventory(shop inventory or chest inventory or crafting recipe list)
        public KeybindList SnapToFirstInventorySlotKey { get; set; } = KeybindList.Parse("I");
        public KeybindList SnapToFirstSecondaryInventorySlotKey { get; set; } = KeybindList.Parse("LeftShift + I");

        // Crafting menu
        public KeybindList CraftingMenuCycleThroughRecipiesKey { get; set; } = KeybindList.Parse("C");
        #endregion

        #region Others
        public KeybindList HealthNStaminaKey { get; set; } = KeybindList.Parse("H");
        public KeybindList PositionKey { get; set; } = KeybindList.Parse("K");
        public KeybindList LocationKey { get; set; } = KeybindList.Parse("LeftAlt + K");
        public KeybindList MoneyKey { get; set; } = KeybindList.Parse("R");
        public KeybindList TimeNSeasonKey { get; set; } = KeybindList.Parse("Q");
        public Boolean VerboseCoordinates { get; set; } = true;
        public Boolean SnapMouse { get; set; } = true;
        #endregion

        // TODO Add the exclusion and focus list too
        // public String ExclusionList { get; set; } = "test";
    }
}