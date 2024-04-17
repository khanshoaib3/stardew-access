# Guides

This page provides guides for playing Stardew Valley with Stardew Access enabled. While Stardew Access attempts to provide its players feature parity with the base game, some activities must be performed differently and/or may not be obvious to new players. This page assumes that you have already installed both Stardew Valley and Stardew Access as well as all required dependencies. Information on how to install both can be found in the [setup page](setup.md).
Information about the controls for Stardew Access and Stardew Valley can be found in [Keybindings](keybindings.md).

## Table Of Contents

- [Key Terms and Controls](#key-terms-and-controls)
- [Creating a New Game](#creating-a-new-game)
- [Saving Your Progress](#saving-your-progress)
- [Using The Tile Viewer](#using-the-tile-viewer)
- [Buying and Selling](#buying-and-selling)
- [Farming](#farming)
    - [Planting Crops](#planting-crops)
    - [Growing Crops](#growing-crops)
    - [Harvesting Crops](#harvesting-crops)
- [Farm Buildings](#farm-buildings)
    - [Constructing Farm Buildings](#constructing-farm-buildings)
    - [Upgrading Farm Buildings](#upgrading-farm-buildings)
    - [Moving Farm Buildings](#moving-farm-buildings)
    - [Demolishing Farm Buildings](#demolishing-farm-buildings)
- [Farm Animals](#farm-animals)
    - [Purchasing Farm Animals](#purchasing-farm-animals)
    - [Moving Farm Animals](#moving-farm-animals)
- [Other Pages](#other-pages)

## Key Terms and Controls

- Controls
    - Primary left click, `left ctrl + Enter` or secondary left click, `[` (left square bracket): Use one of these to emulate a left mouse-click
    - primary right click, `left shift + enter` or secondary right click, `]` (right square bracket): emulate a right mouse click
    - WASD: the four navigation keys used to navigate most of the game
    - arrow keys: the four keys used to navigate certain menus
    - [full list of keybindings](keybindings.md) .
- Terms
    - title screen: the first screen you encounter when launching the game.
    - main menu: The menu immediately after the title screen which lets you create and load games.
    - character creation menu: a menu that allows you to customize options for a new game
    - construction menu: the menu for managing farm buildings accessed in Robin's shop
    - dialogue: a menu pop-up which may present a short list of options or a character's speech
    - shop counter dialogue: a dialogue which appears at the shop counters of businesses which offer more services than just item purchases
    - shop counter menu: a menu accessed through the shop counters of businesses which allow you to purchase items
    - grid: the portion of the map which is used for placing items and buildings
    - tile: a square-shaped portion of the grid where items may be placed
    - cursor: the pointer that sighted players use to select items and options
    - tile cursor: a feature of the tile viewer which allows Stardew Access Players to move the cursor around on the map
    - map interface: a screen which appears when purchasing farm animals or managing farm buildings

## Creating a New Game

Once you are on the  main menu, use the WASD keys to focus on different options. Select the "new game" button to the left of "load game" to open the new game menu (also known as the character customization menu).
In the new game menu, use the right and left arrow keys to move to the next and previous elements, respectively.
To use a text box, interact with it using left mouse click, type the desired text, and finally press `Escape` to stop interacting with it while focused on the desired text box.
To modify the value of a slider, use `up arrow` and `down arrow` or `pageUp` and `pageDown` while focused on the desired slider.
Some controls allow you to cycle back and forth through a set of options. These controls appear in pairs, one immediately after the other and will include "previous" and "next" in their respective names. Focusing on them will announce the currently selected option. Pressing either will announce the newly-selected option. To hear the currently-selected option again, move focus off of the controls and then move focus onto them again.

Character creation controls are shown by default. To toggle their visibility, Press `Left Control + Space`. This is where you will find sliders for options such as hair color and eye color. Color sliders appear in sets of 3 and adjust hue, saturation, and value (brightness). They announce their current value in the same fashion as the "next" and "previous" pairs. Their individual values range from 0% to 99%. Below is an explanation of what each slider does. Note that the game may report different names for each unique color that results from a particular slider configuration.

- Hue spans through the color spectrum from red at the minimum, through the rainbow, and back to red at the maximum.
    - At 99% saturation and 99% value, Hue spans from 0% to 99% as follows: red, orange, yellow, green, blue, indigo, violet, magenta, red
- Saturation spans from none at the minimum to full saturation of the selected hue at maximum.
    - At 0% hue and 99% value, 0% saturation is white and 99% saturation is bright red.
- Value, also known as brightness, adjusts the brightness of the selected hue and saturation from black to full brightness.
    - at 0% hue and 99% saturation, 0% value is black and 99% value is bright red. To achieve a shade of grey, set saturation to 0%.

Another menu known as "advanced options" is accessible from the character creation menu. Its button is located after "skip intro". This menu uses the WASD keys to navigate and adjust options. It can only be dismissed by pressing "OK" at the bottom of the menu.
When finished configuring your new game and character, select "OK" to start the game. If you enabled "skip intro", you will be placed immediately in your farm. Otherwise, the game will proceed to the intro cutscene.

## Saving Your Progress

To save your progress, sleep in your bed. Walk into your bed to enter it and select "go to sleep" from the new dialogue. This is usually done at the end of the day. Exiting the game without sleeping will discard that day's progress.
If you can't enter your bed, try approaching it from a different position. The bed cannot be entered from the bottom or the top tiles since the headboard and footboard are in the way. Assuming you have not moved your bed, it is easiest to enter it from its center tile on the side of the bed.
You can also use the `debug wh` or `debug warphome` commands which will teleport you directly onto the bed. Next, move one block off of the bed. Then move back onto it to open the sleep dialogue.
As stated before, you may need to move in different directions to get off the bed, depending on the position and orientation of your bed.

**Important:** If you have not upgraded your house and you have not moved your bed, move to X: 8, Y: 9. This will place you directly next to the center tile of the bed's left side. The bed is 3 tiles high. To enter the bed, walk to the right. This will only work if your bed has not been moved and you have not upgraded your farmhouse. Doing either of these will change the location of your bed.

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

This feature can be remapped to a controller/gamepad via the `config.json` file. More info available in [keybindings](keybindings.md#tile-viewer-keys) and [configs](config.md#tile-viewer-configs).

## Buying and Selling

Buying and selling is usually accomplished through shops which are scattered throughout the valley and run by various residents of the town. They all offer different products, services, and have slightly different interfaces but all involve very similar steps. Selling your crops, fish, artisan goods, etc. can either be done at a shop or by putting the items into the shipping bin on your farm.

This guide will use Robin's shop as an example since Robin offers both products and services.

To buy items:

1. Enter the shop during business hours (these will vary from shop to shop. Pierre's is closed on Wednesdays) and approach the shop counter.
2. Interact with the shop counter to open the shop dialogue.
3. You can choose various options from the shop dialogue such as constructing farm buildings, upgrading your house, purchasing items, or leaving. Some shops May not offer options and only sell items. In that case, you will be placed immediately in the shop menu.
4. Assuming you selected "shop" or entered a shop that only sells items, you will be placed in the shop menu which presents a list of items that are sold.
5. To buy an item, select the desired item and use left mouse click. You can purchase 5 of an item by holding shift and using left mouse click.

To sell items:

1. follow the first 4 steps in the previous list.
2. Move to your inventory items or press `I`
3. Find the item you want to sell in your inventory. Only certain items may be sold at certain shops.
4. Use `left mouse click` on the item to sell the entire stack. Use ` shift + left mouse click` to sell half the stack.

## Farming

Farming, including planting and harvesting crops, is the primary way to make money in Stardew Valley, especially in the early-game. The following guides will cover how to farm.

- [Planting Crops](#planting-crops)
- [Growing Crops](#growing-crops)
- [Harvesting Crops](#harvesting-crops)

### Planting Crops

Once you have purchased seeds from Pierre's general store in the town (a.k.a) SeedShop) or otherwise obtained seeds, go to your farm and use your tools to clear a plot of land to plant them. Make sure to clear grass as well.
Your scythe does not consume any of your energy. Once you have cleared sufficient land to plant your crops, use your hoe to till the soil.
Next, use your watering can to water the soil and add any [fertilizer](https://stardewvalleywiki.com/Fertilizer) if desired to the soil.
finally, place your crops in the soil. You may plant your crops before watering, but fertilizers **must** be placed before the crop is placed.
Tilling individual tiles can be very tedious, so be sure to check Clint's shop in the town (a.k.a Blacksmith) for tool upgrades. Once you upgrade your hoe, you can hold down `tool button` and hoe several tiles at a time.

### Growing crops

Different crops will take different amounts of time to grow. Most crops will only grow during one season, however they will grow during any season in the greenhouse. Growing season and growing time are listed in Pierre's shop as well as on the item's tooltip in your inventory.
Each season is 28 days long, and it's important to ensure your crop has enough time to grow. If your crop has not been harvested before the start of the next season, it will die unless it also grows in the new season. Most crops only grow in one season. It's important to water your crop daily. Unwatered crops will not die, but they will not grow that day.
Each day has a chance of rain. If it rains, there is no need to water your crops as the rain will take care of it. Crops grown indoors or in the greenhouse must be watered regardless of rain. Check the TV in your farmhouse for next day's forecast.
You can walk over most crops safely with the exception of crops that grow on a trellis. You cannot pass through trellis crops at all, so make sure you leave enough room to reach them.
Some crops, such as green beans, hot peppers, and blueberries, will produce multiple harvests per crop. Once they mature, you can pick the first harvest and continue watering the crops daily as normal and producing more harvests until the end of the season.
Lastly, you may craft [sprinklers](https://stardewvalleywiki.com/Sprinkler) in order to keep crops watered. There are several tiers of sprinklers which keep progressively more tiles watered.

### Harvesting Crops

To harvest a mature crop, walk up to the crop and press `action button` to harvest it. The tile will be emptied and another crop may be placed. If the crop produces multiple harvests, the crop will remain on the tile and you can continue to water it as normal.
Be careful that you do not use your pickaxe on crops as doing so will destroy them, undoing all your hard work.

## Farm Buildings

The following guides are all related to constructing and managing buildings on your farm such as coops, silos, barns, and more. All of them are accomplished through the construction menu in Robin's shop in the mountains. This shop is labeled as "ScienceHouse" in-game. Enter Robin's shop during business hours and select "construct farm buildings" from the shop counter menu to find a list of building blueprints and construction options.

- [Constructing Farm Buildings](#constructing-farm-buildings)
- [Upgrading Farm Buildings](#upgrading-farm-buildings)
- [Moving Farm Buildings](#moving-farm-buildings)
- [Demolishing Farm Buildings](#demolishing-farm-buildings)

### Constructing Farm Buildings

To construct a building on your farm, first decide where you want to build it and ensure that there is no debris in the area. The [tile cursor](#using-the-tile-viewer) is very helpful for accomplishing this. Different buildings will occupy different amounts of space on your farm. Check Robin's shop for more info.
Once you have decided where you want your building to be and cleared debris from the area, collect the necessary materials for construction and ensure you have enough money. Go to Robin's shop, select "construct farm buildings" from the shop counter dialogue, and use the controls in the new menu to select the desired farm building to construct. To repeat information about the currently-selected building in this menu, press `C`.
Once you have selected the building you want to construct, click "construct". A map of your farm will appear on screen.
Use the arrow keys to focus on the coordinate where you want to place the building. As you move the tile cursor around, the map will scroll when the cursor reaches the edge of the portion of the map that is visible on screen. This will result in the tile cursor appearing to move too far as the map scrolls beneath it.
The map will continue scrolling until you move the tile cursor away from the edge of the screen or until the map scrolls all the way to a map edge. To scroll the map a small amount, you must move the tile cursor back and forth to "nudge" the map forward. If you think this is terrible, sighted players agree with you.
Once you are focused on the tile you want to select, use left mouse click to place the building for construction. If the building cannot be constructed, nothing will happen. If the building can be constructed, Robin will speak to you and inform you that she will begin work the next day (unless the next day has a festival, in which case she will begin work the day after tomorrow).
During construction, you will find Robin on your farm hard at work and you will be unable to construct additional buildings or upgrade your house. It will take several days for Robin to construct the farm building.
Note: the tile you select will be the top-left corner of the building. For example, if you are building a silo which is 3 tiles by 3 tiles, select the tile at the very top-left of the 3x3 area you have prepared. If you select X:64, Y:21, the silo will occupy a footprint from X:64, Y:21 to X:67, Y:24.

**If you cannot place your building where you want**, ensure the following are all true:

1. you have enough money to construct the desired building
2. you have enough resources in your inventory to construct the building
3. The area you have selected is *completely clear* of **all** debris
4. There are no other buildings or items overlapping the building you are attempting to construct
5. You are selecting the correct tile (Remember, you are placing the building from the top-left corner) and map scrolling can move you to an undesired tile

### Upgrading Farm Buildings

The barn, shed, and coop farm buildings can be upgraded. Doing this involves many of the same steps as constructing a new farm building, however instead of placing a new building on your farm, you must select an existing building that can be upgraded.
The coop and barn must both be upgraded in their own specific orders, but they have similar names: coop and barn, big coop and big barn, deluxe coop and deluxe barn. The shed only has one upgrade option, "big shed".
As an example: to upgrade a regular coop, select "big coop" from the construction menu. To upgrade a big barn, select "deluxe barn" from the construction menu. You cannot skip upgrade stages.
Once you enter the map interface, place the tile cursor over any portion of the building you want to upgrade and use left mouse click. If successful, Robin will tell you that she will begin work on the upgrade the next day. It will take several days to upgrade the building. Unlike constructing a new building, Robin will be inside the building while she upgrades it. Like constructing a new building, you will not be able to construct any other buildings or upgrade your house while Robin works.

### Moving Farm Buildings

To move a farm building, select "move building" from the construction menu. The currently-selected blueprint does not matter. Once in the map interface, move the tile cursor onto any portion of the building you want to move and use left mouse click.
You will hear a sound once you have successfully selected a building. From this point, the directions are identical to [constructing a new farm building](#constructing-farm-buildings).
Move the tile cursor to the new desired location. The area must be free of debris, items, and other buildings in order to move the selected building. Left mouse click to place the building down. When successful, you will hear another sound and you will be returned to the shop. You do not have to wait any time for your buildings to be moved.

**Important:** Regardless of which portion of the building you select, you will place the building down from its top left corner. For example: if you select the center tile of a silo and then place it down at X:64, Y:21, the silo will occupy a footprint from X:64, Y:21 to X:67, Y:24 regardless of which part of the silo you initially selected.

<!--painting farm buildings. Not yet accessible-->

### Demolishing Farm Buildings

To demolish a farm building, select "demolish building" from the construction menu. The currently-selected blueprint does not matter. Once in the map interface, use the tile cursor to select any portion of the building you want to demolish. There will be no confirmation dialogue, so make sure you actually want to demolish a building before selecting it. If successful, you will hear an explosion sound and be returned to the shop. The building has now been demolished.

## Farm Animals

The following guides provide instructions on how to purchase and move farm animals. This feature currently relies on console commands.

<!--todo: update docs once accessible feature is implemented in-game-->

- [Purchasing Farm Animals](#purchasing-farm-animals)
- [Moving Farm Animals](#moving-farm-animals)

### Purchasing Farm Animals

To purchase animals for your farm, you must first have an appropriate farm building to house them. See [the farm buildings section](#farm-buildings) for details. Not all animals can be housed in all buildings. Chickens and ducks may only be housed in coops, cows and goats may only be housed in barns, and certain other animals require upgraded coops or barns.

Go to Marnie's ranch in the forest (a.k.a AnimalShop) during business hours and select "purchase animals" from the shop counter dialogue.
A new menu will open with a grid of options. Depending on which farm buildings you have constructed, you may see the names of various animals or a message informing you that a specific farm building is required.
If the animal can be purchased, its name will be read along with its price, description, and the type of building it lives in. Select the animal you wish to purchase with left mouse click.
Once the map interface opens, enter the command `buildlist` into the SMAPI console to get a list of all farm buildings. Then enter `buildsel <i>`, replacing `<i>` with the number of the building you want to place the animal in. If successful, you will be presented with a text box to name the animal. Interact with it, enter the desired name, press escape, and then select "ok" below the textbox. Marnie will deliver your animal to their new home.

If you cannot purchase an animal, ensure the following are true:

1. You have the correct farm building for that animal
2. You have enough money to purchase that animal
3. You are selecting the correct farm building
4. The farm building you are selecting has space for that animal

### Moving Farm Animals

To move an animal that you own to another building, use action button on that animal to open their menu. If you have not pet your animal that day yet, you may need to press twice to do this. This menu allows you to sell your animal, rename it, and change its home building. Select "change home building". Once the map interface opens, enter the command `buildlist` to get a list of all farm buildings. Then enter `buildsel <i>`, replacing `<i>` with the number of the building you want to place the animal in. If successful, the animal will be moved to the new building and you will return to the game.

If you cannot move an animal to a new building, ensure that the following are true:

1. The animal can live in the building you are moving it to
2. The building you are moving the animal to has room for the animal
3. You are selecting the correct building

<!--todo: more animal guides probably-->

## Other Pages

- [Readme](README.md)
- [Setup](setup.md)
- [Features](features.md)
- [Keybindings](keybindings.md)
- [Commands](commands.md)
- [Configs](config.md)
