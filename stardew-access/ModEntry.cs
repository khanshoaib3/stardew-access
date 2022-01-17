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
using Microsoft.Xna.Framework.Audio;
using System.Text.RegularExpressions;

namespace stardew_access
{

    public class MainClass : Mod
    {
        private Harmony? harmony;
        private static bool readTile = true, snapMouse = true, isNarratingHudMessage = false, radar = false;
        public static IMonitor? monitor;
        AutoHotkeyEngine ahk;
        public static string hudMessageQueryKey = "";
        public static Radar radarFeature;

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
                ahk.ExecRaw("^Enter::\nSend {LButton}");
                ahk.ExecRaw("]::\nSend {RButton}");
                ahk.ExecRaw("+Enter::\nSend {RButton}");
            }
            catch (Exception e)
            {
                monitor.Log($"Unable to initialize AutoHotKey:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }

            ScreenReader.initializeScreenReader(); // Initialize the screen reader

            this.initializeSounds();

            harmony = new Harmony(ModManifest.UniqueID); // Init harmony

            radarFeature = new Radar();

            #endregion

            #region Harmony Patches

            #region Dialogue Patches
            harmony.Patch(
                   original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.draw), new Type[] { typeof(SpriteBatch) }),
                   postfix: new HarmonyMethod(typeof(DialoguePatches), nameof(DialoguePatches.DialoguePatch))
                );

            harmony.Patch(
               original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.receiveLeftClick)),
               postfix: new HarmonyMethod(typeof(DialoguePatches), nameof(DialoguePatches.ClearDialogueString))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.drawHoverText), new Type[] { typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(int), typeof(int), typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>) }),
                postfix: new HarmonyMethod(typeof(DialoguePatches), nameof(DialoguePatches.HoverTextPatch))
            ); 
            #endregion

            #region Title Menu Patches
            harmony.Patch(
                    original: AccessTools.Method(typeof(TitleMenu), nameof(TitleMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(TitleMenuPatches), nameof(TitleMenuPatches.TitleMenuPatch))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(LoadGameMenu.SaveFileSlot), nameof(LoadGameMenu.SaveFileSlot.Draw), new Type[] { typeof(SpriteBatch), typeof(int) }),
                postfix: new HarmonyMethod(typeof(TitleMenuPatches), nameof(TitleMenuPatches.LoadGameMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(TitleMenuPatches), nameof(TitleMenuPatches.NewGameMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CoopMenu), nameof(CoopMenu.update), new Type[] { typeof(GameTime) }),
                postfix: new HarmonyMethod(typeof(TitleMenuPatches), nameof(TitleMenuPatches.CoopMenuPatch))
            );
            #endregion

            #region Game Menu Patches
            harmony.Patch(
                original: AccessTools.Method(typeof(GameMenu), nameof(GameMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.GameMenuPatch))
            );

            harmony.Patch(
                    original: AccessTools.Method(typeof(OptionsPage), nameof(OptionsPage.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.OptionsPagePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ExitPage), nameof(ExitPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.ExitPagePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingPage), nameof(CraftingPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.CraftingPagePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.InventoryPagePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.ItemGrabMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.GeodeMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.ShopMenuPatch))
            );
            #endregion

            #region Menu Patches
            harmony.Patch(
                    original: AccessTools.Method(typeof(LetterViewerMenu), nameof(LetterViewerMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.LetterViewerMenuPatch))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(ShippingMenu), nameof(ShippingMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.ShippingMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.LevelUpMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ConfirmationDialog), nameof(ConfirmationDialog.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.ConfirmationDialogPatch))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(NamingMenu), new Type[] { typeof(NamingMenu.doneNamingBehavior), typeof(string), typeof(string) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.NamingMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(MineElevatorMenu), nameof(MineElevatorMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.MineElevatorMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(LanguageSelectionMenu), nameof(LanguageSelectionMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.LanguageSelectionMenuPatch))
            );
            #endregion

            #region Quest Patches
            harmony.Patch(
                    original: AccessTools.Method(typeof(SpecialOrdersBoard), nameof(SpecialOrdersBoard.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(QuestPatches), nameof(QuestPatches.SpecialOrdersBoardPatch))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(QuestLog), nameof(QuestLog.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(QuestPatches), nameof(QuestPatches.QuestLogPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(QuestPatches), nameof(QuestPatches.BillboardPatch))
            );
            #endregion

            #region Chat Menu Patches
            harmony.Patch(
                    original: AccessTools.Method(typeof(ChatBox), nameof(ChatBox.update), new Type[] { typeof(GameTime) }),
                    postfix: new HarmonyMethod(typeof(ChatManuPatches), nameof(ChatManuPatches.ChatBoxPatch))
                );
            #endregion

            #region On Menu CLose Patch
            harmony.Patch(
                    original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.exitThisMenu)),
                    postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.IClickableMenuOnExitPatch))
                );
            harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), nameof(Game1.exitActiveMenu)),
                    prefix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.Game1ExitActiveMenuPatch))
                );
            #endregion

            harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), nameof(Game1.playSound)),
                    prefix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.PlaySoundPatch))
                );

            #endregion

            #region Custom Commands
            helper.ConsoleCommands.Add("read_tile", "Toggle read tile feature", (string arg1, string[] arg2) =>
            {
                readTile = !readTile;

                monitor.Log("Read Tile is " + (readTile ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("snap_mouse", "Toggle snap mouse feature", (string arg1, string[] arg2) =>
            {
                snapMouse = !snapMouse;

                monitor.Log("Snap Mouse is " + (snapMouse ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("radar", "Toggle radar feature", (string arg1, string[] arg2) =>
            {
                radar = !radar;

                monitor.Log("Radar " + (snapMouse ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("r_in", "Include an object to radar", (string arg1, string[] arg2) =>
            {
                radar = !radar;

                monitor.Log("Radar " + (snapMouse ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("ref_sr", "Refresh screen reader", (string arg1, string[] arg2) =>
            {
                ScreenReader.initializeScreenReader();

                monitor.Log("Screen Reader refreshed!", LogLevel.Info);
            });
            #endregion

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.onUpdateTicked;
        }

        private void initializeSounds()
        {
            try
            {
                #region Drop Item Sound
                CueDefinition dropItemCueDef = new CueDefinition();
                dropItemCueDef.name = "sa_drop_item";
                dropItemCueDef.instanceLimit = 1;
                dropItemCueDef.limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest;
                SoundEffect dropItemAudio;
                string dropItemFilePath = Path.Combine(this.Helper.DirectoryPath, "sounds/drop_item.wav");
                using (FileStream stream = new(dropItemFilePath, FileMode.Open))
                {
                    dropItemAudio = SoundEffect.FromStream(stream);
                }
                dropItemCueDef.SetSound(dropItemAudio, Game1.audioEngine.GetCategoryIndex("Sound"), false);
                #endregion

                #region Colliding sound
                CueDefinition collidingCueDef = new CueDefinition();
                collidingCueDef.name = "sa_colliding";
                collidingCueDef.instanceLimit = 1;
                collidingCueDef.limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest;
                SoundEffect collidingAudio;
                string collidingFilePath = Path.Combine(Path.Combine(this.Helper.DirectoryPath), "sounds/colliding.wav");
                using (FileStream stream = new(collidingFilePath, FileMode.Open))
                {
                    collidingAudio = SoundEffect.FromStream(stream);
                }
                collidingCueDef.SetSound(collidingAudio, Game1.audioEngine.GetCategoryIndex("Sound"), false);
                #endregion

                #region POI sound
                CueDefinition poiCueDef = new CueDefinition();
                poiCueDef.name = "sa_poi";
                SoundEffect poiAudio;
                string poiFilePath = Path.Combine(Path.Combine(this.Helper.DirectoryPath), "sounds/sound1.wav");
                using (FileStream stream = new(poiFilePath, FileMode.Open))
                {
                    poiAudio = SoundEffect.FromStream(stream);
                }
                poiCueDef.SetSound(poiAudio, Game1.audioEngine.GetCategoryIndex("Footsteps"), false);
                #endregion

                Game1.soundBank.AddCue(dropItemCueDef);
                Game1.soundBank.AddCue(collidingCueDef);
                Game1.soundBank.AddCue(poiCueDef);
            }
            catch (Exception e)
            {
                monitor.Log($"Unable to initialize custom sounds:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        private void onUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            // Reset variables
            MenuPatches.resetGlobalVars();
            QuestPatches.resetGlobalVars();

            SlotAndLocation.narrateCurrentSlot();

            SlotAndLocation.narrateCurrentLocation();

            if (snapMouse)
                SnapMouseToPlayer();

            if(!ReadTile.isReadingTile && readTile)
                ReadTile.run();

            if(!radarFeature.isRunning && radar)
                radarFeature.run();

            if (!isNarratingHudMessage)
            {
                narrateHudMessages();
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

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

            // Narrate money at hand
            if (Equals(e.Button, SButton.R))
            {
                string toSpeak = $"You have {CurrentPlayer.getMoney()}g";
                ScreenReader.say(toSpeak, true);
            }

            // Narrate time and season
            if (Equals(e.Button, SButton.Q))
            {
                string toSpeak = $"Time is {CurrentPlayer.getTimeOfDay()} and it is {CurrentPlayer.getDay()} {CurrentPlayer.getDate()} of {CurrentPlayer.getSeason()}";
                ScreenReader.say(toSpeak, true);
            }

            // Manual read tile
            if (Equals(e.Button, SButton.J))
            {
                ReadTile.run(manuallyTriggered: true);
            }

            // Manual read tile
            if (Equals(e.Button, SButton.B))
            {
                Game1.player.controller = new PathFindController(Game1.player, Game1.currentLocation, new Point(49,13), 2);
                monitor.Log($"{Game1.player.controller.pathToEndPoint==null}", LogLevel.Debug); // true if path not found
            }
        }

        private void SnapMouseToPlayer()
        {
            int x = Game1.player.GetBoundingBox().Center.X - Game1.viewport.X;
            int y = Game1.player.GetBoundingBox().Center.Y - Game1.viewport.Y;

            int offset = 64;

            switch (Game1.player.FacingDirection)
            {
                case 0:
                    y -= offset;
                    break;
                case 1:
                    x += offset;
                    break;
                case 2:
                    y += offset;
                    break;
                case 3:
                    x -= offset;
                    break;
            }

            Game1.setMousePosition(x, y);
        }

        public static async void narrateHudMessages()
        {
            isNarratingHudMessage = true;
            try
            {
                if(Game1.hudMessages.Count > 0)
                {
                    int lastIndex = Game1.hudMessages.Count - 1;
                    HUDMessage lastMessage = Game1.hudMessages[lastIndex];
                    if (!lastMessage.noIcon)
                    {
                        string toSpeak = lastMessage.Message;
                        string searchQuery = toSpeak;

                        searchQuery = Regex.Replace(toSpeak, @"[\d+]", string.Empty);
                        searchQuery.Trim();


                        if (hudMessageQueryKey != searchQuery)
                        {
                            hudMessageQueryKey = searchQuery;

                            ScreenReader.say(toSpeak, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate hud messages:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }

            await Task.Delay(300);
            isNarratingHudMessage = false;
        }
    }
}