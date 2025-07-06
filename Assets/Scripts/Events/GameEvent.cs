using UnityEngine;
using System;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Base ScriptableObject for simple events that do not carry a payload.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Void Game Event")] // Directly create concrete void events
    public class GameEvent : ScriptableObject
    {
        // The actual C# event that other scripts will subscribe/unsubscribe to.
        // Using 'Action' for events without parameters.
        public event Action OnEventRaised;

        /// <summary>
        /// Raises the event, invoking all registered listeners.
        /// </summary>
        public void Raise()
        {
            OnEventRaised?.Invoke();
        }

        // Optional: Methods for manual subscription/unsubscription
        public void AddListener(Action listener)
        {
            OnEventRaised += listener;
        }

        public void RemoveListener(Action listener)
        {
            OnEventRaised -= listener;
        }
    }
}