using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq; // Used for LINQ extension methods
using System; // Required for Enum class
using Assets.Scripts.Combat;

/// <summary>
/// Custom PropertyDrawer for the DamageFloatDictionary class.
/// By targeting the concrete class instead of the generic one, we ensure this drawer is always used.
/// </summary>
[CustomPropertyDrawer(typeof(DamageFloatDictionary))]
public class DamageFloatDictionaryDrawer : PropertyDrawer
{
    private const float AddButtonHeight = 20f;
    private const float Spacing = 5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // --- Get Key Type and Enum Info ---
        // We use reflection to find the enum type from the base class's generic arguments.
        // This makes the drawer reusable for other dictionaries inheriting from SerializableDictionary<Enum, T>.
        Type keyType = fieldInfo.FieldType.BaseType.GetGenericArguments()[0];
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
                Rect keyRect = new Rect(itemRect.x, itemRect.y, itemRect.width * 0.45f - 5, itemRect.height);
                Rect valueRect = new Rect(itemRect.x + itemRect.width * 0.45f, itemRect.y, itemRect.width * 0.45f - 5, itemRect.height);
                Rect removeButtonRect = new Rect(itemRect.x + itemRect.width - 25, itemRect.y, 25, itemRect.height);

                SerializedProperty keyElement = keysProperty.GetArrayElementAtIndex(i);
                SerializedProperty valueElement = valuesProperty.GetArrayElementAtIndex(i);

                // --- Draw Filtered Enum Dropdown ---
                DrawFilteredEnumPopup(keyRect, keyElement, keysProperty, i, keyType, allEnumNames, allEnumValues);
                
                // Draw the value field
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

            // --- Draw 'Add' Button (Disabled if all keys are used) ---
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

    /// <summary>
    /// Draws a custom popup field that only shows enum values not already used as keys.
    /// </summary>
    private void DrawFilteredEnumPopup(Rect position, SerializedProperty keyProperty, SerializedProperty allKeysProperty, int currentIndex, Type keyType, string[] allNames, int[] allValues)
    {
        int currentEnumValue = keyProperty.enumValueIndex;

        // Find all keys currently in use, except for the one at the current index.
        var usedKeys = new HashSet<int>();
        for (int i = 0; i < allKeysProperty.arraySize; i++)
        {
            if (i != currentIndex)
            {
                usedKeys.Add(allKeysProperty.GetArrayElementAtIndex(i).enumValueIndex);
            }
        }

        // Build the list of available options for the popup.
        var availableNames = new List<string>();
        var availableValues = new List<int>();
        for (int i = 0; i < allValues.Length; i++)
        {
            // An option is available if it's not used OR it's the one currently selected for this row.
            if (!usedKeys.Contains(i) || i == currentEnumValue)
            {
                availableNames.Add(allNames[i]);
                availableValues.Add(allValues[i]);
            }
        }

        int selectedIndexInPopup = availableValues.IndexOf(currentEnumValue);
        
        // If the current value is somehow a duplicate from before this system was active, handle it gracefully.
        if (selectedIndexInPopup == -1)
        {
            string currentName = Enum.GetName(keyType, currentEnumValue) ?? "INVALID";
            availableNames.Insert(0, $"{currentName} (Duplicate)");
            availableValues.Insert(0, currentEnumValue);
            selectedIndexInPopup = 0;
        }

        int newSelectedIndexInPopup = EditorGUI.Popup(position, selectedIndexInPopup, availableNames.ToArray());

        // If the user selected a new value, update the property.
        keyProperty.enumValueIndex = availableValues[newSelectedIndexInPopup];
    }

    /// <summary>
    /// Adds a new entry to the dictionary, automatically selecting the first available enum key.
    /// </summary>
    private void AddNewEntry(SerializedProperty keys, SerializedProperty values, int[] allEnumValues)
    {
        var usedKeys = new HashSet<int>();
        for (int i = 0; i < keys.arraySize; i++)
        {
            usedKeys.Add(keys.GetArrayElementAtIndex(i).enumValueIndex);
        }

        // Find the first enum value that is not currently in use.
        int firstAvailableKey = -1;
        foreach (int enumValue in allEnumValues)
        {
            if (!usedKeys.Contains(enumValue))
            {
                firstAvailableKey = enumValue;
                break;
            }
        }

        if (firstAvailableKey != -1)
        {
            keys.arraySize++;
            values.arraySize++;
            keys.GetArrayElementAtIndex(keys.arraySize - 1).enumValueIndex = firstAvailableKey;
            values.GetArrayElementAtIndex(values.arraySize - 1).floatValue = 0f; // Default value
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