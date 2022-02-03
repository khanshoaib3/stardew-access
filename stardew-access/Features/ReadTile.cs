using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace stardew_access.Game
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
                Vector2 gt = CurrentPlayer.getNextTile();
                int x = (int)gt.X;
                int y = (int)gt.Y;
                #endregion

                if (Context.IsPlayerFree)
                {
                    if (!manuallyTriggered && prevTile != gt)
                    {
                        if (MainClass.screenReader != null)
                            MainClass.screenReader.PrevTextTile = " ";
                    }

                    bool isColliding = isCollidingAtTile(x, y);

                    Dictionary<Vector2, Netcode.NetRef<TerrainFeature>> terrainFeature = Game1.currentLocation.terrainFeatures.FieldDict;
                    string? toSpeak = " ";

                    #region Get objects, crops, resource clumps, etc.
                    if (Game1.currentLocation.isCharacterAtTile(gt) != null)
                    {
                        NPC npc = Game1.currentLocation.isCharacterAtTile(gt);
                        toSpeak = npc.displayName;
                    }
                    else if (getFarmAnimalAt(Game1.currentLocation, x, y) != null)
                    {
                        toSpeak = getFarmAnimalAt(Game1.currentLocation, x, y);
                    }
                    else if (Game1.currentLocation.isWaterTile(x, y) && isColliding)
                    {
                        toSpeak = "Water";
                    }
                    else if (Game1.currentLocation.isObjectAtTile(x, y))
                    {
                        string? objectName = getObjectNameAtTile(x, y);
                        if (objectName != null)
                            toSpeak = objectName;
                    }
                    else if (terrainFeature.ContainsKey(gt))
                    {
                        string? terrain = getTerrainFeatureAtTile(terrainFeature[gt]);
                        if (terrain != null)
                            toSpeak = terrain;
                    }
                    else if (Game1.currentLocation.getLargeTerrainFeatureAt(x, y) != null)
                    {
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
                                return;
                            }

                            toSpeak = "Harvestable";
                        }
                        #endregion

                        if (bush.townBush)
                            toSpeak = $"{toSpeak} Town Bush";
                        else if (bush.greenhouseBush)
                            toSpeak = $"{toSpeak} Greenhouse Bush";
                        else
                            toSpeak = $"{toSpeak} Bush";
                    }
                    else if (getResourceClumpAtTile(x, y) != null)
                    {
                        toSpeak = getResourceClumpAtTile(x, y);
                    }
                    else if (isDoorAtTile(x, y))
                    {
                        toSpeak = "Door";
                    }
                    else if (isMineDownLadderAtTile(x, y))
                    {
                        toSpeak = "Ladder";
                    }
                    else if (isMineUpLadderAtTile(x, y))
                    {
                        toSpeak = "Up Ladder";
                    }
                    else if (isElevatorAtTile(x, y))
                    {
                        toSpeak = "Elevator";
                    }
                    else if (getTileInfo(x, y).Item2 != null)
                    {
                        toSpeak = getTileInfo(x, y).Item2;
                    }
                    else if (getJunimoBundleAt(x, y) != null)
                    {
                        toSpeak = getJunimoBundleAt(x, y);
                    }
                    #endregion

                    #region Narrate toSpeak
                    if (toSpeak != " ")
                        if (manuallyTriggered)
                            MainClass.screenReader.Say(toSpeak, true);
                        else
                            MainClass.screenReader.SayWithTileQuery(toSpeak, x, y, true);
                    #endregion

                    #region Play colliding sound effect
                    if (isColliding && prevTile != gt)
                    {
                        Game1.playSound("colliding");
                    }
                    #endregion

                    prevTile = gt;
                }

            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Error in Read Tile:\n{e.Message}\n{e.StackTrace}", LogLevel.Debug);
            }

            await Task.Delay(100);
            isReadingTile = false;
        }

        public static string? getJunimoBundleAt(int x, int y)
        {
            if (Game1.currentLocation is not CommunityCenter)
                return null;

            CommunityCenter communityCenter = (Game1.currentLocation as CommunityCenter);

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
                farmAnimals = (location as Farm).getAllFarmAnimals();
            else if (location is AnimalHouse)
                farmAnimals = (location as AnimalHouse).animals.Values.ToList();

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

        public static string getTerrainFeatureAtTile(Netcode.NetRef<TerrainFeature> terrain)
        {
            string? toReturn = null;
            if (terrain.Get() is HoeDirt)
            {
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
                int whichCrop = (terrain.Get() as GiantCrop).which.Value;
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
            else if (terrain.Get() is Bush)
            {
                toReturn = "Bush";
            }
            else if (terrain.Get() is CosmeticPlant)
            {
                CosmeticPlant cosmeticPlant = (CosmeticPlant)terrain.Get();
                toReturn = cosmeticPlant.textureName().ToLower();

                if (toReturn.Contains("terrain"))
                    toReturn.Replace("terrain", "");

                if (toReturn.Contains("feature"))
                    toReturn.Replace("feature", "");
            }
            else if (terrain.Get() is Flooring)
            {
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
                toReturn = getFruitTree((FruitTree)terrain.Get());
            }
            else if (terrain.Get() is Grass)
            {
                toReturn = "Grass";
            }
            else if (terrain.Get() is Tree)
            {
                toReturn = getTree((Tree)terrain.Get());
            }
            else if (terrain.Get() is Quartz)
            {
                toReturn = "Quartz";
            }
            else if (terrain.Get() is Leaf)
            {
                toReturn = "Leaf";
            }

            return toReturn;
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

        public static string? getObjectNameAtTile(int x, int y)
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
                    return "Weed";
                case 792:
                case 793:
                case 794:
                    return "Fertile weed";
                case 319:
                case 320:
                case 321:
                    return "Ice crystal";
                case 75:
                    return "Geode";
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
                    return "Coloured stone";
                case 668:
                case 670:
                case 845:
                case 846:
                case 847:
                    return "Mine stone";
                case 818:
                    return "Clay stone";
                case 816:
                case 817:
                    return "Fossil stone";
                case 25:
                    return "Stone";
                case 118:
                case 120:
                case 122:
                case 124:
                    return "Barrel";
                case 119:
                case 121:
                case 123:
                case 125:
                    return "Item box";
            }

            if (Game1.currentLocation is Mine or MineShaft)
            {
                switch (index)
                {
                    case 76:
                        return "Frozen geode";
                    case 77:
                        return "Magma geode";
                    case 8:
                    case 66:
                        return "Amethyst node";
                    case 14:
                    case 62:
                        return "Aquamarine node";
                    case 843:
                    case 844:
                        return "Cinder shard node";
                    case 2:
                    case 72:
                        return "Diamond node";
                    case 12:
                    case 60:
                        return "Emerald node";
                    case 44:
                        return "Gem node";
                    case 6:
                    case 70:
                        return "Jade node";
                    case 46:
                        return "Mystic stone";
                    case 74:
                        return "Prismatic node";
                    case 4:
                    case 64:
                        return "Ruby node";
                    case 10:
                    case 68:
                        return "Topaz node";
                    case 819:
                        return "Omni geode node";
                    case 751:
                    case 849:
                        return "Copper node";
                    case 764:
                        return "Gold node";
                    case 765:
                        return "Iridium node";
                    case 290:
                    case 850:
                        return "Iron node";
                }
            }

            if (obj is Chest)
            {
                Chest chest = (Chest)obj;
                toReturn = chest.DisplayName;
            }

            return toReturn;
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

        public static bool isDoorAtTile(int x, int y)
        {
            Point tilePoint = new Point(x, y);
            List<SerializableDictionary<Point, string>> doorList = Game1.currentLocation.doors.ToList();

            for (int i = 0; i < doorList.Count; i++)
            {
                for (int j = 0; j < doorList[i].Keys.Count; j++)
                {
                    if (doorList[i].Keys.Contains(tilePoint))
                        return true;
                }
            }

            return false;
        }

        public static string? getResourceClumpAtTile(int x, int y)
        {

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
    }
}
