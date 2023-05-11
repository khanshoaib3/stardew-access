# Guides

This page contains guides on how to use the mod to play the game.
At the moment, the amount of guides is very few but hopefully this won't be the case by the next update.

## Table Of Contents

- [Guides List](#guides-list)
  - [Creating A New Game](#creating-a-new-game)
  - [Saving The Progress](#saving-the-progress)
  - [Using The Tile Viewer Feature](#using-the-tile-viewer-feature)
  - [Constructing/Upgrading/Moving Buildings](#constructingupgradingmovingdemolishing-buildings)
  - [Purchasing/Moving Farm Animals](#purchasingmoving-farm-animals)
- [Other Pages](#other-pages)

## Guides List

### Creating A New Game

This assumes that you have installed the game and the mod as well, go to the [setup page](setup.md) if you haven't already.
Once the game is loaded and you have entered the title screen, select the new game button to open the new game menu (also known as the character customization menu).
You can use wasd keys to navigate through the menus and `Control + Enter` or `[` key to select a button or other UI elements.
Once you're in the new game menu, use the left and right arrow keys to go to next or previous element and again, `Control + Enter` or `[` key to select it.
Check the full list of keybindings in this menu [here](keybindings.md/#new-game-or-character-customization-menu-keys).
In case of a text box, first select the element with `[` or `Control + Enter` and then type the text you want and press `Escape` key to unselect the text box.
And in case of a slider, use up and down arrow keys or page up and page down keys to change the value.
By default, options related to character appearance are hidden, press `Left Control + Space` to make it visible and then you can edit the farmer's appearance.
When you're done with the configuration, go to the `ok` button and select it which will trigger the intro cutscene (unless you've enabled `skip intro` option which will skip it).

<!-- ### Navigating The World And The Menus -->

### Saving The Progress

To save the progress you have to sleep in the bed at the farm house and to do that, you have to walk into the bed which will open up a dialogue box asking to sleep.
A thing to note is the you can't walk into the bed from the top or bottom as the headboard and footboard will block the path.
You can also use `debug wh` or `debug warphome` command which will teleport you directly to the bed and then you can move one block to left and again move towards right to trigger the dialogue.

### Using The Tile Viewer Feature

With this feature you can browse the map tile by tile without moving the player.
Because the feature moves and snaps the mouse cursor to the tiles, it can also be used to place certain furnitures which might be difficult to place normally like a t.v., rug, calendar on the wall, etc.
You can use the arrow keys on keyboard to use this feature and if you are using a controller you can also remap these to your controller's buttons from the `config.json`.
While using this feature, you can also press `left control + enter` to auto move the player to the focused tile if possible, this can come in handy in places like mines.
Look at the [keybindings](keybindings.md/#tile-viewer-keys) and [configs](config.md/#tile-viewer-configs) of this feature.

<!-- ### Planting And Harvesting -->

<!-- ### Buying And Selling Stuff -->

### Constructing/Upgrading/Moving/Demolishing Buildings

For constructing or moving a building, we first need to mark the tile where we want the building to be constructed/moved.
To do that, stand on that specific tile and enter the command `mark` followed by a index value which can be from 0 to 9.
So, for example, `mark 0`. This list can only store 10 tile positions at a time and it will also be reset once the game is closed.

Also note that the marked tile position will be the top left position of the building, so, clear the area accordingly.
If you are constructing a building whose dimension is 4 by 5 at the tile 66x 75y, then the constructed building will cover the tiles between 66x 75y, 70x 75y, 66x 80y and 70x 80y.

After you've marked the tile, you can go to Robin's shop and select the `construct farm buildings` option.
Now if you want to construct/upgrade/move a building, select the blueprint of that building by using the next and previous buttons. You can press `c` key to repeat the details of the blueprint.
Then select the appropriate button for the task and then it should take you back to the farm map.

If you want to construct/move the building, use the command `buildsel` followed by the index at which you marked the tile previously with the `mark` command (if you don't remember then you can enter the `marklist` command), for example, `buildsel 0`.
If you want to upgrade/demolish a building, you first have to find out the index of the building you want to upgrade/demolish by entering the `buildlist` command and then again enter the `buildsel` command followed by the index of the building.

### Purchasing/Moving Farm Animals

## Other Pages

- [Readme](README.md)
- [Setup](setup.md)
- [Features](features.md)
- [Keybindings](keybindings.md)
- [Commands](commands.md)
- [Configs](config.md)
