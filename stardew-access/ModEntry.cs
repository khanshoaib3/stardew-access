using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using stardew_access.Patches;
using stardew_access.ScreenReader;
using stardew_access.Utils;
using Microsoft.Xna.Framework;

namespace stardew_access
{
    public class MainClass : Mod
    {
        #region Global Vars & Properties

        private static int prevDate = -99;
        private static ModConfig? config;
        private Harmony? harmony;
        private static IMonitor? monitor;
        private static Radar? radarFeature;
        private static IScreenReader? screenReader;
        private static IModHelper? modHelper;
        private static TileViewer? tileViewer;
        private static Warnings? warnings;
        private static ReadTile? readTile;

        internal static ModConfig Config
        {
            get => config ?? throw new InvalidOperationException("Config has not been initialized.");
            set => config = value;
        }
        public static IModHelper? ModHelper
        {
            get => modHelper;
        }

        public static Radar RadarFeature
        {
            get
            {
                radarFeature ??= new Radar();

                return radarFeature;
            }
            set => radarFeature = value;
        }

        internal static string hudMessageQueryKey = "";
        internal static bool isNarratingHudMessage = false;
        internal static bool radarDebug = false;

        public static IScreenReader ScreenReader
        {
            get
            {
                screenReader ??= ScreenReaderController.Initialize();

                return screenReader;
            }
            set => screenReader = value;
        }

        public static TileViewer TileViewerFeature
        {
            get
            {
                tileViewer ??= new TileViewer();
                return tileViewer;
            }
        }

        public static ReadTile ReadTileFeature
        {
            get
            {
                readTile ??= new ReadTile();
                return readTile;
            }
        }

        public static Warnings WarningsFeature
        {
            get
            {
                warnings ??= new Warnings();

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

            ScreenReader = ScreenReaderController.Initialize();
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

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            AppDomain.CurrentDomain.DomainUnload += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) => Translator.Instance.Initialize(ModManifest);


        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            TextBoxPatch.activeTextBoxes = "";
            if (e.OldMenu != null)
            {
                MainClass.DebugLog($"Switched from {e.OldMenu.GetType()} menu, performing cleanup...");
                IClickableMenuPatch.Cleanup(e.OldMenu);
            }
        }

        /// <summary>Returns the Screen Reader class for other mods to use.</summary>
        public override object GetApi() => new API();

        public void OnExit(object? sender, EventArgs? e)
        {
            // This closes the connection with the screen reader, important for linux
            // Don't know if this ever gets called or not but, just in case if it does.
            ScreenReader?.CloseScreenReader();
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs? e)
        {
            StaticTiles.LoadTilesFiles();
            StaticTiles.SetupTilesDicts();
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs? e)
        {
            // The event with id 13 is the Haley's six heart event, the one at the beach requiring the player to find the bracelet
            if (!Context.IsPlayerFree && !(Game1.CurrentEvent is not null && Game1.CurrentEvent.id == 13))
                return;

            // Narrates currently selected inventory slot
            GameStateNarrator.NarrateCurrentSlot();
            // Narrate current location's name
            GameStateNarrator.NarrateCurrentLocation();
            //handle TileCursor update logic
            TileViewerFeature.Update();

            if (Config.Warning)
                WarningsFeature.Update();

            if (Config.ReadTile)
                ReadTileFeature.Update();

            RunRadarFeatureIfEnabled();

            RunHudMessageNarration();

            RefreshBuildListIfRequired();

            async void RunRadarFeatureIfEnabled()
            {
                if (!RadarFeature.isRunning && Config.Radar)
                {
                    RadarFeature.isRunning = true;
                    RadarFeature.Run();
                    await Task.Delay(RadarFeature.delay);
                    RadarFeature.isRunning = false;
                }
            }

            async void RunHudMessageNarration()
            {
                if (!isNarratingHudMessage)
                {
                    isNarratingHudMessage = true;
                    GameStateNarrator.NarrateHudMessages();
                    await Task.Delay(300);
                    isNarratingHudMessage = false;
                }
            }

            void RefreshBuildListIfRequired()
            {
                if (Game1.player != null)
                {
                    if (Game1.timeOfDay >= 600 && prevDate != CurrentPlayer.Date)
                    {
                        prevDate = CurrentPlayer.Date;
                        DebugLog("Refreshing buildlist...");
                        CustomCommands.OnBuildListCalled();
                    }
                }
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs? e)
        {
            if (e == null)
                return;

            void SimulateMouseClicks(
                Action<int, int> leftClickHandler,
                Action<int, int> rightClickHandler
            )
            {
                int mouseX = Game1.getMouseX(true);
                int mouseY = Game1.getMouseY(true);

                if (
                    Config.LeftClickMainKey.JustPressed()
                    || Config.LeftClickAlternateKey.JustPressed()
                )
                {
                    leftClickHandler(mouseX, mouseY);
                }
                else if (
                    Config.RightClickMainKey.JustPressed()
                    || Config.RightClickAlternateKey.JustPressed()
                )
                {
                    rightClickHandler(mouseX, mouseY);
                }
            }

            #region Simulate left and right clicks
            if (!TextBoxPatch.IsAnyTextBoxActive)
            {
                if (Game1.activeClickableMenu != null)
                {
                    SimulateMouseClicks(
                        (x, y) => Game1.activeClickableMenu.receiveLeftClick(x, y),
                        (x, y) => Game1.activeClickableMenu.receiveRightClick(x, y)
                    );
                }
                else if (Game1.currentMinigame != null)
                {
                    SimulateMouseClicks(
                        (x, y) => Game1.currentMinigame.receiveLeftClick(x, y),
                        (x, y) => Game1.currentMinigame.receiveRightClick(x, y)
                    );
                }
            }
            #endregion

            if (!Context.IsPlayerFree)
                return;

            void Narrate(string message) => MainClass.ScreenReader.Say(message, true);

            bool IsMovementKey(SButton button)
            {
                return button.Equals(SButtonExtensions.ToSButton(Game1.options.moveUpButton[0]))
                    || button.Equals(SButtonExtensions.ToSButton(Game1.options.moveDownButton[0]))
                    || button.Equals(SButtonExtensions.ToSButton(Game1.options.moveLeftButton[0]))
                    || button.Equals(SButtonExtensions.ToSButton(Game1.options.moveRightButton[0]));
            }

            // Stops the auto walk   controller if any movement key(WASD) is pressed
            if (TileViewerFeature.isAutoWalking && IsMovementKey(e.Button))
            {
                TileViewerFeature.StopAutoWalking(wasForced: true);
            }

            // Narrate Current Location
            if (Config.LocationKey.JustPressed())
            {
                Narrate(Game1.currentLocation.Name);
                return;
            }

            // Narrate Position
            if (Config.PositionKey.JustPressed())
            {
                string toSpeak = Config.VerboseCoordinates
                    ? $"X: {CurrentPlayer.PositionX}, Y: {CurrentPlayer.PositionY}"
                    : $"{CurrentPlayer.PositionX}, {CurrentPlayer.PositionY}";
                Narrate(toSpeak);
                return;
            }

            // Narrate health and stamina
            if (Config.HealthNStaminaKey.JustPressed())
            {
                if (ModHelper == null)
                    return;

                string toSpeak = Config.HealthNStaminaInPercentage
                    ? Translator.Instance.Translate(
                        "feature-speak_health_n_stamina-in_percentage_format",
                        new
                        {
                            health = CurrentPlayer.PercentHealth,
                            stamina = CurrentPlayer.PercentStamina
                        }
                    )
                    : Translator.Instance.Translate(
                        "feature-speak_health_n_stamina-in_normal_format",
                        new
                        {
                            health = CurrentPlayer.CurrentHealth,
                            stamina = CurrentPlayer.CurrentStamina
                        }
                    );

                Narrate(toSpeak);
                return;
            }

            // Narrate money at hand
            if (Config.MoneyKey.JustPressed())
            {
                Narrate($"You have {CurrentPlayer.Money}g");
                return;
            }

            // Narrate time and season
            if (Config.TimeNSeasonKey.JustPressed())
            {
                Narrate(
                    $"Time is {CurrentPlayer.TimeOfDay} and it is {CurrentPlayer.Day} {CurrentPlayer.Date} of {CurrentPlayer.Season}"
                );
                return;
            }

            // Manual read tile at player's position
            if (Config.ReadStandingTileKey.JustPressed())
            {
                ReadTileFeature.Run(manuallyTriggered: true, playersPosition: true);
                return;
            }

            // Manual read tile at looking tile
            if (Config.ReadTileKey.JustPressed())
            {
                ReadTileFeature.Run(manuallyTriggered: true);
                return;
            }

            // Tile viewing cursor keys
            TileViewerFeature.HandleInput();
        }


        private static void LogMessage(string message, LogLevel logLevel)
        {
            if (monitor == null)
                return;

            monitor.Log(message, logLevel);
        }

        public static void ErrorLog(string message)
        {
            LogMessage(message, LogLevel.Error);
        }

        public static void InfoLog(string message)
        {
            LogMessage(message, LogLevel.Info);
        }

        public static void DebugLog(string message)
        {
            LogMessage(message, LogLevel.Debug);
        }
    }
}
