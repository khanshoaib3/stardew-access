# Config

The mod can be configured by a `config.json` file that can be found in the mod's folder (if you can't find it than try
running game once).
The file is a .json type so you can edit it using any text editor on your computer or you can use online json editors
like [Json Editor Online](https://jsoneditoronline.org/).
For the list of button codes
visit [Player Guide/Key Bindings](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Button_codes).

## Table Of Contents

* [Config List](#config-list)
    * [Mouse Sim Keys Config](#mouse-sim-keys-config)
    * [Read Tile Configs](#read-tile-configs)
    * [Tile Viewer Configs](#tile-viewer-configs)
    * [Grid Movement Configs](#grid-movement-configs)
    * [Object Tracker Configs](#object-tracker-configs)
    * [Radar Configs](#radar-configs)
    * [Menu Keys Configs](#menu-keys-configs)
        * [Common Keys](#common-keys)
        * [Character Creation Menu (New Game Menu) Keys](#character-creation-menu-new-game-menu-keys)
        * [Junimo Note or Community Center Menu Keys](#junimo-note-or-community-center-menu-keys)
        * [Chat Menu Configs](#chat-menu-configs)
    * [Fishing Mini-Game Configs](#fishing-mini-game-configs)
    * [Other Configs](#other-configs)
* [Default Config](#default-config)
* [Other Pages](#other-pages)

## Config List

### Mouse Sim Keys Config

| Name                   | Default Value       | Description                                 |
|------------------------|---------------------|---------------------------------------------|
| LeftClickMainKey       | LeftControl + Enter | Primary key to simulate mouse left click    |
| RightClickMainKey      | LeftShift + Enter   | Primary key to simulate mouse right click   |
| LeftClickAlternateKey  | OemOpenBrackets     | Secondary key to simulate mouse left click  |
| RightClickAlternateKey | OemCloseBrackets    | Secondary key to simulate mouse right click |

### Read Tile Configs

| Name                     | Default Value | Description                                             |
|--------------------------|---------------|---------------------------------------------------------|
| ReadTile                 | true          | Toggle `Read Tile` feature.                             |
| ReadTileKey              | J             | Speak the contents of the tile player is _looking at_.  |
| ReadStandingTileKey      | LeftAlt + J   | Speak the contents of the tile player is _standing on_. |
| ReadFlooring             | false         | Toggle reading floorings.                               |
| WateredToggle            | true          | Toggle speaking watered or unwatered for crops.         |
| ReadTileIndexes          | false         | Toggle speaking tile indexes with other info.           |
| ReadHoedDirtInMineShafts | false         | Toggle speaking hoed dirt (soil) in mine shafts.        |

### Tile Viewer Configs

| Name                              | Default Value       | Description                                                                                                 |
|-----------------------------------|---------------------|-------------------------------------------------------------------------------------------------------------|
| ToggleRelativeCursorLockKey       | L                   | Toggles relative cursor lock i.e. if enabled, the cursor will reset when player moves.                      |
| AutoWalkToTileKey                 | LeftControl + Enter | Auto walk to the tile                                                                                       |
| OpenTileInfoMenuKey               | LeftShift + Enter   | Opens the Tile Info menu for the active tile.                                                               |
| TileCursorUpKey                   | Up                  | Move the cursor one tile up                                                                                 |
| TileCursorRightKey                | Right               | Move the cursor one tile right                                                                              |
| TileCursorDownKey                 | Down                | Move the cursor one tile down                                                                               |
| TileCursorLeftKey                 | Left                | Move the cursor one tile left                                                                               |
| TileCursorPreciseUpKey            | LeftShift + Up      | Move the cursor up by precision i.e. pixel by pixel                                                         |
| TileCursorPreciseRightKey         | LeftShift + Right   | Move the cursor right by precision i.e. pixel by pixel                                                      |
| TileCursorPreciseDownKey          | LeftShift + Down    | Move the cursor down by precision i.e. pixel by pixel                                                       |
| TileCursorPreciseLeftKey          | LeftShift + Left    | Move the cursor left by precision i.e. pixel by pixel                                                       |
| LimitTileCursorToScreen           | false               | Toggle whether to prevent cursor from going out of screen.                                                  |
| TileCursorPreciseMovementDistance | 8                   | Specifies the number of pixels the cursor should move when using precision movement i.e. with _left shift_. |

### Grid Movement Configs

| Name                                  | Default Value | Description                                         |
|---------------------------------------|---------------|-----------------------------------------------------|
| GridMovementActive                    | true          | Enable or disable grid movement feature.            |
| ToggleGridMovementKey                 | I             | Toggle grid movement.                               |
| GridMovementOverrideKey               | LeftControl   | Disable Grid Movement while held                    |
| GridMovementSpeed                     | 100           | Player movement speed (in percentage).              |
| GridMovementTilesPerStep              | 1             | Tiles taken per step.                               |
| GridMovementDelayAfterDirectionChange | 250           | Delay after changing the player's facing direction. |

### Object Tracker Configs

| Name                             | Default Value          | Description                                                   |
|----------------------------------|------------------------|---------------------------------------------------------------|
| OTCycleUpCategory                | LeftControl + PageUp   | Cycle Up Category                                             |
| OTCycleDownCategory              | LeftControl + PageDown | Cycle Down Category                                           |
| OTCycleUpObject                  | PageUp                 | Cycle Up Object                                               |
| OTCycleDownObject                | PageDown               | Cycle Down Object                                             |
| OTMoveToSelectedObject           | LeftControl + Home     | Move to the currently selected object.                        |
| OTReadSelectedObject             | Home                   | Read info about the currently selected object.                |
| OTReadSelectedObjectTileLocation | End                    | Read info about the currently selected objects tile location. |
| OTCancelAutoWalking              | Escape                 | Manually stop Auto Walking.                                   |
| OTSwitchSortingMode              | OemTilde               | Toggle proximity sorting vs alphabetical                      |
| OTSortByProximity                | true                   | If enabled, the default sorting mode will be proximity.       |
| OTAutoRefreshing                 | true                   |                                                               |
| OTWrapLists                      | false                  |                                                               |

### Radar Configs

| Name             | Default Value | Description                                |
|------------------|---------------|--------------------------------------------|
| Radar            | false         | Toggle Radar feature.                      |
| RadarStereoSound | true          | Toggle whether to use stereo sound or mono |

### Menu Keys Configs

#### Common Keys

| Name                                 | Default Value | Description                                                                                   |
|--------------------------------------|---------------|-----------------------------------------------------------------------------------------------|
| PrimaryInfoKey                       | C             | Used to speak additional info on certain menus, [see here](keybindings.md#primary-info-key)   |
| SnapToFirstInventorySlotKey          | I             | Snaps to the first slot in primary inventory i.e., player inventory, shop inventory, etc.     |
| SnapToFirstSecondaryInventorySlotKey | LeftShift + I | Snaps to the first slot in secondary inventory i.e., chest inventory, selling inventory, etc. |
| CraftingMenuCycleThroughRecipesKey   | C             | Cycle through the recipes in crafting menu.                                                   |

#### Character Creation Menu (New Game Menu) Keys

| Name                                        | Default Value       | Description                                |
|---------------------------------------------|---------------------|--------------------------------------------|
| CharacterCreationMenuNextKey                | Right               | Go to next element                         |
| CharacterCreationMenuPreviousKey            | Left                | Go to previous element                     |
| CharacterCreationMenuDesignToggleKey        | LeftControl + Space | Toggle displaying character design options |
| CharacterCreationMenuSliderIncreaseKey      | Up                  | Increase the slider value by 1             |
| CharacterCreationMenuSliderLargeIncreaseKey | PageUp              | Increase the slider value by 10            |
| CharacterCreationMenuSliderDecreaseKey      | Down                | Decrease the slider value by 1             |
| CharacterCreationMenuSliderLargeDecreaseKey | PageDown            | Decrease the slider value by 10            |

#### Junimo Note or Community Center Menu Keys

| Name                              | Default Value | Description                                                  |
|-----------------------------------|---------------|--------------------------------------------------------------|
| BundleMenuIngredientsKey          | I             | Cycle through the ingredients in the current selected bundle |
| BundleMenuInventoryItemsKey       | C             | Cycle through the items in the player's inventory            |
| BundleMenuPurchaseButtonKey       | P             | Move the mouse cursor to purchase button                     |
| BundleMenuIngredientsInputSlotKey | V             | Cycle through the ingredient input slots                     |
| BundleMenuBackButtonKey           | Back          | Move the mouse cursor to back button                         |

#### Chat Menu Configs

| Name                | Default Value | Description                |
|---------------------|---------------|----------------------------|
| ChatMenuNextKey     | PageUp        | Read previous chat message |
| ChatMenuPreviousKey | PageDown      | Read next chat message     |

### Fishing Mini-Game Configs

| Name                     | Default Value | Description                                                                                                                                                                                                                                                                                                            |
|--------------------------|---------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| MaximumFishingDifficulty | 999           | Every fish have a difficulty value which varies from around 0 to 150. You can use this to limit the maximum difficulty any fish can have.                                                                                                                                                                              |
| FixFishingMotionType     | 999           | You can fix what motion type every fish has, by default every fish has a fixed motion type like for squid it's sinker, walleye it's smooth, etc. You can use a value between 0 to 4 to fix the motion. 0 indicates mixed motion type, 1 indicates art, 2 indicates smooth, 3 indicates sinker and 4 indicates floater. |

### Other Configs

| Name                       | Default Value | Description                                                                  |
|----------------------------|---------------|------------------------------------------------------------------------------|
| HealthNStaminaKey          | H             | Narrate health and stamina.                                                  |
| HealthNStaminaInPercentage | true          | Whether to speak health and stamina in percentage or the actual value        |
| List PositionKey           | K             | Narrate player position.                                                     |
| List LocationKey           | LeftAlt + K   | Narrate current location name.                                               |
| List MoneyKey              | R             | Narrate the money the player has currently.                                  |
| List TimeNSeasonKey        | Q             | Narrate the time of day, day and date and season                             |
| VerboseCoordinates         | true          | Whether to speak 'X:' and 'Y:' along with co-ordinates or not                |
| SnapMouse                  | true          | Toggles the snap mouse feature                                               |
| Warning                    | true          | Toggles the warnings feature                                                 |
| TTS                        | true          | Toggles the screen reader/tts.                                               |
| TrackDroppedItems          | true          | Toggles detecting the dropped items.                                         |
| DisableInventoryVerbosity  | false         | If enabled, does not speaks 'not usable here' and 'donatable' in inventories |
| DisableBushVerbosity       | false         | If enabled, does not speak bush type or size; only harvestable.              |
| MacSpeechRate              | 220           | Sets speech rate for the Mac TTS.                                            |

## Default Config

This is the default value of the `config.json` file as per `v1.5.0`:

~~~json
{
  "LeftClickMainKey": "LeftControl + Enter",
  "RightClickMainKey": "LeftShift + Enter",
  "LeftClickAlternateKey": "OemOpenBrackets",
  "RightClickAlternateKey": "OemCloseBrackets",
  "ChatMenuNextKey": "PageUp",
  "ChatMenuPreviousKey": "PageDown",
  "ReadTile": true,
  "ReadTileKey": "J",
  "ReadStandingTileKey": "LeftAlt + J",
  "ReadFlooring": false,
  "WateredToggle": true,
  "ReadTileIndexes": false,
  "TileCursorUpKey": "Up",
  "TileCursorRightKey": "Right",
  "TileCursorDownKey": "Down",
  "TileCursorLeftKey": "Left",
  "TileCursorPreciseUpKey": "LeftShift + Up",
  "TileCursorPreciseRightKey": "LeftShift + Right",
  "TileCursorPreciseDownKey": "LeftShift + Down",
  "TileCursorPreciseLeftKey": "LeftShift + Left",
  "ToggleRelativeCursorLockKey": "L",
  "AutoWalkToTileKey": "LeftControl + Enter",
  "OpenTileInfoMenuKey": "LeftShift + Enter",
  "LimitTileCursorToScreen": false,
  "TileCursorPreciseMovementDistance": 8,
  "Radar": false,
  "RadarStereoSound": true,
  "PrimaryInfoKey": "C",
  "CharacterCreationMenuNextKey": "Right",
  "CharacterCreationMenuPreviousKey": "Left",
  "CharacterCreationMenuSliderIncreaseKey": "Up",
  "CharacterCreationMenuSliderLargeIncreaseKey": "PageUp",
  "CharacterCreationMenuSliderDecreaseKey": "Down",
  "CharacterCreationMenuSliderLargeDecreaseKey": "PageDown",
  "CharacterCreationMenuDesignToggleKey": "LeftControl + Space",
  "BundleMenuIngredientsKey": "I",
  "BundleMenuInventoryItemsKey": "C",
  "BundleMenuPurchaseButtonKey": "P",
  "BundleMenuIngredientsInputSlotKey": "V",
  "BundleMenuBackButtonKey": "Back",
  "SnapToFirstInventorySlotKey": "I",
  "SnapToFirstSecondaryInventorySlotKey": "LeftShift + I",
  "CraftingMenuCycleThroughRecipesKey": "C",
  "GridMovementActive": true,
  "ToggleGridMovementKey": "I",
  "GridMovementOverrideKey": "LeftControl",
  "GridMovementSpeed": 100.0,
  "GridMovementTilesPerStep": 1,
  "GridMovementDelayAfterDirectionChange": 500,
  "OTCycleUpCategory": "LeftControl + PageUp",
  "OTCycleDownCategory": "LeftControl + PageDown",
  "OTCycleUpObject": "PageUp",
  "OTCycleDownObject": "PageDown",
  "OTMoveToSelectedObject": "LeftControl + Home",
  "OTReadSelectedObject": "Home",
  "OTReadSelectedObjectTileLocation": "End",
  "OTCancelAutoWalking": "Escape",
  "OTSwitchSortingMode": "OemTilde",
  "OTAutoRefreshing": true,
  "OTSortByProximity": true,
  "OTWrapLists": false,
  "OTRememberPosition": true,
  "HealthNStaminaKey": "H",
  "HealthNStaminaInPercentage": true,
  "PositionKey": "K",
  "LocationKey": "LeftAlt + K",
  "MoneyKey": "R",
  "TimeNSeasonKey": "Q",
  "VerboseCoordinates": true,
  "SnapMouse": true,
  "Warning": true,
  "TTS": true,
  "TrackDroppedItems": true,
  "DisableInventoryVerbosity": false,
  "DisableBushVerbosity": false,
  "MacSpeechRate": 220.0,
  "MaximumFishingDifficulty": 999,
  "FixFishingMotionType": 999
}
~~~

## Other Pages

- [Readme](README.html)
- [Setup](setup.md)
- [Features](features.md)
- [Keybindings](keybindings.md)
- [Commands](commands.md)
- [Guides](guides.md)
