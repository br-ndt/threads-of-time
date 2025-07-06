using UnityEngine;
using Assets.Scripts.States;
using Assets.Scripts.Configs;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// ScriptableObject event for announcing changes in the GameState.
    /// Carries the new GameState and an optional GameConfig for state-specific data.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Game State Change Event")]
    public class GameStateChangeEvent : ScriptableObject
    {
        public event System.Action<GameState, GameConfig> OnStateChangeRequested;

        /// <summary>
        /// Requests a game state change.
        /// </summary>
        /// <param name="newState">The GameState to transition to.</param>
        /// <param name="config">Optional GameConfig providing data for the new state.</param>
        public void Raise(GameState newState, GameConfig config = null)
        {
            OnStateChangeRequested?.Invoke(newState, config);
        }
    }
}