using Assets.Scripts.Configs;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(ConversationMessage))]
    public class ConversationMessageDrawer : PropertyDrawer
    {
        private readonly string[] messageTypeOptions = { "Simple Message", "Message with Responses" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.managedReferenceValue == null)
            {
                property.managedReferenceValue = new ConversationMessage();
            }

            int currentTypeIndex = 0; // Default to "Simple Message"
            if (property.managedReferenceValue is ConversationResponses)
            {
                currentTypeIndex = 1; // "Message with Responses"
            }

            int selectedTypeIndex = EditorGUI.Popup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Message Type", currentTypeIndex, messageTypeOptions);

            if (selectedTypeIndex != currentTypeIndex)
            {
                property.managedReferenceValue = selectedTypeIndex == 0
                    ? new ConversationMessage()
                    : new ConversationResponses();
            }

            EditorGUI.PropertyField(position, property, label, true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }
    }
}