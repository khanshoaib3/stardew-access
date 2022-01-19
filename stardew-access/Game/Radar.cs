using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace stardew_access.Game
{
    public class Radar
    {
        private List<Vector2> closed;
        private List<Furniture> furnitures;
        private List<NPC> npcs;
        public List<string> exclusions;
        public bool isRunning;

        public Radar()
        {
            isRunning = false;
            closed = new List<Vector2>();
            furnitures = new List<Furniture>();
            npcs = new List<NPC>();
            exclusions = new List<string>();

            exclusions.Add("stone");
            exclusions.Add("weed");
            exclusions.Add("twig");
            exclusions.Add("coloured stone");
            exclusions.Add("mine stone");
            exclusions.Add("clay stone");
            exclusions.Add("fossil stone");
            exclusions.Add("crop");
            exclusions.Add("giant crop");
            exclusions.Add("grass");
            exclusions.Add("tree");
            exclusions.Add("flooring");
            exclusions.Add("water");
            exclusions.Add("door");
        }

        public async void run()
        {
            MainClass.monitor.Log($"\n\nRead Tile started", StardewModdingAPI.LogLevel.Debug);
            isRunning = true;
            Vector2 currPosition = Game1.player.getTileLocation();
            int limit = 5;

            closed.Clear();
            furnitures.Clear();
            npcs.Clear();
            findTile(currPosition, currPosition, limit);

            MainClass.monitor.Log($"\nRead Tile stopped\n\n", StardewModdingAPI.LogLevel.Debug);
            await Task.Delay(3000);
            isRunning = false;
        }

        public void findTile(Vector2 position, Vector2 center, int limit)
        {
            if (Math.Abs(position.X - center.X) > limit)
                return;
            if (Math.Abs(position.Y - center.Y) > limit)
                return;
            if (closed.Contains(position))
                return;

            closed.Add(position);
            checkTile(position);

            Vector2 northPosition = new(position.X, position.Y-1);
            Vector2 eastPosition = new(position.X+1, position.Y);
            Vector2 westPosition = new(position.X-1, position.Y);
            Vector2 southPosition = new(position.X, position.Y+1);

            findTile(northPosition, center, limit);
            findTile(eastPosition, center, limit);
            findTile(westPosition, center, limit);
            findTile(southPosition, center, limit);
        }

        public void checkTile(Vector2 position)
        {
            try
            {
                Dictionary<Vector2, Netcode.NetRef<TerrainFeature>> terrainFeature = Game1.currentLocation.terrainFeatures.FieldDict;

                // Check for npcs
                if (Game1.currentLocation.isCharacterAtTile(position) != null && !exclusions.Contains("npc"))
                {
                    NPC npc = Game1.currentLocation.isCharacterAtTile(position);
                    if (!npcs.Contains(npc))
                    {
                        playSoundAt(position, npc.displayName);
                    }
                }
                // Check for water
                else if (Game1.currentLocation.isWaterTile((int)position.X, (int)position.Y) && !exclusions.Contains("water"))
                {
                    playSoundAt(position, null);
                }
                // Check for objects
                else if (Game1.currentLocation.isObjectAtTile((int)position.X, (int)position.Y))
                {
                    string? objectName = ReadTile.getObjectNameAtTile((int)position.X, (int)position.Y);
                    StardewValley.Object obj = Game1.currentLocation.getObjectAtTile((int)position.X, (int)position.Y);

                    if (objectName != null)
                    {
                        if (obj is Furniture && !exclusions.Contains("furniture"))
                        {
                            if (!furnitures.Contains(obj as Furniture))
                            {
                                furnitures.Add(obj as Furniture);
                                playSoundAt(position, objectName);
                            }
                        }
                        else if(obj is not Furniture)
                        {
                            playSoundAt(position, objectName);
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
                        if (tr.Get() is HoeDirt && !exclusions.Contains("crop"))
                        {
                            playSoundAt(position, terrain);
                        }
                        else if (tr.Get() is GiantCrop && !exclusions.Contains("giant crop"))
                        {
                            playSoundAt(position, terrain);
                        }
                        else if (tr.Get() is Bush && !exclusions.Contains("bush"))
                        {
                            playSoundAt(position, terrain);
                        }
                        else if (tr.Get() is CosmeticPlant && !exclusions.Contains("cosmetic plant"))
                        {
                            playSoundAt(position, terrain);
                        }
                        else if (tr.Get() is Flooring && !exclusions.Contains("flooring"))
                        {
                            playSoundAt(position, terrain);
                        }
                        else if (tr.Get() is FruitTree && !exclusions.Contains("fruit tree"))
                        {
                            playSoundAt(position, terrain);
                        }
                        else if (tr.Get() is Grass && !exclusions.Contains("grass"))
                        {
                            playSoundAt(position, terrain);
                        }
                        else if (tr.Get() is Tree && !exclusions.Contains("tree"))
                        {
                            playSoundAt(position, terrain);
                        }
                        else if (tr.Get() is Quartz && !exclusions.Contains("quartz"))
                        {
                            playSoundAt(position, terrain);
                        }
                        else if (tr.Get() is Leaf && !exclusions.Contains("leaf"))
                        {
                            playSoundAt(position, terrain);
                        }
                    }
                }
                // Check for Mine ladders
                else if (ReadTile.isMineLadderAtTile((int)position.X, (int)position.Y) && !exclusions.Contains("ladder"))
                {
                    playSoundAt(position, null);
                }
                // Check for doors
                else if (ReadTile.isDoorAtTile((int)position.X, (int)position.Y) && !exclusions.Contains("door"))
                {
                    playSoundAt(position, null);
                }
                // Check for buildings on maps
                else if (ReadTile.getBuildingAtTile((int)position.X, (int)position.Y) != null)
                {
                    playSoundAt(position, ReadTile.getBuildingAtTile((int)position.X, (int)position.Y));
                }
                // Check for resource clumps
                else if (ReadTile.getResourceClumpAtTile((int)position.X, (int)position.Y) != null)
                {
                    playSoundAt(position, ReadTile.getResourceClumpAtTile((int)position.X, (int)position.Y));
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"{e.Message}\n{e.StackTrace}\n{e.Source}", StardewModdingAPI.LogLevel.Error);
            }
        }

        public void playSoundAt(Vector2 position, String? searchQuery, bool isNPC = false)
        {
            // Skip if player is directly looking at the tile
            if (CurrentPlayer.getNextTile().Equals(position))
                return;

            if (searchQuery == null || !exclusions.Contains(searchQuery.ToLower().Trim()))
            {
                if(MainClass.radarDebug)
                    MainClass.monitor.Log($"Object:{searchQuery.ToLower().Trim()}\tPosition: X={position.X} Y={position.Y}", StardewModdingAPI.LogLevel.Debug);

                int px = (int)Game1.player.getTileX(); // Player's X postion
                int py = (int)Game1.player.getTileY(); // Player's Y postion

                int ox = (int)position.X; // Object's X postion
                int oy = (int)position.Y; // Object's Y postion

                int dx = ox - px; // Distance of object's X position
                int dy = oy - py; // Distance of object's Y position

                if(dy < 0 && (Math.Abs(dy) >= Math.Abs(dx))) // Object is at top
                {
                    if (isNPC)
                        Game1.currentLocation.localSoundAt("npc_top", position);
                    else
                        Game1.currentLocation.localSoundAt("obj_top", position);
                    MainClass.monitor.Log($"Top", StardewModdingAPI.LogLevel.Debug);
                }
                else if (dx > 0 && (Math.Abs(dx) >= Math.Abs(dy))) // Object is at right
                {
                    if (isNPC)
                        Game1.currentLocation.localSoundAt("npc_right", position);
                    else
                        Game1.currentLocation.localSoundAt("obj_right", position);
                    MainClass.monitor.Log($"Right", StardewModdingAPI.LogLevel.Debug);
                }
                else if (dx < 0 && (Math.Abs(dx) > Math.Abs(dy))) // Object is at left
                {
                    if (isNPC)
                        Game1.currentLocation.localSoundAt("npc_left", position);
                    else
                        Game1.currentLocation.localSoundAt("obj_left", position);
                    MainClass.monitor.Log($"Left", StardewModdingAPI.LogLevel.Debug);
                }
                else if (dy > 0 && (Math.Abs(dy) > Math.Abs(dx))) // Object is at bottom
                {
                    if (isNPC)
                        Game1.currentLocation.localSoundAt("npc_bottom", position);
                    else
                        Game1.currentLocation.localSoundAt("obj_bottom", position);
                    MainClass.monitor.Log($"Bottom", StardewModdingAPI.LogLevel.Debug);
                }
            }
        }
    }
}
