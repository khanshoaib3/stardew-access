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
        private static ModConfig? config;
        private Harmony? harmony;
        private static IMonitor? monitor;
        private static Radar? radarFeature;
        private static IScreenReader? screenReader;
        private static IModHelper? modHelper;

        internal static ModConfig Config { get => config; set => config = value; }
        public static IModHelper? ModHelper { get => modHelper; }
        public static Radar RadarFeature
        {
            get
            {
                if (radarFeature == null) { radarFeature = new Radar(); }
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

            CustomSoundEffects.Initialize();

            CustomCommands.Initialize();

            harmony = new Harmony(ModManifest.UniqueID);
            HarmonyPatches.Initialize(harmony);

            //Initialize marked locations
            for (int i = 0; i < BuildingNAnimalMenuPatches.marked.Length; i++)
            {
                BuildingNAnimalMenuPatches.marked[i] = Vector2.Zero;
            }

            for (int i = 0; i < BuildingNAnimalMenuPatches.availableBuildings.Length; i++)
            {
                BuildingNAnimalMenuPatches.availableBuildings[i] = null;
            }
            #endregion

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.onUpdateTicked;
            AppDomain.CurrentDomain.DomainUnload += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;
        }

        public void OnExit(object? sender, EventArgs? e)
        {
            // Don't if this ever gets called or not but, just in case if it does.
            if (ScreenReader != null)
                ScreenReader.CloseScreenReader();
        }

        /// <summary>Returns the Screen Reader class for other mods to use.</summary>
        public override object GetApi()
        {
            return new API();
        }

        private void onUpdateTicked(object? sender, UpdateTickedEventArgs? e)
        {
            if (!Context.IsPlayerFree)
                return;

            // Narrates currently selected inventory slot
            Other.narrateCurrentSlot();

            // Narrate current location's name
            Other.narrateCurrentLocation();

            if (Config.SnapMouse)
                Other.SnapMouseToPlayer();

            if (!ReadTile.isReadingTile && Config.ReadTile)
            {
                ReadTile.isReadingTile = true;
                ReadTile.run();
                Task.Delay(100).ContinueWith(_ => { ReadTile.isReadingTile = false; });
            }

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
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs? e)
        {
            if (e == null)
                return;

            bool isLeftAltPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt);

            if (Game1.activeClickableMenu != null)
            {
                bool isLeftShiftPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);
                bool isLeftControlPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl);
                bool isCustomizingChrachter = Game1.activeClickableMenu is CharacterCustomization || (TitleMenu.subMenu != null && TitleMenu.subMenu is CharacterCustomization);

                #region Mouse Click Simulation
                // Main Keybinds
                if (isLeftControlPressed && Config.LeftClickMainKey.JustPressed())
                {
                    Game1.activeClickableMenu.receiveLeftClick(Game1.getMouseX(true), Game1.getMouseY(true));
                }
                if (isLeftShiftPressed && Config.RightClickMainKey.JustPressed())
                {
                    Game1.activeClickableMenu.receiveRightClick(Game1.getMouseX(true), Game1.getMouseY(true));
                }

                // Alternate Keybinds
                if (!isCustomizingChrachter && Game1.activeClickableMenu is not AnimalQueryMenu && Config.LeftClickAlternateKey.JustPressed()) // Excluding the character creation menu
                {
                    Game1.activeClickableMenu.receiveLeftClick(Game1.getMouseX(true), Game1.getMouseY(true));
                }
                if (!isCustomizingChrachter && Game1.activeClickableMenu is not AnimalQueryMenu && Config.RightClickAlternateKey.JustPressed()) // Excluding the character creation menu
                {
                    Game1.activeClickableMenu.receiveRightClick(Game1.getMouseX(true), Game1.getMouseY(true));
                }
                #endregion
            }

            if (!Context.IsPlayerFree)
                return;

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
                    toSpeak = $"X: {CurrentPlayer.getPositionX()}, Y: {CurrentPlayer.getPositionY()}";
                }
                else
                {
                    toSpeak = $"{CurrentPlayer.getPositionX()}, {CurrentPlayer.getPositionY()}";
                }

                MainClass.ScreenReader.Say(toSpeak, true);
                return;
            }

            // Narrate health and stamina
            if (Config.HealthNStaminaKey.JustPressed())
            {
                string toSpeak = $"Health is {CurrentPlayer.getHealth()} and Stamina is {CurrentPlayer.getStamina()}";
                MainClass.ScreenReader.Say(toSpeak, true);
                return;
            }

            // Narrate money at hand
            if (Config.MoneyKey.JustPressed())
            {
                string toSpeak = $"You have {CurrentPlayer.getMoney()}g";
                MainClass.ScreenReader.Say(toSpeak, true);
                return;
            }

            // Narrate time and season
            if (Config.TimeNSeasonKey.JustPressed())
            {
                string toSpeak = $"Time is {CurrentPlayer.getTimeOfDay()} and it is {CurrentPlayer.getDay()} {CurrentPlayer.getDate()} of {CurrentPlayer.getSeason()}";
                MainClass.ScreenReader.Say(toSpeak, true);
                return;
            }

            // Manual read tile at player's position
            if (Config.ReadStandingTileKey.JustPressed())
            {
                ReadTile.run(manuallyTriggered: true, playersPosition: true);
                return;
            }

            // Manual read tile at looking tile
            if (Config.ReadTileKey.JustPressed())
            {
                ReadTile.run(manuallyTriggered: true);
                return;
            }
        }

        public static void ErrorLog(string message)
        {
            if (monitor == null)
                return;

            monitor.Log(message, LogLevel.Error);
        }

        public static void DebugLog(string message)
        {
            if (monitor == null)
                return;

            monitor.Log(message, LogLevel.Debug);
        }
    }
}