using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using stardew_access.Patches;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace stardew_access.Features;

public class CustomTilesEditorMenu : IClickableMenu
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // ReSharper disable UnusedAutoPropertyAccessor.Local
    private class TileEntryFormat
    {
        public string NameOrTranslationKey { get; set; }
        public int[] X { get; set; }
        public int[] Y { get; set; }
        public string Category { get; set; }
        public string[]? WithMods { get; set; }
        public string[]? Conditions { get; set; }
    }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    private List<OptionsElement> _options;
    private readonly int _tileX;
    private readonly int _tileY;

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
    
    public CustomTilesEditorMenu(int tileX, int tileY)
        : base(Game1.viewport.Width / 2 - (1050 + borderWidth * 2) / 2,
            Game1.viewport.Height / 2 - (700 + borderWidth * 2) / 2 - 64, 1050 + borderWidth * 2,
            700 + borderWidth * 2 + 64)
    {
        _tileX = tileX;
        _tileY = tileY;
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

        OptionsElement headingLabel = new OptionsElement($"Tile {_tileX}x {_tileY}y in {Game1.currentLocation.Name}")
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
        tileNameTextEntry.textBox.Text = "";
        allClickableComponents.Add(new ClickableComponent(tileNameTextEntry.bounds, tileNameTextEntry.label));
        _options.Add(tileNameTextEntry);

        OptionsDropDown categoryDropDown = new OptionsDropDown($"Category", (int)OptionsIdentifiers.CategoryDropDown)
        {
            dropDownOptions = CATEGORY.Categories.Keys.ToList(),
            dropDownDisplayOptions = CATEGORY.Categories.Keys.ToList(),
            selectedOption = 16,
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

        OptionsDropDown modDependencyDropDown =
            new OptionsDropDown("Mod Dependency", (int)OptionsIdentifiers.ModDependencyDropDown)
            {
                dropDownOptions = dropDownDisplayOptions,
                dropDownDisplayOptions = dropDownDisplayOptions,
                selectedOption = 0,
                bounds = GetNextBounds()
            };
        modDependencyDropDown.RecalculateBounds();
        allClickableComponents.Add(new ClickableComponent(modDependencyDropDown.bounds, modDependencyDropDown.label));
        _options.Add(modDependencyDropDown);

        if (Game1.currentLocation.currentEvent != null)
        {
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
            OptionsDropDown houseUpgradeLevelDropdown = new OptionsDropDown($"Check for farm house upgrade level",
                (int)OptionsIdentifiers.FarmHouseUpgradeLevelDropDown)
            {
                bounds = GetNextBounds(),
                dropDownDisplayOptions = new List<string> { "none", "0", "1", "2", "3" },
                dropDownOptions = new List<string> { "none", "0", "1", "2", "3" },
                selectedOption = 0,
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
        manualQuestIdTextEntry.textBox.Text = "";
        allClickableComponents.Add(new ClickableComponent(manualQuestIdTextEntry.bounds, manualQuestIdTextEntry.label));
        _options.Add(manualQuestIdTextEntry);

        OptionsCheckbox jojaMemberCheckbox =
            new OptionsCheckbox($"Check if player is Joja member", (int)OptionsIdentifiers.JojaMemberCheckbox)
            {
                isChecked = false,
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

        string? tileName = null;
        string category = "other";
        List<string> withMods = new();
        List<string> conditions = new();

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
            return;
        }

        var jsonEntryForTile = new TileEntryFormat
        {
            NameOrTranslationKey = tileName,
            X = new []{_tileX},
            Y = new []{_tileY},
            Category = category,
            WithMods = withMods.Count > 0 ? withMods.ToArray() : null,
            Conditions = conditions.Count > 0? conditions.ToArray() : null,
        };

        JObject root;
        if (File.Exists(JsonLoader.GetFilePath("tiles_user.json", "assets/TileData")))
        {
            Log.Trace("Loading existing tiles_user.json");
            if (!JsonLoader.TryLoadJsonFile("tiles_user.json", out JsonDocument jsonDocument, "assets/TileData"))
            {
                MainClass.ScreenReader.Say("Unable to parse tiles_user.json", true);
                return;
            }

            root = JObject.Parse(JsonSerializer.Serialize(jsonDocument));
        }
        else
        {
            Log.Trace("tiles_user.json not found, creating a new one...");
            root = new JObject();
        }

        if (!root.TryGetValue(Game1.currentLocation.Name, out JToken? locationToken))
        {
            // Creates the location property if not exists
            Log.Trace($"Entry for location {Game1.currentLocation.Name} not found, adding one...");
            locationToken = new JProperty(Game1.currentLocation.Name, new JArray());
            root.Add(locationToken);
        }

        JArray locationValueArray = locationToken.Type == JTokenType.Property
            ? (JArray)locationToken.Value<JProperty>()!.Value
            : locationToken.Value<JArray>()!;

        locationValueArray.Add(JObject.FromObject(jsonEntryForTile));
        
        JsonLoader.SaveJsonFile("tiles_user.json", JsonSerializer.Deserialize<JsonElement>(root.ToString()), "assets/TileData");
        exitThisMenu();
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