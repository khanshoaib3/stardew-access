using stardew_access.Translation;
using StardewModdingAPI;

namespace stardew_access.Commands;

public class ReadTileCommands : ICustomCommand
{
    private IModHelper? _modHelper;

    public void Add(IModHelper modHelper)
    {
        _modHelper = modHelper;

        _modHelper.ConsoleCommands.Add("readtile", "Toggle read tile feature.", callback: ReadTile);
        _modHelper.ConsoleCommands.Add("flooring", "Toggle flooring in read tile.", Flooring);
        _modHelper.ConsoleCommands.Add("watered", "Toggle speaking watered or unwatered for crops.", Watered);
    }

    private void Flooring(string command, string[] args)
    {
        MainClass.Config.ReadFlooring = !MainClass.Config.ReadFlooring;
        _modHelper!.WriteConfig(MainClass.Config);

        Log.Info(Translator.Instance.Translate("commands-read_tile-flooring_toggle",
            new { is_enabled = MainClass.Config.ReadFlooring ? 1 : 0 }, TranslationCategory.CustomCommands));
    }

    private void Watered(string command, string[] args)
    {
        MainClass.Config.WateredToggle = !MainClass.Config.WateredToggle;
        _modHelper!.WriteConfig(MainClass.Config);

        Log.Info(Translator.Instance.Translate("commands-read_tile-watered_toggle",
            new { is_enabled = MainClass.Config.WateredToggle ? 1 : 0 }, TranslationCategory.CustomCommands));
    }

    void ReadTile(string command, string[] args)
    {
        MainClass.Config.ReadTile = !MainClass.Config.ReadTile;
        _modHelper!.WriteConfig(MainClass.Config);

        Log.Info(Translator.Instance.Translate("commands-read_tile-read_tile_toggle",
            new { is_enabled = MainClass.Config.ReadTile ? 1 : 0 }, TranslationCategory.CustomCommands));
    }
}