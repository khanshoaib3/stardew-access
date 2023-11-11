using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using stardew_access.Patches;
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

    private const int HeadingLabel = 0;
    private const int QuestDropDown = 1;
    private const int ManualQuestIdTextBox = 2;

    private ClickableTextureComponent okButton;
    
    public CustomTilesEditorMenu(int tileX, int tileY)
        : base(Game1.viewport.Width / 2 - (1000 + borderWidth * 2) / 2,
            Game1.viewport.Height / 2 - (700 + borderWidth * 2) / 2 - 64, 1000 + borderWidth * 2,
            700 + borderWidth * 2 + 64)
    {
        _tileX = tileX;
        _tileY = tileY;
        _options = new();
        allClickableComponents = new();
        PopulateOptions();
        okButton = new ClickableTextureComponent(
            new Rectangle(this.xPositionOnScreen + 50, this.yPositionOnScreen + this.height - 84, 64, 64),
            Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
        allClickableComponents.Add(okButton);
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
            whichOption = HeadingLabel,
            bounds = GetNextBounds()
        };
        allClickableComponents.Add(new ClickableComponent(headingLabel.bounds, headingLabel.label));
        _options.Add(headingLabel);
        
        OptionsTextEntry tileNameTextEntry = new OptionsTextEntry($"Tile name", -888)
        {
            bounds = GetNextBounds()
        };
        tileNameTextEntry.textBox.Text = "";
        allClickableComponents.Add(new ClickableComponent(tileNameTextEntry.bounds, tileNameTextEntry.label));
        _options.Add(tileNameTextEntry);

        OptionsDropDown categoryDropDown = new OptionsDropDown($"Category", -777)
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
            if (modInfo.Manifest.UniqueID == MainClass.ModHelper!.ModRegistry.ModID) continue;
            dropDownDisplayOptions.Add(modInfo.Manifest.UniqueID);
        }
        OptionsDropDown modDependencyDropDown = new OptionsDropDown("Mod Dependency", -666)
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
            OptionsCheckbox eventCheckbox = new OptionsCheckbox(eventCheckboxLabel, -555)
            {
                isChecked = false,
                bounds = GetNextBounds()
            };
            allClickableComponents.Add(new ClickableComponent(eventCheckbox.bounds, eventCheckbox.label));
            _options.Add(eventCheckbox);
        }

        if (Game1.currentLocation is Farm)
        {
            OptionsCheckbox farmTypeCheckbox = new OptionsCheckbox($"Check for current farm type: {FarmTypeName(Game1.whichFarm)}", -555)
            {
                isChecked = false,
                bounds = GetNextBounds()
            };
            allClickableComponents.Add(new ClickableComponent(farmTypeCheckbox.bounds, farmTypeCheckbox.label));
            _options.Add(farmTypeCheckbox);
        }

        if (Game1.currentLocation is FarmHouse)
        {
            OptionsDropDown houseUpgradeLevelDropdown = new OptionsDropDown($"Check for farm house upgrade level", -231)
            {
                bounds = GetNextBounds(),
                dropDownDisplayOptions = new List<string> { "none", "0", "1", "2", "3" },
                dropDownOptions = new List<string> { "none", "0", "1", "2", "3" },
                selectedOption = 0
            };
            allClickableComponents.Add(new ClickableComponent(houseUpgradeLevelDropdown.bounds, houseUpgradeLevelDropdown.label));
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
        OptionsDropDown questDropDown = new OptionsDropDown("Check if player has quest", QuestDropDown)
        {
            dropDownOptions = questOptions,
            dropDownDisplayOptions = questOptions,
            selectedOption = 0,
            bounds = GetNextBounds()
        };
        questDropDown.RecalculateBounds();
        allClickableComponents.Add(new ClickableComponent(questDropDown.bounds, questDropDown.label));
        _options.Add(questDropDown);

        OptionsTextEntry manualQuestIdTextEntry = new OptionsTextEntry("Quest id", ManualQuestIdTextBox)
        {
            greyedOut = false,
            bounds = GetNextBounds(),
        };
        manualQuestIdTextEntry.textBox.Text = "";
        allClickableComponents.Add(new ClickableComponent(manualQuestIdTextEntry.bounds, manualQuestIdTextEntry.label));
        _options.Add(manualQuestIdTextEntry);

        OptionsCheckbox jojaMemberCheckbox = new OptionsCheckbox($"Check if player is Joja member", -555)
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

    public override void snapToDefaultClickableComponent()
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

        if (okButton.bounds.Contains(x, y))
        {
            exitThisMenu();
        }
        
        base.receiveLeftClick(x, y, playSound);
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
        }

        base.receiveKeyPress(key);
    }

    public override void update(GameTime time)
    {
        OptionsTextEntry? manualQuestIdTextEntry = null;
        OptionsDropDown? questDropDown = null;
        foreach (OptionsElement optionsElement in _options)
        {
            if (optionsElement.whichOption == ManualQuestIdTextBox)
            {
                manualQuestIdTextEntry = optionsElement as OptionsTextEntry;
            }
            else if (optionsElement.whichOption == QuestDropDown)
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
        okButton.draw(b);
        
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