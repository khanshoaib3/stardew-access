using Microsoft.Xna.Framework;
using stardew_access.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stardew_access.Features.Tracker
{
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
                this.AddObjects(StardewAccessObjects.GetObjects());
            }

            TTSpecialPoints SpecialPointsObjects = new();
            if (SpecialPointsObjects.HasObjects()) {
                this.AddObjects(SpecialPointsObjects.GetObjects());
            }

            TTAnimals AnimalObjects = new();
            if (AnimalObjects.HasObjects()) {
                this.AddObjects(AnimalObjects.GetObjects());
            }

            if (!sortByProximity) {
                MainClass.DebugLog("Sorting alphabetically");
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
}
