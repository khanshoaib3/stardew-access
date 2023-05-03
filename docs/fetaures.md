# Features

This page contains details of all the features that are currently in the mod.
If you have any suggestion on improvements on any of these features or about a new feature, you can join the discord server ([here](https://discord.gg/yQjjsDqWQX)) or post an issue ([here](https://github.com/khanshoaib3/stardew-access/issues)).

## Feature List

### Read Tile

Reads the name and information about the tile the player is currently looking (not the one the player is standing on).
This feature uses in-game methods/properties to get the name and information of the object on the tile.
Many tiles don't have textual information so for that, the mod uses a json file to get the name of the tile.
You can find this file in the `assets` folder by the name `static-tiles.json`.
You can also create a `custom-tiles.json` file in the `assets` folder and add new entries which can also be made to work only when a certain mod is installed.
More on this in the guides page.

### Tile Viewer

Allows browsing of the map and snapping mouse to tiles with the arrow keys.
This Feature can be used to place items into the world like calender.
We can press the `AutoWalkToTileKey` (default to left control + enter) to auto walk the player to that tile (if possible).

### Warning

Warns the player when their health or stamina/energy is low.
Also warns when its past midnight.

### Others

Almost all the vanilla menus are patched to be made accessible by the mod.
For better navigation and mouse button simulation, the mod snaps the cursor to the tile adjacent to the player based on where the player is looking.
If you switch or minimize the game's window while it is still not paused, this feature will prevent the cursor from moving.
So be sure to pause the game or open any menu before switching/minimizing the window.
The mod also adds a few keybindings to speak player's current position, location, health, etc.
See the keybindings page for these keybinds.

### Radar

_Note that this is kinda an experimental feature so any feedback on how to improve this feature will be helpful_

Plays a sound at point of interest nearby, like chest, doors, harvestable item, etc.
The game not supporting stereo sound makes this feature somewhat confusing.

## Upcoming Features

### Translations

At the moment, the mod only has translations for one or two features.
We would like to change that and internationalize the entire mod hopefully.

### Tile Viewer Menu

When Shift + enter, it brings up a menu with options to mark that tile just like using the `mark` command and an option to add the tile as a new entry to `custom-tiles.json`.

