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
        [Header("Stats")]
        public int maxHealth = 10;
        public int baseSpeed = 10;
        [SerializeField] DamageFloatDictionary flatDamageModifiers = new();
        [SerializeField] DamageFloatDictionary damageMultipliers = new();
        [SerializeField] DamageFloatDictionary flatResistances = new();
        [SerializeField] DamageFloatDictionary resistanceMultipliers = new();
    }
}