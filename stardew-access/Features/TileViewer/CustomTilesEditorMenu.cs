using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Features;

public class CustomTilesEditorMenu : IClickableMenu
{
    private List<OptionsElement> _options;
    private readonly int _tileX;
    private readonly int _tileY;

    private const int HeadingLabel = 0;
    
    public CustomTilesEditorMenu(int tileX, int tileY)
        : base(Game1.viewport.Width / 2 - (632 + borderWidth * 2) / 2,
            Game1.viewport.Height / 2 - (600 + borderWidth * 2) / 2 - 64, 632 + borderWidth * 2,
            600 + borderWidth * 2 + 64)
    {
        _tileX = tileX;
        _tileY = tileY;
        _options = new();
        allClickableComponents = new();
        PopulateOptions();
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
                Width = width - 100,
                Height = 50
            };
            return currentBounds;
        }
        
        OptionsElement headingLabel = new OptionsElement($"Tile {_tileX}x {_tileY}y in {Game1.currentLocation.NameOrUniqueName}")
        {
            style = OptionsElement.Style.OptionLabel,
            whichOption = HeadingLabel,
            bounds = GetNextBounds()
        };
        allClickableComponents.Add(new ClickableComponent(headingLabel.bounds, headingLabel.label)
        {
            myID = 100,
            downNeighborID = 101
        });
        _options.Add(headingLabel);
        
        OptionsTextEntry tileNameTextEntry = new OptionsTextEntry($"Tile name", -888)
        {
            bounds = GetNextBounds()
        };
        tileNameTextEntry.textBox.Text = "";
        allClickableComponents.Add(new ClickableComponent(tileNameTextEntry.bounds, tileNameTextEntry.label)
        {
            myID = 101,
            upNeighborID = 100,
            downNeighborID = 102
        });
        _options.Add(tileNameTextEntry);

        OptionsDropDown categoryDropDown = new OptionsDropDown($"Category", -777)
        {
            dropDownOptions = CATEGORY.Categories.Keys.ToList(),
            dropDownDisplayOptions = CATEGORY.Categories.Keys.ToList(),
            selectedOption = 16,
            bounds = GetNextBounds()
        };
        categoryDropDown.RecalculateBounds();
        allClickableComponents.Add(new ClickableComponent(categoryDropDown.bounds, categoryDropDown.label)
        {
            myID = 102,
            upNeighborID = 101,
            downNeighborID = 103
        });
        _options.Add(categoryDropDown);

        OptionsDropDown modDependencyDropDown = new OptionsDropDown("Mod Dependency", -666)
        {
            dropDownOptions = MainClass.ModHelper!.ModRegistry.GetAll().Select(modInfo => modInfo.Manifest.UniqueID)
                .ToList(),
            dropDownDisplayOptions = MainClass.ModHelper!.ModRegistry.GetAll()
                .Select(modInfo => modInfo.Manifest.UniqueID).ToList(),
            selectedOption = 0,
            bounds = GetNextBounds()
        };
        modDependencyDropDown.RecalculateBounds();
        allClickableComponents.Add(new ClickableComponent(modDependencyDropDown.bounds, modDependencyDropDown.label)
        {
            myID = 103,
            upNeighborID = 102
        });
        _options.Add(modDependencyDropDown);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        foreach (var el in _options)
        {
            if (el is OptionsDropDown) continue;
            if (!el.bounds.Contains(x, y)) continue;
            
            el.receiveLeftClick(x, y);
            return;
        }
        
        base.receiveLeftClick(x, y, playSound);
    }

    public override void leftClickHeld(int x, int y)
    {
        foreach (var el in _options)
        {
            if (!el.bounds.Contains(x, y)) continue;
            
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
            
            el.leftClickReleased(x, y);
            return;
        }
        
        base.releaseLeftClick(x, y);
    }

    public override void receiveKeyPress(Keys key)
    {
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
        OptionsElementUtils.NarrateOptionsElements(_options);
    }

    public override void draw(SpriteBatch b)
    {
        Game1.DrawBox(xPositionOnScreen, yPositionOnScreen, width, height);

        foreach (var el in _options)
        {
            el.draw(b, 0, 0);
        }
        
        drawMouse(b);
    }
}