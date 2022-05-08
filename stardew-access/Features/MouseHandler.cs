using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using xTile;
using StardewValley;


namespace stardew_access.Features
{
    public class MouseHandler
    {

        private Vector2 ViewingOffset = Vector2.Zero;

        private Vector2 PlayerFacingVector
        {
            get
            {
switch (Game1.player.FacingDirection)
                {
                    case 0:
                        return new Vector2(0, - Game1.tileSize);
                    case 1:
                        return new Vector2(Game1.tileSize, 0);
                    case 2:
                        return new Vector2(0, Game1.tileSize);
                    case 3:
                        return new Vector2(-Game1.tileSize, 0);
                    default:
                        return Vector2.Zero;
                }
            }
        }

        private Vector2 PlayerPosition
        {
            get
            {
                int x = Game1.player.GetBoundingBox().Center.X - Game1.viewport.X;
                int y = Game1.player.GetBoundingBox().Center.Y - Game1.viewport.Y;
                return new Vector2(x, y);
            }
        }

        private static (int, int) GetMapTileDimensions()
        {
            Map map = Game1.currentLocation.map;
            return (map.Layers[0].LayerWidth, map.Layers[0].LayerHeight);
        }
            
        public bool MoveTileView(Vector2 delta)
        {
            Vector2 dest = this.PlayerPosition + this.PlayerFacingVector + this.ViewingOffset + delta;
            if (Utility.isOnScreen(dest, 0))
            {
                this.ViewingOffset += delta;
                return true;
            }
            return false;
        }

        private void SnapMouseToPlayer()
        {
            Vector2 snapPosition = this.PlayerPosition + this.PlayerFacingVector + this.ViewingOffset;
            Point snapPoint = new Point((int)snapPosition.X, (int)snapPosition.Y);
            if (Utility.isOnScreen(snapPoint, 0))
            Game1.setMousePosition(snapPoint.X, snapPoint.Y);
        }

public void update()
        {
            if (MainClass.Config.SnapMouse)
            this.SnapMouseToPlayer();
        }
    
    private static bool IsTileOnMap(Vector2 tile)
    {
        (int width, int height) dimensions = GetMapTileDimensions();
        if (tile.X < 0 || tile.X >= dimensions.width) return false;
        if (tile.Y < 0 || tile.Y >= dimensions.height) return false;
        return true;
    }
}
}
