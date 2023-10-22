using static stardew_access.Utils.ObjectUtils;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Text;
using stardew_access.Translation;

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
                        ? Translator.Instance.Translate("terrain_util-forage_crop_types", new
                            { type = dirt.crop.whichForageCrop.Value })
                        : GetObjectById(dirt.crop.indexOfHarvest.Value)
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

            if (MainClass.Config.WateredToggle && dirtDetails.IsWatered)
                detailString.Append($"{Translator.Instance.Translate("terrain_util-crop-watered")} ");
            else if (!MainClass.Config.WateredToggle && !dirtDetails.IsWatered)
                detailString.Append($"{Translator.Instance.Translate("terrain_util-crop-unwatered")} ");

            if (dirtDetails.IsFertilized)
                detailString.Append($"{Translator.Instance.Translate("terrain_util-fertilized")} ");

            if (dirtDetails.CropType != null)
            {
                if (dirtDetails.IsReadyForHarvest)
                    detailString.Append($"{Translator.Instance.Translate("terrain_util-harvestable")} ");
                if (dirtDetails.IsDead)
                    detailString.Append($"{Translator.Instance.Translate("terrain_util-crop-dead")} ");
                detailString.Append(dirtDetails.CropType);
            }
            else if (!ignoreIfEmpty)
            {
                detailString.Append(Translator.Instance.Translate("terrain_util-crop-soil"));
            }

            return detailString.ToString().Trim();
        }

        public static (string TreeType, int GrowthStage, bool IsHarvestable) GetFruitTreeInfo(FruitTree fruitTree)
        {
            int stage = fruitTree.growthStage.Value;
            int fruitIndex = fruitTree.indexOfFruit.Get();
            string treeType = GetObjectById(fruitIndex);
            bool isHarvestable = fruitTree.fruitsOnTree.Value > 0;

            return (treeType, stage, isHarvestable);
        }

        public static string GetFruitTreeInfoString((string TreeType, int GrowthStage, bool IsHarvestable) fruitTreeDetails)
        {
            string growthStage = Translator.Instance.Translate("terrain_util-fruit_tree_growth_stage",
                new { stage = fruitTreeDetails.GrowthStage });

            string result = $"{fruitTreeDetails.TreeType} {growthStage}";
            if (fruitTreeDetails.IsHarvestable)
            {
                result = $"{Translator.Instance.Translate("terrain_util-harvestable")} {result}";
            }
            return result;
        }

        public static string GetFruitTreeInfoString(FruitTree fruitTree)
        {
            var fruitTreeDetails = GetFruitTreeInfo(fruitTree);
            return GetFruitTreeInfoString(fruitTreeDetails);
        }

        public static (int TreeType, int GrowthStage, string SeedName, bool IsFertilized) GetTreeInfo(Tree tree)
        {
            int treeStage = tree.growthStage.Value;
            string seedName = "";
            
            if (tree.treeType.Value <= 3 || tree.treeType.Value == 8)
                seedName = GetObjectById(308 + (tree.treeType.Value == 8 ? -16 : tree.treeType.Value));

            return (tree.treeType.Value, treeStage, seedName, tree.fertilized.Value);
        }

        public static string GetTreeInfoString((int TreeType, int GrowthStage, string SeedName, bool IsFertilized) treeDetails)
        {
            string treeType = Translator.Instance.Translate("terrain_util-tree_type",
                new { type = treeDetails.TreeType });
            # if debug
            if (treeDetails.TreeType == 5 || treeDetails.TreeType == 9)
                treeType = $"{treeType} 2";
            #endif

            if (treeDetails.GrowthStage == 0)
            {
                if (treeDetails.TreeType == 1 || treeDetails.TreeType == 2 || treeDetails.TreeType == 3 ||
                    treeDetails.TreeType == 8)
                {
                    return (treeDetails.IsFertilized
                        ? $"{Translator.Instance.Translate("terrain_util-fertilized")} "
                        : "") + treeDetails.SeedName;
                }
                else
                {
                    return (treeDetails.IsFertilized
                            ? $"{Translator.Instance.Translate("terrain_util-fertilized")} "
                            : "") + $"{treeType} {Translator.Instance.Translate("terrain_util-tree-seedling")}";
                }
            }

            string growthStage = Translator.Instance.Translate("terrain_util-tree_growth_stage",
                new { stage = treeDetails.GrowthStage });

            return (treeDetails.IsFertilized ? $"{Translator.Instance.Translate("terrain_util-fertilized")} ":"") + $"{treeType} {growthStage}";
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
            string description = isPathway ? "tile_name-pathway" : (isSteppingStone ? "tile_name-stepping_stone" : "tile_name-flooring");
            return Translator.Instance.Translate(description);
        }

        public static string GetGrassInfoString(Grass grass)
        {
            // in case we ever need to do more logic with grass; i.E. updates or grass mods
            // for now just return translation key, as there seems to be no way to get "grass" in translated form from the game.
            return "tile-grass-name";
        }

        public static (bool IsTownBush, bool IsGreenhouseBush, bool IsHarvestable, int BushType, int Age, int ShakeOff) GetBushInfo(Bush bush)
        {
            // Local function to get shake off object value
            int GetBushShakeOff(string season)
            {
                int shakeOff = season switch
                {
                    "spring" => 296, // Salmonberry
                    "fall" => 410, // Blackberry
                    _ => -1 // none
                };
                return bush.size.Value switch
                {
                    3 => 815, // Tea Leaves
                    4 => 73, // Golden Walnut
                    _ => shakeOff
                };
            }

            string season = bush.overrideSeason.Value == -1 
                ? Game1.GetSeasonForLocation(bush.currentLocation) 
                : Utility.getSeasonNameFromNumber(bush.overrideSeason.Value);
            
            int shakeOff = GetBushShakeOff(season);

            bool isHarvestable = !bush.townBush.Value && bush.tileSheetOffset.Value == 1 && bush.inBloom(season, Game1.dayOfMonth);

            return (bush.townBush.Value, bush.greenhouseBush.Value, isHarvestable, bush.size.Value, bush.getAge(), shakeOff);
        }

        public static string GetBushInfoString((bool IsTownBush, bool IsGreenhouseBush, bool IsHarvestable, int BushType, int Age, int ShakeOff) bushInfo)
        {
            StringBuilder bushInfoString = new();

            // Add the harvest status and item name if it's harvestable
            if (bushInfo.IsHarvestable)
            {
                //string harvestableItemName = ObjectUtils.GetObjectById(shakeOff);
                bushInfoString.Append($"{Translator.Instance.Translate("terrain_util-harvestable")} {GetObjectById(bushInfo.ShakeOff)} ");
            }

            // Add the type of the bush
            if (!MainClass.Config!.DisableBushVerbosity)
            {
                if (bushInfo.IsTownBush)
                {
                    bushInfoString.Append($"{Translator.Instance.Translate("terrain_util-bush-town")} ");
                }
                else if (bushInfo.IsGreenhouseBush)
                {
                    bushInfoString.Append($"{Translator.Instance.Translate("terrain_util-bush-greenhouse")} ");
                }

                bushInfoString.Append($"{Translator.Instance.Translate("terrain_util-bush_type",
                    new { type = bushInfo.BushType, has_matured = (bushInfo.Age < Bush.daysToMatureGreenTeaBush) ? 1 : 0 })} ");
            } else {
                // only name Tea bushes as they're plantable / harvestable
                if (bushInfo.BushType == Bush.greenTeaBush)
                    bushInfoString.Append($"{Translator.Instance.Translate("terrain_util-bush_type",
                        new { type = 3, has_matured = (bushInfo.Age < Bush.daysToMatureGreenTeaBush) ? 1 : 0 })} ");
            }

            // Append the word "Bush" to all except for tea bush
            if (bushInfo.BushType != Bush.greenTeaBush)
                bushInfoString.Append(Translator.Instance.Translate("terrain_util-bush"));

            return bushInfoString.ToString().Trim();
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
