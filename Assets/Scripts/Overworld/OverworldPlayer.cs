using Assets.Scripts.Configs;
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
            if (enemy != null && (enemy.ActionConfig != null))
            {
                Debug.Log("Player entered overworld trigger!");
                if (enemy.ActionConfig != null)
                {
                    // if it's not a BattleConfig, it is a ConversationConfig. see OverworldEnemy
                    if (enemy.ActionConfig is BattleConfig)
                    {
                        gameStateChangeEvent.Raise((GameState.Battle, enemy.ActionConfig));
                    }
                    else
                    {
                        conversationStartEvent.Raise(enemy.ActionConfig as ConversationConfig);
                    }
                }
            }
        }
    }
}