using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class StartFromBootstrap
{
    private static readonly string pathToBootstrap = "Assets/Scenes/Bootstrap.unity";

    static StartFromBootstrap()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().path);

            if (SceneManager.GetActiveScene().path != pathToBootstrap)
            {
                EditorSceneManager.OpenScene(pathToBootstrap);
            }
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            string previousScene = EditorPrefs.GetString("PreviousScene", string.Empty);
            if (!string.IsNullOrEmpty(previousScene))
            {
                EditorSceneManager.OpenScene(previousScene);
                EditorPrefs.DeleteKey("PreviousScene");
            }
        }
    }
}