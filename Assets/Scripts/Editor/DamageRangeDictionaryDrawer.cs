using System.Collections.Generic;
using System;
using System.Linq;
using Assets.Scripts.Combat;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom PropertyDrawer for the DamageRangeDictionary.
/// This is very similar to the previous dictionary drawer, but targets the new type.
/// </summary>
[CustomPropertyDrawer(typeof(DamageRangeDictionary))]
public class DamageRangeDictionaryDrawer : EnumFloatDictionaryDrawer { }