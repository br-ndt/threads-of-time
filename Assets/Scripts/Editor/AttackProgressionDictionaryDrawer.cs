// File: Editor/AttackProgressionDictionaryDrawer.cs
using UnityEditor;
using UnityEngine;
using Assets.Scripts.Configs;

[CustomPropertyDrawer(typeof(AttackProgressionDictionary))]
public class AttackProgressionDictionaryDrawer : PropertyDrawer
{
    private const float RemoveButtonWidth = 20f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label, true);

        if (property.isExpanded)
        {
            var keysProp = property.FindPropertyRelative("keys");
            var valuesProp = property.FindPropertyRelative("values");

            EditorGUI.indentLevel++;
            float currentY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            for (int i = 0; i < keysProp.arraySize; i++)
            {
                // Calculate the total height for this single dictionary entry
                var valueElement = valuesProp.GetArrayElementAtIndex(i);
                var attacksListProperty = valueElement.FindPropertyRelative("Attacks");
                float entryHeight = EditorGUI.GetPropertyHeight(attacksListProperty, true);

                // Define the Rect for the top line of the entry
                Rect lineRect = new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight);
                
                // --- NEW: Split the line for Key, Value Foldout, and Remove Button ---
                float keyWidth = 80f; // Fixed width for "Level [  #  ]"
                float valueWidth = lineRect.width - keyWidth - RemoveButtonWidth - IndentWidth() - 5f;

                Rect keyRect = new Rect(lineRect.x + IndentWidth(), lineRect.y, keyWidth, lineRect.height);
                Rect valueRect = new Rect(keyRect.xMax, lineRect.y, valueWidth, lineRect.height);
                Rect removeRect = new Rect(valueRect.xMax + 5, lineRect.y, RemoveButtonWidth, lineRect.height);
                
                // Draw the "Level" label and the integer key field
                EditorGUI.LabelField(new Rect(keyRect.x, keyRect.y, 40, keyRect.height), "Level");
                EditorGUI.PropertyField(new Rect(keyRect.x + 40, keyRect.y, keyWidth - 40, keyRect.height), keysProp.GetArrayElementAtIndex(i), GUIContent.none);
                
                // Draw the "Attacks" list. PropertyField will handle the foldout and drawing children.
                EditorGUI.PropertyField(valueRect, attacksListProperty, new GUIContent("Attacks"), true);

                if (GUI.Button(removeRect, "X"))
                {
                    keysProp.DeleteArrayElementAtIndex(i);
                    valuesProp.DeleteArrayElementAtIndex(i);
                    break;
                }
                
                // Advance Y position by the total height of this entry
                currentY += entryHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            // --- Add Button ---
            Rect addButtonRect = new Rect(position.x + (position.width - 50f) / 2, currentY, 50f, 20f);
            if (GUI.Button(addButtonRect, "Add"))
            {
                keysProp.arraySize++;
                valuesProp.arraySize++;
            }

            EditorGUI.indentLevel--;
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = EditorGUIUtility.singleLineHeight;
        if (property.isExpanded)
        {
            totalHeight += EditorGUIUtility.standardVerticalSpacing;

            var valuesProp = property.FindPropertyRelative("values");
            if (valuesProp != null) // Safety check
            {
                for (int i = 0; i < valuesProp.arraySize; i++)
                {
                    var attacksListProperty = valuesProp.GetArrayElementAtIndex(i).FindPropertyRelative("Attacks");
                    totalHeight += EditorGUI.GetPropertyHeight(attacksListProperty, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            
            totalHeight += 20f + EditorGUIUtility.standardVerticalSpacing; // Add button height
        }
        return totalHeight;
    }
    
    private float IndentWidth() => EditorGUI.indentLevel * 15f;
}