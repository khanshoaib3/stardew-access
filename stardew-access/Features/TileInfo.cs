using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace stardew_access.Features
{
    public class TileInfo
    {
        public static string[] trackable_machines = { "bee house", "cask", "press", "keg", "machine", "maker", "preserves jar", "bone mill", "kiln", "crystalarium", "furnace", "geode crusher", "tapper", "lightning rod", "incubator", "wood chipper", "worm bin", "loom", "statue of endless fortune", "statue of perfection", "crab pot" };
        private static readonly Dictionary<int, string> ResourceClumpNames = new Dictionary<int, string>
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
            (string? name, CATEGORY? category) tileDetail = getNameWithCategoryAtTile(tile, currentLocation);

            if (tileDetail.category == null)
                tileDetail.category = CATEGORY.Others;

            return (tileDetail.name, tileDetail.category.ToString());
        }

        ///<summary>Returns the name of the object at tile</summary>
        public static string? getNameAtTile(Vector2 tile, GameLocation? currentLocation = null)
        {
            if (currentLocation is null) currentLocation = Game1.currentLocation;
            return getNameWithCategoryAtTile(tile, currentLocation).name;
        }

        ///<summary>Returns the name of the object at tile alongwith it's category</summary>
        public static (string? name, CATEGORY? category) getNameWithCategoryAtTile(Vector2 tile, GameLocation? currentLocation, bool lessInfo = false)
        {
            if (currentLocation is null) currentLocation = Game1.currentLocation;
            int x = (int)tile.X;
            int y = (int)tile.Y;
            string? toReturn = null;
            CATEGORY? category = CATEGORY.Others;

            // Commented out; this call takes ~30 ms by itself and is usually not used.
            // Called directly only in the if conditional where it is used.
            //bool isColliding = isCollidingAtTile(x, y, currentLocation);
            var terrainFeature = currentLocation.terrainFeatures.FieldDict;
            string? door = getDoorAtTile(x, y, currentLocation);
            string? warp = getWarpPointAtTile(x, y, currentLocation);
            (CATEGORY? category, string? name) dynamicTile = getDynamicTilesInfo(x, y, currentLocation, lessInfo);
            string? junimoBundle = getJunimoBundleAt(x, y, currentLocation);
            string? resourceClump = getResourceClumpAtTile(x, y, currentLocation, lessInfo);
            string? farmAnimal = getFarmAnimalAt(currentLocation, x, y);
            string? parrot = currentLocation is IslandLocation islandLocation ? getParrotPerchAtTile(x, y, islandLocation) : null;
            (string? name, CATEGORY category) staticTile = StaticTiles.GetStaticTileInfoAtWithCategory(x, y, currentLocation.Name);
            string? bush = getBushAtTile(x, y, currentLocation, lessInfo);

            if (currentLocation.isCharacterAtTile(tile) is NPC npc)
            {
                toReturn = npc.displayName;
                if (npc.isVillager() || npc.CanSocialize)
                    category = CATEGORY.Farmers;
                else
                    category = CATEGORY.NPCs;
            }
            else if (farmAnimal is not null)
            {
                toReturn = farmAnimal;
                category = CATEGORY.FarmAnimals;
            }
            else if (staticTile.name != null)
            {
                toReturn = staticTile.name;
                category = staticTile.category;
            }
            else if (dynamicTile.name != null)
            {
                toReturn = dynamicTile.name;
                category = dynamicTile.category;
            }
            else if (currentLocation is VolcanoDungeon && ((VolcanoDungeon)currentLocation).IsCooledLava(x, y) && !lessInfo)
            {
                toReturn = "Cooled lava";
                category = CATEGORY.WaterTiles;
            }
            else if (currentLocation is VolcanoDungeon && StardewValley.Monsters.LavaLurk.IsLavaTile((VolcanoDungeon)currentLocation, x, y) && !lessInfo)
            {
                toReturn = "Lava";
                category = CATEGORY.WaterTiles;
            }
            else if (currentLocation.isObjectAtTile(x, y))
            {
                (string? name, CATEGORY? category) obj = getObjectAtTile(x, y, currentLocation, lessInfo);
                toReturn = obj.name;
                category = obj.category;
            }
            else if (currentLocation.isWaterTile(x, y) && !lessInfo && isCollidingAtTile(x, y, currentLocation))
            {
                toReturn = "Water";
                category = CATEGORY.WaterTiles;
            }
            else if (resourceClump != null)
            {
                toReturn = resourceClump;
                category = CATEGORY.ResourceClumps;
            }
            else if (terrainFeature.ContainsKey(tile))
            {
                (string? name, CATEGORY category) tf = getTerrainFeatureAtTile(terrainFeature[tile]);
                string? terrain = tf.name;
                if (terrain != null)
                {
                    toReturn = terrain;
                    category = tf.category;
                }

            }
            else if (bush != null)
            {
                toReturn = bush;
                category = CATEGORY.Bush;
            }
            else if (warp != null)
            {
                toReturn = warp;
                category = CATEGORY.Doors;
            }
            else if (door != null)
            {
                toReturn = door;
                category = CATEGORY.Doors;
            }
            else if (isMineDownLadderAtTile(x, y, currentLocation))
            {
                toReturn = "Ladder";
                category = CATEGORY.Doors;
            }
            else if (isShaftAtTile(x, y, currentLocation))
            {
                toReturn = "Shaft";
                category = CATEGORY.Doors;
            }
            else if (isMineUpLadderAtTile(x, y, currentLocation))
            {
                toReturn = "Up Ladder";
                category = CATEGORY.Doors;
            }
            else if (isElevatorAtTile(x, y, currentLocation))
            {
                toReturn = "Elevator";
                category = CATEGORY.Doors;
            }
            else if (parrot != null)
            {
                toReturn = parrot;
                category = CATEGORY.Buildings;
            }
            else if (junimoBundle != null)
            {
                toReturn = junimoBundle;
                category = CATEGORY.JunimoBundle;
            }

            #region Track dropped items
            if (MainClass.Config.TrackDroppedItems)
            {
                try
                {
                    NetCollection<Debris> droppedItems = currentLocation.debris;
                    int droppedItemsCount = droppedItems.Count();
                    if (droppedItemsCount > 0)
                    {
                        for (int i = 0; i < droppedItemsCount; i++)
                        {
                            var item = droppedItems[i];
                            int xPos = ((int)item.Chunks[0].position.Value.X / Game1.tileSize) + 1;
                            int yPos = ((int)item.Chunks[0].position.Value.Y / Game1.tileSize) + 1;
                            if (xPos != x || yPos != y) continue;

                            if (item.item == null) continue;

                            string name = item.item.DisplayName;
                            int count = item.item.Stack;

                            if (toReturn is null)
                                return ($"Dropped Item: {count} {name}", CATEGORY.DroppedItems);
                            else
                                toReturn = $"{toReturn}, Dropped Item: {count} {name}";
                            item = null;
                        }
                    }
                }
                catch (Exception e)
                {
                    MainClass.ErrorLog($"An error occured while detecting dropped items:\n{e.Message}");
                }
            }
            #endregion

            return (toReturn, category);
        }

        public static string? getBushAtTile(int x, int y, GameLocation currentLocation, bool lessInfo = false)
        {
            string? toReturn = null;
            Bush? bush = (Bush)currentLocation.getLargeTerrainFeatureAt(x, y);
            if (bush is null)
                return null;
            if (lessInfo && (bush.tilePosition.Value.X != x || bush.tilePosition.Value.Y != y))
                return null;

            int size = bush.size.Value;

            #region Check if bush is harvestable or not
            if (!bush.townBush.Value && (int)bush.tileSheetOffset.Value == 1 && bush.inBloom(Game1.GetSeasonForLocation(currentLocation), Game1.dayOfMonth))
            {
                // Taken from the game's code
                string season = ((int)bush.overrideSeason.Value == -1) ? Game1.GetSeasonForLocation(currentLocation) : Utility.getSeasonNameFromNumber(bush.overrideSeason.Value);
                int shakeOff = -1;
                if (!(season == "spring"))
                {
                    if (season == "fall")
                    {
                        shakeOff = 410;
                    }
                }
                else
                {
                    shakeOff = 296;
                }
                if ((int)size == 3)
                {
                    shakeOff = 815;
                }
                if ((int)size == 4)
                {
                    shakeOff = 73;
                }
                if (shakeOff == -1)
                {
                    return null;
                }

                toReturn = "Harvestable";
            }
            #endregion

            if (bush.townBush.Value)
                toReturn = $"{toReturn} Town Bush";
            else if (bush.greenhouseBush.Value)
                toReturn = $"{toReturn} Greenhouse Bush";
            else
                toReturn = $"{toReturn} Bush";

            return toReturn;
        }

        public static string? getJunimoBundleAt(int x, int y, GameLocation currentLocation)
        {
            string? name = null;
            if (currentLocation is CommunityCenter communityCenter)
            {
                name = (x, y) switch
                {
                    (14, 5) => "Pantry",
                    (14, 23) => "Crafts Room",
                    (40, 10) => "Fish Tank",
                    (63, 14) => "Boiler Room",
                    (55, 6) => "Vault",
                    (46, 12) => "Bulletin Board",
                    _ => null,
                };
                if (name is not null && communityCenter.shouldNoteAppearInArea(CommunityCenter.getAreaNumberFromName(name)))
                    return $"{name} bundle";
            }
            else if (currentLocation is AbandonedJojaMart)
            {
                name = (x, y) switch
                {
                    (8, 8) => "Missing",
                    _ => null,
                };

                if (name is not null)
                    return $"{name} bundle";
            }

            return null;
        }

        public static bool isCollidingAtTile(int x, int y, GameLocation currentLocation)
        {
            Rectangle rect = new Rectangle(x * 64 + 1, y * 64 + 1, 62, 62);

            /* Reference
            // Check whether the position is a warp point, if so then return false, sometimes warp points are 1 tile off the map for example in coops and barns
            if (isWarpPointAtTile(x, y, currentLocation)) return false;

            if (currentLocation.isCollidingPosition(rect, Game1.viewport, true, 0, glider: false, Game1.player, pathfinding: true))
            {
                return true;
            }

            if (currentLocation is Woods && getStumpsInWoods(x, y, currentLocation) is not null)
                return true;

            return false;
            */
            
            // Optimized
            // Sometimes warp points are 1 tile off the map for example in coops and barns; check that this is not a warp point
            if (!isWarpPointAtTile(x, y, currentLocation)) 
            {
                // not a warp point
                //directly return the value of the logical comparison rather than wasting time in conditional
                return currentLocation.isCollidingPosition(rect, Game1.viewport, true, 0, glider: false, Game1.player, pathfinding: true) || (currentLocation is Woods woods && getStumpsInWoods(x, y, woods) is not null);
            }
            // was a warp point; return false
            return false;
        }

        public static Boolean isWarpPointAtTile(int x, int y, GameLocation currentLocation)
        {
            if (currentLocation is null) return false;

            int warpsCount = currentLocation.warps.Count();
            for (int i = 0; i < warpsCount; i++)
            {
                if (currentLocation.warps[i].X == x && currentLocation.warps[i].Y == y) return true;
            }

            return false;
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
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>category: This is the category of the tile. Default to Furnitures.
        /// <br/>name: This is the name of the tile. Default to null if the tile tile has nothing on it.</returns>
        public static (CATEGORY? category, string? name) getDynamicTilesInfo(int x, int y, GameLocation currentLocation, bool lessInfo = false)
        {
            if (currentLocation.orePanPoint.Value != Point.Zero && currentLocation.orePanPoint.Value == new Point(x, y))
            {
                return (CATEGORY.Interactables, "panning spot");
            }
            else if (currentLocation is Farm farm)
            {
                if (farm.GetMainMailboxPosition().X == x && farm.GetMainMailboxPosition().Y == y)
                    return (CATEGORY.Interactables, "Mail box");
                else
                {
                    Building building = farm.getBuildingAt(new Vector2(x, y));
                    if (building is not null)
                    {
                        string name = building.buildingType.Value;

                        // Prepend fish name for fish ponds
                        if (building is FishPond fishPond)
                        {
                            if (fishPond.fishType.Value >= 0)
                            {
                                name = $"{Game1.objectInformation[fishPond.fishType.Value].Split('/')[4]} {name}";
                            }
                        }

                        // Detect doors, input slots, etc.
                        if ((building.humanDoor.Value.X + building.tileX.Value) == x && (building.humanDoor.Value.Y + building.tileY.Value) == y)
                            return (CATEGORY.Doors, name + " Door");
                        else if ((building.animalDoor.Value.X + building.tileX.Value) == x && (building.animalDoor.Value.Y + building.tileY.Value) == y)
                            return (CATEGORY.Doors, name + " Animal Door " + ((building.animalDoorOpen.Value) ? "Opened" : "Closed"));
                        else if (building.tileX.Value == x && building.tileY.Value == y)
                            return (CATEGORY.Buildings, name);
                        else if (building is Mill && (building.tileX.Value + 1) == x && (building.tileY.Value + 1) == y)
                            return (CATEGORY.Buildings, name + " input");
                        else if (building is Mill && (building.tileX.Value + 3) == x && (building.tileY.Value + 1) == y)
                            return (CATEGORY.Buildings, name + " output");
                        else
                            return (CATEGORY.Buildings, name);
                    }
                }
            }
            else if (currentLocation.currentEvent is not null)
            {
                string event_name = currentLocation.currentEvent.FestivalName;
                if (event_name == "Egg Festival" && x == 21 && y == 55)
                {
                    return (CATEGORY.Interactables, "Egg Festival Shop");
                }
                else if (event_name == "Flower Dance" && x == 28 && y == 37)
                {
                    return (CATEGORY.Interactables, "Flower Dance Shop");
                }
                else if (event_name == "Luau" && x == 35 && y == 13)
                {
                    return (CATEGORY.Interactables, "Soup Pot");
                }
                else if (event_name == "Spirit's Eve" && x == 25 && y == 49)
                {
                    return (CATEGORY.Interactables, "Spirit's Eve Shop");
                }
                else if (event_name == "Stardew Valley Fair")
                {
                    if (x == 16 && y == 52)
                        return (CATEGORY.Interactables, "Stardew Valley Fair Shop");
                    else if (x == 23 && y == 62)
                        return (CATEGORY.Interactables, "Slingshot Game");
                    else if (x == 34 && y == 65)
                        return (CATEGORY.Interactables, "Purchase Star Tokens");
                    else if (x == 33 && y == 70)
                        return (CATEGORY.Interactables, "The Wheel");
                    else if (x == 23 && y == 70)
                        return (CATEGORY.Interactables, "Fishing Challenge");
                    else if (x == 47 && y == 87)
                        return (CATEGORY.Interactables, "Fortune Teller");
                    else if (x == 38 && y == 59)
                        return (CATEGORY.Interactables, "Grange Display");
                    else if (x == 30 && y == 56)
                        return (CATEGORY.Interactables, "Strength Game");
                    else if (x == 26 && y == 33)
                        return (CATEGORY.Interactables, "Free Burgers");
                }
                else if (event_name == "Festival of Ice" && x == 55 && y == 31)
                {
                    return (CATEGORY.Interactables, "Travelling Cart");
                }
                else if (event_name == "Feast of the Winter Star" && x == 18 && y == 61)
                {
                    return (CATEGORY.Interactables, "Feast of the Winter Star Shop");
                }

            }
            else if (currentLocation is Town)
            {
                if (SpecialOrder.IsSpecialOrdersBoardUnlocked() && x == 62 && y == 93)
                    return (CATEGORY.Interactables, "Special quest board");
            }
            else if (currentLocation is FarmHouse farmHouse)
            {
                if (farmHouse.upgradeLevel >= 1)
                    if (farmHouse.getKitchenStandingSpot().X == x && (farmHouse.getKitchenStandingSpot().Y - 1) == y)
                        return (CATEGORY.Interactables, "Stove");
                    else if ((farmHouse.getKitchenStandingSpot().X + 1) == x && (farmHouse.getKitchenStandingSpot().Y - 1) == y)
                        return (CATEGORY.Others, "Sink");
                    else if (farmHouse.fridgePosition.X == x && farmHouse.fridgePosition.Y == y)
                        return (CATEGORY.Interactables, "Fridge");
            }
            else if (currentLocation is IslandFarmHouse islandFarmHouse)
            {
                if ((islandFarmHouse.fridgePosition.X - 2) == x && islandFarmHouse.fridgePosition.Y == y)
                    return (CATEGORY.Interactables, "Stove");
                else if ((islandFarmHouse.fridgePosition.X - 1) == x && islandFarmHouse.fridgePosition.Y == y)
                    return (CATEGORY.Others, "Sink");
                else if (islandFarmHouse.fridgePosition.X == x && islandFarmHouse.fridgePosition.Y == y)
                    return (CATEGORY.Interactables, "Fridge");
            }
            else if (currentLocation is Forest forest)
            {
                if (forest.travelingMerchantDay && x == 27 && y == 11)
                    return (CATEGORY.Interactables, "Travelling Cart");
                else if (forest.log != null && x == 2 && y == 7)
                    return (CATEGORY.Interactables, "Log");
                else if (forest.log == null && x == 0 && y == 7)
                    return (CATEGORY.Doors, "Secret Woods Entrance");
            }
            else if (currentLocation is Beach beach)
            {
                if (MainClass.ModHelper == null)
                    return (null, null);

                if (MainClass.ModHelper.Reflection.GetField<NPC>(beach, "oldMariner").GetValue() is NPC mariner && mariner.getTileLocation() == new Vector2(x, y))
                {
                    return (CATEGORY.NPCs, "Old Mariner");
                }
                else if (x == 58 && y == 13)
                {
                    if (!beach.bridgeFixed.Value)
                        return (CATEGORY.Interactables, "Repair Bridge");
                    else
                        return (CATEGORY.Bridges, "Bridge");
                }
            }
            else if (currentLocation is CommunityCenter communityCenter)
            {
                if (communityCenter.missedRewardsChestVisible.Value && x == 22 && y == 10)
                    return (CATEGORY.Containers, "Missed Rewards Chest");
            }
            else if (currentLocation is BoatTunnel)
            {
                if (x == 4 && y == 9)
                    return (CATEGORY.Interactables, ((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatFixed")) ? "Repair " : "") + "Ticket Machine");
                else if (x == 6 && y == 8)
                    return (((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull")) ? CATEGORY.Interactables : CATEGORY.Decor), ((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull")) ? "Repair " : "") + "Boat Hull");
                else if (x == 8 && y == 9)
                    return (((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor")) ? CATEGORY.Interactables : CATEGORY.Decor), ((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor")) ? "Repair " : "") + "Boat Anchor");
            }
            else if (currentLocation is IslandLocation islandLocation)
            {
                var nutTracker = Game1.player.team.collectedNutTracker;
                if (islandLocation.IsBuriedNutLocation(new Point(x, y)) && !nutTracker.ContainsKey($"Buried_{islandLocation.Name}_{x}_{y}"))
                {
                    return (CATEGORY.Interactables, "Diggable spot");
                }
                else if (islandLocation.locationGemBird.Value is IslandGemBird bird && ((int)bird.position.X / Game1.tileSize) == x && ((int)bird.position.Y / Game1.tileSize) == y)
                {
                    return (CATEGORY.NPCs, GetGemBirdName(bird));
                }
                else if (currentLocation is IslandWest islandWest)
                {
                    if ((islandWest.shippingBinPosition.X == x || (islandWest.shippingBinPosition.X + 1) == x) && islandWest.shippingBinPosition.Y == y)
                        return (CATEGORY.Interactables, "Shipping Bin");
                }
                else if (currentLocation is IslandNorth islandNorth)
                {
                    if (islandNorth.traderActivated.Value && x == 36 && y == 71)
                        return (CATEGORY.Interactables, "Island Trader");
                }
            }
            else if (currentLocation.Name.Equals("coop", StringComparison.OrdinalIgnoreCase))
            {
                if (x >= 6 && x <= 9 && y == 3)
                {
                    (string? name, CATEGORY category) bench = getObjectAtTile(x, y, currentLocation, true);
                    if (bench.name is not null && bench.name.Contains("hay", StringComparison.OrdinalIgnoreCase))
                        return (CATEGORY.Others, "Feeding Bench");
                    else
                        return (CATEGORY.Others, "Empty Feeding Bench");
                }
            }
            else if (currentLocation.Name.Equals("coop2", StringComparison.OrdinalIgnoreCase) || currentLocation.Name.Equals("big coop", StringComparison.OrdinalIgnoreCase))
            {
                if (x >= 6 && x <= 13 && y == 3)
                {
                    (string? name, CATEGORY category) bench = getObjectAtTile(x, y, currentLocation, true);
                    if (bench.name is not null && bench.name.Contains("hay", StringComparison.OrdinalIgnoreCase))
                        return (CATEGORY.Others, "Feeding Bench");
                    else
                        return (CATEGORY.Others, "Empty Feeding Bench");
                }
            }
            else if (currentLocation.Name.Equals("coop3", StringComparison.OrdinalIgnoreCase) || currentLocation.Name.Equals("deluxe coop", StringComparison.OrdinalIgnoreCase))
            {
                if (x >= 6 && x <= 17 && y == 3)
                {
                    (string? name, CATEGORY category) bench = getObjectAtTile(x, y, currentLocation, true);
                    if (bench.name is not null && bench.name.Contains("hay", StringComparison.OrdinalIgnoreCase))
                        return (CATEGORY.Others, "Feeding Bench");
                    else
                        return (CATEGORY.Others, "Empty Feeding Bench");
                }
            }
            else if (currentLocation.Name.Equals("barn", StringComparison.OrdinalIgnoreCase))
            {
                if (x >= 8 && x <= 11 && y == 3)
                {
                    (string? name, CATEGORY category) bench = getObjectAtTile(x, y, currentLocation, true);
                    if (bench.name != null && bench.name.Contains("hay", StringComparison.OrdinalIgnoreCase))
                        return (CATEGORY.Others, "Feeding Bench");
                    else
                        return (CATEGORY.Others, "Empty Feeding Bench");
                }
            }
            else if (currentLocation.Name.Equals("barn2", StringComparison.OrdinalIgnoreCase) || currentLocation.Name.Equals("big barn", StringComparison.OrdinalIgnoreCase))
            {
                if (x >= 8 && x <= 15 && y == 3)
                {
                    (string? name, CATEGORY category) bench = getObjectAtTile(x, y, currentLocation, true);
                    if (bench.name is not null && bench.name.Contains("hay", StringComparison.OrdinalIgnoreCase))
                        return (CATEGORY.Others, "Feeding Bench");
                    else
                        return (CATEGORY.Others, "Empty Feeding Bench");
                }
            }
            else if (currentLocation.Name.Equals("barn3", StringComparison.OrdinalIgnoreCase) || currentLocation.Name.Equals("deluxe barn", StringComparison.OrdinalIgnoreCase))
            {
                if (x >= 8 && x <= 19 && y == 3)
                {
                    (string? name, CATEGORY category) bench = getObjectAtTile(x, y, currentLocation, true);
                    if (bench.name is not null && bench.name.Contains("hay", StringComparison.OrdinalIgnoreCase))
                        return (CATEGORY.Others, "Feeding Bench");
                    else
                        return (CATEGORY.Others, "Empty Feeding Bench");
                }
            }
            else if (currentLocation is LibraryMuseum libraryMuseum)
            {
                foreach (KeyValuePair<Vector2, int> pair in libraryMuseum.museumPieces.Pairs)
                {
                    if (pair.Key.X == x && pair.Key.Y == y)
                    {
                        string displayName = Game1.objectInformation[pair.Value].Split('/')[0];
                        return (CATEGORY.Interactables, $"{displayName} showcase");
                    }
                }

                int booksFound = Game1.netWorldState.Value.LostBooksFound.Value;
                for (int x1 = 0; x1 < libraryMuseum.map.Layers[0].LayerWidth; x1++)
                {
                    for (int y1 = 0; y1 < libraryMuseum.map.Layers[0].LayerHeight; y1++)
                    {
                        if (x != x1 || y != y1) continue;

                        if (libraryMuseum.doesTileHaveProperty(x1, y1, "Action", "Buildings") != null && libraryMuseum.doesTileHaveProperty(x1, y1, "Action", "Buildings").Contains("Notes"))
                        {
                            int key = Convert.ToInt32(libraryMuseum.doesTileHaveProperty(x1, y1, "Action", "Buildings").Split(' ')[1]);
                            xTile.Tiles.Tile tile = libraryMuseum.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location(x * 64, y * 64), Game1.viewport.Size);
                            string? action = null;
                            try
                            {
                                tile.Properties.TryGetValue("Action", out xTile.ObjectModel.PropertyValue? value);
                                if (value != null) action = value.ToString();
                            }
                            catch (System.Exception e)
                            {
                                MainClass.ErrorLog($"Cannot get action value at x:{x} y:{y} in LibraryMuseum");
                                MainClass.ErrorLog(e.Message);
                            }

                            if (action != null)
                            {
                                string[] actionParams = action.Split(' ');

                                try
                                {
                                    int which = Convert.ToInt32(actionParams[1]);
                                    if (booksFound >= which)
                                    {
                                        string message = Game1.content.LoadString("Strings\\Notes:" + which);
                                        return (CATEGORY.Interactables, $"{message.Split('\n')[0]} Book");
                                    }
                                }
                                catch (System.Exception e)
                                {
                                    MainClass.ErrorLog(e.Message);
                                }

                                return (CATEGORY.Others, $"Lost Book");
                            }
                        }
                    }
                }
            }
            return (null, null);
        }

        public static (string? name, CATEGORY category) getTerrainFeatureAtTile(Netcode.NetRef<TerrainFeature> terrain)
        {
            string? toReturn = null;
            CATEGORY category = CATEGORY.Others;

            if (terrain.Get() is HoeDirt dirt)
            {
                toReturn = getHoeDirtDetail(dirt);
                category = CATEGORY.Crops;
            }
            else if (terrain.Get() is CosmeticPlant)
            {
                category = CATEGORY.Furnitures;
                CosmeticPlant cosmeticPlant = (CosmeticPlant)terrain.Get();
                toReturn = cosmeticPlant.textureName().ToLower();

                if (toReturn.Contains("terrain"))
                    toReturn.Replace("terrain", "");

                if (toReturn.Contains("feature"))
                    toReturn.Replace("feature", "");
            }
            else if (terrain.Get() is Flooring && MainClass.Config.ReadFlooring)
            {
                category = CATEGORY.Flooring;
                Flooring flooring = (Flooring)terrain.Get();
                bool isPathway = flooring.isPathway.Get();
                bool isSteppingStone = flooring.isSteppingStone.Get();

                toReturn = "Flooring";

                if (isPathway)
                    toReturn = "Pathway";

                if (isSteppingStone)
                    toReturn = "Stepping Stone";
            }
            else if (terrain.Get() is FruitTree)
            {
                category = CATEGORY.Trees;
                toReturn = getFruitTree((FruitTree)terrain.Get());
            }
            else if (terrain.Get() is Grass)
            {
                category = CATEGORY.Debris;
                toReturn = "Grass";
            }
            else if (terrain.Get() is Tree)
            {
                category = CATEGORY.Trees;
                toReturn = getTree((Tree)terrain.Get());
            }

            return (toReturn, category);


        }

        /// <summary>
        /// Returns the detail about the HoeDirt i.e. soil, plant, etc.
        /// </summary>
        /// <param name="dirt">The HoeDirt to be checked</param>
        /// <param name="ignoreIfEmpty">Ignores returning `soil` if empty</param>
        /// <returns>The details about the given HoeDirt</returns>
        public static string getHoeDirtDetail(HoeDirt dirt, bool ignoreIfEmpty = false)
        {
            string detail;

            if (dirt.crop != null && !dirt.crop.forageCrop.Value)
            {
                string cropName = Game1.objectInformation[dirt.crop.indexOfHarvest.Value].Split('/')[0];
                detail = $"{cropName}";

                bool isWatered = dirt.state.Value == HoeDirt.watered;
                bool isHarvestable = dirt.readyForHarvest();
                bool isFertilized = dirt.fertilizer.Value != HoeDirt.noFertilizer;

                if (isWatered && MainClass.Config.WateredToggle)
                    detail = "Watered " + detail;
                else if (!isWatered && !MainClass.Config.WateredToggle)
                    detail = "Unwatered " + detail;

                if (isFertilized)
                    detail = "Fertilized " + detail;

                if (isHarvestable)
                    detail = "Harvestable " + detail;

                if (dirt.crop.dead.Value)
                    detail = "Dead " + detail;
            }
            else if (dirt.crop != null && dirt.crop.forageCrop.Value)
            {
                detail = dirt.crop.whichForageCrop.Value switch
                {
                    1 => "Spring onion",
                    2 => "Ginger",
                    _ => "Forageable crop"
                };
            }
            else
            {
                detail = (ignoreIfEmpty) ? "" : "Soil";
                bool isWatered = dirt.state.Value == HoeDirt.watered;
                bool isFertilized = dirt.fertilizer.Value != HoeDirt.noFertilizer;

                if (isWatered && MainClass.Config.WateredToggle)
                    detail = "Watered " + detail;
                else if (!isWatered && !MainClass.Config.WateredToggle)
                    detail = "Unwatered " + detail;

                if (isFertilized)
                    detail = "Fertilized " + detail;
            }
            return detail;
        }

        public static string getFruitTree(FruitTree fruitTree)
        {
            int stage = fruitTree.growthStage.Value;
            int fruitIndex = fruitTree.indexOfFruit.Get();

            string toReturn = Game1.objectInformation[fruitIndex].Split('/')[0];

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

            if (fruitTree.fruitsOnTree.Value > 0)
                toReturn = $"Harvestable {toReturn}";

            return toReturn;
        }

        public static string getTree(Tree tree)
        {
            int treeType = tree.treeType.Value;
            int treeStage = tree.growthStage.Value;
            string treeName = "tree";
            string seedName = "";

            // Return with the name if it's one of the 3 special trees
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


            if (treeType <= 3)
                seedName = Game1.objectInformation[308 + treeType].Split('/')[0];
            else if (treeType == 8)
                seedName = Game1.objectInformation[292].Split('/')[0];

            if (treeStage >= 1)
            {
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
        /// Determines if a mine down ladder is present at the specified tile location.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="currentLocation">The current GameLocation instance.</param>
        /// <returns>True if a mine down ladder is found at the specified tile, otherwise false.</returns>
        public static bool isMineDownLadderAtTile(int x, int y, GameLocation currentLocation)
        {
            // Check if the current location is a Mine, MineShaft, or has the Name "SkullCave"
            if (currentLocation is Mine or MineShaft || currentLocation.Name == "SkullCave")
            {
                // Get the tile from the "Buildings" layer
                var tile = currentLocation.Map.GetLayer("Buildings").Tiles[x, y];

                // Check if the tile is not null and its TileIndex is 173, which represents a mine down ladder
                return tile != null && tile.TileIndex == 173;
            }

            // No mine down ladder found at the specified tile
            return false;
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
            // Check if the current location is a Mine, MineShaft, or has the Name "SkullCave"
            if (currentLocation is Mine or MineShaft || currentLocation.Name == "SkullCave")
            {
                // Get the tile from the "Buildings" layer
                var tile = currentLocation.Map.GetLayer("Buildings").Tiles[x, y];

                // Check if the tile is not null and its TileIndex is 174, which represents a mine shaft
                return tile != null && tile.TileIndex == 174;
            }

            // No mine shaft found at the specified tile
            return false;
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
            // Check if the current location is a Mine, MineShaft, or has the Name "SkullCave"
            if (currentLocation is Mine or MineShaft || currentLocation.Name == "SkullCave")
            {
                // Get the tile from the "Buildings" layer
                var tile = currentLocation.Map.GetLayer("Buildings").Tiles[x, y];

                // Check if the tile is not null and its TileIndex is 115, which represents a mine up ladder
                return tile != null && tile.TileIndex == 115;
            }

            // No mine up ladder found at the specified tile
            return false;
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
            // Check if the current location is a Mine, MineShaft, or has Name == "SkullCave"
            // This accommodates the mod that adds the mine's elevator to the SkullCave.
            if (currentLocation is Mine or MineShaft || currentLocation.Name == "SkullCave")
            {
                // Get the tile from the "Buildings" layer
                var tile = currentLocation.Map.GetLayer("Buildings").Tiles[x, y];

                // Check if the tile is not null and its TileIndex is 112, which represents an elevator
                return tile != null && tile.TileIndex == 112;
            }

            // Location doesn't have elevators.
            return false;
        }

        /// <summary>
        /// Get the warp point information at the specified tile location.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="currentLocation">The current GameLocation instance.</param>
        /// <returns>The warp point information as a string, or null if no warp point is found.</returns>
        public static string? getWarpPointAtTile(int x, int y, GameLocation currentLocation)
        {
            // Check if the current location is null
            if (currentLocation == null)
            {
                return null;
            }

            // Iterate through the warp points in the current location
            int warpCount = currentLocation.warps.Count;
            for (int i = 0; i < warpCount; i++)
            {
                Warp warpPoint = currentLocation.warps[i];

                // Check if the warp point matches the specified tile coordinates
                if (warpPoint.X == x && warpPoint.Y == y)
                {
                    // Return the warp point information
                    return $"{warpPoint.TargetName} Entrance";
                }
            }

            // No warp point found at the specified tile
            return null;
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
            Point tilePoint = new Point(x, y);
            
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

        /// <summary>
        /// Gets the parrot perch information at the specified tile coordinates in the given island location.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile to check.</param>
        /// <param name="y">The y-coordinate of the tile to check.</param>
        /// <param name="islandLocation">The IslandLocation where the parrot perch might be found.</param>
        /// <returns>
        /// A string containing the parrot perch information if a parrot perch is found at the specified tile;
        /// null if no parrot perch is found.
        /// </returns>
        public static string? getParrotPerchAtTile(int x, int y, IslandLocation islandLocation)
        {
            // Use LINQ to find the first parrot perch at the specified tile (x, y) coordinates
            var foundPerch = islandLocation.parrotUpgradePerches.FirstOrDefault(perch => perch.tilePosition.Value.Equals(new Point(x, y)));

            // If a parrot perch was found at the specified tile coordinates
            if (foundPerch != null)
            {
                string toSpeak = $"Parrot required nuts {foundPerch.requiredNuts.Value}";

                // Return appropriate string based on the current state of the parrot perch
                switch (foundPerch.currentState.Value)
                {
                    case StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Idle:
                        return foundPerch.IsAvailable() ? toSpeak : "Empty parrot perch";
                    case StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.StartBuilding:
                        return "Parrots started building request";
                    case StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Building:
                        return "Parrots building request";
                    case StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Complete:
                        return "Request Completed";
                    default:
                        return toSpeak;
                }
            }

            // If no parrot perch was found, return null
            return null;
        }

        /// <summary>
        /// Retrieves the name of the IslandGemBird based on its item index value.
        /// </summary>
        /// <param name="bird">The IslandGemBird instance.</param>
        /// <returns>A string representing the name of the IslandGemBird.</returns>
        public static String GetGemBirdName(IslandGemBird bird)
        {
            // Use a switch expression to return the appropriate bird name based on the item index value
            return bird.itemIndex.Value switch
            {
                60 => "Emerald Gem Bird",
                62 => "Aquamarine Gem Bird",
                64 => "Ruby Gem Bird",
                66 => "Amethyst Gem Bird",
                68 => "Topaz Gem Bird",
                _ => "Gem Bird", // Default case for when the item index does not match any of the specified values
            };
        }
    }
}
