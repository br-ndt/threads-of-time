using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised when the battle ends (win or loss).
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Battle End Event")]
    public class BattleEndEvent : GameEvent<bool> { }
}