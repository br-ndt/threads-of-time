using UnityEngine;
using Assets.Scripts.Combat;
using Assets.Scripts.Configs;
using System.Collections; // For IBattleActor

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Component representing an AI-controlled enemy actor in battle.
    /// </summary>
    public class EnemyBattleActor : BattleActor<EnemyConfig>
    {
        public int experienceValue;

        /// <summary>
        /// Initializes the enemy with data from an EnemyConfigSO.
        /// This should be called immediately after instantiation.
        /// </summary>
        /// <param name="config">The EnemyConfigSO to use for this enemy.</param>
        public new void Initialize(EnemyConfig config)
        {
            base.Initialize(config);
        }

        // This method will be called by BattleManager to get AI's chosen action
        public PlayerAction ChooseAIAction(IBattleActor playerTarget)
        {
            // Simple AI: always attack the provided player target with its default attack
            if (_actorConfig != null && _actorConfig.attacks[0] != null)
            {
                // if (spriteCharacter != null) spriteCharacter.Play(States.BattleSpriteState.Attack);
                return new PlayerAction(_actorConfig.attacks[0], playerTarget.GameObject);
            }
            else
            {
                Debug.LogWarning($"{DisplayName} has no default attack defined in its config! Cannot choose action.");
                return new PlayerAction(PlayerAction.PlayerActionType.Defend); // Fallback
            }
        }

    }
}