using UnityEngine;
using UnityEditor;
using Assets.Scripts.Configs;
using Assets.Scripts.States; // Required for all editor scripting.

// This attribute tells Unity to use this class to draw the inspector for CutsceneConfig objects.
[CustomEditor(typeof(CutsceneConfig))]
public class CutsceneConfigEditor : Editor
{
    // These will hold the serialized properties of our CutsceneConfig fields.
    private SerializedProperty gameStateOnEndProp;
    private SerializedProperty nextBattleProp;

    // This function is called when the editor is enabled.
    // It's the best place to find and cache your properties.
    private void OnEnable()
    {
        gameStateOnEndProp = serializedObject.FindProperty("gameStateOnEnd");
        nextBattleProp = serializedObject.FindProperty("nextSceneBattle");
    }

    // This function is called every time Unity draws the inspector.
    public override void OnInspectorGUI()
    {
        // Fetches the latest state of the object being inspected.
        serializedObject.Update();

        // Draw the gameStateOnEnd field.
        EditorGUILayout.PropertyField(gameStateOnEndProp);

        // Check the current enum value. We use property.enumValueIndex to get the selected index.
        GameState selectedState = (GameState)gameStateOnEndProp.enumValueIndex;

        // If the selected state is 'Cutscene', then draw the nextCutscene field.
        if (selectedState == GameState.Battle)
        {
            EditorGUILayout.PropertyField(nextBattleProp);
        }

        // Applies any changes made in the inspector to the actual object.
        // This is a crucial step!
        serializedObject.ApplyModifiedProperties();
    }
}