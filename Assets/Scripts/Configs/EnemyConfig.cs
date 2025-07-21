using UnityEngine;

namespace Assets.Scripts.Configs
{
    /// <summary>
    /// Configuration for a specific enemy.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Configs/Enemy Config")]
    public class EnemyConfig : ActorConfig
    {
        public int experienceValue;
    }
}