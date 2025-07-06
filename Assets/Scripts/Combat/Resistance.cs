using UnityEngine;
using Assets.Scripts.Events;
using System.Collections.Generic; // For CombatCalculationEvent

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Applies defensive modifiers to an Entity.
    /// </summary>
    public class Resistance : MonoBehaviour
    {
        [Header("Event Channel")]
        [SerializeField] private CombatCalculationEvent combatCalculationEvent; // Assign the CombatCalculationEvent

        [SerializeField] private DamageFloatDictionary flatResistanceModifiers;
        [SerializeField] private DamageFloatDictionary multResistanceModifiers;
        public float overallResistanceMultiplier = 1.0f;
        public float overallResistanceValue = 0f;
        public float dodgeChanceModifier = 0.05f;

        void OnEnable()
        {
            if (combatCalculationEvent != null)
            {
                combatCalculationEvent.OnCalculationRequested += ApplyDefensiveMods;
            }
        }

        void OnDisable()
        {
            if (combatCalculationEvent != null)
            {
                combatCalculationEvent.OnCalculationRequested -= ApplyDefensiveMods;
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
                    context.Definition.baseDamageByType[modifierEntry.Key] -= modifierEntry.Value;
                }
                foreach (KeyValuePair<DamageType, float> modifierEntry in multResistanceModifiers)
                {
                    context.Definition.damageModifierByType[modifierEntry.Key] -= modifierEntry.Value;
                }
                context.Definition.dodgeChance += dodgeChanceModifier;
                context.Definition.overallResistanceValue += overallResistanceValue;
                if (overallResistanceMultiplier != 0) context.Definition.overallResistanceMultiplier *= overallResistanceMultiplier;
            }
        }
    }
}