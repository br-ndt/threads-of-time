using UnityEngine;
using Assets.Scripts.Events;
using System.Collections.Generic;
using System;
using System.Linq; // For CombatCalculationEvent

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
        [SerializeField] private float overallResistanceValue = 1f; // -1 damage after type-specific resistances
        [SerializeField] private float overallResistanceMultiplier = 0.1f; // -10% damage
        [SerializeField] private float dodgeChanceModifier = 0.05f;
        private bool initialized = false;

        public void Initialize(DamageFloatDictionary flatResistances, DamageFloatDictionary resistanceMultipliers)
        {
            if (!initialized)
            {
                flatResistanceModifiers = flatResistances;
                multResistanceModifiers = resistanceMultipliers;
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
            if (context.Defender == gameObject)
            {
                Debug.Log($"{gameObject.name}: Applying defensive modifiers.");
                foreach (KeyValuePair<DamageType, float> modifierEntry in flatResistanceModifiers)
                {
                    Debug.Log($"{modifierEntry.Key} Flat Resistance: {modifierEntry.Value:F2}");
                    if (context.Definition.baseDamageModifierByType.Keys.Contains(modifierEntry.Key))
                    {
                        context.Definition.baseDamageModifierByType[modifierEntry.Key] -= modifierEntry.Value;
                    }
                    else
                    {
                        context.Definition.baseDamageModifierByType[modifierEntry.Key] = -modifierEntry.Value;
                    }
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
            }
        }
    }
}