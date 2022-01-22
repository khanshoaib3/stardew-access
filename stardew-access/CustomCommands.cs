using StardewModdingAPI;

namespace stardew_access
{
    internal class CustomCommands
    {
        internal static void Initialize(IModHelper helper)
        {

            helper.ConsoleCommands.Add("readtile", "Toggle read tile feature.", (string commmand, string[] args) =>
            {
                MainClass.readTile = !MainClass.readTile;

                MainClass.monitor.Log("Read Tile is " + (MainClass.readTile ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("snapmouse", "Toggle snap mouse feature.", (string commmand, string[] args) =>
            {
                MainClass.snapMouse = !MainClass.snapMouse;

                MainClass.monitor.Log("Snap Mouse is " + (MainClass.snapMouse ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("radar", "Toggle radar feature.", (string commmand, string[] args) =>
            {
                MainClass.radar = !MainClass.radar;

                MainClass.monitor.Log("Radar " + (MainClass.radar ? "on" : "off"), LogLevel.Info);
            });

            #region Radar Feature
            helper.ConsoleCommands.Add("rdebug", "Toggle debugging in radar feature.", (string commmand, string[] args) =>
            {
                MainClass.radarDebug = !MainClass.radarDebug;

                MainClass.monitor.Log("Radar debugging " + (MainClass.radarDebug ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("rstereo", "Toggle stereo sound in radar feature.", (string commmand, string[] args) =>
            {
                MainClass.radarStereoSound = !MainClass.radarStereoSound;

                MainClass.monitor.Log("Stereo sound is " + (MainClass.radarStereoSound ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("rfocus", "Toggle focus mode in radar feature.", (string commmand, string[] args) =>
            {
                bool focus = MainClass.radarFeature.toggleFocus();

                MainClass.monitor.Log("Focus mode is " + (focus? "on" : "off"), LogLevel.Info);
            });

            #region Exclusions
            helper.ConsoleCommands.Add("readd", "Add an object key to the exclusions list of radar feature.", (string commmand, string[] args) =>
                {
                    string? keyToAdd = null;

                    for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                    if (keyToAdd != null)
                    {
                        keyToAdd = keyToAdd.Trim().ToLower();
                        MainClass.radarFeature.exclusions.Add(keyToAdd);
                        MainClass.monitor.Log($"Added {keyToAdd} key to exclusions.", LogLevel.Info);
                    }
                    else
                    {
                        MainClass.monitor.Log("Unable to add the key to exclusions.", LogLevel.Info);
                    }
                });

            helper.ConsoleCommands.Add("reremove", "Remove an object key from the exclusions list of radar feature.", (string commmand, string[] args) =>
            {
                string? keyToAdd = null;

                for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                if (keyToAdd != null)
                {
                    keyToAdd = keyToAdd.Trim().ToLower();
                    if (MainClass.radarFeature.exclusions.Contains(keyToAdd))
                    {
                        MainClass.radarFeature.exclusions.Remove(keyToAdd);
                        MainClass.monitor.Log($"Removed {keyToAdd} key from exclusions.", LogLevel.Info);
                    }
                    else
                    {
                        MainClass.monitor.Log($"Cannot find{keyToAdd} key in exclusions.", LogLevel.Info);
                    }
                }
                else
                {
                    MainClass.monitor.Log("Unable to remove the key from exclusions.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("relist", "List all the exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                if (MainClass.radarFeature.exclusions.Count > 0)
                {
                    for (int i = 0; i < MainClass.radarFeature.exclusions.Count; i++)
                    {
                        MainClass.monitor.Log($"{i + 1}) {MainClass.radarFeature.exclusions[i]}", LogLevel.Info);
                    }
                }
                else
                {
                    MainClass.monitor.Log("No exclusions found.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("recount", "Number of exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                MainClass.monitor.Log($"There are {MainClass.radarFeature.exclusions.Count} exclusiond in the radar feature.", LogLevel.Info);
            });
            #endregion

            #region Focus
            helper.ConsoleCommands.Add("rfadd", "Add an object key to the focus list of radar feature.", (string commmand, string[] args) =>
                {
                    string? keyToAdd = null;

                    for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                    if (keyToAdd != null)
                    {
                        keyToAdd = keyToAdd.Trim().ToLower();
                        MainClass.radarFeature.focus.Add(keyToAdd);
                        MainClass.monitor.Log($"Added {keyToAdd} key to focus.", LogLevel.Info);
                    }
                    else
                    {
                        MainClass.monitor.Log("Unable to add the key to focus.", LogLevel.Info);
                    }
                });

            helper.ConsoleCommands.Add("rfremove", "Remove an object key from the focus list of radar feature.", (string commmand, string[] args) =>
            {
                string? keyToAdd = null;

                for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                if (keyToAdd != null)
                {
                    keyToAdd = keyToAdd.Trim().ToLower();
                    if (MainClass.radarFeature.focus.Contains(keyToAdd))
                    {
                        MainClass.radarFeature.focus.Remove(keyToAdd);
                        MainClass.monitor.Log($"Removed {keyToAdd} key from focus.", LogLevel.Info);
                    }
                    else
                    {
                        MainClass.monitor.Log($"Cannot find{keyToAdd} key in focus.", LogLevel.Info);
                    }
                }
                else
                {
                    MainClass.monitor.Log("Unable to remove the key from exclusions.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("rflist", "List all the exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                if (MainClass.radarFeature.focus.Count > 0)
                {
                    for (int i = 0; i < MainClass.radarFeature.focus.Count; i++)
                    {
                        MainClass.monitor.Log($"{i + 1}) {MainClass.radarFeature.focus[i]}", LogLevel.Info);
                    }
                }
                else
                {
                    MainClass.monitor.Log("No objects found in the focus list.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("rfcount", "Number of exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                MainClass.monitor.Log($"There are {MainClass.radarFeature.focus.Count} objects in the focus list in the radar feature.", LogLevel.Info);
            }); 
            #endregion

            #endregion

            helper.ConsoleCommands.Add("refsr", "Refresh screen reader", (string commmand, string[] args) =>
            {
                MainClass.screenReader.InitializeScreenReader();

                MainClass.monitor.Log("Screen Reader refreshed!", LogLevel.Info);
            });
        }
    }
}
