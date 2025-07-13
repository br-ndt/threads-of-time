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
        [Tooltip("For battle and cutscenes(?)")]
        public Sprite avatar;
        // [Tooltip("This should only be the mesh, materials, and textures required for rendering (check Assets/Models).")]
        // public GameObject modelPrefab;
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