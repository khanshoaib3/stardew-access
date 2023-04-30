# Mod Setup

This page contains setup instructions for both [SMAPI](https://smapi.io/) and stardew access mod. First we need to setup SMAPI and then we can install the mod.

## Table of Contents

<!-- vim-markdown-toc GFM -->

* [SMAPI setup](#smapi-setup)
   * [For Windows](#for-windows)
      * [Xbox Setup](#xbox-setup)
      * [Configuring the game client](#configuring-the-game-client)
         * [For Steam](#for-steam)
         * [For GOG Galaxy](#for-gog-galaxy)
   * [For Linux](#for-linux)
   * [For MacOS](#for-macos)
* [Mod Installation](#mod-installation)
* [Updating The Mod](#updating-the-mod)
* [Other Mods](#other-mods)
   * [Essential](#essential)
   * [Recommend](#recommend)
* [Other Pages](#other-pages)

<!-- vim-markdown-toc -->

## SMAPI setup

### For Windows

*Note: Follow the [Xbox Setup](#xbox-setup) if using the xbox/gamepass version of the game.*

1. Before installing SMAPI, run the game at least once.
2. Download the [latest version of SMAPI](https://smapi.io/).
3. Extract the .zip file somewhere. (Your downloads folder is fine)
4. Double-click `install on Windows.bat`, and follow the on-screen instructions.

#### Xbox Setup

**Before you install SMAPI:**

1. Open the Stardew Valley section in the Xbox app.
2. Click the `3 dots button` (should be next to the share button) then `Manage button`
3. Click the `Files tab` and then `Browse button` to open your game folder
4. Open the Stardew Valley > Content folder. You should see a lot of files with names like api-ms-win-core-*
5. Copy the full path from the address bar at the top.

**Run the SMAPI installer:**

Run the SMAPI installer like usual, but:
- Download the installer to somewhere that's not the game directory (like your downloads folder).
- When it asks where to install, enter the path you copied from the previous step. (In cmd, you can either `right click` or `Ctrl V` to paste)

**After you install SMAPI:**

In your game folder:
1. rename `Stardew Valley.exe` to another name such as `Stardew Valley original.exe`
2. make a copy of `StardewModdingAPI.exe` and name the copy `Stardew Valley.exe`
3. That's it! Now just launch the game through the Xbox app to play with mods. 
   
*Note that when the game updates, you'll need to redo the last two sections.*

#### Configuring the game client

Configuring the game client is not a necessary step, only do this if you want the tracking and awards from the client.
If you choose not to then you will have to launch the game through `StardewModdingAPI.exe`.

*(Note that this gets automatically taken care of for the xbox version while installing SMAPI)*

##### For Steam

1. Open the `properties` for Stardew (you can do this by right clicking the game in the library and then selecting `properties` from the drop down list)
2. Go to the `Launch Options` in the `General` tab and paste the following line:
   - `"{Stardew Valley Folder path}\StardewModdingAPI.exe" %command%`
   - Replace the `{Stardew Valley Folder path}` with the correct path.
   - Default for most users is: `"C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\StardewModdingAPI.exe" %command%`
 
##### For GOG Galaxy

1. Open Notepad and paste in the following: 
   - `start "" "{Stardew Valley Folder path}\StardewModdingAPI.exe"`
   - Replace the `{Stardew Valley Folder path}` with the correct path.
   - Default for most users is: `start "" "C:\Program Files (x86)\GOG Galaxy\Games\Stardew Valley\StardewModdingAPI.exe"`
2. Save the file with the name `start.bat` into the Stardew Valley folder. Make sure to change `Save as type` to `all files` when saving.
3. In the GOG Galaxy client, click on Stardew Valley > settings icon > Manage installation > Configure.
4. In the menu that appears, enable the `Custom executables / arguments` checkbox.
5. Click Add another `executable / arguments`.
6. Choose `start.bat` in the window that appears and click Open.
7. Enable the `Default Executable` radio button under the File 2 section you just added, and click OK.


### For Linux

1. On many Linux distributions, you may need to download and install an older version of libssl (1.1 or 1.x).
   - On Ubuntu, Debian, Linux Mint, and other Debian-based installations, install libssl1.1 (by running `sudo apt install libssl1.1` on a terminal).
   - On Arch Linux and its derivatives, install openssl-1.1 by running `sudo pacman -S openssl-1.1`.
   - On Fedora and its derivatives, install openssl-1.1 by running `sudo dnf in openssl1.1`.
2. Download the latest version of SMAPI.
3. Extract the .zip file somewhere (Your downloads folder is fine).
4. If you installed Steam through Flatpak, see these instructions:
5. instructions for Flatpak 
6. Run the `install on Linux.sh` file, and follow the on-screen instructions.
   - (If the installer asks for your game install path, see how to find your game folder - [here](https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started#Find_your_game_folder).)

### For MacOS

1. Download the latest version of SMAPI.
2. Extract the .zip file somewhere (Your downloads folder is fine).
3. Double-click `install on Mac.command`, and follow the on-screen instructions.

## Mod Installation

1. Download the latest version from [Github](https://github.com/khanshoaib3/stardew-access/releases/latest) (No ads or account requirement) or from [Nexus](https://www.nexusmods.com/stardewvalley/mods/16205/?tab=files).
2. Extract the zip file (extracting it into the download folder should be fine).
3. Copy/Cut the generated `stardew-access` folder.
4. Paste it into the `Mods` folder in your game folder.

## Updating The Mod

To update you just need to delete or move the mod folder from the `Mods` folder to somewhere else and repeat the installation instruction from the previous section.

## Other Mods

### Essential

- [Auto Travel](https://a4a-mods.com/mods/details?uid=1) - developed by [GrumpyCrouton](https://a4a-mods.com/mods/user?user_id=2), allows you to create custom travel points, which you can fast travel to from anywhere.
- [Wasteless Watering](https://a4a-mods.com/mods/details?uid=5) - also developed by [GrumpyCrouton](https://a4a-mods.com/mods/user?user_id=2), you will no longer waste water on non-soil tiles
- [Chat Commands](https://www.nexusmods.com/stardewvalley/mods/2092) - makes it so you can enter commands from chat directly

### Recommend

- [Glue Down Furniture](https://www.nexusmods.com/stardewvalley/mods/10374) - Prevents you from picking up your furniture. We highly recommend installing.

## Other Pages

- [Home](/README.html)
- [Mod Details](/mod-details)
- [Guides](/guides)
- [Useful Coords](/useful-coords)
