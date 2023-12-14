namespace stardew_access.Features.Tracker;

using Utils;
using System.Collections.Generic;
using System.Linq;

class TrackedObjects
{
    /// <summary>
    /// {column:[{Name, SpecialObject}]}
    /// </summary>
    private readonly SortedList<string, Dictionary<string, SpecialObject>> Objects = new();
        
    public SortedList<string, Dictionary<string, SpecialObject>> GetObjects()
    {
        return Objects;
    }

    public void FindObjectsInArea(bool sortByProximity = true)
    {

        TTStardewAccess StardewAccessObjects = new();
        if (StardewAccessObjects.HasObjects()) {
            AddObjects(StardewAccessObjects.GetObjects());
        }

        TTSpecialPoints SpecialPointsObjects = new();
        if (SpecialPointsObjects.HasObjects()) {
            AddObjects(SpecialPointsObjects.GetObjects());
        }

        TTAnimals AnimalObjects = new();
        if (AnimalObjects.HasObjects()) {
            AddObjects(AnimalObjects.GetObjects());
        }

        if (!sortByProximity) {
            Log.Debug("Sorting alphabetically");
            foreach (var cat in Objects) {
                var ordered = cat.Value.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                cat.Value.Clear();
                foreach (var item in ordered) {
                    cat.Value.Add(item.Key, item.Value);
                }
            }
        }
    }

    private void AddObjects(SortedList<string, Dictionary<string, SpecialObject>> objectsToAdd)
    {
        foreach(var kvp in objectsToAdd) {

            string category = kvp.Key;

            if (!Objects.ContainsKey(category)) {
                Objects.Add(category, new());
            }

            foreach(var obj in kvp.Value) {
                if (!Objects.GetValueOrDefault(category!)!.ContainsKey(obj!.Key))
                    Objects.GetValueOrDefault(category)!.Add(obj!.Key, obj!.Value);
            }
        }
    }

}