using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.Scripts.Events;

namespace Assets.Scripts.Configs
{
    [Serializable]
    public class ConversationResponse
    {
        [SerializeField] string _content;
        public string Content => _content;
        [SerializeField] TriggerEvent _triggerIfChosen;
        public TriggerEvent TriggerIfChosen => _triggerIfChosen;
        [SerializeField] ConversationConfig _convoIfChosen;
        public ConversationConfig ConvoIfChosen => _convoIfChosen;
    }

    [Serializable]
    public class ConversationMessage
    {
        [SerializeField] string _content;
        public string Content => _content;
    }

    [Serializable]
    public class ConversationResponses : ConversationMessage
    {
        [SerializeField] List<ConversationResponse> _responses;
        public List<ConversationResponse> Responses => _responses;
    }

    [Serializable]
    public struct ConversationSpeaker
    {
        [SerializeField] string _name;
        [SerializeReference][SerializeField] List<ConversationMessage> _messages;
        [SerializeField] Sprite _sprite;
        public readonly string Name => _name;
        public readonly List<ConversationMessage> Messages => _messages;
        public readonly Sprite Sprite => _sprite;

        public ConversationSpeaker(string name, List<ConversationMessage> messages, Sprite sprite)
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