using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;

[Serializable]
public class DamageFloatDictionary : SerializableDictionary<DamageType, float> { }

// // [Serializable]
// // public class DamageFloatDictionary : IEnumerable<KeyValuePair<DamageType, float>>
// {
//     [SerializeField] private List<DamageType> keys = new();
//     [SerializeField] private List<float> values = new();
//     [SerializeField] private float defaultValue;

//     public DamageFloatDictionary(float defaultValue = 0f, Dictionary<DamageType, float> initial = null)
//     {
//         this.defaultValue = defaultValue;
//         if (initial != null)
//         {
//             foreach (KeyValuePair<DamageType, float> entry in initial)
//             {
//                 keys.Add(entry.Key);
//                 values.Add(entry.Value);
//             }
//         }
//     }

//     public Dictionary<DamageType, float> ToDictionary()
//     {
//         var dict = new Dictionary<DamageType, float>();
//         for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
//         {
//             dict[keys[i]] = values[i];
//         }
//         return dict;
//     }

//     public float this[DamageType key]
//     {
//         get
//         {
//             int index = keys.IndexOf(key);
//             if (index >= 0)
//                 return values[index];
//             // if the instance does not contain a value for that damage type,
//             // just return the defaultValue
//             return defaultValue;
//         }
//         set
//         {
//             int index = keys.IndexOf(key);
//             if (index >= 0)
//             {
//                 values[index] = value;
//             }
//             else
//             {
//                 keys.Add(key);
//                 values.Add(value);
//             }
//         }
//     }

//     public void SetValue(DamageType key, float value)
//     {
//         int index = keys.IndexOf(key);
//         if (index >= 0)
//             values[index] = value;
//         else
//         {
//             keys.Add(key);
//             values.Add(value);
//         }
//     }

//     public bool TryGetValue(DamageType key, out float value)
//     {
//         int index = keys.IndexOf(key);
//         if (index >= 0)
//         {
//             value = values[index];
//             return true;
//         }

//         value = default;
//         return false;
//     }

//     public void Remove(DamageType key)
//     {
//         int index = keys.IndexOf(key);
//         if (index >= 0)
//         {
//             keys.RemoveAt(index);
//             values.RemoveAt(index);
//         }
//     }

//     public IEnumerator<KeyValuePair<DamageType, float>> GetEnumerator()
//     {
//         for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
//         {
//             yield return new KeyValuePair<DamageType, float>(keys[i], values[i]);
//         }
//     }

//     IEnumerator IEnumerable.GetEnumerator()
//     {
//         return GetEnumerator();
//     }

//     // Accessors for PropertyDrawer
//     public List<DamageType> Keys => keys;
//     public List<float> Values => values;
//     public float DefaultValue => defaultValue;
// }