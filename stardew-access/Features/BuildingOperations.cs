using Microsoft.Xna.Framework;
using stardew_access.Patches;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;

namespace stardew_access.Features
{
    internal class BuildingOperations
    {
        internal static Building?[] availableBuildings = new Building[100];
        internal static Vector2[] marked = new Vector2[10];

        public static string? Demolish(Building? toDemolish)
        {
            if (toDemolish == null)
                return null;

            string? response = null;
            // This code is taken from the game's code (CarpenterMenu.cs::654)
            Farm farm = (Farm)Game1.getLocationFromName("Farm");
            Action buildingLockFailed = delegate
            {
                if (CarpenterMenuPatch.isDemolishing)
                {
                    response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed");
                }
            };
            Action continueDemolish = delegate
            {
                if (CarpenterMenuPatch.isDemolishing && toDemolish != null && farm.buildings.Contains(toDemolish))
                {
                    if ((int)toDemolish.daysOfConstructionLeft.Value > 0 || (int)toDemolish.daysUntilUpgrade.Value > 0)
                    {
                        response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction");
                    }
                    else if (toDemolish.indoors.Value != null && toDemolish.indoors.Value is AnimalHouse && ((AnimalHouse)toDemolish.indoors.Value).animalsThatLiveHere.Count > 0)
                    {
                        response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere");
                    }
                    else if (toDemolish.indoors.Value != null && toDemolish.indoors.Value.farmers.Any())
                    {
                        response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere");
                    }
                    else
                    {
                        if (toDemolish.indoors.Value != null && toDemolish.indoors.Value is Cabin)
                        {
                            foreach (Farmer current in Game1.getAllFarmers())
                            {
                                if (current.currentLocation != null && current.currentLocation.Name == ((Cabin)toDemolish.indoors.Value).GetCellarName())
                                {
                                    response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere");
                                    return;
                                }
                            }
                        }
                        if (toDemolish.indoors.Value is Cabin && ((Cabin)toDemolish.indoors.Value).farmhand.Value.isActive())
                        {
                            response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_FarmhandOnline");
                        }
                        else
                        {
                            toDemolish.BeforeDemolish();
                            Chest? chest = null;
                            if (toDemolish.indoors.Value is Cabin)
                            {
                                List<Item> list = ((Cabin)toDemolish.indoors.Value).demolish();
                                if (list.Count > 0)
                                {
                                    chest = new Chest(playerChest: true);
                                    chest.fixLidFrame();
                                    chest.items.Set(list);
                                }
                            }
                            if (farm.destroyStructure(toDemolish))
                            {
                                _ = (int)toDemolish.tileY.Value;
                                _ = (int)toDemolish.tilesHigh.Value;
                                Game1.flashAlpha = 1f;
                                toDemolish.showDestroyedAnimation(Game1.getFarm());
                                Game1.playSound("explosion");
                                Utility.spreadAnimalsAround(toDemolish, farm);
                                if (CarpenterMenuPatch.carpenterMenu != null)
                                    DelayedAction.functionAfterDelay(CarpenterMenuPatch.carpenterMenu.returnToCarpentryMenu, 1500);
                                // freeze = true;
                                if (chest != null)
                                {
                                    farm.objects[new Vector2((int)toDemolish.tileX.Value + (int)toDemolish.tilesWide.Value / 2, (int)toDemolish.tileY.Value + (int)toDemolish.tilesHigh.Value / 2)] = chest;
                                }
                            }
                        }
                    }
                }
            };
            if (toDemolish != null)
            {
                if (toDemolish.indoors.Value != null && toDemolish.indoors.Value is Cabin && !Game1.IsMasterGame)
                {
                    response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed");
                    toDemolish = null;
                    return response;
                }
                if (CarpenterMenuPatch.carpenterMenu != null && !CarpenterMenuPatch.carpenterMenu.CanDemolishThis(toDemolish))
                {
                    toDemolish = null;
                    return response;
                }
                if (CarpenterMenuPatch.carpenterMenu != null && !Game1.IsMasterGame && !CarpenterMenuPatch.carpenterMenu.hasPermissionsToDemolish(toDemolish))
                {
                    toDemolish = null;
                    return response;
                }
            }
            if (toDemolish != null && toDemolish.indoors.Value is Cabin)
            {
                Cabin cabin = (Cabin)toDemolish.indoors.Value;
                if (cabin.farmhand.Value != null && (bool)cabin.farmhand.Value.isCustomized.Value)
                {
                    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\UI:Carpenter_DemolishCabinConfirm", cabin.farmhand.Value.Name), Game1.currentLocation.createYesNoResponses(), delegate (Farmer f, string answer)
                    {
                        if (answer == "Yes")
                        {
                            Game1.activeClickableMenu = CarpenterMenuPatch.carpenterMenu;
                            Game1.player.team.demolishLock.RequestLock(continueDemolish, buildingLockFailed);
                        }
                        else
                        {
                            if (CarpenterMenuPatch.carpenterMenu != null)
                                DelayedAction.functionAfterDelay(CarpenterMenuPatch.carpenterMenu.returnToCarpentryMenu, 1000);
                        }
                    });
                    return response;
                }
            }
            if (toDemolish != null)
            {
                Game1.player.team.demolishLock.RequestLock(continueDemolish, buildingLockFailed);
            }

            return response;
        }

        public static string? Contstruct(Vector2 position)
        {
            string? response = null;
            // This code is taken from the game's code (CarpenterMenu.cs::874)
            Game1.player.team.buildLock.RequestLock(delegate
            {
                if (CarpenterMenuPatch.isOnFarm && Game1.locationRequest == null)
                {
                    if (tryToBuild(position))
                    {
                        if (CarpenterMenuPatch.carpenterMenu != null)
                        {
                            CarpenterMenuPatch.carpenterMenu.CurrentBlueprint.consumeResources();
                            DelayedAction.functionAfterDelay(CarpenterMenuPatch.carpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild, 2000);
                        }
                        // freeze = true;
                    }
                    else
                    {
                        response = Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild");
                    }
                }
                Game1.player.team.buildLock.ReleaseLock();
            });

            return response;
        }

        public static bool tryToBuild(Vector2 position)
        {
            if (CarpenterMenuPatch.carpenterMenu == null)
                return false;
            return ((Farm)Game1.getLocationFromName("Farm")).buildStructure(CarpenterMenuPatch.carpenterMenu.CurrentBlueprint, position, Game1.player, CarpenterMenuPatch.isMagicalConstruction);
        }

        public static string? Upgrade(Building? toUpgrade)
        {
            string? response = null;
            // This code is taken from the game's code (CarpenterMenu.cs::775)
            if (CarpenterMenuPatch.carpenterMenu != null && toUpgrade != null && CarpenterMenuPatch.carpenterMenu.CurrentBlueprint.name != null && toUpgrade.buildingType.Equals(CarpenterMenuPatch.carpenterMenu.CurrentBlueprint.nameOfBuildingToUpgrade))
            {
                CarpenterMenuPatch.carpenterMenu.CurrentBlueprint.consumeResources();
                toUpgrade.daysUntilUpgrade.Value = 2;
                toUpgrade.showUpgradeAnimation(Game1.getFarm());
                Game1.playSound("axe");
                DelayedAction.functionAfterDelay(CarpenterMenuPatch.carpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild, 1500);
                // freeze = true;
                // Game1.multiplayer.globalChatInfoMessage("BuildingBuild", Game1.player.Name, Utility.AOrAn(carpenterMenu.CurrentBlueprint.displayName), carpenterMenu.CurrentBlueprint.displayName, Game1.player.farmName.Value);
            }
            else if (toUpgrade != null)
            {
                response = Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType");
            }
            return response;
        }

        public static string? Paint(Building? toPaint)
        {
            string? response = null;
            // This code is taken from the game's code (CarpenterMenu.cs::793)
            Farm farm_location = Game1.getFarm();
            if (toPaint != null)
            {
                if (!toPaint.CanBePainted())
                {
                    response = Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint");
                    return response;
                }
                if (CarpenterMenuPatch.carpenterMenu != null && !CarpenterMenuPatch.carpenterMenu.HasPermissionsToPaint(toPaint))
                {
                    response = Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint_Permission");
                    return response;
                }
                toPaint.color.Value = Color.White;

                if (CarpenterMenuPatch.carpenterMenu != null)
                    CarpenterMenuPatch.carpenterMenu.SetChildMenu(new StardewValley.Menus.BuildingPaintMenu(toPaint));
            }
            /* TODO Add painting of farm house
            else if (farm_location.GetHouseRect().Contains(Utility.Vector2ToPoint(new Vector2(toPaint.tileX, toPaint.tileY))))
            {
                if (!carpenterMenu.CanPaintHouse())
                {
                    response = Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint");
                }
                else if (!carpenterMenu.HasPermissionsToPaint(null))
                {
                    response = Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint_Permission");
                }
                else
                {
                    carpenterMenu.SetChildMenu(new BuildingPaintMenu("House", () => (farm_location.paintedHouseTexture != null) ? farm_location.paintedHouseTexture : Farm.houseTextures, farm_location.houseSource.Value, farm_location.housePaintColor.Value));
                }
            }*/
            return response;
        }

        public static string? Move(Building? buildingToMove, Vector2 position)
        {
            // This code is taken from the game's code (CarpenterMenu.cs::829)
            if (buildingToMove == null) return null;
            
            string? name = buildingToMove.nameOfIndoorsWithoutUnique;
            name = (name == "null") ? buildingToMove.buildingType.Value : name;

            if ((int)buildingToMove.daysOfConstructionLeft.Value > 0)
            {
                buildingToMove = null;
                return Translator.Instance.Translate("building_operations-move_building-under_construction");
            }
            if (CarpenterMenuPatch.carpenterMenu != null && !CarpenterMenuPatch.carpenterMenu.hasPermissionsToMove(buildingToMove))
            {
                buildingToMove = null;
                return Translator.Instance.Translate("building_operations-move_building-no_permission");
            }
            Game1.playSound("axchop");

            if (buildingToMove != null && ((Farm)Game1.getLocationFromName("Farm")).buildStructure(buildingToMove, position, Game1.player))
            {
                if (buildingToMove is ShippingBin)
                {
                    ((ShippingBin)buildingToMove).initLid();
                }
                if (buildingToMove is GreenhouseBuilding)
                {
                    Game1.getFarm().greenhouseMoved.Value = true;
                }
                buildingToMove.performActionOnBuildingPlacement();
                buildingToMove = null;
                Game1.playSound("axchop");
                DelayedAction.playSoundAfterDelay("dirtyHit", 50);
                DelayedAction.playSoundAfterDelay("dirtyHit", 150);

                return Translator.Instance.Translate("building_operations-move_building-building_moved",
                        new { building_name = name, x_position = position.X, y_position = position.Y });
            }
            else
            {
                Game1.playSound("cancel");
                return Translator.Instance.Translate("building_operations-move_building-cannot_move",
                        new { x_position = position.X, y_position = position.Y });
            }
        }

        public static void PurchaseAnimal(Building? selection)
        {
            if (selection == null)
                return;

            if (PurchaseAnimalsMenuPatch.purchaseAnimalsMenu == null)
                return;

            int x = (selection.tileX.Value * Game1.tileSize) - Game1.viewport.X;
            int y = (selection.tileY.Value * Game1.tileSize) - Game1.viewport.Y;

            if (PurchaseAnimalsMenuPatch.animalBeingPurchased != null && !selection.buildingType.Value.Contains(PurchaseAnimalsMenuPatch.animalBeingPurchased.buildingTypeILiveIn.Value))
            {
                string warn = Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11326", PurchaseAnimalsMenuPatch.animalBeingPurchased.displayType);
                MainClass.ScreenReader.Say(warn, true);
                return;
            }

            if (((AnimalHouse)selection.indoors.Value).isFull())
            {
                string warn = Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11321");
                MainClass.ScreenReader.Say(warn, true);
                return;
            }

            PurchaseAnimalsMenuPatch.purchaseAnimalsMenu.receiveLeftClick(x, y);
        }

        public static void MoveAnimal(Building? selection)
        {
            if (selection == null)
                return;

            if (AnimalQueryMenuPatch.animalQueryMenu == null)
                return;

            if (AnimalQueryMenuPatch.animalBeingMoved == null)
                return;

            // The following code is taken from the game's source code [AnimalQueryMenu.cs::receiveLeftClick]
            if (selection.buildingType.Value.Contains(AnimalQueryMenuPatch.animalBeingMoved.buildingTypeILiveIn.Value))
            {
                if (((AnimalHouse)selection.indoors.Value).isFull())
                {
                    string warn = Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_BuildingFull");
                    MainClass.ScreenReader.Say(warn, true);
                    return;
                }
                if (selection.Equals(AnimalQueryMenuPatch.animalBeingMoved.home))
                {
                    string warn = Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_AlreadyHome");
                    MainClass.ScreenReader.Say(warn, true);
                    return;
                }
                ((AnimalHouse)AnimalQueryMenuPatch.animalBeingMoved.home.indoors.Value).animalsThatLiveHere.Remove(AnimalQueryMenuPatch.animalBeingMoved.myID.Value);
                if (((AnimalHouse)AnimalQueryMenuPatch.animalBeingMoved.home.indoors.Value).animals.ContainsKey(AnimalQueryMenuPatch.animalBeingMoved.myID.Value))
                {
                    ((AnimalHouse)selection.indoors.Value).animals.Add(AnimalQueryMenuPatch.animalBeingMoved.myID.Value, AnimalQueryMenuPatch.animalBeingMoved);
                    ((AnimalHouse)AnimalQueryMenuPatch.animalBeingMoved.home.indoors.Value).animals.Remove(AnimalQueryMenuPatch.animalBeingMoved.myID.Value);
                }
                AnimalQueryMenuPatch.animalBeingMoved.home = selection;
                AnimalQueryMenuPatch.animalBeingMoved.homeLocation.Value = new Vector2((int)selection.tileX.Value, (int)selection.tileY.Value);
                ((AnimalHouse)selection.indoors.Value).animalsThatLiveHere.Add(AnimalQueryMenuPatch.animalBeingMoved.myID.Value);
                AnimalQueryMenuPatch.animalBeingMoved.makeSound();
                Game1.globalFadeToBlack(AnimalQueryMenuPatch.animalQueryMenu.finishedPlacingAnimal);
            }
            else
            {
                string warn = Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_CantLiveThere");
                MainClass.ScreenReader.Say(warn, true);
            }
            return;
        }
    }
}
