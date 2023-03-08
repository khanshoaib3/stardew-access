using stardew_access.Features;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using stardew_access.Patches;
using stardew_access.ScreenReader;
using Microsoft.Xna.Framework;
using StardewValley.Menus;

namespace stardew_access
{
    public class MainClass : Mod
    {
        #region Global Vars & Properties

        #pragma warning disable CS8603
        private static int prevDate = -99;
        private static ModConfig? config;
        private Harmony? harmony;
        private static IMonitor? monitor;
        private static Radar? radarFeature;
        private static StaticTiles? sTiles;
        private static IScreenReader? screenReader;
        private static IModHelper? modHelper;
        private static TileViewer? tileViewer;
        private static Warnings? warnings;
        private static ReadTile? readTile;

        internal static ModConfig Config { get => config; set => config = value; }
        public static IModHelper? ModHelper { get => modHelper; }

        public static StaticTiles STiles
        {
            get
            {
                if (sTiles == null)
                    sTiles = new StaticTiles();

                return sTiles;
            }
            set => sTiles = value;
        }
        public static Radar RadarFeature
        {
            get
            {
                if (radarFeature == null)
                    radarFeature = new Radar();

                return radarFeature;
            }
            set => radarFeature = value;
        }

        public static string hudMessageQueryKey = "";
        public static bool isNarratingHudMessage = false;
        public static bool radarDebug = false;

        public static IScreenReader ScreenReader
        {
            get
            {
                if (screenReader == null)
                    screenReader = new ScreenReaderController().Initialize();

                return screenReader;
            }

            set => screenReader = value;
        }

        public static TileViewer TileViewerFeature
        {
            get
            {
                if (tileViewer == null)
                    tileViewer = new TileViewer();
                return tileViewer;
            }
        }

        public static ReadTile ReadTileFeature
        {
            get
            {
                if (readTile == null)
                    readTile = new ReadTile();
                return readTile;
            }
        }

        public static Warnings WarningsFeature
        {
            get
            {
                if (warnings == null)
                    warnings = new Warnings();

                return warnings;
            }
        }
        #endregion

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            #region Initializations
            Config = helper.ReadConfig<ModConfig>();

            monitor = base.Monitor; // Inititalize monitor
            modHelper = helper;

            Game1.options.setGamepadMode("force_on");

            ScreenReader = new ScreenReaderController().Initialize();
            ScreenReader.Say("Initializing Stardew Access", true);

            CustomSoundEffects.Initialize();

            CustomCommands.Initialize();

            harmony = new Harmony(ModManifest.UniqueID);
            HarmonyPatches.Initialize(harmony);

            //Initialize marked locations
            for (int i = 0; i < BuildingOperations.marked.Length; i++)
            {
                BuildingOperations.marked[i] = Vector2.Zero;
            }

            for (int i = 0; i < BuildingOperations.availableBuildings.Length; i++)
            {
                BuildingOperations.availableBuildings[i] = null;
            }
            #endregion

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.onUpdateTicked;
            helper.Events.GameLoop.GameLaunched += this.onGameLaunched;
            AppDomain.CurrentDomain.DomainUnload += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;
        }

        public void OnExit(object? sender, EventArgs? e)
        {
            // This closes the connection with the screen reader, important for linux
            // Don't know if this ever gets called or not but, just in case if it does.
            if (ScreenReader != null)
                ScreenReader.CloseScreenReader();
        }

        /// <summary>Returns the Screen Reader class for other mods to use.</summary>
        public override object GetApi()
        {
            return new API();
        }

        private void onGameLaunched(object? sender, GameLaunchedEventArgs? e)
        {
            if (sTiles is not null)
                sTiles.SetupTilesDicts();
        }

        private void onUpdateTicked(object? sender, UpdateTickedEventArgs? e)
        {
            if (!Context.IsPlayerFree)
                return;

            // Narrates currently selected inventory slot
            Other.narrateCurrentSlot();

            // Narrate current location's name
            Other.narrateCurrentLocation();

            //handle TileCursor update logic
            TileViewerFeature.update();

            if (Config.Warning)
                WarningsFeature.update();

            if (Config.ReadTile)
                ReadTileFeature.update();

            if (!RadarFeature.isRunning && Config.Radar)
            {
                RadarFeature.isRunning = true;
                RadarFeature.Run();
                Task.Delay(RadarFeature.delay).ContinueWith(_ => { RadarFeature.isRunning = false; });
            }

            if (!isNarratingHudMessage)
            {
                isNarratingHudMessage = true;
                Other.narrateHudMessages();
                Task.Delay(300).ContinueWith(_ => { isNarratingHudMessage = false; });
            }

            if (Game1.player != null)
            {
                if (Game1.timeOfDay >= 600 && prevDate != CurrentPlayer.Date)
                {
                    prevDate = CurrentPlayer.Date;
                    DebugLog("Refreshing buildlist...");
                    CustomCommands.onBuildListCalled();
                }
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs? e)
        {
            if (e == null)
                return;

            #region Simulate left and right clicks
            if (Game1.activeClickableMenu != null && !TextBoxPatch.isAnyTextBoxActive)
            {
                bool isCustomizingCharacter = Game1.activeClickableMenu is CharacterCustomization || (TitleMenu.subMenu != null && TitleMenu.subMenu is CharacterCustomization);

                #region Mouse Click Simulation
                if (Config.LeftClickMainKey.JustPressed() || Config.LeftClickAlternateKey.JustPressed())
                {
                    Game1.activeClickableMenu.receiveLeftClick(Game1.getMouseX(true), Game1.getMouseY(true));
                }

                if (Config.RightClickMainKey.JustPressed() || Config.RightClickAlternateKey.JustPressed())
                {
                    Game1.activeClickableMenu.receiveRightClick(Game1.getMouseX(true), Game1.getMouseY(true));
                }
                #endregion
            }

            if (Game1.currentMinigame != null && !TextBoxPatch.isAnyTextBoxActive)
            {
                #region Mouse Click Simulation
                if (Config.LeftClickMainKey.JustPressed() || Config.LeftClickAlternateKey.JustPressed())
                {
                    Game1.currentMinigame.receiveLeftClick(Game1.getMouseX(true), Game1.getMouseY(true));
                }

                if (Config.RightClickMainKey.JustPressed() || Config.RightClickAlternateKey.JustPressed())
                {
                    Game1.currentMinigame.receiveRightClick(Game1.getMouseX(true), Game1.getMouseY(true));
                }
                #endregion
            }
            #endregion

            if (!Context.IsPlayerFree)
                return;

            // Stops the auto walk controller if any movement key(WASD) is pressed
            if (TileViewerFeature.isAutoWalking &&
            (e.Button.Equals(SButtonExtensions.ToSButton(Game1.options.moveUpButton[0]))
            || e.Button.Equals(SButtonExtensions.ToSButton(Game1.options.moveDownButton[0]))
            || e.Button.Equals(SButtonExtensions.ToSButton(Game1.options.moveLeftButton[0]))
            || e.Button.Equals(SButtonExtensions.ToSButton(Game1.options.moveRightButton[0]))))
            {
                TileViewerFeature.stopAutoWalking(wasForced: true);
            }

            // Narrate Current Location
            if (Config.LocationKey.JustPressed())
            {
                string toSpeak = $"{Game1.currentLocation.Name}";
                MainClass.ScreenReader.Say(toSpeak, true);
                return;
            }

            // Narrate Position
            if (Config.PositionKey.JustPressed())
            {
                string toSpeak;
                if (Config.VerboseCoordinates)
                {
                    toSpeak = $"X: {CurrentPlayer.PositionX}, Y: {CurrentPlayer.PositionY}";
                }
                else
                {
                    toSpeak = $"{CurrentPlayer.PositionX}, {CurrentPlayer.PositionY}";
                }

                MainClass.ScreenReader.Say(toSpeak, true);
                return;
            }

            // Narrate health and stamina
            if (Config.HealthNStaminaKey.JustPressed())
            {
                if (ModHelper == null)
                    return;

                string toSpeak;
                if (Config.HealthNStaminaInPercentage)
                    toSpeak = ModHelper.Translation.Get("manuallytriggered.healthnstamina.percent", new { health = CurrentPlayer.PercentHealth, stamina = CurrentPlayer.PercentStamina });
                else
                    toSpeak = ModHelper.Translation.Get("manuallytriggered.healthnstamina.normal", new { health = CurrentPlayer.CurrentHealth, stamina = CurrentPlayer.CurrentStamina });

                MainClass.ScreenReader.Say(toSpeak, true);
                return;
            }

            // Narrate money at hand
            if (Config.MoneyKey.JustPressed())
            {
                string toSpeak = $"You have {CurrentPlayer.Money}g";
                MainClass.ScreenReader.Say(toSpeak, true);
                return;
            }

            // Narrate time and season
            if (Config.TimeNSeasonKey.JustPressed())
            {
                string toSpeak = $"Time is {CurrentPlayer.TimeOfDay} and it is {CurrentPlayer.Day} {CurrentPlayer.Date} of {CurrentPlayer.Season}";
                MainClass.ScreenReader.Say(toSpeak, true);
                return;
            }

            // Manual read tile at player's position
            if (Config.ReadStandingTileKey.JustPressed())
            {
                ReadTileFeature.run(manuallyTriggered: true, playersPosition: true);
                return;
            }

            // Manual read tile at looking tile
            if (Config.ReadTileKey.JustPressed())
            {
                ReadTileFeature.run(manuallyTriggered: true);
                return;
            }

            // Tile viewing cursor keys
            TileViewerFeature.HandleInput();
        }

        public static void ErrorLog(string message)
        {
            if (monitor == null)
                return;

            monitor.Log(message, LogLevel.Error);
        }

        public static void InfoLog(string message)
        {
            if (monitor == null)
                return;

            monitor.Log(message, LogLevel.Info);
        }

        public static void DebugLog(string message)
        {
            if (monitor == null)
                return;

            monitor.Log(message, LogLevel.Debug);
        }
    }
}
