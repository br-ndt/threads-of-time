using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised whenever a significant gameplay or narrative trigger occurs.
    /// One can also be toggled "off" by using the bool field
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Trigger Event")]
    public class TriggerEvent : GameEvent<bool> { }
}