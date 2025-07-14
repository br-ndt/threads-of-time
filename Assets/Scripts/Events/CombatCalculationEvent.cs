using UnityEngine;
using Assets.Scripts.Combat; // For CombatCalculationContext

namespace Assets.Scripts.Events
{
    /// <summary>
    /// ScriptableObject event for combat calculations.
    /// Carries a CombatCalculationContext which listeners will modify.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Combat Calculation Event")]
    public class CombatCalculationEvent : GameEvent<CombatCalculationContext> { }

}