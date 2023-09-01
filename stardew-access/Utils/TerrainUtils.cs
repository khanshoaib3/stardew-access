using StardewValley;
using StardewValley.TerrainFeatures;
using System.Text;

namespace stardew_access.Utils
{
    public static class TerrainUtils
    {
        public static (bool IsWatered, bool IsFertilized, string? CropType, bool IsReadyForHarvest, bool IsDead) GetDirtInfo(HoeDirt dirt)
        {
            return (
                IsWatered: dirt.state.Value == HoeDirt.watered,
                IsFertilized: dirt.fertilizer.Value != HoeDirt.noFertilizer,
                CropType: dirt.crop != null
                    ? dirt.crop.forageCrop.Value
                        ? dirt.crop.whichForageCrop.Value switch
                        {
                            1 => "Spring onion",
                            2 => "Ginger",
                            _ => "Forageable crop",
                        }
                        : Game1.objectInformation[dirt.crop.indexOfHarvest.Value].Split('/')[0]
                    : null,
                IsReadyForHarvest: dirt.crop != null && dirt.readyForHarvest(),
                IsDead: dirt.crop?.dead.Value ?? false
            );
        }

        public static string GetDirtInfoString(HoeDirt dirt, bool ignoreIfEmpty = false)
        {
            var dirtDetails = GetDirtInfo(dirt);
            return GetDirtInfoString(dirtDetails, ignoreIfEmpty);
        }

        public static string GetDirtInfoString((bool IsWatered, bool IsFertilized, string? CropType, bool IsReadyForHarvest, bool IsDead) dirtDetails, bool ignoreIfEmpty = false)
        {
            StringBuilder detailString = new();

            if (MainClass.Config.WateredToggle)
            {
                detailString.Append(dirtDetails.IsWatered ? "Watered " : "Unwatered ");
            }

            if (dirtDetails.IsFertilized)
                detailString.Append("Fertilized ");

            if (dirtDetails.CropType != null)
            {
                if (dirtDetails.IsReadyForHarvest)
                    detailString.Append("Harvestable ");
                if (dirtDetails.IsDead)
                    detailString.Append("Dead ");
                detailString.Append(dirtDetails.CropType);
            }
            else if (!ignoreIfEmpty)
            {
                detailString.Append("Soil");
            }

            return detailString.ToString().Trim();
        }

        public static (string TreeType, int GrowthStage, bool IsHarvestable) GetFruitTreeInfo(FruitTree fruitTree)
        {
            int stage = fruitTree.growthStage.Value;
            int fruitIndex = fruitTree.indexOfFruit.Get();
            string treeType = Game1.objectInformation[fruitIndex].Split('/')[0];
            bool isHarvestable = fruitTree.fruitsOnTree.Value > 0;

            return (treeType, stage, isHarvestable);
        }

        public static string GetFruitTreeInfoString((string TreeType, int GrowthStage, bool IsHarvestable) fruitTreeDetails)
        {
            string growthStage = fruitTreeDetails.GrowthStage switch
            {
                0 => "seed",
                1 => "sprout",
                2 => "sapling",
                3 => "bush",
                _ => "tree"
            };

            string result = $"{fruitTreeDetails.TreeType} {growthStage}";
            if (fruitTreeDetails.IsHarvestable)
            {
                result = $"Harvestable {result}";
            }
            return result;
        }

        public static string GetFruitTreeInfoString(FruitTree fruitTree)
        {
            var fruitTreeDetails = GetFruitTreeInfo(fruitTree);
            return GetFruitTreeInfoString(fruitTreeDetails);
        }

        public static (int TreeType, int GrowthStage, string SeedName) GetTreeInfo(Tree tree)
        {
            int treeStage = tree.growthStage.Value;
            string seedName = "";

            if (tree.treeType.Value <= 3 || tree.treeType.Value == 8)
                seedName = Game1.objectInformation[308 + (tree.treeType.Value == 8 ? -16 : tree.treeType.Value)].Split('/')[0];

            return (tree.treeType.Value, treeStage, seedName);
        }

        public static string GetTreeInfoString((int TreeType, int GrowthStage, string SeedName) treeDetails)
        {
            string treeType = treeDetails.TreeType switch
            {
                1 => "Oak",
                2 => "Maple",
                3 => "Pine",
                4 => "Winter Tree",
                5 => "Winter Tree",
                6 => "Palm Tree",
                7 => "Mushroom Tree",
                8 => "Mahogany",
                9 => "Palm Tree",
                _ => $"Unknown tree type number {treeDetails.TreeType}"
            };
            # if debug
            if (treeDetails.TreeType == 5 || treeDetails.TreeType == 9)
                treeType = $"{treeType} 2";
            #endif

            if (treeDetails.GrowthStage == 0)
            {
                if (treeDetails.TreeType == 1 || treeDetails.TreeType == 2 || treeDetails.TreeType == 3 || treeDetails.TreeType == 8)
                {
                    return treeDetails.SeedName;
                }
                else
                {
                    return $"{treeType} seedling";
                }
            }

            string growthStage = treeDetails.GrowthStage switch
            {
                1 => "sprout",
                2 => "sapling",
                3 => "bush",
                4 => "bush",
                _ => "tree",
            };

            return $"{treeType} {growthStage}";
        }

        public static string GetTreeInfoString(Tree tree)
        {
            var treeDetails = GetTreeInfo(tree);
            return GetTreeInfoString(treeDetails);
        }

        public static string GetCosmeticPlantInfoString(CosmeticPlant cosmeticPlant)
        {
            string name = cosmeticPlant.textureName().ToLower();
            name = name.Replace("terrain", "").Replace("feature", "").Replace("  ", " ").Trim();
            return name;
        }

        public static string GetFlooringInfoString(Flooring flooring)
        {
            bool isPathway = flooring.isPathway.Get();
            bool isSteppingStone = flooring.isSteppingStone.Get();
            string description = isPathway ? "Pathway" : (isSteppingStone ? "Stepping Stone" : "Flooring");
            return description;
        }

        public static string GetGrassInfoString(Grass grass)
        {
            // in case we ever need to do more logic with grass; i.E. updates or grass mods
            // for now just return translation key, as there seems to be no way to get "grass" in translated form from the game.
            return "tile-grass-name";
        }

        private static int GetBushShakeOff(Bush bush, string season)
        {
            int shakeOff = season switch
            {
                "spring" => 296,
                "fall" => 410,
                _ => -1
            };
            return bush.size.Value switch
            {
                3 => 815,
                4 => 73,
                _ => shakeOff
            };
        }

        public static (bool IsTownBush, bool IsGreenhouseBush, bool IsHarvestable, int ShakeOff) GetBushInfo(Bush bush)
        {
            string season = bush.overrideSeason.Value == -1 
                ? Game1.GetSeasonForLocation(bush.currentLocation) 
                : Utility.getSeasonNameFromNumber(bush.overrideSeason.Value);
            
            int shakeOff = GetBushShakeOff(bush, season);

            bool isHarvestable = !bush.townBush.Value && bush.tileSheetOffset.Value == 1 && bush.inBloom(season, Game1.dayOfMonth);

            return (bush.townBush.Value, bush.greenhouseBush.Value, isHarvestable, shakeOff);
        }

        public static string GetBushInfoString((bool IsTownBush, bool IsGreenhouseBush, bool IsHarvestable, int ShakeOff) bushInfo)
        {
            StringBuilder bushInfoString = new();

            // Add the harvest status if it's harvestable
            if (bushInfo.IsHarvestable)
            {
                bushInfoString.Append("Harvestable ");
            }

            // Add the type of the bush
            if (bushInfo.IsTownBush)
            {
                bushInfoString.Append("Town ");
            }
            else if (bushInfo.IsGreenhouseBush)
            {
                bushInfoString.Append("Greenhouse ");
            }

            // Append the word "Bush" to all
            bushInfoString.Append("Bush");

            return bushInfoString.ToString();
        }

        public static string GetBushInfoString(Bush bush)
        {
            var bushInfo = GetBushInfo(bush);
            return GetBushInfoString(bushInfo);
        }

        public static (string? name, CATEGORY? category) GetTerrainFeatureInfoAndCategory(TerrainFeature? terrainFeature, bool ignoreIfEmpty = false)
        {
            if (terrainFeature == null) return (null, null);
            // Switch on the actual type of the terrain feature
            switch (terrainFeature)
            {
                case LargeTerrainFeature largeTerrainFeature:
                    return GetTerrainFeatureInfoAndCategory(largeTerrainFeature, ignoreIfEmpty);
                case HoeDirt dirt:
                    return (GetDirtInfoString(dirt, ignoreIfEmpty), CATEGORY.Crops);
                case CosmeticPlant cosmeticPlant:
                    return (GetCosmeticPlantInfoString(cosmeticPlant), CATEGORY.Furnitures);
                case Flooring flooring when MainClass.Config.ReadFlooring:
                    return (GetFlooringInfoString(flooring), CATEGORY.Flooring);
                case Flooring _:
                    return (null, null);  // Set to None or another suitable default to avoid logging
                case FruitTree fruitTree:
                    return (GetFruitTreeInfoString(fruitTree), CATEGORY.Trees);
                case Grass grass:
                    return (GetGrassInfoString(grass), CATEGORY.Debris);
                case Tree tree:
                    return (GetTreeInfoString(tree), CATEGORY.Trees);
                default:
                    Log.Warn($"Unknown terrain feature type: {terrainFeature.GetType().Name}", true);
                    return (null, null);
            }
        }

        public static (string? name, CATEGORY? category) GetTerrainFeatureInfoAndCategory(LargeTerrainFeature? largeTerrainFeature, bool ignoreIfEmpty = false)
        {
            if (largeTerrainFeature == null) return (null, null);

            switch (largeTerrainFeature)
            {
                case Bush bush:
                    return (GetBushInfoString(bush), CATEGORY.Bush);
                // Add more cases for other types of LargeTerrainFeature here
                default:
                    Log.Warn($"Unknown LargeTerrainFeature type: {largeTerrainFeature.GetType().Name}", true);
                    return (null, null);
            }
        }
    }
}
