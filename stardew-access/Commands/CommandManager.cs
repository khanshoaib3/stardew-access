using StardewModdingAPI;

namespace stardew_access.Commands;

public class CommandManager
{
    public static void AddAll(IModHelper modHelper)
    {
        List<ICustomCommand> allCommands =
        [
            new ReadTileCommands(),
            new TileMarkingCommands(),
            new OtherCommands(),
            new RadarCommands(),
        ];

        foreach (ICustomCommand command in allCommands)
        {
            command.Add(modHelper);
        }
    }
}