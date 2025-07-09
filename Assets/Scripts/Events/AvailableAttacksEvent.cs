using UnityEngine;
using Assets.Scripts.Combat;
using System.Collections.Generic; // For IBattleActor

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised to notify UI about what targets are available for an attack.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Available Attacks Event")]
    public class AvailableAttacksEvent : ScriptableObject
    {
        public event System.Action<List<AttackDefinition>> OnAttacksAvailable;

        public void Raise(List<AttackDefinition> attacks)
        {
            OnAttacksAvailable?.Invoke(attacks);
        }
    }
}