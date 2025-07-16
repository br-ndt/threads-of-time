using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.States;
using System;

namespace Assets.Scripts.Configs
{
    [Serializable]
    public struct ConversationSpeaker
    {
        [SerializeField] string _name;
        [SerializeField] List<string> _messages;
        [SerializeField] Sprite _sprite;
        public readonly string Name => _name;
        public readonly List<string> Messages => _messages;
        public readonly Sprite Sprite => _sprite;

        public ConversationSpeaker(string name, List<string> messages, Sprite sprite)
        {
            _name = name;
            _messages = messages;
            _sprite = sprite;
        }
    }

    /// <summary>
    /// Configuration for a specific conversation.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Configs/Conversation Config")]
    public class ConversationConfig : SceneActionConfig
    {
        public bool skippable;
        public List<ConversationSpeaker> speakers;
    }
}