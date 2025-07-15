using Assets.Scripts.Utility;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom PropertyDrawer for the FloatRange struct.
/// This creates a user-friendly min-max slider in the inspector.
/// </summary>
[CustomPropertyDrawer(typeof(FloatRange))]
public class FloatRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Find the min and max properties within the FloatRange struct
        SerializedProperty minProp = property.FindPropertyRelative("min");
        SerializedProperty maxProp = property.FindPropertyRelative("max");

        float minValue = minProp.floatValue;
        float maxValue = maxProp.floatValue;

        // Define the layout: 40% for labels, 60% for fields/slider
        Rect labelRect = new Rect(position.x, position.y, position.width * 0.4f, position.height);
        Rect fieldsRect = new Rect(position.x + position.width * 0.4f, position.y, position.width * 0.6f, position.height);

        // Draw the main label for the range
        EditorGUI.LabelField(labelRect, label);

        // Define layout for the two float fields
        float fieldWidth = fieldsRect.width / 2 - 5;
        Rect minFieldRect = new Rect(fieldsRect.x, fieldsRect.y, fieldWidth, fieldsRect.height);
        Rect maxFieldRect = new Rect(fieldsRect.x + fieldWidth + 5, fieldsRect.y, fieldWidth, fieldsRect.height);

        // Draw the float fields for precise input
        minValue = EditorGUI.FloatField(minFieldRect, minValue);
        maxValue = EditorGUI.FloatField(maxFieldRect, maxValue);

        // Draw the MinMaxSlider below the float fields
        Rect sliderRect = new Rect(position.x, position.y, position.width, position.height); // Use full width for slider logic

        // Let's define a visual range for the slider, e.g., 0-100.
        // The actual values can go beyond this, but the slider will be capped.
        float sliderVisualMin = 0f;
        float sliderVisualMax = 100f;

        EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, sliderVisualMin, sliderVisualMax);

        // Ensure min is never greater than max
        if (minValue > maxValue)
        {
            minValue = maxValue;
        }

        // Apply the modified values back to the serialized properties
        minProp.floatValue = minValue;
        maxProp.floatValue = maxValue;

        EditorGUI.EndProperty();
    }
}