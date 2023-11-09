using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Features;

public class TileInfoMenu : DialogueBox
{ 
    // TODO i18n
    private static List<Response> _responses = new()
    {
        new Response("tile_info_menu-mark_tile", "Mark this tile"),
        new Response("tile_info_menu-add_to_custom_tiles", "Add custom tiles entry for this tile"),
        new Response("tile_info_menu-detailed_tile_info", "Speak detailed tile info"),
    };

    private readonly int _tileX;
    private readonly int _tileY;

    public TileInfoMenu(int tileX, int tileY)
        : base("", _responses)
    {
        _tileX = tileX;
        _tileY = tileY;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (GetChildMenu() != null)
        {
            GetChildMenu().receiveLeftClick(x, y, playSound);
            return;
        }

        if (transitioning) return;
        if (characterIndexInDialogue < getCurrentString().Length - 1) return;
        if (safetyTimer > 0) return;
        if (selectedResponse == -1) return;

        switch (_responses[selectedResponse].responseKey)
        {
            case "tile_info_menu-mark_tile":
            {
                HandleTileMarkingOption();
                break;
            }
            case "tile_info_menu-add_to_custom_tiles":
            {
                MainClass.ScreenReader.Say("Custom tile.", true);
                break;
            }
            case "tile_info_menu-detailed_tile_info":
            {
                MainClass.ScreenReader.Say(
                    TileInfo.GetNameAtTileWithBlockedOrEmptyIndication(new Vector2(_tileX, _tileY)), true);
                break;
            }
        }

        base.receiveLeftClick(x, y);
    }

    private void HandleTileMarkingOption()
    {
        if (Game1.currentLocation is not Farm)
        {
            MainClass.ScreenReader.TranslateAndSay("commands-tile_marking-mark-not_in_farm",
                true,
                translationCategory: TranslationCategory.CustomCommands);
            exitThisMenu();
            return;
        }

        // TODO i18n
        NumberSelectionMenu numberSelectionMenu = new NumberSelectionMenu("Select index",
            (i, _, _) => OnNumberSelect(i),
            minValue: 0,
            maxValue: 9,
            defaultNumber: 0);
        SetChildMenu(numberSelectionMenu);
        return;

        void OnNumberSelect(int i)
        {
            BuildingOperations.marked[i] = new Vector2(_tileX, _tileY);
            MainClass.ScreenReader.TranslateAndSay("commands-tile_marking-mark-location_marked", true,
                translationTokens: new
                    { x_position = Game1.player.getTileX(), y_position = Game1.player.getTileY(), index = i },
                translationCategory: TranslationCategory.CustomCommands);
            exitThisMenu();
        }
    }

    public override void receiveKeyPress(Keys key)
    {
        if (GetChildMenu() == null)
            base.receiveKeyPress(key);
        else
            GetChildMenu().receiveKeyPress(key);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        if (GetChildMenu() == null)
            base.receiveRightClick(x, y, playSound);
        else
            GetChildMenu().receiveRightClick(x, y, playSound);
    }

    public override void performHoverAction(int x, int y)
    {
        if (GetChildMenu() == null)
            base.performHoverAction(x, y);
        else
            GetChildMenu().performHoverAction(x, y);
    }

    public override void gamePadButtonHeld(Buttons b)
    {
        if (GetChildMenu() == null)
            base.gamePadButtonHeld(b);
        else
            GetChildMenu().gamePadButtonHeld(b);
    }

    public override void receiveGamePadButton(Buttons b)
    {
        if (GetChildMenu() == null)
            base.receiveGamePadButton(b);
        else 
            GetChildMenu().receiveGamePadButton(b);
    }

    public override void update(GameTime time)
    {
        if (GetChildMenu() == null)
            base.update(time);
        else
            GetChildMenu().update(time);
    }

    public override void draw(SpriteBatch b)
    {
        if (GetChildMenu() == null)
            base.draw(b);
        else
            GetChildMenu().draw(b);
    }
}