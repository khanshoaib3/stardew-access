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

        Log.Info("Flooring is " + (MainClass.Config.ReadFlooring ? "on" : "off"));
    }

    private void Watered(string command, string[] args)
    {
        MainClass.Config.WateredToggle = !MainClass.Config.WateredToggle;
        _modHelper!.WriteConfig(MainClass.Config);

        Log.Info("Watered toggle is " + (MainClass.Config.WateredToggle ? "on" : "off"));
    }

    void ReadTile(string command, string[] args)
    {
        MainClass.Config.ReadTile = !MainClass.Config.ReadTile;
        _modHelper!.WriteConfig(MainClass.Config);

        Log.Info("Read Tile is " + (MainClass.Config.ReadTile ? "on" : "off"));
    }
}