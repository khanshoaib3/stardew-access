using HarmonyLib;
using Microsoft.Xna.Framework;
using stardew_access.Commands;
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
                if (screenReader == null)
                {
                    screenReader = new ScreenReaderImpl();
                    screenReader.InitializeScreenReader();
                }

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
            ScreenReader.Say("Initializing Stardew Access", true);

            CustomSoundEffects.Initialize();

            CommandManager.AddAll(helper);

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

            helper.Events.Input.ButtonPressed += FeatureManager.OnButtonPressedEvent;
            helper.Events.Input.ButtonsChanged += FeatureManager.OnButtonsChangedEvent;
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
            ObjectTracker.Instance.GetLocationObjects();
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            TileManager.EnsureLocationLoaded(Game1.currentLocation);
            RefreshTranslationsOnLocaleChange();
            
            // The event with id 13 is the Haley's six heart event, the one at the beach requiring the player to find the bracelet
            // *** Exiting here will cause GridMovement and ObjectTracker functionality to not work during this event, making the bracelet impossible to track ***
            if (!Context.IsPlayerFree && !(Game1.CurrentEvent is not null && Game1.CurrentEvent.id == 13))
                return;
            
            FeatureManager.UpdateEvent(sender, e);

            RefreshBuildListIfRequired();

            void RefreshBuildListIfRequired()
            {
                if (Game1.player != null)
                {
                    if (Game1.timeOfDay >= 600 && prevDate != CurrentPlayer.Date)
                    {
                        prevDate = CurrentPlayer.Date;
                        Log.Debug("Refreshing buildlist...");
                        TileMarkingCommands.BuildList();
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

        private void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            // exit if warp event is for other players
            if (!e.IsLocalPlayer) return;
            TileUtils.CleanupMaps(e.OldLocation, e.NewLocation);
            FeatureManager.OnPlayerWarpedEvent(sender, e);
        }

        internal static string GetCurrentSaveFileName()
        {
            if (string.IsNullOrEmpty(Constants.CurrentSavePath))
            {
                return "";
            }

            string[] pathParts = Constants.CurrentSavePath.Split(Path.DirectorySeparatorChar);
            string currentSave = pathParts[^1];
            #if DEBUG
            Log.Verbose($"Savefile name is: {currentSave}");
            #endif
            return currentSave;
        }

    }
}
