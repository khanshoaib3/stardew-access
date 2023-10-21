using Microsoft.Xna.Framework;
using stardew_access.Features;
using stardew_access.Utils;
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

            #region Tile marking
            helper.ConsoleCommands.Add("mark", "Marks the player's position for use in building construction in Carpenter Menu.", (string command, string[] args) =>
            {
                if (Game1.currentLocation is not Farm)
                {
                    Log.Info("Can only use this command in the farm");
                    return;
                }

                string? indexInString = args.ElementAtOrDefault(0);
                if (indexInString == null)
                {
                    Log.Info("Enter the index too!");
                    return;
                }

                bool isParsable = int.TryParse(indexInString, out int index);

                if (!isParsable || !(index >= 0 && index <= 9))
                {
                    Log.Info("Index can only be a number and from 0 to 9 only");
                    return;
                }

                BuildingOperations.marked[index] = new Vector2((int)Game1.player.getTileX(), (int)Game1.player.getTileY());
                Log.Info($"Location {(int)Game1.player.getTileX()}x {(int)Game1.player.getTileY()}y added at {index} index.");
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
                    Log.Info("No positions marked!");
                else
                    Log.Info($"Marked positions:{toPrint}\nOpen command menu and use pageup and pagedown to check the list");
            });

            helper.ConsoleCommands.Add("buildlist", "List all buildings for selection for upgrading/demolishing/painting", (string command, string[] args) =>
            {
                OnBuildListCalled();
            });

            helper.ConsoleCommands.Add("buildsel", "Select the building index which you want to upgrade/demolish/paint", (string command, string[] args) =>
            {
                if ((Game1.activeClickableMenu is not CarpenterMenu && Game1.activeClickableMenu is not PurchaseAnimalsMenu && Game1.activeClickableMenu is not AnimalQueryMenu) || (!CarpenterMenuPatch.isOnFarm && !PurchaseAnimalsMenuPatch.isOnFarm && !AnimalQueryMenuPatch.isOnFarm))
                {
                    Log.Info($"Cannot select building.");
                    return;
                }

                string? indexInString = args.ElementAtOrDefault(0);
                if (indexInString == null)
                {
                    Log.Info("Enter the index of the building too! Use buildlist");
                    return;
                }

                bool isParsable = int.TryParse(indexInString, out int index);

                if (!isParsable)
                {
                    Log.Info("Index can only be a number.");
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
                            Log.Info($"No building found with index {index}. Use buildlist.");
                            return;
                        }

                        if (positionIndexInString == null)
                        {
                            Log.Info("Enter the index of marked place too! Use marklist.");
                            return;
                        }

                        isParsable = int.TryParse(positionIndexInString, out positionIndex);

                        if (!isParsable)
                        {
                            Log.Info("Index can only be a number.");
                            return;
                        }
                    }
                }
                else if (CarpenterMenuPatch.isConstructing && !CarpenterMenuPatch.isUpgrading)
                {
                    if (BuildingOperations.marked[index] == Vector2.Zero)
                    {
                        Log.Info($"No marked position found at {index} index.");
                        return;
                    }
                }
                else
                {
                    if (BuildingOperations.availableBuildings[index] == null)
                    {
                        Log.Info($"No building found with index {index}. Use buildlist.");
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
                    if (CarpenterMenuPatch.isConstructing && !CarpenterMenuPatch.isUpgrading) { response = BuildingOperations.Construct(BuildingOperations.marked[index]); }
                    else if (CarpenterMenuPatch.isMoving) { response = BuildingOperations.Move(BuildingOperations.availableBuildings[index], BuildingOperations.marked[positionIndex]); }
                    else if (CarpenterMenuPatch.isDemolishing) { response = BuildingOperations.Demolish(BuildingOperations.availableBuildings[index]); }
                    else if (CarpenterMenuPatch.isUpgrading) { response = BuildingOperations.Upgrade(BuildingOperations.availableBuildings[index]); }
                    else if (CarpenterMenuPatch.isPainting) { response = BuildingOperations.Paint(BuildingOperations.availableBuildings[index]); }
                }

                if (response != null)
                {
                    Log.Info(response);
                }
            });
            #endregion

            #region Other
            helper.ConsoleCommands.Add("refsr", "Refresh screen reader", (string command, string[] args) =>
                        {
                            MainClass.ScreenReader.InitializeScreenReader();

                            Log.Info("Screen Reader refreshed!");
                        });

            helper.ConsoleCommands.Add("refmc", "Refresh mod config", (string command, string[] args) =>
            {
                MainClass.Config = helper.ReadConfig<ModConfig>();

                Log.Info("Mod Config refreshed!");
            });

            // TODO: add Refresh functionality to `AccessibleTileManager and restore this
            /*helper.ConsoleCommands.Add("refst", "Refresh static tiles", (string command, string[] args) =>
            {
                StaticTiles.LoadTilesFiles();
                StaticTiles.SetupTilesDicts();

                Log.Info("Static tiles refreshed!");
            });*/

            helper.ConsoleCommands.Add("hnspercent", "Toggle between speaking in percentage or full health and stamina.", (string command, string[] args) =>
            {
                MainClass.Config.HealthNStaminaInPercentage = !MainClass.Config.HealthNStaminaInPercentage;
                helper.WriteConfig(MainClass.Config);

                Log.Info("Speaking in percentage is " + (MainClass.Config.HealthNStaminaInPercentage ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("snapmouse", "Toggle snap mouse feature.", (string command, string[] args) =>
            {
                MainClass.Config.SnapMouse = !MainClass.Config.SnapMouse;
                helper.WriteConfig(MainClass.Config);

                Log.Info("Snap Mouse is " + (MainClass.Config.SnapMouse ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("warning", "Toggle warnings feature.", (string command, string[] args) =>
            {
                MainClass.Config.Warning = !MainClass.Config.Warning;
                helper.WriteConfig(MainClass.Config);

                Log.Info("Warnings is " + (MainClass.Config.Warning ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("tts", "Toggles the screen reader/tts", (string command, string[] args) =>
            {
                MainClass.Config.TTS = !MainClass.Config.TTS;
                helper.WriteConfig(MainClass.Config);

                Log.Info("TTS is " + (MainClass.Config.TTS ? "on" : "off"));
            });
            #endregion
        }

        internal static void OnBuildListCalled()
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
                Log.Info("No appropriate buildings to list");
            }
            else
            {
                Log.Info($"Available buildings:{toPrint}\nOpen command menu and use pageup and pagedown to check the list");
            }
        }
    }
}

