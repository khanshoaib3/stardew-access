using System;
using System.Timers;
using Microsoft.Xna.Framework;
using static stardew_access.Utils.MovementHelpers;
using StardewValley;
using xTile.Dimensions;

namespace stardew_access.Utils
{
	public sealed class Pathfinder : IDisposable
	{
		private readonly System.Timers.Timer checkPathingTimer;
		private readonly System.Timers.Timer footstepTimer;
		private readonly object pathfindingLock = new();
		private readonly Func<int, int, Vector2?, bool> retryAction;
		private readonly Action<Vector2?> stopAction;
		private Vector2? LastTargetedTile = null;
		private int pathfindingRetryAttempts = 0;
		private readonly int MaxRetryAttempts;
		private readonly int DefaultDirection;
		private readonly int CheckPointTimeout;
		private readonly int MSBetweenCheckingPathfindingController;
		public bool IsActive;

		public Pathfinder(Func<int, int, Vector2?, bool> retryAction, Action<Vector2?> stopAction, int minMillisecondsBetweenSteps = 300, int maxRetryAttempts = 5, int defaultDirection = -1, int checkPointTimeout = 500, int msBetweenCheckingPathfindingController = 1000)
		{
			this.retryAction = retryAction;
			this.stopAction = stopAction;
			this.MaxRetryAttempts = maxRetryAttempts;
			this.DefaultDirection = defaultDirection;
			this.CheckPointTimeout = checkPointTimeout;
			this.MSBetweenCheckingPathfindingController = msBetweenCheckingPathfindingController;
			checkPathingTimer = new(MSBetweenCheckingPathfindingController);
			checkPathingTimer.Elapsed += CheckPathingTimer_Elapsed;

			footstepTimer = new(minMillisecondsBetweenSteps + 50);
			footstepTimer.Elapsed += FootstepTimer_Elapsed;
		}

        void IDisposable.Dispose()
        {
			checkPathingTimer.Dispose();
			footstepTimer.Dispose();
		}

		private void FootstepTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				Farmer player = Game1.player;
				if (player.controller != null)
				{
					player.currentLocation.playTerrainSound(player.getTileLocation());
				}
			}
			catch (Exception ex)
			{
				MainClass.ErrorLog($"Unhandled exception {ex} in FootstepTimer_Elapsed.");
			}
		}

		private void CheckPathingTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				Farmer player = Game1.player;
				GameLocation location = Game1.currentLocation;
				if (player.controller != null && (Game1.activeClickableMenu == null || Game1.IsMultiplayer))
				{
					if (player.controller.timerSinceLastCheckPoint > CheckPointTimeout)
					{
						if (pathfindingRetryAttempts < MaxRetryAttempts)
						{
							pathfindingRetryAttempts++;
							MainClass.DebugLog($"Attempting to restart pathfinding; attempt {pathfindingRetryAttempts} of {MaxRetryAttempts}.");

							if (pathfindingRetryAttempts > MaxRetryAttempts)
							{
								pathfindingRetryAttempts = 0;
								IsActive = false;
								MainClass.ScreenReader.Say("Pathfinding forcibly stopped. Target Lost.", true);

								player.controller.endBehaviorFunction(player, location);
								player.controller = null;
								return;
							}
							else 							if (pathfindingRetryAttempts == 1)
							{
								MainClass.ScreenReader.Say($"Target unreachable, re-trying...", true);
							}
							bool shouldContinue = retryAction.Invoke(pathfindingRetryAttempts, MaxRetryAttempts, LastTargetedTile);
							if (!shouldContinue) pathfindingRetryAttempts = MaxRetryAttempts + 1;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MainClass.ErrorLog($"Unhandled exception {ex} in CheckPathingTimer_Elapsed.");
			}
		}

		internal void StartPathfinding(Farmer player, GameLocation location, Point targetTile, int? direction = null)
		{
			direction ??= DefaultDirection;
			lock (pathfindingLock)
			{
				IsActive = true;
				LastTargetedTile = targetTile.ToVector2();
				StopTimers();
				StartTimers();

				player.controller = new PathFindController(player, location, targetTile, direction.Value, (Character farmer, GameLocation location) =>
				{
					StopPathfinding();
				});
			}
		}

		internal void StopPathfinding()
		{
			lock (pathfindingLock)
			{
				IsActive = false;
				Farmer player = Game1.player;
				StopTimers();

				stopAction.Invoke(LastTargetedTile);

				player.controller = null;
			}
		}

		private void StartTimers()
		{
			footstepTimer.Start();
			checkPathingTimer.Start();
		}

		private void StopTimers()
		{
			footstepTimer.Stop();
			checkPathingTimer.Stop();
		}
	}
}
