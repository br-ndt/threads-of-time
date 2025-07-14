using Assets.Scripts.Configs;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteCharacter2D))]
public class SpriteCharacter2DEditor : Editor
{
    private ActorConfig configToLoad;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpriteCharacter2D character = (SpriteCharacter2D)target;

        GUILayout.Space(10);
        GUILayout.Label("Animation Controls", EditorStyles.boldLabel);

        // Load from Config
        configToLoad = (ActorConfig)EditorGUILayout.ObjectField(
            "Config To Load",
            configToLoad,
            typeof(ActorConfig),
            false);

        if (GUILayout.Button("Load From Config") && configToLoad != null)
        {
            Undo.RecordObject(character, "Load From Config");
            character.LoadFromConfig(configToLoad);
            EditorUtility.SetDirty(character);
        }

        // Flip toggle
        EditorGUI.BeginChangeCheck();
        bool newFlipped = EditorGUILayout.Toggle("Flip?", character.isFlipped);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(character, "Toggle Flip");
            character.isFlipped = newFlipped;
            character.UpdateFrame();
        }

        GUILayout.Space(10);
        GUILayout.Label("Play Available States", EditorStyles.boldLabel);

        // Dynamically list only the states this character actually has
        if (character != null && character.GetAvailableStates() != null)
        {
            foreach (var state in character.GetAvailableStates())
            {
                if (GUILayout.Button($"Play {state}"))
                {
                    Undo.RecordObject(character, $"Play {state}");
                    character.Play(state);
                    EditorUtility.SetDirty(character);
                }
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Reset"))
        {
            Undo.RecordObject(character, "Reset Character");
            character.ResetCharacter();
            EditorUtility.SetDirty(character);
        }
    }
}