using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace stardew_access.Tiles
{
    public static class AccessibleTileHelpers
    {
        // Separate Dictionaries for each category of helper functions
        private static readonly Dictionary<string, Func<Object?, string>> NameHelpers = new();
        private static readonly Dictionary<string, Func<Object?, bool>> ConditionHelpers = new();
        private static readonly Dictionary<string, Func<Object?, Vector2>> CoordinateHelpers = new();
        private static readonly Dictionary<string, Func<Object?, Dictionary<Vector2, string>>> CoordinatesHelpers = new();

        // Register function accepting a category, name, and delegate
        public static void Register(string category, string name, Delegate function)
        {
            switch (category)
            {
                case "Name":
                    NameHelpers[name] = (Func<Object?, string>)function;
                    break;
                case "Condition":
                    ConditionHelpers[name] = (Func<Object?, bool>)function;
                    break;
                case "Coordinate":
                    CoordinateHelpers[name] = (Func<Object?, Vector2>)function;
                    break;
                case "Coordinates":
                    CoordinatesHelpers[name] = (Func<Object?, Dictionary<Vector2, string>>)function;
                    break;
                default:
                    throw new ArgumentException("Unknown category");
            }
        }

        // Auto-registration at construction time
        static AccessibleTileHelpers()
        {
            foreach (var method in typeof(AccessibleTileHelpers).GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                var parts = method.Name.Split('_', 2);
                if (parts.Length == 2)
                {
                    Register(parts[0], parts[1], Delegate.CreateDelegate(method.ReturnType, method));
                }
            }
        }

        // Retrieval methods for each category
        public static Func<Object?, string>? GetNameHelper(string name)
        {
            return NameHelpers.GetValueOrDefault(name);
        }

        public static Func<Object?, bool>? GetConditionHelper(string name)
        {
            return ConditionHelpers.GetValueOrDefault(name);
        }

        public static Func<Object?, Vector2>? GetCoordinateHelper(string name)
        {
            return CoordinateHelpers.GetValueOrDefault(name);
        }

        public static Func<Object?, Dictionary<Vector2, string>>? GetCoordinatesHelper(string name)
        {
            return CoordinatesHelpers.GetValueOrDefault(name);
        }
    }
}