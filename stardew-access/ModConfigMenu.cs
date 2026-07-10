using stardew_access.Integrations;
using StardewModdingAPI;

namespace stardew_access;

// TODO I18n!!
internal static class ModConfigMenu
{
    internal static void Create(IModHelper helper, IManifest manifest, ModConfig config)
    {
        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null) return;

        // register mod
        configMenu.Register(
            mod: manifest,
            reset: () => config = new ModConfig(),
            save: () => helper.WriteConfig(config)
        );

        configMenu.AddPageLink(manifest, "stardew-access-mouse-click", () => "Mouse Click Settings");
        configMenu.AddPageLink(manifest, "stardew-access-chat", () => "Chat Reader Settings");
        configMenu.AddPageLink(manifest, "stardew-access-read-tile", () => "Read Tile Settings");
        configMenu.AddPageLink(manifest, "stardew-access-tile-viewer", () => "Tile Viewer Settings");
        configMenu.AddPageLink(manifest, "stardew-access-radar", () => "Radar Settings");
        configMenu.AddPageLink(manifest, "stardew-access-menu-keys", () => "Menu Keys Settings");
        configMenu.AddPageLink(manifest, "stardew-access-grid-movement", () => "Grid Movement Settings");
        configMenu.AddPageLink(manifest, "stardew-access-object-tracker", () => "Object Tracker Settings");
        configMenu.AddPageLink(manifest, "stardew-access-other", () => "Other Settings");

        AddMouseClickPage(configMenu, manifest, config);
        AddChatSectionPage(configMenu, manifest, config);
        AddReadTilePage(configMenu, manifest, config);
        AddTileViewerPage(configMenu, manifest, config);
        AddRadarPage(configMenu, manifest, config);
        AddMenuKeysPage(configMenu, manifest, config);
        AddGridMovementPage(configMenu, manifest, config);
        AddObjectTrackerPage(configMenu, manifest, config);
        AddOtherPage(configMenu, manifest, config);
    }

    private static void AddMouseClickPage(IGenericModConfigMenuApi configMenu, IManifest manifest, ModConfig config)
    {
        configMenu.AddPage( manifest, pageId: "stardew-access-mouse-click", pageTitle: () => "Mouse Click Settings" );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Left Click (Main Key)",
            tooltip: () => "Primary key to simulate left mouse click.",
            getValue: () => config.LeftClickMainKey,
            setValue: value => config.LeftClickMainKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Right Click (Main Key)",
            tooltip: () => "Primary key to simulate right mouse click.",
            getValue: () => config.RightClickMainKey,
            setValue: value => config.RightClickMainKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Left Click (Alternate Key)",
            tooltip: () => "Secondary key to simulate left mouse click.",
            getValue: () => config.LeftClickAlternateKey,
            setValue: value => config.LeftClickAlternateKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Right Click (Alternate Key)",
            tooltip: () => "Secondary key to simulate right mouse click.",
            getValue: () => config.RightClickAlternateKey,
            setValue: value => config.RightClickAlternateKey = value
        );
    }

    private static void AddChatSectionPage(IGenericModConfigMenuApi configMenu, IManifest manifest, ModConfig config)
    {
        configMenu.AddPage( manifest, pageId: "stardew-access-chat", pageTitle: () => "Chat menu settings" );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Chat Menu - Next Message",
            tooltip: () => "Read the next chat message.",
            getValue: () => config.ChatMenuNextKey,
            setValue: value => config.ChatMenuNextKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Chat Menu - Previous Message",
            tooltip: () => "Read the previous chat message.",
            getValue: () => config.ChatMenuPreviousKey,
            setValue: value => config.ChatMenuPreviousKey = value
        );
    }

    private static void AddReadTilePage(IGenericModConfigMenuApi configMenu, IManifest manifest, ModConfig config)
    {
        configMenu.AddPage(manifest, "stardew-access-read-tile", () => "Read Tile Settings");

        configMenu.AddBoolOption(
            manifest,
            name: () => "Enable Read Tile",
            tooltip: () => "Toggle tile reading feature.",
            getValue: () => config.ReadTile,
            setValue: value => config.ReadTile = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Read Tile (Look At Tile)",
            tooltip: () => "Trigger reading for the tile the player is looking at.",
            getValue: () => config.ReadTileKey,
            setValue: value => config.ReadTileKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Read Tile (Standing Tile)",
            tooltip: () => "Trigger reading for the tile the player is standing on.",
            getValue: () => config.ReadStandingTileKey,
            setValue: value => config.ReadStandingTileKey = value
        );

        configMenu.AddBoolOption(
            manifest,
            name: () => "Read Flooring",
            tooltip: () => "Toggle reading floor tiles.",
            getValue: () => config.ReadFlooring,
            setValue: value => config.ReadFlooring = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Read Flooring Key",
            tooltip: () => "Keybind to manually trigger reading of flooring.",
            getValue: () => config.ReadFlooringKey,
            setValue: value => config.ReadFlooringKey = value
        );

        configMenu.AddBoolOption(
            manifest,
            name: () => "Disable Descriptive Flooring",
            tooltip: () =>
                "Toggle between descriptive flooring names and generic ones (e.g., pathway, stepping stone).",
            getValue: () => config.DisableDescriptiveFlooring,
            setValue: value => config.DisableDescriptiveFlooring = value
        );

        configMenu.AddBoolOption(
            manifest,
            name: () => "Announce Watered/Unwatered",
            tooltip: () => "Toggle speaking watered/unwatered status for crops.",
            getValue: () => config.WateredToggle,
            setValue: value => config.WateredToggle = value
        );

        configMenu.AddBoolOption(
            manifest,
            name: () => "Enable Debug Info (Tile Indexes, Object IDs)",
            tooltip: () => "Toggle speaking extra debug info with tiles (indexes, object IDs, etc.).",
            getValue: () => config.ReadTileDebug,
            setValue: value => config.ReadTileDebug = value
        );

        configMenu.AddBoolOption(
            manifest,
            name: () => "Read Hoed Dirt in Mine Shafts",
            tooltip: () => "Toggle speaking hoed dirt when inside mine shafts.",
            getValue: () => config.ReadHoedDirtInMineShafts,
            setValue: value => config.ReadHoedDirtInMineShafts = value
        );
    }

    private static void AddTileViewerPage(IGenericModConfigMenuApi configMenu, IManifest manifest, ModConfig config)
    {
        configMenu.AddPage(manifest, "stardew-access-tile-viewer", () => "Tile Viewer Settings");

        configMenu.AddKeybindList(
            manifest,
            name: () => "Move Cursor Up",
            tooltip: () => "Move the cursor one tile up.",
            getValue: () => config.TileCursorUpKey,
            setValue: value => config.TileCursorUpKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Move Cursor Right",
            tooltip: () => "Move the cursor one tile right.",
            getValue: () => config.TileCursorRightKey,
            setValue: value => config.TileCursorRightKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Move Cursor Down",
            tooltip: () => "Move the cursor one tile down.",
            getValue: () => config.TileCursorDownKey,
            setValue: value => config.TileCursorDownKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Move Cursor Left",
            tooltip: () => "Move the cursor one tile left.",
            getValue: () => config.TileCursorLeftKey,
            setValue: value => config.TileCursorLeftKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Precision Move Up",
            tooltip: () => "Move the cursor up by pixels (precision).",
            getValue: () => config.TileCursorPreciseUpKey,
            setValue: value => config.TileCursorPreciseUpKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Precision Move Right",
            tooltip: () => "Move the cursor right by pixels (precision).",
            getValue: () => config.TileCursorPreciseRightKey,
            setValue: value => config.TileCursorPreciseRightKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Precision Move Down",
            tooltip: () => "Move the cursor down by pixels (precision).",
            getValue: () => config.TileCursorPreciseDownKey,
            setValue: value => config.TileCursorPreciseDownKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Precision Move Left",
            tooltip: () => "Move the cursor left by pixels (precision).",
            getValue: () => config.TileCursorPreciseLeftKey,
            setValue: value => config.TileCursorPreciseLeftKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Toggle Relative Cursor Lock",
            tooltip: () => "Toggle cursor lock (cursor resets when player moves).",
            getValue: () => config.ToggleRelativeCursorLockKey,
            setValue: value => config.ToggleRelativeCursorLockKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Auto Walk to Tile",
            tooltip: () => "Automatically walk to the tile where cursor is placed.",
            getValue: () => config.AutoWalkToTileKey,
            setValue: value => config.AutoWalkToTileKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Open Tile Info Menu",
            tooltip: () => "Open tile info menu for the active tile.",
            getValue: () => config.OpenTileInfoMenuKey,
            setValue: value => config.OpenTileInfoMenuKey = value
        );

        configMenu.AddBoolOption(
            manifest,
            name: () => "Limit Cursor to Screen",
            tooltip: () => "Prevent cursor from going out of the screen.",
            getValue: () => config.LimitTileCursorToScreen,
            setValue: value => config.LimitTileCursorToScreen = value
        );

        configMenu.AddNumberOption(
            manifest,
            name: () => "Precision Movement Distance",
            tooltip: () => "Number of pixels to move the cursor in precision mode.",
            getValue: () => config.TileCursorPreciseMovementDistance,
            setValue: value => config.TileCursorPreciseMovementDistance = value,
            min: 1,
            max: 64
        );
    }

    private static void AddRadarPage(IGenericModConfigMenuApi configMenu, IManifest manifest, ModConfig config)
    {
        configMenu.AddPage(manifest, "stardew-access-radar", () => "Radar Settings");

        configMenu.AddBoolOption(
            manifest,
            name: () => "Enable Radar",
            tooltip: () => "Toggle the radar feature on or off.",
            getValue: () => config.Radar,
            setValue: value => config.Radar = value
        );

        configMenu.AddBoolOption(
            manifest,
            name: () => "Use Stereo Sound",
            tooltip: () => "Toggle whether radar uses stereo or mono sound.",
            getValue: () => config.RadarStereoSound,
            setValue: value => config.RadarStereoSound = value
        );
    }

    private static void AddMenuKeysPage(IGenericModConfigMenuApi configMenu, IManifest manifest, ModConfig config)
    {
        configMenu.AddPage(manifest, "stardew-access-menu-keys", () => "Menu Keys Settings");

        configMenu.AddSectionTitle(manifest, () => "Common");

        configMenu.AddKeybindList(
            manifest,
            name: () => "Primary Info Key",
            tooltip: () => "Key to speak additional info on certain menus.",
            getValue: () => config.PrimaryInfoKey,
            setValue: value => config.PrimaryInfoKey = value
        );

        // --- Character Creation Menu Keys ---
        configMenu.AddSectionTitle(manifest, () => "Character Creation Menu");

        configMenu.AddKeybindList(
            manifest,
            name: () => "Design Toggle",
            tooltip: () => "Toggle displaying character design options.",
            getValue: () => config.CharacterCreationMenuDesignToggleKey,
            setValue: value => config.CharacterCreationMenuDesignToggleKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Input Modifier",
            tooltip: () => "When pressed, the sliders are increased/decreased in multiples of 10 rather than 1.",
            getValue: () => config.CharacterCreationMenuInputModifierKey,
            setValue: value => config.CharacterCreationMenuInputModifierKey = value
        );

        configMenu.AddBoolOption(
            manifest,
            name: () => "Design Toggle Default Enabled",
            tooltip: () => "If true, character design options are enabled by default.",
            getValue: () => config.CharacterCreationMenuDesignDefaultEnabled,
            setValue: value => config.CharacterCreationMenuDesignDefaultEnabled = value
        );

        // --- Bundle Menu Keys ---
        configMenu.AddSectionTitle(manifest, () => "Bundle Menu");

        configMenu.AddKeybindList(
            manifest,
            name: () => "Cycle Bundle Ingredients",
            tooltip: () => "Cycle through ingredients in the selected bundle.",
            getValue: () => config.BundleMenuIngredientsKey,
            setValue: value => config.BundleMenuIngredientsKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Cycle Inventory Items",
            tooltip: () => "Cycle through items in player's inventory.",
            getValue: () => config.BundleMenuInventoryItemsKey,
            setValue: value => config.BundleMenuInventoryItemsKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Move to Purchase Button",
            tooltip: () => "Move cursor to the purchase button.",
            getValue: () => config.BundleMenuPurchaseButtonKey,
            setValue: value => config.BundleMenuPurchaseButtonKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Cycle Input Slots",
            tooltip: () => "Cycle through the ingredient input slots.",
            getValue: () => config.BundleMenuIngredientsInputSlotKey,
            setValue: value => config.BundleMenuIngredientsInputSlotKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Move to Back Button",
            tooltip: () => "Move cursor to the back button.",
            getValue: () => config.BundleMenuBackButtonKey,
            setValue: value => config.BundleMenuBackButtonKey = value
        );

        // --- Menus with Secondary Inventory ---
        configMenu.AddSectionTitle(manifest, () => "Inventory Shortcuts");

        configMenu.AddKeybindList(
            manifest,
            name: () => "Snap to First Inventory Slot",
            tooltip: () => "Snap to the first slot in the primary inventory.",
            getValue: () => config.SnapToFirstInventorySlotKey,
            setValue: value => config.SnapToFirstInventorySlotKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Snap to First Secondary Inventory Slot",
            tooltip: () => "Snap to the first slot in the secondary inventory.",
            getValue: () => config.SnapToFirstSecondaryInventorySlotKey,
            setValue: value => config.SnapToFirstSecondaryInventorySlotKey = value
        );

        // --- Crafting Menu ---
        configMenu.AddSectionTitle(manifest, () => "Crafting Menu");

        configMenu.AddKeybindList(
            manifest,
            name: () => "Cycle Recipes",
            tooltip: () => "Cycle through recipes in the crafting menu.",
            getValue: () => config.CraftingMenuCycleThroughRecipesKey,
            setValue: value => config.CraftingMenuCycleThroughRecipesKey = value
        );
    }

    private static void AddGridMovementPage(IGenericModConfigMenuApi configMenu, IManifest manifest, ModConfig config)
    {
        configMenu.AddPage(manifest, "stardew-access-grid-movement", () => "Grid Movement Settings");

        configMenu.AddBoolOption(
            manifest,
            name: () => "Enable Grid Movement",
            tooltip: () => "Enable or disable grid movement feature.",
            getValue: () => config.GridMovementActive,
            setValue: value => config.GridMovementActive = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Toggle Grid Movement",
            tooltip: () => "Key to toggle grid movement.",
            getValue: () => config.ToggleGridMovementKey,
            setValue: value => config.ToggleGridMovementKey = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Grid Movement Override",
            tooltip: () => "Hold to disable grid movement temporarily.",
            getValue: () => config.GridMovementOverrideKey,
            setValue: value => config.GridMovementOverrideKey = value
        );

        configMenu.AddNumberOption(
            manifest,
            name: () => "Movement Speed (%)",
            tooltip: () => "Player movement speed in percentage (0 - 120%).",
            getValue: () => (float)config.GridMovementSpeedPercent,
            setValue: value => config.GridMovementSpeedPercent = value,
            min: 0,
            max: 120,
            interval: 1
        );

        configMenu.AddNumberOption(
            manifest,
            name: () => "Tiles Per Step",
            tooltip: () => "Number of tiles taken per step.",
            getValue: () => config.GridMovementTilesPerStep,
            setValue: value => config.GridMovementTilesPerStep = value,
            min: 1,
            max: 5
        );

        configMenu.AddNumberOption(
            manifest,
            name: () => "Delay After First Step (ms)",
            tooltip: () => "Delay in milliseconds after first step.",
            getValue: () => config.GridMovementDelayAfterFirstStep,
            setValue: value => config.GridMovementDelayAfterFirstStep = value,
            min: 0,
            max: 1000,
            interval: 50
        );
    }

    private static void AddObjectTrackerPage(IGenericModConfigMenuApi configMenu, IManifest manifest, ModConfig config)
    {
        configMenu.AddPage(manifest, "stardew-access-object-tracker", () => "Object Tracker Settings");

        // --- Category Cycling ---
        configMenu.AddKeybindList(
            manifest,
            name: () => "Cycle Up Category",
            tooltip: () => "Cycle up through the object categories.",
            getValue: () => config.OTCycleUpCategory,
            setValue: value => config.OTCycleUpCategory = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Cycle Down Category",
            tooltip: () => "Cycle down through the object categories.",
            getValue: () => config.OTCycleDownCategory,
            setValue: value => config.OTCycleDownCategory = value
        );

        // --- Favorites ---
        configMenu.AddKeybindList(
            manifest,
            () => config.OTFavorite1,
            value => config.OTFavorite1 = value,
            () => "Favorite 1",
            () => "Favorite slot 1 keybinding."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.OTFavorite2,
            value => config.OTFavorite2 = value,
            () => "Favorite 2",
            () => "Favorite slot 2 keybinding."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.OTFavorite3,
            value => config.OTFavorite3 = value,
            () => "Favorite 3",
            () => "Favorite slot 3 keybinding."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.OTFavorite4,
            value => config.OTFavorite4 = value,
            () => "Favorite 4",
            () => "Favorite slot 4 keybinding."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.OTFavorite5,
            value => config.OTFavorite5 = value,
            () => "Favorite 5",
            () => "Favorite slot 5 keybinding."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.OTFavorite6,
            value => config.OTFavorite6 = value,
            () => "Favorite 6",
            () => "Favorite slot 6 keybinding."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.OTFavorite7,
            value => config.OTFavorite7 = value,
            () => "Favorite 7",
            () => "Favorite slot 7 keybinding."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.OTFavorite8,
            value => config.OTFavorite8 = value,
            () => "Favorite 8",
            () => "Favorite slot 8 keybinding."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.OTFavorite9,
            value => config.OTFavorite9 = value,
            () => "Favorite 9",
            () => "Favorite slot 9 keybinding."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.OTFavorite10,
            value => config.OTFavorite10 = value,
            () => "Favorite 10",
            () => "Favorite slot 10 keybinding."
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Decrease Favorite Stack",
            tooltip: () => "Decrease favorite stack count.",
            getValue: () => config.OTFavoriteDecreaseStack,
            setValue: value => config.OTFavoriteDecreaseStack = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Increase Favorite Stack",
            tooltip: () => "Increase favorite stack count.",
            getValue: () => config.OTFavoriteIncreaseStack,
            setValue: value => config.OTFavoriteIncreaseStack = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Toggle Save Coordinates",
            tooltip: () => "Toggle saving coordinates to favorites.",
            getValue: () => config.OTFavoriteSaveCoordinatesToggle,
            setValue: value => config.OTFavoriteSaveCoordinatesToggle = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Save Default Favorite",
            tooltip: () => "Save default favorite position.",
            getValue: () => config.OTFavoriteSaveDefault,
            setValue: value => config.OTFavoriteSaveDefault = value
        );

        // --- Object Cycling ---
        configMenu.AddKeybindList(
            manifest,
            name: () => "Cycle Up Object",
            tooltip: () => "Cycle up through objects.",
            getValue: () => config.OTCycleUpObjectGroup,
            setValue: value => config.OTCycleUpObjectGroup = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Cycle Down Object",
            tooltip: () => "Cycle down through objects.",
            getValue: () => config.OTCycleDownObjectGroup,
            setValue: value => config.OTCycleDownObjectGroup = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Move to Selected Object",
            tooltip: () => "Move to the currently selected object.",
            getValue: () => config.OTMoveToSelectedObject,
            setValue: value => config.OTMoveToSelectedObject = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Read Selected Object Info",
            tooltip: () => "Read info about the currently selected object.",
            getValue: () => config.OTReadSelectedObject,
            setValue: value => config.OTReadSelectedObject = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Read Object Tile Location",
            tooltip: () => "Read info about the selected object's tile location.",
            getValue: () => config.OTReadSelectedObjectTileLocation,
            setValue: value => config.OTReadSelectedObjectTileLocation = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Cancel Auto Walking",
            tooltip: () => "Stop auto walking to selected object.",
            getValue: () => config.OTCancelAutoWalking,
            setValue: value => config.OTCancelAutoWalking = value
        );

        configMenu.AddKeybindList(
            manifest,
            name: () => "Switch Sorting Mode",
            tooltip: () => "Toggle between proximity sorting and alphabetical sorting.",
            getValue: () => config.OTSwitchSortingMode,
            setValue: value => config.OTSwitchSortingMode = value
        );

        // --- Toggles ---
        configMenu.AddBoolOption(
            manifest,
            name: () => "Auto Refreshing",
            tooltip: () => "Enable automatic refreshing of object list.",
            getValue: () => config.OTAutoRefreshing,
            setValue: value => config.OTAutoRefreshing = value
        );

        configMenu.AddBoolOption(
            manifest,
            name: () => "Sort by Proximity",
            tooltip: () => "Enable proximity as default sorting mode.",
            getValue: () => config.OTSortByProximity,
            setValue: value => config.OTSortByProximity = value
        );

        configMenu.AddBoolOption(
            manifest,
            name: () => "Wrap Lists",
            tooltip: () => "If enabled, cycling past the last object wraps to the first.",
            getValue: () => config.OTWrapLists,
            setValue: value => config.OTWrapLists = value
        );

        configMenu.AddBoolOption(
            manifest,
            name: () => "Remember Position",
            tooltip: () => "Remember previously selected object position.",
            getValue: () => config.OTRememberPosition,
            setValue: value => config.OTRememberPosition = value
        );
    }

    private static void AddOtherPage(IGenericModConfigMenuApi configMenu, IManifest manifest, ModConfig config)
    {
        configMenu.AddPage(manifest, "stardew-access-other", () => "Other Settings");

        // Narration keys and toggles
        configMenu.AddKeybindList(
            manifest,
            () => config.HealthNStaminaKey,
            value => config.HealthNStaminaKey = value,
            () => "Narrate Health and Stamina",
            () => "Key to narrate current health and stamina."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.HealthNStaminaInPercentage,
            value => config.HealthNStaminaInPercentage = value,
            () => "Speak as Percentage",
            () => "If true, health and stamina will be spoken in percentage rather than raw values."
        );

        configMenu.AddKeybindList(
            manifest,
            () => config.PositionKey,
            value => config.PositionKey = value,
            () => "Narrate Player Position",
            () => "Key to narrate your current X/Y position."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.LocationKey,
            value => config.LocationKey = value,
            () => "Narrate Location Name",
            () => "Key to narrate your current location's name."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.MoneyKey,
            value => config.MoneyKey = value,
            () => "Narrate Money",
            () => "Key to narrate how much money you currently have."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.TimeNSeasonKey,
            value => config.TimeNSeasonKey = value,
            () => "Narrate Time, Date, and Season",
            () => "Key to narrate time of day, day/date, and season."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.RepeatLastTextKey,
            value => config.RepeatLastTextKey = value,
            () => "Repeat Last Narrated Text",
            () => "Key to repeat the last spoken narration."
        );

        // General toggles
        configMenu.AddBoolOption(
            manifest,
            () => config.VerboseCoordinates,
            value => config.VerboseCoordinates = value,
            () => "Verbose Coordinates",
            () => "If enabled, narrates coordinates with 'X:' and 'Y:' prefixes."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.SnapMouse,
            value => config.SnapMouse = value,
            () => "Snap Mouse to Interactive Elements",
            () => "If enabled, the mouse will snap to clickable UI elements automatically."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.Warning,
            value => config.Warning = value,
            () => "Enable Warnings",
            () => "Toggles additional in-game warning messages."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.TTS,
            value => config.TTS = value,
            () => "Enable Screen Reader / TTS",
            () => "Enable or disable the built-in screen reader or text-to-speech engine."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.AutoReadCharacterBubbles,
            value => config.AutoReadCharacterBubbles = value,
            () => "Auto Read Character Bubbles",
            () => "Automatically read character speech bubbles."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.AutoReadCharacterDialog,
            value => config.AutoReadCharacterDialog = value,
            () => "Auto Read Character Dialog",
            () => "Automatically read character dialogs."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.AutoReadQuestionDialog,
            value => config.AutoReadQuestionDialog = value,
            () => "Auto Read Question Dialog",
            () => "Automatically read question dialogs."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.AutoReadBasicDialog,
            value => config.AutoReadBasicDialog = value,
            () => "Auto Read Basic Dialog",
            () => "Automatically read basic dialogs like 'no mail in mailbox'."
        );
        configMenu.AddKeybindList(
            manifest,
            () => config.ManualReadDialogKey,
            value => config.ManualReadDialogKey = value,
            () => "Manual Read Current Dialog",
            () => "Key to manually read the currently shown dialog."
        );

        // Misc toggles
        configMenu.AddBoolOption(
            manifest,
            () => config.TrackDroppedItems,
            value => config.TrackDroppedItems = value,
            () => "Track Dropped Items",
            () => "If enabled, narrates nearby dropped items."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.DisableInventoryVerbosity,
            value => config.DisableInventoryVerbosity = value,
            () => "Reduce Inventory Verbosity",
            () => "Disables some verbose information inside inventory screens."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.DisableBushVerbosity,
            value => config.DisableBushVerbosity = value,
            () => "Reduce Bush Descriptions",
            () => "If enabled, bushes will only be described when harvestable."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.DisableDescriptiveDebris,
            value => config.DisableDescriptiveDebris = value,
            () => "Disable Debris Descriptions",
            () => "Suppresses narration of debris (like wood, rocks) details."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.DisableColorfulSlime,
            value => config.DisableColorfulSlime = value,
            () => "Disable Slime Color Info",
            () => "Prevents narration of slime color information."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.ExtraColors,
            value => config.ExtraColors = value,
            () => "Enable Extra Color Descriptions",
            () => "Enables expanded color palette descriptions."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.DisableInventoryFluentPluralization,
            value => config.DisableInventoryFluentPluralization = value,
            () => "Disable Inventory Pluralization",
            () => "Disables automatic pluralization of inventory items using Fluent."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.YouveGotMailSound,
            value => config.YouveGotMailSound = value,
            () => "Use Mail Notification Sound",
            () => "If disabled, plays a narrated 'You've got mail!' message instead of the notification sound."
        );
        configMenu.AddBoolOption(
            manifest,
            () => config.EnableCheats,
            value => config.EnableCheats = value,
            () => "Enable Cheats",
            () => "Toggles in-game cheat/debug commands."
        );
        configMenu.AddNumberOption(
            manifest,
            () => config.HudDuplicateMessageTimeout,
            value => config.HudDuplicateMessageTimeout = value,
            () => "HUD Duplicate Message Timeout (ms)",
            () => "The time window (in milliseconds) to suppress duplicate HUD messages. If a duplicate message occurs within this interval, it will be ignored. Default is 5 miliseconds. <see langword=\"set\"/> to 0 to disable this feature."
        );

        // Fishing tweaks
        configMenu.AddNumberOption(
            manifest,
            () => config.MaximumFishingDifficulty,
            value => config.MaximumFishingDifficulty = value,
            () => "Max Fishing Difficulty",
            () => "Caps the maximum difficulty any fish can have.",
            min: 0,
            max: 150
        );
        configMenu.AddNumberOption(
            manifest,
            () => config.FixFishingMotionType,
            value => config.FixFishingMotionType = value,
            () => "Fix Fishing Motion Type",
            () => "Fixes the motion type of all fish. 0 = Mixed, 1 = Dart, 2 = Smooth, 3 = Sinker, 4 = Floater.",
            min: 0,
            max: 4
        );

        // Miscellaneous tweaks
        configMenu.AddBoolOption(
            manifest,
            () => config.Use24HourFormat,
            value => config.Use24HourFormat = value,
            () => "Use 24-Hour Time Format",
            () => "Narrates time in 24-hour format instead of 12-hour."
        );
        configMenu.AddNumberOption(
            manifest,
            () => config.EggHuntTimerMultiplier,
            value => config.EggHuntTimerMultiplier = value,
            () => "Egg Hunt Timer Multiplier",
            () => "Scales the Egg Hunt timer length (1x to 3x).",
            min: 1f,
            max: 3f,
            interval: 0.1f
        );
        configMenu.AddNumberOption(
            manifest,
            () => config.MacSpeechRate,
            value => config.MacSpeechRate = value,
            () => "Mac TTS Speech Rate",
            () => "Sets the Mac TTS speech rate (recommend between 100-500).",
            min: 100f,
            max: 500f,
            interval: 10f
        );
    }
}