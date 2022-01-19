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
        public static bool readTile = true, snapMouse = true, isNarratingHudMessage = false, radar = false, radarDebug = true;
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
            helper.ConsoleCommands.Add("read_tile", "Toggle read tile feature", (string commmand, string[] args) =>
            {
                readTile = !readTile;

                monitor.Log("Read Tile is " + (readTile ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("snap_mouse", "Toggle snap mouse feature", (string commmand, string[] args) =>
            {
                snapMouse = !snapMouse;

                monitor.Log("Snap Mouse is " + (snapMouse ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("radar", "Toggle radar feature", (string commmand, string[] args) =>
            {
                radar = !radar;

                monitor.Log("Radar " + (radar ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("r_debug", "Toggle debugging in radar feature", (string commmand, string[] args) =>
            {
                radarDebug = !radarDebug;

                monitor.Log("Radar debugging " + (radarDebug ? "on" : "off"), LogLevel.Info);
            });

            helper.ConsoleCommands.Add("r_ex", "Exclude an object key to radar", (string commmand, string[] args) =>
            {
                string? keyToAdd = null;

                for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                if (keyToAdd != null)
                {
                    keyToAdd = keyToAdd.Trim().ToLower();
                    radarFeature.exclusions.Add(keyToAdd);
                    monitor.Log($"Added {keyToAdd} key to exclusions.", LogLevel.Info);
                }
                else
                {
                    monitor.Log("Unable to add the key to exclusions.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("r_in", "Inlcude an object key to radar", (string commmand, string[] args) =>
            {
                string? keyToAdd = null;

                for (int i = 0; i < args.Count(); i++) { keyToAdd += " " + args[i]; }

                if (keyToAdd != null)
                {
                    keyToAdd = keyToAdd.Trim().ToLower();
                    if (radarFeature.exclusions.Contains(keyToAdd))
                    {
                        radarFeature.exclusions.Remove(keyToAdd);
                        monitor.Log($"Removed {keyToAdd} key from exclusions.", LogLevel.Info);
                    }
                    else
                    {
                        monitor.Log($"Cannot find{keyToAdd} key in exclusions.", LogLevel.Info);
                    }
                }
                else
                {
                    monitor.Log("Unable to remove the key from exclusions.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("r_list", "List all the exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                if (radarFeature.exclusions.Count>0)
                {
                    for(int i = 0;i < radarFeature.exclusions.Count; i++)
                    {
                        monitor.Log($"{i+1}) {radarFeature.exclusions[i]}", LogLevel.Info);
                    }
                }
                else
                {
                    monitor.Log("No exclusions found.", LogLevel.Info);
                }
            });

            helper.ConsoleCommands.Add("r_count", "Number of exclusions in the radar feature.", (string commmand, string[] args) =>
            {
                monitor.Log($"There are {radarFeature.exclusions.Count} exclusiond in the radar feature.", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("ref_sr", "Refresh screen reader", (string commmand, string[] args) =>
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

                #region Nearby NPCs sound
                // Npc top
                CueDefinition npcTopCD = new CueDefinition();
                npcTopCD.name = "npc_top";
                SoundEffect npcTopSE;
                string npcTopFP = Path.Combine(Path.Combine(this.Helper.DirectoryPath), "sounds/npc_top.wav");
                using (FileStream stream = new(npcTopFP, FileMode.Open))
                {
                    npcTopSE = SoundEffect.FromStream(stream);
                }
                npcTopCD.SetSound(npcTopSE, Game1.audioEngine.GetCategoryIndex("Footsteps"), false);

                // Npc left
                CueDefinition npcLeftCD = new CueDefinition();
                npcLeftCD.name = "npc_left";
                SoundEffect npcLeftSE;
                string npcLeftFP = Path.Combine(Path.Combine(this.Helper.DirectoryPath), "sounds/npc_left.wav");
                using (FileStream stream = new(npcLeftFP, FileMode.Open))
                {
                    npcLeftSE = SoundEffect.FromStream(stream);
                }
                npcLeftCD.SetSound(npcLeftSE, Game1.audioEngine.GetCategoryIndex("Footsteps"), false);

                // Npc right
                CueDefinition npcRightCD = new CueDefinition();
                npcRightCD.name = "npc_right";
                SoundEffect npcRightSE;
                string npcRightFP = Path.Combine(Path.Combine(this.Helper.DirectoryPath), "sounds/npc_right.wav");
                using (FileStream stream = new(npcRightFP, FileMode.Open))
                {
                    npcRightSE = SoundEffect.FromStream(stream);
                }
                npcRightCD.SetSound(npcRightSE, Game1.audioEngine.GetCategoryIndex("Footsteps"), false);

                // Npc bottom
                CueDefinition npcBottomCD = new CueDefinition();
                npcBottomCD.name = "npc_bottom";
                SoundEffect npcBottomSE;
                string npcBottomFP = Path.Combine(Path.Combine(this.Helper.DirectoryPath), "sounds/npc_bottom.wav");
                using (FileStream stream = new(npcBottomFP, FileMode.Open))
                {
                    npcBottomSE = SoundEffect.FromStream(stream);
                }
                npcBottomCD.SetSound(npcBottomSE, Game1.audioEngine.GetCategoryIndex("Footsteps"), false);
                #endregion

                #region Nearby objects sound
                // Object top
                CueDefinition objTopCD = new CueDefinition();
                objTopCD.name = "obj_top";
                SoundEffect objTopSE;
                string objTopFP = Path.Combine(Path.Combine(this.Helper.DirectoryPath), "sounds/obj_top.wav");
                using (FileStream stream = new(objTopFP, FileMode.Open))
                {
                    objTopSE = SoundEffect.FromStream(stream);
                }
                objTopCD.SetSound(objTopSE, Game1.audioEngine.GetCategoryIndex("Footsteps"), false);

                // Object left
                CueDefinition objLeftCD = new CueDefinition();
                objLeftCD.name = "obj_left";
                SoundEffect objLeftSE;
                string objLeftFP = Path.Combine(Path.Combine(this.Helper.DirectoryPath), "sounds/obj_left.wav");
                using (FileStream stream = new(objLeftFP, FileMode.Open))
                {
                    objLeftSE = SoundEffect.FromStream(stream);
                }
                objLeftCD.SetSound(objLeftSE, Game1.audioEngine.GetCategoryIndex("Footsteps"), false);

                // Object right
                CueDefinition objRightCD = new CueDefinition();
                objRightCD.name = "obj_right";
                SoundEffect objRightSE;
                string objRightFP = Path.Combine(Path.Combine(this.Helper.DirectoryPath), "sounds/obj_right.wav");
                using (FileStream stream = new(objRightFP, FileMode.Open))
                {
                    objRightSE = SoundEffect.FromStream(stream);
                }
                objRightCD.SetSound(objRightSE, Game1.audioEngine.GetCategoryIndex("Footsteps"), false);

                // Object bottom
                CueDefinition objBottomCD = new CueDefinition();
                objBottomCD.name = "obj_bottom";
                SoundEffect objBottomSE;
                string objBottomFP = Path.Combine(Path.Combine(this.Helper.DirectoryPath), "sounds/obj_bottom.wav");
                using (FileStream stream = new(objBottomFP, FileMode.Open))
                {
                    objBottomSE = SoundEffect.FromStream(stream);
                }
                objBottomCD.SetSound(objBottomSE, Game1.audioEngine.GetCategoryIndex("Footsteps"), false);
                #endregion

                Game1.soundBank.AddCue(dropItemCueDef);
                Game1.soundBank.AddCue(collidingCueDef);
                Game1.soundBank.AddCue(objTopCD);
                Game1.soundBank.AddCue(objLeftCD);
                Game1.soundBank.AddCue(objRightCD);
                Game1.soundBank.AddCue(objBottomCD);
                Game1.soundBank.AddCue(npcTopCD);
                Game1.soundBank.AddCue(npcLeftCD);
                Game1.soundBank.AddCue(npcRightCD);
                Game1.soundBank.AddCue(npcBottomCD);
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

            /*if (Equals(e.Button, SButton.B))
            {
                Game1.player.controller = new PathFindController(Game1.player, Game1.currentLocation, new Point(49,13), 2);
                monitor.Log($"{Game1.player.controller.pathToEndPoint==null}", LogLevel.Debug); // true if path not found
            }*/
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