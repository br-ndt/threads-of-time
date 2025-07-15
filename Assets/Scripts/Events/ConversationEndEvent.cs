using System;
using Assets.Scripts.Configs;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised when a specific actor's turn starts.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Conversation End Event")]
    public class ConversationEndEvent : GameEvent { }
}