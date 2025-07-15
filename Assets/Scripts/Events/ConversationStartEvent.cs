using Assets.Scripts.Configs;
using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised when a specific actor's turn starts.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Conversation Start Event")]
    public class ConversationStartEvent : GameEvent<ConversationConfig> { }
}