using Microsoft.Xna.Framework;
using xTile;
using stardew_access.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Pathfinding;

namespace stardew_access.Features;

/// <summary>
/// Allows browsing of the map and snapping mouse to tiles with the arrow keys
/// </summary>
internal class TileViewer : FeatureBase
{

    //None of these positions take viewport into account; other functions are responsible later
    private Vector2 _viewingOffset = Vector2.Zero;
    private Vector2 _relativeOffsetLockPosition = Vector2.Zero;
    private Boolean _relativeOffsetLock = false;
    private Vector2 _prevPlayerPosition = Vector2.Zero, _prevFacing = Vector2.Zero;
    private Vector2 _finalTile = Vector2.Zero;
    private Vector2 _prevTile = Vector2.Zero;

    public Boolean IsAutoWalking = false;
        
    private static TileViewer? instance;
    public new static TileViewer Instance
    {
        get
        {
            instance ??= new TileViewer();
            return instance;
        }
    }


    private static Vector2 PlayerFacingVector
    {
        get
        {
            return Game1.player.FacingDirection switch
            {
                0 => new Vector2(0, -Game1.tileSize),
                1 => new Vector2(Game1.tileSize, 0),
                2 => new Vector2(0, Game1.tileSize),
                3 => new Vector2(-Game1.tileSize, 0),
                _ => Vector2.Zero,
            };
        }
    }

    private static Vector2 PlayerPosition
    {
        get
        {
            int x = Game1.player.GetBoundingBox().Center.X;
            int y = Game1.player.GetBoundingBox().Center.Y;
            return new Vector2(x, y);
        }
    }

    /// <summary>
    /// Return the position of the tile cursor in pixels from the upper-left corner of the map.
    /// </summary>
    /// <returns>Vector2</returns>
    public Vector2 GetTileCursorPosition()
    {
        if (IsCarpenterMenuBuilderViewport())
        {    
            return new Vector2(Game1.viewport.X / 64, Game1.viewport.Y / 64);
            //return new Vector2((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
        }
        Vector2 target = PlayerPosition;
        if (_relativeOffsetLock)
        {
            target += _relativeOffsetLockPosition;
        }
        else
        {
            target += PlayerFacingVector + _viewingOffset;
        }
        return target;
    }

    /// <summary>
    /// Return the tile at the position of the tile cursor.
    /// </summary>
    /// <returns>Vector2</returns>
    public Vector2 GetViewingTile()
    {
        Vector2 position = GetTileCursorPosition();
        return new Vector2((int)(position.X / Game1.tileSize), (int)(position.Y / Game1.tileSize));
    }

    private static bool IsCarpenterMenuBuilderViewport()
    {
        if (Game1.activeClickableMenu is CarpenterMenu menu)
        {
            return menu.onFarm;
        }
        return false;
    }

    /// <summary>
    /// Handle keyboard input related to the tile viewer.
    /// </summary>
    public override bool OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        // Exit if in a menu
        if (Game1.activeClickableMenu != null && !IsCarpenterMenuBuilderViewport())
        {
            return false;
        }

        if (Game1.activeClickableMenu != null && MainClass.Config.ToggleRelativeCursorLockKey.JustPressed())
        {
            _relativeOffsetLock = !_relativeOffsetLock;
            if (_relativeOffsetLock)
            {
                _relativeOffsetLockPosition = PlayerFacingVector + _viewingOffset;
            }
            else
            {
                _relativeOffsetLockPosition = Vector2.Zero;
            }

            MainClass.ScreenReader.TranslateAndSay("feature-tile_viewer-relative_cursor_lock_info", true, new
            {
                is_enabled = _relativeOffsetLock ? 1 : 0
            });
        }
        else if (MainClass.Config.TileCursorPreciseUpKey.JustPressed())
        {
            CursorMoveInput(new Vector2(0, -MainClass.Config.TileCursorPreciseMovementDistance), true);
        }
        else if (MainClass.Config.TileCursorPreciseRightKey.JustPressed())
        {
            CursorMoveInput(new Vector2(MainClass.Config.TileCursorPreciseMovementDistance, 0), true);
        }
        else if (MainClass.Config.TileCursorPreciseDownKey.JustPressed())
        {
            CursorMoveInput(new Vector2(0, MainClass.Config.TileCursorPreciseMovementDistance), true);
        }
        else if (MainClass.Config.TileCursorPreciseLeftKey.JustPressed())
        {
            CursorMoveInput(new Vector2(-MainClass.Config.TileCursorPreciseMovementDistance, 0), true);
        }
        else if (MainClass.Config.TileCursorUpKey.JustPressed())
        {
            CursorMoveInput(new Vector2(0, -Game1.tileSize));
        }
        else if (MainClass.Config.TileCursorRightKey.JustPressed())
        {
            CursorMoveInput(new Vector2(Game1.tileSize, 0));
        }
        else if (MainClass.Config.TileCursorDownKey.JustPressed())
        {
            CursorMoveInput(new Vector2(0, Game1.tileSize));
        }
        else if (MainClass.Config.TileCursorLeftKey.JustPressed())
        {
            CursorMoveInput(new Vector2(-Game1.tileSize, 0));
        }
        else if (MainClass.Config.AutoWalkToTileKey.JustPressed() && Context.IsPlayerFree)
        {
            StartAutoWalking();
        }
        else if (Game1.activeClickableMenu != null && MainClass.Config.OpenTileInfoMenuKey.JustPressed() && Context.IsPlayerFree)
        {
            Game1.activeClickableMenu = new TileInfoMenu((int)GetViewingTile().X, (int)GetViewingTile().Y);
        }

        // Suppresses button presses (excluding certain buttons) if tile viewer is path finding
        if (Game1.player.controller is null) return false;
        if (!Instance.IsAutoWalking) return false;
        
        if (MainClass.Config.OTCancelAutoWalking.JustPressed())
        {
            #if DEBUG
            Log.Verbose(
                "OnButtonPressed: cancel auto walking button pressed, canceling auto walking for tile viewer.");
            #endif
            StopAutoWalking(wasForced: true);
            MainClass.ModHelper!.Input.Suppress(e.Button);
        }
        else if (InputUtils.IsAnyMovementKey(e.Button))
        {
            #if DEBUG
            Log.Verbose("OnButtonPressed: movement key pressed, canceling auto walking for tile viewer.");
            #endif
            StopAutoWalking(wasForced: true);
        }
        else if (SButtonExtensions.IsUseToolButton(e.Button))
        {
            #if DEBUG
            Log.Verbose(
                "OnButtonPressed: use tool button pressed, canceling auto walking for tile viewer.");
            #endif
            StopAutoWalking(wasForced: true);
            Game1.pressUseToolButton();
        }
        else if (SButtonExtensions.IsActionButton(e.Button))
        {
            #if DEBUG
            Log.Verbose("OnButtonPressed: action button pressed, canceling auto walking for tile viewer.");
            #endif
            StopAutoWalking(wasForced: true);
            Game1.pressActionButton(Game1.input.GetKeyboardState(), Game1.input.GetMouseState(),
                Game1.input.GetGamePadState());
        }

        if (!InputUtils.IsAnyInventorySlotButton(e.Button)
            && !InputUtils.IsToolbarSwapButton(e.Button)
            && !e.Button.Equals(SButton.LeftControl))
        {
            #if DEBUG
            Log.Verbose(
                $"OnButtonPressed: suppressing '{e.Button}' for object tracker/tile viewer auto walking as it is neither any inventory slot button nor the toolbar swap button");
            #endif
            MainClass.ModHelper!.Input.Suppress(e.Button);
        }

        return true;
    }

    private void StartAutoWalking()
    {
        PathFindController controller = new(Game1.player, Game1.currentLocation, GetViewingTile().ToPoint(), Game1.player.FacingDirection)
        {
            allowPlayerPathingInEvent = true
        };
        if (controller.pathToEndPoint != null && controller.pathToEndPoint.Count > 0)
        {
            Game1.player.controller = controller;
            IsAutoWalking = true;
            _finalTile = GetViewingTile();
            ReadTile.Instance.Pause();
            MainClass.ScreenReader.TranslateAndSay("feature-tile_viewer-moving_to", true, new
            {
                tile_x = _finalTile.X,
                tile_y = _finalTile.Y
            });
        }
        else
        {
            _finalTile = GetViewingTile();
            MainClass.ScreenReader.TranslateAndSay("feature-tile_viewer-cannot_move_to", true, new
            {
                tile_x = _finalTile.X,
                tile_y = _finalTile.Y
            });
        }
    }

    /// <summary>
    /// Stop the auto walk controller and reset variables 
    /// </summary>
    /// <param name="wasForced">Narrates a message if set to true.</param>
    public void StopAutoWalking(bool wasForced = false)
    {
        _finalTile = Vector2.Zero;
        IsAutoWalking = false;
        Game1.player.controller = null;
        ReadTile.Instance.Resume();
        if (wasForced)
            MainClass.ScreenReader.TranslateAndSay("feature-tile_viewer-stopped_moving", true);
    }

    private void CursorMoveInput(Vector2 delta, Boolean precise = false)
    {
        bool isCarpenterMenu = IsCarpenterMenuBuilderViewport();
        if (isCarpenterMenu) Game1.panScreen((int)delta.X, (int)delta.Y);
        else if (!TryMoveTileView(delta)) return;
        Vector2 position = isCarpenterMenu ? new Vector2(Game1.viewport.X, Game1.viewport.Y) : GetTileCursorPosition();
        string name = TileInfo.GetNameAtTileWithBlockedOrEmptyIndication(GetViewingTile());
        
        MainClass.ScreenReader.Say(precise
            ? $"{name}, {position.X}, {position.Y}"
            : $"{name}, {(int)(position.X / Game1.tileSize)}, {(int)(position.Y / Game1.tileSize)}", true);
    }

    private bool TryMoveTileView(Vector2 delta)
    {
        Vector2 dest = GetTileCursorPosition() + delta;
        if (!IsPositionOnMap(dest)) return false;
        if ((MainClass.Config.LimitTileCursorToScreen && Utility.isOnScreen(dest, 0)) || !MainClass.Config.LimitTileCursorToScreen)
        {
            if (_relativeOffsetLock)
            {
                _relativeOffsetLockPosition += delta;
            }
            else
            {
                _viewingOffset += delta;
            }
            return true;
        }
        return false;
    }

    private void SnapMouseToPlayer()
    {
        Vector2 cursorPosition = GetTileCursorPosition();
        if (AllowMouseSnap(cursorPosition))
            // Must account for viewport here
            Game1.setMousePosition((int)cursorPosition.X - Game1.viewport.X, (int)cursorPosition.Y - Game1.viewport.Y);
    }

    /// <summary>
    /// Handle tile viewer logic.
    /// </summary>
    public override void Update(object? sender, UpdateTickedEventArgs e)
    {
        //Reset the viewing cursor to the player when they turn or move. This will not reset the locked offset relative cursor position.
        if (_prevFacing != PlayerFacingVector || _prevPlayerPosition != PlayerPosition)
        {
            _viewingOffset = Vector2.Zero;
        }
        _prevFacing = PlayerFacingVector;
        _prevPlayerPosition = PlayerPosition;
        if (MainClass.Config.SnapMouse)
            SnapMouseToPlayer();

        if (!IsAutoWalking) return;
        if (Game1.activeClickableMenu != null)
        {
            StopAutoWalking();
            return;
        }
        
        if (Vector2.Distance(_prevTile, CurrentPlayer.Position) >= 2f)
        {
            _prevTile = CurrentPlayer.Position;
            Game1.player.checkForFootstep();
        }

        if (_finalTile != Vector2.Zero && _finalTile == CurrentPlayer.Position)
        {
            MainClass.ScreenReader.TranslateAndSay("feature-tile_viewer-reached", true);
            StopAutoWalking();
        }
    }

    private static bool AllowMouseSnap(Vector2 point)
    {
        // Prevent snapping if any menu is open or if window loses focus
        if (!Game1.game1.HasKeyboardFocus() && Game1.activeClickableMenu != null && !IsCarpenterMenuBuilderViewport()) return false;

        // Utility.isOnScreen treats a vector as a pixel position, not a tile position
        if (!Utility.isOnScreen(point, 0)) return false;

        //prevent mousing over the toolbar or any other UI component with the tile cursor
        foreach (IClickableMenu menu in Game1.onScreenMenus)
        {
            //must account for viewport here
            if (menu.isWithinBounds((int)point.X - Game1.viewport.X, (int)point.Y - Game1.viewport.Y)) return false;
        }
        return true;
    }

    private static bool IsPositionOnMap(Vector2 position)
    {
        var currentLocation = Game1.currentLocation;
        // Check whether the position is a warp point, if so then return true, sometimes warp points are 1 tile off the map for example in coops and barns
        if (DoorUtils.IsWarpAtTile(((int)(position.X / Game1.tileSize), (int)(position.Y / Game1.tileSize)), currentLocation)) return true;

        //position does not take viewport into account since the entire map needs to be checked.
        Map map = currentLocation.map;
        if (position.X < 0 || position.X > map.Layers[0].DisplayWidth) return false;
        if (position.Y < 0 || position.Y > map.Layers[0].DisplayHeight) return false;
        return true;
    }
}
