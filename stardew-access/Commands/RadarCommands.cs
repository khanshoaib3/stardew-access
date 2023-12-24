using stardew_access.Features;
using StardewModdingAPI;

namespace stardew_access.Commands;

public class RadarCommands : ICustomCommand
{
    private IModHelper? _modHelper;

    public void Add(IModHelper modHelper)
    {
        _modHelper = modHelper;

        _modHelper.ConsoleCommands.Add("radar", "Toggle radar feature.", RadarCommand);

        _modHelper.ConsoleCommands.Add("rdebug", "Toggle debugging in radar feature.", Debug);

        _modHelper.ConsoleCommands.Add("rstereo", "Toggle stereo sound in radar feature.", Stereo);

        _modHelper.ConsoleCommands.Add("rfocus", "Toggle focus mode in radar feature.", Focus);

        _modHelper.ConsoleCommands.Add("rdelay", "Set the delay of radar feature in milliseconds.", Delay);

        _modHelper.ConsoleCommands.Add("rrange", "Set the range of radar feature.", Range);

        _modHelper.ConsoleCommands.Add("readd", "Add an object key to the exclusions list of radar feature.", AddToExclusions);

        _modHelper.ConsoleCommands.Add("reremove", "Remove an object key from the exclusions list of radar feature.", RemoveFromExclusions);

        _modHelper.ConsoleCommands.Add("relist", "List all the exclusions in the radar feature.", ListExclusions);

        _modHelper.ConsoleCommands.Add("reclear", "Clear the focus exclusions in the radar featrure.", ClearExclusions);

        _modHelper.ConsoleCommands.Add("recount", "Number of exclusions in the radar feature.", CountExclusions);

        _modHelper.ConsoleCommands.Add("rfadd", "Add an object key to the focus list of radar feature.", AddToFocus);

        _modHelper.ConsoleCommands.Add("rfremove", "Remove an object key from the focus list of radar feature.", RemoveFromFocus);

        _modHelper.ConsoleCommands.Add("rflist", "List all the focused objects in the radar feature.", ListAllFocus);

        _modHelper.ConsoleCommands.Add("rfclear", "Clear the focus list in the radar featrure.", ClearFocus);

        _modHelper.ConsoleCommands.Add("rfcount", "Number of list in the radar feature.", CountFocus);
    }

    private void CountFocus(string command, string[] args)
    {
        Log.Info($"There are {Radar.Instance.Focus.Count} objects in the focus list in the radar feature.");
    }

    private void ClearFocus(string command, string[] args)
    {
        Radar.Instance.Focus.Clear();
        Log.Info($"Cleared the focus list in the radar feature.");
    }

    private void ListAllFocus(string command, string[] args)
    {
        if (Radar.Instance.Focus.Count > 0)
        {
            string toPrint = "";
            for (int i = 0; i < Radar.Instance.Focus.Count; i++)
            {
                toPrint = $"{toPrint}\t{i + 1}): {Radar.Instance.Focus[i]}";
            }

            Log.Info(toPrint);
        }
        else
        {
            Log.Info("No objects found in the focus list.");
        }
    }

    private void RemoveFromFocus(string command, string[] args)
    {
        string? keyToAdd = null;

        for (int i = 0; i < args.Length; i++)
        {
            keyToAdd += " " + args[i];
        }

        if (keyToAdd != null)
        {
            keyToAdd = keyToAdd.Trim().ToLower();
            if (Radar.Instance.Focus.Contains(keyToAdd))
            {
                Radar.Instance.Focus.Remove(keyToAdd);
                Log.Info($"Removed {keyToAdd} key from focus list.");
            }
            else
            {
                Log.Info($"Cannot find {keyToAdd} key in focus list.");
            }
        }
        else
        {
            Log.Info("Unable to remove the key from focus list.");
        }
    }

    private void AddToFocus(string command, string[] args)
    {
        string? keyToAdd = null;

        for (int i = 0; i < args.Length; i++)
        {
            keyToAdd += " " + args[i];
        }

        if (keyToAdd != null)
        {
            keyToAdd = keyToAdd.Trim().ToLower();
            if (!Radar.Instance.Focus.Contains(keyToAdd))
            {
                Radar.Instance.Focus.Add(keyToAdd);
                Log.Info($"Added {keyToAdd} key to focus list.");
            }
            else
            {
                Log.Info($"{keyToAdd} key already present in the list.");
            }
        }
        else
        {
            Log.Info("Unable to add the key to focus list.");
        }
    }

    private void CountExclusions(string command, string[] args)
    {
        Log.Info($"There are {Radar.Instance.Exclusions.Count} exclusiond in the radar feature.");
    }

    private void ClearExclusions(string command, string[] args)
    {
        Radar.Instance.Exclusions.Clear();
        Log.Info($"Cleared the focus list in the exclusions feature.");
    }

    private void ListExclusions(string command, string[] args)
    {
        if (Radar.Instance.Exclusions.Count > 0)
        {
            string toPrint = "";
            for (int i = 0; i < Radar.Instance.Exclusions.Count; i++)
            {
                toPrint = $"{toPrint}\t{i + 1}: {Radar.Instance.Exclusions[i]}";
            }

            Log.Info(toPrint);
        }
        else
        {
            Log.Info("No exclusions found.");
        }
    }

    private void RemoveFromExclusions(string command, string[] args)
    {
        string? keyToAdd = null;

        for (int i = 0; i < args.Length; i++)
        {
            keyToAdd += " " + args[i];
        }

        if (keyToAdd != null)
        {
            keyToAdd = keyToAdd.Trim().ToLower();
            if (Radar.Instance.Exclusions.Contains(keyToAdd))
            {
                Radar.Instance.Exclusions.Remove(keyToAdd);
                Log.Info($"Removed {keyToAdd} key from exclusions list.");
            }
            else
            {
                Log.Info($"Cannot find {keyToAdd} key in exclusions list.");
            }
        }
        else
        {
            Log.Info("Unable to remove the key from exclusions list.");
        }
    }

    private void AddToExclusions(string command, string[] args)
    {
        string? keyToAdd = null;

        for (int i = 0; i < args.Length; i++)
        {
            keyToAdd += " " + args[i];
        }

        if (keyToAdd != null)
        {
            keyToAdd = keyToAdd.Trim().ToLower();
            if (!Radar.Instance.Exclusions.Contains(keyToAdd))
            {
                Radar.Instance.Exclusions.Add(keyToAdd);
                Log.Info($"Added {keyToAdd} key to exclusions list.");
            }
            else
            {
                Log.Info($"{keyToAdd} key already present in the list.");
            }
        }
        else
        {
            Log.Info("Unable to add the key to exclusions list.");
        }
    }

    private void Range(string command, string[] args)
    {
        string? rangeInString = null;

        if (args.Length > 0)
        {
            rangeInString = args[0];


            bool isParsable = int.TryParse(rangeInString, out int range);

            if (isParsable)
            {
                Radar.Instance.Range = range;
                if (range >= 2 && range <= 10)
                    Log.Info($"Range set to {Radar.Instance.Range}.");
                else
                    Log.Info($"Range should be atleast 2 and maximum 10.");
            }
            else
            {
                Log.Info("Invalid range amount, it can only be in numeric form.");
            }
        }
        else
        {
            Log.Info("Enter the range amount!");
        }
    }

    private void Delay(string command, string[] args)
    {
        string? delayInString = null;

        if (args.Length > 0)
        {
            delayInString = args[0];


            bool isParsable = int.TryParse(delayInString, out int delay);

            if (isParsable)
            {
                Radar.Instance.Delay = delay;
                if (delay >= 1000)
                    Log.Info($"Delay set to {Radar.Instance.Delay} milliseconds.");
                else
                    Log.Info($"Delay should be atleast 1 second or 1000 millisecond long.");
            }
            else
            {
                Log.Info("Invalid delay amount, it can only be in numeric form.");
            }
        }
        else
        {
            Log.Info("Enter the delay amount (in milliseconds)!");
        }
    }

    private void Focus(string command, string[] args)
    {
        bool focus = Radar.Instance.ToggleFocus();

        Log.Info("Focus mode is " + (focus ? "on" : "off"));
    }

    private void Stereo(string command, string[] args)
    {
        MainClass.Config.RadarStereoSound = !MainClass.Config.RadarStereoSound;
        _modHelper!.WriteConfig(MainClass.Config);

        Log.Info("Stereo sound is " + (MainClass.Config.RadarStereoSound ? "on" : "off"));
    }

    private void Debug(string command, string[] args)
    {
        Radar.RadarDebug = !Radar.RadarDebug;

        Log.Info("Radar debugging " + (Radar.RadarDebug ? "on" : "off"));
    }

    private void RadarCommand(string command, string[] args)
    {
        MainClass.Config.Radar = !MainClass.Config.Radar;
        _modHelper!.WriteConfig(MainClass.Config);

        Log.Info("Radar " + (MainClass.Config.Radar ? "on" : "off"));
    }
}