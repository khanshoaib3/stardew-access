using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using stardew_access.Patches;
using stardew_access.Tiles;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Features;

public class TileInfoMenu(int tileX, int tileY) : DialogueBox("", [MarkTileResponse, AddToUserTilesResponse, SpeakDetailedInfoResponse])
{ 
    private const string MarkTileI18NKey = "menu-tile_info-mark_tile";
    private static readonly Response MarkTileResponse = new(MarkTileI18NKey,
        Translator.Instance.Translate(MarkTileI18NKey, TranslationCategory.Menu));
    
    private const string AddToUserTilesI18NKey = "menu-tile_info-add_to_user_tiles_data";
    private static readonly Response AddToUserTilesResponse = new(AddToUserTilesI18NKey,
        Translator.Instance.Translate(AddToUserTilesI18NKey, TranslationCategory.Menu));
    
    private const string SpeakDetailedInfoI18NKey = "menu-tile_info-detailed_tile_info";
    private static readonly Response SpeakDetailedInfoResponse = new(SpeakDetailedInfoI18NKey,
        Translator.Instance.Translate(SpeakDetailedInfoI18NKey, TranslationCategory.Menu));

    private const string DeleteExistingI18NKey = "menu-tile_info-delete_existing_data";
    private static readonly Response DeleteExistingResponse = new(DeleteExistingI18NKey,
        Translator.Instance.Translate(DeleteExistingI18NKey, TranslationCategory.Menu));

    private const string EditExistingI18NKey = "menu-tile_info-edit_existing_data";
    private static readonly Response EditExistingResponse = new(EditExistingI18NKey,
        Translator.Instance.Translate(EditExistingI18NKey, TranslationCategory.Menu));

    private static readonly string DataAlreadyExistMessage =
        Translator.Instance.Translate("menu-tile_info-data_exists", TranslationCategory.Menu);

    private readonly int _tileX = tileX;
    private readonly int _tileY = tileY;
    private AccessibleTile.JsonSerializerFormat? _tempDefaultData = null;

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

        switch (responses[selectedResponse].responseKey)
        {
            case MarkTileI18NKey:
            {
                HandleTileMarkingOption();
                break;
            }
            case AddToUserTilesI18NKey:
            {
                if (UserTilesUtils.TryAndGetTileDataAt(out AccessibleTile.JsonSerializerFormat? tileData, _tileX, _tileY))
                {
                    _tempDefaultData = tileData;
                    responses =
                    [
                        EditExistingResponse, DeleteExistingResponse
                    ];
                    selectedResponse = 0;
                    dialogues =
                    [
                        DataAlreadyExistMessage
                    ];
                    DialogueBoxPatch.Cleanup();
                    break;
                }
                SetChildMenu(new TileDataEntryMenu(_tileX, _tileY));
                break;
            }
            case SpeakDetailedInfoI18NKey:
            {
                MainClass.ScreenReader.Say(
                    TileInfo.GetNameAtTileWithBlockedOrEmptyIndication(new Vector2(_tileX, _tileY)), true);

                Log.Debug($"*******************************************");
                Log.Debug($"Tile: {_tileX}x {_tileY}y");
                Log.Debug($"Back: {Game1.currentLocation?.getTileIndexAt(new Point(_tileX, _tileY), "Back")}");
                Log.Debug($"Buildings: {Game1.currentLocation?.getTileIndexAt(new Point(_tileX, _tileY), "Buildings")}");
                Log.Debug($"Paths: {Game1.currentLocation?.getTileIndexAt(new Point(_tileX, _tileY), "Paths")}");
                Log.Debug($"Front: {Game1.currentLocation?.getTileIndexAt(new Point(_tileX, _tileY), "Front")}");
                Log.Debug($"AlwaysFront: {Game1.currentLocation?.getTileIndexAt(new Point(_tileX, _tileY), "AlwaysFront")}");
                Log.Debug($"*******************************************");

                exitThisMenu();
                break;
            }
            case DeleteExistingI18NKey:
            {
                UserTilesUtils.RemoveTileDataAt(_tileX, _tileY, Game1.currentLocation.NameOrUniqueName);
                MainClass.TileManager.Initialize();
                break;
            }
            case EditExistingI18NKey:
            {
                SetChildMenu(new TileDataEntryMenu(_tileX, _tileY, _tempDefaultData));
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

        NumberSelectionMenu numberSelectionMenu = new(
            Translator.Instance.Translate("menu-tile_info-select_marking_index", TranslationCategory.Menu),
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