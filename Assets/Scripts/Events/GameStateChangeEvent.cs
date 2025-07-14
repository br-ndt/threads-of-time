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
    public class GameStateChangeEvent : GameEvent<(GameState, GameConfig)> { }
}