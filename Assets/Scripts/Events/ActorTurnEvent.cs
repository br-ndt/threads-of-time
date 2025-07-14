using UnityEngine;
using Assets.Scripts.Combat; // For IBattleActor

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised when a specific actor's turn starts.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Actor Turn Event")]
    public class ActorTurnEvent : GameEvent<(IBattleActor, bool)> { }
}