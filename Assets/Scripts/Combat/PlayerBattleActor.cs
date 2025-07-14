using Assets.Scripts.Configs;
using Assets.Scripts.Events;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Component representing a player-controlled actor in battle.
    /// </summary>
    public class PlayerBattleActor : BattleActor<HeroConfig>
    {
        [Header("Event Channels")]
        [SerializeField] private BattleEndEvent battleEndEvent;

        private void Awake()
        {
            _isPlayerControlled = true;
        }

        private new void OnEnable()
        {
            battleEndEvent.OnEventRaised += HandleBattleEnd;
            base.OnEnable();
        }

        private new void OnDisable()
        {
            battleEndEvent.OnEventRaised -= HandleBattleEnd;
            base.OnDisable();
        }

        private void HandleBattleEnd(bool playerWon)
        {
            if (playerWon)
            {
                _spriteCharacter.Play(BattleSpriteState.Run);
                _spriteCharacter.speedMult = 2f;
            }
        }
    }
}