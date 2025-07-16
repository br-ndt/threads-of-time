using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer(typeof(AllowedTypesAttribute))]
public class AllowedTypesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ObjectReference)
        {
            EditorGUI.LabelField(position, label.text, "AllowedTypes attribute can only be used on reference types.");
            return;
        }

        var allowedTypesAttribute = (AllowedTypesAttribute)attribute;

        if (property.objectReferenceValue != null)
        {
            var objectType = property.objectReferenceValue.GetType();
            if (!allowedTypesAttribute.Types.Any(t => t.IsAssignableFrom(objectType)))
            {
                property.objectReferenceValue = null;
                Debug.LogWarning($"Assigned object of type '{objectType.Name}' is not allowed. Only objects of type {string.Join(", ", allowedTypesAttribute.Types.Select(t => t.Name))} are permitted.");
            }
        }

        EditorGUI.ObjectField(position, property, label);
    }
}