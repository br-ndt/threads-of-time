using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    /// <summary>
    /// Parent class to unify battle, cutscene, and conversation scenes.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Configs/SceneAction Config")]
    public abstract class SceneActionConfig : GameConfig
    {
        public string sceneActionID; // Unique ID
        [Header("Event Triggers")]
        [Tooltip("Optional TriggerEvent to call when the SceneAction is successful")] public TriggerEvent onCompleteTrigger;
        [Tooltip("Optional TriggerEvent to call when the SceneAction is failed")] public TriggerEvent onFailTrigger;
        [Tooltip("Optional SceneChanges to start when the SceneAction is over")] public SceneChange sceneChange;
    }
}