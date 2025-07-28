using Assets.Scripts.Combat;
using UnityEditor;

/// <summary>
/// Custom PropertyDrawer for the DamageFloatDictionary class.
/// </summary>
[CustomPropertyDrawer(typeof(DamageFloatDictionary))]
public class DamageFloatDictionaryDrawer : EnumFloatDictionaryDrawer { }