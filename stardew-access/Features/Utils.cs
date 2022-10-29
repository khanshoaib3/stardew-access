namespace stardew_access.Features
{
    /// <summary>
    /// This is a custom enum class and contains the name of groups the objects are divided into for the feature
    /// </summary>
    public class CATEGORY
    {
        private string _typeKeyWord;

        private CATEGORY(string typeKeyWord)
        {
            _typeKeyWord = typeKeyWord;
        }

        public override string ToString()
        {
            return _typeKeyWord;
        }

        public static CATEGORY FromString(string name)
        {
            if (name == "farmer")
                return CATEGORY.Farmers;
            else if (name == "animal")
                return CATEGORY.FarmAnimals;
            else if (name == "npc")
                return CATEGORY.NPCs;
            else if (name == "furniture")
                return CATEGORY.Furnitures;
            else if (name == "flooring")
                return CATEGORY.Flooring;
            else if (name == "debris")
                return CATEGORY.Debris;
            else if (name == "crop")
                return CATEGORY.Crops;
            else if (name == "tree")
                return CATEGORY.Trees;
            else if (name == "bush")
                return CATEGORY.Bush;
            else if (name == "building")
                return CATEGORY.Buildings;
            else if (name == "mine item")
                return CATEGORY.MineItems;
            else if (name == "resource clump")
                return CATEGORY.ResourceClumps;
            else if (name == "container")
                return CATEGORY.Containers;
            else if (name == "bundle")
                return CATEGORY.JunimoBundle;
            else if (name == "door")
                return CATEGORY.Doors;
            else if (name == "water")
                return CATEGORY.WaterTiles;
            else if (name == "interactable")
                return CATEGORY.Interactables;
            else if (name == "decoration")
                return CATEGORY.Decor;
            else if (name == "machine")
                return CATEGORY.Machines;
            else if (name == "bridge")
                return CATEGORY.Bridges;
            else if (name == "dropped item")
                return CATEGORY.DroppedItems;
            else if (name == "other")
                return CATEGORY.Others;

            return Others;
        }

        public static CATEGORY Farmers = new CATEGORY("farmer");
        public static CATEGORY FarmAnimals = new CATEGORY("animal");
        public static CATEGORY NPCs = new CATEGORY("npc");
        public static CATEGORY Furnitures = new CATEGORY("furniture");
        public static CATEGORY Flooring = new CATEGORY("flooring");
        public static CATEGORY Debris = new CATEGORY("debris");
        public static CATEGORY Crops = new CATEGORY("crop");
        public static CATEGORY Trees = new CATEGORY("tree");
        public static CATEGORY Bush = new CATEGORY("bush");
        public static CATEGORY Buildings = new CATEGORY("building");
        public static CATEGORY MineItems = new CATEGORY("mine item");
        public static CATEGORY ResourceClumps = new CATEGORY("resource clump");
        public static CATEGORY Containers = new CATEGORY("container");
        public static CATEGORY JunimoBundle = new CATEGORY("bundle");
        public static CATEGORY Doors = new CATEGORY("door"); // Also includes ladders and elevators
        public static CATEGORY WaterTiles = new CATEGORY("water");
        public static CATEGORY Interactables = new CATEGORY("interactable");
        public static CATEGORY Decor = new CATEGORY("decoration");
        public static CATEGORY Machines = new CATEGORY("machine");
        public static CATEGORY Bridges = new CATEGORY("bridge");
        public static CATEGORY DroppedItems = new CATEGORY("dropped item");
        public static CATEGORY Others = new CATEGORY("other");

    }

    public enum MachineState
    {
        Ready, Busy, Waiting
    }
}