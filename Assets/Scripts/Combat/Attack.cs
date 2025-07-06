using UnityEngine;
using Assets.Scripts.Events;
using System.Collections.Generic; // For CombatCalculationEvent

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Apply an Entity's offensive modifiers to a mutable AttackDefinition.
    /// </summary>
    public class Attack : MonoBehaviour
    {
        [Header("Event Channel")]
        [SerializeField] private CombatCalculationEvent combatCalculationEvent; // Assign the CombatCalculationEvent SO

        [Header("Stats")]
        public float criticalChanceBonus = 0.05f; // +5%
        public float overallDamageMultiplier = 1.1f; // +10% overall damage
        [SerializeField] private DamageFloatDictionary flatDamageModifiers;
        [SerializeField] private DamageFloatDictionary multDamageModifiers;

        void OnEnable()
        {
            if (combatCalculationEvent != null)
            {
                combatCalculationEvent.OnCalculationRequested += ApplyOffensiveMods;
            }
        }

        void OnDisable()
        {
            if (combatCalculationEvent != null)
            {
                combatCalculationEvent.OnCalculationRequested -= ApplyOffensiveMods;
            }
        }

        private void ApplyOffensiveMods(CombatCalculationContext context)
        {
            // Ensure this applies only if THIS character is the attacker
            if (context.Attacker == gameObject)
            {
                Debug.Log($"{gameObject.name}: Applying offensive modifiers.");
                foreach (KeyValuePair<DamageType, float> modifierEntry in flatDamageModifiers)
                {
                    if (context.Definition.damageRangeByType.Keys.Contains(modifierEntry.Key))
                    {
                        context.Definition.damageRangeByType[modifierEntry.Key] += modifierEntry.Value;
                    }
                    context.Definition.baseDamageByType[modifierEntry.Key] += modifierEntry.Value;
                }
                foreach (KeyValuePair<DamageType, float> modifierEntry in multDamageModifiers)
                {
                    context.Definition.damageModifierByType[modifierEntry.Key] += modifierEntry.Value;
                }
                context.Definition.critChanceBonus += criticalChanceBonus;
                context.Definition.overallDamageModifier *= overallDamageMultiplier;
            }
        }
    }
}