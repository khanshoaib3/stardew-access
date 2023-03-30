using Microsoft.Xna.Framework;
using stardew_access.Features;
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
            //TODO organise this, create separate method for all commands
            IModHelper? helper = MainClass.ModHelper;
            if (helper == null)
                return;

            #region Read Tile
            helper.ConsoleCommands.Add("readtile", "Toggle read tile feature.", (string command, string[] args) =>
                        {
                            MainClass.Config.ReadTile = !MainClass.Config.ReadTile;
                            helper.WriteConfig(MainClass.Config);

                            MainClass.InfoLog("Read Tile is " + (MainClass.Config.ReadTile ? "on" : "off"));
                        });

            helper.ConsoleCommands.Add("flooring", "Toggle flooring in read tile.", (string command, string[] args) =>
            {
                MainClass.Config.ReadFlooring = !MainClass.Config.ReadFlooring;
                helper.WriteConfig(MainClass.Config);

                MainClass.InfoLog("Flooring is " + (MainClass.Config.ReadFlooring ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("watered", "Toggle speaking watered or unwatered for crops.", (string command, string[] args) =>
            {
                MainClass.Config.WateredToggle = !MainClass.Config.WateredToggle;
                helper.WriteConfig(MainClass.Config);

                MainClass.InfoLog("Watered toggle is " + (MainClass.Config.WateredToggle ? "on" : "off"));
            });
            #endregion

            #region Radar Feature
            helper.ConsoleCommands.Add("radar", "Toggle radar feature.", (string command, string[] args) =>
            {
                MainClass.Config.Radar = !MainClass.Config.Radar;
                helper.WriteConfig(MainClass.Config);

                MainClass.InfoLog("Radar " + (MainClass.Config.Radar ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("rdebug", "Toggle debugging in radar feature.", (string command, string[] args) =>
            {
                MainClass.radarDebug = !MainClass.radarDebug;

                MainClass.InfoLog("Radar debugging " + (MainClass.radarDebug ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("rstereo", "Toggle stereo sound in radar feature.", (string command, string[] args) =>
            {
                MainClass.Config.RadarStereoSound = !MainClass.Config.RadarStereoSound;
                helper.WriteConfig(MainClass.Config);

                MainClass.InfoLog("Stereo sound is " + (MainClass.Config.RadarStereoSound ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("rfocus", "Toggle focus mode in radar feature.", (string command, string[] args) =>
            {
                bool focus = MainClass.RadarFeature.ToggleFocus();

                MainClass.InfoLog("Focus mode is " + (focus ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("rdelay", "Set the delay of radar feature in milliseconds.", (string command, string[] args) =>
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
                            MainClass.InfoLog($"Delay set to {MainClass.RadarFeature.delay} milliseconds.");
                        else
                            MainClass.InfoLog($"Delay should be atleast 1 second or 1000 millisecond long.");
                    }
                    else
                    {
                        MainClass.InfoLog("Invalid delay amount, it can only be in numeric form.");
                    }

                }
                else
                {
                    MainClass.InfoLog("Enter the delay amount (in milliseconds)!");
                }

            });

            helper.ConsoleCommands.Add("rrange", "Set the range of radar feature.", (string command, string[] args) =>
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
                            MainClass.InfoLog($"Range set to {MainClass.RadarFeature.range}.");
                        else
                            MainClass.InfoLog($"Range should be atleast 2 and maximum 10.");
                    }
                    else
                    {
                        MainClass.InfoLog("Invalid range amount, it can only be in numeric form.");
                    }

                }
                else
                {
                    MainClass.InfoLog("Enter the range amount!");
                }

            });


            #region Exclusions
            helper.ConsoleCommands.Add("readd", "Add an object key to the exclusions list of radar feature.", (string command, string[] args) =>
                {
                    string? keyToAdd = null;

                    for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                    if (keyToAdd != null)
                    {
                        keyToAdd = keyToAdd.Trim().ToLower();
                        if (!MainClass.RadarFeature.exclusions.Contains(keyToAdd))
                        {
                            MainClass.RadarFeature.exclusions.Add(keyToAdd);
                            MainClass.InfoLog($"Added {keyToAdd} key to exclusions list.");
                        }
                        else
                        {
                            MainClass.InfoLog($"{keyToAdd} key already present in the list.");
                        }
                    }
                    else
                    {
                        MainClass.InfoLog("Unable to add the key to exclusions list.");
                    }
                });

            helper.ConsoleCommands.Add("reremove", "Remove an object key from the exclusions list of radar feature.", (string command, string[] args) =>
            {
                string? keyToAdd = null;

                for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                if (keyToAdd != null)
                {
                    keyToAdd = keyToAdd.Trim().ToLower();
                    if (MainClass.RadarFeature.exclusions.Contains(keyToAdd))
                    {
                        MainClass.RadarFeature.exclusions.Remove(keyToAdd);
                        MainClass.InfoLog($"Removed {keyToAdd} key from exclusions list.");
                    }
                    else
                    {
                        MainClass.InfoLog($"Cannot find {keyToAdd} key in exclusions list.");
                    }
                }
                else
                {
                    MainClass.InfoLog("Unable to remove the key from exclusions list.");
                }
            });

            helper.ConsoleCommands.Add("relist", "List all the exclusions in the radar feature.", (string command, string[] args) =>
            {
                if (MainClass.RadarFeature.exclusions.Count > 0)
                {
                    string toPrint = "";
                    for (int i = 0; i < MainClass.RadarFeature.exclusions.Count; i++)
                    {
                        toPrint = $"{toPrint}\t{i + 1}: {MainClass.RadarFeature.exclusions[i]}";
                    }
                    MainClass.InfoLog(toPrint);
                }
                else
                {
                    MainClass.InfoLog("No exclusions found.");
                }
            });

            helper.ConsoleCommands.Add("reclear", "Clear the focus exclusions in the radar featrure.", (string command, string[] args) =>
            {
                MainClass.RadarFeature.exclusions.Clear();
                MainClass.InfoLog($"Cleared the focus list in the exclusions feature.");
            });

            helper.ConsoleCommands.Add("recount", "Number of exclusions in the radar feature.", (string command, string[] args) =>
            {
                MainClass.InfoLog($"There are {MainClass.RadarFeature.exclusions.Count} exclusiond in the radar feature.");
            });
            #endregion

            #region Focus
            helper.ConsoleCommands.Add("rfadd", "Add an object key to the focus list of radar feature.", (string command, string[] args) =>
                {
                    string? keyToAdd = null;

                    for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                    if (keyToAdd != null)
                    {
                        keyToAdd = keyToAdd.Trim().ToLower();
                        if (!MainClass.RadarFeature.focus.Contains(keyToAdd))
                        {
                            MainClass.RadarFeature.focus.Add(keyToAdd);
                            MainClass.InfoLog($"Added {keyToAdd} key to focus list.");
                        }
                        else
                        {
                            MainClass.InfoLog($"{keyToAdd} key already present in the list.");
                        }
                    }
                    else
                    {
                        MainClass.InfoLog("Unable to add the key to focus list.");
                    }
                });

            helper.ConsoleCommands.Add("rfremove", "Remove an object key from the focus list of radar feature.", (string command, string[] args) =>
            {
                string? keyToAdd = null;

                for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                if (keyToAdd != null)
                {
                    keyToAdd = keyToAdd.Trim().ToLower();
                    if (MainClass.RadarFeature.focus.Contains(keyToAdd))
                    {
                        MainClass.RadarFeature.focus.Remove(keyToAdd);
                        MainClass.InfoLog($"Removed {keyToAdd} key from focus list.");
                    }
                    else
                    {
                        MainClass.InfoLog($"Cannot find {keyToAdd} key in focus list.");
                    }
                }
                else
                {
                    MainClass.InfoLog("Unable to remove the key from focus list.");
                }
            });

            helper.ConsoleCommands.Add("rflist", "List all the exclusions in the radar feature.", (string command, string[] args) =>
            {
                if (MainClass.RadarFeature.focus.Count > 0)
                {
                    string toPrint = "";
                    for (int i = 0; i < MainClass.RadarFeature.focus.Count; i++)
                    {
                        toPrint = $"{toPrint}\t{i + 1}): {MainClass.RadarFeature.focus[i]}";
                    }
                    MainClass.InfoLog(toPrint);
                }
                else
                {
                    MainClass.InfoLog("No objects found in the focus list.");
                }
            });

            helper.ConsoleCommands.Add("rfclear", "Clear the focus list in the radar featrure.", (string command, string[] args) =>
            {
                MainClass.RadarFeature.focus.Clear();
                MainClass.InfoLog($"Cleared the focus list in the radar feature.");
            });

            helper.ConsoleCommands.Add("rfcount", "Number of list in the radar feature.", (string command, string[] args) =>
            {
                MainClass.InfoLog($"There are {MainClass.RadarFeature.focus.Count} objects in the focus list in the radar feature.");
            });
            #endregion

            #endregion

            #region Tile marking
            helper.ConsoleCommands.Add("mark", "Marks the player's position for use in building construction in Carpenter Menu.", (string command, string[] args) =>
            {
                if (Game1.currentLocation is not Farm)
                {
                    MainClass.InfoLog("Can only use this command in the farm");
                    return;
                }

                string? indexInString = args.ElementAtOrDefault(0);
                if (indexInString == null)
                {
                    MainClass.InfoLog("Enter the index too!");
                    return;
                }

                int index;
                bool isParsable = int.TryParse(indexInString, out index);

                if (!isParsable || !(index >= 0 && index <= 9))
                {
                    MainClass.InfoLog("Index can only be a number and from 0 to 9 only");
                    return;
                }

                BuildingOperations.marked[index] = new Vector2((int)Game1.player.getTileX(), (int)Game1.player.getTileY());
                MainClass.InfoLog($"Location {(int)Game1.player.getTileX()}x {(int)Game1.player.getTileY()}y added at {index} index.");
            });

            helper.ConsoleCommands.Add("marklist", "List all marked positions.", (string command, string[] args) =>
            {
                string toPrint = "";
                for (int i = 0; i < BuildingOperations.marked.Length; i++)
                {
                    if (BuildingOperations.marked[i] != Vector2.Zero)
                    {
                        toPrint = $"{toPrint}\n Index {i}: {BuildingOperations.marked[i].X}x {BuildingOperations.marked[i].Y}y";
                    }
                }

                if (toPrint == "")
                    MainClass.InfoLog("No positions marked!");
                else
                    MainClass.InfoLog($"Marked positions:{toPrint}\nOpen command menu and use pageup and pagedown to check the list");
            });

            helper.ConsoleCommands.Add("buildlist", "List all buildings for selection for upgrading/demolishing/painting", (string command, string[] args) =>
            {
                onBuildListCalled();
            });

            helper.ConsoleCommands.Add("buildsel", "Select the building index which you want to upgrade/demolish/paint", (string command, string[] args) =>
            {
                if ((Game1.activeClickableMenu is not CarpenterMenu && Game1.activeClickableMenu is not PurchaseAnimalsMenu && Game1.activeClickableMenu is not AnimalQueryMenu) || !CarpenterMenuPatch.isOnFarm)
                {
                    MainClass.InfoLog($"Cannot select building.");
                    return;
                }

                string? indexInString = args.ElementAtOrDefault(0);
                if (indexInString == null)
                {
                    MainClass.InfoLog("Enter the index of the building too! Use buildlist");
                    return;
                }

                int index;
                bool isParsable = int.TryParse(indexInString, out index);

                if (!isParsable)
                {
                    MainClass.InfoLog("Index can only be a number.");
                    return;
                }

                string? positionIndexInString = args.ElementAtOrDefault(1);
                int positionIndex = 0;

                if (CarpenterMenuPatch.isMoving)
                {

                    if (CarpenterMenuPatch.isConstructing || CarpenterMenuPatch.isMoving)
                    {
                        if (BuildingOperations.availableBuildings[index] == null)
                        {
                            MainClass.InfoLog($"No building found with index {index}. Use buildlist.");
                            return;
                        }

                        if (positionIndexInString == null)
                        {
                            MainClass.InfoLog("Enter the index of marked place too! Use marklist.");
                            return;
                        }

                        isParsable = int.TryParse(positionIndexInString, out positionIndex);

                        if (!isParsable)
                        {
                            MainClass.InfoLog("Index can only be a number.");
                            return;
                        }
                    }
                }
                else if (CarpenterMenuPatch.isConstructing && !CarpenterMenuPatch.isUpgrading)
                {
                    if (BuildingOperations.marked[index] == Vector2.Zero)
                    {
                        MainClass.InfoLog($"No marked position found at {index} index.");
                        return;
                    }
                }
                else
                {
                    if (BuildingOperations.availableBuildings[index] == null)
                    {
                        MainClass.InfoLog($"No building found with index {index}. Use buildlist.");
                        return;
                    }
                }

                string? response = null;

                if (Game1.activeClickableMenu is PurchaseAnimalsMenu)
                {
                    BuildingOperations.PurchaseAnimal(BuildingOperations.availableBuildings[index]);
                }
                else if (Game1.activeClickableMenu is AnimalQueryMenu)
                {
                    BuildingOperations.MoveAnimal(BuildingOperations.availableBuildings[index]);
                }
                else
                {
                    if (CarpenterMenuPatch.isConstructing && !CarpenterMenuPatch.isUpgrading) { response = BuildingOperations.Contstruct(BuildingOperations.marked[index]); }
                    else if (CarpenterMenuPatch.isMoving) { response = BuildingOperations.Move(BuildingOperations.availableBuildings[index], BuildingOperations.marked[positionIndex]); }
                    else if (CarpenterMenuPatch.isDemolishing) { response = BuildingOperations.Demolish(BuildingOperations.availableBuildings[index]); }
                    else if (CarpenterMenuPatch.isUpgrading) { response = BuildingOperations.Upgrade(BuildingOperations.availableBuildings[index]); }
                    else if (CarpenterMenuPatch.isPainting) { response = BuildingOperations.Paint(BuildingOperations.availableBuildings[index]); }
                }

                if (response != null)
                {
                    MainClass.InfoLog(response);
                }
            });
            #endregion

            #region Other
            helper.ConsoleCommands.Add("refsr", "Refresh screen reader", (string command, string[] args) =>
                        {
                            MainClass.ScreenReader.InitializeScreenReader();

                            MainClass.InfoLog("Screen Reader refreshed!");
                        });

            helper.ConsoleCommands.Add("refmc", "Refresh mod config", (string command, string[] args) =>
            {
                MainClass.Config = helper.ReadConfig<ModConfig>();

                MainClass.InfoLog("Mod Config refreshed!");
            });

            helper.ConsoleCommands.Add("refst", "Refresh static tiles", (string command, string[] args) =>
            {
                StaticTiles.LoadTilesFiles();
                StaticTiles.SetupTilesDicts();

                MainClass.InfoLog("Static tiles refreshed!");
            });

            helper.ConsoleCommands.Add("hnspercent", "Toggle between speaking in percentage or full health and stamina.", (string command, string[] args) =>
            {
                MainClass.Config.HealthNStaminaInPercentage = !MainClass.Config.HealthNStaminaInPercentage;
                helper.WriteConfig(MainClass.Config);

                MainClass.InfoLog("Speaking in percentage is " + (MainClass.Config.HealthNStaminaInPercentage ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("snapmouse", "Toggle snap mouse feature.", (string command, string[] args) =>
            {
                MainClass.Config.SnapMouse = !MainClass.Config.SnapMouse;
                helper.WriteConfig(MainClass.Config);

                MainClass.InfoLog("Snap Mouse is " + (MainClass.Config.SnapMouse ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("warning", "Toggle warnings feature.", (string command, string[] args) =>
            {
                MainClass.Config.Warning = !MainClass.Config.Warning;
                helper.WriteConfig(MainClass.Config);

                MainClass.InfoLog("Warnings is " + (MainClass.Config.Warning ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("tts", "Toggles the screen reader/tts", (string command, string[] args) =>
            {
                MainClass.Config.TTS = !MainClass.Config.TTS;
                helper.WriteConfig(MainClass.Config);

                MainClass.InfoLog("TTS is " + (MainClass.Config.TTS ? "on" : "off"));
            });
            #endregion
        }

        internal static void onBuildListCalled()
        {
            string toPrint = "";
            Farm farm = (Farm)Game1.getLocationFromName("Farm");
            Netcode.NetCollection<Building> buildings = farm.buildings;
            int buildingIndex = 0;

            for (int i = 0; i < buildings.Count; i++)
            {
                string? name = buildings[i].nameOfIndoorsWithoutUnique;
                name = (name == "null") ? buildings[i].buildingType.Value : name;

                BuildingOperations.availableBuildings[buildingIndex] = buildings[i];
                toPrint = $"{toPrint}\nIndex {buildingIndex}: {name}: At {buildings[i].tileX}x and {buildings[i].tileY}y";
                ++buildingIndex;
            }

            if (toPrint == "")
            {
                MainClass.InfoLog("No appropriate buildings to list");
            }
            else
            {
                MainClass.InfoLog($"Available buildings:{toPrint}\nOpen command menu and use pageup and pagedown to check the list");
            }
        }
    }
}

