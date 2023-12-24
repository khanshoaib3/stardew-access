using Microsoft.Xna.Framework;
using stardew_access.Patches;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace stardew_access.Commands;

public class TileMarkingCommands : ICustomCommand
{
    private IModHelper? _modHelper;

    public void Add(IModHelper modHelper)
    {
        _modHelper = modHelper;

        _modHelper.ConsoleCommands.Add("mark",
            "Marks the player's position for use in building construction in Carpenter Menu.", MarkPosition);

        _modHelper.ConsoleCommands.Add("marklist", "List all marked positions.", ListMarked);

        _modHelper.ConsoleCommands.Add("buildlist",
            "List all buildings for selection for upgrading/demolishing/painting",
            (_, _) => { BuildList(); });

        _modHelper.ConsoleCommands.Add("buildsel", "Select the building index which you want to upgrade/demolish/paint",
            SelectBuilding);
    }

    internal static void BuildList()
    {
        Farm farm = (Farm)Game1.getLocationFromName("Farm");
        Netcode.NetCollection<Building> buildings = farm.buildings;
        int buildingIndex = 0;

        List<string> buildingInfos = [];
        foreach (var building in buildings)
        {
            string? name = building.nameOfIndoorsWithoutUnique;
            name = (name == "null") ? building.buildingType.Value : name;

            BuildingOperations.availableBuildings[buildingIndex] = building;
            buildingInfos.Add(Translator.Instance.Translate("commands-tile_marking-build_list-building_info", new
            {
                index = buildingIndex,
                name,
                x_position = building.tileX.Value,
                y_position = building.tileY.Value
            }, translationCategory: TranslationCategory.CustomCommands));
            ++buildingIndex;
        }
        var toPrint = string.Join("\n", buildingInfos);

        if (string.IsNullOrWhiteSpace(toPrint))
        {
            Log.Info(Translator.Instance.Translate("commands-tile_marking-build_list-no_building", translationCategory: TranslationCategory.CustomCommands));
        }
        else
        {
            Log.Info(Translator.Instance.Translate("commands-tile_marking-build_list-buildings_list", new
            {
                building_infos = toPrint
            }, translationCategory: TranslationCategory.CustomCommands));
        }
    }

    private void MarkPosition(string command, string[] args)
    {
        if (Game1.currentLocation is not Farm)
        {
            Log.Info(Translator.Instance.Translate("commands-tile_marking-mark-not_in_farm",
                translationCategory: TranslationCategory.CustomCommands));
            return;
        }

        string? indexInString = args.ElementAtOrDefault(0);
        if (indexInString == null)
        {
            Log.Info(Translator.Instance.Translate("commands-tile_marking-mark-index_not_entered",
                translationCategory: TranslationCategory.CustomCommands));
            return;
        }

        bool isParsable = int.TryParse(indexInString, out int index);

        if (!isParsable || !(index is >= 0 and <= 9))
        {
            Log.Info(Translator.Instance.Translate("commands-tile_marking-mark-wrong_index",
                translationCategory: TranslationCategory.CustomCommands));
            return;
        }

        BuildingOperations.marked[index] = new Vector2(Game1.player.getTileX(), Game1.player.getTileY());
        Log.Info(Translator.Instance.Translate("commands-tile_marking-mark-location_marked", new
        {
            x_position = Game1.player.getTileX(),
            y_position = Game1.player.getTileY(),
            index
        }, translationCategory: TranslationCategory.CustomCommands));
    }

    private void ListMarked(string command, string[] args)
    {
        List<string> markInfos = [];
        for (int i = 0; i < BuildingOperations.marked.Length; i++)
        {
            if (BuildingOperations.marked[i] == Vector2.Zero) continue;
            
            markInfos.Add(Translator.Instance.Translate("commands-tile_marking-mark_list-mark_info", new
            {
                index = i,
                x_position = BuildingOperations.marked[i].X,
                y_position = BuildingOperations.marked[i].Y
            }, translationCategory: TranslationCategory.CustomCommands));
        }
        var toPrint = string.Join("\n", markInfos);

        if (string.IsNullOrWhiteSpace(toPrint))
        {
            Log.Info(Translator.Instance.Translate("commands-tile_marking-mark_list-not_marked", translationCategory: TranslationCategory.CustomCommands));
        }
        else
        {
            Log.Info(Translator.Instance.Translate("commands-tile_marking-mark_list-marks_list", new
            {
                mark_infos = toPrint
            }, translationCategory: TranslationCategory.CustomCommands));
        }
    }

    private void SelectBuilding(string command, string[] args)
    {
        if ((Game1.activeClickableMenu is not CarpenterMenu && Game1.activeClickableMenu is not PurchaseAnimalsMenu &&
             Game1.activeClickableMenu is not AnimalQueryMenu) || (!CarpenterMenuPatch.isOnFarm &&
                                                                   !PurchaseAnimalsMenuPatch.isOnFarm &&
                                                                   !AnimalQueryMenuPatch.isOnFarm))
        {
            Log.Info(Translator.Instance.Translate("commands-tile_marking-build_sel-cannot_select",
                translationCategory: TranslationCategory.CustomCommands));
            return;
        }

        string? indexInString = args.ElementAtOrDefault(0);
        if (indexInString == null)
        {
            Log.Info(Translator.Instance.Translate("commands-tile_marking-build_sel-building_index_not_entered",
                translationCategory: TranslationCategory.CustomCommands));
            return;
        }

        bool isParsable = int.TryParse(indexInString, out int index);

        if (!isParsable)
        {
            Log.Info(Translator.Instance.Translate("commands-tile_marking-build_sel-wrong_index",
                translationCategory: TranslationCategory.CustomCommands));
            return;
        }

        string? positionIndexInString = args.ElementAtOrDefault(1);
        int positionIndex = 0;

        if (CarpenterMenuPatch.isMoving)
        {
            if (CarpenterMenuPatch.isConstructing || CarpenterMenuPatch.isMoving)
            {
                if (BuildingOperations.availableBuildings[index] == null)
                {
                    Log.Info(Translator.Instance.Translate("commands-tile_marking-build_sel-no_building_found",
                        new { index }, translationCategory: TranslationCategory.CustomCommands));
                    return;
                }

                if (positionIndexInString == null)
                {
                    Log.Info(Translator.Instance.Translate("commands-tile_marking-build_sel-marked_index_not_entered",
                        translationCategory: TranslationCategory.CustomCommands));
                    return;
                }

                isParsable = int.TryParse(positionIndexInString, out positionIndex);

                if (!isParsable)
                {
                    Log.Info(Translator.Instance.Translate("commands-tile_marking-build_sel-wrong_index",
                        translationCategory: TranslationCategory.CustomCommands));
                    return;
                }
            }
        }
        else if (CarpenterMenuPatch.isConstructing && !CarpenterMenuPatch.isUpgrading)
        {
            if (BuildingOperations.marked[index] == Vector2.Zero)
            {
                Log.Info(Translator.Instance.Translate("commands-tile_marking-build_sel-no_marked_position_found",
                    new { index }, translationCategory: TranslationCategory.CustomCommands));
                return;
            }
        }
        else
        {
            if (BuildingOperations.availableBuildings[index] == null)
            {
                Log.Info(Translator.Instance.Translate("commands-tile_marking-build_sel-no_building_found",
                    new { index }, translationCategory: TranslationCategory.CustomCommands));
                return;
            }
        }

        string? response = null;

        if (Game1.activeClickableMenu is PurchaseAnimalsMenu)
        {
            BuildingOperations.PurchaseAnimal(BuildingOperations.availableBuildings[index]);
        }
        else if (Game1.activeClickableMenu is AnimalQueryMenu)
        {
            BuildingOperations.MoveAnimal(BuildingOperations.availableBuildings[index]);
        }
        else
        {
            if (CarpenterMenuPatch.isConstructing && !CarpenterMenuPatch.isUpgrading)
            {
                response = BuildingOperations.Construct(BuildingOperations.marked[index]);
            }
            else if (CarpenterMenuPatch.isMoving)
            {
                response = BuildingOperations.Move(BuildingOperations.availableBuildings[index],
                    BuildingOperations.marked[positionIndex]);
            }
            else if (CarpenterMenuPatch.isDemolishing)
            {
                response = BuildingOperations.Demolish(BuildingOperations.availableBuildings[index]);
            }
            else if (CarpenterMenuPatch.isUpgrading)
            {
                response = BuildingOperations.Upgrade(BuildingOperations.availableBuildings[index]);
            }
            else if (CarpenterMenuPatch.isPainting)
            {
                response = BuildingOperations.Paint(BuildingOperations.availableBuildings[index]);
            }
        }

        if (response != null)
        {
            Log.Info(response);
        }
    }
}