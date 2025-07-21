using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Configs
{
    /// <summary>
    /// Configuration for a specific battle.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Configs/Battle Config")]
    public class BattleConfig : SceneActionConfig
    {
        [Header("Heroes")]
        public List<HeroConfig> staticHeroes = new();
        [Header("Enemies")]
        public List<EnemyConfig> enemies = new();


        [Header("Environment")]
        public string battleBackgroundSceneName; // Name of the scene for the battle background
        public string battleMusicID; // ID for battle music

        [Header("Rewards")]
        // placeholder
        public int goldReward;
        public List<string> itemRewards = new List<string>();
    }
}