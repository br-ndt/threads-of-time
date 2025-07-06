using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Defines the static properties of a specific attack.
    /// </summary>
    [CreateAssetMenu(menuName = "Combat/Attack Definition")]
    public class AttackDefinition : ScriptableObject
    {
        [Header("Basic Properties")]
        public string attackName = "New Attack";

        [Header("Damage Distribution")]
        // This will now show up in the Inspector as a list of DamageType-Float pairs
        public DamageFloatDictionary baseDamageByType = new();
        public DamageFloatDictionary damageRangeByType = new();
        public DamageFloatDictionary damageModifierByType = new();
        public float overallDamageModifier = 1f;
        public float overallResistanceMultiplier = 0f;
        public float overallResistanceValue = 0f;

        [Header("Chances")]
        [Range(0f, 1f)]
        public float baseCritChance = 0.05f; // 5%
        public float critChanceBonus = 0.00f; // 0%;
        public float dodgeChance = 0.05f; // 5%
    }
}