using UnityEngine;
using Assets.Scripts.Combat;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised whenever an actor's health changes.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Health Change Event")]
    public class HealthChangeEvent : ScriptableObject
    {
        // Action will carry: The actor whose health changed, their current health, their max health
        public event System.Action<IBattleActor, float, float> OnHealthChanged;

        /// <summary>
        /// Raises the health change event.
        /// </summary>
        /// <param name="actor">The IBattleActor whose health changed.</param>
        /// <param name="currentHealth">The actor's new current health.</param>
        /// <param name="maxHealth">The actor's maximum health.</param>
        public void Raise(IBattleActor actor, float currentHealth, float maxHealth)
        {
            OnHealthChanged?.Invoke(actor, currentHealth, maxHealth);
        }
    }
}