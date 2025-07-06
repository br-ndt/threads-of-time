using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised when the battle ends (win or loss).
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Battle End Event")]
    public class BattleEndEvent : ScriptableObject
    {
        public event System.Action<bool> OnBattleEnded; // true for win, false for loss

        public void Raise(bool wonBattle)
        {
            OnBattleEnded?.Invoke(wonBattle);
        }
    }
}