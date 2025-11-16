# General Guides

This page provides general guides for playing Stardew Valley with Stardew Access enabled.  The guides assume that you have already installed both Stardew Valley and Stardew Access as well as all required dependencies. Information on how to install both can be found in the [setup page](../setup.md).

Information about the controls for Stardew Access and Stardew Valley can be found in [Keybindings](../keybindings.md).

## Table of Contents

- [Key Terms and Controls](#key-terms-and-controls)
- [Creating a New Game](#creating-a-new-game)
- [Saving Your Progress](#saving-your-progress)
- [Buying and Selling](#buying-and-selling)
- [Fishing](#fishing)
- [Other Guides](#other-guides)

## Key Terms and Controls

- Controls
    - Primary left click, `left ctrl + Enter` or secondary left click, `[` (left square bracket): Use one of these to emulate a left mouse-click
    - primary right click, `left shift + enter` or secondary right click, `]` (right square bracket): emulate a right mouse click
    - WASD: the four navigation keys used to navigate most of the game
    - arrow keys: the four keys used to navigate certain menus
    - [full list of keybindings](../keybindings.md) .
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
To modify the value of a slider, use `up arrow` and `down arrow` to adjust by 1 unit or `pageUp` and `pageDown` to adjust by 10 units while focused on the desired slider.
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

## Fishing

The fishing game features many sounds which aren't heard anywhere else in the valley. It is recommended that you use a gamepad when fishing to take advantage of the haptic feedback. Many Stardew Access players forego the minigame and install an automatic fishing mod due to the difficulty in catching fish. Sighted players struggle with the minigame as well. Below are useful resources:

- [Yet Another Fishing Mod](https://www.nexusmods.com/stardewvalley/mods/20391)
- [Stardew Valley Fish Bite Sound](https://www.youtube.com/watch?v=sKd1mwB1I3g)
- [Sine Wave](https://www.youtube.com/watch?v=yyvyt0HyvFk)
- [Square Wave](https://www.youtube.com/watch?v=HHh-VdsVzmA)

In order to fish, you first need a fishing rod. You can get your first fishing rod by visiting Willy at the beach after he sends you a letter in the mail, usually on day 2 of spring. Once you visit Willy, he will introduce himself and present you with the bamboo pole. You can fish in almost every body of water in the valley, including some elevator levels in [the mines](https://stardewvalleywiki.com/The_Mines). The [object tracker](../features.md#object-tracker) will present fishing spots in the fishing category when they are available. These are not necessary to use and you can fish in front of water where you like. These spots make it easy to get to water.

Once you have your fishing rod, stand in front of and face the body of water you want to fish in. Hold your tool key to begin casting. You will hear a sine wave rising in pitch. The pitch of this sine wave represents your casting power: the higher the pitch, the further you will cast your line. Release your tool key when you reach the desired power level. You will hear the line splash into the water if your cast was successful, otherwise you must cast again. If you have trouble casting your line, make sure that you're positioned correctly in front of the water and try adjusting your casting power on your next cast. Finding the correct spot can be tricky when fishing in the ocean.

Next, you must wait until  a fish bites. Stardew Valley will play a distinctive sound, linked above, when there is something on your line. It might be a fish, a treasure chest, an edible item such as seaweed or algae, or garbage. Regardless of what the item is, you must quickly press your tool key once again to reel it in. If it is a fish, you will hear a metallic shing and the fishing minigame will begin.

Once the fishing minigame starts, you will once again hear a sine wave that raises and lowers in pitch. This pitch represents where the fish is. The goal of the minigame is to move the fish within a narrow range. You will know that the fish is within this narrow range or "sweet spot" when the sine wave changes into the sound of your line reeling in. If the pitch of the sine wave is very low, press your tool key repeatedly to raise it and move into the sweet spot. If the pitch of the sine wave is very high, hold your tool key to lower the pitch and move into the sweet spot. Once the fish is in the sweet spot, you will hear square wave blips. When these blips play in the sweet spot, they signify that you are 20% closer to catching the fish. If you hear blips when the sine wave is playing, they signify that you have lost 20% of your progress toward catching the fish. If the sine wave pitch raises or lowers too far, the fish will escape and you must cast your line and play the minigame again. With practice and some luck, you will be catching fish!

Currently, identifying treasure before catching is not accessible with Stardew Access. Don't be discouraged if you struggle with the fishing minigame. It is intentionally very difficult, especially with no powerups and a low level fishing rod. Once you upgrade from the bamboo pole, you can attach items like [bait](https://stardewvalleywiki.com/Bait) to the [fiberglass rod](https://stardewvalleywiki.com/Fiberglass_Rod) and [tackle](https://stardewvalleywiki.com/Tackle) to an [iridium rod](https://stardewvalleywiki.com/Iridium_Rod). You can also use certain food items to boost your chances of catching fish. Alternatively, you can attempt to use the [training rod](https://stardewvalleywiki.com/Training_Rod).

## Other guides

- [Stardew Access Guides](stardew-access-guides.md)
- [Farming Guides](farming-guides.md)
- [Mining Guides](mining-guides.md)

**[Back to guides home](guides-home.md)

**[Back to readme...](../README.md)**
