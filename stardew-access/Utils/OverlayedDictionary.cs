using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace stardew_access.Utils
{
    public class OverlayedDictionary<TKey, TValue> : CustomDictBase<TKey, TValue> where TKey : notnull
    {
        private readonly List<IDictionary<TKey, TValue>> layers;
        private readonly Dictionary<string, IDictionary<TKey, TValue>> layersByName;

        #region Constructors
        // Constructor 1: Takes an existing list of dictionaries
        public OverlayedDictionary(List<IDictionary<TKey, TValue>> dictionaries, List<string?>? layerNames = null)
        {
            // Initialize layers
            this.layers = dictionaries;
            this.layersByName = new();

            // If layer names are provided, ensure the counts match
            if (layerNames != null)
            {
                if (layerNames.Count != dictionaries.Count)
                {
                    throw new ArgumentException("The number of layer names must match the number of dictionaries.");
                }

                // Populate the named layers
                for (int i = 0; i < layerNames.Count; i++)
                {
                    string? name = layerNames[i];
                    if (name != null)
                    {
                        layersByName[name] = dictionaries[i];
                    }
                }
            }
        }

        // Constructor 2: Takes a single dictionary, adds it to a new list, and calls Constructor 1
        public OverlayedDictionary(IDictionary<TKey, TValue> singleLayer, string? singleLayerName = null) : this(new List<IDictionary<TKey, TValue>> { singleLayer }, singleLayerName == null ? null : new List<string?> { singleLayerName }) { }

        // Constructor 3: Creates an empty dictionary, then calls Constructor 2, which in turn calls Constructor 1
        public OverlayedDictionary(string? singleLayerName = null) : this(new Dictionary<TKey, TValue>(), singleLayerName) { }
        #endregion

        #region Layer Management
        public void AddLayer(IDictionary<TKey, TValue> newLayer, string? layerName = null)
        {
            layers.Add(newLayer);
            if (layerName != null)
                layersByName.Add(layerName, newLayer);
        }

        public bool ContainsLayer(IDictionary<TKey, TValue> layer)
        {
            return layers.Contains(layer);
        }

        public IDictionary<TKey, TValue>? GetLayer(int index)
        {
            if (index >= 0 && index < layers.Count)
            {
                return layers[index];
            }
            return null;
        }

        public IDictionary<TKey, TValue>? GetLayer(string layerName)
        {
            layersByName.TryGetValue(layerName, out IDictionary<TKey, TValue>? layer);
            return layer;
        }

        public string? GetLayerName(IDictionary<TKey, TValue> layer)
        {
            return layersByName.FirstOrDefault(x => x.Value.Equals(layer)).Key;
        }

        private IDictionary<TKey, TValue> GetReadableLayer(string layerName)
        {
            // Fetch the specific layer by its name
            if (layersByName.TryGetValue(layerName, out IDictionary<TKey, TValue>? layer) && layer != null)
            {
                // Check if the layer exists and is not read-only
                if (layer.IsReadOnly)
                {
                    throw new InvalidOperationException($"The layer '{layerName}' is read-only.");
                }
                return layer;
            }
            else
            {
                throw new ArgumentException($"No layer exists with the name '{layerName}'.");
            }
        }

        public void RemoveLayer(IDictionary<TKey, TValue>? layerToRemove = null)
        {
            if (layerToRemove == null && layers.Count > 0)
            {
                layerToRemove = layers[^1];  // Get the last layer
            }
            
            if (layerToRemove != null)
            {
                layers.Remove(layerToRemove);
                
                // Remove from layersByName if it exists
                string keyToRemove = layersByName.FirstOrDefault(x => x.Value.Equals(layerToRemove)).Key;
                if (!string.IsNullOrEmpty(keyToRemove))
                {
                    layersByName.Remove(keyToRemove);
                }
            }
        }

        public void RemoveLayer(int index)
        {
            if (index >= 0 && index < layers.Count)
            {
                IDictionary<TKey, TValue> layerToRemove = layers[index];
                RemoveLayer(layerToRemove);
            }
            else
            {
                Log.Verbose($"No such layer exists at index {index}.", true);
            }
        }

        public void RemoveLayer(string layerName)
        {
            if (layersByName.TryGetValue(layerName, out IDictionary<TKey, TValue>? layerToRemove))
            {
                RemoveLayer(layerToRemove!);
            }
            else
            {
                Log.Verbose($"No such layer exists by key name {layerName}.", true);
            }
        }

        public Dictionary<TKey, TValue> Snapshot
        {
            get
            {
                Dictionary<TKey, TValue> snapshot = new();

                // Loop over the dictionaries in reverse order (top layer first)
                for (int i = layers.Count - 1; i >= 0; i--)
                {
                    var layer = layers[i];
                    foreach (var kvp in layer)
                    {
                        snapshot.TryAdd(kvp.Key, kvp.Value);  // Using Dictionary's built-in TryAdd method
                    }
                }

                return snapshot;
            }
        }
        #endregion

        #region Dictionary Interface
        public static explicit operator Dictionary<TKey, TValue>(OverlayedDictionary<TKey, TValue> overlayed)
        {
            return overlayed.Snapshot;
        }

        public override void Add(TKey key, TValue value)
        {
            if (IsReadOnly) throw new InvalidOperationException("Dictionary is read-only.");
            layers[^1].Add(key, value);  // Add to the top-most layer
        }

        public void Add(TKey key, TValue value, string layerName) => GetReadableLayer(layerName).Add(key, value);

        public void Add(KeyValuePair<TKey, TValue> item, string layerName) => GetReadableLayer(layerName).Add(item);

        public override void Clear()
        {
            if (IsReadOnly) throw new InvalidOperationException("Dictionary is read-only.");
            layers.Clear();
            layers.Add(new Dictionary<TKey, TValue>());  // Add an empty dictionary as the new bottom layer
        }

        public void Clear(string layerName) => GetReadableLayer(layerName).Clear();
        
        public bool Contains(KeyValuePair<TKey, TValue> item, string layerName) => GetReadableLayer(layerName).Contains(item);

        public override bool ContainsKey(TKey key)
        {
            foreach (IDictionary<TKey, TValue> dict in layers)
            {
                if (dict.ContainsKey(key))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsKey(TKey key, string layerName) => GetReadableLayer(layerName).ContainsKey(key);

        public List<TValue> GetAllValuesForKey(TKey key)
        {
            List<TValue> values = new();
            
            // Iterate backwards over the layers to preserve the overlay order
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                if (layers[i].TryGetValue(key, out TValue? value))
                {
                    values.Add(value!);
                }
            }
            
            return values;
        }

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            HashSet<TKey> seenKeys = new();
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                IDictionary<TKey, TValue> dict = layers[i];
                foreach (KeyValuePair<TKey, TValue> kvp in dict)
                {
                    // Skip this key if we've already seen it in a higher layer
                    if (seenKeys.Contains(kvp.Key))
                    {
                        continue;
                    }

                    // Add to seen keys and yield return
                    seenKeys.Add(kvp.Key);
                    yield return kvp;
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator(string layerName) => GetReadableLayer(layerName).GetEnumerator();

        public override bool Remove(TKey key)
        {
            if (IsReadOnly) throw new InvalidOperationException("Dictionary is read-only.");
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                if (layers[i].ContainsKey(key))
                {
                    layers[i].Remove(key);
                    return true;  // Remove only the top-most occurrence and break
                }
            }
            return false;
        }

        public bool Remove(TKey key, string layerName) => GetReadableLayer(layerName).Remove(key);


        public bool Remove(KeyValuePair<TKey, TValue> item, string layerName) => GetReadableLayer(layerName).Remove(item);

        public bool Remove(TKey key, bool all)
        {
            if (!all)
            {
                return Remove(key);  // Call Remove 1 if 'all' is false
            }
            else
            {
                bool removed = false;
                for (int i = layers.Count - 1; i >= 0; i--)
                {
                    if (layers[i].ContainsKey(key))
                    {
                        if (IsReadOnly) Log.Warn($"Layer at index {i} is read-only; cannot remove key {key}.");

                        layers[i].Remove(key);
                        removed = true;
                    }
                }
                return removed;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item, bool all)
        {
            if (Contains(item))
            {
                return Remove(item.Key, all);  // Call the Remove 2 function with just the key and 'all'
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("OverlayedDictionary {");
            sb.AppendLine($"\tNumber of Layers: {layers.Count}");
            sb.AppendLine($"\tNamed Layers: {string.Join(", ", layersByName.Keys)}");

            for (int i = 0; i < layers.Count; i++)
            {
                var layer = layers[i];
                string name = GetLayerName(layer) ?? "";
                if (name != "") name = $" ({name})";
                sb.AppendLine($"\tLayer {i}{name}:");
                string layerType = layer is OverlayedDictionary<TKey, TValue> ? "OverlayedDictionary" : "Dictionary";
                sb.AppendLine($"\t\tType: {layerType}");

                foreach (var keyValuePair in layer)
                {
                    sb.AppendLine($"\t\tKey: {keyValuePair.Key}, Value: {keyValuePair.Value}");
                }
            }

            sb.Append('}');
            return sb.ToString();
        }

        public bool TryAdd(TKey key, TValue value, string layerName) => GetReadableLayer(layerName).TryAdd(key, value);

        public override bool TryGetValue(TKey key, out TValue value)
        {
            for (int i = layers.Count-1; i>=0; i--)
            {
                IDictionary<TKey, TValue> dict = layers[i];
                // possible null value is expected here
                #pragma warning disable CS8601 // Possible null reference assignment.
                if (dict.TryGetValue(key, out value))
                {
                    return true;
                }
            }
            value = default;
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value, string layerName) => GetReadableLayer(layerName).TryGetValue(key, out value);
        #pragma warning restore CS8601 // Possible null reference assignment.
        #endregion
    }
}
