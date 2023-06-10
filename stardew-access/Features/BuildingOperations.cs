using Microsoft.Xna.Framework;
using stardew_access.Patches;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;

namespace stardew_access.Utils
{
    internal class BuildingOperations
    {
        internal static Building?[] availableBuildings = new Building[100];
        internal static Vector2[] marked = new Vector2[10];

        private static string? CheckDemolishConditions(Building? toDemolish)
        {
            if (toDemolish == null)
                return "No building to demolish.";

            if (toDemolish.daysOfConstructionLeft.Value > 0 || toDemolish.daysUntilUpgrade.Value > 0)
            {
                return Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction");
            }

            if (toDemolish.indoors.Value is AnimalHouse animalHouse && animalHouse.animalsThatLiveHere.Count > 0)
            {
                return Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere");
            }

            if (toDemolish.indoors.Value?.farmers.Any() == true)
            {
                return Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere");
            }

            if (toDemolish.indoors.Value is Cabin cabin)
            {
                if (Game1.getAllFarmers().Any(farmer => farmer.currentLocation?.Name == cabin.GetCellarName()))
                {
                    return Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere");
                }

                if (cabin.farmhand.Value.isActive())
                {
                    return Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_FarmhandOnline");
                }
            }

            return null;
        }

        private static void HandleDemolition(Building toDemolish)
        {
            Farm farm = (Farm)Game1.getLocationFromName("Farm");
            toDemolish.BeforeDemolish();
            Chest? chest = null;
            if (toDemolish.indoors.Value is Cabin cabin)
            {
                List<Item> list = cabin.demolish();
                if (list.Count > 0)
                {
                    chest = new Chest(playerChest: true);
                    chest.fixLidFrame();
                    chest.items.Set(list);
                }
            }
            if (farm.destroyStructure(toDemolish))
            {
                Game1.flashAlpha = 1f;
                toDemolish.showDestroyedAnimation(Game1.getFarm());
                Game1.playSound("explosion");
                Utility.spreadAnimalsAround(toDemolish, farm);
                if (CarpenterMenuPatch.carpenterMenu != null)
                    DelayedAction.functionAfterDelay(CarpenterMenuPatch.carpenterMenu.returnToCarpentryMenu, 1500);
                if (chest != null)
                {
                    farm.objects[new Vector2((int)toDemolish.tileX.Value + (int)toDemolish.tilesWide.Value / 2, (int)toDemolish.tileY.Value + (int)toDemolish.tilesHigh.Value / 2)] = chest;
                }
            }
        }

        private static void HandleFailedDemolition()
        {
            if (CarpenterMenuPatch.carpenterMenu != null)
                DelayedAction.functionAfterDelay(CarpenterMenuPatch.carpenterMenu.returnToCarpentryMenu, 1000);
            MainClass.ScreenReader.Say("Building failed", true);
        }

        public static string? Demolish(Building? toDemolish)
        {
            if (toDemolish == null)
            {
                return null;
            }

            string? demolishError = CheckDemolishConditions(toDemolish);
            if (demolishError != null)
            {
                return demolishError;
            }

            Game1.player.team.demolishLock.RequestLock(() => HandleDemolition(toDemolish), HandleFailedDemolition);

            return null;
        }

        public static string? Construct(Vector2 position)
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

                CarpenterMenuPatch.carpenterMenu?.SetChildMenu(new StardewValley.Menus.BuildingPaintMenu(toPaint));
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
            if (buildingToMove == null)
            {
                return null;
            }

            string name = buildingToMove.nameOfIndoorsWithoutUnique == "null" ? buildingToMove.buildingType.Value : buildingToMove.nameOfIndoorsWithoutUnique;

            if (buildingToMove.daysOfConstructionLeft.Value > 0)
            {
                return "Building under construction, cannot move";
            }
            if (CarpenterMenuPatch.carpenterMenu != null && !CarpenterMenuPatch.carpenterMenu.hasPermissionsToMove(buildingToMove))
            {
                return "You don't have permission to move this building";
            }
            Game1.playSound("axchop");

            if (!((Farm)Game1.getLocationFromName("Farm")).buildStructure(buildingToMove, position, Game1.player))
            {
                Game1.playSound("cancel");
                return $"Cannot move building to {position.X}x {position.Y}y";
            }

            switch (buildingToMove)
            {
                case ShippingBin shippingBin:
                    shippingBin.initLid();
                    break;
                case GreenhouseBuilding:
                    Game1.getFarm().greenhouseMoved.Value = true;
                    break;
            }

            buildingToMove.performActionOnBuildingPlacement();
            Game1.playSound("axchop");
            DelayedAction.playSoundAfterDelay("dirtyHit", 50);
            DelayedAction.playSoundAfterDelay("dirtyHit", 150);

            return $"{name} moved to {position.X}x {position.Y}y";
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
