using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using KdTree;
using KdTree.Math;

namespace stardew_access.Features
{
    /// <summary>
    /// Represents categories that objects can belong to. This class provides predefined categories
    /// accessible as static properties and supports adding new categories at runtime. Predefined categories
    /// can be accessed like enum values, while both static and dynamic categories can be accessed via the
    /// `Categories` property or the `FromString` method.
    /// </summary>
    /// <remarks>
    /// The CATEGORY.Others is used as a default value by the FromString method.
    /// Use the FromString method to obtain an existing category.
    ///
    /// Examples:
    /// - Access a predefined category like an enum: CATEGORY.Farmers
    /// - Obtain a category using the FromString method: CATEGORY.FromString("farmer")
    /// - Add a new category: CATEGORY.AddNewCategory("custom_category")
    /// - Retrieve a category using the public dictionary: CATEGORY.Categories["custom_category"]
    /// - Obtain the string representation of a category: CATEGORY.Farmers.ToString()
    /// </remarks>
    public sealed class CATEGORY
    {
        private readonly string _typeKeyWord;

        private CATEGORY(string typeKeyWord)
        {
            _typeKeyWord = typeKeyWord;
        }

        public override string ToString()
        {
            return _typeKeyWord;
        }

        public static IReadOnlyDictionary<string, CATEGORY> Categories => _categories;

        private static readonly Dictionary<string, CATEGORY> _categories = new(StringComparer.OrdinalIgnoreCase)
        {
            {"farmer", new CATEGORY("farmer")},
            {"animal", new CATEGORY("animal")},
            {"npc", new CATEGORY("npc")},
            {"furniture", new CATEGORY("furniture")},
            {"flooring", new CATEGORY("flooring")},
            {"debris", new CATEGORY("debris")},
            {"crop", new CATEGORY("crop")},
            {"tree", new CATEGORY("tree")},
            {"bush", new CATEGORY("bush")},
            {"building", new CATEGORY("building")},
            {"mine item", new CATEGORY("mine item")},
            {"resource clump", new CATEGORY("resource clump")},
            {"container", new CATEGORY("container")},
            {"bundle", new CATEGORY("bundle")},
            {"door", new CATEGORY("door")},
            {"water", new CATEGORY("water")},
            {"interactable", new CATEGORY("interactable")},
            {"decoration", new CATEGORY("decoration")},
            {"machine", new CATEGORY("machine")},
            {"bridge", new CATEGORY("bridge")},
            {"dropped item", new CATEGORY("dropped item")},
            {"other", new CATEGORY("other")}
        };


        /// <summary>
        /// Retrieves a CATEGORY instance by its string name.
        /// Names are case-insensitive. If the name is not found, returns the 'Others' category.
        /// </summary>
        /// <param name="name">The string name of the category to retrieve.</param>
        /// <returns>The CATEGORY instance corresponding to the given name or the 'Others' category if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided name is null.</exception>
        public static CATEGORY FromString(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Category name cannot be null or empty.", nameof(name));
            }

            return Categories.TryGetValue(name, out CATEGORY? category) ? category ?? CATEGORY.Others : CATEGORY.Others;
        }

        /// <summary>
        /// Adds a new CATEGORY with the specified name.
        /// Names are case-insensitive.
        /// </summary>
        /// <param name="name">The name of the new category to be added.</param>
        /// <returns>
        /// True if a new category was added; false if the category already exists.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided name is null or empty.</exception>
        public static bool AddNewCategory(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            if (!Categories.ContainsKey(name))
            {
                _categories[name] = new CATEGORY(name);
                return true;
            }
            return false;
        }

        public static CATEGORY Farmers => FromString("farmer");
        public static CATEGORY FarmAnimals => FromString("animal");
        public static CATEGORY NPCs => FromString("npc");
        public static CATEGORY Furnitures => FromString("furniture");
        public static CATEGORY Flooring => FromString("flooring");
        public static CATEGORY Debris => FromString("debris");
        public static CATEGORY Crops => FromString("crop");
        public static CATEGORY Trees => FromString("tree");
        public static CATEGORY Bush => FromString("bush");
        public static CATEGORY Buildings => FromString("building");
        public static CATEGORY MineItems => FromString("mine item");
        public static CATEGORY ResourceClumps => FromString("resource clump");
        public static CATEGORY Containers => FromString("container");
        public static CATEGORY JunimoBundle => FromString("bundle");
        public static CATEGORY Doors => FromString("door");
        public static CATEGORY WaterTiles => FromString("water");
        public static CATEGORY Interactables => FromString("interactable");
        public static CATEGORY Decor => FromString("decoration");
        public static CATEGORY Machines => FromString("machine");
        public static CATEGORY Bridges => FromString("bridge");
        public static CATEGORY DroppedItems => FromString("dropped item");
        public static CATEGORY Others => FromString("other");
    }

    public enum MachineState
    {
        Ready, Busy, Waiting
    }

    public static class Utils
    {
        /// <summary>
        /// Loads a JSON file from the specified file name in the assets folder.
        /// </summary>
        /// <param name="fileName">The name of the JSON file to load.</param>
        /// <returns>A <see cref="JsonElement"/> containing the deserialized JSON data, or default if an error occurs.</returns>
        public static JsonElement LoadJsonFile(string fileName)
        {
            string filePath = Path.Combine(MainClass.ModHelper!.DirectoryPath, "assets", fileName);

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<JsonElement>(json);
            }
            catch (FileNotFoundException ex)
            {
                MainClass.ErrorLog($"{fileName} file not found: {ex.Message}");
            }
            catch (JsonException ex)
            {
                MainClass.ErrorLog($"Error parsing {fileName}: {ex.Message}");
            }
            catch (Exception ex)
            {
                MainClass.ErrorLog($"An error occurred while initializing {fileName}: {ex.Message}");
            }

            return default;
        }
    }
    
    /// <summary>
    /// Provides functionality for matching colors based on RGB and HSV values.
    /// </summary>
    public static class ColorMatcher
    {
        /// <summary>
        /// Name of the colors JSON file to load.
        /// </summary>
        private const string ColorsFileName = "colors.json";

        /// <summary>
        /// Dictionary mapping color codes to color names.
        /// </summary>
        private static readonly Dictionary<string, string> _colorCodeToName = LoadColors();

        /// <summary>
        /// Dictionary mapping color tree indices to color codes.
        /// </summary>
        private static Dictionary<int, string> _colorCodeToIndex;

        /// <summary>
        /// KdTree used for nearest neighbor search of colors.
        /// </summary>
        private static readonly KdTree<float, int>_colorTree = CreateColorKDTree(_colorCodeToName, out _colorCodeToIndex);

        /// <summary>
        /// Converts an RGB string in the format 'RRGGBB' to a tuple of integers (r, g, b).
        /// </summary>
        /// <param name="rgb">The RGB string in the format 'RRGGBB'.</param>
        /// <returns>A tuple containing the integer values for red, green, and blue.</returns>
        /// <exception cref="ArgumentException">Thrown when the input string is not in the correct format.</exception>
        private static (int r, int g, int b) RGBStringToInt(string rgb)
        {
            try
            {
                int r = Convert.ToInt32(rgb[..2], 16);
                int g = Convert.ToInt32(rgb[2..4], 16);
                int b = Convert.ToInt32(rgb[4..6], 16);

                return (r, g, b);
            }
            catch (Exception ex)
            {
                MainClass.ErrorLog($"Failed to convert RGB string '{rgb}' to integers: {ex.Message}");
                throw new ArgumentException("The RGB string must be in the format 'RRGGBB'.", nameof(rgb));
            }
        }

        /// <summary>
        /// Converts an HSV color to an RGB color as a tuple of integers (r, g, b).
        /// </summary>
        /// <param name="h">The hue value in the range [0, 360).</param>
        /// <param name="s">The saturation value in the range [0, 1].</param>
        /// <param name="v">The value (brightness) in the range [0, 1].</param>
        /// <returns>A tuple containing the integer values for red, green, and blue.</returns>
        private static (int r, int g, int b) HSVToRGB(float h, float s, float v)
        {
            try
            {
                // Step 1: Calculate the hi value by dividing the hue by 60 and taking the remainder after division by 6.
                int hi = (int)(h / 60) % 6;

                // Step 2: Calculate the f value as the fractional part of the hue divided by 60.
                float f = h / 60 - hi;

                // Step 3: Calculate intermediate values p, q, and t using the value (brightness) and saturation.
                float p = v * (1 - s);
                float q = v * (1 - f * s);
                float t = v * (1 - (1 - f) * s);

                // Step 4: Determine the red, green, and blue values based on the hi value.
                (float r, float g, float b) = hi switch
                {
                    0 => (v, t, p),
                    1 => (q, v, p),
                    2 => (p, v, t),
                    3 => (p, q, v),
                    4 => (t, p, v),
                    _ => (v, p, q)
                };

                // Step 5: Convert the float values for red, green, and blue to integers in the range [0, 255].
                return ((int)(r * 255), (int)(g * 255), (int)(b * 255));
            }
            catch (Exception ex)
            {
                MainClass.ErrorLog($"Failed to convert HSV ({h}, {s}, {v}) to RGB: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads color information from the colors.json file and returns a dictionary mapping color codes to color names.
        /// </summary>
        /// <returns>A dictionary with color codes as keys and color names as values.</returns>
        private static Dictionary<string, string> LoadColors()
        {
            var colorCodeToName = new Dictionary<string, string>();

            JsonElement colorJson = Utils.LoadJsonFile(ColorsFileName);

            foreach (var colorCode in colorJson.EnumerateObject())
            {
                if (colorCode.Value.ValueKind == JsonValueKind.String)
                {
                    colorCodeToName[colorCode.Name] = colorCode.Value.GetString()!;
                }
                else
                {
                    MainClass.ErrorLog($"Failed to parse color name for color code '{colorCode.Name}'.");
                }
            }

            return colorCodeToName;
        }

        /// <summary>
        /// Creates a K-d tree of colors using the provided color code to name dictionary, and outputs a dictionary mapping color indices to color codes.
        /// </summary>
        /// <param name="colorCodeToName">A dictionary mapping color codes to color names.</param>
        /// <param name="colorCodeToIndex">A dictionary that will be populated with color indices as keys and color codes as values.</param>
        /// <returns>A K-d tree of colors with float arrays representing RGB values as keys and color indices as values.</returns>
        private static KdTree<float, int> CreateColorKDTree(Dictionary<string, string> colorCodeToName, out Dictionary<int, string> colorCodeToIndex)
        {
            var colorTree = new KdTree<float, int>(3, new FloatMath());
            colorCodeToIndex = new Dictionary<int, string>();

            int index = 0;
            foreach (var colorCode in colorCodeToName.Keys)
            {
                (int r, int g, int b) = RGBStringToInt(colorCode);

                colorTree.Add(new float[] { r, g, b }, index);
                colorCodeToIndex[index++] = colorCode;
            }

            MainClass.InfoLog($"Loaded {index} colors into the K-d tree.");

            return colorTree;
        }

        /// <summary>
        /// Gets the nearest color name from the K-d tree based on the provided RGB values.
        /// </summary>
        /// <param name="r">The red component of the color (0-255).</param>
        /// <param name="g">The green component of the color (0-255).</param>
        /// <param name="b">The blue component of the color (0-255).</param>
        /// <returns>The name of the nearest color or "Unknown" if a matching color is not found.</returns>
        public static string GetNearestColorName(int r, int g, int b)
        {
            var queryColor = new float[] { r, g, b };
            var nearestNode = _colorTree.GetNearestNeighbours(queryColor, 1);

            if (nearestNode.Length > 0 && _colorCodeToIndex.TryGetValue(nearestNode[0].Value, out string? nearestColorCode))
            {
                return _colorCodeToName.TryGetValue(nearestColorCode!, out var colorName) ? colorName : "Unknown";
            }

            MainClass.ErrorLog($"Unable to find a nearest color for the RGB values: ({r}, {g}, {b}).");
            return "Unknown";
        }

        /// <summary>
        /// Returns the nearest color name to the provided RGB string.
        /// </summary>
        /// <param name="rgb">RGB string in the format "RRGGBB".</param>
        /// <returns>The nearest color name or "Unknown" if the nearest color is not found.</returns>
        public static string GetNearestColorNameFromRGBString(string rgb)
        {
            try
            {
                (int r, int g, int b) = RGBStringToInt(rgb);
                return GetNearestColorName(r, g, b);
            }
            catch (Exception ex)
            {
                MainClass.ErrorLog($"Failed to get nearest color name from RGB string '{rgb}': {ex.Message}");
                return rgb;
            }
        }

        /// <summary>
        /// Returns the nearest color name to the provided HSV values.
        /// </summary>
        /// <param name="h">Hue value in the range [0, 360].</param>
        /// <param name="s">Saturation value in the range [0, 1].</param>
        /// <param name="v">Value (brightness) in the range [0, 1].</param>
        /// <returns>The nearest color name or "Unknown" if the nearest color is not found.</returns>
        public static string GetNearestColorNameFromHSV(float h, float s, float v)
        {
            try
            {
                (int r, int g, int b) = HSVToRGB(h, s, v);
                return GetNearestColorName(r, g, b);
            }
            catch (Exception ex)
            {
                MainClass.ErrorLog($"Failed to get nearest color name from HSV ({h}, {s}, {v}): {ex.Message}");
                (int r, int g, int b) = HSVToRGB(h, s, v);
                return $"{r:X2}{g:X2}{b:X2}";
            }
        }

        /// <summary>
        /// Gets the nearest color name from HSV values provided as percentages in the range of 0-99.
        /// </summary>
        /// <param name="hPercentage">Hue percentage in the range of 0-99.</param>
        /// <param name="sPercentage">Saturation percentage in the range of 0-99.</param>
        /// <param name="vPercentage">Value percentage in the range of 0-99.</param>
        /// <returns>The nearest color name or the RGB representation if the color name cannot be found.</returns>
        public static string GetNearestColorNameFromHSVPercentage(float hPercentage, float sPercentage, float vPercentage)
        {
            // Convert the percentages to the range 0-1 or 0-359
            float h = hPercentage * 359f / 99f;
            float s = sPercentage / 99f;
            float v = vPercentage / 99f;

            return GetNearestColorNameFromHSV(h, s, v);
        }
    }
}
