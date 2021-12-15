using stardew_access.Game;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Patches;
using AutoHotkey.Interop;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

namespace stardew_access
{

    public class MainClass : Mod
    {
        private Harmony? harmony;
        private static bool isReadingTile = false, readTile = false;
        private static Vector2 prevTile;
        public static IMonitor? monitor;
        AutoHotkeyEngine ahk;

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
                ahk.ExecRaw("]::\nSend {RButton}");
            }
            catch (Exception e)
            {

                monitor.Log($"Unable to initialize AutoHotKey:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }

            ScreenReader.initializeScreenReader(); // Initialize the screen reader

            harmony = new Harmony(ModManifest.UniqueID); // Init harmony

            #endregion

            #region Harmony Patches

            harmony.Patch(
               original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.draw), new Type[] { typeof(SpriteBatch) }),
               postfix: new HarmonyMethod(typeof(DialoguePatcher), nameof(DialoguePatcher.DialoguePatch))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.receiveLeftClick)),
               postfix: new HarmonyMethod(typeof(DialoguePatcher), nameof(DialoguePatcher.ClearDialogueString))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.drawHoverText), new Type[] { typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(int), typeof(int), typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>) }),
                postfix: new HarmonyMethod(typeof(DialoguePatcher), nameof(DialoguePatcher.HoverTextPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(TitleMenu), nameof(TitleMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatch), nameof(MenuPatch.TitleMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(LoadGameMenu.SaveFileSlot), nameof(LoadGameMenu.SaveFileSlot.Draw), new Type[] { typeof(SpriteBatch), typeof(int) }),
                postfix: new HarmonyMethod(typeof(MenuPatch), nameof(MenuPatch.LoadGameMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ExitPage), nameof(ExitPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatch), nameof(MenuPatch.ExitPagePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatch), nameof(MenuPatch.NewGameMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(LetterViewerMenu), nameof(LetterViewerMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatch), nameof(MenuPatch.LetterViewerMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(QuestLog), nameof(QuestLog.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatch), nameof(MenuPatch.QuestLogPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatch), nameof(MenuPatch.BillboardPatch))
            );

            #endregion

            helper.ConsoleCommands.Add("read_tile", "Toggle read tile feature", (string arg1, string[] arg2) => {
                readTile = !readTile;
            });

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.onUpdateTicked;
        }

        private void onUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            MenuPatch.resetGlobalVars();

            SnapMouseToPlayer();

            if(!isReadingTile && readTile)
                ReadTile();
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.activeClickableMenu == null)
            {
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

                if (Equals(e.Button, SButton.J))
                {
                    Game1.pressActionButton(Game1.input.GetKeyboardState(), Game1.input.GetMouseState(), Game1.input.GetGamePadState());
                }

                if (Equals(e.Button, SButton.L))
                {
                    Game1.pressUseToolButton();
                }
                
            }
        }

        private void SnapMouseToPlayer()
        {
            int x = Game1.player.GetBoundingBox().Center.X - Game1.viewport.X;
            int y = Game1.player.GetBoundingBox().Center.Y - Game1.viewport.Y;

            Game1.setMousePosition(x, y);
        }

        private static async void ReadTile()
        {
            isReadingTile = true;
            if (Context.IsPlayerFree)
            {
                Dictionary<Vector2, Netcode.NetRef<TerrainFeature>> terrainFeature = Game1.currentLocation.terrainFeatures.FieldDict;
                Vector2 gt = Game1.player.GetGrabTile();

                StardewValley.Object obj = Game1.currentLocation.getObjectAtTile((int)gt.X, (int)gt.Y);
                if (!Equals(gt, prevTile))
                {
                    prevTile = gt;
                    if (obj != null)
                    {
                        string name = obj.name;

                        string checkQuery = $"x:{gt.X} y:{gt.Y}";

                        ScreenReader.say(name, true);

                    }
                    else if (terrainFeature.ContainsKey(gt))
                    {
                        Netcode.NetRef<TerrainFeature> terrain = terrainFeature[gt];

                        if (terrain.Get() is HoeDirt)
                        {
                            HoeDirt dirt = (HoeDirt)terrain.Get();
                            if (dirt.crop != null)
                            {
                                string cropName = Game1.objectInformation[dirt.crop.indexOfHarvest];
                                cropName = cropName.Substring(0, cropName.IndexOf("/"));

                                string toSpeak = $"{cropName}";
                                ScreenReader.say(toSpeak, true);
                            }
                        }

                    }
                }
            }

            await Task.Delay(100);
            isReadingTile = false;
        }
    }
}