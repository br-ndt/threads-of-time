using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised that carries an integer.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Integer Game Event")]
    public class IntegerGameEvent : GameEvent<int> { }
}