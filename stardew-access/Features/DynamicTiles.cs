using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using static stardew_access.Features.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace stardew_access.Features
{
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
        ///
        /// And the following Island LocationTypes:
        /// - IslandNorth
        /// - IslandWest
        /// - VolcanoDungeon
        ///
        /// The class also supports the following named locations:
        /// - Barn (and its upgraded versions)
        /// - Coop (and its upgraded versions)
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
		private static readonly HashSet<object> loggedLocations = new();

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
			JsonElement json = LoadJsonFile("event-tiles.json");

			if (json.ValueKind == JsonValueKind.Undefined)
			{
				// If the JSON couldn't be loaded or parsed, return an empty dictionary
				return new Dictionary<string, Dictionary<(int x, int y), string>>();
			}

			var eventTiles = new Dictionary<string, Dictionary<(int x, int y), string>>();

			// Iterate over the JSON properties to create a dictionary representation of the data
			foreach (JsonProperty eventProperty in json.EnumerateObject())
			{
				string eventName = eventProperty.Name;
				var coordinates = new Dictionary<(int x, int y), string>();

				// Iterate over the coordinate properties to create a nested dictionary with coordinate tuples as keys
				foreach (JsonProperty coordinateProperty in eventProperty.Value.EnumerateObject())
				{
					string[] xy = coordinateProperty.Name.Split(',');
					int x = int.Parse(xy[0]);
					int y = int.Parse(xy[1]);
					coordinates.Add((x, y), value: coordinateProperty.Value.GetString() ?? string.Empty);
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
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetBeachInfo(Beach beach, int x, int y)
		{
			if (MainClass.ModHelper == null)
			{
				return (null, null);
			}
			if (MainClass.ModHelper.Reflection.GetField<NPC>(beach, "oldMariner").GetValue() is NPC mariner && mariner.getTileLocation() == new Vector2(x, y))
			{
				return ("Old Mariner", CATEGORY.NPCs);
			}
			else if (x == 58 && y == 13)
			{
				if (!beach.bridgeFixed.Value)
				{
					return ("Repair Bridge", CATEGORY.Interactables);
				}
				else
				{
					return ("Bridge", CATEGORY.Bridges);
				}
			}

			return (null, null);
		}

		/// <summary>
		/// Retrieves information about interactables or other features at a given coordinate in a BoatTunnel.
		/// </summary>
		/// <param name="boatTunnel">The BoatTunnel to search.</param>
		/// <param name="x">The x-coordinate to search.</param>
		/// <param name="y">The y-coordinate to search.</param>
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetBoatTunnelInfo(BoatTunnel boatTunnel, int x, int y)
		{
			if (boatTunnel is null)
			{
				throw new ArgumentNullException(nameof(boatTunnel));
			}

			if (x == 4 && y == 9)
			{
				return ((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatFixed") ? "Repair " : "") + "Ticket Machine", CATEGORY.Interactables);
			}
			else if (x == 6 && y == 8)
			{
				return ((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull") ? "Repair " : "") + "Boat Hull", (!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull") ? CATEGORY.Interactables : CATEGORY.Decor));
			}
			else if (x == 8 && y == 9)
			{
				return ((!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor") ? "Repair " : "") + "Boat Anchor", (!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor") ? CATEGORY.Interactables : CATEGORY.Decor));
			}

			return (null, null);
		}

		/// <summary>
		/// Retrieves information about interactables or other features at a given coordinate in a CommunityCenter.
		/// </summary>
		/// <param name="communityCenter">The CommunityCenter to search.</param>
		/// <param name="x">The x-coordinate to search.</param>
		/// <param name="y">The y-coordinate to search.</param>
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetCommunityCenterInfo(CommunityCenter communityCenter, int x, int y)
		{
			if (communityCenter.missedRewardsChestVisible.Value && x == 22 && y == 10)
			{
				return ("Missed Rewards Chest", CATEGORY.Containers);
			}

			return (null, null);
		}

		/// <summary>
		/// Gets the building information for a given position on a farm.
		/// </summary>
		/// <param name="building">The Building instance.</param>
		/// <param name="x">The x-coordinate of the position.</param>
		/// <param name="y">The y-coordinate of the position.</param>
		/// <returns>A tuple containing the name and CATEGORY of the door or building found, or (null, null) if no door or building is found.</returns>
		private static (string? name, CATEGORY? category) GetBuildingInfo(Building building, int x, int y)
		{
			string name = building.buildingType.Value;
			int buildingTileX = building.tileX.Value;
			int buildingTileY = building.tileY.Value;

			// If the building is a FishPond, prepend the fish name
			if (building is FishPond fishPond && fishPond.fishType.Value >= 0)
			{
				name = $"{Game1.objectInformation[fishPond.fishType.Value].Split('/')[4]} {name}";
			}

			// Calculate differences in x and y coordinates
			int offsetX = x - buildingTileX;
			int offsetY = y - buildingTileY;

			// Check if the position matches the human door
			if (building.humanDoor.Value.X == offsetX && building.humanDoor.Value.Y == offsetY)
			{
				return (name + " Door", CATEGORY.Doors);
			}
			// Check if the position matches the animal door
			else if (building.animalDoor.Value.X == offsetX && building.animalDoor.Value.Y == offsetY)
			{
				return (name + " Animal Door " + ((building.animalDoorOpen.Value) ? "Opened" : "Closed"), CATEGORY.Doors);
			}
			// Check if the position matches the building's top-left corner
			else if (offsetX == 0 && offsetY == 0)
			{
				return (name, CATEGORY.Buildings);
			}
			// Special handling for Mill buildings
			else if (building is Mill)
			{
				// Check if the position matches the input
				if (offsetX == 1 && offsetY == 1)
				{
					return (name + " input", CATEGORY.Buildings);
				}
				// Check if the position matches the output
				else if (offsetX == 3 && offsetY == 1)
				{
					return (name + " output", CATEGORY.Buildings);
				}
			}

			// Return the building name for any other position within the building's area
			return (name, CATEGORY.Buildings);
		}

		/// <summary>
		/// Retrieves information about interactables or other features at a given coordinate in a Farm.
		/// </summary>
		/// <param name="farm">The Farm to search.</param>
		/// <param name="x">The x-coordinate to search.</param>
		/// <param name="y">The y-coordinate to search.</param>
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetFarmInfo(Farm farm, int x, int y)
		{
			var mainMailboxPos = farm.GetMainMailboxPosition();
			Building building = farm.getBuildingAt(new Vector2(x, y));

			if (mainMailboxPos.X == x && mainMailboxPos.Y == y)
			{
				return ("Mail box", CATEGORY.Interactables);
			}
			else if (building is not null) // Check if there is a building at the current position
			{
				return GetBuildingInfo(building, x, y);
			}

			return (null, null);
		}

		/// <summary>
		/// Retrieves information about interactables or other features at a given coordinate in a FarmHouse.
		/// </summary>
		/// <param name="farmHouse">The FarmHouse to search.</param>
		/// <param name="x">The x-coordinate to search.</param>
		/// <param name="y">The y-coordinate to search.</param>
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetFarmHouseInfo(FarmHouse farmHouse, int x, int y)
		{
			if (farmHouse.upgradeLevel >= 1)
			{
				int kitchenX = farmHouse.getKitchenStandingSpot().X;
				int kitchenY = farmHouse.getKitchenStandingSpot().Y - 1;

				if (kitchenX == x && kitchenY == y)
				{
					return ("Stove", CATEGORY.Interactables);
				}
				else if (kitchenX + 1 == x && kitchenY == y)
				{
					return ("Sink", CATEGORY.Others);
				}
				else if (farmHouse.fridgePosition.X == x && farmHouse.fridgePosition.Y == y)
				{
					return ("Fridge", CATEGORY.Interactables);
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
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetForestInfo(Forest forest, int x, int y)
		{
			if (forest.travelingMerchantDay && x == 27 && y == 11)
			{
				return ("Travelling Cart", CATEGORY.Interactables);
			}
			else if (forest.log != null && x == 2 && y == 7)
			{
				return ("Log", CATEGORY.Interactables);
			}
			else if (forest.log == null && x == 0 && y == 7)
			{
				return ("Secret Woods Entrance", CATEGORY.Doors);
			}

			return (null, null);
		}

		/// <summary>
		/// Retrieves information about interactables, NPCs, or other features at a given coordinate in an IslandFarmHouse.
		/// </summary>
		/// <param name="islandFarmHouse">The IslandFarmHouse to search.</param>
		/// <param name="x">The x-coordinate to search.</param>
		/// <param name="y">The y-coordinate to search.</param>
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetIslandFarmHouseInfo(IslandFarmHouse islandFarmHouse, int x, int y)
		{
			int fridgeX = islandFarmHouse.fridgePosition.X;
			int fridgeY = islandFarmHouse.fridgePosition.Y;
			if (fridgeX - 2 == x && fridgeY == y)
			{
				return ("Stove", CATEGORY.Interactables);
			}
			else if (fridgeX - 1 == x && fridgeY == y)
			{
				return ("Sink", CATEGORY.Others);
			}
			else if (fridgeX == x && fridgeY == y)
			{
				return ("Fridge", CATEGORY.Interactables);
			}

			return (null, null);
		}

		/// <summary>
		/// Retrieves information about interactables, NPCs, or other features at a given coordinate in an IslandNorth.
		/// </summary>
		/// <param name="islandNorth">The IslandNorth to search.</param>
		/// <param name="x">The x-coordinate to search.</param>
		/// <param name="y">The y-coordinate to search.</param>
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetIslandNorthInfo(IslandNorth islandNorth, int x, int y)
		{
			// Check if the trader is activated and the coordinates match the trader's location
			if (islandNorth.traderActivated.Value && x == 36 && y == 71)
			{
				return ("Island Trader", CATEGORY.Interactables);
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
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetIslandWestInfo(IslandWest islandWest, int x, int y)
		{
			// Check if the coordinates match the shipping bin's location
			if ((islandWest.shippingBinPosition.X == x || (islandWest.shippingBinPosition.X + 1) == x) && islandWest.shippingBinPosition.Y == y)
			{
				return ("Shipping Bin", CATEGORY.Interactables);
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
		/// <returns>A tuple containing the name of the tile and the CATEGORY, or (null, null) if no relevant tile is found.</returns>
		private static (string? name, CATEGORY? category) GetVolcanoDungeonInfo(VolcanoDungeon dungeon, int x, int y)
		{
			if (dungeon.IsCooledLava(x, y))
			{
				return ("Cooled lava", CATEGORY.WaterTiles);
			}
			else if (StardewValley.Monsters.LavaLurk.IsLavaTile(dungeon, x, y))
			{
				return ("Lava", CATEGORY.WaterTiles);
			}

			return (null, null);
		}

		/// <summary>
		/// Retrieves information about interactables, NPCs, or other features at a given coordinate in a named IslandLocation.
		/// </summary>
		/// <param name="islandLocation">The named IslandLocation to search.</param>
		/// <param name="x">The x-coordinate to search.</param>
		/// <param name="y">The y-coordinate to search.</param>
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetNamedIslandLocationInfo(IslandLocation islandLocation, int x, int y)
		{
			object locationType = islandLocation is not null and IslandLocation ? islandLocation.Name ?? "Undefined Island Location" : islandLocation!.GetType();

			// Implement specific logic for named  IslandLocations here, if necessary

			// Unimplemented locations are logged.
			// Check if the location has already been logged
			if (!loggedLocations.Contains(locationType))
			{
				// Log the message
				MainClass.DebugLog($"Called GetNamedIslandLocationInfo with unimplemented IslandLocation of type {islandLocation.GetType()} and name {islandLocation.Name}");

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
				60 => "Emerald Gem Bird",
				62 => "Aquamarine Gem Bird",
				64 => "Ruby Gem Bird",
				66 => "Amethyst Gem Bird",
				68 => "Topaz Gem Bird",
				_ => "Gem Bird", // Default case for when the item index does not match any of the specified values
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
                string toSpeak = $"Parrot required nuts {foundPerch.requiredNuts.Value}";

				// Return appropriate string based on the current state of the parrot perch
				return foundPerch.currentState.Value switch
				{
					StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Idle => foundPerch.IsAvailable() ? toSpeak : "Empty parrot perch",
					StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.StartBuilding => "Parrots started building request",
					StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Building => "Parrots building request",
					StardewValley.BellsAndWhistles.ParrotUpgradePerch.UpgradeState.Complete => "Request Completed",
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
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetIslandLocationInfo(IslandLocation islandLocation, int x, int y)
		{
			var nutTracker = Game1.player.team.collectedNutTracker;
            string? parrot = GetParrotPerchAtTile(islandLocation, x, y);
			if (islandLocation.IsBuriedNutLocation(new Point(x, y)) && !nutTracker.ContainsKey($"Buried_{islandLocation.Name}_{x}_{y}"))
			{
				return ("Diggable spot", CATEGORY.Interactables);
			}
			else if (islandLocation.locationGemBird.Value is IslandGemBird bird && ((int)bird.position.X / Game1.tileSize) == x && ((int)bird.position.Y / Game1.tileSize) == y)
			{
				return (GetGemBirdName(bird), CATEGORY.NPCs);
			}
            else if (parrot != null)
            {
                return (parrot, CATEGORY.Buildings);
            }

		return islandLocation switch
		{
			IslandNorth islandNorth => GetIslandNorthInfo(islandNorth, x, y),
			IslandWest islandWest => GetIslandWestInfo(islandWest, x, y),
			VolcanoDungeon dungeon => GetVolcanoDungeonInfo(dungeon, x, y),
			_ => GetNamedIslandLocationInfo(islandLocation, x, y)
		};
		}

		/// <summary>
		/// Retrieves the value of the "Action" property from the Buildings layer tile at the given coordinates.
		/// </summary>
		/// <param name="libraryMuseum">The LibraryMuseum containing the tile.</param>
		/// <param name="x">The x-coordinate of the tile.</param>
		/// <param name="y">The y-coordinate of the tile.</param>
		/// <returns>The value of the "Action" property as a string, or null if the property is not found.</returns>
		private static string? GetTileActionPropertyValue(LibraryMuseum libraryMuseum, int x, int y)
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
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetLibraryMuseumInfo(LibraryMuseum libraryMuseum, int x, int y)
		{
			if (libraryMuseum.museumPieces.TryGetValue(new Vector2(x, y), out int museumPiece))
			{
				string displayName = Game1.objectInformation[museumPiece].Split('/')[0];
				return ($"{displayName} showcase", CATEGORY.Interactables);
			}

			int booksFound = Game1.netWorldState.Value.LostBooksFound.Value;
			string? action = libraryMuseum.doesTileHaveProperty(x, y, "Action", "Buildings");
			if (action != null && action.Contains("Notes"))
			{
				string? actionPropertyValue = GetTileActionPropertyValue(libraryMuseum, x, y);

				if (actionPropertyValue != null)
				{
					int which = Convert.ToInt32(actionPropertyValue.Split(' ')[1]);
					if (booksFound >= which)
					{
						string message = Game1.content.LoadString("Strings\\Notes:" + which);
						return ($"{message.Split('\n')[0]} Book", CATEGORY.Interactables);
					}
					return ($"Lost Book", CATEGORY.Others);
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
		/// <returns>A tuple containing the name and CATEGORY of the object found, or (null, null) if no relevant object is found.</returns>
		private static (string? name, CATEGORY? category) GetTownInfo(Town town, int x, int y)
		{
			if (SpecialOrder.IsSpecialOrdersBoardUnlocked() && x == 62 && y == 93)
			{
				return ("Special quest board", CATEGORY.Interactables);
			}

			return (null, null);
		}

		/// <summary>
		/// Gets the feeding bench information for barns and coops.
		/// </summary>
		/// <param name="currentLocation">The current GameLocation instance.</param>
		/// <param name="x">The x coordinate of the tile.</param>
		/// <param name="y">The y coordinate of the tile.</param>
		/// <returns>A tuple of (string? name, CATEGORY? category) for the feeding bench, or null if not applicable.</returns>
		private static (string? name, CATEGORY? category)? GetFeedingBenchInfo(GameLocation currentLocation, int x, int y)
		{
			string locationName = currentLocation.Name;

			if (FeedingBenchBounds.TryGetValue(locationName, out var bounds) && x >= bounds.minX && x <= bounds.maxX && y == bounds.y)
			{
				(string? name, CATEGORY category) = TileInfo.getObjectAtTile(x, y, currentLocation, true);
				return (name?.Contains("hay", StringComparison.OrdinalIgnoreCase) == true ? "Feeding Bench" : "Empty Feeding Bench", category);
			}

			return null;
		}

		/// <summary>
		/// Gets information about the current location by its name.
		/// </summary>
		/// <param name="currentLocation">The current GameLocation instance.</param>
		/// <param name="x">The x coordinate of the tile.</param>
		/// <param name="y">The y coordinate of the tile.</param>
		/// <returns>A tuple of (string? name, CATEGORY? category) for the object in the location, or null if not applicable.</returns>
		private static (string? name, CATEGORY? category) GetLocationByNameInfo(GameLocation currentLocation, int x, int y)
		{
            object locationType = currentLocation is not null and GameLocation ? currentLocation.Name ?? "Undefined GameLocation" : currentLocation!.GetType();			string locationName = currentLocation.Name ?? "";
			if (locationName.Contains("coop", StringComparison.OrdinalIgnoreCase) || locationName.Contains("barn", StringComparison.OrdinalIgnoreCase))
			{
				var feedingBenchInfo = GetFeedingBenchInfo(currentLocation, x, y);
				if (feedingBenchInfo.HasValue)
				{
					return feedingBenchInfo.Value;
				} // else if something other than feeding benches in barns and coops...
			} //else if something other than barns and coops...

			// Unimplemented locations are logged.
			// Check if the location has already been logged
			if (!loggedLocations.Contains(locationType))
			{
				// Log the message
				MainClass.DebugLog($"Called GetLocationByNameInfo with unimplemented GameLocation of type {currentLocation.GetType()} and name {currentLocation.Name}");

				// Add the location to the HashSet to prevent logging it again
				loggedLocations.Add(locationType);
			}

			return (null, null);
		}

		/// <summary>
		/// Retrieves the dynamic tile information for the given coordinates in the specified location.
		/// </summary>
		/// <param name="x">The x-coordinate of the tile.</param>
		/// <param name="y">The y-coordinate of the tile.</param>
		/// <param name="currentLocation">The current GameLocation instance.</param>
		/// <param name="lessInfo">An optional boolean to return less detailed information. Defaults to false.</param>
		/// <returns>A tuple containing the name and CATEGORY of the dynamic tile, or null values if not found.</returns>
		public static (string? name, CATEGORY? category) GetDynamicTileAt(int x, int y, GameLocation currentLocation, bool lessInfo = false)
		{
			// Check for panning spots
			if (currentLocation.orePanPoint.Value != Point.Zero && currentLocation.orePanPoint.Value == new Point(x, y))
			{
				return ("panning spot", CATEGORY.Interactables);
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
				Beach beach => GetBeachInfo(beach, x, y),
				BoatTunnel boatTunnel => GetBoatTunnelInfo(boatTunnel, x, y),
				CommunityCenter communityCenter => GetCommunityCenterInfo(communityCenter, x, y),
				Farm farm => GetFarmInfo(farm, x, y),
				FarmHouse farmHouse => GetFarmHouseInfo(farmHouse, x, y),
				Forest forest => GetForestInfo(forest, x, y),
				IslandFarmHouse islandFarmHouse => GetIslandFarmHouseInfo(islandFarmHouse, x, y),
				IslandLocation islandLocation => GetIslandLocationInfo(islandLocation, x, y),
				LibraryMuseum libraryMuseum => GetLibraryMuseumInfo(libraryMuseum, x, y),
				Town town => GetTownInfo(town, x, y),
				_ => GetLocationByNameInfo(currentLocation, x, y)
			};
		}
	}
}
