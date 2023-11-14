using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using stardew_access.Patches;
using stardew_access.Tiles;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace stardew_access.Features;

public class CustomTilesEditorMenu : IClickableMenu
{
    private List<OptionsElement> _options;
    private readonly int _tileX;
    private readonly int _tileY;
    private AccessibleTile.JsonSerializerFormat? _defaultData;

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

    private ClickableTextureComponent _okButton;
    
    public CustomTilesEditorMenu(int tileX, int tileY, AccessibleTile.JsonSerializerFormat? defaultData = null)
        : base(Game1.viewport.Width / 2 - (1050 + borderWidth * 2) / 2,
            Game1.viewport.Height / 2 - (700 + borderWidth * 2) / 2 - 64, 1050 + borderWidth * 2,
            700 + borderWidth * 2 + 64)
    {
        _tileX = tileX;
        _tileY = tileY;
        _defaultData = defaultData;
        _options = new();
        allClickableComponents = new();
        PopulateOptions();
        _okButton = new ClickableTextureComponent(
            new Rectangle(this.xPositionOnScreen + 50, this.yPositionOnScreen + this.height - 84, 64, 64),
            Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
        allClickableComponents.Add(_okButton);
        AssignIDs();
        snapToDefaultClickableComponent();
    }

    private void PopulateOptions()
    {
        Rectangle currentBounds = Rectangle.Empty;
        bool defaultJojaMemberCheckboxState = false;
        string defaultQuestId = "";
        string defaultFarmHouseLevel = "none";
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
                        defaultFarmHouseLevel = arg ?? "none";
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

        Rectangle GetNextBounds()
        {
            currentBounds = new Rectangle()
            {
                X = xPositionOnScreen + 50,
                Y = currentBounds == Rectangle.Empty ? yPositionOnScreen + 20 : currentBounds.Y + 80,
                Width = 400,
                Height = 50
            };
            return currentBounds;
        }

        OptionsElement headingLabel = new OptionsElement($"Tile {_tileX}x {_tileY}y in {Game1.currentLocation.NameOrUniqueName}")
        {
            style = OptionsElement.Style.OptionLabel,
            whichOption = (int)OptionsIdentifiers.HeadingLabel,
            bounds = GetNextBounds()
        };
        allClickableComponents.Add(new ClickableComponent(headingLabel.bounds, headingLabel.label));
        _options.Add(headingLabel);

        OptionsTextEntry tileNameTextEntry =
            new OptionsTextEntry($"Tile name", (int)OptionsIdentifiers.TileNameTextEntry)
            {
                bounds = GetNextBounds()
            };
        tileNameTextEntry.textBox.Text = _defaultData?.NameOrTranslationKey ?? "";
        allClickableComponents.Add(new ClickableComponent(tileNameTextEntry.bounds, tileNameTextEntry.label));
        _options.Add(tileNameTextEntry);

        List<string> categoriesList = CATEGORY.Categories.Keys.ToList();
        OptionsDropDown categoryDropDown = new OptionsDropDown($"Category", (int)OptionsIdentifiers.CategoryDropDown)
        {
            dropDownOptions = categoriesList,
            dropDownDisplayOptions = categoriesList,
            selectedOption = (_defaultData is not null) ? categoriesList.IndexOf(_defaultData.Category) : 16,
            bounds = GetNextBounds()
        };
        categoryDropDown.RecalculateBounds();
        allClickableComponents.Add(new ClickableComponent(categoryDropDown.bounds, categoryDropDown.label));
        _options.Add(categoryDropDown);

        List<string> dropDownDisplayOptions = new()
        {
            "none"
        };
        foreach (var modInfo in MainClass.ModHelper!.ModRegistry.GetAll())
        {
            if (modInfo.Manifest.UniqueID == MainClass.ModHelper.ModRegistry.ModID) continue;
            dropDownDisplayOptions.Add(modInfo.Manifest.UniqueID);
        }
        int selectedOption = 0;

        if (_defaultData != null && _defaultData.WithMods != null && _defaultData.WithMods.Length > 0)
        {
            foreach (var modsId in _defaultData.WithMods)
            {
                if (dropDownDisplayOptions.Contains(modsId)) continue;
                dropDownDisplayOptions.Insert(0, modsId);
            }

            selectedOption = dropDownDisplayOptions.IndexOf(_defaultData.WithMods[0]);
        }

        OptionsDropDown modDependencyDropDown =
            new OptionsDropDown("Mod Dependency", (int)OptionsIdentifiers.ModDependencyDropDown)
            {
                dropDownOptions = dropDownDisplayOptions,
                dropDownDisplayOptions = dropDownDisplayOptions,
                selectedOption = selectedOption,
                bounds = GetNextBounds()
            };
        modDependencyDropDown.RecalculateBounds();
        allClickableComponents.Add(new ClickableComponent(modDependencyDropDown.bounds, modDependencyDropDown.label));
        _options.Add(modDependencyDropDown);

        if (Game1.currentLocation.currentEvent != null)
        {
            // TODO add new condition checker
            string eventCheckboxLabel = Game1.currentLocation.currentEvent.isFestival
                ? $"Check for festival: {Game1.currentLocation.currentEvent.FestivalName}"
                : Game1.currentLocation.currentEvent.isWedding
                    ? $"Check for wedding"
                    : $"Check for current event with id {Game1.currentLocation.currentEvent.id}";
            OptionsCheckbox eventCheckbox =
                new OptionsCheckbox(eventCheckboxLabel, (int)OptionsIdentifiers.EventCheckbox)
                {
                    isChecked = false,
                    bounds = GetNextBounds()
                };
            allClickableComponents.Add(new ClickableComponent(eventCheckbox.bounds, eventCheckbox.label));
            _options.Add(eventCheckbox);
        }

        if (Game1.currentLocation is Farm)
        {
            OptionsCheckbox farmTypeCheckbox =
                new OptionsCheckbox($"Check for current farm type: {FarmTypeName(Game1.whichFarm)}",
                    (int)OptionsIdentifiers.FarmTypeCheckbox)
                {
                    isChecked = false,
                    bounds = GetNextBounds()
                };
            allClickableComponents.Add(new ClickableComponent(farmTypeCheckbox.bounds, farmTypeCheckbox.label));
            _options.Add(farmTypeCheckbox);
        }

        if (Game1.currentLocation is FarmHouse)
        {
            List<string> upgradeLevelList = new List<string> { "none", "0", "1", "2", "3" };
            OptionsDropDown houseUpgradeLevelDropdown = new OptionsDropDown($"Check for farm house upgrade level",
                (int)OptionsIdentifiers.FarmHouseUpgradeLevelDropDown)
            {
                bounds = GetNextBounds(),
                dropDownDisplayOptions = upgradeLevelList,
                dropDownOptions = upgradeLevelList,
                selectedOption = upgradeLevelList.IndexOf(defaultFarmHouseLevel),
            };
            allClickableComponents.Add(new ClickableComponent(houseUpgradeLevelDropdown.bounds,
                houseUpgradeLevelDropdown.label));
            _options.Add(houseUpgradeLevelDropdown);
        }

        List<string> questOptions = new List<string>
        {
            "Enter quest id manually"
        };
        foreach (var quest in Game1.player.questLog)
        {
            questOptions.Add(quest.GetName());
        }

        OptionsDropDown questDropDown =
            new OptionsDropDown("Check if player has quest", (int)OptionsIdentifiers.QuestDropDown)
            {
                dropDownOptions = questOptions,
                dropDownDisplayOptions = questOptions,
                selectedOption = 0,
                bounds = GetNextBounds()
            };
        questDropDown.RecalculateBounds();
        allClickableComponents.Add(new ClickableComponent(questDropDown.bounds, questDropDown.label));
        _options.Add(questDropDown);

        OptionsTextEntry manualQuestIdTextEntry =
            new OptionsTextEntry("Quest id", (int)OptionsIdentifiers.ManualQuestIdTextBox)
            {
                bounds = GetNextBounds(),
            };
        manualQuestIdTextEntry.textBox.Text = defaultQuestId;
        allClickableComponents.Add(new ClickableComponent(manualQuestIdTextEntry.bounds, manualQuestIdTextEntry.label));
        _options.Add(manualQuestIdTextEntry);

        OptionsCheckbox jojaMemberCheckbox =
            new OptionsCheckbox($"Check if player is Joja member", (int)OptionsIdentifiers.JojaMemberCheckbox)
            {
                isChecked = defaultJojaMemberCheckboxState,
                bounds = GetNextBounds()
            };
        allClickableComponents.Add(new ClickableComponent(jojaMemberCheckbox.bounds, jojaMemberCheckbox.label));
        _options.Add(jojaMemberCheckbox);
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
        string category = "other";
        List<string> withMods = new();
        List<string> conditions = new();
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
                    category = categoryDropDown.dropDownDisplayOptions[categoryDropDown.selectedOption];
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
            MainClass.ScreenReader.Say("Tile name cannot be empty or null", true);
            exitThisMenu();
            return;
        }

        tileInfo = new AccessibleTile.JsonSerializerFormat()
        {
            NameOrTranslationKey = tileName,
            X = new[] { _tileX },
            Y = new[] { _tileY },
            Category = category,
            WithMods = withMods.Count > 0 ? withMods.ToArray() : null,
            Conditions = conditions.Count > 0 ? conditions.ToArray() : null,
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

    private string FarmTypeName(int whichFarm) => whichFarm switch
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
}