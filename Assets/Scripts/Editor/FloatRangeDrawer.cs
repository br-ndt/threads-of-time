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

        // Use PrefixLabel to draw the label and get the remaining rect for controls.
        // This is the standard way and handles indentation correctly.
        Rect controlRect = EditorGUI.PrefixLabel(position, label);

        // Find the min and max properties within the FloatRange struct
        SerializedProperty minProp = property.FindPropertyRelative("min");
        SerializedProperty maxProp = property.FindPropertyRelative("max");

        float minValue = minProp.floatValue;
        float maxValue = maxProp.floatValue;

        // --- Define Layout ---
        // We'll give the number fields a fixed width and the slider will fill the rest.
        const float fieldWidth = 50f;
        const float spacing = 5f;

        // Calculate the Rect for each part of the control
        Rect minFieldRect = new Rect(controlRect.x, controlRect.y, fieldWidth, controlRect.height);
        Rect maxFieldRect = new Rect(controlRect.xMax - fieldWidth, controlRect.y, fieldWidth, controlRect.height);
        Rect sliderRect = new Rect(minFieldRect.xMax + spacing, controlRect.y, controlRect.width - (fieldWidth * 2) - (spacing * 2), controlRect.height);

        // --- Draw Controls ---
        // The order doesn't matter here since the Rects do not overlap.

        // Draw the float fields for precise input
        minValue = EditorGUI.FloatField(minFieldRect, minValue);
        maxValue = EditorGUI.FloatField(maxFieldRect, maxValue);

        // Define a visual range for the slider. For a more advanced version,
        // you could create a custom attribute to define these limits in your code.
        float sliderVisualMin = 0f;
        float sliderVisualMax = 100f;

        // Draw the MinMaxSlider in the space between the float fields
        EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, sliderVisualMin, sliderVisualMax);
        
        // --- Apply Values ---
        // Ensure min is never greater than max after slider or field input
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