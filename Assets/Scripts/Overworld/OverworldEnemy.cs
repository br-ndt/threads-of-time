using Assets.Scripts.Configs;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Overworld
{
    public class OverworldEnemy : MonoBehaviour
    {
        [SerializeField] private SceneActionConfig _actionConfig;

        public SceneActionConfig ActionConfig => _actionConfig;
    }
}