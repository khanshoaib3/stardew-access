using stardew_access.Game;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using stardew_access.Patches;
using AutoHotkey.Interop;

namespace stardew_access
{

    public class MainClass : Mod
    {
        private Harmony? harmony;
        public static bool readTile = true;
        public static bool snapMouse = true;
        public static bool isNarratingHudMessage = false;
        public static bool radar = false;
        public static bool radarDebug = true;
        public static bool radarStereoSound = true;
        public static IMonitor? monitor;
        AutoHotkeyEngine ahk;
        public static string hudMessageQueryKey = "";
        public static Radar radarFeature;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            #region Initializations

            monitor = Monitor; // Inititalize monitor

            Game1.options.setGamepadMode("force_on");

            // Initialize AutoHotKey
            try
            {
                ahk = AutoHotkeyEngine.Instance;
                ahk.ExecRaw("[::\nSend {LButton}");
                ahk.ExecRaw("^Enter::\nSend {LButton}");
                ahk.ExecRaw("]::\nSend {RButton}");
                ahk.ExecRaw("+Enter::\nSend {RButton}");
            }
            catch (Exception e)
            {
                monitor.Log($"Unable to initialize AutoHotKey:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }

            ScreenReader.initializeScreenReader();

            CustomSoundEffects.Initialize(helper);

            CustomCommands.Initialize(helper);

            radarFeature = new Radar();

            harmony = new Harmony(ModManifest.UniqueID);
            HarmonyPatches.Initialize(harmony);

            #endregion

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.onUpdateTicked;
        }

        private void onUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            // Reset variables
            MenuPatches.resetGlobalVars();
            QuestPatches.resetGlobalVars();

            Other.narrateCurrentSlot();

            Other.narrateCurrentLocation();

            if (snapMouse)
                Other.SnapMouseToPlayer();

            if(!ReadTile.isReadingTile && readTile)
                ReadTile.run();

            if(!radarFeature.isRunning && radar)
                radarFeature.run();

            if (!isNarratingHudMessage)
            {
                Other.narrateHudMessages();
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            // Narrate health and stamina
            if (Equals(e.Button, SButton.H))
            {
                string toSpeak = $"Health is {CurrentPlayer.getHealth()} and Stamina is {CurrentPlayer.getStamina()}";
                ScreenReader.say(toSpeak, true);
            }

            // Narrate Position
            if (Equals(e.Button, SButton.K))
            {
                string toSpeak = $"X: {CurrentPlayer.getPositionX()} , Y: {CurrentPlayer.getPositionY()}";
                ScreenReader.say(toSpeak, true);
            }

            // Narrate money at hand
            if (Equals(e.Button, SButton.R))
            {
                string toSpeak = $"You have {CurrentPlayer.getMoney()}g";
                ScreenReader.say(toSpeak, true);
            }

            // Narrate time and season
            if (Equals(e.Button, SButton.Q))
            {
                string toSpeak = $"Time is {CurrentPlayer.getTimeOfDay()} and it is {CurrentPlayer.getDay()} {CurrentPlayer.getDate()} of {CurrentPlayer.getSeason()}";
                ScreenReader.say(toSpeak, true);
            }

            // Manual read tile
            if (Equals(e.Button, SButton.J))
            {
                ReadTile.run(manuallyTriggered: true);
            }

            /*if (Equals(e.Button, SButton.B))
            {
                Game1.player.controller = new PathFindController(Game1.player, Game1.currentLocation, new Point(49,13), 2);
                monitor.Log($"{Game1.player.controller.pathToEndPoint==null}", LogLevel.Debug); // true if path not found
            }*/
        }
    }
}