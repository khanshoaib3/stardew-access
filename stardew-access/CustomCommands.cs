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

                MainClass.Monitor.Log("Read Tile is " + (MainClass.readTile ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("snapmouse", "Toggle snap mouse feature.", (string commmand, string[] args) =>
            {
                MainClass.snapMouse = !MainClass.snapMouse;

                MainClass.Monitor.Log("Snap Mouse is " + (MainClass.snapMouse ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("radar", "Toggle radar feature.", (string commmand, string[] args) =>
            {
                MainClass.radar = !MainClass.radar;

                MainClass.Monitor.Log("Radar " + (MainClass.radar ? "on" : "off"), LogLevel.Info);
            });

            #region Radar Feature
            helper.ConsoleCommands.Add("rdebug", "Toggle debugging in radar feature.", (string commmand, string[] args) =>
            {
                MainClass.radarDebug = !MainClass.radarDebug;

                MainClass.Monitor.Log("Radar debugging " + (MainClass.radarDebug ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("rstereo", "Toggle stereo sound in radar feature.", (string commmand, string[] args) =>
            {
                MainClass.radarStereoSound = !MainClass.radarStereoSound;

                MainClass.Monitor.Log("Stereo sound is " + (MainClass.radarStereoSound ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("rfocus", "Toggle focus mode in radar feature.", (string commmand, string[] args) =>
            {
                bool focus = MainClass.RadarFeature.ToggleFocus();

                MainClass.Monitor.Log("Focus mode is " + (focus ? "on" : "off"), LogLevel.Info);
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
                        MainClass.RadarFeature.delay = delay;
                        if (delay >= 1000)
                            MainClass.Monitor.Log($"Delay set to {MainClass.RadarFeature.delay} milliseconds.", LogLevel.Info);
                        else
                            MainClass.Monitor.Log($"Delay should be atleast 1 second or 1000 millisecond long.", LogLevel.Info);
                    }
                    else
                    {
                        MainClass.Monitor.Log("Invalid delay amount, it can only be in numeric form.", LogLevel.Info);
                    }

                }
                else
                {
                    MainClass.Monitor.Log("Enter the delay amount (in milliseconds)!", LogLevel.Info);
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
                        MainClass.RadarFeature.range = range;
                        if (range >= 2 && range <= 10)
                            MainClass.Monitor.Log($"Range set to {MainClass.RadarFeature.range}.", LogLevel.Info);
                        else
                            MainClass.Monitor.Log($"Range should be atleast 2 and maximum 10.", LogLevel.Info);
                    }
                    else
                    {
                        MainClass.Monitor.Log("Invalid range amount, it can only be in numeric form.", LogLevel.Info);
                    }

                }
                else
                {
                    MainClass.Monitor.Log("Enter the range amount!", LogLevel.Info);
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
                        if (!MainClass.RadarFeature.exclusions.Contains(keyToAdd))
                        {
                            MainClass.RadarFeature.exclusions.Add(keyToAdd);
                            MainClass.Monitor.Log($"Added {keyToAdd} key to exclusions list.", LogLevel.Info);
                        }
                        else
                        {
                            MainClass.Monitor.Log($"{keyToAdd} key already present in the list.", LogLevel.Info);
                        }
                    }
                    else
                    {
                        MainClass.Monitor.Log("Unable to add the key to exclusions list.", LogLevel.Info);
                    }
                });

            helper.ConsoleCommands.Add("reremove", "Remove an object key from the exclusions list of radar feature.", (string commmand, string[] args) =>
            {
                string? keyToAdd = null;

                for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                if (keyToAdd != null)
                {
                    keyToAdd = keyToAdd.Trim().ToLower();
                    if (MainClass.RadarFeature.exclusions.Contains(keyToAdd))
                    {
                        MainClass.RadarFeature.exclusions.Remove(keyToAdd);
                        MainClass.Monitor.Log($"Removed {keyToAdd} key from exclusions list.", LogLevel.Info);
                    }
                    else
                    {
                        MainClass.Monitor.Log($"Cannot find {keyToAdd} key in exclusions list.", LogLevel.Info);
                    }
                }
                else
                {
                    MainClass.Monitor.Log("Unable to remove the key from exclusions list.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("relist", "List all the exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                if (MainClass.RadarFeature.exclusions.Count > 0)
                {
                    string toPrint = "";
                    for (int i = 0; i < MainClass.RadarFeature.exclusions.Count; i++)
                    {
                        toPrint = $"{toPrint}\t{i + 1}: {MainClass.RadarFeature.exclusions[i]}";
                    }
                    MainClass.Monitor.Log(toPrint, LogLevel.Info);
                }
                else
                {
                    MainClass.Monitor.Log("No exclusions found.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("reclear", "Clear the focus exclusions in the radar featrure.", (string commmand, string[] args) =>
            {
                MainClass.RadarFeature.exclusions.Clear();
                MainClass.Monitor.Log($"Cleared the focus list in the exclusions feature.", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("recount", "Number of exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                MainClass.Monitor.Log($"There are {MainClass.RadarFeature.exclusions.Count} exclusiond in the radar feature.", LogLevel.Info);
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
                        if (!MainClass.RadarFeature.focus.Contains(keyToAdd))
                        {
                            MainClass.RadarFeature.focus.Add(keyToAdd);
                            MainClass.Monitor.Log($"Added {keyToAdd} key to focus list.", LogLevel.Info);
                        }
                        else
                        {
                            MainClass.Monitor.Log($"{keyToAdd} key already present in the list.", LogLevel.Info);
                        }
                    }
                    else
                    {
                        MainClass.Monitor.Log("Unable to add the key to focus list.", LogLevel.Info);
                    }
                });

            helper.ConsoleCommands.Add("rfremove", "Remove an object key from the focus list of radar feature.", (string commmand, string[] args) =>
            {
                string? keyToAdd = null;

                for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                if (keyToAdd != null)
                {
                    keyToAdd = keyToAdd.Trim().ToLower();
                    if (MainClass.RadarFeature.focus.Contains(keyToAdd))
                    {
                        MainClass.RadarFeature.focus.Remove(keyToAdd);
                        MainClass.Monitor.Log($"Removed {keyToAdd} key from focus list.", LogLevel.Info);
                    }
                    else
                    {
                        MainClass.Monitor.Log($"Cannot find {keyToAdd} key in focus list.", LogLevel.Info);
                    }
                }
                else
                {
                    MainClass.Monitor.Log("Unable to remove the key from focus list.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("rflist", "List all the exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                if (MainClass.RadarFeature.focus.Count > 0)
                {
                    string toPrint = "";
                    for (int i = 0; i < MainClass.RadarFeature.focus.Count; i++)
                    {
                        toPrint = $"{toPrint}\t{i + 1}): {MainClass.RadarFeature.focus[i]}";
                    }
                    MainClass.Monitor.Log(toPrint, LogLevel.Info);
                }
                else
                {
                    MainClass.Monitor.Log("No objects found in the focus list.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("rfclear", "Clear the focus list in the radar featrure.", (string commmand, string[] args) =>
            {
                MainClass.RadarFeature.focus.Clear();
                MainClass.Monitor.Log($"Cleared the focus list in the radar feature.", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("rfcount", "Number of list in the radar feature.", (string commmand, string[] args) =>
            {
                MainClass.Monitor.Log($"There are {MainClass.RadarFeature.focus.Count} objects in the focus list in the radar feature.", LogLevel.Info);
            });
            #endregion

            #endregion

            #region Tile marking
            helper.ConsoleCommands.Add("mark", "Marks the player's position for use in building construction in Carpenter Menu.", (string commmand, string[] args) =>
            {
                if (Game1.currentLocation is not Farm)
                {
                    MainClass.Monitor.Log("Can only use this command in the farm", LogLevel.Info);
                    return;
                }

                string? indexInString = args.ElementAtOrDefault(0);
                if (indexInString == null)
                {
                    MainClass.Monitor.Log("Enter the index too!", LogLevel.Info);
                    return;
                }

                int index;
                bool isParsable = int.TryParse(indexInString, out index);

                if (!isParsable || !(index >= 0 && index <= 9))
                {
                    MainClass.Monitor.Log("Index can only be a number and from 0 to 9 only", LogLevel.Info);
                    return;
                }

                BuildingNAnimalMenuPatches.marked[index] = new Vector2((int)Game1.player.getTileX(), (int)Game1.player.getTileY());
                MainClass.Monitor.Log($"Location {(int)Game1.player.getTileX()}x {(int)Game1.player.getTileY()}y added at {index} index.", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("marklist", "List all marked positions.", (string commmand, string[] args) =>
            {
                string toPrint = "";
                for (int i = 0; i < BuildingNAnimalMenuPatches.marked.Length; i++)
                {
                    if (BuildingNAnimalMenuPatches.marked[i] != Vector2.Zero)
                    {
                        toPrint = $"{toPrint}\n Index {i}: {BuildingNAnimalMenuPatches.marked[i].X}x {BuildingNAnimalMenuPatches.marked[i].Y}y";
                    }
                }

                if (toPrint == "")
                    MainClass.Monitor.Log("No positions marked!", LogLevel.Info);
                else
                    MainClass.Monitor.Log($"Marked positions:{toPrint}\nOpen command menu and use pageup and pagedown to check the list", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("buildlist", "List all buildings for selection for upgrading/demolishing/painting", (string commmand, string[] args) =>
            {
                if ((Game1.activeClickableMenu is not CarpenterMenu && Game1.activeClickableMenu is not PurchaseAnimalsMenu) || !BuildingNAnimalMenuPatches.isOnFarm)
                {
                    MainClass.Monitor.Log($"Cannot list buildings.", LogLevel.Info);
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
                    MainClass.Monitor.Log("No appropriate buildings to list", LogLevel.Info);
                }
                else
                {
                    MainClass.Monitor.Log($"Available buildings:{toPrint}\nOpen command menu and use pageup and pagedown to check the list", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("buildsel", "Select the building index which you want to upgrade/demolish/paint", (string commmand, string[] args) =>
            {
                if ((Game1.activeClickableMenu is not CarpenterMenu && Game1.activeClickableMenu is not PurchaseAnimalsMenu) || !BuildingNAnimalMenuPatches.isOnFarm)
                {
                    MainClass.Monitor.Log($"Cannot select building.", LogLevel.Info);
                    return;
                }

                string? indexInString = args.ElementAtOrDefault(0);
                if (indexInString == null)
                {
                    MainClass.Monitor.Log("Enter the index of the building too! Use buildlist", LogLevel.Info);
                    return;
                }

                int index;
                bool isParsable = int.TryParse(indexInString, out index);

                if (!isParsable)
                {
                    MainClass.Monitor.Log("Index can only be a number.", LogLevel.Info);
                    return;
                }

                string positionIndexInString = args.ElementAtOrDefault(1);
                int positionIndex = 0;

                if (BuildingNAnimalMenuPatches.isMoving)
                {

                    if (BuildingNAnimalMenuPatches.isConstructing || BuildingNAnimalMenuPatches.isMoving)
                    {
                        if (BuildingNAnimalMenuPatches.availableBuildings[index] == null)
                        {
                            MainClass.Monitor.Log($"No building found with index {index}. Use buildlist.", LogLevel.Info);
                            return;
                        }

                        if (positionIndexInString == null)
                        {
                            MainClass.Monitor.Log("Enter the index of marked place too! Use marklist.", LogLevel.Info);
                            return;
                        }

                        isParsable = int.TryParse(positionIndexInString, out positionIndex);

                        if (!isParsable)
                        {
                            MainClass.Monitor.Log("Index can only be a number.", LogLevel.Info);
                            return;
                        }
                    }
                }
                else if (BuildingNAnimalMenuPatches.isConstructing && !BuildingNAnimalMenuPatches.isUpgrading)
                {
                    if (BuildingNAnimalMenuPatches.marked[index] == Vector2.Zero)
                    {
                        MainClass.Monitor.Log($"No marked position found at {index} index.", LogLevel.Info);
                        return;
                    }
                }
                else
                {
                    if (BuildingNAnimalMenuPatches.availableBuildings[index] == null)
                    {
                        MainClass.Monitor.Log($"No building found with index {index}. Use buildlist.", LogLevel.Info);
                        return;
                    }
                }

                string? response = null;

                if (Game1.activeClickableMenu is PurchaseAnimalsMenu) { BuildingNAnimalMenuPatches.PurchaseAnimal(BuildingNAnimalMenuPatches.availableBuildings[index]); }
                else
                {
                    if (BuildingNAnimalMenuPatches.isConstructing && !BuildingNAnimalMenuPatches.isUpgrading) { response = BuildingNAnimalMenuPatches.Contstruct(BuildingNAnimalMenuPatches.marked[index]); }
                    else if (BuildingNAnimalMenuPatches.isMoving) { response = BuildingNAnimalMenuPatches.Move(BuildingNAnimalMenuPatches.availableBuildings[index], BuildingNAnimalMenuPatches.marked[positionIndex]); }
                    else if (BuildingNAnimalMenuPatches.isDemolishing) { response = BuildingNAnimalMenuPatches.Demolish(BuildingNAnimalMenuPatches.availableBuildings[index]); }
                    else if (BuildingNAnimalMenuPatches.isUpgrading) { response = BuildingNAnimalMenuPatches.Upgrade(BuildingNAnimalMenuPatches.availableBuildings[index]); }
                    else if (BuildingNAnimalMenuPatches.isPainting) { response = BuildingNAnimalMenuPatches.Paint(BuildingNAnimalMenuPatches.availableBuildings[index]); }
                }

                if (response != null)
                {
                    MainClass.Monitor.Log(response, LogLevel.Info);
                }
            });
            #endregion

            helper.ConsoleCommands.Add("refsr", "Refresh screen reader", (string commmand, string[] args) =>
            {
                MainClass.ScreenReader.InitializeScreenReader();

                MainClass.Monitor.Log("Screen Reader refreshed!", LogLevel.Info);
            });
        }
    }
}

