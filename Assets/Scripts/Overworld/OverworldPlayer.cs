using Assets.Scripts.Configs;
using Assets.Scripts.Events;
using Assets.Scripts.States;
using Assets.Scripts.Triggers;
using UnityEngine;

namespace Assets.Scripts.Overworld
{
    public class OverworldPlayer : MonoBehaviour
    {
        [SerializeField] private GameStateChangeEvent requestGameStateChange;
        [SerializeField] private ConversationStartEvent conversationStartEvent;
        [SerializeField] private TriggerCheckEvent triggerCheckEvent;
        [SerializeField] private RecordTriggerEvent recordTriggerEvent;
        void OnTriggerEnter(Collider other)
        {
            OverworldEnemy enemy = other.GetComponent<OverworldEnemy>();
            if (enemy != null && (enemy.ActionConfig != null || enemy.TriggerConfig != null))
            {
                Debug.Log("Player entered overworld trigger!");
                if (enemy.ActionConfig != null)
                {
                    // if it's not a BattleConfig, it is a ConversationConfig. see OverworldEnemy
                    if (enemy.ActionConfig is BattleConfig)
                    {
                        requestGameStateChange.Raise((GameState.Battle, enemy.ActionConfig));
                    }
                    if (enemy.ActionConfig is ConversationConfig)
                    {
                        conversationStartEvent.Raise(enemy.ActionConfig as ConversationConfig);
                    }
                }
                if (enemy.TriggerConfig != null)
                {
                    TriggerCheckContext context = new(enemy.TriggerConfig.requiredTriggers);
                    triggerCheckEvent.Raise(context);
                    if (context.IsValid)
                    {
                        foreach (TriggerEvent trigger in enemy.TriggerConfig.emittedTriggers)
                        {
                            trigger.Raise(true);
                        }
                        recordTriggerEvent.Raise((enemy.TriggerConfig.emittedTriggers, true));
                    }
                }
            }
        }
    }
}