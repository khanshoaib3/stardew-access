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
            {"bridges", new CATEGORY("bridges")},
            {"buildings", new CATEGORY("buildings")},
            {"bushes", new CATEGORY("bushes")},
            {"bundles", new CATEGORY("bundles")},
            {"containers", new CATEGORY("containers")},
            {"crops", new CATEGORY("crops")},
            {"debris", new CATEGORY("debris")},
            {"decor", new CATEGORY("decor")},
            {"doors", new CATEGORY("doors")},
            {"dropped_items", new CATEGORY("dropped_items")},
            {"farmers", new CATEGORY("farmers")},
            {"fishing", new CATEGORY("fishing")},
            {"fishponds", new CATEGORY("fish ponds")},
            {"flooring", new CATEGORY("flooring")},
            {"furniture", new CATEGORY("furniture")},
            {"interactables", new CATEGORY("interactables")},
            {"machines", new CATEGORY("machines")},
            {"mine_items", new CATEGORY("mine_items")},
            {"npcs", new CATEGORY("npcs")},
            {"other", new CATEGORY("other")},
            {"pending", new CATEGORY("pending")},
            {"quest", new CATEGORY("quest")},
            {"ready", new CATEGORY("ready")},
            {"resource_clumps", new CATEGORY("resource_clumps")},
            {"trees", new CATEGORY("trees")},
            {"unknown", new CATEGORY("unknown")},
            {"water", new CATEGORY("water")}
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
        public static readonly CATEGORY Bridges = FromString("bridges");
        public static readonly CATEGORY Buildings = FromString("buildings");
        public static readonly CATEGORY Bundles = FromString("bundles");
        public static readonly CATEGORY Bushes = FromString("bushes");
        public static readonly CATEGORY Crops = FromString("crops");
        public static readonly CATEGORY Debris = FromString("debris");
        public static readonly CATEGORY Containers = FromString("containers");
        public static readonly CATEGORY Decor = FromString("decor");
        public static readonly CATEGORY Doors = FromString("doors");
        public static readonly CATEGORY DroppedItems = FromString("dropped_items");
        public static readonly CATEGORY Farmers = FromString("farmers");
        public static readonly CATEGORY Fishing = FromString("fishing");
        public static readonly CATEGORY Fishponds = FromString("fishponds");
        public static readonly CATEGORY Flooring = FromString("flooring");
        public static readonly CATEGORY Furniture = FromString("furniture");
        public static readonly CATEGORY Interactables = FromString("interactables");
        public static readonly CATEGORY Machines = FromString("machines");
        public static readonly CATEGORY MineItems = FromString("mine_items");
        public static readonly CATEGORY NPCs = FromString("npcs");
        public static readonly CATEGORY Other = FromString("other");
        public static readonly CATEGORY Pending = FromString("pending");
        public static readonly CATEGORY Quest = FromString("quest");
        public static readonly CATEGORY Ready = FromString("ready");
        public static readonly CATEGORY ResourceClumps = FromString("resource_clumps");
        public static readonly CATEGORY Trees = FromString("trees");
        public static readonly CATEGORY Unknown = FromString("unknown");
        public static readonly CATEGORY Water = FromString("water");
    }
}