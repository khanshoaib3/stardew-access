using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace stardew_access.Utils
{
    public abstract class CustomDictBase<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
    {
        // Abstract methods that subclasses must implement
        public abstract void Add(TKey key, TValue value);
        public abstract bool ContainsKey(TKey key);
        public abstract bool Remove(TKey key);
        public abstract bool TryGetValue(TKey key, out TValue value);
        public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

        // Concrete implementations that depend on the abstract methods
        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                {
                    return value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                Add(key, value);
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                List<TKey> keys = new();
                foreach (KeyValuePair<TKey, TValue> kvp in this)
                {
                    keys.Add(kvp.Key);
                }
                return new ReadOnlyCollection<TKey>(keys);
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                List<TValue> values = new();
                foreach (KeyValuePair<TKey, TValue> kvp in this)
                {
                    values.Add(kvp.Value);
                }
                return new ReadOnlyCollection<TValue>(values);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        virtual public void Clear()
        {
            foreach (TKey key in Keys)
            {
                Remove(key);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (TryGetValue(item.Key, out TValue value))
            {
                return EqualityComparer<TValue>.Default.Equals(value, item.Value);
            }
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                array[arrayIndex++] = kvp;
            }
        }

        public int Count => Keys.Count;
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual bool IsReadOnly => false;

        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                return Remove(item.Key);
            }
            return false;
        }

        public virtual bool TryAdd(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                Add(key, value);
                return true;
            }
            return false;
        }

        public virtual bool TryAdd(KeyValuePair<TKey, TValue> kvp)
        {
            return TryAdd(kvp.Key, kvp.Value);
        }
    }

    public class OverlayedDictionary<TKey, TValue> : CustomDictBase<TKey, TValue> where TKey : notnull
    {
        private readonly List<IDictionary<TKey, TValue>> internalDictionaries;

        // Constructor 1: Takes an existing list of dictionaries
        public OverlayedDictionary(List<IDictionary<TKey, TValue>> dictionaries)
        {
            this.internalDictionaries = dictionaries;
        }

        // Constructor 2: Takes a single dictionary, adds it to a new list, and calls Constructor 1
        public OverlayedDictionary(IDictionary<TKey, TValue> singleDict) : this(new List<IDictionary<TKey, TValue>> { singleDict }) { }

        // Constructor 3: Creates an empty dictionary, then calls Constructor 2, which in turn calls Constructor 1
        public OverlayedDictionary() : this(new Dictionary<TKey, TValue>()) { }

        public void AddLayer(IDictionary<TKey, TValue> newLayer)
        {
            internalDictionaries.Add(newLayer);
        }

        public void RemoveLayer()
        {
            if (internalDictionaries.Count > 0)
            {
                internalDictionaries.RemoveAt(internalDictionaries.Count - 1);
            }
        }

        public IDictionary<TKey, TValue> GetLayer(int index)
        {
            return internalDictionaries[index];
        }

        public bool ContainsLayer(IDictionary<TKey, TValue> layer)
        {
            return internalDictionaries.Contains(layer);
        }

        public Dictionary<TKey, TValue> Snapshot
        {
            get
            {
                Dictionary<TKey, TValue> snapshot = new();

                // Loop over the dictionaries in reverse order (top layer first)
                for (int i = internalDictionaries.Count - 1; i >= 0; i--)
                {
                    var layer = internalDictionaries[i];
                    foreach (var kvp in layer)
                    {
                        snapshot.TryAdd(kvp.Key, kvp.Value);  // Using Dictionary's built-in TryAdd method
                    }
                }

                return snapshot;
            }
        }

        public static explicit operator Dictionary<TKey, TValue>(OverlayedDictionary<TKey, TValue> overlayed)
        {
            return overlayed.Snapshot; // or whatever you end up naming the property
        }

        public override void Add(TKey key, TValue value)
        {
            internalDictionaries[^1].Add(key, value);  // Add to the top-most layer
        }

        public override void Clear()
        {
            internalDictionaries.Clear();
            internalDictionaries.Add(new Dictionary<TKey, TValue>());  // Add an empty dictionary as the new bottom layer
        }

        public override bool ContainsKey(TKey key)
        {
            foreach (IDictionary<TKey, TValue> dict in internalDictionaries)
            {
                if (dict.ContainsKey(key))
                {
                    return true;
                }
            }
            return false;
        }

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            HashSet<TKey> seenKeys = new();
            for (int i = internalDictionaries.Count - 1; i >= 0; i--)
            {
                IDictionary<TKey, TValue> dict = internalDictionaries[i];
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

        public override bool Remove(TKey key)
        {
            for (int i = internalDictionaries.Count - 1; i >= 0; i--)
            {
                if (internalDictionaries[i].ContainsKey(key))
                {
                    internalDictionaries[i].Remove(key);
                    return true;  // Remove only the top-most occurrence and break
                }
            }
            return false;
        }

        public bool Remove(TKey key, bool all)
        {
            if (!all)
            {
                return Remove(key);  // Call Remove 1 if 'all' is false
            }
            else
            {
                bool removed = false;
                for (int i = internalDictionaries.Count - 1; i >= 0; i--)
                {
                    if (internalDictionaries[i].ContainsKey(key))
                    {
                        internalDictionaries[i].Remove(key);
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

        public override bool TryGetValue(TKey key, out TValue value)
        {
            foreach (IDictionary<TKey, TValue> dict in internalDictionaries)
            {
                // possible null value is expected here
                #pragma warning disable CS8601 // Possible null reference assignment.
                if (dict.TryGetValue(key, out value))
                {
                    return true;
                }
            }
            value = default;
            #pragma warning restore CS8601 // Possible null reference assignment.
            return false;
        }
    }
}
