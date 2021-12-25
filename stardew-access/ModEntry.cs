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
using StardewValley.Locations;

namespace stardew_access
{

    public class MainClass : Mod
    {
        private Harmony? harmony;
        private static bool isReadingTile = false, readTile = true, snapMouse = true;
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

            helper.ConsoleCommands.Add("ref_sr", "Refresh screen reader", (string arg1, string[] arg2) =>
            {
                ScreenReader.initializeScreenReader();

                monitor.Log("Screen Reader refreshed!", LogLevel.Info);
            });
            #endregion

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.onUpdateTicked;
        }

        private void onUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            MenuPatch.resetGlobalVars();

            SlotAndLocation.narrateCurrentSlot();

            SlotAndLocation.narrateCurrentLocation();

            if (snapMouse)
                SnapMouseToPlayer();

            if(!isReadingTile && readTile)
                ReadTile();
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

        private static async void ReadTile()
        {
            isReadingTile = true;

            try
            {
                #region Get Correct Grab Tile
                int x = Game1.player.GetBoundingBox().Center.X;
                int y = Game1.player.GetBoundingBox().Center.Y;

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
                
                x /= Game1.tileSize;
                y /= Game1.tileSize;
                Vector2 gt = new Vector2(x, y);
                #endregion

                if (!Game1.currentLocation.isTilePassable(Game1.player.nextPosition(Game1.player.getDirection()), Game1.viewport))
                {
                    ScreenReader.sayWithTileQuery("Colliding", x, y, true);
                }

                if (Context.IsPlayerFree)
                {
                    Dictionary<Vector2, Netcode.NetRef<TerrainFeature>> terrainFeature = Game1.currentLocation.terrainFeatures.FieldDict;

                    if (Game1.currentLocation.isWaterTile(x, y))
                    {
                        ScreenReader.sayWithTileQuery("Water", x, y, true);
                    }
                    else if (Game1.currentLocation.getObjectAtTile(x, y) != null)
                    {
                        #region Objects at tile (TODO)
                        StardewValley.Object obj = Game1.currentLocation.getObjectAtTile(x, y);
                        string name = obj.DisplayName;

                        // TODO add individual stone narration using parentSheetIndex
                        // monitor.Log(obj.parentSheetIndex.ToString(), LogLevel.Debug);
                        if (Game1.objectInformation.ContainsKey(obj.ParentSheetIndex) && name.ToLower().Equals("stone"))
                        {
                            string info = Game1.objectInformation[obj.parentSheetIndex];
                            if (info.ToLower().Contains("copper"))
                                name = "Copper " + name;
                        }

                        ScreenReader.sayWithTileQuery(name, x, y, true);
                        #endregion
                    }
                    else if (terrainFeature.ContainsKey(gt))
                    {
                        #region Terrain Feature
                        Netcode.NetRef<TerrainFeature> terrain = terrainFeature[gt];

                        if (terrain.Get() is HoeDirt)
                        {
                            HoeDirt dirt = (HoeDirt)terrain.Get();
                            if (dirt.crop != null)
                            {
                                string cropName = Game1.objectInformation[dirt.crop.indexOfHarvest].Split('/')[0];
                                string toSpeak = $"{cropName}";

                                bool isWatered = dirt.state.Value == HoeDirt.watered;
                                bool isHarvestable = dirt.readyForHarvest();
                                bool isFertilized = dirt.fertilizer.Value != HoeDirt.noFertilizer;

                                if (isWatered)
                                    toSpeak = "Watered " + toSpeak;

                                if (isFertilized)
                                    toSpeak = "Fertilized " + toSpeak;

                                if (isHarvestable)
                                    toSpeak = "Harvestable " + toSpeak;

                                ScreenReader.sayWithTileQuery(toSpeak, x, y, true);
                            }
                            else
                            {
                                string toSpeak = "Soil";
                                bool isWatered = dirt.state.Value == HoeDirt.watered;
                                bool isFertilized = dirt.fertilizer.Value != HoeDirt.noFertilizer;

                                if (isWatered)
                                    toSpeak = "Watered " + toSpeak;

                                if (isFertilized)
                                    toSpeak = "Fertilized " + toSpeak;

                                ScreenReader.sayWithTileQuery(toSpeak, x, y, true);
                            }
                        }
                        else if (terrain.Get() is Bush)
                        {
                            string toSpeak = "Bush";
                            ScreenReader.sayWithTileQuery(toSpeak, x, y, true);
                        }
                        else if (terrain.Get() is CosmeticPlant)
                        {
                            CosmeticPlant cosmeticPlant = (CosmeticPlant)terrain.Get();
                            string toSpeak = cosmeticPlant.textureName().ToLower();

                            if (toSpeak.Contains("terrain"))
                                toSpeak.Replace("terrain", "");

                            if (toSpeak.Contains("feature"))
                                toSpeak.Replace("feature", "");

                            ScreenReader.sayWithTileQuery(toSpeak, x, y, true);
                        }
                        else if (terrain.Get() is Flooring)
                        {
                            Flooring flooring = (Flooring)terrain.Get();
                            bool isPathway = flooring.isPathway.Get();
                            bool isSteppingStone = flooring.isSteppingStone.Get();

                            string toSpeak = "Flooring";

                            if (isPathway)
                                toSpeak = "Pathway";

                            if (isSteppingStone)
                                toSpeak = "Stepping Stone";

                            ScreenReader.sayWithTileQuery(toSpeak, x, y, true);
                        }
                        else if (terrain.Get() is FruitTree)
                        {
                            FruitTree fruitTree = (FruitTree)terrain.Get();
                            string toSpeak = Game1.objectInformation[fruitTree.treeType].Split('/')[0];

                            ScreenReader.sayWithTileQuery(toSpeak, x, y, true);
                        }
                        else if (terrain.Get() is Grass)
                        {
                            Grass grass = (Grass)terrain.Get();
                            string toSpeak = "Grass";

                            ScreenReader.sayWithTileQuery(toSpeak, x, y, true);
                        }
                        else if (terrain.Get() is Tree)
                        {
                            Tree tree = (Tree)terrain.Get();
                            int treeType = tree.treeType;
                            int stage = tree.growthStage.Value;
                            string toSpeak = "";

                            if (Game1.player.getEffectiveSkillLevel(2) >= 1)
                            {
                                toSpeak = Game1.objectInformation[308 + (int)treeType].Split('/')[0];
                            }
                            else if (Game1.player.getEffectiveSkillLevel(2) >= 1 && (int)treeType <= 3)
                            {
                                toSpeak = Game1.objectInformation[308 + (int)treeType].Split('/')[0];
                            }
                            else if (Game1.player.getEffectiveSkillLevel(2) >= 1 && (int)treeType == 8)
                            {
                                toSpeak = Game1.objectInformation[292 + (int)treeType].Split('/')[0];
                            }

                            toSpeak = $"{toSpeak}, {stage} stage";

                            ScreenReader.sayWithTileQuery(toSpeak, x, y, true);
                        }
                        else if (terrain.Get() is Quartz)
                        {
                            string toSpeak = "Quartz";
                            ScreenReader.sayWithTileQuery(toSpeak, x, y, true);
                        }
                        else if (terrain.Get() is Leaf)
                        {
                            string toSpeak = "Leaf";
                            ScreenReader.sayWithTileQuery(toSpeak, x, y, true);
                        }
                        #endregion
                    }
                    else
                    {
                        #region Resource CLumps
                        Game1.currentLocation.resourceClumps.ToList().ForEach(clump =>
                            {
                                if (clump.occupiesTile(x, y))
                                {
                                    string toSpeak;
                                    int index = clump.parentSheetIndex;

                                    switch (index)
                                    {
                                        case 600:
                                            toSpeak = "Large Stump";
                                            break;
                                        case 602:
                                            toSpeak = "Hollow Log";
                                            break;
                                        case 622:
                                            toSpeak = "Meteorite";
                                            break;
                                        case 752:
                                        case 754:
                                        case 756:
                                        case 758:
                                            toSpeak = "Mine Rock";
                                            break;
                                        case 672:
                                            toSpeak = "Boulder";
                                            break;
                                        default:
                                            toSpeak = "Unknown";
                                            break;
                                    }

                                    ScreenReader.sayWithTileQuery(toSpeak, x, y, true);
                                    return;
                                }
                            });
                        #endregion

                        #region Doors
                        Game1.currentLocation.doors.ToList().ForEach(item =>
                        {
                            item.Keys.ToList().ForEach(ydoor =>
                            {
                                if (Equals(x, ydoor.X) && Equals(y, ydoor.Y))
                                {
                                    ScreenReader.sayWithTileQuery("Door", x, y, true);
                                }
                            });
                        });
                        #endregion

                        #region Ladder
                        if (Game1.inMine || Game1.currentLocation is Mine)
                        {
                            int index = Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y].TileIndex;

                            if (index == 173 || index == 174)
                                ScreenReader.sayWithTileQuery("Ladder", x, y, true);
                        }
                        #endregion
                    }

                }
            }
            catch (Exception e)
            {
                monitor.Log($"Error in Read Tile:\n{e.Message}\n{e.StackTrace}");
            }

            await Task.Delay(100);
            isReadingTile = false;
        }
    }
}