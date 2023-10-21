using StardewModdingAPI;

namespace stardew_access.Commands;

public class CommandManager
{
    public static void AddAll(IModHelper modHelper)
    {
        List<ICustomCommand> allCommands = new List<ICustomCommand>()
        {
            new ReadTileCommands(),
        };

        foreach (ICustomCommand command in allCommands)
        {
            command.Add(modHelper);
        }
    }
}