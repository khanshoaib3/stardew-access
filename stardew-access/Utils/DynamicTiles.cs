using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.SpecialOrders;
using StardewValley.TokenizableStrings;

namespace stardew_access.Utils;

using Translation;
using StardewValley.TerrainFeatures;
using StardewValley.Menus;

/// <summary>
/// Provides methods to locate tiles of interest in various game locations that are conditional or unpredictable (I.E. not static).
/// </summary>
/// <remarks>
/// The DynamicTiles class currently supports the following location types:
/// - Beach
/// - BoatTunnel
/// - CommunityCenter
/// - Farm
/// - FarmHouse
/// - Forest
/// - IslandFarmHouse
/// - IslandLocation
/// - LibraryMuseum
/// - Town
/// - MineShaft
///
/// And the following Island LocationTypes:
/// - IslandNorth
/// - IslandWest
/// - VolcanoDungeon
///
/// The class also supports the following named locations:
/// - Barn (and its upgraded versions)
/// - Coop (and its upgraded versions)
/// - Mastery Cave
/// - Witch Hut
///
/// The class does not yet support the following location types, but consider adding support in future updates:
/// - AbandonedJojaMart
/// - AdventureGuild
/// - BathHousePool
/// - BeachNightMarket
/// - BugLand
/// - BusStop
/// - Caldera
/// - Cellar
/// - Club
/// - Desert
/// - FarmCave
/// - FishShop
/// - JojaMart
/// - ManorHouse
/// - MermaidHouse
/// - Mine
/// - Mountain
/// - MovieTheater
/// - Railroad
/// - SeedShop
/// - Sewer
/// - Submarine
/// - Summit
/// - WizardHouse
/// - Woods
///
/// The class does not yet support the following named locations, but consider adding support in future updates:
/// - "AnimalShop"
/// - "Backwoods"
/// - "BathHouse_Entry"
/// - "BathHouse_MensLocker"
/// - "BathHouse_WomensLocker"
/// - "Blacksmith"
/// - "ElliottHouse"
/// - "FarmGreenHouse"
/// - "Greenhouse"
/// - "HaleyHouse"
/// - "HarveyRoom"
/// - "Hospital"
/// - "JoshHouse"
/// - "LeahHouse"
/// - "LeoTreeHouse"
/// - "Saloon"
/// - "SamHouse"
/// - "SandyHouse"
/// - "ScienceHouse"
/// - "SebastianRoom"
/// - "SkullCave"
/// - "Sunroom"
/// - "Tent"
/// - "Trailer"
/// - "Trailer_Big"
/// - "Tunnel"
/// - "WitchHut"
/// - "WitchSwamp"
/// - "WitchWarpCave"
/// - "WizardHouseBasement"
///
/// The class does not yet support the following IslandLocation location types, but consider adding support in future updates:
/// - IslandEast
/// - IslandFarmCave
/// - IslandFieldOffice
/// - IslandHut
/// - IslandShrine
/// - IslandSouth
/// - IslandSouthEast
/// - IslandSouthEastCave
/// - IslandWestCave1
///
/// The class does not yet support the following IslandLocation named locations, but consider adding support in future updates:
/// - "CaptainRoom"
/// - "IslandNorthCave1"
/// - "QiNutRoom"
/// </remarks>
public class DynamicTiles
{
    // Static instance for the singleton pattern
    private static DynamicTiles? _instance;

    /// <summary>
    /// The singleton instance of the <see cref="DynamicTiles"/> class.
    /// </summary>
    public static DynamicTiles Instance
    {
        get
        {
            _instance ??= new DynamicTiles();
            return _instance;
        }
    }

    // HashSet for storing which unimplemented locations have been previously logged
    private static readonly HashSet<object> loggedLocations = [];

    // Dictionary of coordinates for feeding benches in barns and coops
    private static readonly Dictionary<string, (int minX, int maxX, int y)> FeedingBenchBounds = new()
    {
        { "Barn", (8, 11, 3) },
        { "Barn2", (8, 15, 3) },
        { "Big Barn", (8, 15, 3) },
        { "Barn3", (8, 19, 3) },
        { "Deluxe Barn", (8, 19, 3) },
        { "Coop", (6, 9, 3) },
        { "Coop2", (6, 13, 3) },
        { "Big Coop", (6, 13, 3) },
        { "Coop3", (6, 17, 3) },
        { "Deluxe Coop", (6, 17, 3) }
    };

    // Dictionary to hold event info
    private static readonly Dictionary<string, Dictionary<(int X, int Y), string>> EventInteractables;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicTiles"/> class.
    /// Loads the event file.
    /// </summary>
    static DynamicTiles()
    {
        EventInteractables = LoadEventTiles();
    }

    /// <summary>
    /// Loads event tiles from the "event-tiles.json" file and returns a dictionary representation of the data.
    /// </summary>
    /// <returns>
    /// A dictionary with event names as keys and nested dictionaries as values, where nested dictionaries have
    /// coordinate tuples (x, y) as keys and tile names as values.
    /// </returns>
    private static Dictionary<string, Dictionary<(int x, int y), string>> LoadEventTiles()
    {
        const string EventTilesFileName = "event-tiles.json";
        bool loaded = JsonLoader.TryLoadJsonFile(EventTilesFileName, out JToken? json, subdir: "assets/TileData");

        if (!loaded || json == null || json.Type != JTokenType.Object)
        {
            // If the JSON couldn't be loaded, parsed, or is not a JSON object, return an empty dictionary
            return [];
        }

        var eventTiles = new Dictionary<string, Dictionary<(int x, int y), string>>();

        // Iterate over the JSON properties to create a dictionary representation of the data
        foreach (var eventProperty in ((JObject)json).Properties())
        {
            string eventName = eventProperty.Name;
            var coordinates = new Dictionary<(int x, int y), string>();

            // Iterate over the coordinate properties to create a nested dictionary with coordinate tuples as keys
            if (eventProperty.Value is JObject coordinatesObject)
            {
                foreach (var coordinateProperty in coordinatesObject.Properties())
                {
                    string[] xy = coordinateProperty.Name.Split(',');
                    if (xy.Length == 2 && int.TryParse(xy[0], out int x) && int.TryParse(xy[1], out int y))
                    {
                        coordinates.Add((x, y), value: coordinateProperty.Value.ToString() ?? string.Empty);
                    }
                    else
                    {
                        Log.Warn($"Invalid coordinate format '{coordinateProperty.Name}' in {EventTilesFileName}.");
                    }
                }
            }

            eventTiles.Add(eventName, coordinates);
        }

        return eventTiles;
    }

    /// <summary>
    /// Retrieves information about interactables, NPCs, or other features at a given coordinate in a Beach.
    /// </summary>
    /// <param name="beach">The Beach to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetBeachInfo(Beach beach, int x, int y, bool lessInfo = false)
    {
        if (MainClass.ModHelper == null)
        {
            return (null, null);
        }
        if (MainClass.ModHelper.Reflection.GetField<NPC>(beach, "oldMariner").GetValue() is NPC mariner && mariner.Tile == new Vector2(x, y))
        {
            return ("npc_name-old_mariner", CATEGORY.NPCs);
        }
        else if (x == 58 && y == 13)
        {
            if (!beach.bridgeFixed.Value)
            {
                return (Translator.Instance.Translate("prefix-repair", new { content = Translator.Instance.Translate("tile_name-bridge") }), CATEGORY.Pending);
            }
            else
            {
                return ("tile_name-bridge", CATEGORY.Bridges);
            }
        }

        if (Game1.CurrentEvent is not null && Game1.CurrentEvent.id == "13" && x == 53 && y == 8)
        {
            return ("item-haley_bracelet-name", CATEGORY.DroppedItems);
        }
        return (null, null);
    }

    /// <summary>
    /// Retrieves information about interactables or other features at a given coordinate in a BoatTunnel.
    /// </summary>
    /// <param name="boatTunnel">The BoatTunnel to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetBoatTunnelInfo(BoatTunnel boatTunnel, int x, int y, bool lessInfo = false)
    {
        // Check if the player has received the specified mail or not
        static bool HasMail(string mail) => Game1.MasterPlayer.hasOrWillReceiveMail(mail);

        // If the position matches one of the interactable elements in the boat tunnel
        if ((x, y) == (4, 9) || (x, y) == (6, 8) || (x, y) == (8, 9))
        {
            string mail = (x, y) switch
            {
                (4, 9) => "willyBoatTicketMachine",
                (6, 8) => "willyBoatHull",
                (8, 9) => "willyBoatAnchor",
                _ => throw new InvalidOperationException("Unexpected (x, y) values"),
            };

            string itemName = Translator.Instance.Translate(
                (x, y) switch
                {
                    (4, 9) => "tile_name-ticket_machine",
                    (6, 8) => "tile_name-boat_hull",
                    (8, 9) => "tile_name-boat_anchor",
                    _ => throw new InvalidOperationException("Unexpected (x, y) values"),
                }
            );

            CATEGORY category = (x, y) == (4, 9)
                ? !HasMail(mail)
                        ? CATEGORY.Pending
                        : !HasMail("willyBoatHull") || !HasMail("willyBoatAnchor") ? CATEGORY.Decor : CATEGORY.Interactables
                : !HasMail(mail) ? CATEGORY.Pending : CATEGORY.Decor;

            return ((!HasMail(mail) ? Translator.Instance.Translate("prefix-repair", new { content = itemName }) : itemName), category);
        }

        return (null, null);
    }

    /// <summary>
    /// Retrieves information about interactables or other features at a given coordinate in a CommunityCenter.
    /// </summary>
    /// <param name="communityCenter">The CommunityCenter to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetCommunityCenterInfo(CommunityCenter communityCenter, int x, int y, bool lessInfo = false)
    {
        if (communityCenter.missedRewardsChestVisible.Value && x == 22 && y == 10)
        {
            return ("tile_name-missed_reward_chest", CATEGORY.Containers);
        }

        return (null, null);
    }

    /// <summary>
    /// Gets the building information for a given position on a farm.
    /// </summary>
    /// <param name="building">The Building instance.</param>
    /// <param name="x">The x-coordinate of the position.</param>
    /// <param name="y">The y-coordinate of the position.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the door or building found, or (null, null) if no door or building is found.</returns>
    private static (string? name, CATEGORY? category) GetBuildingInfo(Building building, int x, int y, bool lessInfo = false)
    {
        // Internal name; never translated. Mill stays "Mill"
        string type = building.buildingType.Value;
        // Translated name, E.G> "Mill" becomes "Molino" for Spanish.
        // not all buildings have translated names, E.G. "Petbowl" and "Farmhouse" remain untranslated.
        // TODO add translation keys for untranslated building names.
        string name = TokenParser.ParseText(building.GetData().Name);
        int buildingTileX = building.tileX.Value;
        int buildingTileY = building.tileY.Value;
        // Calculate differences in x and y coordinates
        int offsetX = x - buildingTileX;
        int offsetY = y - buildingTileY;

        // Set default category
        CATEGORY category = CATEGORY.Buildings;
        if (type == "Shipping Bin")
        {
            category = CATEGORY.Containers;
        }
        else if (building is PetBowl bowl)
        {
            category = bowl.HasPet()
                ? !bowl.watered.Value
                    ? CATEGORY.Pending
                    : CATEGORY.Interactables
                : CATEGORY.Decor;

            name = Translator.Instance.Translate("tile-pet_bowl-prefix", new
            {
                is_in_use = bowl.HasPet() ? 1 : 0,
                is_empty = !bowl.watered.Value ? 1 : 0,
                name
            });
        }
        else if (building.GetIndoors() is Cabin cabin)
        {
            string ownerName = cabin.owner.displayName;
            if (offsetX is 2 && offsetY is 1)
            {
                name = string.IsNullOrWhiteSpace(ownerName)
                    ? $"{name} {Translator.Instance.Translate("tile-door")}" // Cabin Door
                    : $"{cabin.owner.Name} {name} {Translator.Instance.Translate("tile-door")}"; // [Owner] Cabin Door
                category = CATEGORY.Doors;
                goto PassableTilesCheck;
            }

            if (offsetX is not 4 || offsetY is not 2)
            {
                // Prepend cabin's owner name to the cabin
                if (cabin.HasOwner && !cabin.IsOwnedByCurrentPlayer && !string.IsNullOrWhiteSpace(ownerName))
                    name = $"{cabin.owner.Name} {name}";
                goto PassableTilesCheck;
            }

            // Cabin Mail Box

            category = CATEGORY.Interactables;

            if (!cabin.IsOwnedByCurrentPlayer)
            {
                // Prepend the owner's name to mail box
                name = string.IsNullOrWhiteSpace(ownerName)
                    ? $"{name} {Translator.Instance.Translate("tile_name-mail_box")}" // Cabin Mail Box
                    : $"{cabin.owner.Name} {Translator.Instance.Translate("tile_name-mail_box")}"; // [Owner] Mail Box
                goto PassableTilesCheck;
            }

            // Mail Box (with unread status)
            name = Translator.Instance.Translate("tile_name-mail_box");
            var mailbox = Game1.player.mailbox;
            if (mailbox is not null && mailbox.Count > 0)
            {
                name = Translator.Instance.Translate("tile_name-mail_box-unread_mail_count-prefix", new
                {
                    mail_count = mailbox.Count,
                    content = name
                });
                category = CATEGORY.Ready;
            }
        }
        else if (building.GetIndoors() is FarmHouse farmHouse && farmHouse.HasOwner && !farmHouse.IsOwnedByCurrentPlayer)
        {
            name = $"{farmHouse.owner.displayName} {name}";
        }
        // If the building is a FishPond, prepend the fish name
        else if (building is FishPond fishPond && fishPond.fishType.Value != "0" && fishPond.fishType.Value != "")
        {
            name = $"{ItemRegistry.GetDataOrErrorItem(fishPond.fishType.Value).DisplayName} {name}";
            category = CATEGORY.Fishponds;
        }
        // Check if the position matches the human door
        if (building.humanDoor.Value.X == offsetX && building.humanDoor.Value.Y == offsetY)
        {
            name = Translator.Instance.Translate("suffix-building_door", new { content = name });
            category = CATEGORY.Doors;
        }
        // Check if the position matches the animal door. In case of barns, as the animal door is 2 tiles wide, the following if condition checks for both animal door tiles.
        else if ((building.animalDoor.Value.X == offsetX || (type == "Barn" && building.animalDoor.Value.X == offsetX - 1)) && building.animalDoor.Value.Y == offsetY)
        {
            name = Translator.Instance.Translate("tile-building_animal_door-suffix", new
            {
                name, // using inferred member name; silences IDE0037
                is_open = (building.animalDoorOpen.Value) ? 1 : 0,
                less_info = lessInfo ? 1 : 0
            });
            category = CATEGORY.Doors;
        }
        // Special handling for Mill buildings
        else if (type == "Mill")
        {
            if (offsetY == 1)
            {
                // Check if the position matches the input
                if (offsetX == 1)
                {
                    name = Translator.Instance.Translate("suffix-mill_input", new { content = name });
                    category = CATEGORY.Interactables;
                }
                // Check if the position matches the output
                else if (offsetX == 3)
                {
                    name = Translator.Instance.Translate("suffix-mill_output", new { content = name });
                    category = CATEGORY.Interactables;
                }
            }
        }
    // Any building tile not matched will return building's name and Buildings category 

    PassableTilesCheck:
        if (!building.isTilePassable(new Vector2(x, y)))
        {
            return (name, category);
        }
        else
        {
            // Ignore parts of buildings that are outside, I.E. Farmhouse porch.
            return (null, null);
        }
    }

    /// <summary>
    /// Retrieves information about interactables or other features at a given coordinate in a Farm.
    /// </summary>
    /// <param name="farm">The Farm to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetFarmInfo(Farm farm, int x, int y, bool lessInfo = false)
    {
        var mainMailboxPos = farm.GetMainMailboxPosition();
        Building building = farm.getBuildingAt(new Vector2(x, y));

        if (mainMailboxPos.X == x && mainMailboxPos.Y == y)
        {
            string mailboxName = Translator.Instance.Translate("tile_name-mail_box");
            CATEGORY mailboxCategory = CATEGORY.Interactables;

            if (!(farm.GetMainFarmHouse().GetIndoors() as FarmHouse)!.IsOwnedByCurrentPlayer)
            {
                mailboxName = $"{(farm.GetMainFarmHouse().GetIndoors() as FarmHouse)!.owner.displayName} {mailboxName}";
                return (mailboxName, mailboxCategory);
            }

            var mailbox = Game1.player.mailbox;
            if (mailbox is not null && mailbox.Count > 0)
            {
                mailboxName = Translator.Instance.Translate("tile_name-mail_box-unread_mail_count-prefix", new
                {
                    mail_count = mailbox.Count,
                    content = mailboxName
                });
                mailboxCategory = CATEGORY.Ready;
            }

            return (mailboxName, mailboxCategory);
        }
        else if (building is not null) // Check if there is a building at the current position
        {
            return GetBuildingInfo(building, x, y, lessInfo);
        }

        // Speaks the Grandpa Evaluation score i.e., numbers of candles lit on the shrine after year 3
        if (farm.GetGrandpaShrinePosition().X == x && farm.GetGrandpaShrinePosition().Y == y)
        {
            return (Translator.Instance.Translate("dynamic_tile-farm-grandpa_shrine", new
            {
                candles = farm.grandpaScore.Value
            }), CATEGORY.Interactables);
        }

        return (null, null);
    }

    /// <summary>
    /// Retrieves information about interactables or other features at a given coordinate in a FarmHouse.
    /// </summary>
    /// <param name="farmHouse">The FarmHouse to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetFarmHouseInfo(FarmHouse farmHouse, int x, int y, bool lessInfo = false)
    {
        if (farmHouse.upgradeLevel >= 1)
        {
            int kitchenX = farmHouse.getKitchenStandingSpot().X;
            int kitchenY = farmHouse.getKitchenStandingSpot().Y - 1;

            if (kitchenX == x && kitchenY == y)
            {
                return ("tile_name-stove", CATEGORY.Interactables);
            }
            else if (kitchenX + 1 == x && kitchenY == y)
            {
                return ("tile_name-sink", CATEGORY.Water);
            }
            else if (farmHouse.fridgePosition.X == x && farmHouse.fridgePosition.Y == y)
            {
                return ("tile_name-fridge", CATEGORY.Containers);
            }
        }

        return (null, null);
    }

    /// <summary>
    /// Retrieves information about interactables or other features at a given coordinate in a Forest.
    /// </summary>
    /// <param name="forest">The Forest to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetForestInfo(Forest forest, int x, int y, bool lessInfo = false)
    {
        if (forest.travelingMerchantDay && x == 27 && y == 11)
        {
            return ("tile_name-traveling_cart", CATEGORY.Interactables);
        }
        else if (forest.travelingMerchantDay && x == 23 && y == 11)
        {
            return ("tile_name-traveling_cart_pig", CATEGORY.NPCs);
        }
        else if (Game1.MasterPlayer.mailReceived.Contains("raccoonTreeFallen") && x == 56 && y == 6)
        {
            return ("tile-forest-giant_tree_sump", forest.stumpFixed.Value ? CATEGORY.Decor : CATEGORY.Quest);
        }

        return (null, null);
    }

    /// <summary>
    /// Retrieves information about interactables, NPCs, or other features at a given coordinate in an IslandFarmHouse.
    /// </summary>
    /// <param name="islandFarmHouse">The IslandFarmHouse to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetIslandFarmHouseInfo(IslandFarmHouse islandFarmHouse, int x, int y, bool lessInfo = false)
    {
        int fridgeX = islandFarmHouse.fridgePosition.X;
        int fridgeY = islandFarmHouse.fridgePosition.Y;
        if (fridgeX - 2 == x && fridgeY == y)
        {
            return ("tile_name-stove", CATEGORY.Interactables);
        }
        else if (fridgeX - 1 == x && fridgeY == y)
        {
            return ("tile_name-sink", CATEGORY.Water);
        }
        else if (fridgeX == x && fridgeY == y)
        {
            return ("tile_name-fridge", CATEGORY.Containers);
        }

        return (null, null);
    }

    /// <summary>
    /// Retrieves information about interactables, NPCs, or other features at a given coordinate in an IslandNorth.
    /// </summary>
    /// <param name="islandNorth">The IslandNorth to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetIslandNorthInfo(IslandNorth islandNorth, int x, int y, bool lessInfo = false)
    {
        // Check if the trader is activated and the coordinates match the trader's location
        if (islandNorth.traderActivated.Value && x == 36 && y == 71)
        {
            return ("npc_name-island_trader", CATEGORY.Interactables);
        }
        else if (!islandNorth.caveOpened.Value && y == 47 && (x == 21 || x == 22))
        {
            return ("tile-resource_clump-boulder-name", CATEGORY.ResourceClumps);
        }

        // Return (null, null) if no relevant object is found
        return (null, null);
    }

    /// <summary>
    /// Retrieves information about interactables, NPCs, or other features at a given coordinate in an IslandWest.
    /// </summary>
    /// <param name="islandWest">The IslandWest to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetIslandWestInfo(IslandWest islandWest, int x, int y, bool lessInfo = false)
    {
        // Check if the coordinates match the shipping bin's location
        if ((islandWest.shippingBinPosition.X == x || (islandWest.shippingBinPosition.X + 1) == x) && islandWest.shippingBinPosition.Y == y)
        {
            return ("building_name-shipping_bin", CATEGORY.Containers);
        }

        // Return (null, null) if no relevant object is found
        return (null, null);
    }

    /// <summary>
    /// Retrieves information about tiles at a given coordinate in a VolcanoDungeon.
    /// </summary>
    /// <param name="dungeon">The VolcanoDungeon to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name of the tile and the CATEGORY, or (null, null) if no relevant tile is found.</returns>
    private static (string? name, CATEGORY? category) GetVolcanoDungeonInfo(VolcanoDungeon dungeon, int x, int y, bool lessInfo = false)
    {
        if (!lessInfo)
        {
            if (dungeon.IsCooledLava(x, y))
            {
                return ("tile-cooled_lava-name", CATEGORY.Water);
            }
            else if (StardewValley.Monsters.LavaLurk.IsLavaTile(dungeon, x, y))
            {
                return ("tile-lava-name", CATEGORY.Water);
            }
        }


        // Back[547]: The gate preventing access to the exit {Buildings[0]: Closed & Buildings[-1]:Opened} 
        // Back[496]: Pressure pad (Unpressed)
        // Back[497]: Pressure pad (pressed)
        // Buildings[546]: Left pillar of gate
        // Buildings[548]: Right pillar of gate

        if (dungeon.getTileIndexAt(new Point(x, y), "Back") is 496 or 497)
        {
            return ("tile-volcano_dungeon-pressure_pad", CATEGORY.Interactables);
        }

        if (dungeon.getTileIndexAt(new Point(x, y), "Back") is 547 && dungeon.getTileIndexAt(new Point(x, y), "Buildings") is 0)
        {
            return ("tile-volcano_dungeon-gate", CATEGORY.Doors);
        }

        return (null, null);
    }

    /// <summary>
    /// Retrieves information about interactables, NPCs, or other features at a given coordinate in a named IslandLocation.
    /// </summary>
    /// <param name="islandLocation">The named IslandLocation to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetNamedIslandLocationInfo(IslandLocation islandLocation, int x, int y, bool lessInfo = false)
    {
        object locationType = islandLocation is not null and IslandLocation ? islandLocation.Name ?? "Undefined Island Location" : islandLocation!.GetType();

        // Implement specific logic for named  IslandLocations here, if necessary

        if (locationType.ToString()!.Contains("qinutroom", StringComparison.OrdinalIgnoreCase))
        {
            if (Game1.player.team.SpecialOrderActive("QiChallenge12") && x == 1 && y == 4)
            {
                return ("dynamic_tile-qi_nut_room-collection_box", CATEGORY.Interactables);
            }
            return (null, null);
        }

        // Unimplemented locations are logged.
        // Check if the location has already been logged
        if (!loggedLocations.Contains(locationType))
        {
            // Log the message
            Log.Debug($"Called GetNamedIslandLocationInfo with unimplemented IslandLocation of type {islandLocation.GetType()} and name {islandLocation.Name}");

            // Add the location to the HashSet to prevent logging it again
            loggedLocations.Add(locationType);
        }

        return (null, null);
    }

    /// <summary>
    /// Retrieves the name of the IslandGemBird based on its item index value.
    /// </summary>
    /// <param name="bird">The IslandGemBird instance.</param>
    /// <returns>A string representing the name of the IslandGemBird.</returns>
    private static String GetGemBirdName(IslandGemBird bird)
    {
        // Use a switch expression to return the appropriate bird name based on the item index value
        return bird.itemIndex.Value switch
        {
            "60" => "npc_name-emerald_gem_bird",
            "62" => "npc_name-aquamarine_gem_bird",
            "64" => "npc_name-ruby_gem_bird",
            "66" => "npc_name-amethyst_gem_bird",
            "68" => "npc_name-topaz_gem_bird",
            _ => "npc_name-gem_bird", // Default case for when the item index does not match any of the specified values
        };
    }

    /// <summary>
    /// Gets the parrot perch information at the specified tile coordinates in the given island location.
    /// </summary>
    /// <param name="x">The x-coordinate of the tile to check.</param>
    /// <param name="y">The y-coordinate of the tile to check.</param>
    /// <param name="islandLocation">The IslandLocation where the parrot perch might be found.</param>
    /// <returns>A string containing the parrot perch information if a parrot perch is found at the specified tile; null if no parrot perch is found.</returns>
    private static string? GetParrotPerchAtTile(IslandLocation islandLocation, int x, int y)
    {
        // Use LINQ to find the first parrot perch at the specified tile (x, y) coordinates
        var foundPerch = islandLocation.parrotUpgradePerches.FirstOrDefault(perch => perch.tilePosition.Value.Equals(new Point(x, y)));

        // If a parrot perch was found at the specified tile coordinates
        if (foundPerch != null)
        {
            if (foundPerch.upgradeName.Value == "GoldenParrot") return Translator.Instance.Translate("building-golden_parrot");
            if (islandLocation is IslandWest islandWest)
            {
                if (islandWest.farmhouseMailbox.Value && foundPerch.tilePosition.X == 81 && foundPerch.tilePosition.Y == 40)
                {
                    return "tile_name-mail_box";
                }
            }
            string toSpeak = Translator.Instance.Translate("building-parrot_perch-required_nuts", new { item_count = foundPerch.requiredNuts.Value });

            // Return appropriate string based on the current state of the parrot perch
            return foundPerch.currentState.Value switch
            {
                StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Idle => foundPerch.IsAvailable() ? toSpeak : "building-parrot_perch-upgrade_state_idle",
                StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.StartBuilding => "building-parrot_perch-upgrade_state_start_building",
                StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Building => "building-parrot_perch-upgrade_state_building",
                StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Complete => "building-parrot_perch-upgrade_state_complete",
                _ => toSpeak,
            };
        }

        // If no parrot perch was found, return null
        return null;
    }

    /// <summary>
    /// Retrieves information about interactables, NPCs, or other features at a given coordinate in an IslandLocation.
    /// </summary>
    /// <param name="islandLocation">The IslandLocation to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetIslandLocationInfo(IslandLocation islandLocation, int x, int y, bool lessInfo = false)
    {
        var nutTracker = Game1.player.team.collectedNutTracker;
        string? parrot = GetParrotPerchAtTile(islandLocation, x, y);
        if (islandLocation.IsBuriedNutLocation(new Point(x, y)) && !nutTracker.Contains($"Buried_{islandLocation.Name}_{x}_{y}"))
        {
            return ("tile_name-diggable_spot", CATEGORY.Interactables);
        }
        else if (islandLocation.locationGemBird.Value is IslandGemBird bird && ((int)bird.position.X / Game1.tileSize) == x && ((int)bird.position.Y / Game1.tileSize) == y)
        {
            return (GetGemBirdName(bird), CATEGORY.NPCs);
        }
        else if (parrot != null)
        {
            if (islandLocation is IslandWest islandWest && islandWest.farmhouseMailbox.Value && parrot == "tile_name-mail_box")
            {
                var mailbox = Game1.player.mailbox;
                string content = Translator.Instance.Translate(parrot);
                if (mailbox != null && mailbox.Count > 0)
                {
                    return (Translator.Instance.Translate("tile_name-mail_box-unread_mail_count-prefix", new { mail_count = mailbox.Count, content }), CATEGORY.Ready);
                }
                return (content, CATEGORY.Interactables);
            }
            else
            {
                return (parrot, CATEGORY.Buildings);
            }
        }

        return islandLocation switch
        {
            IslandNorth islandNorth => GetIslandNorthInfo(islandNorth, x, y, lessInfo),
            IslandWest islandWest => GetIslandWestInfo(islandWest, x, y, lessInfo),
            VolcanoDungeon dungeon => GetVolcanoDungeonInfo(dungeon, x, y, lessInfo),
            _ => GetNamedIslandLocationInfo(islandLocation, x, y, lessInfo)
        };
    }

    /// <summary>
    /// Retrieves the value of the "Action" property from the Buildings layer tile at the given coordinates.
    /// </summary>
    /// <param name="libraryMuseum">The LibraryMuseum containing the tile.</param>
    /// <param name="x">The x-coordinate of the tile.</param>
    /// <param name="y">The y-coordinate of the tile.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>The value of the "Action" property as a string, or null if the property is not found.</returns>
    private static string? GetTileActionPropertyValue(LibraryMuseum libraryMuseum, int x, int y, bool lessInfo = false)
    {
        xTile.Tiles.Tile tile = libraryMuseum.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location(x * 64, y * 64), Game1.viewport.Size);
        return tile.Properties.TryGetValue("Action", out xTile.ObjectModel.PropertyValue? value) ? value.ToString() : null;
    }

    /// <summary>
    /// Retrieves information about interactables, NPCs, or other features at a given coordinate in a LibraryMuseum.
    /// </summary>
    /// <param name="libraryMuseum">The LibraryMuseum to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetLibraryMuseumInfo(LibraryMuseum libraryMuseum, int x, int y, bool lessInfo = false)
    {
        if (libraryMuseum.museumPieces.TryGetValue(new Vector2(x, y), out string museumPiece))
        {
            string displayName = TokenParser.ParseText(Game1.objectData[museumPiece].DisplayName);
            return (Translator.Instance.Translate("tile-museum_piece_showcase-suffix", new { content = displayName }), CATEGORY.Interactables);
        }

        int booksFound = Game1.netWorldState.Value.LostBooksFound;
        string? action = libraryMuseum.doesTileHaveProperty(x, y, "Action", "Buildings");
        if (action != null && action.Contains("Notes"))
        {
            string? actionPropertyValue = GetTileActionPropertyValue(libraryMuseum, x, y, lessInfo);

            if (actionPropertyValue != null)
            {
                int which = Convert.ToInt32(actionPropertyValue.Split(' ')[1]);
                if (booksFound >= which)
                {
                    string message = Game1.content.LoadString("Strings\\Notes:" + which);
                    return (Translator.Instance.Translate("item-suffix-book", new { content = message.Split('\n')[0] }), CATEGORY.Interactables);
                }
                return ("item-lost_book-name", CATEGORY.Other);
            }
        }

        return (null, null);
    }

    /// <summary>
    /// Retrieves information about interactables or other features at a given coordinate in a Town.
    /// </summary>
    /// <param name="town">The Town to search.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? name, CATEGORY? category) GetTownInfo(Town town, int x, int y, bool lessInfo = false)
    {
        if (SpecialOrder.IsSpecialOrdersBoardUnlocked() && x == 62 && y == 93)
        {
            return ("tile_name-special_quest_board", CATEGORY.Interactables);
        }

        if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater") && x == 98 && y == 51)
        {
            return ("tile_name-movie_ticket_machine", CATEGORY.Interactables);
        }

        if (Game1.CurrentEvent is not null && Game1.CurrentEvent.isFestival && x == 0 && y == 54)
        {
            return ("tile-town_festival_exit-name", CATEGORY.Doors);
        }

        return (null, null);
    }

    /// <summary>
    /// Retrieves information about interactables or other features at a given coordinate in Railroad.
    /// </summary>
    /// <param name="town">Railroad instance for reference.</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? translationKeyOrName, CATEGORY? category) GetRailroadInfo(Railroad railroad, int x, int y, bool lessInfo = false)
    {
        if (!railroad.witchStatueGone.Get() && !Game1.MasterPlayer.mailReceived.Contains("witchStatueGone") &&
            x == 54 && y == 35)
        {
            return ("tile-railroad-witch_statue-name", CATEGORY.Interactables);
        }

        return (null, null);
    }

    /// <summary>
    /// Retrieves information about interactables or other features at a given coordinate in the MineShaft.
    /// </summary>
    /// <param name="mineShaft">MineShaft instance for reference</param>
    /// <param name="x">The x-coordinate to search.</param>
    /// <param name="y">The y-coordinate to search.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
    private static (string? translationKeyOrName, CATEGORY? category) GetMineShaftInfo(MineShaft mineShaft, int x, int y, bool lessInfo = false)
    {
        if (mineShaft.getTileIndexAt(new Point(x, y), "Buildings") is 194 or 195 or 224)
        {
            return (mineShaft.getMineArea() is MineShaft.frostArea
                ? "tile-mine_shaft-coal_bag"
                : Translator.Instance.Translate("static_tile-common-minecart", TranslationCategory.StaticTiles), CATEGORY.Interactables);
        }

        if (mineShaft.doesTileHaveProperty(x, y, "Type", "Back") is "Dirt")
        {
            if (mineShaft.doesTileHaveProperty(x, y, "Diggable", "Back") != null)
            {
                bool hasAlreadyDug = mineShaft.terrainFeatures.FieldDict.TryGetValue(new Vector2(x, y), out var tf) && tf.Get() is HoeDirt { crop: null };
                return hasAlreadyDug ? (null, null) : ("tile-mine_shaft-dirt", CATEGORY.Flooring);
            }

            if (mineShaft.getTileIndexAt(new Point(x, y), "Back") is 0)
            {
                return ("tile-mine_shaft-duggy_hole", CATEGORY.Decor);
            }
        }

        return (null, null);
    }

    /// <summary>
    /// Gets the feeding bench information for barns and coops.
    /// </summary>
    /// <param name="currentLocation">The current GameLocation instance.</param>
    /// <param name="x">The x coordinate of the tile.</param>
    /// <param name="y">The y coordinate of the tile.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple of (string? name, CATEGORY? category) for the feeding bench, or null if not applicable.</returns>
    private static (string? name, CATEGORY? category)? GetFeedingBenchInfo(GameLocation currentLocation, int x, int y, bool lessInfo = false)
    {
        string locationName = currentLocation.Name;

        if (FeedingBenchBounds.TryGetValue(locationName, out var bounds) && x >= bounds.minX && x <= bounds.maxX && y == bounds.y)
        {
            (string? name, CATEGORY category) = TileInfo.GetObjectAtTile(currentLocation, x, y, true);
            bool isEmpty = name != null && name.Contains("hay", StringComparison.OrdinalIgnoreCase);
            if (isEmpty)
                category = CATEGORY.Pending;
            return (Translator.Instance.Translate("tile_name-feeding_bench", new { is_empty = (isEmpty ? 0 : 1) }), category);
        }

        return null;
    }

    /// <summary>
    /// Gets information about the current location by its name.
    /// </summary>
    /// <param name="currentLocation">The current GameLocation instance.</param>
    /// <param name="x">The x coordinate of the tile.</param>
    /// <param name="y">The y coordinate of the tile.</param>
    /// <param name="lessInfo">Optional. If true, returns information only if the tile coordinates match the resource clump's origin. Default is false.</param>
    /// <returns>A tuple of (string? name, CATEGORY? category) for the object in the location, or null if not applicable.</returns>
    private static (string? name, CATEGORY? category) GetLocationByNameInfo(GameLocation currentLocation, int x, int y, bool lessInfo = false)
    {
        object locationType = currentLocation is not null and GameLocation ? currentLocation.Name ?? "Undefined GameLocation" : currentLocation!.GetType();
        string locationName = currentLocation.Name ?? "";

        if (locationName.Contains("coop", StringComparison.OrdinalIgnoreCase) || locationName.Contains("barn", StringComparison.OrdinalIgnoreCase))
        {
            var feedingBenchInfo = GetFeedingBenchInfo(currentLocation, x, y);
            if (feedingBenchInfo.HasValue)
            {
                return feedingBenchInfo.Value;
            } // else if something other than feeding benches in barns and coops...
        } //else if something other than barns and coops...

        if (locationName.Contains("witchhut", StringComparison.OrdinalIgnoreCase) && x == 4 && y == 11 && !Game1.player.mailReceived.Contains("hasPickedUpMagicInk"))
        {
            return ("item_name-magic_ink", CATEGORY.Interactables);
        }

        if (locationName.ToLower().Contains("masterycave"))
        {
            if (x == 10 && y == 9 && !MasteryTrackerMenu.hasCompletedAllMasteryPlaques())
            {
                return ("item-mastery_cave-grandpa_letter", CATEGORY.Interactables);
            }
            else if (x == 4 && y == 6)
            {
                return (Translator.Instance.Translate("dynamic_tile-mastery_cave-pedestal", new { has_hat = MasteryTrackerMenu.hasCompletedAllMasteryPlaques() ? 1 : 0 }), CATEGORY.Decor);
            }
        }

        // Unimplemented locations are logged.
        // Check if the location has already been logged
        if (!loggedLocations.Contains(locationType))
        {
            // Log the message
            Log.Debug($"Called GetLocationByNameInfo with unimplemented GameLocation of type {currentLocation.GetType()} and name {currentLocation.Name}");

            // Add the location to the HashSet to prevent logging it again
            loggedLocations.Add(locationType);
        }

        return (null, null);
    }

    /// <summary>
    /// Retrieves the dynamic tile information for the given coordinates in the specified location.
    /// </summary>
    /// <param name="currentLocation">The current GameLocation instance.</param>
    /// <param name="x">The x-coordinate of the tile.</param>
    /// <param name="y">The y-coordinate of the tile.</param>
    /// <param name="lessInfo">An optional boolean to return less detailed information. Defaults to false.</param>
    /// <returns>A tuple containing the name and CATEGORY of the dynamic tile, or null values if not found.</returns>
    public static (string? name, CATEGORY? category) GetDynamicTileAt(GameLocation currentLocation, int x, int y, bool lessInfo = false)
    {
        (string? translationKeyOrName, CATEGORY? category) = GetDynamicTileWithTranslationKeyOrNameAt(currentLocation, x, y, lessInfo);

        if (translationKeyOrName == null)
            return (null, null);

        translationKeyOrName = Translator.Instance.Translate(translationKeyOrName, disableWarning: true);

        return (translationKeyOrName, category);
    }


    public static (string? translationKeyOrName, CATEGORY? category) GetDynamicTileWithTranslationKeyOrNameAt(GameLocation currentLocation, int x, int y, bool lessInfo = false)
    {
        // Check for panning spots
        if (currentLocation.orePanPoint.Value != Point.Zero && currentLocation.orePanPoint.Value == new Point(x, y))
        {
            return ("tile_name-panning_spot", CATEGORY.Interactables);
        }
        // Check if the current location has an event
        else if (currentLocation.currentEvent is not null)
        {
            string eventName = currentLocation.currentEvent.FestivalName;
            // Attempt to retrieve the nested dictionary for the event name from the EventInteractables dictionary
            if (EventInteractables.TryGetValue(eventName, out var coordinateDictionary))
            {
                // Attempt to retrieve the interactable value from the nested dictionary using the coordinates (x, y) as the key
                if (coordinateDictionary.TryGetValue((x, y), value: out var interactable))
                {
                    // If the interactable value is found, return the corresponding category and interactable name
                    return (interactable, CATEGORY.Interactables);
                }
            }
        }

        // Retrieve dynamic tile information based on the current location type
        return currentLocation switch
        {
            Beach beach => GetBeachInfo(beach, x, y, lessInfo),
            BoatTunnel boatTunnel => GetBoatTunnelInfo(boatTunnel, x, y, lessInfo),
            CommunityCenter communityCenter => GetCommunityCenterInfo(communityCenter, x, y, lessInfo),
            Farm farm => GetFarmInfo(farm, x, y, lessInfo),
            FarmHouse farmHouse => GetFarmHouseInfo(farmHouse, x, y, lessInfo),
            Forest forest => GetForestInfo(forest, x, y, lessInfo),
            IslandFarmHouse islandFarmHouse => GetIslandFarmHouseInfo(islandFarmHouse, x, y, lessInfo),
            IslandLocation islandLocation => GetIslandLocationInfo(islandLocation, x, y, lessInfo),
            LibraryMuseum libraryMuseum => GetLibraryMuseumInfo(libraryMuseum, x, y, lessInfo),
            Town town => GetTownInfo(town, x, y, lessInfo),
            Railroad railroad => GetRailroadInfo(railroad, x, y, lessInfo),
            MineShaft mineShaft => GetMineShaftInfo(mineShaft, x, y, lessInfo),
            _ => GetLocationByNameInfo(currentLocation, x, y, lessInfo)
        };
    }
}
