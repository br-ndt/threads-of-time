using System.Collections.Generic;
using Assets.Scripts.Triggers;
using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised whenever a system wants to check the activated triggers.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Trigger Check Event")]
    public class TriggerCheckEvent : GameEvent<TriggerCheckContext> { }
}