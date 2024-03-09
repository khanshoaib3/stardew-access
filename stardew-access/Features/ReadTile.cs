namespace stardew_access.Features;

using Microsoft.Xna.Framework;
using Translation;
using Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

/// <summary>
/// Reads the name and information about a tile.
/// </summary>
internal class ReadTile : FeatureBase
{
    private bool _isBusy; // To pause execution of run method between fixed intervals
    private readonly int _delay; // Length of each interval (in ms)
    private bool _shouldPause; // To pause the execution
    private Vector2 _prevTile;
        
    private static ReadTile? instance;
    public new static ReadTile Instance
    {
        get
        {
            instance ??= new ReadTile();
            return instance;
        }
    }

    private ReadTile()
    {
        _isBusy = false;
        _delay = 100;
    }

    public override void Update(object? sender, UpdateTickedEventArgs e)
    {
        if (!MainClass.Config.ReadTile)
            return;
            
        if (_isBusy)
            return;

        if (_shouldPause)
            return;

        _isBusy = true;
        Run();
        Task.Delay(_delay).ContinueWith(_ => { _isBusy = false; });
    }

    public override bool OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        // Exit if in a menu
        if (Game1.activeClickableMenu != null)
        {
            #if DEBUG
            Log.Verbose("OnButtonPressed: returning due to 'Game1.activeClickableMenu' not being null AKA in a menu");
            #endif
            return false;
        }

        // Manual read tile at player's position
        if (MainClass.Config.ReadStandingTileKey.JustPressed())
        {
            ReadTile.Instance.Run(manuallyTriggered: true, playersPosition: true);
            return true;
        }

        // Manual read tile at looking tile
        if (MainClass.Config.ReadTileKey.JustPressed())
        {
            ReadTile.Instance.Run(manuallyTriggered: true);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Pauses the feature for the provided time.
    /// </summary>
    /// <param name="time">The amount of time we want to pause the execution (in ms).<br/>Default is 2500 (2.5s).</param>
    public void PauseUntil(int time = 2500)
    {
        _shouldPause = true;
        Task.Delay(time).ContinueWith(_ => { _shouldPause = false; });
    }

    /// <summary>
    /// Pauses the feature
    /// </summary>
    public void Pause()
    {
        _shouldPause = true;
    }

    /// <summary>
    /// Resumes the feature
    /// </summary>
    public void Resume()
    {
        _shouldPause = false;
    }

    public void Run(bool manuallyTriggered = false, bool playersPosition = false)
    {
        try
        {
            Vector2 tile;

            #region Get Tile
            int x, y;
            if (!playersPosition)
            {
                // Grab tile
                tile = CurrentPlayer.FacingTile;
            }
            else
            {
                // Player's standing tile
                tile = CurrentPlayer.Position;
            }
            x = (int)tile.X;
            y = (int)tile.Y;
            #endregion

            // The event with id 13 is the Haley's six heart event, the one at the beach requiring the player to find the bracelet
            if (Context.IsPlayerFree || (Game1.CurrentEvent is not null && Game1.CurrentEvent.id == "13"))
            {
                if (!manuallyTriggered && _prevTile != tile)
                {
                    MainClass.ScreenReader.PrevTextTile = "";
                }

                var currentLocation = Game1.currentLocation;
                bool isColliding = TileInfo.IsCollidingAtTile(currentLocation, x, y);

                (string? name, string? category) = TileInfo.GetNameWithCategoryNameAtTile(tile, currentLocation);

                #region Narrate toSpeak
                if (name != null)
                    if (manuallyTriggered)
                        MainClass.ScreenReader.Say(Translator.Instance.Translate("feature-read_tile-manually_triggered_info", new {tile_name = name, tile_category = category}), true);
                    else
                        MainClass.ScreenReader.SayWithTileQuery(name, x, y, true);
                if (MainClass.Config.ReadTileIndexes)
                {
                    if (manuallyTriggered)
                    {
                        ObjectTracker.Instance.GetLocationObjects(resetFocus: true);
                        var coords = (x, y);
                        Dictionary<string, int>? layerAndIndex = TileUtils.GetTileLayers(coords);

                        if (layerAndIndex is not null &&layerAndIndex.Count > 0)
                        {
                            string output = Translator.Instance.Translate("feature-read_tile-tile_indexes");
                            foreach (var kvp in layerAndIndex)
                            {
                                output += $"{kvp.Key} {kvp.Value}, ";
                            }

                            // Trim the trailing comma and space
                            output = output[..^2];

                            MainClass.ScreenReader.Say(output, false);
                        }
                        else
                        {
                            MainClass.ScreenReader.TranslateAndSay("feature-read_tile-no_tile_found", false);
                        }
                    }
                }
                #endregion

                #region Play colliding sound effect
                if (isColliding && _prevTile != tile)
                {
                    Game1.playSound("colliding");
                }
                #endregion

                if (!manuallyTriggered)
                    _prevTile = tile;
            }

        }
        catch (Exception e)
        {
            Log.Error($"Error in Read Tile:\n{e.Message}\n{e.StackTrace}");
        }
    }
}