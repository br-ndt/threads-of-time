using UnityEngine;
using Assets.Scripts.Events; // For CombatCalculationEvent
using Assets.Scripts.States;
using Unity.VisualScripting; // For GameState, to ensure we're in Battle state

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Orchestrates combat actions, including damage calculation.
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        [Header("Event Channel")]
        [SerializeField] private CombatCalculationEvent attackCalculationEvent;
        [SerializeField] private CombatCalculationEvent defendCalculationEvent;

        [Header("Debug References")]
        [SerializeField] private GameObject characterA; // Assign Character A in Inspector
        [SerializeField] private GameObject monsterZ;   // Assign Monster Z in Inspector

        /// <summary>
        /// Initiates an attack calculation and applies its effects.
        /// </summary>
        public void PerformAttack(GameObject attacker, GameObject defender, AttackDefinition attackDefinition)
        {
            // 1. Create a new context for this specific calculation
            CombatCalculationContext context = new(
                attacker, defender, Instantiate(attackDefinition)
            );

            // 2. Raise the event, allowing all listeners to modify the context
            attackCalculationEvent.Raise(context);
            defendCalculationEvent.Raise(context);

            // 3. After all listeners have run, calculate the final values
            context.CalculateFinalValues();

            // 4. Use the final calculated values
            Debug.Log($"Attack '{context.Definition.attackName}' by {context.Attacker.name} on {context.Defender.name}:");
            Debug.Log($"Final Damage: {context.FinalDamage:F2}"); // F2 for 2 decimal places

            // 5. Apply effects to health, trigger animations, sound effects, etc.
            defender.GetComponent<Health>().TakeDamage(context.FinalDamage);
            // Example: if (context.ApplyPoison) defender.GetComponent<StatusEffects>().ApplyPoison(context.BasePoisonDamage + context.FlatPoisonDamageBonus);
        }
    }
}