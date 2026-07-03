using HarmonyLib;
using Microsoft.Xna.Framework;
using stardew_access.Commands;
using stardew_access.Features;
using stardew_access.Features.Navigator;
using stardew_access.Framework;
using stardew_access.Patches;
using stardew_access.ScreenReader;
using stardew_access.Tiles;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace stardew_access;

public class MainClass : Mod
{
    #region Global Vars & Properties

    private static int prevDate = -99;
    private static bool FirstRun = true;
    private static bool CheckedMailToday = false;
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

        CustomSoundEffects.Initialize();

        CommandManager.RegisterAll(helper);

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
        helper.Events.Content.LocaleChanged += OnLocaleChanged;
        helper.Events.Display.Rendering += OnRenderingStart;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.Display.MenuChanged += OnMenuChanged;
        helper.Events.Content.AssetRequested += AssetHandler.OnAssetRequested;
        helper.Events.Content.AssetsInvalidated += AssetHandler.OnAssetsInvalidated;
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
        if (Config.EnableCheats)
        {
            Program.enableCheats = true;
            Log.Info("Enabled debug cheats");
        }
        TileManager.Initialize();
        ModConfigMenu.Create(modHelper!, ModManifest, config!);
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs? e)
    {
        CheckedMailToday = false;
        ObjectTracker.Instance.GetLocationObjects();
    }

    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!CheckedMailToday && (Game1.currentLocation is Farm || (Game1.currentLocation is StardewValley.Locations.IslandWest islandWest && islandWest.farmhouseMailbox.Value)) && Game1.player.mailbox is not null && Game1.player.mailbox.Count > 0)
        {
            if (Config.YouveGotMailSound)
                Game1.playSound("youve_got_mail");
            else
                ScreenReader.TranslateAndSayWithChecker("feature-speak_youve_got_mail", false);
            CheckedMailToday = true;
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu != null && IClickableMenuPatch.CurrentMenu is null)
        {
            IClickableMenuPatch.ManuallyCallingDrawPatch = true;
            IClickableMenuPatch.DrawPatch();
        }

        TileManager.EnsureLocationLoaded(Game1.currentLocation);
        
        FeatureManager.UpdateEvent(sender, e);

        RefreshBuildListIfRequired();

        static void RefreshBuildListIfRequired()
        {
            if (Game1.player != null && Context.IsPlayerFree)
            {
                if (Game1.timeOfDay >= 600 && prevDate != CurrentPlayer.Date)
                {
                    prevDate = CurrentPlayer.Date;
                    Log.Debug("Refreshing buildlist...");
                    TileMarkingCommands.BuildList([]);
                }
            }
        }
    }

    private void OnRenderingStart(object? sender, RenderingEventArgs renderingEventArgs)
    { 
        if (FirstRun)
        {
            Log.Trace("First WindowResized.");
            Translator.Instance.Initialize(ModManifest);
            Translator.Instance.CustomFunctions!.LoadLanguageHelper();
            FirstRun = false;
            ModHelper!.Events.Display.Rendering -= OnRenderingStart;
            Log.Trace("Removed OnFirstWindowResized");
            ScreenReader.TranslateAndSayWithMenuChecker("menu-title-stardew_access_loaded", true, new { version = ModManifest.Version, cheats = Program.enableCheats ? 1 : 0 });
        }
    }

    private void OnLocaleChanged(object? sender, LocaleChangedEventArgs e)
    {
        Log.Info($"LanguageCode changed from \"{e.OldLanguage}\" to \"{e.NewLanguage}\", reloading translations");
        Translator.Instance.Initialize(ModManifest);
        Translator.Instance.CustomFunctions!.LoadLanguageHelper();
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
