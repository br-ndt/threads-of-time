using Assets.Scripts.Configs;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Overworld
{
    public class OverworldEnemy : MonoBehaviour
    {
        [SerializeField] private BattleConfig _battleConfig;
        [SerializeField] private ConversationConfig _conversationConfig;

        public BattleConfig BattleConfig => _battleConfig;
        public ConversationConfig ConversationConfig => _conversationConfig;
    }
}