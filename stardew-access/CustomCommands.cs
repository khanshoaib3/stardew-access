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
            IModHelper? helper = MainClass.ModHelper;
            if (helper == null)
                return;

            helper.ConsoleCommands.Add("readtile", "Toggle read tile feature.", (string commmand, string[] args) =>
            {
                MainClass.Config.ReadTile = !MainClass.Config.ReadTile;
                helper.WriteConfig(MainClass.Config);

                MainClass.DebugLog("Read Tile is " + (MainClass.Config.ReadTile ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("snapmouse", "Toggle snap mouse feature.", (string commmand, string[] args) =>
            {
                MainClass.Config.SnapMouse = !MainClass.Config.SnapMouse;
                helper.WriteConfig(MainClass.Config);

                MainClass.DebugLog("Snap Mouse is " + (MainClass.Config.SnapMouse ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("flooring", "Toggle flooring in read tile.", (string commmand, string[] args) =>
            {
                MainClass.Config.ReadFlooring = !MainClass.Config.ReadFlooring;
                helper.WriteConfig(MainClass.Config);

                MainClass.DebugLog("Flooring is " + (MainClass.Config.ReadFlooring ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("radar", "Toggle radar feature.", (string commmand, string[] args) =>
            {
                MainClass.Config.Radar = !MainClass.Config.Radar;
                helper.WriteConfig(MainClass.Config);

                MainClass.DebugLog("Radar " + (MainClass.Config.Radar ? "on" : "off"));
            });

            #region Radar Feature
            helper.ConsoleCommands.Add("rdebug", "Toggle debugging in radar feature.", (string commmand, string[] args) =>
            {
                MainClass.radarDebug = !MainClass.radarDebug;

                MainClass.DebugLog("Radar debugging " + (MainClass.radarDebug ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("rstereo", "Toggle stereo sound in radar feature.", (string commmand, string[] args) =>
            {
                MainClass.Config.RadarStereoSound = !MainClass.Config.RadarStereoSound;
                helper.WriteConfig(MainClass.Config);

                MainClass.DebugLog("Stereo sound is " + (MainClass.Config.RadarStereoSound ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("rfocus", "Toggle focus mode in radar feature.", (string commmand, string[] args) =>
            {
                bool focus = MainClass.RadarFeature.ToggleFocus();

                MainClass.DebugLog("Focus mode is " + (focus ? "on" : "off"));
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
                            MainClass.DebugLog($"Delay set to {MainClass.RadarFeature.delay} milliseconds.");
                        else
                            MainClass.DebugLog($"Delay should be atleast 1 second or 1000 millisecond long.");
                    }
                    else
                    {
                        MainClass.DebugLog("Invalid delay amount, it can only be in numeric form.");
                    }

                }
                else
                {
                    MainClass.DebugLog("Enter the delay amount (in milliseconds)!");
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
                            MainClass.DebugLog($"Range set to {MainClass.RadarFeature.range}.");
                        else
                            MainClass.DebugLog($"Range should be atleast 2 and maximum 10.");
                    }
                    else
                    {
                        MainClass.DebugLog("Invalid range amount, it can only be in numeric form.");
                    }

                }
                else
                {
                    MainClass.DebugLog("Enter the range amount!");
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
                            MainClass.DebugLog($"Added {keyToAdd} key to exclusions list.");
                        }
                        else
                        {
                            MainClass.DebugLog($"{keyToAdd} key already present in the list.");
                        }
                    }
                    else
                    {
                        MainClass.DebugLog("Unable to add the key to exclusions list.");
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
                        MainClass.DebugLog($"Removed {keyToAdd} key from exclusions list.");
                    }
                    else
                    {
                        MainClass.DebugLog($"Cannot find {keyToAdd} key in exclusions list.");
                    }
                }
                else
                {
                    MainClass.DebugLog("Unable to remove the key from exclusions list.");
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
                    MainClass.DebugLog(toPrint);
                }
                else
                {
                    MainClass.DebugLog("No exclusions found.");
                }
            });

            helper.ConsoleCommands.Add("reclear", "Clear the focus exclusions in the radar featrure.", (string commmand, string[] args) =>
            {
                MainClass.RadarFeature.exclusions.Clear();
                MainClass.DebugLog($"Cleared the focus list in the exclusions feature.");
            });

            helper.ConsoleCommands.Add("recount", "Number of exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                MainClass.DebugLog($"There are {MainClass.RadarFeature.exclusions.Count} exclusiond in the radar feature.");
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
                            MainClass.DebugLog($"Added {keyToAdd} key to focus list.");
                        }
                        else
                        {
                            MainClass.DebugLog($"{keyToAdd} key already present in the list.");
                        }
                    }
                    else
                    {
                        MainClass.DebugLog("Unable to add the key to focus list.");
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
                        MainClass.DebugLog($"Removed {keyToAdd} key from focus list.");
                    }
                    else
                    {
                        MainClass.DebugLog($"Cannot find {keyToAdd} key in focus list.");
                    }
                }
                else
                {
                    MainClass.DebugLog("Unable to remove the key from focus list.");
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
                    MainClass.DebugLog(toPrint);
                }
                else
                {
                    MainClass.DebugLog("No objects found in the focus list.");
                }
            });

            helper.ConsoleCommands.Add("rfclear", "Clear the focus list in the radar featrure.", (string commmand, string[] args) =>
            {
                MainClass.RadarFeature.focus.Clear();
                MainClass.DebugLog($"Cleared the focus list in the radar feature.");
            });

            helper.ConsoleCommands.Add("rfcount", "Number of list in the radar feature.", (string commmand, string[] args) =>
            {
                MainClass.DebugLog($"There are {MainClass.RadarFeature.focus.Count} objects in the focus list in the radar feature.");
            });
            #endregion

            #endregion

            #region Tile marking
            helper.ConsoleCommands.Add("mark", "Marks the player's position for use in building construction in Carpenter Menu.", (string commmand, string[] args) =>
            {
                if (Game1.currentLocation is not Farm)
                {
                    MainClass.DebugLog("Can only use this command in the farm");
                    return;
                }

                string? indexInString = args.ElementAtOrDefault(0);
                if (indexInString == null)
                {
                    MainClass.DebugLog("Enter the index too!");
                    return;
                }

                int index;
                bool isParsable = int.TryParse(indexInString, out index);

                if (!isParsable || !(index >= 0 && index <= 9))
                {
                    MainClass.DebugLog("Index can only be a number and from 0 to 9 only");
                    return;
                }

                BuildingNAnimalMenuPatches.marked[index] = new Vector2((int)Game1.player.getTileX(), (int)Game1.player.getTileY());
                MainClass.DebugLog($"Location {(int)Game1.player.getTileX()}x {(int)Game1.player.getTileY()}y added at {index} index.");
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
                    MainClass.DebugLog("No positions marked!");
                else
                    MainClass.DebugLog($"Marked positions:{toPrint}\nOpen command menu and use pageup and pagedown to check the list");
            });

            helper.ConsoleCommands.Add("buildlist", "List all buildings for selection for upgrading/demolishing/painting", (string commmand, string[] args) =>
            {
                if ((Game1.activeClickableMenu is not CarpenterMenu && Game1.activeClickableMenu is not PurchaseAnimalsMenu && Game1.activeClickableMenu is not AnimalQueryMenu) || !BuildingNAnimalMenuPatches.isOnFarm)
                {
                    MainClass.DebugLog($"Cannot list buildings.");
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
                    MainClass.DebugLog("No appropriate buildings to list");
                }
                else
                {
                    MainClass.DebugLog($"Available buildings:{toPrint}\nOpen command menu and use pageup and pagedown to check the list");
                }
            });

            helper.ConsoleCommands.Add("buildsel", "Select the building index which you want to upgrade/demolish/paint", (string commmand, string[] args) =>
            {
                if ((Game1.activeClickableMenu is not CarpenterMenu && Game1.activeClickableMenu is not PurchaseAnimalsMenu && Game1.activeClickableMenu is not AnimalQueryMenu) || !BuildingNAnimalMenuPatches.isOnFarm)
                {
                    MainClass.DebugLog($"Cannot select building.");
                    return;
                }

                string? indexInString = args.ElementAtOrDefault(0);
                if (indexInString == null)
                {
                    MainClass.DebugLog("Enter the index of the building too! Use buildlist");
                    return;
                }

                int index;
                bool isParsable = int.TryParse(indexInString, out index);

                if (!isParsable)
                {
                    MainClass.DebugLog("Index can only be a number.");
                    return;
                }

                string? positionIndexInString = args.ElementAtOrDefault(1);
                int positionIndex = 0;

                if (BuildingNAnimalMenuPatches.isMoving)
                {

                    if (BuildingNAnimalMenuPatches.isConstructing || BuildingNAnimalMenuPatches.isMoving)
                    {
                        if (BuildingNAnimalMenuPatches.availableBuildings[index] == null)
                        {
                            MainClass.DebugLog($"No building found with index {index}. Use buildlist.");
                            return;
                        }

                        if (positionIndexInString == null)
                        {
                            MainClass.DebugLog("Enter the index of marked place too! Use marklist.");
                            return;
                        }

                        isParsable = int.TryParse(positionIndexInString, out positionIndex);

                        if (!isParsable)
                        {
                            MainClass.DebugLog("Index can only be a number.");
                            return;
                        }
                    }
                }
                else if (BuildingNAnimalMenuPatches.isConstructing && !BuildingNAnimalMenuPatches.isUpgrading)
                {
                    if (BuildingNAnimalMenuPatches.marked[index] == Vector2.Zero)
                    {
                        MainClass.DebugLog($"No marked position found at {index} index.");
                        return;
                    }
                }
                else
                {
                    if (BuildingNAnimalMenuPatches.availableBuildings[index] == null)
                    {
                        MainClass.DebugLog($"No building found with index {index}. Use buildlist.");
                        return;
                    }
                }

                string? response = null;

                if (Game1.activeClickableMenu is PurchaseAnimalsMenu)
                {
                    BuildingNAnimalMenuPatches.PurchaseAnimal(BuildingNAnimalMenuPatches.availableBuildings[index]);
                }
                else if (Game1.activeClickableMenu is AnimalQueryMenu)
                {
                    BuildingNAnimalMenuPatches.MoveAnimal(BuildingNAnimalMenuPatches.availableBuildings[index]);
                }
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
                    MainClass.DebugLog(response);
                }
            });
            #endregion

            helper.ConsoleCommands.Add("refsr", "Refresh screen reader", (string commmand, string[] args) =>
            {
                MainClass.ScreenReader.InitializeScreenReader();

                MainClass.DebugLog("Screen Reader refreshed!");
            });

            helper.ConsoleCommands.Add("refmc", "Refresh mod config", (string commmand, string[] args) =>
            {
                MainClass.Config = helper.ReadConfig<ModConfig>();

                MainClass.DebugLog("Mod Config refreshed!");
            });

            helper.ConsoleCommands.Add("refst", "Refresh static tiles", (string commmand, string[] args) =>
            {
                MainClass.STiles = new Features.StaticTiles();

                MainClass.DebugLog("Static tiles refreshed!");
            });
        }
    }
}

