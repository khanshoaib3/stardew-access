using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using stardew_access.Utils;
using static stardew_access.Utils.MovementHelpers;

namespace stardew_access.Features
{
	internal class GridMovement
	{
		private const int TimerInterval = 1000;
		public Boolean is_warping = false;
		public Boolean is_moving = false;

		//stop player from moving too fast
		public int minMillisecondsBetweenSteps = 300;
		readonly System.Timers.Timer timer = new();

		// Define a dictionary that maps the direction to the corresponding vector
		private readonly Dictionary<int, Vector2> directionVectors = new()
		{
			{ 0, new Vector2(0, -1) },
			{ 1, new Vector2(1, 0) },
			{ 2, new Vector2(0, 1) },
			{ 3, new Vector2(-1, 0) }
		};

		public GridMovement()
		{
			//set is_moving after x time to allow the next grid movement
			timer.Interval = minMillisecondsBetweenSteps;
			timer.Elapsed += Timer_Elapsed;
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			is_moving = false;
			//this is called if it hasn't already naturally been called by the game so the player doesn't freeze if the warp is unsuccessful
			if(is_warping) {
				this.HandleFinishedWarping(true);
			}
			timer.Stop();
		}

		public void HandleGridMovement(int direction, InputButton pressedButton)
		{
			Farmer player = Game1.player;
			GameLocation location = Game1.currentLocation;
			timer.Interval = minMillisecondsBetweenSteps - (player.addedSpeed * (minMillisecondsBetweenSteps / 9));

			MainClass.LastGridMovementButtonPressed = pressedButton;
			MainClass.LastGridMovementDirection = direction;

			if (this.is_warping == true || is_moving || Game1.IsChatting || !Game1.player.CanMove || (Game1.CurrentEvent!=null && !Game1.CurrentEvent.canMoveAfterDialogue()))
				return;

			if (HandlePlayerDirection(direction)) return;

			is_moving = true;
			timer.Start();

			Log.Debug($"Move Direction: {direction}");

			Vector2 tileLocation = player.getTileLocation();

			// Use the directionVectors dictionary to update the tileLocation
			if (directionVectors.ContainsKey(direction))
			{
				tileLocation = Vector2.Add(tileLocation, directionVectors[direction]);
			}

			Log.Debug($"Move To: {tileLocation}");

			Rectangle position = new((int)tileLocation.X * Game1.tileSize, (int)tileLocation.Y * Game1.tileSize, Game1.tileSize, Game1.tileSize);
			Warp warp = location.isCollidingWithWarpOrDoor(position, Game1.player);
			if(warp != null)
			{
				HandleWarpInteraction(warp, location, tileLocation);
			}
			else 
			{
				HandlePlayerMovement(tileLocation, direction);
			}
			CenterPlayer();
		}

		private bool HandlePlayerDirection(int direction)
		{
			if (Game1.player.FacingDirection == direction) return false;
			
			Game1.player.faceDirection(direction);
			Game1.playSound("dwop");
			is_moving = true;
			timer.Start();
			return true;
		}

		private static void HandlePlayerMovement(Vector2 tileLocation, int direction)
		{
			Farmer player = Game1.player;
			GameLocation location = Game1.currentLocation;

			PathFindController pathfinder = new(player, location, tileLocation.ToPoint(), direction);
			if (pathfinder.pathToEndPoint != null) {
				//valid point
				player.Position = tileLocation * Game1.tileSize;
				location.playTerrainSound(tileLocation);
				CenterPlayer();
			}
		}

		private void HandleWarpInteraction(Warp warp, GameLocation location, Vector2 tileLocation)
		{
			if (TileInfo.GetDoorAtTile(location, (int)tileLocation.X, (int)tileLocation.Y) is not null)
			{
				// Manually check for door and pressActionButton() method instead of warping (warping also works when the door is locked, for example it warps to the Pierre's shop before it's opening time)
				Log.Debug("Collides with Door");
				Game1.pressActionButton(Game1.GetKeyboardState(), Game1.input.GetMouseState(), Game1.input.GetGamePadState());
			}
			else
			{
				Log.Debug("Collides with Warp");

				if (location.checkAction(new Location((int)tileLocation.X * Game1.tileSize, (int)tileLocation.Y * Game1.tileSize), Game1.viewport, Game1.player))
				{
					timer.Stop();
				}
				else
				{
					//repurpose timer to wait a short period to prevent the player from spam warping
					//this prevents the door sound from going off multiple times
					//it also prevents the player from being locked up if a warp was unsuccessful
					timer.Stop();
					timer.Interval = TimerInterval;
					timer.Start();

					Game1.playSound("doorOpen");
					Game1.player.warpFarmer(warp);
					this.is_warping = true;
				}
			}
		}

		internal void PlayerWarped(object? sender, WarpedEventArgs e)
		{
			this.HandleFinishedWarping();
		}

		private void HandleFinishedWarping(bool failWarp = false)
		{
			Game1.player.canMove = true;
			this.is_moving = false;
			if (this.is_warping) {
				this.is_warping = false;

				if(failWarp) {
					Log.Debug("Failed to walk through entrance.");
				} else {
					Game1.playSound("doorClose");
				}
			}
		}
	}
}
