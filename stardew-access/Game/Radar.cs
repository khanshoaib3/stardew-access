using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Diagnostics;

namespace stardew_access.Game
{
    public class Radar
    {
        private List<Vector2> closed;
        private List<Furniture> furnitures;
        public List<string> exclusions;
        public bool isRunning;

        public Radar()
        {
            isRunning = false;
            closed = new List<Vector2>();
            furnitures = new List<Furniture>();
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
        }

        public async void run()
        {
            isRunning = true;
            Vector2 currPosition = Game1.player.getTileLocation();
            int limit = 5;

            closed.Clear();
            furnitures.Clear();
            findTile(currPosition, currPosition, limit);

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

            Vector2 northPosition = new Vector2(position.X, position.Y-1);
            Vector2 eastPosition = new Vector2(position.X+1, position.Y);
            Vector2 westPosition = new Vector2(position.X-1, position.Y);
            Vector2 southPosition = new Vector2(position.X, position.Y+1);

            findTile(northPosition, center, limit);
            findTile(eastPosition, center, limit);
            findTile(westPosition, center, limit);
            findTile(southPosition, center, limit);
        }

        public void checkTile(Vector2 position)
        {
            Dictionary<Vector2, Netcode.NetRef<TerrainFeature>> terrainFeature = Game1.currentLocation.terrainFeatures.FieldDict;

            if (Game1.currentLocation.isObjectAtTile((int)position.X, (int)position.Y))
            {
                string? obj = ReadTile.getObjectNameAtTile((int)position.X, (int)position.Y);
                StardewValley.Object @object = Game1.currentLocation.getObjectAtTile((int)position.X, (int)position.Y);

                if (@object is Furniture && !exclusions.Contains("furniture"))
                {
                    if (!furnitures.Contains(@object as Furniture))
                    {
                        furnitures.Add(@object as Furniture);
                        Game1.currentLocation.localSoundAt("sa_poi", position);
                        MainClass.monitor.Log($"FUR:{@object.DisplayName}\tX:{position.X}\tY:{position.Y}", StardewModdingAPI.LogLevel.Debug);
                    }
                }
                else if (!exclusions.Contains(obj.ToLower()))
                {
                    Game1.currentLocation.localSoundAt("sa_poi", position);
                    MainClass.monitor.Log($"OBJ:{obj}\tX:{position.X}\tY:{position.Y}", StardewModdingAPI.LogLevel.Debug);
                }
            }
            else if (terrainFeature.ContainsKey(position))
            {
                Netcode.NetRef<TerrainFeature> tr = terrainFeature[position];
                string? terrain = ReadTile.getTerrainFeatureAtTile(tr);
                if (tr != null)
                {
                    if(tr.Get() is HoeDirt && !exclusions.Contains("crop"))
                    {
                        Game1.currentLocation.localSoundAt("sa_poi", position);
                        MainClass.monitor.Log($"CROP:{terrain}\tX:{position.X}\tY:{position.Y}", StardewModdingAPI.LogLevel.Debug);
                    }
                    else if(tr.Get() is GiantCrop && !exclusions.Contains("giant crop"))
                    {
                        Game1.currentLocation.localSoundAt("sa_poi", position);
                        MainClass.monitor.Log($"BUSH:{terrain}\tX:{position.X}\tY:{position.Y}", StardewModdingAPI.LogLevel.Debug);
                    }
                    else if (tr.Get() is Bush && !exclusions.Contains("bush"))
                    {
                        Game1.currentLocation.localSoundAt("sa_poi", position);
                        MainClass.monitor.Log($"BUSH:{terrain}\tX:{position.X}\tY:{position.Y}", StardewModdingAPI.LogLevel.Debug);
                    }
                    else if (tr.Get() is CosmeticPlant && !exclusions.Contains("cosmetic plant"))
                    {
                        Game1.currentLocation.localSoundAt("sa_poi", position);
                        MainClass.monitor.Log($"COSMETIC_PLANT:{terrain}\tX:{position.X}\tY:{position.Y}", StardewModdingAPI.LogLevel.Debug);
                    }
                    else if (tr.Get() is Flooring && !exclusions.Contains("flooring"))
                    {
                        Game1.currentLocation.localSoundAt("sa_poi", position);
                        MainClass.monitor.Log($"FLOORING:{terrain}\tX:{position.X}\tY:{position.Y}", StardewModdingAPI.LogLevel.Debug);
                    }
                    else if (tr.Get() is FruitTree && !exclusions.Contains("fruit tree"))
                    {
                        Game1.currentLocation.localSoundAt("sa_poi", position);
                        MainClass.monitor.Log($"FRUTI_TREE:{terrain}\tX:{position.X}\tY:{position.Y}", StardewModdingAPI.LogLevel.Debug);
                    }
                    else if (tr.Get() is Grass && !exclusions.Contains("grass"))
                    {
                        Game1.currentLocation.localSoundAt("sa_poi", position);
                        MainClass.monitor.Log($"GRASS:{terrain}\tX:{position.X}\tY:{position.Y}", StardewModdingAPI.LogLevel.Debug);
                    }
                    else if (tr.Get() is Tree && !exclusions.Contains("tree"))
                    {
                        Game1.currentLocation.localSoundAt("sa_poi", position);
                        MainClass.monitor.Log($"TREE:{terrain}\tX:{position.X}\tY:{position.Y}", StardewModdingAPI.LogLevel.Debug);
                    }
                    else if (tr.Get() is Quartz && !exclusions.Contains("quartz"))
                    {
                        Game1.currentLocation.localSoundAt("sa_poi", position);
                        MainClass.monitor.Log($"QUARTZ:{terrain}\tX:{position.X}\tY:{position.Y}", StardewModdingAPI.LogLevel.Debug);
                    }
                    else if (tr.Get() is Leaf && !exclusions.Contains("leaf"))
                    {
                        Game1.currentLocation.localSoundAt("sa_poi", position);
                        MainClass.monitor.Log($"LEAF:{terrain}\tX:{position.X}\tY:{position.Y}", StardewModdingAPI.LogLevel.Debug);
                    }
                }
            }
        }
    }
}
