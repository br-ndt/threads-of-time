using Assets.Scripts.Configs;
using UnityEditor;

/// <summary>
/// Custom PropertyDrawer for the ConditionFloatDictionary class.
/// </summary>
[CustomPropertyDrawer(typeof(ConditionFloatDictionary))]
public class ConditionFloatDictionaryDrawer : EnumFloatDictionaryDrawer { }