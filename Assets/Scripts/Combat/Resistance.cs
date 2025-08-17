using UnityEngine;
using Assets.Scripts.Events;
using System.Collections.Generic;
using System;
using System.Linq;
using Assets.Scripts.States;
using Assets.Scripts.Configs; // For CombatCalculationEvent

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Applies defensive modifiers to an Entity.
    /// </summary>
    public class Resistance : MonoBehaviour
    {
        [Header("Event Channel")]
        [SerializeField] private CombatCalculationEvent combatCalculationEvent; // Assign the CombatCalculationEvent

        [Header("Stats")]
        [SerializeField] private DamageFloatDictionary flatResistanceModifiers;
        [SerializeField] private DamageFloatDictionary multResistanceModifiers;
        [SerializeField] private ConditionBooleanDictionary conditionImmunities;
        [SerializeField] private ConditionFloatDictionary conditionResistanceModifiers;
        [SerializeField] private float overallResistanceValue = 1f; // -1 damage after type-specific resistances
        [SerializeField] private float overallResistanceMultiplier = 0.1f; // -10% damage
        [SerializeField] private float dodgeChanceModifier = 0.05f;
        private bool initialized = false;

        public void Initialize(DamageFloatDictionary flatResistances, DamageFloatDictionary resistanceMultipliers, ConditionBooleanDictionary conImmunities, ConditionFloatDictionary conditionResistances)
        {
            if (!initialized)
            {
                flatResistanceModifiers = flatResistances;
                multResistanceModifiers = resistanceMultipliers;
                conditionImmunities = conImmunities;
                conditionResistanceModifiers = conditionResistances;
                initialized = true;
            }
        }

        void OnEnable()
        {
            if (combatCalculationEvent != null)
            {
                combatCalculationEvent.OnEventRaised += ApplyDefensiveMods;
            }
        }

        void OnDisable()
        {
            if (combatCalculationEvent != null)
            {
                combatCalculationEvent.OnEventRaised -= ApplyDefensiveMods;
            }
        }

        private void ApplyDefensiveMods(CombatCalculationContext context)
        {
            // Ensure this applies only if THIS character is the defender
            if (context.Defender != null && context.Defender.GameObject == gameObject)
            {
                Debug.Log($"{gameObject.name}: Applying defensive modifiers.");
                foreach (KeyValuePair<DamageType, float> modifierEntry in flatResistanceModifiers)
                {
                    Debug.Log($"{modifierEntry.Key} Flat Resistance: {modifierEntry.Value:F2}");
                    if (context.Definition.baseDamageModifierByType.Keys.Contains(modifierEntry.Key))
                    {
                        context.Definition.baseDamageModifierByType[modifierEntry.Key] -= modifierEntry.Value;
                    }
                    else if (context.Definition.baseDamageRangeByType.Keys.Contains(modifierEntry.Key))
                    {
                        context.Definition.baseDamageModifierByType[modifierEntry.Key] = -modifierEntry.Value;
                    }
                    // we reach here if the resistance type in question doesn't apply to the attack
                }
                foreach (KeyValuePair<DamageType, float> modifierEntry in multResistanceModifiers)
                {
                    Debug.Log($"{modifierEntry.Key} Mult Resistance: {modifierEntry.Value * 100}%");
                    if (context.Definition.damageMultiplierByType.Keys.Contains(modifierEntry.Key))
                    {
                        context.Definition.damageMultiplierByType[modifierEntry.Key] -= modifierEntry.Value;
                    }
                    else
                    {
                        context.Definition.damageMultiplierByType[modifierEntry.Key] = 1 - modifierEntry.Value;
                    }
                }
                context.Definition.dodgeChance += dodgeChanceModifier;
                context.Definition.overallResistanceModifier += overallResistanceValue;
                context.Definition.overallResistanceMultiplier += overallResistanceMultiplier;
                foreach (KeyValuePair<Condition, float> resistanceEntry in conditionResistanceModifiers)
                {
                    Debug.Log($"{resistanceEntry.Key} Mult Resistance: {resistanceEntry.Value * 100}%");
                    if (context.Definition.conditionStats.Keys.Contains(resistanceEntry.Key))
                    {
                        context.Definition.conditionStats[resistanceEntry.Key].Chance -= resistanceEntry.Value;
                    }
                    else
                    {
                        context.Definition.conditionStats[resistanceEntry.Key].Chance = 0 - resistanceEntry.Value;
                    }
                }
                foreach (KeyValuePair<Condition, bool> immunityEntry in conditionImmunities)
                {
                    if (context.Definition.conditionStats.Keys.Contains(immunityEntry.Key))
                    {
                        Debug.Log($"{immunityEntry.Key} Immunity!");
                        context.Definition.conditionStats[immunityEntry.Key].Chance = 0;
                    }
                }
            }
        }
    }
}