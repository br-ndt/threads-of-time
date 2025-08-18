// File: Editor/ConditionStatsDictionaryDrawer.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Assets.Scripts.Configs;

[CustomPropertyDrawer(typeof(ConditionStatsDictionary))]
public class ConditionStatsDictionaryDrawer : PropertyDrawer
{
    private const float AddButtonHeight = 20f;
    private const float Spacing = 5f;
    private const float RemoveButtonWidth = 25f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

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
                Rect lineRect = new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight);

                float fieldWidth = 150f; // Width for Turns and Chance fields
                Rect removeRect = new Rect(lineRect.xMax - RemoveButtonWidth, lineRect.y, RemoveButtonWidth, lineRect.height);
                Rect chanceRect = new Rect(removeRect.x - fieldWidth, lineRect.y, fieldWidth, lineRect.height);
                Rect turnsRect = new Rect(chanceRect.x - fieldWidth, lineRect.y, fieldWidth, lineRect.height);
                Rect keyRect = new Rect(lineRect.x, lineRect.y, turnsRect.x - lineRect.x - Spacing, lineRect.height);

                SerializedProperty keyElement = keysProperty.GetArrayElementAtIndex(i);
                SerializedProperty valueElement = valuesProperty.GetArrayElementAtIndex(i);

                SerializedProperty turnsProp = valueElement.FindPropertyRelative("_turns");
                SerializedProperty chanceProp = valueElement.FindPropertyRelative("_chance");

                DrawFilteredEnumPopup(keyRect, keyElement, keysProperty, i, keyType, allEnumNames, allEnumValues);

                EditorGUI.LabelField(new Rect(turnsRect.x, turnsRect.y, 75, turnsRect.height), "Turns:");
                EditorGUI.PropertyField(new Rect(turnsRect.x + 75, turnsRect.y, turnsRect.width - 75, turnsRect.height), turnsProp, GUIContent.none);

                EditorGUI.LabelField(new Rect(chanceRect.x, chanceRect.y, 75, chanceRect.height), "Chance:");
                EditorGUI.PropertyField(new Rect(chanceRect.x + 75, chanceRect.y, chanceRect.width - 75, chanceRect.height), chanceProp, GUIContent.none);

                if (GUI.Button(removeRect, "-"))
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
            if (i != currentIndex)
            {
                usedKeys.Add(allKeysProperty.GetArrayElementAtIndex(i).enumValueIndex);
            }
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

            var newValue = values.GetArrayElementAtIndex(values.arraySize - 1);
            newValue.FindPropertyRelative("_turns").intValue = 1;
            newValue.FindPropertyRelative("_chance").floatValue = 100f;
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