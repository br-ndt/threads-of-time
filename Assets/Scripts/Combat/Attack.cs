using UnityEngine;
using Assets.Scripts.Events;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Used for cataloguing an Entity's offensive modifiers, and then applying them to a mutable CombatCalculationContext
    /// </summary>
    public class Attack : MonoBehaviour
    {
        [Header("Event Channel")]
        [SerializeField] private CombatCalculationEvent combatCalculationEvent;

        [Header("Stats")]
        [SerializeField] private DamageRangeDictionary rangeDamageModifiers;
        [SerializeField] private DamageFloatDictionary flatDamageModifiers;
        [SerializeField] private DamageFloatDictionary multDamageModifiers;
        public float overallDamageModifier = 1f; // +1 overall damage after resistances
        public float overallDamageMultiplier = 0.1f; // +10% overall damage
        public float criticalChanceBonus = 0.05f; // +5%

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
                foreach (KeyValuePair<DamageType, FloatRange> rangeEntry in rangeDamageModifiers)
                {
                    Debug.Log($"{rangeEntry.Key} Damage Range: {rangeEntry.Value.min:F2}-{rangeEntry.Value.max:F2}");
                    if (context.Definition.baseDamageRangeByType.Keys.Contains(rangeEntry.Key))
                    {
                        context.Definition.baseDamageRangeByType[rangeEntry.Key] += rangeEntry.Value;
                    }
                    else
                    {
                        context.Definition.baseDamageRangeByType[rangeEntry.Key] = rangeEntry.Value;
                    }
                }
                foreach (KeyValuePair<DamageType, float> modifierEntry in flatDamageModifiers)
                {
                    Debug.Log($"{modifierEntry.Key} Damage: {modifierEntry.Value:F2}");
                    if (context.Definition.baseDamageModifierByType.Keys.Contains(modifierEntry.Key))
                    {
                        context.Definition.baseDamageModifierByType[modifierEntry.Key] += modifierEntry.Value;
                    }
                    else
                    {
                        context.Definition.baseDamageModifierByType[modifierEntry.Key] = modifierEntry.Value;
                    }
                }
                foreach (KeyValuePair<DamageType, float> multiplierEntry in multDamageModifiers)
                {
                    Debug.Log($"{multiplierEntry.Key} Damage Multiplier: {multiplierEntry.Value:F2}");
                    if (context.Definition.damageMultiplierByType.Keys.Contains(multiplierEntry.Key))
                    {
                        context.Definition.damageMultiplierByType[multiplierEntry.Key] += multiplierEntry.Value;
                    }
                    else
                    {
                        context.Definition.damageMultiplierByType[multiplierEntry.Key] = 1 + multiplierEntry.Value;
                    }
                }
                context.Definition.critChanceBonus += criticalChanceBonus;
                context.Definition.overallDamageModifier += overallDamageModifier;
                context.Definition.overallDamageMultiplier += overallDamageMultiplier;
            }
        }
    }
}