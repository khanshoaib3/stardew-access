using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Pathfinding;
using xTile.Dimensions;


namespace stardew_access.Features;
using Utils;
using static Utils.MovementHelpers;
using System.Timers;

internal class GridMovement : FeatureBase
{
    private const int TimerInterval = 1000;
    // These aren't backwards! Higher number means slower speed.
    // The 0-100% span is over the difference ((SpeedMinimum - SpeedMaximum) / 100)
    private const int SpeedMinimum = 2000;
    private const int SpeedMaximum = 1040;
    private int StepCounter = 0;
    private readonly int tilesPerStep = MainClass.Config.GridMovementTilesPerStep;
    public Boolean is_warping = false;
    public Boolean is_moving = false;

    //stop player from moving too fast
    //public int minMillisecondsBetweenSteps = 300;
    readonly Timer timer = new();

    private static int? LastGridMovementDirection = null;
    private static InputButton? LastGridMovementButtonPressed = null;

    // Define a dictionary that maps the direction to the corresponding vector
    private readonly Dictionary<int, Vector2> directionVectors = new()
    {
        { 0, new Vector2(0, -1) },
        { 1, new Vector2(1, 0) },
        { 2, new Vector2(0, 1) },
        { 3, new Vector2(-1, 0) }
    };

    private static GridMovement? instance;

    public new static GridMovement Instance
    {
        get
        {
            instance ??= new GridMovement();
            return instance;
        }
    }

    public GridMovement()
    {
        //set is_moving after x time to allow the next grid movement
        timer.Interval = SpeedMaximum;
        timer.Elapsed += Timer_Elapsed;
    }

    public override void Update(object? sender, UpdateTickedEventArgs e)
    {
        // The event with id 13 is the Haley's six heart event, the one at the beach requiring the player to find the bracelet
        // *** Exiting here will cause GridMovement and ObjectTracker functionality to not work during this event, making the bracelet impossible to track ***
        if (!Context.IsPlayerFree && !(Game1.CurrentEvent is not null && Game1.CurrentEvent.id == "13"))
            return;

        Farmer player = Game1.player;
        if (MainClass.ModHelper == null) return;
        if (!LastGridMovementButtonPressed.HasValue) return;

        SButton button = LastGridMovementButtonPressed.Value.ToSButton();
        bool isButtonDown = MainClass.ModHelper.Input.IsDown(button) ||
                            MainClass.ModHelper.Input.IsSuppressed(button);
        bool? isGridMovementActive = MainClass.Config?.GridMovementActive;
        bool? isGridMovementMoving = is_moving;

        if (LastGridMovementDirection is not null && Game1.activeClickableMenu == null &&
            isGridMovementActive == true && isGridMovementMoving == false &&
            MainClass.Config?.GridMovementOverrideKey.IsDown() == false && isButtonDown)
        {
            timer.Interval = (SpeedMinimum - (SpeedMinimum - SpeedMaximum) * (MainClass.Config.GridMovementSpeedPercent / 100d)) / player.getMovementSpeed();
            HandleGridMovement(LastGridMovementDirection.Value, LastGridMovementButtonPressed.Value);
        }
    }

    public override bool OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        // Exit if in a menu
        if (Game1.activeClickableMenu != null)
        {
            return false;
        }

        // GridMovement 
        if (is_warping)
        {
            MainClass.ModHelper!.Input.Suppress(e.Button);
            return true;
        }

        if (!Context.CanPlayerMove)
        {
            return true;
        }

        if (MainClass.Config.GridMovementOverrideKey.IsDown())
        {
            return true;
        }

        if (!MainClass.Config.GridMovementActive)
        {
            return true;
        }

        e.Button.TryGetStardewInput(out InputButton keyboardButton);
        e.Button.TryGetController(out Buttons controllerButton);

        var directionMappings = new Dictionary<(InputButton, Buttons), int>
        {
            { (Game1.options.moveUpButton[0], Buttons.DPadUp), 0 },
            { (Game1.options.moveRightButton[0], Buttons.DPadRight), 1 },
            { (Game1.options.moveDownButton[0], Buttons.DPadDown), 2 },
            { (Game1.options.moveLeftButton[0], Buttons.DPadLeft), 3 }
        };

        foreach (var mapping in directionMappings)
        {
            if (keyboardButton.Equals(mapping.Key.Item1) || controllerButton.Equals(mapping.Key.Item2))
            {
                // This effects the time between changing direction / first step and subsequent steps
                // Interval will be updated after first step if key is held.
                if (!timer.Enabled)
                    timer.Interval = MainClass.Config.GridMovementDelayAfterFirstStep;
                else if (!is_warping)
                {
                    is_moving = false;
                    timer.Stop();
                }
                HandleGridMovement(mapping.Value, keyboardButton);
                MainClass.ModHelper!.Input.Suppress(e.Button);
                break;
            }
        }

        return false;
    }

    public override void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree)
            return;

        if (MainClass.Config.ToggleGridMovementKey.JustPressed())
        {
            MainClass.Config.GridMovementActive = !MainClass.Config.GridMovementActive;
            MainClass.ScreenReader.TranslateAndSay("feature-grid_movement_status", true, new { is_active = MainClass.Config.GridMovementActive ? 1 : 0 });
            return;
        }

        base.OnButtonsChanged(sender, e);
    }

    public override void OnPlayerWarped(object? sender, WarpedEventArgs e)
    {
        HandleFinishedWarping();
        StepCounter = 0;
    }

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        is_moving = false;
        //this is called if it hasn't already naturally been called by the game so the player doesn't freeze if the warp is unsuccessful
        if (is_warping)
        {
            HandleFinishedWarping(true);
        }
        timer.Stop();
    }

    public void HandleGridMovement(int direction, InputButton pressedButton)
    {
        GameLocation location = Game1.currentLocation;
        Farmer player = Game1.player;

        LastGridMovementButtonPressed = pressedButton;
        LastGridMovementDirection = direction;

        if (is_warping == true || is_moving || Game1.IsChatting || !Game1.player.CanMove || (Game1.CurrentEvent != null && !Game1.CurrentEvent.canMoveAfterDialogue()))
            return;

        if (HandlePlayerDirection(direction)) return;

        is_moving = true;
        timer.Start();

        Log.Debug($"Move Direction: {direction}");

        Vector2 tileLocation = player.Tile;

        // Use the directionVectors dictionary to update the tileLocation
        if (directionVectors.ContainsKey(direction))
        {
            tileLocation = Vector2.Add(tileLocation, directionVectors[direction]);
        }

        // if (Game1.currentLocation.isCollidingPosition(CurrentPlayer.FacingTileBoundingBox, Game1.viewport, isFarmer: true, -1, glider: false, Game1.player))
        if (TileInfo.IsCollidingAtTile(Game1.currentLocation, CurrentPlayer.FacingTile))
        {
            return;
        }

#if DEBUG
        Log.Verbose($"Move To: {tileLocation}");
#endif

        Rectangle position = new((int)tileLocation.X * Game1.tileSize, (int)tileLocation.Y * Game1.tileSize, Game1.tileSize, Game1.tileSize);
        Warp warp = location.isCollidingWithWarpOrDoor(position, Game1.player);
        if (warp != null)
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

    private void HandlePlayerMovement(Vector2 tileLocation, int direction)
    {
        Farmer player = Game1.player;
        GameLocation location = Game1.currentLocation;

        try
        {
            PathFindController pathfinder = new(player, location, tileLocation.ToPoint(), direction);
            if (pathfinder.pathToEndPoint != null)
            {
                //valid point
                player.Position = tileLocation * Game1.tileSize;
                if (++StepCounter % tilesPerStep == 0)
                    location.playTerrainSound(tileLocation);
                CenterPlayer();
            }
        }
        catch (System.Exception ex)
        {
            Log.Debug($"[GridMovement] Pathfinding failed: {ex.Message}");
        }
    }

    private void HandleWarpInteraction(Warp warp, GameLocation location, Vector2 tileLocation)
    {
#if DEBUG
        Log.Verbose($"GridMovement.HandleWarpInteraction: Handling Warp {warp} from location {location} at {tileLocation}");
#endif
        if (TileInfo.GetDoorAtTile(location, (int)tileLocation.X, (int)tileLocation.Y, true, true) is not null
            || DynamicTiles.GetDynamicTileAt(location, (int)tileLocation.X, (int)tileLocation.Y, lessInfo: true).category == CATEGORY.Doors)
        {
            // Manually check for door and pressActionButton() method instead of warping (warping also works when the door is locked, for example it warps to the Pierre's shop before it's opening time)
#if DEBUG
            Log.Verbose("Collides with Door");
#endif
            Game1.pressActionButton(Game1.GetKeyboardState(), Game1.input.GetMouseState(), Game1.input.GetGamePadState());
        }
        else
        {
#if DEBUG
            Log.Verbose("Collides with Warp");
#endif

            // Check if player will be out of bounds at the warp point while an event is up,
            // In the case of festivals, the end festival dialogue appears,
            // but it isn't triggered by 'location.checkAction' rather than by 'location.checkCollision'.
            var position = new Rectangle((int)tileLocation.X * Game1.tileSize, (int)tileLocation.Y * Game1.tileSize, Game1.tileSize / 2, Game1.tileSize / 2);
            if (location.IsOutOfBounds(position) && Game1.eventUp)
            {
#if DEBUG
                Log.Verbose("Player will be out of bounds at the warp's position.");
#endif
                bool? isFestival = location.currentEvent?.isFestival;
                if (isFestival.HasValue && isFestival.GetValueOrDefault())
                {
#if DEBUG
                    Log.Verbose("A festival is up, checking for collision event.");
#endif
                    if (location.currentEvent?.checkForCollision(position, Game1.player) ?? false)
                        return;
                }
            }

            var tileLocation1 = new Location((int)tileLocation.X * Game1.tileSize, (int)tileLocation.Y * Game1.tileSize);
            if (location.checkAction(tileLocation1, Game1.viewport, Game1.player))
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

                // TODO Replace with custom sound
                Game1.playSound("doorOpen");
                Game1.player.warpFarmer(warp);
                is_warping = true;
            }
        }
    }

    private void HandleFinishedWarping(bool failWarp = false)
    {
        Game1.player.canMove = true;
        is_moving = false;
        if (is_warping)
        {
            is_warping = false;

            if (failWarp)
            {
                Log.Debug("Failed to walk through entrance.");
            }
            else
            {
                // TODO Replace with custom sound
                Game1.playSound("doorClose");
            }
        }
    }
}
