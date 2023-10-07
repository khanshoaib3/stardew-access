namespace stardew_access.Features;

using Microsoft.Xna.Framework;
using Utils;
using StardewValley;
using StardewValley.Objects;

internal class Radar : FeatureBase
{
    private readonly List<Vector2> _closed;
    private readonly List<Furniture> _furniture;
    private readonly List<NPC> _npcs;
    public List<string> Exclusions;
    private List<string> _tempExclusions;
    public List<string> Focus;
    public bool IsRunning;
    public bool RadarFocus = false;
    public int Delay, Range;

    public static bool RadarDebug = false;

    private static Radar? instance;

    public new static Radar Instance
    {
        get
        {
            instance ??= new Radar();
            return instance;
        }
    }

    public Radar()
    {
        Delay = 3000;
        Range = 5;

        IsRunning = false;
        _closed = new List<Vector2>();
        _furniture = new List<Furniture>();
        _npcs = new List<NPC>();
        Exclusions = new List<string>();
        _tempExclusions = new List<string>();
        Focus = new List<string>();

        Exclusions.Add("stone");
        Exclusions.Add("weed");
        Exclusions.Add("twig");
        Exclusions.Add("coloured stone");
        Exclusions.Add("ice crystal");
        Exclusions.Add("clay stone");
        Exclusions.Add("fossil stone");
        Exclusions.Add("street lamp");
        Exclusions.Add("crop");
        Exclusions.Add("tree");
        Exclusions.Add("flooring");
        Exclusions.Add("water");
        Exclusions.Add("debris");
        Exclusions.Add("grass");
        Exclusions.Add("decoration");
        Exclusions.Add("bridge");
        Exclusions.Add("other");

        /* Not excluded Categories
         *
         *
         * exclusions.Add("farmer");
         * exclusions.Add("animal");
         * exclusions.Add("npc");
         * exclusions.Add("furniture")
         * exclusions.Add("building");
         * exclusions.Add("resource clump");
         * exclusions.Add("mine item");
         * exclusions.Add("container");
         * exclusions.Add("bundle");
         * exclusions.Add("door");
         * exclusions.Add("machine");
         * exclusions.Add("interactable");
         */
    }

    public override void Update()
    {
        RunRadarFeatureIfEnabled();

        async void RunRadarFeatureIfEnabled()
        {
            if (!IsRunning && MainClass.Config.Radar)
            {
                IsRunning = true;
                Run();
                await Task.Delay(Delay);
                IsRunning = false;
            }
        }
    }

    public void Run()
    {
        if (RadarDebug)
            Log.Debug($"\n\nRead Tile started");

        Vector2 currPosition = Game1.player.getTileLocation();

        _closed.Clear();
        _furniture.Clear();
        _npcs.Clear();

        SearchNearbyTiles(currPosition, Range);

        if (RadarDebug)
            Log.Debug($"\nRead Tile stopped\n\n");
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
        var currentLocation = Game1.currentLocation;
        Dictionary<Vector2, (string, string)> detectedTiles = new();

        Queue<Vector2> toSearch = new();
        HashSet<Vector2> searched = new();
        int[] dirX = { -1, 0, 1, 0 };
        int[] dirY = { 0, 1, 0, -1 };

        toSearch.Enqueue(center);
        searched.Add(center);

        while (toSearch.Count > 0)
        {
            Vector2 item = toSearch.Dequeue();
            if (playSound)
                CheckTileAndPlaySound(item, currentLocation);
            else
            {
                (bool, string?, string) tileInfo = CheckTile(item, currentLocation);
                if (tileInfo.Item1 && tileInfo.Item2 != null)
                {
                    // Add detected tile to the dictionary
                    detectedTiles.Add(item, (tileInfo.Item2, tileInfo.Item3));
                }
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = new(item.X + dirX[i], item.Y + dirY[i]);

                if (IsValid(dir, center, searched, limit))
                {
                    toSearch.Enqueue(dir);
                    searched.Add(dir);
                }
            }
        }

        searched.Clear();
        return detectedTiles;
    }

    /// <summary>
    /// Search the entire location using Breadth First Search algorithm(BFS).
    /// </summary>
    /// <returns>A dictionary with all the detected tiles along with the name of the object on it and it's category.</returns>
    public static Dictionary<Vector2, (string, string)> SearchLocation()
    {
        //var watch = new Stopwatch();
        //watch.Start();
        var currentLocation = Game1.currentLocation;
        Dictionary<Vector2, (string, string)> detectedTiles = new();
        Vector2 position = Vector2.Zero;
        (bool, string? name, string category) tileInfo;

        Queue<Vector2> toSearch = new();
        HashSet<Vector2> searched = new();
        int[] dirX = { -1, 0, 1, 0 };
        int[] dirY = { 0, 1, 0, -1 };
        int count = 0;

        toSearch.Enqueue(Game1.player.getTileLocation());
        searched.Add(Game1.player.getTileLocation());

        //watch.Stop();
        //var elapsedMs = watch.ElapsedMilliseconds;
        //Log.Debug($"Search init duration: {elapsedMs}");
        //watch.Reset();
        //watch.Start();
        while (toSearch.Count > 0)
        {
            Vector2 item = toSearch.Dequeue();
            tileInfo = CheckTile(item, currentLocation, true);
            if (tileInfo.Item1 && tileInfo.name != null)
            {
                // Add detected tile to the dictionary
                detectedTiles.Add(item, (tileInfo.name, tileInfo.category));
            }

            count++;

            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = new(item.X + dirX[i], item.Y + dirY[i]);

                if (!searched.Contains(dir) && (DoorUtils.IsWarpAtTile(((int)dir.X, (int)dir.Y), currentLocation) ||
                                                currentLocation.isTileOnMap(dir)))
                {
                    toSearch.Enqueue(dir);
                    searched.Add(dir);
                }
            }
        }

        //watch.Stop();
        //elapsedMs = watch.ElapsedMilliseconds;
        //Log.Debug($"Search loop duration: {elapsedMs}; {count} iterations.");
        searched.Clear();
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
    public static bool IsValid(Vector2 item, Vector2 center, HashSet<Vector2> searched, int limit)
    {
        if (Math.Abs(item.X - center.X) > limit)
            return false;
        if (Math.Abs(item.Y - center.Y) > limit)
            return false;

        if (searched.Contains(item))
            return false;

        return true;
    }

    public static (bool, string? name, string category) CheckTile(Vector2 position, GameLocation currentLocation,
        bool lessInfo = false)
    {
        (string? name, CATEGORY? category) = TileInfo.GetNameWithCategoryAtTile(position, currentLocation, lessInfo);
        if (name == null)
            return (false, null, CATEGORY.Others.ToString());

        category ??= CATEGORY.Others;

        return (true, name, category.ToString());
    }

    public void CheckTileAndPlaySound(Vector2 position, GameLocation currentLocation)
    {
        try
        {
            if (currentLocation.isObjectAtTile((int)position.X, (int)position.Y))
            {
                (string? name, CATEGORY category) objDetails =
                    TileInfo.GetObjectAtTile(currentLocation, (int)position.X, (int)position.Y);
                string? objectName = objDetails.name;
                CATEGORY category = objDetails.category;
                StardewValley.Object obj = currentLocation.getObjectAtTile((int)position.X, (int)position.Y);

                if (objectName != null)
                {
                    objectName = objectName.ToLower().Trim();

                    if (obj is Furniture furniture)
                    {
                        if (!_furniture.Contains(furniture))
                        {
                            _furniture.Add(furniture);
                            PlaySoundAt(position, objectName, category, currentLocation);
                        }
                    }
                    else
                        PlaySoundAt(position, objectName, category, currentLocation);
                }
            }
            else
            {
                (string? name, CATEGORY? category) = TileInfo.GetNameWithCategoryAtTile(position, currentLocation);
                if (name != null)
                {
                    category ??= CATEGORY.Others;

                    PlaySoundAt(position, name, category, currentLocation);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"{e.Message}\n{e.StackTrace}\n{e.Source}");
        }
    }

    public void PlaySoundAt(Vector2 position, string searchQuery, CATEGORY category, GameLocation currentLocation)
    {
        #region Check whether to skip the object or not

        // Skip if player is directly looking at the tile
        if (CurrentPlayer.FacingTile.Equals(position))
            return;

        if (!RadarFocus)
        {
            if ((Exclusions.Contains(category.ToString().ToLower().Trim()) ||
                 Exclusions.Contains(searchQuery.ToLower().Trim())))
                return;

            // Check if a word in searchQuery matches the one in exclusions list
            string[] sqArr = searchQuery.ToLower().Trim().Split(" ");
            for (int j = 0; j < sqArr.Length; j++)
            {
                if (Exclusions.Contains(sqArr[j]))
                    return;
            }
        }
        else
        {
            if (Focus.Count >= 0)
            {
                bool found = false;

                // Check if a word in searchQuery matches the one in focus list
                string[] sqArr = searchQuery.ToLower().Trim().Split(" ");
                for (int j = 0; j < sqArr.Length; j++)
                {
                    if (Focus.Contains(sqArr[j]))
                    {
                        found = true;
                        break;
                    }
                }

                // This condition has to be after the for loop
                if (!found && !(Focus.Contains(category.ToString().ToLower().Trim()) ||
                                Focus.Contains(searchQuery.ToLower().Trim())))
                    return;
            }
            else
                return;
        }

        #endregion

        if (RadarDebug)
            Log.Error($"{RadarFocus}\tObject:{searchQuery.ToLower().Trim()}\tPosition: X={position.X} Y={position.Y}");

        int px = (int)Game1.player.getTileX(); // Player's X postion
        int py = (int)Game1.player.getTileY(); // Player's Y postion

        int ox = (int)position.X; // Object's X postion
        int oy = (int)position.Y; // Object's Y postion

        int dx = ox - px; // Distance of object's X position
        int dy = oy - py; // Distance of object's Y position

        if (dy < 0 && (Math.Abs(dy) >= Math.Abs(dx))) // Object is at top
        {
            currentLocation.localSoundAt(GetSoundName(category, "top"), position);
        }
        else if (dx > 0 && (Math.Abs(dx) >= Math.Abs(dy))) // Object is at right
        {
            currentLocation.localSoundAt(GetSoundName(category, "right"), position);
        }
        else if (dx < 0 && (Math.Abs(dx) > Math.Abs(dy))) // Object is at left
        {
            currentLocation.localSoundAt(GetSoundName(category, "left"), position);
        }
        else if (dy > 0 && (Math.Abs(dy) > Math.Abs(dx))) // Object is at bottom
        {
            currentLocation.localSoundAt(GetSoundName(category, "bottom"), position);
        }
    }

    public static string GetSoundName(CATEGORY category, string post)
    {
        string soundName = $"_{post}";

        if (!MainClass.Config.RadarStereoSound)
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
        else if (category == CATEGORY.Containers) // Chests
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
        RadarFocus = !RadarFocus;

        if (RadarFocus)
            EnableFocus();
        else
            DisableFocus();

        return RadarFocus;
    }

    public void EnableFocus()
    {
        _tempExclusions = Exclusions.ToList();
        Exclusions.Clear();
    }

    public void DisableFocus()
    {
        Exclusions = _tempExclusions.ToList();
        _tempExclusions.Clear();
    }
}