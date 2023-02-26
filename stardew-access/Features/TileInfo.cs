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

        ///<summary>Returns the name of the object at tile alongwith it's category's name</summary>
        public static (string? name, string? categoryName) getNameWithCategoryNameAtTile(Vector2 tile)
        {
            (string? name, CATEGORY? category) tileDetail = getNameWithCategoryAtTile(tile);

            if (tileDetail.category == null)
                tileDetail.category = CATEGORY.Others;

            return (tileDetail.name, tileDetail.category.ToString());
        }

        ///<summary>Returns the name of the object at tile</summary>
        public static string? getNameAtTile(Vector2 tile)
        {
            return getNameWithCategoryAtTile(tile).name;
        }

        ///<summary>Returns the name of the object at tile alongwith it's category</summary>
        public static (string? name, CATEGORY? category) getNameWithCategoryAtTile(Vector2 tile, bool lessInfo = false)
        {
            int x = (int)tile.X;
            int y = (int)tile.Y;
            string? toReturn = null;
            CATEGORY? category = CATEGORY.Others;

            bool isColliding = isCollidingAtTile(x, y);
            var terrainFeature = Game1.currentLocation.terrainFeatures.FieldDict;
            string? door = getDoorAtTile(x, y);
            string? warp = getWarpPointAtTile(x, y);
            (CATEGORY? category, string? name) dynamicTile = getDynamicTilesInfo(x, y, lessInfo);
            string? junimoBundle = getJunimoBundleAt(x, y);
            string? resourceClump = getResourceClumpAtTile(x, y, lessInfo);
            string? farmAnimal = getFarmAnimalAt(Game1.currentLocation, x, y);
            string? parrot = getParrotPerchAtTile(x, y);
            (string? name, CATEGORY category) staticTile = MainClass.STiles.getStaticTileInfoAtWithCategory(x, y);
            string? bush = getBushAtTile(x, y, lessInfo);

            if (Game1.currentLocation.isCharacterAtTile(tile) is NPC npc)
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
            else if (Game1.currentLocation is VolcanoDungeon && ((VolcanoDungeon)Game1.currentLocation).IsCooledLava(x, y) && !lessInfo)
            {
                toReturn = "Cooled lava";
                category = CATEGORY.WaterTiles;
            }
            else if (Game1.currentLocation is VolcanoDungeon && StardewValley.Monsters.LavaLurk.IsLavaTile((VolcanoDungeon)Game1.currentLocation, x, y) && !lessInfo)
            {
                toReturn = "Lava";
                category = CATEGORY.WaterTiles;
            }
            else if (Game1.currentLocation.isObjectAtTile(x, y))
            {
                (string? name, CATEGORY? category) obj = getObjectAtTile(x, y, lessInfo);
                toReturn = obj.name;
                category = obj.category;
            }
            else if (Game1.currentLocation.isWaterTile(x, y) && isColliding && !lessInfo)
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
            else if (isMineDownLadderAtTile(x, y))
            {
                toReturn = "Ladder";
                category = CATEGORY.Doors;
            }
            else if (isShaftAtTile(x, y))
            {
                toReturn = "Shaft";
                category = CATEGORY.Doors;
            }
            else if (isMineUpLadderAtTile(x, y))
            {
                toReturn = "Up Ladder";
                category = CATEGORY.Doors;
            }
            else if (isElevatorAtTile(x, y))
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
                    NetCollection<Debris> droppedItems = Game1.currentLocation.debris;
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

        public static string? getBushAtTile(int x, int y, bool lessInfo = false)
        {
            string? toReturn = null;
            Bush? bush = (Bush)Game1.currentLocation.getLargeTerrainFeatureAt(x, y);
            if (bush is null)
                return null;
            if (lessInfo && (bush.tilePosition.Value.X != x || bush.tilePosition.Value.Y != y))
                return null;

            int size = bush.size.Value;

            #region Check if bush is harvestable or not
            if (!bush.townBush.Value && (int)bush.tileSheetOffset.Value == 1 && bush.inBloom(Game1.GetSeasonForLocation(Game1.currentLocation), Game1.dayOfMonth))
            {
                // Taken from the game's code
                string season = ((int)bush.overrideSeason.Value == -1) ? Game1.GetSeasonForLocation(Game1.currentLocation) : Utility.getSeasonNameFromNumber(bush.overrideSeason.Value);
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

        public static string? getJunimoBundleAt(int x, int y)
        {
            string? name = null;
            if (Game1.currentLocation is CommunityCenter communityCenter)
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
            else if (Game1.currentLocation is AbandonedJojaMart)
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

        public static bool isCollidingAtTile(int x, int y)
        {
            Rectangle rect = new Rectangle(x * 64 + 1, y * 64 + 1, 62, 62);

            /* Reference
            // Check whether the position is a warp point, if so then return false, sometimes warp points are 1 tile off the map for example in coops and barns
            if (isWarpPointAtTile(x, y)) return false;

            if (Game1.currentLocation.isCollidingPosition(rect, Game1.viewport, true, 0, glider: false, Game1.player, pathfinding: true))
            {
                return true;
            }

            if (Game1.currentLocation is Woods && getStumpsInWoods(x, y) is not null)
                return true;

            return false;
            */
            
            // Optimized
            // Sometimes warp points are 1 tile off the map for example in coops and barns; check that this is not a warp point
            if (!isWarpPointAtTile(x, y)) 
            {
                // not a warp point
                //directly return the value of the logical comparison rather than wasting time in conditional
                return ((Game1.currentLocation.isCollidingPosition(rect, Game1.viewport, true, 0, glider: false, Game1.player, pathfinding: true)) || (Game1.currentLocation is Woods && getStumpsInWoods(x, y) is not null));
            }
            // was a warp point; return false
            return false;
        }

        public static Boolean isWarpPointAtTile(int x, int y)
        {
            if (Game1.currentLocation is null) return false;

            int warpsCount = Game1.currentLocation.warps.Count();
            for (int i = 0; i < warpsCount; i++)
            {
                if (Game1.currentLocation.warps[i].X == x && Game1.currentLocation.warps[i].Y == y) return true;
            }

            return false;
        }

        public static string? getFarmAnimalAt(GameLocation? location, int x, int y)
        {
            if (location is null || (location is not Farm && location is not AnimalHouse))
                return null;

            //if (location is not Farm && location is not AnimalHouse)
                //return null;

            List<FarmAnimal>? farmAnimals = null;

            if (location is Farm)
                farmAnimals = ((Farm)location).getAllFarmAnimals();
            else if (location is AnimalHouse)
                farmAnimals = ((AnimalHouse)location).animals.Values.ToList();

            if (farmAnimals == null || farmAnimals.Count <= 0)
                return null;

            for (int i = 0; i < farmAnimals.Count; i++)
            {
                int fx = farmAnimals[i].getTileX();
                int fy = farmAnimals[i].getTileY();

                if (fx.Equals(x) && fy.Equals(y))
                {
                    string name = farmAnimals[i].displayName;
                    int age = farmAnimals[i].age.Value;
                    string type = farmAnimals[i].displayType;

                    return $"{name}, {type}, age {age}";
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>category: This is the category of the tile. Default to Furnitures.
        /// <br/>name: This is the name of the tile. Default to null if the tile tile has nothing on it.</returns>
        public static (CATEGORY? category, string? name) getDynamicTilesInfo(int x, int y, bool lessInfo = false)
        {
            if (Game1.currentLocation.orePanPoint.Value != Point.Zero && Game1.currentLocation.orePanPoint.Value == new Point(x, y))
            {
                return (CATEGORY.Interactables, "panning spot");
            }
            else if (Game1.currentLocation is Farm farm)
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
            else if (Game1.currentLocation.currentEvent is not null)
            {
                string event_name = Game1.currentLocation.currentEvent.FestivalName;
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
            else if (Game1.currentLocation is Town)
            {
                if (SpecialOrder.IsSpecialOrdersBoardUnlocked() && x == 62 && y == 93)
                    return (CATEGORY.Interactables, "Special quest board");
            }
            else if (Game1.currentLocation is FarmHouse farmHouse)
            {
                if (farmHouse.upgradeLevel >= 1)
                    if (farmHouse.getKitchenStandingSpot().X == x && (farmHouse.getKitchenStandingSpot().Y - 1) == y)
                        return (CATEGORY.Interactables, "Stove");
                    else if ((farmHouse.getKitchenStandingSpot().X + 1) == x && (farmHouse.getKitchenStandingSpot().Y - 1) == y)
                        return (CATEGORY.Others, "Sink");
                    else if (farmHouse.fridgePosition.X == x && farmHouse.fridgePosition.Y == y)
                        return (CATEGORY.Interactables, "Fridge");
            }
            else if (Game1.currentLocation is IslandFarmHouse islandFarmHouse)
            {
                if ((islandFarmHouse.fridgePosition.X - 2) == x && islandFarmHouse.fridgePosition.Y == y)
                    return (CATEGORY.Interactables, "Stove");
                else if ((islandFarmHouse.fridgePosition.X - 1) == x && islandFarmHouse.fridgePosition.Y == y)
                    return (CATEGORY.Others, "Sink");
                else if (islandFarmHouse.fridgePosition.X == x && islandFarmHouse.fridgePosition.Y == y)
                    return (CATEGORY.Interactables, "Fridge");
            }
            else if (Game1.currentLocation is Forest forest)
            {
                if (forest.travelingMerchantDay && x == 27 && y == 11)
                    return (CATEGORY.Interactables, "Travelling Cart");
                else if (forest.log != null && x == 2 && y == 7)
                    return (CATEGORY.Interactables, "Log");
                else if (forest.log == null && x == 0 && y == 7)
                    return (CATEGORY.Doors, "Secret Woods Entrance");
            }
            else if (Game1.currentLocation is Beach beach)
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
            else if (Game1.currentLocation is CommunityCenter communityCenter)
            {
                if (communityCenter.missedRewardsChestVisible.Value && x == 22 && y == 10)
                    return (CATEGORY.Containers, "Missed Rewards Chest");
            }
            else if (Game1.currentLocation is BoatTunnel)
            {
                if (x == 4 && y == 9)
                    return (CATEGORY.Interactables, ((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatFixed")) ? "Repair " : "") + "Ticket Machine");
                else if (x == 6 && y == 8)
                    return (((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull")) ? CATEGORY.Interactables : CATEGORY.Decor), ((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull")) ? "Repair " : "") + "Boat Hull");
                else if (x == 8 && y == 9)
                    return (((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor")) ? CATEGORY.Interactables : CATEGORY.Decor), ((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor")) ? "Repair " : "") + "Boat Anchor");
            }
            else if (Game1.currentLocation is IslandLocation islandLocation)
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
                else if (Game1.currentLocation is IslandWest islandWest)
                {
                    if ((islandWest.shippingBinPosition.X == x || (islandWest.shippingBinPosition.X + 1) == x) && islandWest.shippingBinPosition.Y == y)
                        return (CATEGORY.Interactables, "Shipping Bin");
                }
                else if (Game1.currentLocation is IslandNorth islandNorth)
                {
                    if (islandNorth.traderActivated.Value && x == 36 && y == 71)
                        return (CATEGORY.Interactables, "Island Trader");
                }
            }
            else if (Game1.currentLocation.Name.ToLower().Equals("coop"))
            {
                if (x >= 6 && x <= 9 && y == 3)
                {
                    (string? name, CATEGORY category) bench = getObjectAtTile(x, y, true);
                    if (bench.name != null && bench.name.ToLower().Contains("hay"))
                        return (CATEGORY.Others, "Feeding Bench");
                    else
                        return (CATEGORY.Others, "Empty Feeding Bench");
                }
            }
            else if (Game1.currentLocation.Name.ToLower().Equals("big coop") || Game1.currentLocation.Name.ToLower().Equals("coop2"))
            {
                if (x >= 6 && x <= 13 && y == 3)
                {
                    (string? name, CATEGORY category) bench = getObjectAtTile(x, y, true);
                    if (bench.name != null && bench.name.ToLower().Contains("hay"))
                        return (CATEGORY.Others, "Feeding Bench");
                    else
                        return (CATEGORY.Others, "Empty Feeding Bench");
                }
            }
            else if (Game1.currentLocation.Name.ToLower().Equals("deluxe coop") || Game1.currentLocation.Name.ToLower().Equals("coop3"))
            {
                if (x >= 6 && x <= 17 && y == 3)
                {
                    (string? name, CATEGORY category) bench = getObjectAtTile(x, y, true);
                    if (bench.name != null && bench.name.ToLower().Contains("hay"))
                        return (CATEGORY.Others, "Feeding Bench");
                    else
                        return (CATEGORY.Others, "Empty Feeding Bench");
                }
            }
            else if (Game1.currentLocation.Name.ToLower().Equals("barn"))
            {
                if (x >= 8 && x <= 11 && y == 3)
                {
                    (string? name, CATEGORY category) bench = getObjectAtTile(x, y, true);
                    if (bench.name != null && bench.name.ToLower().Contains("hay"))
                        return (CATEGORY.Others, "Feeding Bench");
                    else
                        return (CATEGORY.Others, "Empty Feeding Bench");
                }
            }
            else if (Game1.currentLocation.Name.ToLower().Equals("big barn") || Game1.currentLocation.Name.ToLower().Equals("barn2"))
            {
                if (x >= 8 && x <= 15 && y == 3)
                {
                    (string? name, CATEGORY category) bench = getObjectAtTile(x, y, true);
                    if (bench.name != null && bench.name.ToLower().Contains("hay"))
                        return (CATEGORY.Others, "Feeding Bench");
                    else
                        return (CATEGORY.Others, "Empty Feeding Bench");
                }
            }
            else if (Game1.currentLocation.Name.ToLower().Equals("deluxe barn") || Game1.currentLocation.Name.ToLower().Equals("barn3"))
            {
                if (x >= 8 && x <= 19 && y == 3)
                {
                    (string? name, CATEGORY category) bench = getObjectAtTile(x, y, true);
                    if (bench.name != null && bench.name.ToLower().Contains("hay"))
                        return (CATEGORY.Others, "Feeding Bench");
                    else
                        return (CATEGORY.Others, "Empty Feeding Bench");
                }
            }
            else if (Game1.currentLocation is LibraryMuseum libraryMuseum)
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
        public static (string? name, CATEGORY category) getObjectAtTile(int x, int y, bool lessInfo = false)
        {
            (string? name, CATEGORY category) toReturn = (null, CATEGORY.Others);

            StardewValley.Object obj = Game1.currentLocation.getObjectAtTile(x, y);
            if (obj == null) return toReturn;

            int index = obj.ParentSheetIndex;
            toReturn.name = obj.DisplayName;

            // Get object names based on index
            (string? name, CATEGORY category) correctNameAndCategory = getCorrectNameAndCategoryFromIndex(index, obj.Name);

            if (obj is Chest)
            {
                Chest chest = (Chest)obj;
                toReturn = (chest.DisplayName, CATEGORY.Containers);
            }
            else if (obj is IndoorPot indoorPot)
            {
                toReturn.name = $"{obj.DisplayName}, {getHoeDirtDetail(indoorPot.hoeDirt.Value, true)}";
            }
            else if (obj is Sign sign)
            {
                if (sign.displayItem.Value != null)
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
                    toReturn.category = CATEGORY.Furnitures;

            }
            else if (obj.IsSprinkler() && obj.heldObject.Value != null) // Detect the upgrade attached to the sprinkler
            {
                if (MainClass.ModHelper != null && obj.heldObject.Value.Name.ToLower().Contains("pressure nozzle"))
                {
                    toReturn.name = MainClass.ModHelper.Translation.Get("readtile.sprinkler.pressurenozzle", new { value = toReturn.name });
                }
                else if (MainClass.ModHelper != null && obj.heldObject.Value.Name.ToLower().Contains("enricher"))
                {
                    toReturn.name = MainClass.ModHelper.Translation.Get("readtile.sprinkler.enricher", new { value = toReturn.name });
                }
                else // fall through 
                {
                    toReturn.name = $"{obj.heldObject.Value.DisplayName} {toReturn.name}";
                }
            }
            else if ((obj.Type == "Crafting" && obj.bigCraftable.Value) || obj.Name.ToLower().Equals("crab pot"))
            {
                foreach (string machine in trackable_machines)
                {
                    if (obj.Name.ToLower().Contains(machine))
                    {
                        toReturn.name = obj.DisplayName;
                        toReturn.category = CATEGORY.Machines;
                    }
                }
            }
            else if (correctNameAndCategory.name != null)
                toReturn = correctNameAndCategory;
            else if (obj.name.ToLower().Equals("stone"))
                toReturn.category = CATEGORY.Debris;
            else if (obj.name.ToLower().Equals("twig"))
                toReturn.category = CATEGORY.Debris;
            else if (obj.name.ToLower().Contains("quartz"))
                toReturn.category = CATEGORY.MineItems;
            else if (obj.name.ToLower().Contains("earth crystal"))
                toReturn.category = CATEGORY.MineItems;
            else if (obj.name.ToLower().Contains("frozen tear"))
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

        private static MachineState GetMachineState(StardewValley.Object machine)
        {
            if (machine is CrabPot crabPot)
            {
                if (crabPot.bait.Value is not null && crabPot.heldObject.Value is null)
                    return MachineState.Busy;
                if (crabPot.bait.Value is not null && crabPot.heldObject.Value is not null)
                    return MachineState.Ready;
            }
            return GetMachineState(machine.readyForHarvest.Value, machine.MinutesUntilReady, machine.heldObject.Value);
        }

        private static MachineState GetMachineState(bool readyForHarvest, int minutesUntilReady, StardewValley.Object? heldObject)
        {
            if (readyForHarvest || (heldObject is not null && minutesUntilReady <= 0))
                return MachineState.Ready;
            else if (minutesUntilReady > 0)
                return MachineState.Busy;
            else
                return MachineState.Waiting;
        }

        private static (string? name, CATEGORY category) getCorrectNameAndCategoryFromIndex(int index, string objName)
        {
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

            if (objName.ToLower().Contains("stone"))
            {
                switch (index)
                {
                    case 76:
                        return ("Frozen geode", CATEGORY.MineItems);
                    case 77:
                        return ("Magma geode", CATEGORY.MineItems);
                    case 75:
                        return ("Geode", CATEGORY.MineItems);
                    case 819:
                        return ("Omni geode node", CATEGORY.MineItems);
                    case 32:
                    case 34:
                    case 36:
                    case 38:
                    case 40:
                    case 42:
                    case 48:
                    case 50:
                    case 52:
                    case 54:
                    case 56:
                    case 58:
                        return ("Coloured stone", CATEGORY.Debris);
                    case 668:
                    case 670:
                    case 845:
                    case 846:
                    case 847:
                        return ("Mine stone", CATEGORY.MineItems);
                    case 818:
                        return ("Clay stone", CATEGORY.Debris);
                    case 816:
                    case 817:
                        return ("Fossil stone", CATEGORY.Debris);
                    case 25:
                        return ("Mussel Node", CATEGORY.MineItems);
                    case 95:
                        return ("Radioactive Node", CATEGORY.MineItems);
                    case 843:
                    case 844:
                        return ("Cinder shard node", CATEGORY.MineItems);
                    case 8:
                    case 66:
                        return ("Amethyst node", CATEGORY.MineItems);
                    case 14:
                    case 62:
                        return ("Aquamarine node", CATEGORY.MineItems);
                    case 2:
                    case 72:
                        return ("Diamond node", CATEGORY.MineItems);
                    case 12:
                    case 60:
                        return ("Emerald node", CATEGORY.MineItems);
                    case 44:
                        return ("Gem node", CATEGORY.MineItems);
                    case 6:
                    case 70:
                        return ("Jade node", CATEGORY.MineItems);
                    case 46:
                        return ("Mystic stone", CATEGORY.MineItems);
                    case 74:
                        return ("Prismatic node", CATEGORY.MineItems);
                    case 4:
                    case 64:
                        return ("Ruby node", CATEGORY.MineItems);
                    case 10:
                    case 68:
                        return ("Topaz node", CATEGORY.MineItems);
                    case 751:
                    case 849:
                        return ("Copper node", CATEGORY.MineItems);
                    case 764:
                        return ("Gold node", CATEGORY.MineItems);
                    case 765:
                        return ("Iridium node", CATEGORY.MineItems);
                    case 290:
                    case 850:
                        return ("Iron node", CATEGORY.MineItems);
                }
            }

            return (null, CATEGORY.Others);
        }
        #endregion  

        public static String GetGemBirdName(IslandGemBird bird)
        {
            return bird.itemIndex.Value switch
            {
                60 => "Emerald Gem Bird",
                62 => "Aquamarine Gem Bird",
                64 => "Ruby Gem Bird",
                66 => "Amethyst Gem Bird",
                68 => "Topaz Gem Bird",
                _ => "Gem Bird",
            };
        }

        public static bool isMineDownLadderAtTile(int x, int y)
        {
            try
            {
                if (Game1.currentLocation is Mine or MineShaft)
                {
                    if (Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y] == null)
                        return false;

                    int index = Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y].TileIndex;

                    if (index == 173)
                    {
                        return true;
                    }
                }
            }
            catch (Exception) { }

            return false;
        }

        public static bool isShaftAtTile(int x, int y)
        {
            try
            {
                if (Game1.currentLocation is Mine or MineShaft)
                {
                    if (Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y] == null)
                        return false;

                    if (Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y].TileIndex == 174)
                        return true;
                }
            }
            catch (Exception) { }

            return false;
        }

        public static bool isMineUpLadderAtTile(int x, int y)
        {
            try
            {
                if (Game1.currentLocation is Mine or MineShaft)
                {
                    if (Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y] == null)
                        return false;

                    if (Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y].TileIndex == 115)
                        return true;
                }
            }
            catch (Exception) { }

            return false;
        }

        public static bool isElevatorAtTile(int x, int y)
        {
            try
            {
                if (Game1.currentLocation is Mine or MineShaft)
                {
                    if (Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y] == null)
                        return false;

                    if (Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y].TileIndex == 112)
                        return true;
                }
            }
            catch (Exception) { }

            return false;
        }

        public static string? getWarpPointAtTile(int x, int y)
        {
            try
            {
                if (Game1.currentLocation == null) return null;

                int warpCount = Game1.currentLocation.warps.Count();
                for (int i = 0; i < warpCount; i++)
                {
                    Warp warpPoint = Game1.currentLocation.warps[i];
                    if (warpPoint.X != x || warpPoint.Y != y) continue;

                    return $"{warpPoint.TargetName} Entrance";
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Error while detecting warp points.\n{e.Message}");
            }

            return null;
        }

        public static string? getDoorAtTile(int x, int y)
        {
            Point tilePoint = new Point(x, y);
            StardewValley.Network.NetPointDictionary<string, Netcode.NetString> doorList = Game1.currentLocation.doors;

            for (int i = 0; i < doorList.Count(); i++)
            {
                if (doorList.ContainsKey(tilePoint))
                {
                    string? doorName;
                    doorList.TryGetValue(tilePoint, out doorName);

                    if (doorName != null)
                        return $"{doorName} door";
                    else
                        return "door";
                }
            }

            return null;
        }

        public static string? getResourceClumpAtTile(int x, int y, bool lessInfo = false)
        {
            if (Game1.currentLocation is Woods)
                return getStumpsInWoods(x, y, lessInfo);

            for (int i = 0; i < Game1.currentLocation.resourceClumps.Count; i++)
            {
                if (!Game1.currentLocation.resourceClumps[i].occupiesTile(x, y))
                    continue;

                if (lessInfo && (Game1.currentLocation.resourceClumps[i].tile.X != x || Game1.currentLocation.resourceClumps[i].tile.Y != y))
                    continue;

                int index = Game1.currentLocation.resourceClumps[i].parentSheetIndex.Value;

                switch (index)
                {
                    case 600:
                        return "Large Stump";
                    case 602:
                        return "Hollow Log";
                    case 622:
                        return "Meteorite";
                    case 752:
                    case 754:
                    case 756:
                    case 758:
                        return "Mine Rock";
                    case 672:
                        return "Boulder";
                    case 190:
                        return "Giant Cauliflower";
                    case 254:
                        return "Giant Melon";
                    case 276:
                        return "Giant Pumpkin";
                    default:
                        return "Unknown";
                }
            }

            return null;
        }

        public static string? getStumpsInWoods(int x, int y, bool lessInfo = false)
        {
            if (Game1.currentLocation is not Woods)
                return null;

            Netcode.NetObjectList<ResourceClump> stumps = ((Woods)Game1.currentLocation).stumps;
            for (int i = 0; i < stumps.Count; i++)
            {
                if (!stumps[i].occupiesTile(x, y))
                    continue;

                if (lessInfo && (stumps[i].tile.X != x || stumps[i].tile.Y != y))
                    continue;

                return "Large Stump";
            }
            return null;
        }

        public static string? getParrotPerchAtTile(int x, int y)
        {
            if (Game1.currentLocation is not IslandLocation islandLocation)
                return null;

            int perchCount = islandLocation.parrotUpgradePerches.Count();
            for (int i = 0; i < perchCount; i++)
            {
                var perch = islandLocation.parrotUpgradePerches[i];
                if (!perch.tilePosition.Value.Equals(new Point(x, y)))
                    continue;

                string toSpeak = $"Parrot required nuts {perch.requiredNuts.Value}";

                if (!perch.IsAvailable())
                    return "Empty parrot perch";
                else if (perch.currentState.Value == StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Idle)
                    return toSpeak;
                else if (perch.currentState.Value == StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.StartBuilding)
                    return "Parrots started building request";
                else if (perch.currentState.Value == StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Building)
                    return "Parrots building request";
                else if (perch.currentState.Value == StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Complete)
                    return $"Request Completed";
                else
                    return toSpeak;
            }

            return null;
        }

    }
}
