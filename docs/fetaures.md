## Features

Contains details of all the features currently in the mod.

### Read Tile

Reads the name and information about the tile the player is currently looking (not the one the player is standing on).

### Tile Viewer

Allows browsing of the map and snapping mouse to tiles with the arrow keys. This Feature can be used to place items into the world like calender.

**Releated Config:-**

- `TileCursorPreciseMovementDistance` = Default to *8*. Specifies the number of pixels the cursor should move when using precision movement i.e. with *left shift*.
- `LimitTileCursorToScreen` = Default to *false*. Toggle whether to prevent cursor from going out of screen.

### Snap Mouse

Snaps the mouse cursor to the adjacent tile to the player according to the direction the player is facing.

### Warning

Warns the player when their health or stamina/energy is low. Also warns when its past midnight.

### Manually Triggered

These features are manually triggered when a certain key is pressed.

**Related Keybinds:-**

- `[ or Ctrl Enter`	= Simulate mouse left click.
- `] or Shift Enter` = Simulate mouse right click.
- `H` = Narrate health and stamina.
- `Left alt + K` = Narrate current location name.
- `K` = Narrate player position.
- `Q` = Narrate the time of day, day and date and season
- `R` = Narrate the money the player has currently.

<!-- #TODO add API -->

### Radar

Plays the sound at any point of interest nearby. This feature is not fully developed so haven't included the commads for it here as they are overwhelming.

