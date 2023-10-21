using StardewModdingAPI;

namespace stardew_access.Commands;

public class OtherCommands : ICustomCommand
{
    private IModHelper? _modHelper;

    public void Add(IModHelper modHelper)
    {
        _modHelper = modHelper;

        _modHelper.ConsoleCommands.Add("refsr", "Refresh screen reader", RefreshScreenReader);

        _modHelper.ConsoleCommands.Add("refmc", "Refresh mod config", RefreshModConfig);

        // TODO: add Refresh functionality to `AccessibleTileManager and restore this
        /*helper.ConsoleCommands.Add("refst", "Refresh static tiles", (string command, string[] args) =>
        {
            StaticTiles.LoadTilesFiles();
            StaticTiles.SetupTilesDicts();

            Log.Info("Static tiles refreshed!");
        });*/

        _modHelper.ConsoleCommands.Add("hnspercent", "Toggle between speaking in percentage or full health and stamina.", HnsPercentageToggle);

        _modHelper.ConsoleCommands.Add("snapmouse", "Toggle snap mouse feature.", SnapMouseToggle);

        _modHelper.ConsoleCommands.Add("warning", "Toggle warnings feature.", WarningsToggle);

        _modHelper.ConsoleCommands.Add("tts", "Toggles the screen reader/tts", TtsToggle);
    }

    private void RefreshScreenReader(string command, string[] args)
    {
        MainClass.ScreenReader.InitializeScreenReader();

        Log.Info("Screen Reader refreshed!");
    }

    private void RefreshModConfig(string command, string[] args)
    {
        MainClass.Config = _modHelper!.ReadConfig<ModConfig>();

        Log.Info("Mod Config refreshed!");
    }

    private void HnsPercentageToggle(string command, string[] args)
    {
        MainClass.Config.HealthNStaminaInPercentage = !MainClass.Config.HealthNStaminaInPercentage;
        _modHelper!.WriteConfig(MainClass.Config);

        Log.Info("Speaking in percentage is " + (MainClass.Config.HealthNStaminaInPercentage ? "on" : "off"));
    }

    private void SnapMouseToggle(string command, string[] args)
    {
        MainClass.Config.SnapMouse = !MainClass.Config.SnapMouse;
        _modHelper!.WriteConfig(MainClass.Config);

        Log.Info("Snap Mouse is " + (MainClass.Config.SnapMouse ? "on" : "off"));
    }

    private void WarningsToggle(string command, string[] args)
    {
        MainClass.Config.Warning = !MainClass.Config.Warning;
        _modHelper!.WriteConfig(MainClass.Config);

        Log.Info("Warnings is " + (MainClass.Config.Warning ? "on" : "off"));
    }

    private void TtsToggle(string command, string[] args)
    {
        MainClass.Config.TTS = !MainClass.Config.TTS;
        _modHelper!.WriteConfig(MainClass.Config);

        Log.Info("TTS is " + (MainClass.Config.TTS ? "on" : "off"));
    }
}