using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Combat;

namespace Assets.Scripts.Configs
{
    /// <summary>
    /// Configuration for a specific enemy.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Configs/Enemy Config")]
    public class EnemyConfig : GameConfig
    {
        public string enemyName = "Enemy";
        public Sprite avatar;
        [Header("Attribs")]
        public List<AttackDefinition> attacks = new();
        [Header("Anims")]
        public List<SpriteBookConfig> animations;
        [Header("Stats")]
        public int maxHealth = 10;
        public int baseSpeed = 10;
        [SerializeField] public DamageFloatDictionary flatDamageModifiers = new();
        [SerializeField] public DamageFloatDictionary damageMultipliers = new();
        [SerializeField] public DamageFloatDictionary flatResistances = new();
        [SerializeField] public DamageFloatDictionary resistanceMultipliers = new();
    }
}