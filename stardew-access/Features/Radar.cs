using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace stardew_access.Features
{

    /// <summary>
    /// This is a custom enum class and contains the name of groups the objects are divided into for the feature
    /// </summary>
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
        public static CATEGORY Debris = new CATEGORY("debris");
        public static CATEGORY Crops = new CATEGORY("crop");
        public static CATEGORY Trees = new CATEGORY("tree");
        public static CATEGORY Bush = new CATEGORY("bush");
        public static CATEGORY Buildings = new CATEGORY("building");
        public static CATEGORY MineItems = new CATEGORY("mine item");
        public static CATEGORY ResourceClumps = new CATEGORY("resource clump");
        public static CATEGORY Chests = new CATEGORY("chest");
        public static CATEGORY JunimoBundle = new CATEGORY("bundle");
        public static CATEGORY Doors = new CATEGORY("door"); // Also includes ladders and elevators
        public static CATEGORY Others = new CATEGORY("other");
        public static CATEGORY WaterTiles = new CATEGORY("water");

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
        public int delay, range;

        public Radar()
        {
            delay = 3000;
            range = 5;

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
            exclusions.Add("ice crystal");
            exclusions.Add("clay stone");
            exclusions.Add("fossil stone");
            exclusions.Add("street lamp");
            exclusions.Add("crop");
            exclusions.Add("tree");
            exclusions.Add("flooring");
            exclusions.Add("water");
            exclusions.Add("grass");

            /* Not excluded Categories
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
            if (MainClass.radarDebug)
                MainClass.GetMonitor().Log($"\n\nRead Tile started", StardewModdingAPI.LogLevel.Debug);

            isRunning = true;
            Vector2 currPosition = Game1.player.getTileLocation();

            closed.Clear();
            furnitures.Clear();
            npcs.Clear();

            SearchNearbyTiles(currPosition, range);

            if (MainClass.radarDebug)
                MainClass.GetMonitor().Log($"\nRead Tile stopped\n\n", StardewModdingAPI.LogLevel.Debug);

            await Task.Delay(delay);
            isRunning = false;
        }

        /// <summary>
        /// Search the area using Breadth First Search algorithm(BFS).
        /// </summary>
        /// <param name="center">The starting point.</param>
        /// <param name="limit">The limiting factor or simply radius of the search area.</param>
        /// <param name="playSound">True by default if False then it will not play sound and only return the list of detected tiles(for api).</param>
        /// <returns>A dictionary with all the detected tiles along with the name of the object on it and it's category.</returns>
        public Dictionary<Vector2, (string, string)> SearchNearbyTiles(Vector2 center, int limit, bool playSound = true)
        {
            Dictionary<Vector2, (string, string)> detectedTiles = new Dictionary<Vector2, (string, string)>();

            Queue<Vector2> toSearch = new Queue<Vector2>();
            List<Vector2> searched = new List<Vector2>();
            int[] dirX = { -1, 0, 1, 0 };
            int[] dirY = { 0, 1, 0, -1 };
            int count = 0;

            toSearch.Enqueue(center);
            searched.Add(center);

            while (toSearch.Count > 0)
            {
                Vector2 item = toSearch.Dequeue();
                if (playSound)
                    CheckTileAndPlaySound(item);
                else
                {
                    (bool, string?, string) tileInfo = CheckTile(item);
                    if (tileInfo.Item1 && tileInfo.Item2 != null)
                    {
                        // Add detected tile to the dictionary
                        detectedTiles.Add(item, (tileInfo.Item2, tileInfo.Item3));
                    }
                }
                count++;

                for (int i = 0; i < 4; i++)
                {
                    Vector2 dir = new Vector2(item.X + dirX[i], item.Y + dirY[i]);

                    if (isValid(dir, center, searched, limit))
                    {
                        toSearch.Enqueue(dir);
                        searched.Add(dir);
                    }
                }
            }

            return detectedTiles;
        }

        /// <summary>
        /// Checks if the provided tile position is within the range/radius and whether the tile has already been checked or not.
        /// </summary>
        /// <param name="item">The position of the tile to be searched.</param>
        /// <param name="center">The starting point of the search.</param>
        /// <param name="searched">The list of searched items.</param>
        /// <param name="limit">The radius of search</param>
        /// <returns>Returns true if the tile is valid for search.</returns>
        public bool isValid(Vector2 item, Vector2 center, List<Vector2> searched, int limit)
        {
            if (Math.Abs(item.X - center.X) > limit)
                return false;
            if (Math.Abs(item.Y - center.Y) > limit)
                return false;

            if (searched.Contains(item))
                return false;

            return true;
        }

        public (bool, string?, string) CheckTile(Vector2 position)
        {
            (string?, CATEGORY?) tileDetail = ReadTile.getNameWithCategoryAtTile(position);
            if (tileDetail.Item1 == null)
                return (false, null, CATEGORY.Others.ToString());

            if (tileDetail.Item2 == null)
                tileDetail.Item2 = CATEGORY.Others;

            return (true, tileDetail.Item1, tileDetail.Item2.ToString());

        }

        public void CheckTileAndPlaySound(Vector2 position)
        {
            try
            {
                if (Game1.currentLocation.isObjectAtTile((int)position.X, (int)position.Y))
                {
                    (string?, CATEGORY) objDetails = ReadTile.getObjectAtTile((int)position.X, (int)position.Y);
                    string? objectName = objDetails.Item1;
                    CATEGORY category = objDetails.Item2;
                    StardewValley.Object obj = Game1.currentLocation.getObjectAtTile((int)position.X, (int)position.Y);

                    if (objectName != null)
                    {
                        objectName = objectName.ToLower().Trim();

                        if (obj is Furniture)
                        {
                            if (!furnitures.Contains((Furniture)obj))
                            {
                                furnitures.Add((Furniture)obj);
                                PlaySoundAt(position, objectName, category);
                            }
                        }
                        else
                            PlaySoundAt(position, objectName, category);

                    }
                }
                else
                {
                    (string?, CATEGORY?) tileDetail = ReadTile.getNameWithCategoryAtTile(position);
                    if (tileDetail.Item1 != null)
                    {
                        if (tileDetail.Item2 == null)
                            tileDetail.Item2 = CATEGORY.Others;

                        PlaySoundAt(position, tileDetail.Item1, tileDetail.Item2);
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.GetMonitor().Log($"{e.Message}\n{e.StackTrace}\n{e.Source}", StardewModdingAPI.LogLevel.Error);
            }
        }

        public void PlaySoundAt(Vector2 position, string searchQuery, CATEGORY category)
        {
            #region Check whether to skip the object or not

            // Skip if player is directly looking at the tile
            if (CurrentPlayer.getNextTile().Equals(position))
                return;

            if (!radarFocus)
            {
                if ((exclusions.Contains(category.ToString().ToLower().Trim()) || exclusions.Contains(searchQuery.ToLower().Trim())))
                    return;

                // Check if a word in searchQuery matches the one in exclusions list
                string[] sqArr = searchQuery.ToLower().Trim().Split(" ");
                for (int j = 0; j < sqArr.Length; j++)
                {
                    if (exclusions.Contains(sqArr[j]))
                        return;
                }
            }
            else
            {
                if (focus.Count >= 0)
                {
                    bool found = false;

                    // Check if a word in searchQuery matches the one in focus list
                    string[] sqArr = searchQuery.ToLower().Trim().Split(" ");
                    for (int j = 0; j < sqArr.Length; j++)
                    {
                        if (focus.Contains(sqArr[j]))
                        {
                            found = true;
                            break;
                        }
                    }

                    // This condition has to be after the for loop
                    if (!found && !(focus.Contains(category.ToString().ToLower().Trim()) || focus.Contains(searchQuery.ToLower().Trim())))
                        return;
                }
                else
                    return;
            }
            #endregion

            if (MainClass.radarDebug)
                MainClass.GetMonitor().Log($"{radarFocus}\tObject:{searchQuery.ToLower().Trim()}\tPosition: X={position.X} Y={position.Y}", StardewModdingAPI.LogLevel.Debug);

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

            if (!MainClass.radarStereoSound)
                soundName = $"_mono{soundName}";

            if (category == CATEGORY.Farmers) // Villagers and farmers
                soundName = $"npc{soundName}";
            else if (category == CATEGORY.FarmAnimals) // Farm Animals
                soundName = $"npc{soundName}";
            else if (category == CATEGORY.NPCs) // Other npcs, also includes enemies
                soundName = $"npc{soundName}";
            else if (category == CATEGORY.WaterTiles) // Water tiles
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Furnitures) // Furnitures
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
            else if (category == CATEGORY.Debris) // Grass and debris
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
