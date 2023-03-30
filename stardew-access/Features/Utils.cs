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

        private static readonly Dictionary<string, CATEGORY> _categories = new Dictionary<string, CATEGORY>(StringComparer.OrdinalIgnoreCase)
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

            return Categories.TryGetValue(name, out CATEGORY category) ? category : Others;
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
}