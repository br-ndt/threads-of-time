using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Combat;

namespace Assets.Scripts.Configs
{
    /// <summary>
    /// Configuration for a specific hero.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Configs/Hero Config")]
    public class HeroConfig : GameConfig
    {
        public string heroName = "Hero";
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