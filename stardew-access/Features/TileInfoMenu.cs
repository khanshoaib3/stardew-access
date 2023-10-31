using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Features;

public class TileInfoMenu : IClickableMenu
{
    private DialogueBox? _dialogueBox;
    private List<Response> _responses;

    public TileInfoMenu()
        : base()
    {
        _responses = new List<Response>
        {
            new Response("tile_info_menu-mark_tile", "Mark this tile"),
            new Response("tile_info_menu-detailed_tile_info", "Speak detailed tile info"),
            new Response("tile_info_menu-add_to_custom_tiles", "Add custom tiles entry for this tile"),
        };
        _dialogueBox = new DialogueBox(Dialogue.dialogueNeutral, _responses)
        {
            isQuestion = true,
            characterDialogue = null
        };
        Game1.dialogueUp = true;
    }

    public override void receiveKeyPress(Keys key)
    {
        _dialogueBox?.receiveKeyPress(key);
        base.receiveKeyPress(key);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (_dialogueBox is not null)
        {
            if (_dialogueBox.transitioning)
                return;
            if (_dialogueBox.characterIndexInDialogue < _dialogueBox.getCurrentString().Length - 1)
            {
                _dialogueBox.characterIndexInDialogue = _dialogueBox.getCurrentString().Length - 1;
            }
            else
            {
                if (_dialogueBox.safetyTimer > 0)
                    return;
                if (_dialogueBox.selectedResponse == -1)
                    return;

                switch (_responses[_dialogueBox.selectedResponse].responseKey)
                {
                    case "tile_info_menu-mark_tile":
                    {
                        MainClass.ScreenReader.Say("Tile marked!", true);
                        _dialogueBox.receiveLeftClick(x, y);
                        closeDialogueOverride();
                        _dialogueBox = null;
                        exitThisMenu();
                        return;
                    }
                    case "tile_info_menu-detailed_tile_info":
                    {
                        MainClass.ScreenReader.Say("Detail info.", true);
                        _dialogueBox.receiveLeftClick(x, y);
                        closeDialogueOverride();
                        _dialogueBox = null;
                        exitThisMenu();
                        return;
                    }
                    case "tile_info_menu-add_to_custom_tiles":
                    {
                        MainClass.ScreenReader.Say("Custom tile.", true);
                        _dialogueBox.receiveLeftClick(x, y);
                        closeDialogueOverride();
                        _dialogueBox = null;
                        exitThisMenu();
                        return;
                    }
                }
            }

            void closeDialogueOverride()
            {
                _dialogueBox.exitThisMenu(false);
                Game1.dialogueUp = false;
                if (Game1.messagePause)
                    Game1.pauseTime = 500f;
                if (Game1.currentObjectDialogue.Count > 0)
                    Game1.currentObjectDialogue.Dequeue();
                Game1.currentDialogueCharacterIndex = 0;
                if (Game1.currentObjectDialogue.Count > 0)
                {
                    Game1.dialogueUp = true;
                    Game1.questionChoices.Clear();
                    Game1.dialogueTyping = true;
                }

                Game1.tvStation = -1;
                if (!Game1.eventUp)
                {
                    if (!Game1.isWarping)
                        Game1.player.CanMove = true;
                    Game1.player.movementDirections.Clear();
                }
                else if (Game1.currentLocation.currentEvent.CurrentCommand > 0 ||
                         Game1.currentLocation.currentEvent.specialEventVariable1)
                {
                    if (!Game1.isFestival() || !Game1.currentLocation.currentEvent.canMoveAfterDialogue())
                        ++Game1.currentLocation.currentEvent.CurrentCommand;
                    else
                        Game1.player.CanMove = true;
                }

                Game1.questionChoices.Clear();

                if (Game1.afterDialogues == null)
                    return;
                Game1.afterFadeFunction afterDialogues = Game1.afterDialogues;
                Game1.afterDialogues = (Game1.afterFadeFunction)null;
                afterDialogues();
            }
        }
        
        base.receiveLeftClick(x, y);
    }

    public override void performHoverAction(int x, int y)
    {
        _dialogueBox?.performHoverAction(x, y);
        base.performHoverAction(x, y);
    }

    public override void update(GameTime time)
    {
        _dialogueBox?.update(time);
        base.update(time);
    }

    public override void draw(SpriteBatch b)
    {
        _dialogueBox?.draw(b);
        base.draw(b);
    }
}