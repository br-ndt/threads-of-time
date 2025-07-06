using UnityEngine;
using System;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Base ScriptableObject for events that carry a payload of type T.
    /// </summary>
    /// <typeparam name="T">The type of the payload this event will carry.</typeparam>
    public abstract class GameEvent<T> : ScriptableObject
    {
        // The actual C# event that other scripts will subscribe/unsubscribe to.
        // Using 'Action<T>' simplifies the delegate signature.
        public event Action<T> OnEventRaised;

        /// <summary>
        /// Raises the event, invoking all registered listeners with the provided payload.
        /// </summary>
        /// <param name="value">The payload to pass to the listeners.</param>
        public void Raise(T value)
        {
            // The '?' (null-conditional operator) ensures we don't try to invoke
            // if there are no listeners subscribed.
            OnEventRaised?.Invoke(value);
        }

        // Optional: Methods for manual subscription/unsubscription if preferred over direct '+=' '/-='
        // However, direct '+=' and '-=' are generally fine and more common.
        public void AddListener(Action<T> listener)
        {
            OnEventRaised += listener;
        }

        public void RemoveListener(Action<T> listener)
        {
            OnEventRaised -= listener;
        }
    }
}