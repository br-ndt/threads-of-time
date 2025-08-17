using UnityEngine;
using Assets.Scripts.Combat;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised whenever an actor takes damage.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Damage Taken Event")]
    public class DamageTakenEvent : GameEvent<(IBattleActor, float)> { }
}