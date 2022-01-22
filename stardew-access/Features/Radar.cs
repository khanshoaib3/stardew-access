using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace stardew_access.Game
{

    // Custom enum for category
    public class CATEGORY
    {
        private string _typeKeyWord;

        private CATEGORY(string typeKeyWord)
        {
            _typeKeyWord = typeKeyWord;
        }

        public override string ToString()
        {
            return _typeKeyWord;
        }

        public static CATEGORY Farmers = new CATEGORY("farmer");
        public static CATEGORY FarmAnimals = new CATEGORY("animal");
        public static CATEGORY NPCs = new CATEGORY("npc");
        public static CATEGORY Furnitures = new CATEGORY("furniture");
        public static CATEGORY Flooring = new CATEGORY("flooring");
        public static CATEGORY Grass = new CATEGORY("grass");
        public static CATEGORY Crops = new CATEGORY("crop");
        public static CATEGORY Trees = new CATEGORY("tree") ;
        public static CATEGORY Buildings = new CATEGORY("building");
        public static CATEGORY MineItems = new CATEGORY("mine item");
        public static CATEGORY Chests = new CATEGORY("chest");
        public static CATEGORY WaterTiles = new CATEGORY("water");
        public static CATEGORY Others = new CATEGORY("other");

    }

    public class Radar
    {
        private List<Vector2> closed;
        private List<Furniture> furnitures;
        private List<NPC> npcs;
        public List<string> exclusions;
        private List<string> temp_exclusions;
        public List<string> focus;
        public bool isRunning;
        public bool radarFocus = false;

        public Radar()
        {
            isRunning = false;
            closed = new List<Vector2>();
            furnitures = new List<Furniture>();
            npcs = new List<NPC>();
            exclusions = new List<string>();
            temp_exclusions = new List<string>();
            focus = new List<string>();

            exclusions.Add("stone");
            exclusions.Add("weed");
            exclusions.Add("twig");
            exclusions.Add("coloured stone");
            exclusions.Add("clay stone");
            exclusions.Add("fossil stone");
            exclusions.Add("crop");
            exclusions.Add("tree");
            exclusions.Add("flooring");
            exclusions.Add("water");
            exclusions.Add("grass");

            /* Not excluded
             * 
             * 
             * exclusions.Add("farmer");
             * exclusions.Add("animal");
             * exclusions.Add("npc");
             * exclusions.Add("furniture")
             * exclusions.Add("other");
             * exclusions.Add("building");
             * exclusions.Add("mine item");
             * exclusions.Add("chest"); 
             */
        }

        public async void Run()
        {
            if(MainClass.radarDebug)
                MainClass.monitor.Log($"\n\nRead Tile started", StardewModdingAPI.LogLevel.Debug);

            isRunning = true;
            Vector2 currPosition = Game1.player.getTileLocation();
            int limit = 5;

            closed.Clear();
            furnitures.Clear();
            npcs.Clear();
            FindTile(currPosition, currPosition, limit);

            if(MainClass.radarDebug)
                MainClass.monitor.Log($"\nRead Tile stopped\n\n", StardewModdingAPI.LogLevel.Debug);

            await Task.Delay(3000);
            isRunning = false;
        }

        public void FindTile(Vector2 position, Vector2 center, int limit)
        {
            if (Math.Abs(position.X - center.X) > limit)
                return;
            if (Math.Abs(position.Y - center.Y) > limit)
                return;
            if (closed.Contains(position))
                return;

            closed.Add(position);
            CheckTile(position);

            Vector2 northPosition = new(position.X, position.Y-1);
            Vector2 eastPosition = new(position.X+1, position.Y);
            Vector2 westPosition = new(position.X-1, position.Y);
            Vector2 southPosition = new(position.X, position.Y+1);

            FindTile(northPosition, center, limit);
            FindTile(eastPosition, center, limit);
            FindTile(westPosition, center, limit);
            FindTile(southPosition, center, limit);
        }

        public void CheckTile(Vector2 position)
        {
            try
            {
                Dictionary<Vector2, Netcode.NetRef<TerrainFeature>> terrainFeature = Game1.currentLocation.terrainFeatures.FieldDict;

                // Check for npcs
                if (Game1.currentLocation.isCharacterAtTile(position) != null)
                {
                    NPC npc = Game1.currentLocation.isCharacterAtTile(position);
                    if (!npcs.Contains(npc))
                    {
                        if (npc.isVillager() || npc.CanSocialize)
                            PlaySoundAt(position, npc.displayName, CATEGORY.Farmers); // Villager
                        else
                            PlaySoundAt(position, npc.displayName, CATEGORY.NPCs);
                    }
                }
                // Check for animals
                else if (ReadTile.getFarmAnimalAt(Game1.currentLocation, (int)position.X, (int)position.Y) != null)
                {
                    string name = ReadTile.getFarmAnimalAt(Game1.currentLocation, (int)position.X, (int)position.Y, onlyName: true);
                    PlaySoundAt(position, name, CATEGORY.FarmAnimals);
                }
                // Check for water
                else if (Game1.currentLocation.isWaterTile((int)position.X, (int)position.Y))
                {
                    PlaySoundAt(position, null, CATEGORY.WaterTiles);
                }
                // Check for objects
                else if (Game1.currentLocation.isObjectAtTile((int)position.X, (int)position.Y))
                {
                    string? objectName = ReadTile.getObjectNameAtTile((int)position.X, (int)position.Y);
                    StardewValley.Object obj = Game1.currentLocation.getObjectAtTile((int)position.X, (int)position.Y);

                    if (objectName != null)
                    {
                        objectName = objectName.ToLower().Trim();

                        if (obj is Furniture)
                        {
                            if (!furnitures.Contains(obj as Furniture))
                            {
                                furnitures.Add(obj as Furniture);
                                PlaySoundAt(position, objectName, CATEGORY.Furnitures);
                            }
                        }
                        else if(obj is Chest)
                        {
                            PlaySoundAt(position, objectName, CATEGORY.Chests);
                        }
                        else if (obj is not Furniture && obj is not Chest)
                        {
                            bool isMineItem = false;

                            if(objectName.Contains("node") || objectName.Contains("mystic stone") || objectName.Contains("jade stone"))
                                isMineItem = true;
                            else if (objectName.Contains("geode") || objectName.Contains("mine stone") || objectName.Contains("barrel") || objectName.Contains("item box"))
                                isMineItem = true;

                            if (isMineItem)
                                PlaySoundAt(position, objectName, CATEGORY.MineItems);
                            else
                                PlaySoundAt(position, objectName, CATEGORY.Others);
                        }
                    }
                }
                // Check for terrain features
                else if (terrainFeature.ContainsKey(position))
                {
                    Netcode.NetRef<TerrainFeature> tr = terrainFeature[position];
                    string? terrain = ReadTile.getTerrainFeatureAtTile(tr).ToLower();
                    if (terrain != null)
                    {
                        if (tr.Get() is HoeDirt)
                        {
                            PlaySoundAt(position, terrain, CATEGORY.Crops);
                        }
                        else if (tr.Get() is GiantCrop)
                        {
                            PlaySoundAt(position, terrain, CATEGORY.Crops);
                        }
                        else if (tr.Get() is Bush)
                        {
                            PlaySoundAt(position, terrain, CATEGORY.Others);
                        }
                        else if (tr.Get() is CosmeticPlant)
                        {
                            PlaySoundAt(position, terrain, CATEGORY.Furnitures);
                        }
                        else if (tr.Get() is Flooring)
                        {
                            PlaySoundAt(position, terrain, CATEGORY.Flooring);
                        }
                        else if (tr.Get() is FruitTree)
                        {
                            PlaySoundAt(position, terrain, CATEGORY.Trees);
                        }
                        else if (tr.Get() is Grass)
                        {
                            PlaySoundAt(position, terrain, CATEGORY.Grass);
                        }
                        else if (tr.Get() is Tree)
                        {
                            PlaySoundAt(position, terrain, CATEGORY.Trees);
                        }
                        else if (tr.Get() is Quartz)
                        {
                            PlaySoundAt(position, terrain, CATEGORY.Others);
                        }
                        else if (tr.Get() is Leaf)
                        {
                            PlaySoundAt(position, terrain, CATEGORY.Others);
                        }
                    }
                }
                // Check for Mine ladders
                else if (ReadTile.isMineLadderAtTile((int)position.X, (int)position.Y))
                {
                    PlaySoundAt(position, "ladder", CATEGORY.Buildings);
                }
                // Check for doors
                else if (ReadTile.isDoorAtTile((int)position.X, (int)position.Y))
                {
                    PlaySoundAt(position, "door", CATEGORY.Buildings);
                }
                // Check for buildings on maps
                else if (ReadTile.getBuildingAtTile((int)position.X, (int)position.Y) != null)
                {
                    PlaySoundAt(position, ReadTile.getBuildingAtTile((int)position.X, (int)position.Y), CATEGORY.Buildings);
                }
                // Check for resource clumps
                else if (ReadTile.getResourceClumpAtTile((int)position.X, (int)position.Y) != null)
                {
                    PlaySoundAt(position, "resource clump", CATEGORY.MineItems);
                }
                // Check for junimo bundle
                else if (ReadTile.getJunimoBundleAt((int)position.X, (int)position.Y) != null)
                {
                    PlaySoundAt(position, "junimo bundle", CATEGORY.Chests);
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"{e.Message}\n{e.StackTrace}\n{e.Source}", StardewModdingAPI.LogLevel.Error);
            }
        }

        public void PlaySoundAt(Vector2 position, String searchQuery, CATEGORY category)
        {
            // Skip if player is directly looking at the tile
            if (CurrentPlayer.getNextTile().Equals(position))
                return;

            if (!radarFocus && (exclusions.Contains(category.ToString()) || exclusions.Contains(searchQuery.ToLower().Trim())))
                return;

            if (radarFocus && (!focus.Contains(category.ToString())) && !focus.Contains(searchQuery.ToLower().Trim()))
                return;

            if (MainClass.radarDebug)
                MainClass.monitor.Log($"Object:{searchQuery.ToLower().Trim()}\tPosition: X={position.X} Y={position.Y}", StardewModdingAPI.LogLevel.Debug);

            int px = (int)Game1.player.getTileX(); // Player's X postion
            int py = (int)Game1.player.getTileY(); // Player's Y postion

            int ox = (int)position.X; // Object's X postion
            int oy = (int)position.Y; // Object's Y postion

            int dx = ox - px; // Distance of object's X position
            int dy = oy - py; // Distance of object's Y position

            if (dy < 0 && (Math.Abs(dy) >= Math.Abs(dx))) // Object is at top
            {
                Game1.currentLocation.localSoundAt(GetSoundName(category, "top"), position);
            }
            else if (dx > 0 && (Math.Abs(dx) >= Math.Abs(dy))) // Object is at right
            {
                Game1.currentLocation.localSoundAt(GetSoundName(category, "right"), position);
            }
            else if (dx < 0 && (Math.Abs(dx) > Math.Abs(dy))) // Object is at left
            {
                Game1.currentLocation.localSoundAt(GetSoundName(category, "left"), position);
            }
            else if (dy > 0 && (Math.Abs(dy) > Math.Abs(dx))) // Object is at bottom
            {
                Game1.currentLocation.localSoundAt(GetSoundName(category, "bottom"), position);
            }

        }

        public static string GetSoundName(CATEGORY category, string post)
        {
            string soundName = $"_{post}";

            if(MainClass.radarStereoSound)
                soundName = $"_mono{soundName}";

            if(category == CATEGORY.Farmers) // Villagers and farmers
                soundName = $"npc{soundName}";
            if (category == CATEGORY.FarmAnimals) // Farm Animals
                soundName = $"npc{soundName}";
            else if(category == CATEGORY.NPCs) // Other npcs, also includes enemies
                soundName = $"obj{soundName}";
            else if(category == CATEGORY.WaterTiles) // Water tiles
                soundName = $"obj{soundName}";
            else if(category == CATEGORY.Furnitures) // Furnitures
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Others) // Other Objects
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Crops) // Crops
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Trees) // Trees
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Buildings) // Buildings
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.MineItems) // Mine items
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Chests) // Chests
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Grass) // Grass
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Flooring) // Flooring
                soundName = $"obj{soundName}";
            else // Default
                soundName = $"obj{soundName}";

            return soundName;
        }

        public bool ToggleFocus()
        {
            radarFocus = !radarFocus;

            if (radarFocus)
                EnableFocus();
            else
                DisableFocus();

            return radarFocus;
        }

        public void EnableFocus()
        {
            temp_exclusions = exclusions.ToList();
            exclusions.Clear();
        }

        public void DisableFocus()
        {
            exclusions = temp_exclusions.ToList();
            temp_exclusions.Clear();
        }
    }
}
