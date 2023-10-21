using Microsoft.Xna.Framework;
using stardew_access.Features;
using stardew_access.Utils;
using stardew_access.Patches;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace stardew_access
{
    internal class CustomCommands
    {
        internal static void Initialize()
        {
            //TODO organise this, create separate method for all commands
            IModHelper? helper = MainClass.ModHelper;
            if (helper == null)
                return;

            #region Other
            helper.ConsoleCommands.Add("refsr", "Refresh screen reader", (string command, string[] args) =>
                        {
                            MainClass.ScreenReader.InitializeScreenReader();

                            Log.Info("Screen Reader refreshed!");
                        });

            helper.ConsoleCommands.Add("refmc", "Refresh mod config", (string command, string[] args) =>
            {
                MainClass.Config = helper.ReadConfig<ModConfig>();

                Log.Info("Mod Config refreshed!");
            });

            // TODO: add Refresh functionality to `AccessibleTileManager and restore this
            /*helper.ConsoleCommands.Add("refst", "Refresh static tiles", (string command, string[] args) =>
            {
                StaticTiles.LoadTilesFiles();
                StaticTiles.SetupTilesDicts();

                Log.Info("Static tiles refreshed!");
            });*/

            helper.ConsoleCommands.Add("hnspercent", "Toggle between speaking in percentage or full health and stamina.", (string command, string[] args) =>
            {
                MainClass.Config.HealthNStaminaInPercentage = !MainClass.Config.HealthNStaminaInPercentage;
                helper.WriteConfig(MainClass.Config);

                Log.Info("Speaking in percentage is " + (MainClass.Config.HealthNStaminaInPercentage ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("snapmouse", "Toggle snap mouse feature.", (string command, string[] args) =>
            {
                MainClass.Config.SnapMouse = !MainClass.Config.SnapMouse;
                helper.WriteConfig(MainClass.Config);

                Log.Info("Snap Mouse is " + (MainClass.Config.SnapMouse ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("warning", "Toggle warnings feature.", (string command, string[] args) =>
            {
                MainClass.Config.Warning = !MainClass.Config.Warning;
                helper.WriteConfig(MainClass.Config);

                Log.Info("Warnings is " + (MainClass.Config.Warning ? "on" : "off"));
            });

            helper.ConsoleCommands.Add("tts", "Toggles the screen reader/tts", (string command, string[] args) =>
            {
                MainClass.Config.TTS = !MainClass.Config.TTS;
                helper.WriteConfig(MainClass.Config);

                Log.Info("TTS is " + (MainClass.Config.TTS ? "on" : "off"));
            });
            #endregion
        }
    }
}

