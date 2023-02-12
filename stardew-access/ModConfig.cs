using StardewModdingAPI.Utilities;

namespace stardew_access
{
    internal class ModConfig
    {
        // https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Input#SButton button key codes

        #region Simulate mouse clicks
        public KeybindList LeftClickMainKey { get; set; } = KeybindList.Parse("LeftControl + Enter"); // Primary key to simulate mouse left click
        public KeybindList RightClickMainKey { get; set; } = KeybindList.Parse("LeftShift + Enter"); // Primary key to simulate mouse right click
        public KeybindList LeftClickAlternateKey { get; set; } = KeybindList.Parse("OemOpenBrackets"); // Secondary key to simulate mouse left click
        public KeybindList RightClickAlternateKey { get; set; } = KeybindList.Parse("OemCloseBrackets"); // Secondary key to simulate mouse right click
        #endregion

        #region Chat menu
        public KeybindList ChatMenuNextKey { get; set; } = KeybindList.Parse("PageUp"); // Read previous chat message
        public KeybindList ChatMenuPreviousKey { get; set; } = KeybindList.Parse("PageDown");  // Read next chat message
        #endregion

        #region Read tile
        public Boolean ReadTile { get; set; } = true; // Toggle this feature.
        public KeybindList ReadTileKey { get; set; } = KeybindList.Parse("J"); // Manually trigger read tile for the tile player is *looking at*.
        public KeybindList ReadStandingTileKey { get; set; } = KeybindList.Parse("LeftAlt + J"); // Manually trigger read tile for the tile player is *standing on*.
        public Boolean ReadFlooring { get; set; } = false; // Toggle reading floorings.
        public Boolean WateredToggle { get; set; } = true; // Toggle speaking watered or unwatered for crops.
        #endregion

        #region Tile viewer
        public KeybindList TileCursorUpKey { get; set; } = KeybindList.Parse("Up"); // Move the cursor one tile up
        public KeybindList TileCursorRightKey { get; set; } = KeybindList.Parse("Right"); // Move the cursor one tile right
        public KeybindList TileCursorDownKey { get; set; } = KeybindList.Parse("Down"); // Move the cursor one tile down
        public KeybindList TileCursorLeftKey { get; set; } = KeybindList.Parse("Left"); // Move the cursor one tile left
        public KeybindList TileCursorPreciseUpKey { get; set; } = KeybindList.Parse("LeftShift + Up"); // Move the cursor up by precision i.e. pixel by pixel
        public KeybindList TileCursorPreciseRightKey { get; set; } = KeybindList.Parse("LeftShift + Right"); // Move the cursor right by precision i.e. pixel by pixel
        public KeybindList TileCursorPreciseDownKey { get; set; } = KeybindList.Parse("LeftShift + Down"); // Move the cursor down by precision i.e. pixel by pixel
        public KeybindList TileCursorPreciseLeftKey { get; set; } = KeybindList.Parse("LeftShift + Left"); // Move the cursor left by precision i.e. pixel by pixel
        public KeybindList ToggleRelativeCursorLockKey { get; set; } = KeybindList.Parse("L"); // Toggles realative cursor lock i.e. if enabled, the cursor will reset when player moves.
        public KeybindList AutoWalkToTileKey { get; set; } = KeybindList.Parse("LeftControl + Enter"); // Auto walk to the tile
        public bool LimitTileCursorToScreen { get; set; } = false; // #TODO add command for this // Toggle whether to prevent cursor from going out of screen.
        public int TileCursorPreciseMovementDistance { get; set; } = 8;  // Specifies the number of pixels the cursor should move when using precision movement i.e. with *left shift*.
        #endregion

        #region Radar
        public Boolean Radar { get; set; } = false;
        public Boolean RadarStereoSound { get; set; } = true;
        #endregion

        #region Menu Keys
        public KeybindList PrimaryInfoKey { get; set; } = KeybindList.Parse("C");

        // Character Creation menu (new game menu) keys
        public KeybindList CharacterCreationMenuNextKey { get; set; } = KeybindList.Parse("Right");
        public KeybindList CharacterCreationMenuPreviousKey { get; set; } = KeybindList.Parse("Left");
        public KeybindList CharacterCreationMenuSliderIncreaseKey { get; set; } = KeybindList.Parse("Up");
        public KeybindList CharacterCreationMenuSliderLargeIncreaseKey { get; set; } = KeybindList.Parse("PageUp");
        public KeybindList CharacterCreationMenuSliderDecreaseKey { get; set; } = KeybindList.Parse("Down");
        public KeybindList CharacterCreationMenuSliderLargeDecreaseKey { get; set; } = KeybindList.Parse("PageDown");
        public KeybindList CharacterCreationMenuDesignToggleKey { get; set; } = KeybindList.Parse("LeftControl + Space");

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
        public KeybindList HealthNStaminaKey { get; set; } = KeybindList.Parse("H"); // Narrate health and stamina.
        public bool HealthNStaminaInPercentage { get; set; } = true;
        public KeybindList PositionKey { get; set; } = KeybindList.Parse("K"); // Narrate player position.
        public KeybindList LocationKey { get; set; } = KeybindList.Parse("LeftAlt + K"); // Narrate current location name.
        public KeybindList MoneyKey { get; set; } = KeybindList.Parse("R"); // Narrate the money the player has currently.
        public KeybindList TimeNSeasonKey { get; set; } = KeybindList.Parse("Q"); // Narrate the time of day, day and date and season
        public Boolean VerboseCoordinates { get; set; } = true;
        public Boolean SnapMouse { get; set; } = true; // Toggles the snap mouse feature
        public Boolean Warning { get; set; } = true; // Toggles the warnings feature
        public Boolean TTS { get; set; } = true; // Toggles the screen reader/tts.
        public Boolean TrackDroppedItems {get; set;} = true; // Toggles detecting the dropped items.
        #endregion

        public int MaximumFishingDifficulty { get; set; } = 999; // TODO Add doc
        public int FixFishingMotionType { get; set; } = 999;

        // TODO Add the exclusion and focus list too
        // public String ExclusionList { get; set; } = "test";
    }
}
