using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;

namespace stardew_access.Features
{
    public class MouseHandler
    {

        public Vector2 ViewingOffset { get; set; } = Vector2.Zero;

        public Vector2 PlayerFacingVector
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

        public Vector2 PlayerPosition
        {
            get
            {
                int x = Game1.player.GetBoundingBox().Center.X - Game1.viewport.X;
                int y = Game1.player.GetBoundingBox().Center.Y - Game1.viewport.Y;
                return new Vector2(x, y);
            }
        }

        public void SnapMouseToPlayer()
        {
            Vector2 snapPosition = this.PlayerPosition + this.PlayerFacingVector + this.ViewingOffset;
            if (Utility.isOnScreen(snapPosition, 0))
            Game1.setMousePosition((int)snapPosition.X, (int)snapPosition.Y);
        }
    }
}
