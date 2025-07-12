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
        [Tooltip("The damage range (or dice roll) of this attack, keyed by damage type.")]
        public DamageRangeDictionary baseDamageRangeByType = new();
        [Tooltip("The flat damage applied by this attack, keyed by damage type.")]
        public DamageFloatDictionary baseDamageModifierByType = new();
        [Tooltip("The damage multiplier applied to the flat damage of the same damage type.")]
        public DamageFloatDictionary damageMultiplierByType = new();
        [Tooltip("The flat damage added to the sum of all damage types, after type-specific resistance. Should be 0 for most cases.")]
        public float overallDamageModifier = 0f;
        [Tooltip("The damage multiplier applied to the sum of all damage types, after type-specific resistance. Should be 1 for most cases.")]
        public float overallDamageMultiplier = 1f;
        [Tooltip("The flat damage resistance, which subtracts from the sum of all damage types, after type-specific resistance. Should be 0 for most cases.")]
        public float overallResistanceModifier = 0f;
        [Tooltip("The damage resistance multiplier applied to the sum of all damage types, after resistance. Because this works additively with the overallDamageMultiplier, it should be 0 for most cases.")]
        public float overallResistanceMultiplier = 0f;

        [Header("Chances")]
        [Range(0f, 1f)]
        [Tooltip("The base crit chance of the ability. Should be 0 for most cases, but can be used for auto-crit or auto-fail-crit.")]
        // TODO(tbrandt): remove this and make it a global? use a bool for auto crit?
        public float baseCritChance = 0.0f; // 0%
        [Tooltip("The crit chance bonus conferred by the ability.")]
        public float critChanceBonus = 0.00f; // 0%;
        [Tooltip("The base dodge chance of this ability. Usually 0 and reliant on defender's dodge, but can be used to apply lack of accuracy.")]
        public float dodgeChance = 0.05f; // 5%
    }
}