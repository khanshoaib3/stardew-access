

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
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
                        ScreenReader.prevTextTile = " ";
                    }

                    Dictionary<Vector2, Netcode.NetRef<TerrainFeature>> terrainFeature = Game1.currentLocation.terrainFeatures.FieldDict;
                    string toSpeak = " ";

                    #region Get objects, crops, resource clumps, etc.
                    if (Game1.currentLocation.isCharacterAtTile(gt) != null)
                    {
                        NPC npc = Game1.currentLocation.isCharacterAtTile(gt);
                        toSpeak = npc.displayName;
                    }
                    else if (Game1.currentLocation.isWaterTile(x, y))
                    {
                        toSpeak = "Water";
                    }
                    else if (Game1.currentLocation.getObjectAtTile(x, y) != null)
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
                    else if (isDoorAtTile(x, y))
                    {
                        toSpeak = "Door";
                    }
                    else if (isMineLadderAtTile(x, y))
                    {
                        toSpeak = "Ladder";
                    }
                    else if (getResourceClumpAtTile(x, y) != null)
                    {
                        toSpeak = getResourceClumpAtTile(x, y);
                    }
                    else if (!Game1.currentLocation.isTilePassable(Game1.player.nextPosition(Game1.player.getDirection()), Game1.viewport))
                    {
                        toSpeak = "Colliding";
                    }
                    #endregion

                    #region Narrate toSpeak
                    if (toSpeak != " ")
                        if (manuallyTriggered)
                            ScreenReader.say(toSpeak, true);
                        else
                            ScreenReader.sayWithTileQuery(toSpeak, x, y, true);
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
            else if(stage == 1)
                toReturn = $"{toReturn} sprout";
            else if(stage == 2)
                toReturn = $"{toReturn} sapling";
            else if(stage == 3)
                toReturn = $"{toReturn} bush";
            else if(stage >= 4)
                toReturn = $"{toReturn} tree";

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
                else if(treeStage == 2)
                    treeName = $"{treeName} sapling";
                else if(treeStage == 3 || treeStage == 4)
                    treeName = $"{treeName} bush";
                else if(treeStage >= 5)
                    treeName = $"{treeName} tree";

                return treeName;
            }

            return seedName;
        }

        public static string getObjectNameAtTile(int x, int y)
        {
            string? toReturn = null;

            StardewValley.Object obj = Game1.currentLocation.getObjectAtTile(x, y);
            toReturn = obj.DisplayName;

            // TODO add individual stone narration using parentSheetIndex
            // monitor.Log(obj.parentSheetIndex.ToString(), LogLevel.Debug);
            if (Game1.objectInformation.ContainsKey(obj.ParentSheetIndex) && toReturn.ToLower().Equals("stone"))
            {
                string info = Game1.objectInformation[obj.parentSheetIndex];
                if (info.ToLower().Contains("copper"))
                    toReturn = "Copper " + toReturn;
            }
            return toReturn;
        }

        public static bool isMineLadderAtTile(int x, int y)
        {
            if (Game1.inMine || Game1.currentLocation is Mine)
            {
                int index = Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y].TileIndex;

                if (index == 173 || index == 174)
                    return true;
            }

            return false;
        }

        public static bool isDoorAtTile(int x, int y)
        {
            Point tilePoint = new Point(x, y);
            List<SerializableDictionary<Point, string>> doorList = Game1.currentLocation.doors.ToList();

            for (int i=0; i < doorList.Count; i++)
            { 
                for(int j = 0; j< doorList[i].Keys.Count; j++)
                {
                    if (doorList[i].Keys.Contains(tilePoint))
                        return true;
                }
            }

            return false;
        }

        public static string getResourceClumpAtTile(int x, int y)
        {
            string? toReturn = null;

            for(int i = 0; i < Game1.currentLocation.resourceClumps.Count; i++)
            {
                if (Game1.currentLocation.resourceClumps[i].occupiesTile(x, y))
                {
                    int index = Game1.currentLocation.resourceClumps[i].parentSheetIndex;

                    switch (index)
                    {
                        case 600:
                            toReturn = "Large Stump";
                            break;
                        case 602:
                            toReturn = "Hollow Log";
                            break;
                        case 622:
                            toReturn = "Meteorite";
                            break;
                        case 752:
                        case 754:
                        case 756:
                        case 758:
                            toReturn = "Mine Rock";
                            break;
                        case 672:
                            toReturn = "Boulder";
                            break;
                        default:
                            toReturn = "Unknown";
                            break;
                    }

                    return toReturn;
                }
            }

            return toReturn;
        }
    }
}
