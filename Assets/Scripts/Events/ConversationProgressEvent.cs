using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised when a specific actor's turn starts.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Conversation Progress Event")]
    public class ConversationProgressEvent : GameEvent<string> { }
}