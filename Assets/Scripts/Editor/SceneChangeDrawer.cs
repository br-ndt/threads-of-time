using UnityEngine;
using UnityEditor;
using System;
using Assets.Scripts.States;
using Assets.Scripts.Configs;

[CustomPropertyDrawer(typeof(SceneChange))]
public class SceneChangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            var newStateProp = property.FindPropertyRelative("newState");
            var configProp = property.FindPropertyRelative("newStateConfig"); 
            
            var stateRect = new Rect(position.x, foldoutRect.yMax + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(stateRect, newStateProp);
            
            GameState currentState = (GameState)newStateProp.enumValueIndex;

            if (StateNeedsConfig(currentState))
            {
                var configRect = new Rect(position.x, stateRect.yMax + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(configRect, configProp);
            }
            
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
        {
            return CalculateHeightForLines(1);
        }

        var newStateProp = property.FindPropertyRelative("newState");
        GameState currentState = (GameState)newStateProp.enumValueIndex;

        if (StateNeedsConfig(currentState))
        {
            return CalculateHeightForLines(3);
        }
        else
        {
            return CalculateHeightForLines(2);
        }
    }

    /// <summary>
    /// Helper method to define which states have a config.
    /// Both OnGUI and GetPropertyHeight use this to stay in sync.
    /// </summary>
    private bool StateNeedsConfig(GameState state)
    {
        switch (state)
        {
            case GameState.Battle:
            case GameState.Cutscene:
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Helper method to calculate total height for a given number of lines.
    /// </summary>
    private float CalculateHeightForLines(int lineCount)
    {
        return (EditorGUIUtility.singleLineHeight * lineCount) + (EditorGUIUtility.standardVerticalSpacing * (lineCount - 1));
    }
}