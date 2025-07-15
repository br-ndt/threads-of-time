using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    // Mark the class as Serializable so Unity can save and load it.
    // Use generic type parameters K for Key and V for Value.
    // The where clauses constrain K and V to be types that Unity can serialize.
    // For K, Enum, string, int, float, bool, etc., are generally serializable.
    // For V, float, int, string, Vector3, etc., are generally serializable.
    [Serializable]
    public class SerializableDictionary<K, V> : IEnumerable<KeyValuePair<K, V>>
    {
        // Use private fields with SerializeField attribute to make them visible and editable in the Unity Inspector.
        // Unity's serialization system works by saving the public fields and fields marked with [SerializeField].
        [SerializeField] private List<K> keys = new();
        [SerializeField] private List<V> values = new();

        // Constructor to initialize the dictionary with an optional default value and initial data.
        // The 'default(V)' provides the default value for type V (e.g., 0 for numbers, null for reference types).
        public SerializableDictionary(Dictionary<K, V> initial = null)
        {
            if (initial != null)
            {
                foreach (KeyValuePair<K, V> entry in initial)
                {
                    keys.Add(entry.Key);
                    values.Add(entry.Value);
                }
            }
        }

        /// <summary>
        /// Converts this SerializableDictionary to a standard System.Collections.Generic.Dictionary.
        /// This is useful for performing operations not easily done with the serialized lists,
        /// or for compatibility with APIs that expect a standard Dictionary.
        /// </summary>
        /// <returns>A new Dictionary containing the key-value pairs.</returns>
        public Dictionary<K, V> ToDictionary()
        {
            var dict = new Dictionary<K, V>();
            // Ensure both lists are of the same length to avoid out-of-bounds errors
            for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
            {
                // Add or update the dictionary. If a key appears multiple times in 'keys', the last value wins.
                dict[keys[i]] = values[i];
            }
            return dict;
        }

        /// <summary>
        /// Indexer for accessing or setting values by key.
        /// This allows dictionary-like syntax: myDictionary[key] = value;
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The value associated with the specified key, or the default value if the key is not found.</returns>
        public V this[K key]
        {
            get
            {
                int index = keys.IndexOf(key);
                if (index >= 0 && index < values.Count) // Ensure index is valid for values list as well
                    return values[index];

                // If the key is not found, return the predefined default value.
                throw new Exception($"Invalid accessor {key} in {this}");
            }
            set
            {
                int index = keys.IndexOf(key);
                if (index >= 0 && index < values.Count)
                {
                    // If key exists, update its corresponding value.
                    values[index] = value;
                }
                else
                {
                    // If key does not exist, add a new key-value pair.
                    keys.Add(key);
                    values.Add(value);
                }
            }
        }

        /// <summary>
        /// Sets the value for a given key. Adds the key-value pair if the key does not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(K key, V value)
        {
            // This method provides an explicit way to set a value,
            // functionally similar to the setter of the indexer.
            this[key] = value;
        }

        /// <summary>
        /// Attempts to retrieve the value associated with the specified key.
        /// This method is safer than direct access via the indexer if a key may not exist,
        /// as it avoids adding a new entry if the key is not found and allows checking for existence.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the value parameter.</param>
        /// <returns>True if the dictionary contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(K key, out V value)
        {
            int index = keys.IndexOf(key);
            if (index >= 0 && index < values.Count)
            {
                value = values[index];
                return true;
            }

            value = default; // Assign default value for V if key not found
            return false;
        }

        /// <summary>
        /// Removes the key-value pair with the specified key from the SerializableDictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        public void Remove(K key)
        {
            int index = keys.IndexOf(key);
            if (index >= 0 && index < values.Count)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
            }
        }

        /// <summary>
        /// Gets the number of key-value pairs contained in the SerializableDictionary.
        /// </summary>
        public int Count => keys.Count; // Assuming keys.Count and values.Count are always synchronized after operations

        /// <summary>
        /// Gets a collection containing the keys in the SerializableDictionary.
        /// </summary>
        public IReadOnlyList<K> Keys => keys.AsReadOnly();

        /// <summary>
        /// Gets a collection containing the values in the SerializableDictionary.
        /// </summary>
        public IReadOnlyList<V> Values => values.AsReadOnly();

        // --- IEnumerable Implementation ---
        // These methods enable the SerializableDictionary to be iterated over using a foreach loop.

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
            {
                yield return new KeyValuePair<K, V>(keys[i], values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}