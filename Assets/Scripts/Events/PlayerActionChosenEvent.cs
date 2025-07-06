using UnityEngine;
using Assets.Scripts.Combat; // For PlayerAction

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised by UI when the player has chosen an action.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Player Action Chosen Event")]
    public class PlayerActionChosenEvent : ScriptableObject
    {
        public event System.Action<PlayerAction> OnActionChosen;

        public void Raise(PlayerAction action)
        {
            OnActionChosen?.Invoke(action);
        }
    }
}