using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DamageFloatDictionary))]
public class DamageFloatDictionaryDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var keys = property.FindPropertyRelative("keys");
        int lines = 2 + keys.arraySize + 1; // label + defaultValue + each entry + add button
        return lines * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Begin
        EditorGUI.BeginProperty(position, label, property);
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float y = position.y;

        // Label header
        Rect labelRect = new(position.x, y, position.width, lineHeight);
        property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);
        y += lineHeight + spacing;

        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        // Get properties
        var defaultValueProp = property.FindPropertyRelative("defaultValue");
        var keysProp = property.FindPropertyRelative("keys");
        var valuesProp = property.FindPropertyRelative("values");

        // Draw defaultValue
        Rect defaultValueRect = new(position.x, y, position.width, lineHeight);
        EditorGUI.PropertyField(defaultValueRect, defaultValueProp);
        y += lineHeight + spacing;

        // Draw each key-value pair
        for (int i = 0; i < keysProp.arraySize; i++)
        {
            var keyProp = keysProp.GetArrayElementAtIndex(i);
            var valueProp = valuesProp.GetArrayElementAtIndex(i);

            Rect keyRect = new(position.x, y, position.width * 0.4f, lineHeight);
            Rect valRect = new(position.x + position.width * 0.45f, y, position.width * 0.4f, lineHeight);
            Rect removeRect = new(position.x + position.width * 0.9f, y, 20, lineHeight);

            EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);
            EditorGUI.PropertyField(valRect, valueProp, GUIContent.none);

            if (GUI.Button(removeRect, "X"))
            {
                keysProp.DeleteArrayElementAtIndex(i);
                valuesProp.DeleteArrayElementAtIndex(i);
                break; // Prevent layout issues during GUI loop
            }

            y += lineHeight + spacing;
        }

        // Add entry button
        Rect buttonRect = new(position.x, y, position.width, lineHeight);
        if (GUI.Button(buttonRect, "Add Entry"))
        {
            keysProp.arraySize++;
            valuesProp.arraySize++;
        }

        // End
        EditorGUI.EndProperty();
    }
}
