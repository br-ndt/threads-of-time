using UnityEngine;
using Assets.Scripts.Combat; // For IBattleActor

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised when a specific actor's turn starts.
    /// Payload is a tuple, of the actor related to the event, and whether their turn is starting.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Actor Turn Event")]
    public class ActorTurnEvent : GameEvent<(IBattleActor, bool)> { }
}