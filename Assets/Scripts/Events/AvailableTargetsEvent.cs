using UnityEngine;
using Assets.Scripts.Combat;
using System.Collections.Generic; // For IBattleActor

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised to notify UI about what targets are available for an attack.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Available Targets Event")]
    public class AvailableTargetsEvent : GameEvent<List<IBattleActor>> { }
}