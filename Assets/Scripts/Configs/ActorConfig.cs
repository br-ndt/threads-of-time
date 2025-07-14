using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    /// <summary>
    /// Abstract class for any Actor's config
    /// </summary>
    public abstract class ActorConfig : GameConfig
    {
        [SerializeField] public string actorName = "Actor";
        [SerializeField] public Sprite avatar;
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
