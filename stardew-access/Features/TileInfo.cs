using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace stardew_access.Features
{
    public class TileInfo
    {
        public static string[] trackable_machines = { "bee house", "cask", "press", "keg", "machine", "maker", "preserves jar", "bone mill", "kiln", "crystalarium", "furnace", "geode crusher", "tapper", "lightning rod", "incubator", "wood chipper", "worm bin", "loom" };

        ///<summary>Returns the name of the object at tile alongwith it's category's name</summary>
        public static (string? name, string? categoryName) getNameWithCategoryNameAtTile(Vector2 tile)
        {
            (string? name, CATEGORY? category) tileDetail = getNameWithCategoryAtTile(tile);

            if (tileDetail.category == null)
                tileDetail.category = CATEGORY.Others;

            return (tileDetail.name, tileDetail.category.ToString());
        }

        ///<summary>Returns the name of the object at tile</summary>
        public static string? getNameAtTile(Vector2 tile)
        {
            return getNameWithCategoryAtTile(tile).name;
        }

        ///<summary>Returns the name of the object at tile alongwith it's category</summary>
        public static (string? name, CATEGORY? category) getNameWithCategoryAtTile(Vector2 tile)
        {
            int x = (int)tile.X;
            int y = (int)tile.Y;
            string? toReturn = "";
            CATEGORY? category = CATEGORY.Others;

            bool isColliding = isCollidingAtTile(x, y);
            Dictionary<Vector2, Netcode.NetRef<TerrainFeature>> terrainFeature = Game1.currentLocation.terrainFeatures.FieldDict;
            string? door = getDoorAtTile(x, y);
            (CATEGORY? category, string? name) dynamicTile = getDynamicTilesInfo(x, y);
            string? junimoBundle = getJunimoBundleAt(x, y);
            string? resourceClump = getResourceClumpAtTile(x, y);
            string? farmAnimal = getFarmAnimalAt(Game1.currentLocation, x, y);
            string? parrot = getParrotPerchAtTile(x, y);
            (string? name, CATEGORY category) staticTile = MainClass.STiles.getStaticTileInfoAtWithCategory(x, y);

            if (Game1.currentLocation.isCharacterAtTile(tile) != null)
            {
                NPC npc = Game1.currentLocation.isCharacterAtTile(tile);
                toReturn = npc.displayName;
                if (npc.isVillager() || npc.CanSocialize)
                    category = CATEGORY.Farmers;
                else
                    category = CATEGORY.NPCs;
            }
            else if (farmAnimal != null)
            {
                toReturn = farmAnimal;
                category = CATEGORY.FarmAnimals;
            }
            else if (staticTile.name != null)
            {
                toReturn = staticTile.name;
                category = staticTile.category;
            }
            else if (dynamicTile.name != null)
            {
                toReturn = dynamicTile.name;
                category = dynamicTile.category;
            }
            else if (Game1.currentLocation is VolcanoDungeon && ((VolcanoDungeon)Game1.currentLocation).IsCooledLava(x, y))
            {
                toReturn = "Cooled lava";
                category = CATEGORY.WaterTiles;
            }
            else if (Game1.currentLocation is VolcanoDungeon && StardewValley.Monsters.LavaLurk.IsLavaTile((VolcanoDungeon)Game1.currentLocation, x, y))
            {
                toReturn = "Lava";
                category = CATEGORY.WaterTiles;
            }
            else if (Game1.currentLocation.isWaterTile(x, y) && isColliding)
            {
                toReturn = "Water";
                category = CATEGORY.WaterTiles;
            }
            else if (Game1.currentLocation.isObjectAtTile(x, y))
            {
                (string? name, CATEGORY? category) obj = getObjectAtTile(x, y);
                toReturn = obj.name;
                category = obj.category;
            }
            else if (terrainFeature.ContainsKey(tile))
            {
                (string? name, CATEGORY category) tf = getTerrainFeatureAtTile(terrainFeature[tile]);
                string? terrain = tf.name;
                if (terrain != null)
                {
                    toReturn = terrain;
                    category = tf.category;
                }

            }
            else if (Game1.currentLocation.getLargeTerrainFeatureAt(x, y) != null)
            {
                toReturn = getBushAtTile(x, y);
                category = CATEGORY.Bush;
            }
            else if (resourceClump != null)
            {
                toReturn = resourceClump;
                category = CATEGORY.ResourceClumps;
            }
            else if (door != null)
            {
                toReturn = door;
                category = CATEGORY.Doors;
            }
            else if (isMineDownLadderAtTile(x, y))
            {
                toReturn = "Ladder";
                category = CATEGORY.Doors;
            }
            else if (isMineUpLadderAtTile(x, y))
            {
                toReturn = "Up Ladder";
                category = CATEGORY.Doors;
            }
            else if (isElevatorAtTile(x, y))
            {
                toReturn = "Elevator";
                category = CATEGORY.Doors;
            }
            else if (parrot != null)
            {
                toReturn = parrot;
                category = CATEGORY.Buildings;
            }
            else if (junimoBundle != null)
            {
                toReturn = junimoBundle;
                category = CATEGORY.JunimoBundle;
            }

            if (toReturn == "")
                return (null, category);

            return (toReturn, category);
        }

        public static string? getBushAtTile(int x, int y)
        {
            string? toReturn = null;
            Bush bush = (Bush)Game1.currentLocation.getLargeTerrainFeatureAt(x, y);
            int size = bush.size.Value;

            #region Check if bush is harvestable or not
            if (!bush.townBush.Value && (int)bush.tileSheetOffset.Value == 1 && bush.inBloom(Game1.GetSeasonForLocation(Game1.currentLocation), Game1.dayOfMonth))
            {
                // Taken from the game's code
                string season = ((int)bush.overrideSeason.Value == -1) ? Game1.GetSeasonForLocation(Game1.currentLocation) : Utility.getSeasonNameFromNumber(bush.overrideSeason.Value);
                int shakeOff = -1;
                if (!(season == "spring"))
                {
                    if (season == "fall")
                    {
                        shakeOff = 410;
                    }
                }
                else
                {
                    shakeOff = 296;
                }
                if ((int)size == 3)
                {
                    shakeOff = 815;
                }
                if ((int)size == 4)
                {
                    shakeOff = 73;
                }
                if (shakeOff == -1)
                {
                    return null;
                }

                toReturn = "Harvestable";
            }
            #endregion

            if (bush.townBush.Value)
                toReturn = $"{toReturn} Town Bush";
            else if (bush.greenhouseBush.Value)
                toReturn = $"{toReturn} Greenhouse Bush";
            else
                toReturn = $"{toReturn} Bush";

            return toReturn;
        }

        public static string? getJunimoBundleAt(int x, int y)
        {
            string? name = null;
            if (Game1.currentLocation is CommunityCenter communityCenter)
            {
                name = (x, y) switch
                {
                    (14, 5) => "Pantry",
                    (14, 23) => "Crafts Room",
                    (40, 10) => "Fish Tank",
                    (63, 14) => "Boiler Room",
                    (55, 6) => "Vault",
                    (46, 11) => "Bulletin Board",
                    _ => null,
                };
                if (name != null && communityCenter.shouldNoteAppearInArea(CommunityCenter.getAreaNumberFromName(name)))
                    return $"{name} bundle";
            }
            else if (Game1.currentLocation is AbandonedJojaMart)
            {
                name = (x, y) switch
                {
                    (8, 8) => "Missing",
                    _ => null,
                };

                if (name != null)
                    return $"{name} bundle";
            }

            return null;
        }

        public static bool isCollidingAtTile(int x, int y)
        {
            Rectangle rect = new Rectangle(x * 64 + 1, y * 64 + 1, 62, 62);

            if (Game1.currentLocation.isCollidingPosition(rect, Game1.viewport, true, 0, glider: false, Game1.player, pathfinding: true))
            {
                return true;
            }

            if (Game1.currentLocation is Woods && getStumpsInWoods(x, y) != null)
                return true;

            return false;
        }

        public static string? getFarmAnimalAt(GameLocation? location, int x, int y, bool onlyName = false)
        {
            if (location == null)
                return null;

            if (location is not Farm && location is not AnimalHouse)
                return null;

            List<FarmAnimal>? farmAnimals = null;

            if (location is Farm)
                farmAnimals = ((Farm)location).getAllFarmAnimals();
            else if (location is AnimalHouse)
                farmAnimals = ((AnimalHouse)location).animals.Values.ToList();

            if (farmAnimals == null || farmAnimals.Count <= 0)
                return null;

            for (int i = 0; i < farmAnimals.Count; i++)
            {
                int fx = farmAnimals[i].getTileX();
                int fy = farmAnimals[i].getTileY();

                if (fx.Equals(x) && fy.Equals(y))
                {
                    string name = farmAnimals[i].displayName;
                    int age = farmAnimals[i].age.Value;
                    string type = farmAnimals[i].displayType;

                    if (onlyName)
                        return name;

                    return $"{name}, {type}, age {age}";
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>category: This is the category of the tile. Default to Furnitures.
        /// <br/>name: This is the name of the tile. Default to null if the tile tile has nothing on it.</returns>
        public static (CATEGORY? category, string? name) getDynamicTilesInfo(int x, int y)
        {
            if (Game1.currentLocation is Farm farm)
            {
                Building building = farm.getBuildingAt(new Vector2(x, y));
                if (building != null)
                    return (CATEGORY.Buildings, building.buildingType.Value);
            }
            else if (Game1.currentLocation is Town)
            {
                if (SpecialOrder.IsSpecialOrdersBoardUnlocked() && x == 62 && y == 93)
                    return (CATEGORY.Interactables, "Special quest board");
            }
            else if (Game1.currentLocation is FarmHouse farmHouse)
            {
                if (farmHouse.upgradeLevel >= 1)
                    if (farmHouse.getKitchenStandingSpot().X == x && (farmHouse.getKitchenStandingSpot().Y - 1) == y) // Standing spot is where the player will stand
                        return (CATEGORY.Interactables, "Kitchen");
                    else if (farmHouse.fridgePosition.X == x && farmHouse.fridgePosition.Y == y)
                        return (CATEGORY.Interactables, "Fridge");
            }
            else if (Game1.currentLocation is IslandFarmHouse islandFarmHouse)
            {
                if ((islandFarmHouse.fridgePosition.X - 1) == x && islandFarmHouse.fridgePosition.Y == y)
                    return (CATEGORY.Interactables, "Kitchen");
                else if (islandFarmHouse.fridgePosition.X == x && islandFarmHouse.fridgePosition.Y == y)
                    return (CATEGORY.Interactables, "Fridge");
            }
            else if (Game1.currentLocation is Forest forest)
            {
                if (forest.travelingMerchantDay && x == 27 && y == 11)
                    return (CATEGORY.Interactables, "Travelling Merchant");
            }
            else if (Game1.currentLocation is Beach beach)
            {
                if (x == 58 && y == 13)
                {
                    if (!beach.bridgeFixed.Value)
                        return (CATEGORY.Interactables, "Repair Bridge");
                    else
                        return (CATEGORY.Bridges, "Bridge");
                }
            }
            else if (Game1.currentLocation is CommunityCenter communityCenter)
            {
                if (communityCenter.missedRewardsChestVisible.Value && x == 22 && y == 10)
                    return (CATEGORY.Chests, "Missed Rewards Chest");
            }
            else if (Game1.currentLocation is BoatTunnel)
            {
                if (x == 4 && y == 9)
                    return (CATEGORY.Interactables, ((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatFixed")) ? "Repair " : "") + "Ticket Machine");
                else if (x == 6 && y == 8)
                    return (((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull")) ? CATEGORY.Interactables : CATEGORY.Decor), ((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull")) ? "Repair " : "") + "Boat Hull");
                else if (x == 8 && y == 9)
                    return (((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor")) ? CATEGORY.Interactables : CATEGORY.Decor), ((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor")) ? "Repair " : "") + "Boat Anchor");
            }

            return (null, null);
        }

        public static (string? name, CATEGORY category) getTerrainFeatureAtTile(Netcode.NetRef<TerrainFeature> terrain)
        {
            string? toReturn = null;
            CATEGORY category = CATEGORY.Others;

            if (terrain.Get() is HoeDirt)
            {
                category = CATEGORY.Crops;
                HoeDirt dirt = (HoeDirt)terrain.Get();
                if (dirt.crop != null && !dirt.crop.forageCrop.Value)
                {
                    string cropName = Game1.objectInformation[dirt.crop.indexOfHarvest.Value].Split('/')[0];
                    toReturn = $"{cropName}";

                    bool isWatered = dirt.state.Value == HoeDirt.watered;
                    bool isHarvestable = dirt.readyForHarvest();
                    bool isFertilized = dirt.fertilizer.Value != HoeDirt.noFertilizer;

                    if (isWatered)
                        toReturn = "Watered " + toReturn;

                    if (isFertilized)
                        toReturn = "Fertilized " + toReturn;

                    if (isHarvestable)
                        toReturn = "Harvestable " + toReturn;
                }
                else if (dirt.crop != null && dirt.crop.forageCrop.Value)
                {
                    toReturn = dirt.crop.whichForageCrop.Value switch
                    {
                        1 => "Spring onion",
                        2 => "Ginger",
                        _ => "Forageable crop"
                    };
                }
                else
                {
                    toReturn = "Soil";
                    bool isWatered = dirt.state.Value == HoeDirt.watered;
                    bool isFertilized = dirt.fertilizer.Value != HoeDirt.noFertilizer;

                    if (isWatered)
                        toReturn = "Watered " + toReturn;

                    if (isFertilized)
                        toReturn = "Fertilized " + toReturn;
                }
            }
            else if (terrain.Get() is GiantCrop)
            {
                category = CATEGORY.Crops;
                int whichCrop = ((GiantCrop)terrain.Get()).which.Value;
                switch (whichCrop)
                {
                    case 0:
                        toReturn = "Cauliflower";
                        break;
                    case 1:
                        toReturn = "Melon";
                        break;
                    case 2:
                        toReturn = "Pumpkin";
                        break;
                }
            }
            else if (terrain.Get() is CosmeticPlant)
            {
                category = CATEGORY.Furnitures;
                CosmeticPlant cosmeticPlant = (CosmeticPlant)terrain.Get();
                toReturn = cosmeticPlant.textureName().ToLower();

                if (toReturn.Contains("terrain"))
                    toReturn.Replace("terrain", "");

                if (toReturn.Contains("feature"))
                    toReturn.Replace("feature", "");
            }
            else if (terrain.Get() is Flooring && MainClass.Config.ReadFlooring)
            {
                category = CATEGORY.Flooring;
                Flooring flooring = (Flooring)terrain.Get();
                bool isPathway = flooring.isPathway.Get();
                bool isSteppingStone = flooring.isSteppingStone.Get();

                toReturn = "Flooring";

                if (isPathway)
                    toReturn = "Pathway";

                if (isSteppingStone)
                    toReturn = "Stepping Stone";
            }
            else if (terrain.Get() is FruitTree)
            {
                category = CATEGORY.Trees;
                toReturn = getFruitTree((FruitTree)terrain.Get());
            }
            else if (terrain.Get() is Grass)
            {
                category = CATEGORY.Debris;
                toReturn = "Grass";
            }
            else if (terrain.Get() is Tree)
            {
                category = CATEGORY.Trees;
                toReturn = getTree((Tree)terrain.Get());
            }
            else if (terrain.Get() is Quartz)
            {
                category = CATEGORY.MineItems;
                toReturn = "Quartz";
            }

            return (toReturn, category);
        }

        public static string getFruitTree(FruitTree fruitTree)
        {
            int stage = fruitTree.growthStage.Value;
            int fruitIndex = fruitTree.indexOfFruit.Get();

            string toReturn = Game1.objectInformation[fruitIndex].Split('/')[0];

            if (stage == 0)
                toReturn = $"{toReturn} seed";
            else if (stage == 1)
                toReturn = $"{toReturn} sprout";
            else if (stage == 2)
                toReturn = $"{toReturn} sapling";
            else if (stage == 3)
                toReturn = $"{toReturn} bush";
            else if (stage >= 4)
                toReturn = $"{toReturn} tree";

            if (fruitTree.fruitsOnTree.Value > 0)
                toReturn = $"Harvestable {toReturn}";

            return toReturn;
        }

        public static string getTree(Tree tree)
        {
            int treeType = tree.treeType.Value;
            int treeStage = tree.growthStage.Value;
            string treeName = "tree";
            string seedName = "";

            // Return with the name if it's one of the 3 special trees
            switch (treeType)
            {
                case 4:
                case 5:
                    return "Winter Tree";
                case 6:
                    return "Palm Tree";
                case 7:
                    return "Mushroom Tree";
            }


            if (treeType <= 3)
                seedName = Game1.objectInformation[308 + treeType].Split('/')[0];
            else if (treeType == 8)
                seedName = Game1.objectInformation[292].Split('/')[0];

            if (treeStage >= 1)
            {
                switch (seedName.ToLower())
                {
                    case "mahogany seed":
                        treeName = "Mahogany";
                        break;
                    case "acorn":
                        treeName = "Oak";
                        break;
                    case "maple seed":
                        treeName = "Maple";
                        break;
                    case "pine cone":
                        treeName = "Pine";
                        break;
                    default:
                        treeName = "Coconut";
                        break;
                }

                if (treeStage == 1)
                    treeName = $"{treeName} sprout";
                else if (treeStage == 2)
                    treeName = $"{treeName} sapling";
                else if (treeStage == 3 || treeStage == 4)
                    treeName = $"{treeName} bush";
                else if (treeStage >= 5)
                    treeName = $"{treeName} tree";

                return treeName;
            }

            return seedName;
        }

        #region Objects
        public static (string? name, CATEGORY category) getObjectAtTile(int x, int y)
        {
            (string? name, CATEGORY category) toReturn = (null, CATEGORY.Others);

            StardewValley.Object obj = Game1.currentLocation.getObjectAtTile(x, y);
            int index = obj.ParentSheetIndex;
            toReturn.name = obj.DisplayName;

            // Get object names based on index
            (string? name, CATEGORY category) correctNameAndCategory = getCorrectNameAndCategoryFromIndex(index);
            if (correctNameAndCategory.name != null)
                toReturn = correctNameAndCategory;

            if (toReturn.name.ToLower().Equals("stone")) // Fix for `Busy stone`
                toReturn.category = CATEGORY.Debris;

            if (obj is Chest)
            {
                Chest chest = (Chest)obj;
                toReturn = (chest.DisplayName, CATEGORY.Chests);
            }
            else if (obj is Furniture)
                toReturn.category = CATEGORY.Furnitures;
            else if (obj.Type == "Crafting" && obj.bigCraftable.Value)
            {

                foreach (string machine in trackable_machines)
                {
                    if (obj.Name.ToLower().Contains(machine))
                    {
                        toReturn.name = obj.DisplayName;
                        toReturn.category = CATEGORY.Machines;
                    }
                }
            }

            if (toReturn.category == CATEGORY.Others) // Fix for `Harvestable table` and `Busy nodes`
            {
                MachineState machineState = GetMachineState(obj);
                if (machineState == MachineState.Ready)
                    toReturn.name = $"Harvestable {toReturn.name}";
                else if (machineState == MachineState.Busy)
                    toReturn.name = $"Busy {toReturn.name}";
            }
            return toReturn;
        }

        private static MachineState GetMachineState(StardewValley.Object machine)
        {
            if (machine is CrabPot crabPot)
                if (crabPot.bait.Value is not null && crabPot.heldObject.Value is null)
                    return MachineState.Busy;
            return GetMachineState(machine.readyForHarvest.Value, machine.MinutesUntilReady, machine.heldObject.Value);
        }

        private static MachineState GetMachineState(bool readyForHarvest, int minutesUntilReady, StardewValley.Object? heldObject)
        {
            if (readyForHarvest || (heldObject is not null && minutesUntilReady <= 0))
                return MachineState.Ready;
            else if (minutesUntilReady > 0)
                return MachineState.Busy;
            else
                return MachineState.Waiting;
        }

        private static (string? name, CATEGORY category) getCorrectNameAndCategoryFromIndex(int index)
        {
            switch (index)
            {
                case 313:
                case 314:
                case 315:
                case 316:
                case 317:
                case 318:
                case 452:
                case 674:
                case 675:
                case 676:
                case 677:
                case 678:
                case 679:
                case 750:
                case 784:
                case 785:
                case 786:
                    return ("Weed", CATEGORY.Debris);
                case 792:
                case 793:
                case 794:
                    return ("Fertile weed", CATEGORY.Debris);
                case 319:
                case 320:
                case 321:
                    return ("Ice crystal", CATEGORY.Debris);
                case 75:
                    return ("Geode", CATEGORY.MineItems);
                case 32:
                case 34:
                case 36:
                case 38:
                case 40:
                case 42:
                case 48:
                case 50:
                case 52:
                case 54:
                case 56:
                case 58:
                    return ("Coloured stone", CATEGORY.Debris);
                case 668:
                case 670:
                case 845:
                case 846:
                case 847:
                    return ("Mine stone", CATEGORY.MineItems);
                case 818:
                    return ("Clay stone", CATEGORY.Debris);
                case 816:
                case 817:
                    return ("Fossil stone", CATEGORY.Debris);
                case 118:
                case 120:
                case 122:
                case 124:
                    return ("Barrel", CATEGORY.MineItems);
                case 119:
                case 121:
                case 123:
                case 125:
                    return ("Item box", CATEGORY.MineItems);
            }

            if (Game1.currentLocation is Mine or MineShaft)
            {
                switch (index)
                {
                    case 76:
                        return ("Frozen geode", CATEGORY.MineItems);
                    case 77:
                        return ("Magma geode", CATEGORY.MineItems);
                    case 8:
                    case 66:
                        return ("Amethyst node", CATEGORY.MineItems);
                    case 14:
                    case 62:
                        return ("Aquamarine node", CATEGORY.MineItems);
                    case 843:
                    case 844:
                        return ("Cinder shard node", CATEGORY.MineItems);
                    case 2:
                    case 72:
                        return ("Diamond node", CATEGORY.MineItems);
                    case 12:
                    case 60:
                        return ("Emerald node", CATEGORY.MineItems);
                    case 44:
                        return ("Gem node", CATEGORY.MineItems);
                    case 6:
                    case 70:
                        return ("Jade node", CATEGORY.MineItems);
                    case 46:
                        return ("Mystic stone", CATEGORY.MineItems);
                    case 74:
                        return ("Prismatic node", CATEGORY.MineItems);
                    case 4:
                    case 64:
                        return ("Ruby node", CATEGORY.MineItems);
                    case 10:
                    case 68:
                        return ("Topaz node", CATEGORY.MineItems);
                    case 819:
                        return ("Omni geode node", CATEGORY.MineItems);
                    case 751:
                    case 849:
                        return ("Copper node", CATEGORY.MineItems);
                    case 764:
                        return ("Gold node", CATEGORY.MineItems);
                    case 765:
                        return ("Iridium node", CATEGORY.MineItems);
                    case 290:
                    case 850:
                        return ("Iron node", CATEGORY.MineItems);
                }
            }

            return (null, CATEGORY.Others);
        }
        #endregion  

        public static bool isMineDownLadderAtTile(int x, int y)
        {
            try
            {
                if (Game1.currentLocation is Mine or MineShaft)
                {
                    int? index = null;

                    if (Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y] != null)
                        index = Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y].TileIndex;

                    if (index == 173 || index == 174)
                        return true;
                }
            }
            catch (Exception) { }

            return false;
        }

        public static bool isMineUpLadderAtTile(int x, int y)
        {
            try
            {
                if (Game1.currentLocation is Mine or MineShaft)
                {
                    int? index = null;

                    if (Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y] != null)
                        index = Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y].TileIndex;

                    if (index == 115)
                        return true;
                }
            }
            catch (Exception) { }

            return false;
        }

        public static bool isElevatorAtTile(int x, int y)
        {
            try
            {
                if (Game1.currentLocation is Mine or MineShaft)
                {
                    int? index = null;

                    if (Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y] != null)
                        index = Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y].TileIndex;

                    if (index == 112)
                        return true;
                }
            }
            catch (Exception) { }

            return false;
        }

        public static string? getDoorAtTile(int x, int y)
        {
            Point tilePoint = new Point(x, y);
            StardewValley.Network.NetPointDictionary<string, Netcode.NetString> doorList = Game1.currentLocation.doors;

            for (int i = 0; i < doorList.Count(); i++)
            {
                if (doorList.ContainsKey(tilePoint))
                {
                    string? doorName;
                    doorList.TryGetValue(tilePoint, out doorName);

                    if (doorName != null)
                        return $"{doorName} door";
                    else
                        return "door";
                }
            }

            return null;
        }

        public static string? getResourceClumpAtTile(int x, int y)
        {
            if (Game1.currentLocation is Woods)
                return getStumpsInWoods(x, y);

            for (int i = 0; i < Game1.currentLocation.resourceClumps.Count; i++)
            {
                if (Game1.currentLocation.resourceClumps[i].occupiesTile(x, y))
                {
                    int index = Game1.currentLocation.resourceClumps[i].parentSheetIndex.Value;

                    switch (index)
                    {
                        case 600:
                            return "Large Stump";
                        case 602:
                            return "Hollow Log";
                        case 622:
                            return "Meteorite";
                        case 752:
                        case 754:
                        case 756:
                        case 758:
                            return "Mine Rock";
                        case 672:
                            return "Boulder";
                        default:
                            return "Unknown";
                    }
                }
            }

            return null;
        }

        public static string? getStumpsInWoods(int x, int y)
        {
            if (Game1.currentLocation is not Woods)
                return null;

            if ((x == 8 || x == 9) && y == 7)
            {
                return "Old Master Cannoli";
            }
            Netcode.NetObjectList<ResourceClump> stumps = ((Woods)Game1.currentLocation).stumps;
            for (int i = 0; i < stumps.Count; i++)
            {
                if (stumps[i].occupiesTile(x, y))
                {
                    return "Large Stump";
                }
            }
            return null;
        }

        public static string? getParrotPerchAtTile(int x, int y)
        {
            if (Game1.currentLocation is not IslandLocation islandLocation)
                return null;

            foreach (var perch in islandLocation.parrotUpgradePerches)
            {
                if (!perch.tilePosition.Value.Equals(new Point(x, y)))
                    continue;

                string toSpeak = $"Parrot required nuts {perch.requiredNuts.Value}";

                if (!perch.IsAvailable())
                    return "Empty parrot perch";
                else if (perch.currentState.Value == StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Idle)
                    return toSpeak;
                else if (perch.currentState.Value == StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.StartBuilding)
                    return "Parrots started building request";
                else if (perch.currentState.Value == StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Building)
                    return "Parrots building request";
                else if (perch.currentState.Value == StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Complete)
                    return $"Request Completed";
                else
                    return toSpeak;
            }

            return null;
        }

    }
}