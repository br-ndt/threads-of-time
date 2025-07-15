using UnityEngine;
using System;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Base ScriptableObject for events that carry no payload.
    /// </summary>
    public abstract class GameEvent : ScriptableObject
    {
        // The actual C# event that other scripts will subscribe/unsubscribe to.
        // Using 'Action<T>' simplifies the delegate signature.
        public event Action OnEventRaised;

        /// <summary>
        /// Raises the event, invoking all registered listeners with the provided payload.
        /// </summary>
        /// <param name="value">The payload to pass to the listeners.</param>
        public void Raise()
        {
            OnEventRaised?.Invoke();
        }

        // Optional: Methods for manual subscription/unsubscription if preferred over direct '+=' '/-='
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