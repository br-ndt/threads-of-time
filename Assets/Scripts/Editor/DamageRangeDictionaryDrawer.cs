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
public class DamageRangeDictionaryDrawer : PropertyDrawer
{
    private const float AddButtonHeight = 20f;
    private const float Spacing = 5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Type keyType = typeof(DamageType); // We can be specific here
        string[] allEnumNames = Enum.GetNames(keyType);
        int[] allEnumValues = (int[])Enum.GetValues(keyType);

        SerializedProperty keysProperty = property.FindPropertyRelative("keys");
        SerializedProperty valuesProperty = property.FindPropertyRelative("values");

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label, true);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            float currentY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            int? indexToRemove = null;

            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                Rect itemRect = new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight);
                Rect keyRect = new Rect(itemRect.x, itemRect.y, itemRect.width * 0.4f, itemRect.height);
                Rect valueRect = new Rect(itemRect.x + itemRect.width * 0.4f + 5, itemRect.y, itemRect.width * 0.6f - 35, itemRect.height);
                Rect removeButtonRect = new Rect(itemRect.x + itemRect.width - 25, itemRect.y, 25, itemRect.height);

                SerializedProperty keyElement = keysProperty.GetArrayElementAtIndex(i);
                SerializedProperty valueElement = valuesProperty.GetArrayElementAtIndex(i);

                // Draw the filtered enum dropdown for the key
                DrawFilteredEnumPopup(keyRect, keyElement, keysProperty, i, keyType, allEnumNames, allEnumValues);

                // Draw the value field. This will automatically use our new FloatRangeDrawer!
                EditorGUI.PropertyField(valueRect, valueElement, GUIContent.none);

                if (GUI.Button(removeButtonRect, "-"))
                {
                    indexToRemove = i;
                }

                currentY += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (indexToRemove.HasValue)
            {
                keysProperty.DeleteArrayElementAtIndex(indexToRemove.Value);
                valuesProperty.DeleteArrayElementAtIndex(indexToRemove.Value);
            }

            Rect addButtonRect = new Rect(position.x + (position.width - 100) / 2, currentY + Spacing, 100, AddButtonHeight);
            bool allKeysUsed = keysProperty.arraySize >= allEnumNames.Length;

            EditorGUI.BeginDisabledGroup(allKeysUsed);
            if (GUI.Button(addButtonRect, "Add Entry"))
            {
                AddNewEntry(keysProperty, valuesProperty, allEnumValues);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    private void DrawFilteredEnumPopup(Rect position, SerializedProperty keyProperty, SerializedProperty allKeysProperty, int currentIndex, Type keyType, string[] allNames, int[] allValues)
    {
        int currentEnumValue = keyProperty.enumValueIndex;
        var usedKeys = new HashSet<int>();
        for (int i = 0; i < allKeysProperty.arraySize; i++)
        {
            if (i != currentIndex) usedKeys.Add(allKeysProperty.GetArrayElementAtIndex(i).enumValueIndex);
        }

        var availableNames = new List<string>();
        var availableValues = new List<int>();
        for (int i = 0; i < allValues.Length; i++)
        {
            if (!usedKeys.Contains(i) || i == currentEnumValue)
            {
                availableNames.Add(allNames[i]);
                availableValues.Add(allValues[i]);
            }
        }

        int selectedIndexInPopup = availableValues.IndexOf(currentEnumValue);
        if (selectedIndexInPopup == -1)
        {
            string currentName = Enum.GetName(keyType, currentEnumValue) ?? "INVALID";
            availableNames.Insert(0, $"{currentName} (Duplicate)");
            availableValues.Insert(0, currentEnumValue);
            selectedIndexInPopup = 0;
        }

        int newSelectedIndexInPopup = EditorGUI.Popup(position, selectedIndexInPopup, availableNames.ToArray());
        keyProperty.enumValueIndex = availableValues[newSelectedIndexInPopup];
    }

    private void AddNewEntry(SerializedProperty keys, SerializedProperty values, int[] allEnumValues)
    {
        var usedKeys = new HashSet<int>();
        for (int i = 0; i < keys.arraySize; i++)
        {
            usedKeys.Add(keys.GetArrayElementAtIndex(i).enumValueIndex);
        }

        int firstAvailableKey = allEnumValues.FirstOrDefault(val => !usedKeys.Contains(val));

        if (usedKeys.Count < allEnumValues.Length)
        {
            keys.arraySize++;
            values.arraySize++;
            keys.GetArrayElementAtIndex(keys.arraySize - 1).enumValueIndex = firstAvailableKey;

            // Set a default FloatRange value
            SerializedProperty newRange = values.GetArrayElementAtIndex(values.arraySize - 1);
            newRange.FindPropertyRelative("min").floatValue = 10f;
            newRange.FindPropertyRelative("max").floatValue = 20f;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            SerializedProperty keysProperty = property.FindPropertyRelative("keys");
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
                   (keysProperty.arraySize * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)) +
                   AddButtonHeight + Spacing * 2;
        }
        return EditorGUIUtility.singleLineHeight;
    }
}