# Commands

This page contains the list of all commands added by the mod.
You can install the [Chat Commands](https://www.nexusmods.com/stardewvalley/mods/2092) mod which will enable entering
the commands from the in-game chat box instead of using the terminal.
For a list of commands added by SMAPI, you can visit the [Console commands](https://stardewvalleywiki.com/Modding:Console_commands) page.

## Table Of Contents

* [Custom Commands List](#custom-commands-list)
    * [Read tile related](#read-tile-related)
    * [Building related](#building-related)
    * [Other](#other)
    * [Radar related](#radar-related)
* [Other Pages](#other-pages)

## Custom Commands List

### Read tile related

| Command  | Description                                    |
|----------|------------------------------------------------|
| readtile | Toggle Read Tile feature                       |
| flooring | Toggle reading flooring                        |
| watered  | Toggle speaking watered or unwatered for crops |

### Building related

| Command   | Description                                                                                         | Special Syntax (If any)                  | Argument details (If any)                                                                                                         | Example      |
|-----------|-----------------------------------------------------------------------------------------------------|------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------|--------------|
| mark      | Marks the player's position for use in building construction in Carpenter Menu                      | mark [Index:number]                      | Index: the index at which we want to save the position. From 0 to 9 only                                                          | mark 0       |
| marklist  | List all marked positions                                                                           |                                          |                                                                                                                                   | marklist     |
| buildlist | List all buildings for selection for upgrading/demolishing/painting                                 |                                          |                                                                                                                                   | buildlist    |
| buildsel  | Select the building index which you want to upgrade, demolish and paint.                            | buildsel [Index:number]                  | Index: the index of the building we want to select, use buildlist command to list the buildings with their index                  | buildsel 3   |
| buildsel  | Select the marked position index where we want to construct the building.                           | buildsel [Index:number]                  | Index: the index of the marked position, use marklist command to list the marked positions with their index                       | buildsel 0   |
| buildsel  | Select the building index along with a index of marked position where we want to move the building. | buildsel [Index1:number] [Index2:number] | Index1: the index of the building we want to select. Index2: the index of the marked position where we want to move the building. | buildsel 3 0 |

### Other

| Command    | Description                                                      |
|------------|------------------------------------------------------------------|
| snapmouse  | Toggle Snap Mouse Feature                                        |
| hnspercent | Toggle between speaking in percentage or full health and stamina |
| warning    | Toggle warnings feature                                          |
| tts        | Toggles the screen reader/tts                                    |
| refsr      | Refreshes screen reader                                          |
| refst      | (Temporarily disabled) Refreshes static tiles json file          |
| refmc      | Refreshes mod config json file                                   |

### Radar related

| Command  | Description                                                    |
|----------|----------------------------------------------------------------|
| radar    | Toggle Radar feature                                           |
| rdebug   | Toggle debugging in radar feature                              |
| rstereo  | Toggle stereo sound in radar feature                           |
| rfocus   | Toggle focus mode in radar feature                             |
| rdelay   | Set the delay of radar feature in milliseconds                 |
| rrange   | Set the range of radar feature                                 |
| readd    | Add an object key to the exclusions list of radar feature.     |
| reremove | Remove an object key from the exclusions list of radar feature |
| relist   | List all the exclusions in the radar feature                   |
| reclear  | Remove all keys from the exclusions list in the radar feature  |
| recount  | Number of exclusions in the radar feature                      |
| rfadd    | Add an object key to the focus list of radar feature.          |
| rfremove | Remove an object key from the focus list of radar feature      |
| rflist   | List all the focus in the radar feature                        |
| rfclear  | Remove all keys from the focus list in the radar feature       |
| rfcount  | Number of focus in the radar feature                           |

## Other Pages

- [Readme](README.md)
- [Setup](setup.md)
- [Features](features.md)
- [Keybindings](keybindings.md)
- [Configs](config.md)
- [Guides](guides.md)
