using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace stardew_access.Tiles
{
    public static class AccessibleTileHelpers
    {
        // Separate Dictionaries for each category of helper functions
        private static readonly Dictionary<string, Func<ConditionalBase, string>> NameHelpers = new();
        private static readonly Dictionary<string, Func<ConditionalBase, bool>> ConditionHelpers = new();
        private static readonly Dictionary<string, Func<ConditionalBase, Vector2>> CoordinateHelpers = new();
        private static readonly Dictionary<string, Func<ConditionalBase, Dictionary<Vector2, string>>> CoordinatesHelpers = new();

        // Register function accepting a category, name, and delegate
        public static void Register(string category, string name, Delegate function)
        {
            #if DEBUG
            Log.Trace($"AccessibleTileHelpers: Registering new {category} helper \"{name}\"");
            #endif
            switch (category)
            {
                case "Name":
                    NameHelpers[name] = (Func<ConditionalBase, string>)function;
                    break;
                case "Condition":
                    ConditionHelpers[name] = (Func<ConditionalBase, bool>)function;
                    break;
                case "Coordinate":
                    CoordinateHelpers[name] = (Func<ConditionalBase, Vector2>)function;
                    break;
                case "Coordinates":
                    CoordinatesHelpers[name] = (Func<ConditionalBase, Dictionary<Vector2, string>>)function;
                    break;
                default:
                    throw new ArgumentException("Unknown category");
            }
        }

        // Auto-registration at construction time
        static AccessibleTileHelpers()
        {
            Log.Trace("Initializing AccessibleTileHelpers");
            foreach (var method in typeof(AccessibleTileHelpers).GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                var parts = method.Name.Split('_', 2);
                if (parts.Length == 2)
                {
                    string category = parts[0];
                    string name = parts[1];
                    Delegate function = category switch
                    {
                        "Name" => Delegate.CreateDelegate(typeof(Func<ConditionalBase, string>), method),
                        "Condition" => Delegate.CreateDelegate(typeof(Func<ConditionalBase, bool>), method),
                        "Coordinate" => Delegate.CreateDelegate(typeof(Func<ConditionalBase, Vector2>), method),
                        "Coordinates" => Delegate.CreateDelegate(typeof(Func<ConditionalBase, Dictionary<Vector2, string>>), method),
                        _ => throw new ArgumentException("Unknown category")
                    };
                    Register(category, name, function);
                }
            }
        }

        // Retrieval methods for each category
        public static bool TryGetNameHelper(string name, out Func<ConditionalBase, string>? nameFunc)
        {
            return NameHelpers.TryGetValue(name, out nameFunc);
        }

        public static bool TryGetConditionHelper(string name, out Func<ConditionalBase, bool>? conditionFunc)
        {
            return ConditionHelpers.TryGetValue(name, out conditionFunc);
        }

        public static bool TryGetCoordinateHelper(string name, out Func<ConditionalBase, Vector2>? coordinateFunc)
        {
            return CoordinateHelpers.TryGetValue(name, out coordinateFunc);
        }

        public static bool TryGetCoordinatesHelper(string name, out Func<ConditionalBase, Dictionary<Vector2, string>>? coordinatesFunc)
        {
            return CoordinatesHelpers.TryGetValue(name, out coordinatesFunc);
        }

        // helpers
        public static bool Condition_Farm(ConditionalBase obj)
        {
            if (obj.ConditionArgs.TryGetValue("Farm", out string? args) && !string.IsNullOrEmpty(args))
            {
                // Map the farm names to indexes
                int farmTypeIndex = args.ToLower() switch
                {
                    "standard" => 0,
                    "riverland" => 1,
                    "forest" => 2,
                    "hill-top" => 3,
                    "wilderness" => 4,
                    "fourcorners" => 5,
                    "beach" => 6,
                    _ => 7,
                };
                // Return true if current farm matches args; false otherwise.
                return farmTypeIndex == Game1.whichFarm;
            }
            throw new ArgumentException($"Farm name for Farm condition on {obj} must be non-empty and one of 'default', 'riverland', 'forest', 'hill-top', 'wilderness', 'fourcorners', or 'beach'");
        }

        public static bool Condition_FarmHouse(ConditionalBase obj)
        {
            if (obj.ConditionArgs.TryGetValue("FarmHouse", out string? args) && !string.IsNullOrEmpty(args))
            {
                // args should be digits
                if (int.TryParse(args, out int level))
                {
                    FarmHouse house = Utility.getHomeOfFarmer(Game1.player!);
                    return house.upgradeLevel >= level;
                }
            }
            throw new ArgumentException($"Level for FarmHouse on {obj} must be a non-empty, valid integer");
        }

        public static bool Condition_HasQuest(ConditionalBase obj)
        {
            if (obj.ConditionArgs.TryGetValue("HasQuest", out string? args) && !string.IsNullOrEmpty(args))
            {
                // args should be digits
                if (int.TryParse(args, out int id))
                {
                    return Game1.player!.hasQuest(id);
                }
            }
            throw new ArgumentException($"ID for HasQuest on {obj} must be a non-empty, valid integer");
        }

        public static bool Condition_JojaMember(ConditionalBase _)
        {
            // Return true if the player has the "JojaMember" mail, otherwise false
            return Game1.MasterPlayer.mailReceived.Contains("JojaMember");
        }

    }
}