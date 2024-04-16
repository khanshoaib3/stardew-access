# Mod Setup

## Table of Contents

<!-- TOC -->

* [Table of Contents](#table-of-contents)
* [Requirements](#requirements)
    * [SMAPI setup](#smapi-setup)
        * [For Windows](#for-windows)
            * [Xbox Setup](#xbox-setup)
            * [Configuring the game client](#configuring-the-game-client)
                * [For Steam](#for-steam)
                * [For GOG Galaxy](#for-gog-galaxy)
        * [For Linux](#for-linux)
        * [For MacOS](#for-macos)
    * [Installing Kokoro and Project Fluent](#installing-kokoro-and-project-fluent)
* [Stardew Access Installation](#stardew-access-installation)
* [Updating Stardew Access](#updating-stardew-access)
* [Other Mods](#other-mods)
    * [Essentials](#essentials)
    * [Recommended](#recommended)
* [Other Pages](#other-pages)

<!-- TOC -->

## Requirements

1. [SMAPI](#smapi-setup)
2. [Kokoro](#installing-kokoro-and-project-fluent)
3. [Project Fluent](#installing-kokoro-and-project-fluent)

### SMAPI setup

#### For Windows

_Note: Follow the [Xbox Setup](#xbox-setup) if using the xbox/gamepass version of the game._

1. Before installing SMAPI, run the game at least once.
2. Download the [latest version of SMAPI](https://smapi.io/).
3. Extract the .zip file somewhere. (Your downloads folder is fine)
4. Double-click `install on Windows.bat`, and follow the on-screen instructions.

##### Xbox Setup

**Before you install SMAPI:**

1. Open the Stardew Valley section in the Xbox app.
2. Click the `3 dots button` (should be next to the share button) then `Manage button`
3. Click the `Files tab` and then `Browse button` to open your game folder
4. Open the Stardew Valley > Content folder. You should see a lot of files with names like api-ms-win-core-\*
5. Copy the full path from the address bar at the top.

**Run the SMAPI installer:**

Run the SMAPI installer like usual, but:

- Download the installer to somewhere that's not the game directory (like your downloads folder).
- When it asks where to install, enter the path you copied from the previous step. (In cmd, you can either `right click`
  or `Ctrl V` to paste)

**After you install SMAPI:**

In your game folder:

1. rename `Stardew Valley.exe` to another name such as `Stardew Valley original.exe`
2. make a copy of `StardewModdingAPI.exe` and name the copy `Stardew Valley.exe`
3. That's it! Now just launch the game through the Xbox app to play with mods.

_Note that when the game updates, you'll need to redo the last two sections._

##### Configuring the game client

Configuring the game client is not a necessary step, only do this if you want the tracking and awards from the client.
If you choose not to then you will have to launch the game through `StardewModdingAPI.exe`.

_(Note that this gets automatically taken care of for the xbox version while installing SMAPI)_

###### For Steam

1. Open the `properties` for Stardew (you can do this by right clicking the game in the library and then
   selecting `properties` from the drop down list)
2. Go to the `Launch Options` in the `General` tab and paste the following line:
    - `"{Stardew Valley Folder path}\StardewModdingAPI.exe" %command%`
    - Replace the `{Stardew Valley Folder path}` with the correct path.
    - Default for most users
      is: `"C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\StardewModdingAPI.exe" %command%`

###### For GOG Galaxy

1. Open Notepad and paste in the following:
    - `start "" "{Stardew Valley Folder path}\StardewModdingAPI.exe"`
    - Replace the `{Stardew Valley Folder path}` with the correct path.
    - Default for most users
      is: `start "" "C:\Program Files (x86)\GOG Galaxy\Games\Stardew Valley\StardewModdingAPI.exe"`
2. Save the file with the name `start.bat` into the Stardew Valley folder. Make sure to change `Save as type`
   to `all files` when saving.
3. In the GOG Galaxy client, click on Stardew Valley > settings icon > Manage installation > Configure.
4. In the menu that appears, enable the `Custom executables / arguments` checkbox.
5. Click Add another `executable / arguments`.
6. Choose `start.bat` in the window that appears and click Open.
7. Enable the `Default Executable` radio button under the File 2 section you just added, and click OK.

#### For Linux

1. On many Linux distributions, you may need to download and install an older version of libssl (1.1 or 1.x).
    - On Ubuntu, Debian, Linux Mint, and other Debian-based installations, install libssl1.1 (by
      running `sudo apt install libssl1.1` on a terminal).
    - On Arch Linux and its derivatives, install openssl-1.1 by running `sudo pacman -S openssl-1.1`.
    - On Fedora and its derivatives, install openssl-1.1 by running `sudo dnf in openssl1.1`.
2. Download the latest version of SMAPI.
3. Extract the .zip file somewhere (Your downloads folder is fine).
4. If you installed Steam through Flatpak, see these instructions:
5. instructions for Flatpak
6. Run the `install on Linux.sh` file, and follow the on-screen instructions.
    - (If the installer asks for your game install path, see how to
       [find your game folder here](https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started#Find_your_game_folder).)

#### For MacOS

1. Download the latest version of SMAPI.
2. Extract the .zip file somewhere (Your downloads folder is fine).
3. Double-click `install on Mac.command`, and follow the on-screen instructions.

### Installing Kokoro and Project Fluent

As of v1.5.0, [Kokoro](https://www.nexusmods.com/stardewvalley/mods/15682) and [Project Fluent ](https://www.nexusmods.com/stardewvalley/mods/12638) are now dependencies for
stardew access and as such, the mod won't run without
them.
Project Fluent is mainly used for providing a way to have translation mods for stardew access and also the ability to
use [Mozilla's project fluent](https://projectfluent.org/) instead of regular json for translation files.
Kokoro is a core mod for many of [Shockah's](https://www.nexusmods.com/stardewvalley/users/133612513) mods and is required for Project Fluent to work.

Installation of Project Fluent and Kokoro is essentially the same as installing any other mod, here are the steps:

1. Download the v1.1.0 of Project Fluent
   from [this Nexus direct link](https://www.nexusmods.com/stardewvalley/mods/12638?tab=files&file_id=56519) ([login](https://users.nexusmods.com/auth/sign_in)
   if you haven't already) or
   from [Github](https://github.com/Shockah/Stardew-Valley-Mods/releases/download/release%2Fproject-fluent%2F1.1.0/ProjectFluent.1.1.0.zip).
    - Do note that the Github link is only temporary and might get removed in future as the owner only publishes mod
      updates to Nexus.
2. Download the v3.0.0 of Kokoro from
   [this Nexus direct link](https://www.nexusmods.com/stardewvalley/mods/15682?tab=files&file_id=82817)
3. Extract both zip files and move the contents of each into the `Mods` folder in your game's folder.

## Stardew Access Installation

1. Download the latest version from [Github](https://github.com/khanshoaib3/stardew-access/releases/latest) or
   from [Nexus](https://www.nexusmods.com/stardewvalley/mods/16205/?tab=files).
2. Extract the zip file (extracting it into the download folder should be fine).
3. Copy/Cut the generated `stardew-access` folder.
4. Paste it into the `Mods` folder in your game folder.

_In case you are experiencing any kinds of bugs, it is recommended that you install the debug zip.
It will generate more stuff in the log helpful for debugging._

## Updating Stardew Access

To update you just need to delete or move the mod folder from the `Mods` folder to somewhere else and repeat the
installation instruction from the previous section.

## Other Mods

We are working on integrating several mods with stardew access and if you want to track our progress and/or want to
suggest a mod that you want to be integrated, follow
this [issue page](https://github.com/khanshoaib3/stardew-access/issues/181).

### Essentials

- [Auto Travel](https://a4a-mods.com/mods/details?uid=1) - developed
  by [GrumpyCrouton](https://a4a-mods.com/mods/user?user_id=2), allows you to create custom travel points, which you can
  fast travel to from anywhere.
- [Wasteless Watering](https://a4a-mods.com/mods/details?uid=5) - also developed
  by [GrumpyCrouton](https://a4a-mods.com/mods/user?user_id=2), you will no longer waste water on non-soil tiles
- [Chat Commands](https://www.nexusmods.com/stardewvalley/mods/2092) - makes it so you can enter commands from chat
  directly

### Recommended

- [Glue Down Furniture](https://www.nexusmods.com/stardewvalley/mods/10374) - Prevents you from picking up your
  furniture. We highly recommend installing.

## Other Pages

- [Readme](README.md)
- [Features](features.md)
- [Keybindings](keybindings.md)
- [Commands](commands.md)
- [Configs](config.md)
- [Guides](guides.md)
