using Microsoft.Xna.Framework;
using stardew_access.Patches;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;

namespace stardew_access.Utils;

internal class BuildingOperations
{
    internal static Building?[] availableBuildings = new Building[100];
    internal static Vector2[] marked = new Vector2[10];

    public static string? Demolish(Building? toDemolish)
    {
        if (toDemolish == null)
        {
            return null;
        }

        var carpenterMenu = CarpenterMenuPatch.carpenterMenu;
        if (carpenterMenu == null) return null;

        string? response = null;

        GameLocation farm = carpenterMenu.TargetLocation;
        Building? destroyed = toDemolish;
        GameLocation interior = destroyed.GetIndoors();
        Cabin? cabin = interior as Cabin;
        void buildingLockFailed()
        {
            if (CarpenterMenuPatch.isDemolishing)
            {
                response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed");
            }
        }
        void continueDemolish()
        {
            if (!CarpenterMenuPatch.isDemolishing || destroyed == null || !farm.buildings.Contains(destroyed))
            {
                response = "building_operations-no_building_to_demolish";
                return;
            }

            if ((int)destroyed.daysOfConstructionLeft.Value > 0 || (int)destroyed.daysUntilUpgrade.Value > 0)
            {
                response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction");
                return;
            }
            else if (interior is AnimalHouse animalHouse && animalHouse.animalsThatLiveHere.Count > 0)
            {
                response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere");
                return;
            }
            else if (interior != null && interior.farmers.Any())
            {
                response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere");
                return;
            }
            else
            {
                if (cabin != null)
                {
                    foreach (Farmer allFarmer in Game1.getAllFarmers())
                    {
                        if (allFarmer.currentLocation != null && allFarmer.currentLocation.Name == cabin.GetCellarName())
                        {
                            response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere");
                            return;
                        }
                    }
                    if (cabin.IsOwnerActivated)
                    {
                        response = Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_FarmhandOnline");
                        return;
                    }
                }
                destroyed.BeforeDemolish();
                Chest? chest = null;
                if (cabin != null)
                {
                    List<Item> list = cabin.demolish();
                    if (list.Count > 0)
                    {
                        chest = new Chest(playerChest: true);
                        chest.fixLidFrame();
                        chest.Items.OverwriteWith(list);
                    }
                }
                if (farm.destroyStructure(destroyed))
                {
                    Game1.flashAlpha = 1f;
                    destroyed.showDestroyedAnimation(carpenterMenu.TargetLocation);
                    Game1.playSound("explosion");
                    Utility.spreadAnimalsAround(destroyed, farm);
                    DelayedAction.functionAfterDelay(carpenterMenu.returnToCarpentryMenu, 1500);
                    // FIXME Try using ref to change the value
                    // freeze = true;
                    if (chest != null)
                    {
                        farm.objects[new Vector2((int)destroyed.tileX.Value + (int)destroyed.tilesWide.Value / 2, (int)destroyed.tileY.Value + (int)destroyed.tilesHigh.Value / 2)] = chest;
                    }
                }
            }
        }
        if (destroyed != null)
        {
            if (cabin != null && !Game1.IsMasterGame)
            {
                return Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed");
            }
            if (!carpenterMenu.CanDemolishThis(destroyed))
            {
                // TODO add a message
                return null;
            }
            if (!Game1.IsMasterGame && !carpenterMenu.hasPermissionsToDemolish(destroyed))
            {
                // TODO add a message
                return null;
            }
        }
        Cabin? cabin2 = cabin;
        if (cabin2 != null && cabin2.HasOwner && cabin!.owner.isCustomized.Value)
        {
            Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\UI:Carpenter_DemolishCabinConfirm", cabin.owner.Name), Game1.currentLocation.createYesNoResponses(), delegate (Farmer f, string answer)
            {
                if (answer == "Yes")
                {
                    Game1.activeClickableMenu = carpenterMenu;
                    Game1.player.team.demolishLock.RequestLock(continueDemolish, buildingLockFailed);
                }
                else
                {
                    DelayedAction.functionAfterDelay(carpenterMenu.returnToCarpentryMenu, 500);
                }
            });
        }
        else if (destroyed != null)
        {
            Game1.player.team.demolishLock.RequestLock(continueDemolish, buildingLockFailed);
        }

        return response;
    }

    public static string? Construct(Vector2 position)
    {
        var carpenterMenu = CarpenterMenuPatch.carpenterMenu;
        if (carpenterMenu == null) return null;

        string? response = null;
        // This code is taken from the game's code (CarpenterMenu.cs::receiveleftCick)
        Game1.player.team.buildLock.RequestLock(delegate
        {
            if (carpenterMenu.onFarm && Game1.locationRequest == null)
            {
                if (carpenterMenu.tryToBuild())
                {
                    carpenterMenu.ConsumeResources();
                    DelayedAction.functionAfterDelay(carpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild, 2000);
                    // TODO try this with ref
                    carpenterMenu.freeze = true;
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

    public static string? Upgrade(Building? toUpgrade)
    {
        string? response = null;
        var carpenterMenu = CarpenterMenuPatch.carpenterMenu;
        // This code is taken from the game's code (CarpenterMenu.cs::receiveLeftClick)
        if (carpenterMenu != null && toUpgrade != null && toUpgrade.buildingType.Value == carpenterMenu.Blueprint.UpgradeFrom)
        {
            carpenterMenu.ConsumeResources();
		toUpgrade.upgradeName.Value = carpenterMenu.Blueprint.Id;
            toUpgrade.daysUntilUpgrade.Value = Math.Max(carpenterMenu.Blueprint.BuildDays, 1);
            toUpgrade.showUpgradeAnimation(carpenterMenu.TargetLocation);
            Game1.playSound("axe");
            DelayedAction.functionAfterDelay(carpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild, 1500);
            // TODO Try to add these
            // freeze = true;
		// Game1.multiplayer.globalChatInfoMessage("BuildingBuild", Game1.player.Name, "aOrAn:" + Blueprint.TokenizedDisplayName, Blueprint.TokenizedDisplayName, Game1.player.farmName);
		if (carpenterMenu.Blueprint.BuildDays < 1)
		{
			toUpgrade.FinishConstruction();
		}
		else
		{
			Game1.netWorldState.Value.MarkUnderConstruction(carpenterMenu.Builder, toUpgrade);
		}
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
        /* TODO Add painting of farm house
        // This code is taken from the game's code (CarpenterMenu.cs::793)
        Farm farm_location = Game1.getFarm();
        if (toPaint != null)
        {
            if (!toPaint.CanBePainted())
            {
                response = Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint");
                return response;
            }
            var carpenterMenu = CarpenterMenuPatch.carpenterMenu;
            if (carpenterMenu != null && !carpenterMenu.HasPermissionsToPaint(toPaint))
            {
                response = Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint_Permission");
                return response;
            }
            toPaint.color.Value = Color.White;

            carpenterMenu?.SetChildMenu(new StardewValley.Menus.BuildingPaintMenu(toPaint));
        }
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
        //TODO Update this!
        if (buildingToMove == null) return null;

        var carpenterMenu = CarpenterMenuPatch.carpenterMenu;
        if (carpenterMenu != null && !carpenterMenu.hasPermissionsToMove(buildingToMove))
        {
            buildingToMove = null;
            return Translator.Instance.Translate("building_operations-move_building-no_permission");
        }
        Game1.playSound("axchop");
        string name = buildingToMove.GetIndoorsName() == "null" ? buildingToMove.buildingType.Value : buildingToMove.GetIndoorsName();

        if (buildingToMove.daysOfConstructionLeft.Value > 0)
        {
            return Translator.Instance.Translate("building_operations-move_building-under_construction");
        }
        if (carpenterMenu != null && !carpenterMenu.hasPermissionsToMove(buildingToMove))
        {
            return Translator.Instance.Translate("building_operations-move_building-no_permission");
        }
        Game1.playSound("axchop");

        if (!((Farm)Game1.getLocationFromName("Farm")).buildStructure(buildingToMove, position, Game1.player))
        {
            Game1.playSound("cancel");
            return Translator.Instance.Translate("building_operations-move_building-cannot_move", new
            {
                x_position = position.X,
                y_position = position.Y
            });
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

        return Translator.Instance.Translate("building_operations-move_building-building_moved", new
        {
            building_name = name,
            x_position = position.X,
            y_position = position.Y
        });
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
        if (selection == null) return;
        if (AnimalQueryMenuPatch.animalQueryMenu == null) return;
        if (AnimalQueryMenuPatch.animalBeingMoved == null) return;

        // The following code is taken from the game's source code [AnimalQueryMenu.cs::receiveLeftClick]
        if (AnimalQueryMenuPatch.animalBeingMoved.CanLiveIn(selection))
        {
	    AnimalHouse animalHouse = (AnimalHouse)selection.GetIndoors();
	    if (animalHouse.isFull())
            {
                MainClass.ScreenReader.Say(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_BuildingFull"), true);
                return;
            }
            if (selection.Equals(AnimalQueryMenuPatch.animalBeingMoved.home))
            {
                MainClass.ScreenReader.Say(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_AlreadyHome"), true);
                return;
            }
	    AnimalHouse obj = (AnimalHouse)AnimalQueryMenuPatch.animalBeingMoved.home.GetIndoors();
	    obj.animalsThatLiveHere.Remove(AnimalQueryMenuPatch.animalBeingMoved.myID.Value);
	    if (obj.animals.Remove(AnimalQueryMenuPatch.animalBeingMoved.myID.Value))
	    {
	    	animalHouse.adoptAnimal(AnimalQueryMenuPatch.animalBeingMoved);
	    }
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
