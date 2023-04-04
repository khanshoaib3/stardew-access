using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Text;

namespace stardew_access.Features
{
    public class TileInfo
    {
		private static readonly string[] trackable_machines = { "bee house", "cask", "press", "keg", "machine", "maker", "preserves jar", "bone mill", "kiln", "crystalarium", "furnace", "geode crusher", "tapper", "lightning rod", "incubator", "wood chipper", "worm bin", "loom", "statue of endless fortune", "statue of perfection", "crab pot" };
        private static readonly Dictionary<int, string> ResourceClumpNames = new()
		{
            { 600, "Large Stump" },
            { 602, "Hollow Log" },
            { 622, "Meteorite" },
            { 672, "Boulder" },
            { 752, "Mine Rock" },
            { 754, "Mine Rock" },
            { 756, "Mine Rock" },
            { 758, "Mine Rock" },
            { 190, "Giant Cauliflower" },
            { 254, "Giant Melon" },
            { 276, "Giant Pumpkin" }
        };

        ///<summary>Returns the name of the object at tile alongwith it's category's name</summary>
        public static (string? name, string? categoryName) getNameWithCategoryNameAtTile(Vector2 tile, GameLocation? currentLocation)
        {
            (string? name, CATEGORY? category) = getNameWithCategoryAtTile(tile, currentLocation);

            category ??= CATEGORY.Others;

            return (name, category.ToString());
        }

        ///<summary>Returns the name of the object at tile</summary>
        public static string? GetNameAtTile(Vector2 tile, GameLocation? currentLocation = null)
        {
            currentLocation ??= Game1.currentLocation;
            return getNameWithCategoryAtTile(tile, currentLocation).name;
        }

        ///<summary>Returns the name of the object at tile alongwith it's category</summary>
        public static (string? name, CATEGORY? category) getNameWithCategoryAtTile(Vector2 tile, GameLocation? currentLocation, bool lessInfo = false)
        {
            currentLocation ??= Game1.currentLocation;
            int x = (int)tile.X;
            int y = (int)tile.Y;

            var terrainFeature = currentLocation.terrainFeatures.FieldDict;

            if (currentLocation.isCharacterAtTile(tile) is NPC npc)
            {
                CATEGORY category = npc.isVillager() || npc.CanSocialize ? CATEGORY.Farmers : CATEGORY.NPCs;
                return (npc.displayName, category);
            }

            string? farmAnimal = getFarmAnimalAt(currentLocation, x, y);
            if (farmAnimal is not null)
            {
                return (farmAnimal, CATEGORY.FarmAnimals);
            }

            (string? name, CATEGORY category) staticTile = StaticTiles.GetStaticTileInfoAtWithCategory(x, y, currentLocation.Name);
            if (staticTile.name != null)
            {
                return (staticTile.name, staticTile.category);
            }

            (string? name, CATEGORY? category) dynamicTile = DynamicTiles.GetDynamicTileAt(x, y, currentLocation, lessInfo);
            if (dynamicTile.name != null)
            {
                return (dynamicTile.name, dynamicTile.category);
            }

            if (currentLocation.isObjectAtTile(x, y))
            {
                (string? name, CATEGORY? category) obj = getObjectAtTile(x, y, currentLocation, lessInfo);
                return (obj.name, obj.category);
            }

            if (currentLocation.isWaterTile(x, y) && !lessInfo && IsCollidingAtTile(x, y, currentLocation))
            {
                return ("Water", CATEGORY.WaterTiles);
            }

            string? resourceClump = getResourceClumpAtTile(x, y, currentLocation, lessInfo);
            if (resourceClump != null)
            {
                return (resourceClump, CATEGORY.ResourceClumps);
            }

            if (terrainFeature.TryGetValue(tile, out var tf))
            {
                (string? name, CATEGORY category) terrain = getTerrainFeatureAtTile(tf);
                if (terrain.name != null)
                {
                    return (terrain.name, terrain.category);
                }
            }

            string? bush = GetBushAtTile(x, y, currentLocation, lessInfo);
            if (bush != null)
            {
                return (bush, CATEGORY.Bush);
            }

            string? door = getDoorAtTile(x, y, currentLocation);
            string? warp = getWarpPointAtTile(x, y, currentLocation);
            if (warp != null || door != null)
            {
                return (warp ?? door, CATEGORY.Doors);
            }

            string? junimoBundle = GetJunimoBundleAt(x, y, currentLocation);
            if (junimoBundle != null)
            {
                return (junimoBundle, CATEGORY.JunimoBundle);
            }

            // Track dropped items
            if (MainClass.Config.TrackDroppedItems)
            {
                try
                {
                    foreach (var item in currentLocation.debris)
                    {
                        int xPos = ((int)item.Chunks[0].position.Value.X / Game1.tileSize) + 1;
                        int yPos = ((int)item.Chunks[0].position.Value.Y / Game1.tileSize) + 1;
                        if (xPos != x || yPos != y || item.item == null) continue;

                        string name = item.item.DisplayName;
                        int count = item.item.Stack;
                        return ($"Dropped Item: {count} {name}", CATEGORY.DroppedItems);
                    }
                }
                catch (Exception e)
                {
                    MainClass.ErrorLog($"An error occurred while detecting dropped items:\n{e.Message}");
                }
            }

            return (null, CATEGORY.Others);
        }

        /// <summary>
        /// Gets the bush at the specified tile coordinates in the provided GameLocation.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <param name="currentLocation">The GameLocation instance to search for bushes.</param>
        /// <param name="lessInfo">Whether to return less information about the bush.</param>
        /// <returns>A string describing the bush if one is found at the specified coordinates, otherwise null.</returns>
        public static string? GetBushAtTile(int x, int y, GameLocation currentLocation, bool lessInfo = false)
        {
            Bush? bush = (Bush)currentLocation.getLargeTerrainFeatureAt(x, y);

            if (bush is null || (lessInfo && (bush.tilePosition.Value.X != x || bush.tilePosition.Value.Y != y)))
                return null;

            if (!bush.townBush.Value && bush.tileSheetOffset.Value == 1 && bush.inBloom(Game1.GetSeasonForLocation(currentLocation), Game1.dayOfMonth))
            {
                string season = bush.overrideSeason.Value == -1 ? Game1.GetSeasonForLocation(currentLocation) : Utility.getSeasonNameFromNumber(bush.overrideSeason.Value);
                int shakeOff = season switch
                {
                    "spring" => 296,
                    "fall" => 410,
                    _ => -1
                };

                shakeOff = bush.size.Value switch
                {
                    3 => 815,
                    4 => 73,
                    _ => shakeOff
                };

                if (shakeOff == -1)
                {
                    return null;
                }

                return bush.townBush.Value
                    ? "Harvestable Town Bush"
                    : bush.greenhouseBush.Value
                        ? "Harvestable Greenhouse Bush"
                        : "Harvestable Bush";
            }

            return bush.townBush.Value
                ? "Town Bush"
                : bush.greenhouseBush.Value
                    ? "Greenhouse Bush"
                    : "Bush";
        }

        /// <summary>
        /// Determines if there is a Junimo bundle at the specified tile coordinates in the provided GameLocation.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <param name="currentLocation">The GameLocation instance to search for Junimo bundles.</param>
        /// <returns>The name of the Junimo bundle if one is found at the specified coordinates, otherwise null.</returns>
        public static string? GetJunimoBundleAt(int x, int y, GameLocation currentLocation)
        {
            if (currentLocation is CommunityCenter communityCenter)
            {
                // Determine the name of the bundle based on the tile coordinates
                string? name = (x, y) switch
                {
                    (14, 5) => "Pantry",
                    (14, 23) => "Crafts Room",
                    (40, 10) => "Fish Tank",
                    (63, 14) => "Boiler Room",
                    (55, 6) => "Vault",
                    (46, 12) => "Bulletin Board",
                    _ => null,
                };

                // If a bundle name is found and a note should appear in the area, return the bundle name
                if (name is not null && communityCenter.shouldNoteAppearInArea(CommunityCenter.getAreaNumberFromName(name)))
                    return $"{name} bundle";
            }
            else if (currentLocation is AbandonedJojaMart)
            {
                // Determine the name of the bundle based on the tile coordinates
                string? name = (x, y) switch
                {
                    (8, 8) => "Missing",
                    _ => null,
                };

                if (name is not null)
                    // Bundle name was found
                    return $"{name} bundle";
            }

            // No bundle was found
            return null;
        }

        /// <summary>
        /// Determines if there is a collision at the specified tile coordinates in the provided GameLocation.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <param name="currentLocation">The GameLocation instance to search for collisions.</param>
        /// <returns>True if a collision is detected at the specified tile coordinates, otherwise False.</returns>
        public static bool IsCollidingAtTile(int x, int y, GameLocation currentLocation)
        {
            // This function highly optimized over readability because `currentLocation.isCollidingPosition` takes ~30ms on the Farm map, more on larger maps I.E. Forest.
            // Return the result of the logical comparison directly, inlining operations
            // Check if the tile is NOT a warp point and if it collides with an object or terrain feature
            // OR if the tile has stumps in a Woods location
            return !isWarpPointAtTile(x, y, currentLocation) &&
                   (currentLocation.isCollidingPosition(new Rectangle(x * 64 + 1, y * 64 + 1, 62, 62), Game1.viewport, true, 0, glider: false, Game1.player, pathfinding: true) ||
                   (currentLocation is Woods woods && getStumpsInWoods(x, y, woods) is not null));
        }

        /// <summary>
        /// Returns the Warp object at the specified tile coordinates or null if not found.
        /// </summary>
        private static Warp? GetWarpAtTile(int x, int y, GameLocation currentLocation)
        {
            if (currentLocation is null) return null;

            int warpsCount = currentLocation.warps.Count;
            for (int i = 0; i < warpsCount; i++)
            {
                if (currentLocation.warps[i].X == x && currentLocation.warps[i].Y == y)
                    return currentLocation.warps[i];
            }

            return null;
        }

        /// <summary>
        /// Returns the name of the warp point at the specified tile coordinates, or null if not found.
        /// </summary>
        public static string? getWarpPointAtTile(int x, int y, GameLocation currentLocation)
        {
            Warp? warpPoint = GetWarpAtTile(x, y, currentLocation);

            if (warpPoint != null)
            {
                return $"{warpPoint.TargetName} Entrance";
            }

            return null;
        }

        /// <summary>
        /// Returns true if there's a warp point at the specified tile coordinates, or false otherwise.
        /// </summary>
        public static bool isWarpPointAtTile(int x, int y, GameLocation currentLocation)
        {
            return GetWarpAtTile(x, y, currentLocation) != null;
        }

        /// <summary>
        /// Gets the farm animal at the specified tile coordinates in the given location.
        /// </summary>
        /// <param name="location">The location where the farm animal might be found. Must be either a Farm or an AnimalHouse (coop, barn, etc).</param>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <returns>
        /// A string containing the farm animal's name, type, and age if a farm animal is found at the specified tile;
        /// null if no farm animal is found or if the location is not a Farm or an AnimalHouse.
        /// </returns>
        public static string? getFarmAnimalAt(GameLocation? location, int x, int y)
        {
            // Return null if the location is null or not a Farm or AnimalHouse
            if (location is null || !(location is Farm || location is AnimalHouse))
                return null;

            // Use an empty enumerable to store farm animals if no animals are found
            IEnumerable<FarmAnimal> farmAnimals = Enumerable.Empty<FarmAnimal>();

            // If the location is a Farm, get all the farm animals
            if (location is Farm farm)
                farmAnimals = farm.getAllFarmAnimals();
            // If the location is an AnimalHouse, get all the animals from the AnimalHouse
            else if (location is AnimalHouse animalHouse)
                farmAnimals = animalHouse.animals.Values;

            // Use LINQ to find the first farm animal at the specified tile (x, y) coordinates
            var foundAnimal = farmAnimals.FirstOrDefault(farmAnimal => farmAnimal.getTileX() == x && farmAnimal.getTileY() == y);

            // If a farm animal was found at the specified tile coordinates
            if (foundAnimal != null)
            {
                string name = foundAnimal.displayName;
                int age = foundAnimal.age.Value;
                string type = foundAnimal.displayType;

                // Return a formatted string with the farm animal's name, type, and age
                return $"{name}, {type}, age {age}";
            }

            // If no farm animal was found, return null
            return null;
        }

        /// <summary>
        /// Retrieves the name and category of the terrain feature at the given tile.
        /// </summary>
        /// <param name="terrain">A reference to the terrain feature to be checked.</param>
        /// <returns>A tuple containing the name and category of the terrain feature at the tile.</returns>
        public static (string? name, CATEGORY category) getTerrainFeatureAtTile(Netcode.NetRef<TerrainFeature> terrain)
        {
            // Get the terrain feature from the reference
            var terrainFeature = terrain.Get();
            
            // Check if the terrain feature is HoeDirt
            if (terrainFeature is HoeDirt dirt)
            {
                return (getHoeDirtDetail(dirt), CATEGORY.Crops);
            }
            // Check if the terrain feature is a CosmeticPlant
            else if (terrainFeature is CosmeticPlant cosmeticPlant)
            {
                string toReturn = cosmeticPlant.textureName().ToLower();

                toReturn = toReturn.Replace("terrain", "").Replace("feature", "");

                return (toReturn, CATEGORY.Furnitures);
            }
            // Check if the terrain feature is Flooring
            else if (terrainFeature is Flooring flooring && MainClass.Config.ReadFlooring)
            {
                bool isPathway = flooring.isPathway.Get();
                bool isSteppingStone = flooring.isSteppingStone.Get();
                string toReturn = isPathway ? "Pathway" : (isSteppingStone ? "Stepping Stone" : "Flooring");

                return (toReturn, CATEGORY.Flooring);
            }
            // Check if the terrain feature is a FruitTree
            else if (terrainFeature is FruitTree fruitTree)
            {
                return (getFruitTree(fruitTree), CATEGORY.Trees);
            }
            // Check if the terrain feature is Grass
            else if (terrainFeature is Grass)
            {
                return ("Grass", CATEGORY.Debris);
            }
            // Check if the terrain feature is a Tree
            else if (terrainFeature is Tree tree)
            {
                return (getTree(tree), CATEGORY.Trees);
            }

            return (null, CATEGORY.Others);
        }

        /// <summary>
        /// Retrieves a detailed description of HoeDirt, including its soil, plant, and other relevant information.
        /// </summary>
        /// <param name="dirt">The HoeDirt object to get details for.</param>
        /// <param name="ignoreIfEmpty">If true, the method will return an empty string for empty soil; otherwise, it will return "Soil".</param>
        /// <returns>A string representing the details of the provided HoeDirt object.</returns>
        public static string getHoeDirtDetail(HoeDirt dirt, bool ignoreIfEmpty = false)
        {
            // Use StringBuilder for efficient string manipulation
            StringBuilder detail = new();

            // Calculate isWatered and isFertilized only once
            bool isWatered = dirt.state.Value == HoeDirt.watered;
            bool isFertilized = dirt.fertilizer.Value != HoeDirt.noFertilizer;

            // Check the watered status and append it to the detail string
            if (isWatered && MainClass.Config.WateredToggle)
                detail.Append("Watered ");
            else if (!isWatered && !MainClass.Config.WateredToggle)
                detail.Append("Unwatered ");

            // Check if the dirt is fertilized and append it to the detail string
            if (isFertilized)
                detail.Append("Fertilized ");

            // Check if the dirt has a crop
            if (dirt.crop != null)
            {
                // Handle forage crops
                if (dirt.crop.forageCrop.Value)
                {
                    detail.Append(dirt.crop.whichForageCrop.Value switch
                    {
                        1 => "Spring onion",
                        2 => "Ginger",
                        _ => "Forageable crop"
                    });
                }
                else // Handle non-forage crops
                {
                    // Append the crop name to the detail string
                    string cropName = Game1.objectInformation[dirt.crop.indexOfHarvest.Value].Split('/')[0];
                    detail.Append(cropName);

                    // Check if the crop is harvestable and prepend it to the detail string
                    if (dirt.readyForHarvest())
                        detail.Insert(0, "Harvestable ");

                    // Check if the crop is dead and prepend it to the detail string
                    if (dirt.crop.dead.Value)
                        detail.Insert(0, "Dead ");
                }
            }
            else if (!ignoreIfEmpty) // If there's no crop and ignoreIfEmpty is false, append "Soil" to the detail string
            {
                detail.Append("Soil");
            }

            return detail.ToString();
        }

        /// <summary>
        /// Retrieves the fruit tree's display name based on its growth stage and fruit index.
        /// </summary>
        /// <param name="fruitTree">The FruitTree object to get the name for.</param>
        /// <returns>The fruit tree's display name.</returns>
        public static string getFruitTree(FruitTree fruitTree)
        {
            int stage = fruitTree.growthStage.Value;
            int fruitIndex = fruitTree.indexOfFruit.Get();

            // Get the base name of the fruit tree from the object information
            string toReturn = Game1.objectInformation[fruitIndex].Split('/')[0];

            // Append the growth stage description to the fruit tree name
            if (stage == 0)
                toReturn = $"{toReturn} seed";
            else if (stage == 1)
                toReturn = $"{toReturn} sprout";
            else if (stage == 2)
                toReturn = $"{toReturn} sapling";
            else if (stage == 3)
                toReturn = $"{toReturn} bush";
            else if (stage >= 4)
                toReturn = $"{toReturn} tree";

            // If there are fruits on the tree, prepend "Harvestable" to the name
            if (fruitTree.fruitsOnTree.Value > 0)
                toReturn = $"Harvestable {toReturn}";

            return toReturn;
        }

        /// <summary>
        /// Retrieves the tree's display name based on its type and growth stage.
        /// </summary>
        /// <param name="tree">The Tree object to get the name for.</param>
        /// <returns>The tree's display name.</returns>
        public static string getTree(Tree tree)
        {
            int treeType = tree.treeType.Value;
            int treeStage = tree.growthStage.Value;
			string seedName = "";

			// Handle special tree types and return their names
			switch (treeType)
            {
                case 4:
                case 5:
                    return "Winter Tree";
                case 6:
                    return "Palm Tree";
                case 7:
                    return "Mushroom Tree";
            }

            // Get the seed name for the tree type
            if (treeType <= 3)
                seedName = Game1.objectInformation[308 + treeType].Split('/')[0];
            else if (treeType == 8)
                seedName = Game1.objectInformation[292].Split('/')[0];

            // Determine the tree name and growth stage description
            if (treeStage >= 1)
            {
				string treeName;
				switch (seedName.ToLower())
				{
					case "mahogany seed":
						treeName = "Mahogany";
						break;
					case "acorn":
						treeName = "Oak";
						break;
					case "maple seed":
						treeName = "Maple";
						break;
					case "pine cone":
						treeName = "Pine";
						break;
					default:
						treeName = "Coconut";
						break;
				}

				// Append the growth stage description to the tree name
				if (treeStage == 1)
                    treeName = $"{treeName} sprout";
                else if (treeStage == 2)
                    treeName = $"{treeName} sapling";
                else if (treeStage == 3 || treeStage == 4)
                    treeName = $"{treeName} bush";
                else if (treeStage >= 5)
                    treeName = $"{treeName} tree";

                return treeName;
            }

            // Return the seed name if the tree is at stage 0
            return seedName;
        }

        #region Objects
        /// <summary>
        /// Retrieves the name and category of an object at a specific tile in the game location.
        /// </summary>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        /// <param name="currentLocation">The current game location.</param>
        /// <param name="lessInfo">An optional parameter to display less information, set to false by default.</param>
        /// <returns>A tuple containing the object's name and category.</returns>
        public static (string? name, CATEGORY category) getObjectAtTile(int x, int y, GameLocation currentLocation, bool lessInfo = false)
        {
            (string? name, CATEGORY category) toReturn = (null, CATEGORY.Others);

            // Get the object at the specified tile
            StardewValley.Object obj = currentLocation.getObjectAtTile(x, y);
            if (obj == null) return toReturn;

            int index = obj.ParentSheetIndex;
            toReturn.name = obj.DisplayName;

            // Get object names and categories based on index
            (string? name, CATEGORY category) correctNameAndCategory = getCorrectNameAndCategoryFromIndex(index, obj.Name);

            // Check the object type and assign the appropriate name and category
            if (obj is Chest chest)
            {
                toReturn = (chest.DisplayName, CATEGORY.Containers);
            }
            else if (obj is IndoorPot indoorPot)
            {
                toReturn.name = $"{obj.DisplayName}, {getHoeDirtDetail(indoorPot.hoeDirt.Value, true)}";
            }
            else if (obj is Sign sign && sign.displayItem.Value != null)
            {
                toReturn.name = $"{sign.DisplayName}, {sign.displayItem.Value.DisplayName}";
            }
            else if (obj is Furniture furniture)
            {
                if (lessInfo && (furniture.TileLocation.X != x || furniture.TileLocation.Y != y))
                {
                    toReturn.category = CATEGORY.Others;
                    toReturn.name = null;
                }
                else
                {
                    toReturn.category = CATEGORY.Furnitures;
                }
            }
            else if (obj.IsSprinkler() && obj.heldObject.Value != null) // Detect the upgrade attached to the sprinkler
            {
                string heldObjectName = obj.heldObject.Value.Name;
                if (MainClass.ModHelper is not null)
                {
                    if (heldObjectName.Contains("pressure nozzle", StringComparison.OrdinalIgnoreCase))
                    {
                        toReturn.name = MainClass.ModHelper.Translation.Get("readtile.sprinkler.pressurenozzle", new { value = toReturn.name });
                    }
                    else if (heldObjectName.Contains("enricher", StringComparison.OrdinalIgnoreCase))
                    {
                        toReturn.name = MainClass.ModHelper.Translation.Get("readtile.sprinkler.enricher", new { value = toReturn.name });
                    }
                    else
                    {
                        toReturn.name = $"{obj.heldObject.Value.DisplayName} {toReturn.name}";
                    }
                }
            }
            else if ((obj.Type == "Crafting" && obj.bigCraftable.Value) || obj.Name.Equals("crab pot", StringComparison.OrdinalIgnoreCase))
            {
                foreach (string machine in trackable_machines)
                {
                    if (obj.Name.Contains(machine, StringComparison.OrdinalIgnoreCase))
                    {
                        toReturn.name = obj.DisplayName;
                        toReturn.category = CATEGORY.Machines;
                    }
                }
            }
            else if (correctNameAndCategory.name != null)
            {
                toReturn = correctNameAndCategory;
            }
            else if (obj.name.Equals("stone", StringComparison.OrdinalIgnoreCase))
                toReturn.category = CATEGORY.Debris;
            else if (obj.name.Equals("twig", StringComparison.OrdinalIgnoreCase))
                toReturn.category = CATEGORY.Debris;
            else if (obj.name.Contains("quartz", StringComparison.OrdinalIgnoreCase))
                toReturn.category = CATEGORY.MineItems;
            else if (obj.name.Contains("earth crystal", StringComparison.OrdinalIgnoreCase))
                toReturn.category = CATEGORY.MineItems;
            else if (obj.name.Contains("frozen tear", StringComparison.OrdinalIgnoreCase))
                toReturn.category = CATEGORY.MineItems;

            if (toReturn.category == CATEGORY.Machines) // Fix for `Harvestable table` and `Busy nodes`
            {
                MachineState machineState = GetMachineState(obj);
                if (machineState == MachineState.Ready)
                    toReturn.name = $"Harvestable {toReturn.name}";
                else if (machineState == MachineState.Busy)
                    toReturn.name = $"Busy {toReturn.name}";
            }

            return toReturn;
        }

        /// <summary>
        /// Determines the state of a machine based on its properties.
        /// </summary>
        /// <param name="machine">The machine object to get the state of.</param>
        /// <returns>A MachineState enumeration value representing the machine's state.</returns>
        private static MachineState GetMachineState(StardewValley.Object machine)
        {
            // Check if the machine is a CrabPot and determine its state based on bait and heldObject
            if (machine is CrabPot crabPot)
            {
                bool hasBait = crabPot.bait.Value is not null;
                bool hasHeldObject = crabPot.heldObject.Value is not null;

                if (hasBait && !hasHeldObject)
                    return MachineState.Busy;
                else if (hasBait && hasHeldObject)
                    return MachineState.Ready;
            }

            // For other machine types, determine the state based on readyForHarvest, MinutesUntilReady, and heldObject
            return GetMachineState(machine.readyForHarvest.Value, machine.MinutesUntilReady, machine.heldObject.Value);
        }

        /// <summary>
        /// Determines the state of a machine based on its readiness for harvest, remaining minutes, and held object.
        /// </summary>
        /// <param name="readyForHarvest">A boolean indicating if the machine is ready for harvest.</param>
        /// <param name="minutesUntilReady">The number of minutes remaining until the machine is ready.</param>
        /// <param name="heldObject">The object held by the machine, if any.</param>
        /// <returns>A MachineState enumeration value representing the machine's state.</returns>
        private static MachineState GetMachineState(bool readyForHarvest, int minutesUntilReady, StardewValley.Object? heldObject)
        {
            // Determine the machine state based on the input parameters
            if (readyForHarvest || (heldObject is not null && minutesUntilReady <= 0))
            {
                return MachineState.Ready;
            }
            else if (minutesUntilReady > 0)
            {
                return MachineState.Busy;
            }
            else
            {
                return MachineState.Waiting;
            }
        }

        /// <summary>
        /// Retrieves the correct name and category for an object based on its index and name.
        /// </summary>
        /// <param name="index">The object's index value.</param>
        /// <param name="objName">The object's name.</param>
        /// <returns>A tuple containing the object's correct name and category.</returns>
        private static (string? name, CATEGORY category) getCorrectNameAndCategoryFromIndex(int index, string objName)
        {
            // Check the index for known cases and return the corresponding name and category
            switch (index)
            {
                case 313:
                case 314:
                case 315:
                case 316:
                case 317:
                case 318:
                case 452:
                case 674:
                case 675:
                case 676:
                case 677:
                case 678:
                case 679:
                case 750:
                case 784:
                case 785:
                case 786:
                    return ("Weed", CATEGORY.Debris);
                case 792:
                case 793:
                case 794:
                    return ("Fertile weed", CATEGORY.Debris);
                case 319:
                case 320:
                case 321:
                    return ("Ice crystal", CATEGORY.Debris);
                case 118:
                case 120:
                case 122:
                case 124:
                    return ("Barrel", CATEGORY.MineItems);
                case 119:
                case 121:
                case 123:
                case 125:
                    return ("Item box", CATEGORY.MineItems);
            }

            // Check if the object name contains "stone" and handle specific cases based on index
            if (objName.Contains("stone", StringComparison.OrdinalIgnoreCase))
            {
                // Return the corresponding name and category for specific stone-related objects
                switch (index)
                {
                    case 76:
                        return ("Frozen geode", CATEGORY.MineItems);
                    // ... (include other cases)
                    case 290:
                    case 850:
                        return ("Iron node", CATEGORY.MineItems);
                }
            }

            // Return null for the name and the Others category if no match is found
            return (null, CATEGORY.Others);
        }
        #endregion  

        /// <summary>
        /// Check if a tile with the specified index exists at the given coordinates in the specified location.
        /// </summary>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        /// <param name="currentLocation">The current game location.</param>
        /// <param name="targetTileIndex">The target tile index to check for.</param>
        /// <returns>True if a tile with the specified index exists at the given coordinates, false otherwise.</returns>
        private static bool CheckTileIndex(int x, int y, GameLocation currentLocation, int targetTileIndex)
        {
            var tile = currentLocation.Map.GetLayer("Buildings").Tiles[x, y];
            return tile != null && tile.TileIndex == targetTileIndex;
        }

        /// <summary>
        /// Determines if a mine down ladder is present at the specified tile location.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="currentLocation">The current GameLocation instance.</param>
        /// <returns>True if a mine down ladder is found at the specified tile, otherwise false.</returns>
        public static bool isMineDownLadderAtTile(int x, int y, GameLocation currentLocation)
        {
            return currentLocation is Mine or MineShaft || currentLocation.Name == "SkullCave" 
                   ? CheckTileIndex(x, y, currentLocation, 173)
                   : false;
        }

        /// <summary>
        /// Determines if a mine shaft is present at the specified tile location.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="currentLocation">The current GameLocation instance.</param>
        /// <returns>True if a mine shaft is found at the specified tile, otherwise false.</returns>
        public static bool isShaftAtTile(int x, int y, GameLocation currentLocation)
        {
            return currentLocation is Mine or MineShaft || currentLocation.Name == "SkullCave"
                   ? CheckTileIndex(x, y, currentLocation, 174)
                   : false;
        }

        /// <summary>
        /// Determines if a mine up ladder is present at the specified tile location.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="currentLocation">The current GameLocation instance.</param>
        /// <returns>True if a mine up ladder is found at the specified tile, otherwise false.</returns>
        public static bool isMineUpLadderAtTile(int x, int y, GameLocation currentLocation)
        {
            return currentLocation is Mine or MineShaft || currentLocation.Name == "SkullCave"
                   ? CheckTileIndex(x, y, currentLocation, 115)
                   : false;
        }

        /// <summary>
        /// Determines if an elevator is present at the specified tile location.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="currentLocation">The current GameLocation instance.</param>
        /// <returns>True if an elevator is found at the specified tile, otherwise false.</returns>
        public static bool isElevatorAtTile(int x, int y, GameLocation currentLocation)
        {
            return currentLocation is Mine or MineShaft || currentLocation.Name == "SkullCave"
                   ? CheckTileIndex(x, y, currentLocation, 112)
                   : false;
        }

        /// <summary>
        /// Gets the door information at the specified tile coordinates in the given location.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <param name="currentLocation">The GameLocation where the door might be found.</param>
        /// <returns>
        /// A string containing the door information if a door is found at the specified tile;
        /// null if no door is found.
        /// </returns>
        public static string? getDoorAtTile(int x, int y, GameLocation currentLocation)
        {
            // Create a Point object from the given tile coordinates
            Point tilePoint = new(x, y);
            
            // Access the doorList in the current location
            StardewValley.Network.NetPointDictionary<string, Netcode.NetString> doorList = currentLocation.doors;

            // Check if the doorList contains the given tile point
            if (doorList.TryGetValue(tilePoint, out string? doorName))
            {
                // Return the door information with the name if available, otherwise use "door"
                return doorName != null ? $"{doorName} door" : "door";
            }

            // No matching door found
            return null;
        }

        /// <summary>
        /// Gets the resource clump information at the specified tile coordinates in the given location.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <param name="currentLocation">The GameLocation where the resource clump might be found.</param>
        /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
        /// <returns>
        /// A string containing the resource clump information if a resource clump is found at the specified tile;
        /// null if no resource clump is found.
        /// </returns>
        public static string? getResourceClumpAtTile(int x, int y, GameLocation currentLocation, bool lessInfo = false)
        {
            // Check if the current location is Woods and handle stumps in woods separately
            if (currentLocation is Woods woods)
                return getStumpsInWoods(x, y, woods, lessInfo);

            // Iterate through resource clumps in the location using a for loop for performance reasons
            for (int i = 0, count = currentLocation.resourceClumps.Count; i < count; i++)
            {
                var resourceClump = currentLocation.resourceClumps[i];

                // Check if the resource clump occupies the tile and meets the lessInfo condition
                if (resourceClump.occupiesTile(x, y) && (!lessInfo || (resourceClump.tile.X == x && resourceClump.tile.Y == y)))
                {
                    // Get the resource clump name if available, otherwise use "Unknown"
                    return ResourceClumpNames.TryGetValue(resourceClump.parentSheetIndex.Value, out string? resourceName) ? resourceName : "Unknown";
                }
            }

            // No matching resource clump found
            return null;
        }

        /// <summary>
        /// Gets the stump information at the specified tile coordinates in the given Woods location.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <param name="woods">The Woods location where the stump might be found.</param>
        /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the stump's origin. Default is false.</param>
        /// <returns>
        /// A string containing the stump information if a stump is found at the specified tile;
        /// null if no stump is found.
        /// </returns>
        public static string? getStumpsInWoods(int x, int y, Woods woods, bool lessInfo = false)
        {
            // Iterate through stumps in the Woods location
            foreach (var stump in woods.stumps)
            {
                // Check if the stump occupies the tile and meets the lessInfo condition
                if (stump.occupiesTile(x, y) && (!lessInfo || (stump.tile.X == x && stump.tile.Y == y)))
                {
                    // Return stump information
                    return "Large Stump";
                }
            }

            // No matching stump found
            return null;
        }
    }
}
