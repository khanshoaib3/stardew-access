using Microsoft.Xna.Framework;
using xTile;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Features
{

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
            Vector2 target = PlayerPosition;
            if (this._relativeOffsetLock)
            {
                target += this._relativeOffsetLockPosition;
            }
            else
            {
                target += PlayerFacingVector + this._viewingOffset;
            }
            return target;
        }

        /// <summary>
        /// Return the tile at the position of the tile cursor.
        /// </summary>
        /// <returns>Vector2</returns>
        public Vector2 GetViewingTile()
        {
            Vector2 position = this.GetTileCursorPosition();
            return new Vector2((int)(position.X / Game1.tileSize), (int)(position.Y / Game1.tileSize));
        }

        /// <summary>
        /// Handle keyboard input related to the tile viewer.
        /// </summary>
        public void HandleInput()
        {
            if (MainClass.Config.ToggleRelativeCursorLockKey.JustPressed())
            {
                this._relativeOffsetLock = !this._relativeOffsetLock;
                if (this._relativeOffsetLock)
                {
                    this._relativeOffsetLockPosition = PlayerFacingVector + this._viewingOffset;
                }
                else
                {
                    this._relativeOffsetLockPosition = Vector2.Zero;
                }
                MainClass.ScreenReader.Say("Relative cursor lock " + (this._relativeOffsetLock ? "enabled" : "disabled") + ".", true);
            }
            else if (MainClass.Config.TileCursorPreciseUpKey.JustPressed())
            {
                this.CursorMoveInput(new Vector2(0, -MainClass.Config.TileCursorPreciseMovementDistance), true);
            }
            else if (MainClass.Config.TileCursorPreciseRightKey.JustPressed())
            {
                this.CursorMoveInput(new Vector2(MainClass.Config.TileCursorPreciseMovementDistance, 0), true);
            }
            else if (MainClass.Config.TileCursorPreciseDownKey.JustPressed())
            {
                this.CursorMoveInput(new Vector2(0, MainClass.Config.TileCursorPreciseMovementDistance), true);
            }
            else if (MainClass.Config.TileCursorPreciseLeftKey.JustPressed())
            {
                this.CursorMoveInput(new Vector2(-MainClass.Config.TileCursorPreciseMovementDistance, 0), true);
            }
            else if (MainClass.Config.TileCursorUpKey.JustPressed())
            {
                this.CursorMoveInput(new Vector2(0, -Game1.tileSize));
            }
            else if (MainClass.Config.TileCursorRightKey.JustPressed())
            {
                this.CursorMoveInput(new Vector2(Game1.tileSize, 0));
            }
            else if (MainClass.Config.TileCursorDownKey.JustPressed())
            {
                this.CursorMoveInput(new Vector2(0, Game1.tileSize));
            }
            else if (MainClass.Config.TileCursorLeftKey.JustPressed())
            {
                this.CursorMoveInput(new Vector2(-Game1.tileSize, 0));
            }
            else if (MainClass.Config.AutoWalkToTileKey.JustPressed() && StardewModdingAPI.Context.IsPlayerFree)
            {
                this.StartAutoWalking();
            }
        }

        private void StartAutoWalking()
        {
            PathFindController controller = new(Game1.player, Game1.currentLocation, this.GetViewingTile().ToPoint(), Game1.player.FacingDirection)
            {
                allowPlayerPathingInEvent = true
            };
            if (controller.pathToEndPoint != null && controller.pathToEndPoint.Count > 0)
            {
                Game1.player.controller = controller;
                this.IsAutoWalking = true;
                this._finalTile = this.GetViewingTile();
                ReadTile.Instance.Pause();
                MainClass.ScreenReader.Say($"Moving to {this._finalTile.X}x {this._finalTile.Y}y", true);
            }
            else
            {
                MainClass.ScreenReader.Say($"Cannot move to {this._finalTile.X}x {this._finalTile.Y}y", true);
            }
        }

        /// <summary>
        /// Stop the auto walk controller and reset variables 
        /// </summary>
        /// <param name="wasForced">Narrates a message if set to true.</param>
        public void StopAutoWalking(bool wasForced = false)
        {
            this._finalTile = Vector2.Zero;
            this.IsAutoWalking = false;
            Game1.player.controller = null;
            ReadTile.Instance.Resume();
            if (wasForced)
                MainClass.ScreenReader.Say("Stopped moving", true);
        }

        private void CursorMoveInput(Vector2 delta, Boolean precise = false)
        {
            if (!TryMoveTileView(delta)) return;
            Vector2 position = this.GetTileCursorPosition();
            Vector2 tile = this.GetViewingTile();
            String? name = TileInfo.GetNameAtTile(tile);

            // Prepend the player's name if the viewing tile is occupied by the player itself
            if (CurrentPlayer.PositionX == (int)tile.X && CurrentPlayer.PositionY == (int)tile.Y)
            {
                name = $"{Game1.player.displayName}, {name}";
            }

            if (name == null)
            {
                // Report if a tile is empty or blocked if there is nothing on it
                if (TileInfo.IsCollidingAtTile(Game1.currentLocation, (int)tile.X, (int)tile.Y))
                {
                    name = "blocked";
                }
                else
                {
                    name = "empty";
                }
            }
            if (precise)
            {
                MainClass.ScreenReader.Say($"{name}, {position.X}, {position.Y}", true);
            }
            else
            {
                MainClass.ScreenReader.Say($"{name}, {(int)(position.X / Game1.tileSize)}, {(int)(position.Y / Game1.tileSize)}", true);
            }
        }

        private bool TryMoveTileView(Vector2 delta)
        {
            Vector2 dest = this.GetTileCursorPosition() + delta;
            if (!IsPositionOnMap(dest)) return false;
            if ((MainClass.Config.LimitTileCursorToScreen && Utility.isOnScreen(dest, 0)) || !MainClass.Config.LimitTileCursorToScreen)
            {
                if (this._relativeOffsetLock)
                {
                    this._relativeOffsetLockPosition += delta;
                }
                else
                {
                    this._viewingOffset += delta;
                }
                return true;
            }
            return false;
        }

        private void SnapMouseToPlayer()
        {
            Vector2 cursorPosition = this.GetTileCursorPosition();
            if (AllowMouseSnap(cursorPosition))
                // Must account for viewport here
                Game1.setMousePosition((int)cursorPosition.X - Game1.viewport.X, (int)cursorPosition.Y - Game1.viewport.Y);
        }

        /// <summary>
        /// Handle tile viewer logic.
        /// </summary>
        public override void Update()
        {
            //Reset the viewing cursor to the player when they turn or move. This will not reset the locked offset relative cursor position.
            if (this._prevFacing != PlayerFacingVector || this._prevPlayerPosition != PlayerPosition)
            {
                this._viewingOffset = Vector2.Zero;
            }
            this._prevFacing = PlayerFacingVector;
            this._prevPlayerPosition = PlayerPosition;
            if (MainClass.Config.SnapMouse)
                this.SnapMouseToPlayer();

            if (this.IsAutoWalking)
            {
                if (Vector2.Distance(this._prevTile, CurrentPlayer.Position) >= 2f)
                {
                    _prevTile = CurrentPlayer.Position;
                    Game1.player.checkForFootstep();
                }

                if (this._finalTile != Vector2.Zero && this._finalTile == CurrentPlayer.Position)
                {
                    MainClass.ScreenReader.Say("Reached destination", true);
                    this.StopAutoWalking();
                }
            }

        }

        private static bool AllowMouseSnap(Vector2 point)
        {
            // Prevent snapping if any menu is open
            if (Game1.activeClickableMenu != null) return false;

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
}
