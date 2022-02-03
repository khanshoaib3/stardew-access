using Microsoft.Xna.Framework;
using stardew_access.Patches;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace stardew_access
{
    internal class CustomCommands
    {
        internal static void Initialize()
        {
            IModHelper helper = MainClass.ModHelper;

            helper.ConsoleCommands.Add("readtile", "Toggle read tile feature.", (string commmand, string[] args) =>
            {
                MainClass.readTile = !MainClass.readTile;

                MainClass.monitor.Log("Read Tile is " + (MainClass.readTile ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("snapmouse", "Toggle snap mouse feature.", (string commmand, string[] args) =>
            {
                MainClass.snapMouse = !MainClass.snapMouse;

                MainClass.monitor.Log("Snap Mouse is " + (MainClass.snapMouse ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("radar", "Toggle radar feature.", (string commmand, string[] args) =>
            {
                MainClass.radar = !MainClass.radar;

                MainClass.monitor.Log("Radar " + (MainClass.radar ? "on" : "off"), LogLevel.Info);
            });

            #region Radar Feature
            helper.ConsoleCommands.Add("rdebug", "Toggle debugging in radar feature.", (string commmand, string[] args) =>
            {
                MainClass.radarDebug = !MainClass.radarDebug;

                MainClass.monitor.Log("Radar debugging " + (MainClass.radarDebug ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("rstereo", "Toggle stereo sound in radar feature.", (string commmand, string[] args) =>
            {
                MainClass.radarStereoSound = !MainClass.radarStereoSound;

                MainClass.monitor.Log("Stereo sound is " + (MainClass.radarStereoSound ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("rfocus", "Toggle focus mode in radar feature.", (string commmand, string[] args) =>
            {
                bool focus = MainClass.radarFeature.ToggleFocus();

                MainClass.monitor.Log("Focus mode is " + (focus ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("rdelay", "Set the delay of radar feature in milliseconds.", (string commmand, string[] args) =>
            {
                string? delayInString = null;

                if (args.Length > 0)
                {
                    delayInString = args[0];

                    int delay;

                    bool isParsable = int.TryParse(delayInString, out delay);

                    if (isParsable)
                    {
                        MainClass.radarFeature.delay = delay;
                        if (delay >= 1000)
                            MainClass.monitor.Log($"Delay set to {MainClass.radarFeature.delay} milliseconds.", LogLevel.Info);
                        else
                            MainClass.monitor.Log($"Delay should be atleast 1 second or 1000 millisecond long.", LogLevel.Info);
                    }
                    else
                    {
                        MainClass.monitor.Log("Invalid delay amount, it can only be in numeric form.", LogLevel.Info);
                    }

                }
                else
                {
                    MainClass.monitor.Log("Enter the delay amount (in milliseconds)!", LogLevel.Info);
                }

            });

            helper.ConsoleCommands.Add("rrange", "Set the range of radar feature.", (string commmand, string[] args) =>
            {
                string? rangeInString = null;

                if (args.Length > 0)
                {
                    rangeInString = args[0];

                    int range;

                    bool isParsable = int.TryParse(rangeInString, out range);

                    if (isParsable)
                    {
                        MainClass.radarFeature.range = range;
                        if (range >= 2 && range <= 10)
                            MainClass.monitor.Log($"Range set to {MainClass.radarFeature.range}.", LogLevel.Info);
                        else
                            MainClass.monitor.Log($"Range should be atleast 2 and maximum 10.", LogLevel.Info);
                    }
                    else
                    {
                        MainClass.monitor.Log("Invalid range amount, it can only be in numeric form.", LogLevel.Info);
                    }

                }
                else
                {
                    MainClass.monitor.Log("Enter the range amount!", LogLevel.Info);
                }

            });


            #region Exclusions
            helper.ConsoleCommands.Add("readd", "Add an object key to the exclusions list of radar feature.", (string commmand, string[] args) =>
                {
                    string? keyToAdd = null;

                    for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                    if (keyToAdd != null)
                    {
                        keyToAdd = keyToAdd.Trim().ToLower();
                        if (!MainClass.radarFeature.exclusions.Contains(keyToAdd))
                        {
                            MainClass.radarFeature.exclusions.Add(keyToAdd);
                            MainClass.monitor.Log($"Added {keyToAdd} key to exclusions list.", LogLevel.Info);
                        }
                        else
                        {
                            MainClass.monitor.Log($"{keyToAdd} key already present in the list.", LogLevel.Info);
                        }
                    }
                    else
                    {
                        MainClass.monitor.Log("Unable to add the key to exclusions list.", LogLevel.Info);
                    }
                });

            helper.ConsoleCommands.Add("reremove", "Remove an object key from the exclusions list of radar feature.", (string commmand, string[] args) =>
            {
                string? keyToAdd = null;

                for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                if (keyToAdd != null)
                {
                    keyToAdd = keyToAdd.Trim().ToLower();
                    if (MainClass.radarFeature.exclusions.Contains(keyToAdd))
                    {
                        MainClass.radarFeature.exclusions.Remove(keyToAdd);
                        MainClass.monitor.Log($"Removed {keyToAdd} key from exclusions list.", LogLevel.Info);
                    }
                    else
                    {
                        MainClass.monitor.Log($"Cannot find {keyToAdd} key in exclusions list.", LogLevel.Info);
                    }
                }
                else
                {
                    MainClass.monitor.Log("Unable to remove the key from exclusions list.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("relist", "List all the exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                if (MainClass.radarFeature.exclusions.Count > 0)
                {
                    string toPrint = "";
                    for (int i = 0; i < MainClass.radarFeature.exclusions.Count; i++)
                    {
                        toPrint = $"{toPrint}\t{i + 1}: {MainClass.radarFeature.exclusions[i]}";
                    }
                    MainClass.monitor.Log(toPrint, LogLevel.Info);
                }
                else
                {
                    MainClass.monitor.Log("No exclusions found.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("reclear", "Clear the focus exclusions in the radar featrure.", (string commmand, string[] args) =>
            {
                MainClass.radarFeature.exclusions.Clear();
                MainClass.monitor.Log($"Cleared the focus list in the exclusions feature.", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("recount", "Number of exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                MainClass.monitor.Log($"There are {MainClass.radarFeature.exclusions.Count} exclusiond in the radar feature.", LogLevel.Info);
            });
            #endregion

            #region Focus
            helper.ConsoleCommands.Add("rfadd", "Add an object key to the focus list of radar feature.", (string commmand, string[] args) =>
                {
                    string? keyToAdd = null;

                    for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                    if (keyToAdd != null)
                    {
                        keyToAdd = keyToAdd.Trim().ToLower();
                        if (!MainClass.radarFeature.focus.Contains(keyToAdd))
                        {
                            MainClass.radarFeature.focus.Add(keyToAdd);
                            MainClass.monitor.Log($"Added {keyToAdd} key to focus list.", LogLevel.Info);
                        }
                        else
                        {
                            MainClass.monitor.Log($"{keyToAdd} key already present in the list.", LogLevel.Info);
                        }
                    }
                    else
                    {
                        MainClass.monitor.Log("Unable to add the key to focus list.", LogLevel.Info);
                    }
                });

            helper.ConsoleCommands.Add("rfremove", "Remove an object key from the focus list of radar feature.", (string commmand, string[] args) =>
            {
                string? keyToAdd = null;

                for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                if (keyToAdd != null)
                {
                    keyToAdd = keyToAdd.Trim().ToLower();
                    if (MainClass.radarFeature.focus.Contains(keyToAdd))
                    {
                        MainClass.radarFeature.focus.Remove(keyToAdd);
                        MainClass.monitor.Log($"Removed {keyToAdd} key from focus list.", LogLevel.Info);
                    }
                    else
                    {
                        MainClass.monitor.Log($"Cannot find {keyToAdd} key in focus list.", LogLevel.Info);
                    }
                }
                else
                {
                    MainClass.monitor.Log("Unable to remove the key from focus list.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("rflist", "List all the exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                if (MainClass.radarFeature.focus.Count > 0)
                {
                    string toPrint = "";
                    for (int i = 0; i < MainClass.radarFeature.focus.Count; i++)
                    {
                        toPrint = $"{toPrint}\t{i + 1}): {MainClass.radarFeature.focus[i]}";
                    }
                    MainClass.monitor.Log(toPrint, LogLevel.Info);
                }
                else
                {
                    MainClass.monitor.Log("No objects found in the focus list.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("rfclear", "Clear the focus list in the radar featrure.", (string commmand, string[] args) =>
            {
                MainClass.radarFeature.focus.Clear();
                MainClass.monitor.Log($"Cleared the focus list in the radar feature.", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("rfcount", "Number of list in the radar feature.", (string commmand, string[] args) =>
            {
                MainClass.monitor.Log($"There are {MainClass.radarFeature.focus.Count} objects in the focus list in the radar feature.", LogLevel.Info);
            });
            #endregion

            #endregion

            #region Tile marking
            helper.ConsoleCommands.Add("mark", "Marks the player's position for use in building cunstruction in Carpenter Menu.", (string commmand, string[] args) =>
            {
                if (Game1.currentLocation is not Farm)
                {
                    MainClass.monitor.Log("Can only use this command in the farm", LogLevel.Info);
                    return;
                }

                string? indexInString = args.ElementAtOrDefault(0);
                if (indexInString == null)
                {
                    MainClass.monitor.Log("Enter the index too! Example syntax: mark 0, here 0 is the index and it can be from 0 to 9 only", LogLevel.Info);
                    return;
                }

                int index;
                bool isParsable = int.TryParse(indexInString, out index);

                if (!isParsable || !(index >= 0 && index <= 9))
                {
                    MainClass.monitor.Log("Index can only be a number and from 0 to 9 only", LogLevel.Info);
                    return;
                }

                BuildingNAnimalMenuPatches.marked[index] = new Vector2((int)Game1.player.getTileX(), (int)Game1.player.getTileY());
                MainClass.monitor.Log($"Location {(int)Game1.player.getTileX()}x {(int)Game1.player.getTileY()}y add at {index} index.", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("marklist", "List all marked positions.", (string commmand, string[] args) =>
            {
                string toPrint = "";
                for (int i = 0; i < BuildingNAnimalMenuPatches.marked.Length; i++)
                {
                    if (BuildingNAnimalMenuPatches.marked[i] != Vector2.Zero)
                    {
                        toPrint = $"{toPrint}\t Index {i}: {BuildingNAnimalMenuPatches.marked[i].X}x {BuildingNAnimalMenuPatches.marked[i].Y}y";
                    }
                }

                if (toPrint == "")
                    MainClass.monitor.Log("No positions marked!", LogLevel.Info);
                else
                    MainClass.monitor.Log($"Marked positions:\t{toPrint}", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("buildlist", "List all buildings for selection for upgrading/demolishing/painting", (string commmand, string[] args) =>
            {
                if (Game1.activeClickableMenu is not CarpenterMenu || !BuildingNAnimalMenuPatches.isOnFarm)
                {
                    MainClass.monitor.Log($"Cannot list buildings.", LogLevel.Info);
                    return;
                }

                string toPrint = "";
                Farm farm = (Farm)Game1.getLocationFromName("Farm");
                Netcode.NetCollection<Building> buildings = farm.buildings;
                int buildingIndex = 0;

                for (int i = 0; i < buildings.Count; i++)
                {
                    string? name = buildings[i].nameOfIndoorsWithoutUnique;
                    name = (name == "null") ? buildings[i].buildingType.Value : name;

                    BuildingNAnimalMenuPatches.availableBuildings[buildingIndex] = buildings[i];
                    toPrint = $"{toPrint}\nIndex {buildingIndex}: {name}: At {buildings[i].tileX}x and {buildings[i].tileY}y";
                    ++buildingIndex;
                }

                if (toPrint == "")
                {
                    MainClass.monitor.Log("No appropriate buildings to list", LogLevel.Info);
                }
                else
                {
                    MainClass.monitor.Log($"Available buildings:{toPrint}\nOpen command menu and use pageup and pagedown to check the list", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("buildsel", "Select the building index which you want to upgrade/demolish/paint", (string commmand, string[] args) =>
            {

                if (Game1.activeClickableMenu is not CarpenterMenu || !BuildingNAnimalMenuPatches.isOnFarm)
                {
                    MainClass.monitor.Log($"Cannot list buildings.", LogLevel.Info);
                    return;
                }

                string? indexInString = args.ElementAtOrDefault(0);
                if (indexInString == null)
                {
                    MainClass.monitor.Log("Enter the index of the building too! Use buildlist", LogLevel.Info);
                    return;
                }

                int index;
                bool isParsable = int.TryParse(indexInString, out index);

                if (!isParsable)
                {
                    MainClass.monitor.Log("Index can only be a number.", LogLevel.Info);
                    return;
                }


                if (BuildingNAnimalMenuPatches.isConstructing || BuildingNAnimalMenuPatches.isMoving)
                {
                    if (BuildingNAnimalMenuPatches.marked[index] == Vector2.Zero)
                    {
                        MainClass.monitor.Log($"No marked position found at {index} index.", LogLevel.Info);
                        return;
                    }
                }
                else
                {
                    if (BuildingNAnimalMenuPatches.availableBuildings[index] == null)
                    {
                        MainClass.monitor.Log($"No building found with index {index}. Use buildlist.", LogLevel.Info);
                        return;
                    }
                }

                string? response = null;
                if (BuildingNAnimalMenuPatches.isConstructing) { response = BuildingNAnimalMenuPatches.Contstruct(BuildingNAnimalMenuPatches.availableBuildings[index], BuildingNAnimalMenuPatches.marked[index]); }
                else if (BuildingNAnimalMenuPatches.isMoving) { response = BuildingNAnimalMenuPatches.Move(BuildingNAnimalMenuPatches.availableBuildings[index], BuildingNAnimalMenuPatches.marked[index]); }
                if (BuildingNAnimalMenuPatches.isDemolishing) { response = BuildingNAnimalMenuPatches.Demolish(BuildingNAnimalMenuPatches.availableBuildings[index]); }
                else if (BuildingNAnimalMenuPatches.isUpgrading) { response = BuildingNAnimalMenuPatches.Upgrade(BuildingNAnimalMenuPatches.availableBuildings[index]); }
                else if (BuildingNAnimalMenuPatches.isPainting) { response = BuildingNAnimalMenuPatches.Paint(BuildingNAnimalMenuPatches.availableBuildings[index]); }

                if (response != null)
                {
                    MainClass.monitor.Log(response, LogLevel.Info);
                }
            });
            #endregion

            helper.ConsoleCommands.Add("refsr", "Refresh screen reader", (string commmand, string[] args) =>
            {
                MainClass.screenReader.InitializeScreenReader();

                MainClass.monitor.Log("Screen Reader refreshed!", LogLevel.Info);
            });
        }
    }
}

