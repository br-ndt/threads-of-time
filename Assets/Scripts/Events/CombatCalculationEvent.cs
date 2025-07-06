using UnityEngine;
using Assets.Scripts.Combat; // For CombatCalculationContext

namespace Assets.Scripts.Events
{
    /// <summary>
    /// ScriptableObject event for combat calculations.
    /// Carries a CombatCalculationContext which listeners will modify.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Combat Calculation Event")]
    public class CombatCalculationEvent : ScriptableObject
    {
        public event System.Action<CombatCalculationContext> OnCalculationRequested;

        /// <summary>
        /// Raises the event, allowing listeners to modify the provided context.
        /// </summary>
        /// <param name="context">The mutable CombatCalculationContext to be processed.</param>
        public void Raise(CombatCalculationContext context)
        {
            OnCalculationRequested?.Invoke(context);
        }
    }
}