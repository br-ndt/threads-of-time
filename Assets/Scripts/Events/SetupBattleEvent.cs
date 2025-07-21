using UnityEngine;
using Assets.Scripts.Combat; // For SetupBattleContext

namespace Assets.Scripts.Events
{
    /// <summary>
    /// ScriptableObject event for combat calculations.
    /// Carries a SetupBattleContext which listeners will modify.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Set Up Battle Event")]
    public class SetupBattleEvent : GameEvent<SetupBattleContext> { }

}