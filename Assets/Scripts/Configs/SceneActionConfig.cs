using UnityEngine;

namespace Assets.Scripts.Configs
{
    /// <summary>
    /// Parent class to unify battle, cutscene, and conversation scenes.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Configs/SceneAction Config")]
    public abstract class SceneActionConfig : GameConfig
    {
        public string sceneActionID; // Unique ID for this sceneAction
        public SceneChange sceneChange;
    }
}