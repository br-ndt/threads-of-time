using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Concrete ScriptableObject event for passing an integer value.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Integer Game Event")]
    public class IntGameEvent : GameEvent<int>
    {
        // No additional code needed here, it inherits all functionality from GameEvent<int>
    }
}