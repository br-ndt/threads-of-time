using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Concrete ScriptableObject event for simple trigger events (no payload).
    /// This file is technically optional if you put [CreateAssetMenu] on GameEvent.cs directly,
    /// but can be useful for clearer asset type naming in the editor.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Void Game Event")] // Can be renamed if you like
    public class VoidGameEvent : GameEvent
    {
        // No additional code needed here, it inherits all functionality from GameEvent
    }
}