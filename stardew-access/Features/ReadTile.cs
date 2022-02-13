using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace stardew_access.Features
{
    public class ReadTile
    {
        public static bool isReadingTile = false;
        public static Vector2 prevTile;

        public ReadTile()
        {
            isReadingTile = false;
        }

        public static async void run(bool manuallyTriggered = false)
        {
            isReadingTile = true;

            try
            {
                #region Get Next Grab Tile
                Vector2 tile = CurrentPlayer.getNextTile();
                int x = (int)tile.X;
                int y = (int)tile.Y;
                #endregion

                if (Context.IsPlayerFree)
                {
                    if (!manuallyTriggered && prevTile != tile)
                    {
                        if (MainClass.GetScreenReader() != null)
                            MainClass.GetScreenReader().PrevTextTile = " ";
                    }

                    bool isColliding = isCollidingAtTile(x, y);

                    string? toSpeak = getNameAtTile(tile);

                    #region Narrate toSpeak
                    if (toSpeak != null)
                        if (MainClass.GetScreenReader() != null)
                            if (manuallyTriggered)
                                MainClass.GetScreenReader().Say(toSpeak, true);
                            else
                                MainClass.GetScreenReader().SayWithTileQuery(toSpeak, x, y, true);
                    #endregion

                    #region Play colliding sound effect
                    if (isColliding && prevTile != tile)
                    {
                        Game1.playSound("colliding");
                    }
                    #endregion

                    prevTile = tile;
                }

            }
            catch (Exception e)
            {
                MainClass.GetMonitor().Log($"Error in Read Tile:\n{e.Message}\n{e.StackTrace}", LogLevel.Debug);
            }

            await Task.Delay(100);
            isReadingTile = false;
        }

        ///<summary>Returns the name of the object at tile alongwith it's category's name</summary>
        public static (string?, string?) getNameWithCategoryNameAtTile(Vector2 tile)
        {
            (string?, CATEGORY?) tileDetail = getNameWithCategoryAtTile(tile);

            if (tileDetail.Item2 == null)
                tileDetail.Item2 = CATEGORY.Others;

            return (tileDetail.Item1, tileDetail.Item2.ToString());
        }

        ///<summary>Returns the name of the object at tile alongwith it's category</summary>
        public static (string?, CATEGORY?) getNameWithCategoryAtTile(Vector2 tile)
        {
            int x = (int)tile.X;
            int y = (int)tile.Y;
            string? toReturn = "";
            CATEGORY? category = CATEGORY.Others;

            bool isColliding = isCollidingAtTile(x, y);
            Dictionary<Vector2, Netcode.NetRef<TerrainFeature>> terrainFeature = Game1.currentLocation.terrainFeatures.FieldDict;
            string? door = getDoorAtTile(x, y);
            (CATEGORY?, string?) tileInfo = getTileInfo(x, y);
            string? junimoBundle = getJunimoBundleAt(x, y);
            string? resourceClump = getResourceClumpAtTile(x, y);
            string? farmAnimal = getFarmAnimalAt(Game1.currentLocation, x, y);

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
            else if (Game1.currentLocation.isWaterTile(x, y) && isColliding)
            {
                toReturn = "Water";
                category = CATEGORY.WaterTiles;
            }
            else if (Game1.currentLocation.isObjectAtTile(x, y))
            {
                (string?, CATEGORY?) obj = getObjectAtTile(x, y);
                toReturn = obj.Item1;
                category = obj.Item2;
            }
            else if (terrainFeature.ContainsKey(tile))
            {
                (string?, CATEGORY) tf = getTerrainFeatureAtTile(terrainFeature[tile]);
                string? terrain = tf.Item1;
                if (terrain != null)
                {
                    toReturn = terrain;
                    category = tf.Item2;
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
            else if (tileInfo.Item2 != null)
            {
                toReturn = tileInfo.Item2;
                category = tileInfo.Item1;
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

        ///<summary>Returns the name of the object at tile</summary>
        public static string? getNameAtTile(Vector2 tile)
        {
            int x = (int)tile.X;
            int y = (int)tile.Y;
            string? toReturn = "";

            bool isColliding = isCollidingAtTile(x, y);
            Dictionary<Vector2, Netcode.NetRef<TerrainFeature>> terrainFeature = Game1.currentLocation.terrainFeatures.FieldDict;
            string? door = getDoorAtTile(x, y);
            (CATEGORY?, string?) tileInfo = getTileInfo(x, y);
            string? junimoBundle = getJunimoBundleAt(x, y);
            string? resourceClump = getResourceClumpAtTile(x, y);
            string? farmAnimal = getFarmAnimalAt(Game1.currentLocation, x, y);

            if (Game1.currentLocation.isCharacterAtTile(tile) != null)
            {
                NPC npc = Game1.currentLocation.isCharacterAtTile(tile);
                toReturn = npc.displayName;
            }
            else if (farmAnimal != null)
            {
                toReturn = farmAnimal;
            }
            else if (Game1.currentLocation.isWaterTile(x, y) && isColliding)
            {
                toReturn = "Water";
            }
            else if (Game1.currentLocation.isObjectAtTile(x, y))
            {
                toReturn = getObjectAtTile(x, y).Item1;
            }
            else if (terrainFeature.ContainsKey(tile))
            {
                string? terrain = getTerrainFeatureAtTile(terrainFeature[tile]).Item1;
                if (terrain != null)
                    toReturn = terrain;
            }
            else if (Game1.currentLocation.getLargeTerrainFeatureAt(x, y) != null)
            {
                toReturn = getBushAtTile(x, y);
            }
            else if (resourceClump != null)
            {
                toReturn = resourceClump;
            }
            else if (door != null)
            {
                toReturn = door;
            }
            else if (isMineDownLadderAtTile(x, y))
            {
                toReturn = "Ladder";
            }
            else if (isMineUpLadderAtTile(x, y))
            {
                toReturn = "Up Ladder";
            }
            else if (isElevatorAtTile(x, y))
            {
                toReturn = "Elevator";
            }
            else if (tileInfo.Item2 != null)
            {
                toReturn = tileInfo.Item2;
            }
            else if (junimoBundle != null)
            {
                toReturn = junimoBundle;
            }

            if (toReturn == "")
                return null;

            return toReturn;
        }

        public static string? getBushAtTile(int x, int y)
        {
            string? toReturn = null;
            Bush bush = (Bush)Game1.currentLocation.getLargeTerrainFeatureAt(x, y);
            int size = bush.size;

            #region Check if bush is harvestable or not
            if (!bush.townBush && (int)bush.tileSheetOffset == 1 && bush.inBloom(Game1.GetSeasonForLocation(Game1.currentLocation), Game1.dayOfMonth))
            {
                // Taken from the game's code
                string season = ((int)bush.overrideSeason == -1) ? Game1.GetSeasonForLocation(Game1.currentLocation) : Utility.getSeasonNameFromNumber(bush.overrideSeason);
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

            if (bush.townBush)
                toReturn = $"{toReturn} Town Bush";
            else if (bush.greenhouseBush)
                toReturn = $"{toReturn} Greenhouse Bush";
            else
                toReturn = $"{toReturn} Bush";

            return toReturn;
        }

        public static string? getJunimoBundleAt(int x, int y)
        {
            if (Game1.currentLocation is not CommunityCenter)
                return null;

            CommunityCenter communityCenter = ((CommunityCenter)Game1.currentLocation);

            string? name = (x, y) switch
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
            else
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
        /// <returns>Item1: This is the category of the tile. Default to Furnitures.
        /// <br/>Item2: This is the name of the tile. Default to null if the tile tile has nothing on it.</returns>
        public static (CATEGORY?, string?) getTileInfo(int x, int y)
        {

            int? index = null;

            if (Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y] != null)
                index = Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y].TileIndex;
            /* Add More
            MainClass.monitor.Log(index.ToString(), LogLevel.Debug);
            */

            if (Game1.currentLocation is Farm)
            {
                Building building = ((Farm)Game1.currentLocation).getBuildingAt(new Vector2(x, y));
                if (building != null)
                {
                    return (CATEGORY.Buildings, building.buildingType.Value);
                }
            }

            if (index != null)
            {
                switch (index)
                {
                    case 1955:
                    case 41:
                        return (CATEGORY.Furnitures, "Mail Box");
                    case 1003:
                        return (CATEGORY.Furnitures, "Street lamp");
                    case 78:
                        return (CATEGORY.Furnitures, "Trash bin");
                    case 617:
                        return (CATEGORY.Furnitures, "Daily quest");
                    case 616:
                        return (CATEGORY.Furnitures, "Calender");
                }

                if (Game1.currentLocation is FarmHouse || Game1.currentLocation is IslandFarmHouse)
                {
                    switch (index)
                    {
                        case 173:
                            return (CATEGORY.Furnitures, "Fridge");
                        case 169:
                        case 170:
                        case 171:
                        case 172:
                            return (CATEGORY.Furnitures, "Kitchen");
                    }
                }

            }

            return (null, null);
        }

        public static (string?, CATEGORY) getTerrainFeatureAtTile(Netcode.NetRef<TerrainFeature> terrain)
        {
            string? toReturn = null;
            CATEGORY category = CATEGORY.Others;

            if (terrain.Get() is HoeDirt)
            {
                category = CATEGORY.Crops;
                HoeDirt dirt = (HoeDirt)terrain.Get();
                if (dirt.crop != null)
                {
                    string cropName = Game1.objectInformation[dirt.crop.indexOfHarvest].Split('/')[0];
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
            else if (terrain.Get() is Flooring)
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

        public static (string?, CATEGORY) getObjectAtTile(int x, int y)
        {
            string? toReturn = null;

            StardewValley.Object obj = Game1.currentLocation.getObjectAtTile(x, y);
            int index = obj.ParentSheetIndex;
            toReturn = obj.DisplayName;

            // Get object names based on index
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

            if (obj is Chest)
            {
                Chest chest = (Chest)obj;
                return (chest.DisplayName, CATEGORY.Chests);
            }

            if (obj is Furniture)
                return (toReturn, CATEGORY.Furnitures);

            if (toReturn != null)
                return (toReturn, CATEGORY.Others);

            return (null, CATEGORY.Others);
        }

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
                    int index = Game1.currentLocation.resourceClumps[i].parentSheetIndex;

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
            {
                return null;
            }
            Netcode.NetObjectList<ResourceClump> stumps = ((Woods)Game1.currentLocation).stumps;
            for (int i = 0; i < stumps.Count; i++)
            {
                if (stumps[i].occupiesTile(x, y))
                {
                    return "large stump";
                }
            }
            return null;
        }
    }
}
