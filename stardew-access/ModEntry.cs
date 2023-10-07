using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using stardew_access.Features;
using stardew_access.Patches;
using stardew_access.ScreenReader;
using stardew_access.Tiles;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace stardew_access
{
    public class MainClass : Mod
    {
        #region Global Vars & Properties

        private static int prevDate = -99;
        private static LocalizedContentManager.LanguageCode previousLanguageCode;
        private static bool FirstRun = true;
        private static ModConfig? config;
        private static IScreenReader? screenReader;
        private static IModHelper? modHelper;
        private static ObjectTracker? objectTracker;

        internal static ModConfig Config
        {
            get => config ?? throw new InvalidOperationException("Config has not been initialized.");
            set => config = value;
        }
        internal static IModHelper? ModHelper => modHelper;

        internal static IScreenReader ScreenReader
        {
            get
            {
                screenReader ??= ScreenReaderController.Initialize();

                return screenReader;
            }
            set => screenReader = value;
        }

        internal static AccessibleTileManager TileManager
        {
            get
            {
                return AccessibleTileManager.Instance;
            }
        }

        internal static ObjectTracker ObjectTrackerFeature
        {
            get
            {
                objectTracker ??= new ObjectTracker();
                return objectTracker;
            }
        }
        internal static Boolean IsUsingPathfinding = false;

        #endregion

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            #region Initializations
            Log.Init(base.Monitor); // Initialize monitor
            #if DEBUG
            Log.Verbose("Initializing Stardew-Access");
            #endif

            Config = helper.ReadConfig<ModConfig>();
            modHelper = helper;

            Game1.options.setGamepadMode("force_on");

            CustomFluentFunctions.RegisterLanguageHelper("en", typeof(EnglishHelper));
            ScreenReader = ScreenReaderController.Initialize();
            ScreenReader.Say("Initializing Stardew Access", true);

            CustomSoundEffects.Initialize();

            CustomCommands.Initialize();

            PatchManager.PatchAll(new Harmony(ModManifest.UniqueID));

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

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.Player.Warped += OnPlayerWarped;
            helper.Events.Display.Rendering += OnRenderingStart;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            AppDomain.CurrentDomain.DomainUnload += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;
        }

        /// <summary>Returns the Screen Reader class for other mods to use.</summary>
        public override object GetApi() => new API();

        public void OnExit(object? sender, EventArgs? e)
        {
            // This closes the connection with the screen reader, important for linux
            // Don't know if this ever gets called or not but, just in case if it does.
            ScreenReader?.CloseScreenReader();
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs? e)
        {
            TileManager.Initialize();
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs? e)
        {
            ObjectTrackerFeature.GetLocationObjects();
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            TileManager.EnsureLocationLoaded(Game1.currentLocation);
            RefreshTranslationsOnLocaleChange();
            
            // The event with id 13 is the Haley's six heart event, the one at the beach requiring the player to find the bracelet
            // *** Exiting here will cause GridMovement and ObjectTracker functionality to not work during this event, making the bracelet impossible to track ***
            if (!Context.IsPlayerFree && !(Game1.CurrentEvent is not null && Game1.CurrentEvent.id == 13))
                return;
            
            FeatureManager.UpdateAll();

            RefreshBuildListIfRequired();

            RunObjectTrackerFeatureIfEnabled();

            void RefreshBuildListIfRequired()
            {
                if (Game1.player != null)
                {
                    if (Game1.timeOfDay >= 600 && prevDate != CurrentPlayer.Date)
                    {
                        prevDate = CurrentPlayer.Date;
                        Log.Debug("Refreshing buildlist...");
                        CustomCommands.OnBuildListCalled();
                    }
                }
            }

            void RefreshTranslationsOnLocaleChange()
            {
                if (previousLanguageCode == Game1.content.GetCurrentLanguage()) return;
                
                Log.Trace("Locale changed! Refreshing translations...");
                previousLanguageCode = Game1.content.GetCurrentLanguage();
                Translator.Instance.Initialize(ModManifest);
            }

            void RunObjectTrackerFeatureIfEnabled()
            {
                if (e.IsMultipleOf(15) && Config != null && Config.OTAutoRefreshing)
                {
                    ObjectTrackerFeature.Tick();
                }
            }
        }

        private void OnRenderingStart(object? sender, RenderingEventArgs renderingEventArgs)
        { 
            if (FirstRun)
            {
                Log.Trace("First WindowResized.");
                previousLanguageCode = Game1.content.GetCurrentLanguage();
                Translator.Instance.Initialize(ModManifest);
                Translator.Instance.CustomFunctions!.LoadLanguageHelper();
                FirstRun = false;
                ModHelper!.Events.Display.Rendering -= OnRenderingStart;
                Log.Trace("Removed OnFirstWindowResized");
            }
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            TextBoxPatch.activeTextBoxes = "";
            if (e.OldMenu != null)
            {
                Log.Debug($"Switched from {e.OldMenu.GetType()} menu, performing cleanup...");
                IClickableMenuPatch.Cleanup(e.OldMenu);
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (Game1.player.controller is not null && Config!.OTCancelAutoWalking.JustPressed())
            {
                #if DEBUG
                Log.Verbose("OnButtonPressed: canceling OTAutoWalking.");
                #endif
                Game1.player.controller.endBehaviorFunction(Game1.player, Game1.currentLocation);
                Helper.Input.Suppress(e.Button);
            }

            if (Config is null)
            {
                #if DEBUG
                Log.Verbose("OnButtonPressed: returning due to 'Config' being null");
                #endif
                return;
            }

            #region Simulate left and right clicks
            if (!TextBoxPatch.IsAnyTextBoxActive)
            {
                if (Game1.activeClickableMenu != null)
                {
                    MouseUtils.SimulateMouseClicks(
                        (x, y) => Game1.activeClickableMenu.receiveLeftClick(x, y),
                        (x, y) => Game1.activeClickableMenu.receiveRightClick(x, y)
                    );
                }
                else if (Game1.currentMinigame != null)
                {
                    MouseUtils.SimulateMouseClicks(
                        (x, y) => Game1.currentMinigame.receiveLeftClick(x, y),
                        (x, y) => Game1.currentMinigame.receiveRightClick(x, y)
                    );
                }
            }
            #endregion

            // Exit if in a menu
            if (Game1.activeClickableMenu != null)
            {
                #if DEBUG
                Log.Verbose("OnButtonPressed: returning due to 'Game1.activeClickableMenu' not being null AKA in a menu");
                #endif
                return;
            }

            // Code only run during game play below this line 
            
            // Stops the auto walk   controller if any movement key(WASD) is pressed
            if (TileViewer.Instance.IsAutoWalking && IsMovementKey(e.Button))
                TileViewer.Instance.StopAutoWalking(wasForced: true);

            // Narrate Current Location
            if (Config.LocationKey.JustPressed())
                Narrate(Game1.currentLocation.Name);

            // Narrate Position
            if (Config.PositionKey.JustPressed())
            {
                string toSpeak = Config.VerboseCoordinates
                    ? $"X: {CurrentPlayer.PositionX}, Y: {CurrentPlayer.PositionY}"
                    : $"{CurrentPlayer.PositionX}, {CurrentPlayer.PositionY}";
                Narrate(toSpeak);
            }

            // Narrate health and stamina
            if (Config.HealthNStaminaKey.JustPressed())
            {
                if (ModHelper == null)
                    return;

                // TODO unify translation keys
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
            }

            // Narrate money at hand
            if (Config.MoneyKey.JustPressed())
                Narrate($"You have {CurrentPlayer.Money}g");

            // Narrate time and season
            if (Config.TimeNSeasonKey.JustPressed())
                Narrate($"Time is {CurrentPlayer.TimeOfDay} and it is {CurrentPlayer.Day} {CurrentPlayer.Date} of {CurrentPlayer.Season}");

            // Manual read tile at player's position
            if (Config.ReadStandingTileKey.JustPressed())
                ReadTile.Instance.Run(manuallyTriggered: true, playersPosition: true);

            // Manual read tile at looking tile
            if (Config.ReadTileKey.JustPressed())
                ReadTile.Instance.Run(manuallyTriggered: true);

            // Tile viewing cursor keys
            TileViewer.Instance.HandleInput();

            // GridMovement 
            if (Game1.player.controller is not null || (GridMovement.Instance.is_warping))
            {
                Helper.Input.Suppress(e.Button);
                #if DEBUG
                Log.Verbose("OnButtonPressed: returning due to Game1.player.controller not being null or GridMovementFeature.is_warping being true");
                #endif
                return;
            }
            if (!Context.CanPlayerMove)
            {
                #if DEBUG
                Log.Verbose("OnButtonPressed: returning due to 'Context.CanPlayerMove' being false");
                #endif
                return;
            }
            HandleGridMovement();

            // local functions
            void Narrate(string message) => MainClass.ScreenReader.Say(message, true);

            bool IsMovementKey(SButton button)
            {
                return button.Equals(SButtonExtensions.ToSButton(Game1.options.moveUpButton[0]))
                    || button.Equals(SButtonExtensions.ToSButton(Game1.options.moveDownButton[0]))
                    || button.Equals(SButtonExtensions.ToSButton(Game1.options.moveLeftButton[0]))
                    || button.Equals(SButtonExtensions.ToSButton(Game1.options.moveRightButton[0]));
            }

            void HandleGridMovement()
            {
                if (Config!.GridMovementOverrideKey.IsDown())
                {
                    #if DEBUG
                    Log.Verbose("HandleGridMovement: returning due to 'Config.GridMovementOverrideKey.IsDown()' being true");
                    #endif
                    return;
                }

                if (!Config!.GridMovementActive)
                {
                    #if DEBUG
                    Log.Verbose("HandleGridMovement: returning due to 'Config.GridMovementActive' being false");
                    #endif
                    return;
                }

                e.Button.TryGetStardewInput(out InputButton keyboardButton);
                e.Button.TryGetController(out Buttons controllerButton);

                var directionMappings = new Dictionary<(InputButton, Buttons), int>
                {
                    {(Game1.options.moveUpButton[0], Buttons.DPadUp), 0},
                    {(Game1.options.moveRightButton[0], Buttons.DPadRight), 1},
                    {(Game1.options.moveDownButton[0], Buttons.DPadDown), 2},
                    {(Game1.options.moveLeftButton[0], Buttons.DPadLeft), 3}
                };

                foreach (var mapping in directionMappings)
                {
                    if (keyboardButton.Equals(mapping.Key.Item1) || controllerButton.Equals(mapping.Key.Item2))
                    {
                        GridMovement.Instance.HandleGridMovement(mapping.Value, keyboardButton);
                        Helper.Input.Suppress(e.Button);
                        break;
                    }
                }
            }
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if(Config!.ToggleGridMovementKey.JustPressed())
            {
                Config!.GridMovementActive = !Config!.GridMovementActive;
                string output = "Grid Movement Status: " + (Config!.GridMovementActive ? "Active" : "Inactive");
                MainClass.ScreenReader.Say(output, true);
                return;
            } 
            ObjectTrackerFeature?.HandleKeys(sender, e);
        }

        private void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            TileUtils.CleanupMaps(e.OldLocation, e.NewLocation);
            GridMovement.Instance.PlayerWarped(sender, e);
            ObjectTrackerFeature?.GetLocationObjects(resetFocus: true);
        }
    }
}
