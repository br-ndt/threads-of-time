using Assets.Scripts.Events;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Overworld
{
    public class OverworldPlayer : MonoBehaviour
    {
        [SerializeField] private GameStateChangeEvent gameStateChangeEvent;
        [SerializeField] private ConversationStartEvent conversationStartEvent;
        void OnTriggerEnter(Collider other)
        {
            OverworldEnemy enemy = other.GetComponent<OverworldEnemy>();
            if (enemy != null)
            {
                Debug.Log("Player entered overworld trigger!");
                if (enemy.ConversationConfig && conversationStartEvent != null)
                {
                    if (enemy.BattleConfig)
                    {
                        enemy.ConversationConfig.gameStateOnEnd = GameState.Battle;
                        enemy.ConversationConfig.nextSceneBattle = enemy.BattleConfig;
                    }
                    conversationStartEvent.Raise(enemy.ConversationConfig);
                }
                else if (gameStateChangeEvent != null)
                {
                    gameStateChangeEvent.Raise((GameState.Battle, enemy.BattleConfig));
                }
            }
        }
    }
}