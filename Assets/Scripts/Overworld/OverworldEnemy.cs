using Assets.Scripts.Configs;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Overworld
{
    public class OverworldEnemy : MonoBehaviour
    {
        [SerializeField] private SceneActionConfig _actionConfig;
        [SerializeField] private TriggerConfig _triggerConfig;

        public SceneActionConfig ActionConfig => _actionConfig;
        public TriggerConfig TriggerConfig => _triggerConfig;
    }
}