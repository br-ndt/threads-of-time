using UnityEngine;
using Assets.Scripts.Combat;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised whenever an actor's health changes.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Health Change Event")]
    public class HealthChangeEvent : GameEvent<(IBattleActor, float, float)> { }
}