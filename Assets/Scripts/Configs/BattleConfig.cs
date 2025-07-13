using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Configs
{
    /// <summary>
    /// Configuration for a specific battle.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Configs/Battle Config")]
    public class BattleConfig : GameConfig
    {
        [Header("Enemies")]
        public List<EnemyConfig> enemies = new(); // TODO: List of enemy prefab IDs or data IDs

        [Header("Heroes")]
        public List<HeroConfig> heroes = new();

        [Header("Environment")]
        public string battleBackgroundSceneName; // Name of the scene for the battle background
        public string battleMusicID; // ID for battle music

        [Header("Rewards")]
        public int goldReward;
        public List<string> itemRewards = new List<string>(); // Example: Item IDs
    }
}