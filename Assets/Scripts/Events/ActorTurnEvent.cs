using UnityEngine;
using Assets.Scripts.Combat; // For IBattleActor

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised when a specific actor's turn starts.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Actor Turn Event")]
    public class ActorTurnEvent : ScriptableObject
    {
        public event System.Action<IBattleActor> OnTurnStarted;

        public void Raise(IBattleActor actor)
        {
            OnTurnStarted?.Invoke(actor);
        }
    }
}