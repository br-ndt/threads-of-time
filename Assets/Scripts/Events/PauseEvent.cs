using UnityEngine;
using Assets.Scripts.Combat;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised whenever Esc is pressed.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Pause Event")]
    public class PauseEvent : GameEvent<bool> { }
}