# Commands

This page contains the list of all commands added by Stardew Access.
For a list of SMAPI commands, visit the [Console commands](https://stardewvalleywiki.com/Modding:Console_commands) page. You **must** have the Console Commands mod. If you have removed it, get it back by reinstalling SMAPI.

## Table Of Contents

- [Tile Reader Commands](#tile-reader-commands)
- [Building Commands](#building-commands)
- [Narration & Verbosity Commands](#narration--verbosity-commands)
- [Radar Commands](#radar-commands)
- [Miscellaneous Commands](#miscellaneous-commands)
- [Other Pages](#other-pages)

## Tile Reader Commands

| Command    | Description                                    |
|------------|------------------------------------------------|
| readtile   | Toggle Read Tile feature                       |
| snapmouse  | Toggle Snap Mouse Feature                      |
| flooring   | Toggle reading flooring                        |
| watered    | Toggle speaking watered or unwatered for crops |

## Building Commands

| Command   | Description                                                                                          | Special Syntax (If any)                  | Argument details (If any)                                                                                        | Example      |
|-----------|------------------------------------------------------------------------------------------------------|------------------------------------------|------------------------------------------------------------------------------------------------------------------|--------------|
| mark      | Marks the player's current position at a specified index                                             | mark [Index:number]                      | Index: the desired index to save the position (0 - 9)                                                            | mark 0       |
| marklist  | List all marked positions                                                                            |                                          |                                                                                                                  | marklist     |
| buildlist | List all buildings for selection                                                                     |                                          |                                                                                                                  | buildlist    |
| buildsel  | Select the index of a building to place a farm animal in the desired building                        | buildsel [Index:number]                  | Index: the index of the building we want to select, use buildlist command to list the buildings with their index | buildsel 3   |

## Narration & Verbosity Commands

| Command         | Description                                                                                                           |
|-----------------|-----------------------------------------------------------------------------------------------------------------------|
| rlt ``<index>`` | Repeats the last phrase narrated. ``<index>`` specifies how many phrases before the latest are skipped when repeating |
| hnspercent      | Toggle between speaking in percentage or full health and stamina                                                      |
| warning         | Toggle warnings feature                                                                                               |
| tts             | Toggles the screen reader/tts                                                                                         |
| refsr           | Refreshes screen reader                                                                                               |

## Radar Commands

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
