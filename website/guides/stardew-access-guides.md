# Stardew Access

This guide covers all core user features added by Stardew Access, how they work, and how to use them.

## Table of Contents

- [Using The Tile Viewer](#using-the-tile-viewer)
- [Using The Object Tracker](#using-the-object-tracker)
    - [Object Tracker](#object-tracker)
    - [Object Tracker Favorites](#object-tracker-favorites)
        - [Tracking Coordinates With Favorites](#tracking-coordinates-with-favorites)
- [Other Guides](#other-guides)

## Using The Tile Viewer

The tile viewer allows you to move the tile cursor, review the map tile by tile without moving your character, move your character to the selected tile, and get information about the selected tile.
Use the arrow keys to move the tile cursor to the desired location. The cursor automatically follows the tile cursor as the cursor moves. This allows you to interact with nearby items, such as the following: watering crops, tilling soil, placing furniture, and more.
You can move your character to the selected tile with `primary left click` (default is )`left ctrl + enter`).
You can get information about a tile by using `primary right click` (default is `left shift + enter`).
This will open a dialogue which provides you with various options:

1. Mark this tile
    - adds the selected tile to the mark index of your choice. This feature was formerly used for constructing farm buildings.
    - Use command `marklist` in the SMAPI console to show all marked positions
2. Add this tile to user tile data
    - Allows you to mark a tile and assign it a category so that it appears in the object tracker
    - You can enable additional checks, such as active quests, mod dependencies, farm type, or whether the player is a Joja member
3. Speak detailed tile info
    - Get more information about the selected tile

This feature can be remapped to a controller/gamepad via the `config.json` file. More info available in [keybindings](../keybindings.md#tile-viewer-keys) and [configs](../config.md#tile-viewer-configs).

## Using The Object Tracker

The object tracker is an extremely helpful feature of Stardew Access. It lets you browse all of the items, NPCs, interactable items, and other points of interest on the current map. For a full list of keys and config options, check out the relevant sections in [keybinds](../keybindings.md#object-tracker-keys) and [configs](../config.md#object-tracker-configs).

- [Object Tracker](#object-tracker)
- [Object Tracker Favorites](#object-tracker-favorites)
    - [Tracking Coordinates With Favorites](#tracking-coordinates-with-favorites)

### Object Tracker

The object tracker does two things: it lists all available objects in their respective categories, and it allows you to get their position and travel to them.
Categories can be browsed with `left ctrl +pageUp` and `left ctrl + pageDown`. They are listed in alphabetical order, so "doors" will always come before "mine items" which will always come before "pending" which will always come before "resources" and so on.
Not all categories will show up in all locations. The categories will only be visible if there is an object that fits that category on the current map. If you are in the town, the chances of seeing the "mine items" category will be very, very, very low.

To browse objects within a category, use `pageUp` and `pageDown`. objects will appear within each category in the order of their proximity to you.
IF you wish for them to show in alphabetical order, press `~ (tilde)` to toggle between proximity and alphabetical order. Note that each time the list refreshes, the order of objects will change if proximity is enabled, but categories will not change their order. If a category is visible but all items that pertain to it are removed from the map, that category will disappear once the object list refreshes. Your focus will be moved to a visible category.

To get info on where an object is, use `home` or `end`. `home` will provide both relative directions (north 5) and absolute coordinates of the player and the object. `End` will only provide absolute coordinates. Pressing either of these keys will refresh the objects list.

### Object Tracker Favorites

You can set and manage favorite objects on a per-map basis. This portion of the object tracker uses the `left alt` and `right alt` keys interchangeably, so this guide will refer to them collectively as `alt`.

Each map has a set of favorites which can be set by holding `alt` and double-tapping a number on the number row. Be sure to focus your object tracker on the object which you want to set as the favorite first. Use the numbers on the number row, 1 through 0 to access 10 favorites at a time.
Once a favorite is set, double-tap that number while holding `alt` to travel to that object. To hear the object location, single-tap the respective number while holding `alt`. To erase a favorite object, triple-tap its respective number while holding `alt`.

To access more than 10 favorite objects, hold `alt` and press `+ (plus)` or `- (minus)` to browse through favorite stacks. Each map can have up to several hundred stacks, giving you access to many thousands of favorites per map.

#### Tracking coordinates With Favorites

To track a specific tile coordinate, press `tab + ~` to toggle saving coordinates. When the feature is enabled, hold `alt` and double-tap the desired number on an empty favorite to save the coordinate. Don't worry about what object the object tracker is focused on. To stop saving coordinates and return to normal functionality, disable saving coordinates with the same keystroke.

## Other guides

- [General Guides](general-guides.md)
- [Farming Guides](farming-guides.md)
- [Mining Guides](mining-guides.md)

**[Back to guides home](guides-home.md)

**[Back to readme...](../README.md)**
