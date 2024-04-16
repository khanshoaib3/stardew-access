# Features

## Table Of Contents

* [Table Of Contents](#table-of-contents)
* [Feature List](#feature-list)
    * [Read Tile](#read-tile)
    * [Tile Viewer](#tile-viewer)
    * [Tile Info Menu](#tile-info-menu)
    * [Object Tracker](#object-tracker)
    * [Grid Movement](#grid-movement)
    * [Warning](#warning)
    * [Others](#others)
    * [Radar](#radar)
* [Other Pages](#other-pages)

## Feature List

### Read Tile

Reads the name and information about the tile the player is currently facing (not the one the player is standing on).
This feature uses in-game methods/properties to get the name and information of the object on the tile.
Many tiles don't have textual information so for that, the mod uses a json file to get the name of the tile.
You can find this file in the `assets/TilesData` folder by the name `tiles.json`.
You can also create a `tiles_user.json` file in the `assets/TilesData` folder and add new entries which can also be made
to work
only when a certain mod(s) is installed or on a specific farm type, etc.
You don't have to manually enter the entries, you can use the Tile Info Menu for that
but there will be a guide for manually adding entries into the `tiles_user.json` file soon.

See
related: [keybindings](keybindings.md#global-keys), [commands](commands.md#read-tile-related), [configs](config.md#read-tile-configs).

### Tile Viewer

Allows browsing of the map and snapping mouse to tiles with the arrow keys.
This Feature can be used to place items into the world like calendar.
We can press the `AutoWalkToTileKey` (default to left control + enter) to auto walk the player to that tile (if
possible).
Pressing the `OpenTileInfoMenuKey` (default to left shift + enter) to open the tile info menu for the active tile.

See related: [keybindings](keybindings.md#tile-viewer-keys), [configs](config.md#tile-viewer-configs).

### Tile Info Menu

Can be opened by using the `OpenTileInfoMenuKey` (default to Left Shift + Enter) while using the tile viewer.
The menu contains an option to mark a tile for the various building related operations.
Another option to add entries in the `tiles_user.json` for a tile.

**Current Limitations:**

1. The third option, the one that should speak the details about the tile only speaks the tile's name.
2. We can only have one mod as a dependency although having multiple in the `tiles_user.json` is supported.
3. We can only set the currently ongoing event or the current farm type as dependency.
4. Festivals are not properly detected.

### Object Tracker

Allows finding and tracking down objects on the map.
This feature also allows to auto navigate to an object or speaks it's relative distance from the player.

See related: [keybindings](keybindings.md#object-tracker-keys), [configs](config.md#object-tracker-configs).

### Grid Movement

When enabled, the player moves tile by tile instead of freely.
This feature is most helpful when planting/harvesting crops or in any case where precise movement is required.

_Note: In case you encounter the player moving more than one step or the speed being faster than usual,
try reducing the speed of grid movement from the config._

See related: [keybindings](keybindings.md#grid-movement-keys), [configs](config.md#grid-movement-configs).

### Warning

Warns the player when their health or stamina/energy is low.
Also warns when its past midnight.

### Others

Almost all the vanilla menus are patched to be made accessible by the mod.
For better navigation and mouse button simulation, the mod snaps the cursor to the tile adjacent to the player based on
where the player is looking.
If you switch or minimize the game's window while it is still not paused, this feature will prevent the cursor from
moving.
So be sure to pause the game or open any menu before switching/minimizing the window.
The mod also adds a few keybindings to speak player's current position, location, health, etc.
See the keybindings page for these keybinds.

### Radar

_Note that this is kinda an experimental feature so any feedback on how to improve this feature will be helpful_

Plays a sound at point of interest nearby, like chest, doors, harvestable item, etc.
The game not supporting stereo sound makes this feature somewhat confusing.

See related: [commands](commands.md#radar-related), [configs](config.md#radar-configs).

## Other Pages

- [Readme](README.md)
- [Setup](setup.md)
- [Keybindings](keybindings.md)
- [Commands](commands.md)
- [Configs](config.md)
- [Guides](guides.md)
