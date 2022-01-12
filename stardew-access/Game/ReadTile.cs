

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
                    else if (!Game1.currentLocation.isTilePassable(Game1.player.nextPosition(Game1.player.getDirection()), Game1.viewport))
                    {
                        toSpeak = "Colliding";
                    }
                    else if (Game1.currentLocation.isWaterTile(x, y))
                    {
                        toSpeak = "Water";
                    }
                    else if (Game1.currentLocation.getObjectAtTile(x, y) != null)
                    {
                        #region Objects at tile (TODO)
                        
                        #endregion
                    }
                    else if (terrainFeature.ContainsKey(gt))
                    {
                        #region Terrain Feature
                        Netcode.NetRef<TerrainFeature> terrain = terrainFeature[gt];

                        if (terrain.Get() is HoeDirt)
                        {
                            HoeDirt dirt = (HoeDirt)terrain.Get();
                            if (dirt.crop != null)
                            {
                                string cropName = Game1.objectInformation[dirt.crop.indexOfHarvest].Split('/')[0];
                                toSpeak = $"{cropName}";

                                bool isWatered = dirt.state.Value == HoeDirt.watered;
                                bool isHarvestable = dirt.readyForHarvest();
                                bool isFertilized = dirt.fertilizer.Value != HoeDirt.noFertilizer;

                                if (isWatered)
                                    toSpeak = "Watered " + toSpeak;

                                if (isFertilized)
                                    toSpeak = "Fertilized " + toSpeak;

                                if (isHarvestable)
                                    toSpeak = "Harvestable " + toSpeak;
                            }
                            else
                            {
                                toSpeak = "Soil";
                                bool isWatered = dirt.state.Value == HoeDirt.watered;
                                bool isFertilized = dirt.fertilizer.Value != HoeDirt.noFertilizer;

                                if (isWatered)
                                    toSpeak = "Watered " + toSpeak;

                                if (isFertilized)
                                    toSpeak = "Fertilized " + toSpeak;
                            }
                        }
                        else if (terrain.Get() is Bush)
                        {
                            toSpeak = "Bush";
                        }
                        else if (terrain.Get() is CosmeticPlant)
                        {
                            CosmeticPlant cosmeticPlant = (CosmeticPlant)terrain.Get();
                            toSpeak = cosmeticPlant.textureName().ToLower();

                            if (toSpeak.Contains("terrain"))
                                toSpeak.Replace("terrain", "");

                            if (toSpeak.Contains("feature"))
                                toSpeak.Replace("feature", "");
                        }
                        else if (terrain.Get() is Flooring)
                        {
                            Flooring flooring = (Flooring)terrain.Get();
                            bool isPathway = flooring.isPathway.Get();
                            bool isSteppingStone = flooring.isSteppingStone.Get();

                            toSpeak = "Flooring";

                            if (isPathway)
                                toSpeak = "Pathway";

                            if (isSteppingStone)
                                toSpeak = "Stepping Stone";
                        }
                        else if (terrain.Get() is FruitTree)
                        {
                            FruitTree fruitTree = (FruitTree)terrain.Get();
                            toSpeak = Game1.objectInformation[fruitTree.treeType].Split('/')[0];
                        }
                        else if (terrain.Get() is Grass)
                        {
                            Grass grass = (Grass)terrain.Get();
                            toSpeak = "Grass";
                        }
                        else if (terrain.Get() is Tree)
                        {
                            Tree tree = (Tree)terrain.Get();
                            int treeType = tree.treeType;
                            int stage = tree.growthStage.Value;

                            if (Game1.player.getEffectiveSkillLevel(2) >= 1)
                            {
                                toSpeak = Game1.objectInformation[308 + (int)treeType].Split('/')[0];
                            }
                            else if (Game1.player.getEffectiveSkillLevel(2) >= 1 && (int)treeType <= 3)
                            {
                                toSpeak = Game1.objectInformation[308 + (int)treeType].Split('/')[0];
                            }
                            else if (Game1.player.getEffectiveSkillLevel(2) >= 1 && (int)treeType == 8)
                            {
                                toSpeak = Game1.objectInformation[292 + (int)treeType].Split('/')[0];
                            }

                            toSpeak = $"{toSpeak}, {stage} stage";
                        }
                        else if (terrain.Get() is Quartz)
                        {
                            toSpeak = "Quartz";
                        }
                        else if (terrain.Get() is Leaf)
                        {
                            toSpeak = "Leaf";
                        }
                        #endregion
                    }
                    else if (isDoorAtTile(x, y))
                        toSpeak = "Door";
                    else if (isMineLadderAtTile(x, y))
                        toSpeak = "Ladder";
                    else
                    {
                        string? resourceClump = getResourceClumpAtTile(x, y);
                        if (resourceClump!=null)
                            toSpeak = resourceClump;
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
                MainClass.monitor.Log($"Error in Read Tile:\n{e.Message}\n{e.StackTrace}");
            }

            await Task.Delay(100);
            isReadingTile = false;
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
