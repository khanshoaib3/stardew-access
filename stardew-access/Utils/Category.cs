using stardew_access.Translation;

namespace stardew_access.Utils
{
    /// <summary>
    /// Represents categories that objects can belong to. This class provides predefined categories
    /// accessible as static properties and supports adding new categories at runtime. Predefined categories
    /// can be accessed like enum values, while both static and dynamic categories can be accessed via the
    /// `Categories` property or the `FromString` method.
    /// </summary>
    /// <remarks>
    /// The CATEGORY.Other is used as a default value by the FromString method.
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
            if (!Translator.Instance.IsAvailable($"object_category-{_typeKeyWord}"))
                return _typeKeyWord;

            return Translator.Instance.Translate($"object_category-{_typeKeyWord}");
        }

        public string Key => _typeKeyWord;
        public string Value => ToString();

        public static IReadOnlyDictionary<string, CATEGORY> Categories => _categories;

        private static readonly Dictionary<string, CATEGORY> _categories = new(StringComparer.OrdinalIgnoreCase)
        {
            {"animals", new CATEGORY("animals")},
            {"bridge", new CATEGORY("bridge")},
            {"building", new CATEGORY("building")},
            {"bush", new CATEGORY("bush")},
            {"bundle", new CATEGORY("bundle")},
            {"container", new CATEGORY("container")},
            {"crop", new CATEGORY("crop")},
            {"debris", new CATEGORY("debris")},
            {"decoration", new CATEGORY("decoration")},
            {"door", new CATEGORY("door")},
            {"dropped_item", new CATEGORY("dropped_item")},
            {"farmer", new CATEGORY("farmer")},
            {"fishing", new CATEGORY("fishing")},
            {"fishpond", new CATEGORY("fish pond")},
            {"flooring", new CATEGORY("flooring")},
            {"furniture", new CATEGORY("furniture")},
            {"interactable", new CATEGORY("interactable")},
            {"machine", new CATEGORY("machine")},
            {"mine_item", new CATEGORY("mine_item")},
            {"npc", new CATEGORY("npc")},
            {"pending", new CATEGORY("pending")},
            {"ready", new CATEGORY("ready")},
            {"resource_clump", new CATEGORY("resource_clump")},
            {"tree", new CATEGORY("tree")},
            {"water", new CATEGORY("water")},
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

            if (Categories.TryGetValue(name, out CATEGORY? category) && category != null)
            {
                return category;
            } else {
                if (AddNewCategory(name))
                {
                    if (Categories.TryGetValue(name, out var newCategory) && category != null)
                        return newCategory;
                }
            }
            return Other;
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

        public static readonly CATEGORY Animals = FromString("animals");
        public static readonly CATEGORY Bridges = FromString("bridge");
        public static readonly CATEGORY Buildings = FromString("building");
        public static readonly CATEGORY Bundle = FromString("bundle");
        public static readonly CATEGORY Bush = FromString("bush");
        public static readonly CATEGORY Crops = FromString("crop");
        public static readonly CATEGORY Debris = FromString("debris");
        public static readonly CATEGORY Containers = FromString("container");
        public static readonly CATEGORY Decor = FromString("decoration");
        public static readonly CATEGORY Doors = FromString("door");
        public static readonly CATEGORY DroppedItems = FromString("dropped_item");
        public static readonly CATEGORY Farmers = FromString("farmer");
        public static readonly CATEGORY Fishing = FromString("fishing");
        public static readonly CATEGORY Fishpond = FromString("fishpond");
        public static readonly CATEGORY Flooring = FromString("flooring");
        public static readonly CATEGORY Furniture = FromString("furniture");
        public static readonly CATEGORY Interactable = FromString("interactable");
        public static readonly CATEGORY Machines = FromString("machine");
        public static readonly CATEGORY MineItems = FromString("mine_item");
        public static readonly CATEGORY NPCs = FromString("npc");
        public static readonly CATEGORY Pending = FromString("pending");
        public static readonly CATEGORY Ready = FromString("ready");
        public static readonly CATEGORY ResourceClumps = FromString("resource_clump");
        public static readonly CATEGORY Trees = FromString("tree");
        public static readonly CATEGORY Water = FromString("water");
        public static readonly CATEGORY Other = FromString("other");
    }
}