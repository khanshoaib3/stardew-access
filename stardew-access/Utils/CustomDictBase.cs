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
                List<TKey> keys = [];
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
                List<TValue> values = [];
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
            if (IsReadOnly) throw new InvalidOperationException("Dictionary is read-only.");
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
    }
