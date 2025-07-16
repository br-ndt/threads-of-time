using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised whenever a system wants to check the activated triggers.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Record Trigger Event")]
    public class RecordTriggerEvent : GameEvent<(List<TriggerEvent>, bool)> { }
}