using StardewValley;

namespace stardew_access.Utils
{
    public class AnimalUtils
    {
        public static Dictionary<(int x, int y), FarmAnimal>? GetAnimalsByLocation(GameLocation location)
        {
            IEnumerable<FarmAnimal>? farmAnimals = location switch
            {
                Farm farm => farm.getAllFarmAnimals(),
                AnimalHouse animalHouse => animalHouse.Animals.Values,
                _ => null
            };

            if (farmAnimals is null) return null;

            Dictionary<(int x, int y), FarmAnimal> animalByCoordinate = [];

            // Populate the dictionary
            foreach (FarmAnimal animal in farmAnimals)
            {
                animalByCoordinate[(animal.getTileX(), animal.getTileY())] = animal;
            }

            return animalByCoordinate;
        }
    }
}
