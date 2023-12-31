using StardewModdingAPI.Utilities;

// ReSharper disable InconsistentNaming

namespace stardew_access;

internal class ModConfig
{
    // https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Input#SButton button key codes

    #region Simulate mouse clicks

    /// <summary>
    /// Primary key to simulate mouse left click
    /// </summary>
    public KeybindList LeftClickMainKey { get; set; } = KeybindList.Parse("LeftControl + Enter");

    /// <summary>
    /// Primary key to simulate mouse right click
    /// </summary>
    public KeybindList RightClickMainKey { get; set; } = KeybindList.Parse("LeftShift + Enter");

    /// <summary>
    /// Secondary key to simulate mouse left click
    /// </summary>
    public KeybindList LeftClickAlternateKey { get; set; } = KeybindList.Parse("OemOpenBrackets");

    /// <summary>
    /// Secondary key to simulate mouse right click
    /// </summary>
    public KeybindList RightClickAlternateKey { get; set; } = KeybindList.Parse("OemCloseBrackets");

    #endregion

    #region Chat menu

    /// <summary>
    /// Read previous chat message
    /// </summary>
    public KeybindList ChatMenuNextKey { get; set; } = KeybindList.Parse("PageUp");

    /// <summary>
    /// Read next chat message
    /// </summary>
    public KeybindList ChatMenuPreviousKey { get; set; } = KeybindList.Parse("PageDown");

    #endregion

    #region Read tile

    /// <summary>
    /// Toggle this feature.
    /// </summary>
    public Boolean ReadTile { get; set; } = true;

    /// <summary>
    /// Manually trigger read tile for the tile player is *looking at*.
    /// </summary>
    public KeybindList ReadTileKey { get; set; } = KeybindList.Parse("J");

    /// <summary>
    /// Manually trigger read tile for the tile player is *standing on*.
    /// </summary>
    public KeybindList ReadStandingTileKey { get; set; } = KeybindList.Parse("LeftAlt + J");

    /// <summary>
    /// Toggle reading flooring.
    /// </summary>
    public Boolean ReadFlooring { get; set; } = false;

    /// <summary>
    /// Toggle speaking watered or unwatered for crops.
    /// </summary>
    public Boolean WateredToggle { get; set; } = true;

    /// <summary>
    /// Toggle speaking tile indexes with other info.
    /// </summary>
    public Boolean ReadTileIndexes { get; set; } = false;

    #endregion

    #region Tile viewer

    /// <summary>
    /// Move the cursor one tile up
    /// </summary>
    public KeybindList TileCursorUpKey { get; set; } = KeybindList.Parse("Up");

    /// <summary>
    /// Move the cursor one tile right
    /// </summary>
    public KeybindList TileCursorRightKey { get; set; } = KeybindList.Parse("Right");

    /// <summary>
    /// Move the cursor one tile down
    /// </summary>
    public KeybindList TileCursorDownKey { get; set; } = KeybindList.Parse("Down");

    /// <summary>
    /// Move the cursor one tile left
    /// </summary>
    public KeybindList TileCursorLeftKey { get; set; } = KeybindList.Parse("Left");

    /// <summary>
    /// Move the cursor up by precision i.e. pixel by pixel
    /// </summary>
    public KeybindList TileCursorPreciseUpKey { get; set; } = KeybindList.Parse("LeftShift + Up");

    /// <summary>
    /// Move the cursor right by precision i.e. pixel by pixel
    /// </summary>
    public KeybindList TileCursorPreciseRightKey { get; set; } = KeybindList.Parse("LeftShift + Right");

    /// <summary>
    /// Move the cursor down by precision i.e. pixel by pixel
    /// </summary>
    public KeybindList TileCursorPreciseDownKey { get; set; } = KeybindList.Parse("LeftShift + Down");

    /// <summary>
    /// Move the cursor left by precision i.e. pixel by pixel
    /// </summary>
    public KeybindList TileCursorPreciseLeftKey { get; set; } = KeybindList.Parse("LeftShift + Left");

    /// <summary>
    /// Toggles relative cursor lock i.e. if enabled, the cursor will reset when player moves.
    /// </summary>
    public KeybindList ToggleRelativeCursorLockKey { get; set; } = KeybindList.Parse("L");

    /// <summary>
    /// Auto walk to the tile
    /// </summary>
    public KeybindList AutoWalkToTileKey { get; set; } = KeybindList.Parse("LeftControl + Enter");

    /// <summary>
    /// Opens the Tile Info menu for the active tile. Default is `LeftShift + Enter`.
    /// </summary>
    public KeybindList OpenTileInfoMenuKey { get; set; } = KeybindList.Parse("LeftShift + Enter");

    /// <summary>
    /// Toggle whether to prevent cursor from going out of screen.
    /// </summary>
    public bool LimitTileCursorToScreen { get; set; } = false;

    /// <summary>
    /// Specifies the number of pixels the cursor should move when using precision movement i.e. with *left shift*.
    /// </summary>
    public int TileCursorPreciseMovementDistance { get; set; } = 8;

    #endregion

    #region Radar

    /// <summary>
    /// Toggle Radar feature.
    /// </summary>
    public Boolean Radar { get; set; } = false;

    /// <summary>
    /// Toggle whether to use stereo sound or mono
    /// </summary>
    public Boolean RadarStereoSound { get; set; } = true;

    #endregion

    #region Menu Keys

    /// <summary>
    /// Used to speak additional info on certain menus.
    /// </summary>
    public KeybindList PrimaryInfoKey { get; set; } = KeybindList.Parse("C");

    // Character Creation menu (new game menu) keys

    /// <summary>
    /// Go to next element
    /// </summary>
    public KeybindList CharacterCreationMenuNextKey { get; set; } = KeybindList.Parse("Right");

    /// <summary>
    /// Go to previous element
    /// </summary>
    public KeybindList CharacterCreationMenuPreviousKey { get; set; } = KeybindList.Parse("Left");

    /// <summary>
    /// Increase the slider value by 1
    /// </summary>
    public KeybindList CharacterCreationMenuSliderIncreaseKey { get; set; } = KeybindList.Parse("Up");

    /// <summary>
    /// Increase the slider value by 10
    /// </summary>
    public KeybindList CharacterCreationMenuSliderLargeIncreaseKey { get; set; } = KeybindList.Parse("PageUp");

    /// <summary>
    /// Decrease the slider value by 1
    /// </summary>
    public KeybindList CharacterCreationMenuSliderDecreaseKey { get; set; } = KeybindList.Parse("Down");

    /// <summary>
    /// Decrease the slider value by 10
    /// </summary>
    public KeybindList CharacterCreationMenuSliderLargeDecreaseKey { get; set; } = KeybindList.Parse("PageDown");

    /// <summary>
    /// Toggle displaying character design options
    /// </summary>
    public KeybindList CharacterCreationMenuDesignToggleKey { get; set; } = KeybindList.Parse("LeftControl + Space, RightControl + Space");
    public Boolean CharacterCreationMenuDesignDefaultEnabled { get; set; } = true;


    // Bundle menu keys

    /// <summary>
    /// Cycle through the ingredients in the current selected bundle
    /// </summary>
    public KeybindList BundleMenuIngredientsKey { get; set; } = KeybindList.Parse("I");

    /// <summary>
    /// Cycle through the items in the player's inventory
    /// </summary>
    public KeybindList BundleMenuInventoryItemsKey { get; set; } = KeybindList.Parse("C");

    /// <summary>
    /// Move the mouse cursor to purchase button
    /// </summary>
    public KeybindList BundleMenuPurchaseButtonKey { get; set; } = KeybindList.Parse("P");

    /// <summary>
    /// Cycle through the ingredient input slots
    /// </summary>
    public KeybindList BundleMenuIngredientsInputSlotKey { get; set; } = KeybindList.Parse("V");

    /// <summary>
    /// Move the mouse cursor to back button
    /// </summary>
    public KeybindList BundleMenuBackButtonKey { get; set; } = KeybindList.Parse("Back");

    // Menus with secondary inventory(shop inventory or chest inventory or crafting recipe list)

    /// <summary>
    /// Snaps to the first slot in primary inventory i.e.,
    /// </summary>
    public KeybindList SnapToFirstInventorySlotKey { get; set; } = KeybindList.Parse("I");

    /// <summary>
    /// Snaps to the first slot in secondary inventory i.e.,
    /// </summary>
    public KeybindList SnapToFirstSecondaryInventorySlotKey { get; set; } = KeybindList.Parse("LeftShift + I");

    // Crafting menu

    /// <summary>
    /// Cycle through the recipes in crafting menu.
    /// </summary>
    public KeybindList CraftingMenuCycleThroughRecipesKey { get; set; } = KeybindList.Parse("C");

    #endregion

    #region GridMovement

    /// <summary>
    /// Enable or disable grid movement feature.
    /// </summary>
    public Boolean GridMovementActive { get; set; } = true;

    /// <summary>
    /// Toggle grid movement.
    /// </summary>
    public KeybindList ToggleGridMovementKey { get; set; } = KeybindList.Parse("I");

    /// <summary>
    /// Disable Grid Movement while held
    /// </summary>
    public KeybindList GridMovementOverrideKey { get; set; } = KeybindList.Parse("LeftControl");

    private double _GridMovementSpeedPercent = 100d;

    /// <summary>
    /// Player movement speed (in percentage).
    /// </summary>
    public double GridMovementSpeedPercent
    {
        get { return Math.Clamp(_GridMovementSpeedPercent, 0.0d, 120.0d); }
        set { _GridMovementSpeedPercent = Math.Clamp(value, 0.0d, 120.0d); }
    }

    /// <summary>
    /// Tiles taken per step.
    /// </summary>
    public int GridMovementTilesPerStep { get; set; } = 1;

    /// <summary>
    /// Delay after first step.
    /// </summary>
    public int GridMovementDelayAfterFirstStep { get; set; } = 300;

    #endregion

    # region ObjectTracker

    /// <summary>
    /// Cycle Up Category
    /// </summary>
    public KeybindList OTCycleUpCategory { get; set; } = KeybindList.Parse("LeftControl + PageUp");

    /// <summary>
    /// Cycle Down Category
    /// </summary>
    public KeybindList OTCycleDownCategory { get; set; } = KeybindList.Parse("LeftControl + PageDown");

    // favorites
    public KeybindList OTFavorite1 { get; set; } = KeybindList.Parse("LeftAlt + D1, RightAlt + D1");
    public KeybindList OTFavorite2 { get; set; } = KeybindList.Parse("LeftAlt + D2, RightAlt + D2");
    public KeybindList OTFavorite3 { get; set; } = KeybindList.Parse("LeftAlt + D3, RightAlt + D3");
    public KeybindList OTFavorite4 { get; set; } = KeybindList.Parse("LeftAlt + D4, RightAlt + D4");
    public KeybindList OTFavorite5 { get; set; } = KeybindList.Parse("LeftAlt + D5, RightAlt + D5");
    public KeybindList OTFavorite6 { get; set; } = KeybindList.Parse("LeftAlt + D6, RightAlt + D6");
    public KeybindList OTFavorite7 { get; set; } = KeybindList.Parse("LeftAlt + D7, RightAlt + D7");
    public KeybindList OTFavorite8 { get; set; } = KeybindList.Parse("LeftAlt + D8, RightAlt + D8");
    public KeybindList OTFavorite9 { get; set; } = KeybindList.Parse("LeftAlt + D9, RightAlt + D9");
    public KeybindList OTFavorite10 { get; set; } = KeybindList.Parse("LeftAlt + D0, RightAlt + D0");
    public KeybindList OTFavoriteDecreaseStack { get; set; } = KeybindList.Parse("LeftAlt + OemMinus, RightAlt + OemMinus");
    public KeybindList OTFavoriteIncreaseStack { get; set; } = KeybindList.Parse("LeftAlt + OemPlus, RightAlt + OemPlus");

    /// <summary>
    /// Cycle Up Object
    /// </summary>
    public KeybindList OTCycleUpObject { get; set; } = KeybindList.Parse("PageUp");

    /// <summary>
    /// Cycle Down Object
    /// </summary>
    public KeybindList OTCycleDownObject { get; set; } = KeybindList.Parse("PageDown");

    /// <summary>
    /// Move to the currently selected object.
    /// </summary>
    public KeybindList OTMoveToSelectedObject { get; set; } = KeybindList.Parse("LeftControl + Home");

    /// <summary>
    /// Read info about the currently selected object.
    /// </summary>
    public KeybindList OTReadSelectedObject { get; set; } = KeybindList.Parse("Home");

    /// <summary>
    /// Read info about the currently selected objects tile location.
    /// Can be used to read info about the currently selected object with different verbosity
    /// </summary>
    public KeybindList OTReadSelectedObjectTileLocation { get; set; } = KeybindList.Parse("End");

    /// <summary>
    /// Manually stop Auto Walking.
    /// </summary>
    public KeybindList OTCancelAutoWalking { get; set; } = KeybindList.Parse("Escape");

    /// <summary>
    /// Toggle proximity sorting vs alphabetical
    /// </summary>
    public KeybindList OTSwitchSortingMode { get; set; } = KeybindList.Parse("OemTilde");

    /// <summary>
    /// 
    /// </summary>
    public Boolean OTAutoRefreshing { get; set; } = true;

    /// <summary>
    /// If enabled, the default sorting mode will be proximity. 
    /// </summary>
    public Boolean OTSortByProximity { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public Boolean OTWrapLists { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public Boolean OTRememberPosition { get; set; } = true;

    #endregion

    #region Others

    /// <summary>
    /// Narrate health and stamina.
    /// </summary>
    public KeybindList HealthNStaminaKey { get; set; } = KeybindList.Parse("H");

    /// <summary>
    /// Whether to speak health and stamina in percentage or the actual value.
    /// </summary>
    public bool HealthNStaminaInPercentage { get; set; } = true;

    /// <summary>
    /// Narrate player position.
    /// </summary>
    public KeybindList PositionKey { get; set; } = KeybindList.Parse("K");

    /// <summary>
    /// Narrate current location name.
    /// </summary>
    public KeybindList LocationKey { get; set; } = KeybindList.Parse("LeftAlt + K");

    /// <summary>
    /// Narrate the money the player has currently.
    /// </summary>
    public KeybindList MoneyKey { get; set; } = KeybindList.Parse("R");

    /// <summary>
    /// Narrate the time of day, day and date and season
    /// </summary>
    public KeybindList TimeNSeasonKey { get; set; } = KeybindList.Parse("Q");

    /// <summary>
    /// Whether to speak 'X:' and 'Y:' along with co-ordinates or not.
    /// </summary>
    public Boolean VerboseCoordinates { get; set; } = true;

    /// <summary>
    /// Toggles the snap mouse feature
    /// </summary>
    public Boolean SnapMouse { get; set; } = true;

    /// <summary>
    /// Toggles the warnings feature
    /// </summary>
    public Boolean Warning { get; set; } = true;

    /// <summary>
    /// Toggles the screen reader/tts.
    /// </summary>
    public Boolean TTS { get; set; } = true;

    /// <summary>
    /// Toggles detecting the dropped items.
    /// </summary>
    public Boolean TrackDroppedItems { get; set; } = true;

    /// <summary>
    /// If enabled, does not speaks 'not usable here' and 'donatable' in inventories
    /// </summary>
    public Boolean DisableInventoryVerbosity { get; set; } = false;

    /// <summary>
    /// If enabled, does not speak bush type or size; only harvestable.
    /// </summary>
    public Boolean DisableBushVerbosity { get; set; } = false;

    /// <summary>
    /// Sets speech rate for the Mac TTS.
    /// </summary>
    public Single MacSpeechRate { get; set; } = 220;

    #endregion


    /// <summary>
    ///  Every fish have a difficulty value which varies from around 0 to 150. You can use this to limit the maximum difficulty any fish can have.                                                                                                                                                                              
    /// </summary>
    public int MaximumFishingDifficulty { get; set; } = 999;

    /// <summary>
    ///  You can fix what motion type every fish has, by default every fish has a fixed motion type like for squid it's sinker, walleye it's smooth, etc. You can use a value between 0 to 4 to fix the motion. 0 indicates mixed motion type, 1 indicates art, 2 indicates smooth, 3 indicates sinker and 4 indicates floater.
    /// </summary>
    public int FixFishingMotionType { get; set; } = 999;

    // TODO Add the exclusion and focus list too
    // public String ExclusionList { get; set; } = "test";
}