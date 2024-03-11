using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using stardew_access.Patches;
using stardew_access.Tiles;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace stardew_access.Features;

public class TileDataEntryMenu : IClickableMenu
{
    private readonly List<OptionsElement> _options;
    private readonly int _tileX;
    private readonly int _tileY;
    private Rectangle _currentBounds = Rectangle.Empty;
    private readonly AccessibleTile.JsonSerializerFormat? _defaultData;

    private enum OptionsIdentifiers
    {
        HeadingLabel = 236, // Starting with 0 creates some wierd bugs with farm house upgrade level drop down
        TileNameTextEntry,
        CategoryDropDown,
        ModDependencyDropDown,
        EventCheckbox,
        FarmTypeCheckbox,
        FarmHouseUpgradeLevelDropDown,
        QuestDropDown,
        ManualQuestIdTextBox,
        JojaMemberCheckbox
    }

    private readonly ClickableTextureComponent _okButton;
    private static readonly string None = Translator.Instance.Translate("menu-tile_data_entry-none", TranslationCategory.Menu);
    
    public TileDataEntryMenu(int tileX, int tileY, AccessibleTile.JsonSerializerFormat? defaultData = null)
        : base(Game1.viewport.Width / 2 - (1175 + borderWidth * 2) / 2,
            Game1.viewport.Height / 2 - (700 + borderWidth * 2) / 2 - 64, 1175 + borderWidth * 2,
            700 + borderWidth * 2 + 64)
    {
        _tileX = tileX;
        _tileY = tileY;
        _defaultData = defaultData;
        _options = [];
        allClickableComponents = [];
        PopulateOptions();
        _okButton = new ClickableTextureComponent(
            new Rectangle(xPositionOnScreen + 50, yPositionOnScreen + height - 84, 64, 64),
            Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
        allClickableComponents.Add(_okButton);
        AssignIDs();
        snapToDefaultClickableComponent();
    }

    private void PopulateOptions()
    {
        bool defaultJojaMemberCheckboxState = false;
        string defaultQuestId = "";
        string defaultFarmHouseLevel = None;
        if (_defaultData != null && _defaultData.Conditions != null && _defaultData.Conditions.Length > 0)
        {
            foreach (var condition in _defaultData.Conditions)
            {
                string[] parts = condition.Split(':');
                string functionName = parts[0];
                string? arg = parts.Length > 1 ? parts[1] : null;
                switch (functionName)
                {
                    case "Farm":
                        // Change to or add drop down
                        break;
                    case "FarmHouse":
                        defaultFarmHouseLevel = arg ?? None;
                        break;
                    case "HasQuest":
                        defaultQuestId = arg ?? "";
                        break;
                    case "JojaMember":
                        defaultJojaMemberCheckboxState = true;
                        break;
                    case "ActiveEvent":
                        // Change to or add drop down
                        break;
                }
            }
        }

        AddLabel( Translator.Instance.Translate("menu-tile_data_entry-heading_label",
                new { tile_x = _tileX, tile_y = _tileY, location_name = Game1.currentLocation.NameOrUniqueName },
                TranslationCategory.Menu),
            (int)OptionsIdentifiers.HeadingLabel);

        AddTextEntry("menu-tile_data_entry-tile_name_text_box_label", (int)OptionsIdentifiers.TileNameTextEntry,
            _defaultData?.NameOrTranslationKey ?? "");

        List<string> categoriesList = CATEGORY.Categories.Values.Select(category => category.Value).ToList();
        AddDropDown("menu-tile_data_entry-categories_drop_down_label", (int)OptionsIdentifiers.CategoryDropDown,
            categoriesList, (_defaultData is not null) ? CATEGORY.Categories.Keys.ToList().IndexOf(_defaultData.Category!) : 16);

        List<string> modsIdsList = [None];
        foreach (var modInfo in MainClass.ModHelper!.ModRegistry.GetAll())
        {
            if (modInfo.Manifest.UniqueID == MainClass.ModHelper.ModRegistry.ModID) continue;
            modsIdsList.Add(modInfo.Manifest.UniqueID);
        }
        int selectedModsIdIndex = 0;

        if (_defaultData != null && _defaultData.WithMods != null && _defaultData.WithMods.Length > 0)
        {
            foreach (var modsId in _defaultData.WithMods)
            {
                if (modsIdsList.Contains(modsId)) continue;
                modsIdsList.Insert(0, modsId);
            }

            selectedModsIdIndex = modsIdsList.IndexOf(_defaultData.WithMods[0]);
        }

        AddDropDown("menu-tile_data_entry-mod_dependency_drop_down_label",
            (int)OptionsIdentifiers.ModDependencyDropDown, modsIdsList, selectedModsIdIndex);

        if (Game1.currentLocation.currentEvent != null)
        {
            object tokens = new
            {
                is_festival = Game1.currentLocation.currentEvent.isFestival ? 1 : 0,
                festival_name = Game1.currentLocation.currentEvent.FestivalName,
                is_wedding = Game1.currentLocation.currentEvent.isWedding ? 1 : 0,
                event_id = Game1.currentLocation.currentEvent.id
            };
            AddCheckbox(
                Translator.Instance.Translate("menu-tile_data_entry-event_check_box_label", tokens, TranslationCategory.Menu),
                (int)OptionsIdentifiers.EventCheckbox, false);
        }

        if (Game1.currentLocation is Farm)
        {
            AddCheckbox(
                Translator.Instance.Translate("menu-tile_data_entry-farm_type_check_box_label",
                    new { farm_type = FarmTypeDisplayName(Game1.whichFarm) }, TranslationCategory.Menu),
                (int)OptionsIdentifiers.FarmTypeCheckbox, false);
        }

        if (Game1.currentLocation is FarmHouse)
        {
            List<string> upgradeLevelList = [None, "0", "1", "2", "3"];
            AddDropDown("menu-tile_data_entry-farm_house_upgrade_level_drop_down_label",
                (int)OptionsIdentifiers.FarmHouseUpgradeLevelDropDown,
                upgradeLevelList, upgradeLevelList.IndexOf(defaultFarmHouseLevel));
        }

        List<string> questOptions =
        [
            Translator.Instance.Translate("menu-tile_data_entry-quest_drop_down-manual_entry_option", TranslationCategory.Menu),
            .. Game1.player.questLog.Select(quest => quest.GetName()),
        ];
        AddDropDown("menu-tile_data_entry-quest_drop_down_label", (int)OptionsIdentifiers.QuestDropDown, questOptions,
            0);

        AddTextEntry("menu-tile_data_entry-manual_quest_id_text_box_label",
            (int)OptionsIdentifiers.ManualQuestIdTextBox, defaultQuestId);

        AddCheckbox("menu-tile_data_entry-joja_member_checkbox_label", (int)OptionsIdentifiers.JojaMemberCheckbox,
            defaultJojaMemberCheckboxState);
    }

    private Rectangle GetNextBounds()
    {
        _currentBounds = new Rectangle()
        {
            X = xPositionOnScreen + 50,
            Y = _currentBounds == Rectangle.Empty ? yPositionOnScreen + 20 : _currentBounds.Y + 80,
            Width = 400,
            Height = 50
        };
        return _currentBounds;
    }

    private void AddLabel(string labelOrTranslationKey, int identifier)
    {
        labelOrTranslationKey = Translator.Instance.Translate(labelOrTranslationKey, TranslationCategory.Menu, disableWarning: true);
        OptionsElement element = new(labelOrTranslationKey)
        {
            style = OptionsElement.Style.OptionLabel,
            whichOption = identifier,
            bounds = GetNextBounds()
        };
        allClickableComponents.Add(new ClickableComponent(element.bounds, element.label));
        _options.Add(element);
    }

    private void AddCheckbox(string labelOrTranslationKey, int identifier, bool isChecked)
    {
        labelOrTranslationKey = Translator.Instance.Translate(labelOrTranslationKey, TranslationCategory.Menu, disableWarning: true);
        OptionsCheckbox checkbox = new(labelOrTranslationKey, identifier)
        {
            isChecked = isChecked,
            bounds = GetNextBounds()
        };
        allClickableComponents.Add(new ClickableComponent(checkbox.bounds, checkbox.label));
        _options.Add(checkbox);
    }

    private void AddTextEntry(string labelOrTranslationKey, int identifier, string defaultText)
    {
        labelOrTranslationKey = Translator.Instance.Translate(labelOrTranslationKey, TranslationCategory.Menu, disableWarning: true);
        OptionsTextEntry textEntry = new(labelOrTranslationKey, identifier)
        {
            bounds = GetNextBounds(),
        };
        textEntry.textBox.Text = defaultText;
        allClickableComponents.Add(new ClickableComponent(textEntry.bounds, textEntry.label));
        _options.Add(textEntry);
    }

    private void AddDropDown(string labelOrTranslationKey, int identifier, List<string> options, int selectedOption)
    {
        labelOrTranslationKey = Translator.Instance.Translate(labelOrTranslationKey, TranslationCategory.Menu, disableWarning: true);
        OptionsDropDown dropDown = new(labelOrTranslationKey, identifier)
        {
            dropDownOptions = options,
            dropDownDisplayOptions = options,
            selectedOption = selectedOption,
            bounds = GetNextBounds()
        };
        dropDown.RecalculateBounds();
        allClickableComponents.Add(new ClickableComponent(dropDown.bounds, dropDown.label));
        _options.Add(dropDown);
    }

    private void AssignIDs()
    {
        int currId = 100;
        for (int i = 0; i < allClickableComponents.Count; i++)
        {
            allClickableComponents[i].myID = currId;
            allClickableComponents[i].upNeighborID = (i - 1 > -1) ? currId - 1 : -1;
            allClickableComponents[i].downNeighborID = (i + 1 < allClickableComponents.Count) ? currId + 1 : -1;
            ++currId;
        }
    }

    public sealed override void snapToDefaultClickableComponent()
    {
        setCurrentlySnappedComponentTo(100);
        snapCursorToCurrentSnappedComponent();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        foreach (var el in _options)
        {
            if (el is OptionsDropDown) continue;
            if (!el.bounds.Contains(x, y)) continue;
            if (el.greyedOut) continue;
            
            el.receiveLeftClick(x, y);
            return;
        }

        if (!_okButton.bounds.Contains(x, y))
        {
            base.receiveLeftClick(x, y, playSound);
            return;
        }

        GetEnteredTileInformation(out var tileInfo);
        if (tileInfo == null) return;
        if (_defaultData != null) UserTilesUtils.RemoveTileDataAt(_tileX, _tileY, Game1.currentLocation.NameOrUniqueName);
        UserTilesUtils.AddTileData(tileInfo);
        MainClass.TileManager.Initialize();
        exitThisMenu();
    }

    private void GetEnteredTileInformation(out AccessibleTile.JsonSerializerFormat? tileInfo)
    {
        string? tileName = null;
        string category = CATEGORY.Other.Key;
        List<string> withMods = [];
        List<string> conditions = [];
        tileInfo = null;

        bool questAdded = false;
        foreach (OptionsElement optionsElement in _options)
        {
            switch (optionsElement.whichOption)
            {
                case (int)OptionsIdentifiers.HeadingLabel:
                    break;
                case (int)OptionsIdentifiers.TileNameTextEntry:
                    tileName = (optionsElement as OptionsTextEntry)!.textBox.Text;
                    break;
                case (int)OptionsIdentifiers.CategoryDropDown:
                    var categoryDropDown = (optionsElement as OptionsDropDown)!;
                    category = CATEGORY.Categories.Keys.ElementAtOrDefault(categoryDropDown.selectedOption) ?? category;
                    break;
                case (int)OptionsIdentifiers.ModDependencyDropDown:
                    var modDependencyDropDown = (optionsElement as OptionsDropDown)!;
                    if (modDependencyDropDown.selectedOption != 0)
                        withMods.Add(
                            modDependencyDropDown.dropDownDisplayOptions[modDependencyDropDown.selectedOption]);
                    break;
                case (int)OptionsIdentifiers.EventCheckbox:
                    var eventCheckbox = (optionsElement as OptionsCheckbox)!;
                    if (eventCheckbox.isChecked && Game1.currentLocation.currentEvent != null)
                        conditions.Add($"ActiveEvent:{Game1.currentLocation.currentEvent.id}");
                    break;
                case (int)OptionsIdentifiers.FarmTypeCheckbox:
                    var farmTypeCheckbox = (optionsElement as OptionsCheckbox)!;
                    if (farmTypeCheckbox.isChecked)
                        conditions.Add($"Farm:{FarmTypeName(Game1.whichFarm)}");
                    break;
                case (int)OptionsIdentifiers.FarmHouseUpgradeLevelDropDown:
                    var farmHouseUpgradeDropDown = (optionsElement as OptionsDropDown)!;
                    if (farmHouseUpgradeDropDown.selectedOption != 0)
                        conditions.Add(
                            $"FarmHouse:{farmHouseUpgradeDropDown.dropDownDisplayOptions[farmHouseUpgradeDropDown.selectedOption]}");
                    break;
                case (int)OptionsIdentifiers.QuestDropDown:
                    var questDropDown = (optionsElement as OptionsDropDown)!;
                    if (questDropDown.selectedOption == 0) break;

                    foreach (var quest in Game1.player.questLog)
                    {
                        if (!quest.GetName()
                                .Equals(questDropDown.dropDownDisplayOptions[questDropDown.selectedOption]))
                            continue;
                        conditions.Add($"HasQuest:{quest.id.Value}");
                        questAdded = true;
                        break;
                    }

                    break;
                case (int)OptionsIdentifiers.ManualQuestIdTextBox:
                    if (questAdded) break;
                    var manualQuestIdTextEntry = (optionsElement as OptionsTextEntry)!;
                    if (string.IsNullOrEmpty(manualQuestIdTextEntry.textBox.Text)) break;
                    conditions.Add($"HasQuest:{manualQuestIdTextEntry.textBox.Text}");
                    break;
                case (int)OptionsIdentifiers.JojaMemberCheckbox:
                    var jojaMemberCheckbox = (optionsElement as OptionsCheckbox)!;
                    if (jojaMemberCheckbox.isChecked)
                        conditions.Add("JojaMember");
                    break;
            }
        }

        if (string.IsNullOrEmpty(tileName))
        {
            MainClass.ScreenReader.TranslateAndSay("menu-tile_data_entry-tile_name_empty", true,
                translationCategory: TranslationCategory.Menu);
            return;
        }

        tileInfo = new AccessibleTile.JsonSerializerFormat()
        {
            NameOrTranslationKey = tileName,
            X = [_tileX],
            Y = [_tileY],
            Category = category,
            WithMods = withMods.Count > 0 ? [.. withMods] : null,
            Conditions = conditions.Count > 0 ? [.. conditions] : null,
        };
    }

    public override void leftClickHeld(int x, int y)
    {
        foreach (var el in _options)
        {
            if (!el.bounds.Contains(x, y)) continue;
            if (el.greyedOut) continue;
            
            el.leftClickHeld(x, y);
            return;
        }
        
        base.leftClickHeld(x, y);
    }

    public override void releaseLeftClick(int x, int y)
    {
        foreach (var el in _options)
        {
            if (!el.bounds.Contains(x, y)) continue;
            if (el.greyedOut) continue;
            
            el.leftClickReleased(x, y);
            return;
        }
        
        base.releaseLeftClick(x, y);
    }

    public override void receiveKeyPress(Keys key)
    {
        if (TextBoxPatch.IsAnyTextBoxActive) // Suppress any key input if text box is active
            return;
        
        int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
        foreach (var el in _options)
        {
            if (!el.bounds.Contains(x, y)) continue;
            el.receiveKeyPress(key);
            break;
        }

        base.receiveKeyPress(key);
    }

    public override void update(GameTime time)
    {
        OptionsTextEntry? manualQuestIdTextEntry = null;
        OptionsDropDown? questDropDown = null;
        foreach (OptionsElement optionsElement in _options)
        {
            if (optionsElement.whichOption == (int)OptionsIdentifiers.ManualQuestIdTextBox)
            {
                manualQuestIdTextEntry = optionsElement as OptionsTextEntry;
            }
            else if (optionsElement.whichOption == (int)OptionsIdentifiers.QuestDropDown)
            {
                questDropDown = optionsElement as OptionsDropDown;
            }
        }

        if (manualQuestIdTextEntry is not null && questDropDown is not null)
        {
            manualQuestIdTextEntry.greyedOut = questDropDown.selectedOption != 0;
        }

        int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
        if (_okButton.bounds.Contains(x, y))
        {
            MainClass.ScreenReader.TranslateAndSayWithMenuChecker("common-ui-ok_button", true);
            return;
        }
        
        OptionsElementUtils.NarrateOptionsElements(_options);
    }

    public override void draw(SpriteBatch b)
    {
        Game1.DrawBox(xPositionOnScreen, yPositionOnScreen, width, height);

        foreach (var el in _options)
        {
            el.draw(b, 0, 0);
        }
        _okButton.draw(b);
        
        drawMouse(b);
    }

    private static string FarmTypeName(int whichFarm) => whichFarm switch
    {
        0 => "standard",
        1 => "riverland",
        2 => "forest",
        3 => "hill-top",
        4 => "wilderness",
        5 => "fourcorners",
        6 => "beach",
        _ => ""
    };

    private static string FarmTypeDisplayName(int whichFarm) => whichFarm switch
    {
        0 => Game1.content.LoadString("Strings\\UI:Character_FarmStandard").Split("_")[0],
        1 => Game1.content.LoadString("Strings\\UI:Character_FarmFishing").Split("_")[0],
        2 => Game1.content.LoadString("Strings\\UI:Character_FarmForaging").Split("_")[0],
        3 => Game1.content.LoadString("Strings\\UI:Character_FarmMining").Split("_")[0],
        4 => Game1.content.LoadString("Strings\\UI:Character_FarmCombat").Split("_")[0],
        5 => Game1.content.LoadString("Strings\\UI:Character_FarmFourCorners").Split("_")[0],
        6 => Game1.content.LoadString("Strings\\UI:Character_FarmBeach").Split("_")[0],
        _ => ""
    };
}