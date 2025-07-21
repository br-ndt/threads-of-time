using Assets.Scripts.Configs;
using Assets.Scripts.Events;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Component representing a player-controlled actor in battle.
    /// </summary>
    public class HeroBattleActor : BattleActor<HeroConfig>
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

        /// <summary>
        /// Initializes the hero actor with data from a Hero instance.
        /// This should be called immediately after instantiation.
        /// </summary>
        /// <param name="config">The Hero instance to use for this actor.</param>
        public void Initialize(Hero hero)
        {
            _actorConfig = hero.BaseConfig;
            _health = GetComponent<Health>();
            _resistance = GetComponent<Resistance>();
            _spriteCharacter = GetComponentInChildren<SpriteCharacter2D>();

            if (_spriteCharacter != null)
            {
                _spriteCharacter.LoadFromConfig(_actorConfig);
            }

            // Set base stats from config
            // Some of these still need to be updated for 
            // Level scaling, marked with empty comments
            _actorID = _actorConfig.actorID;
            _displayName = _actorConfig.actorName;
            _avatar = _actorConfig.avatar;
            _attacks = hero.AvailableAttacks;
            _health.Initialize(hero.CurrentHealth, hero.MaxHealth);
            _resistance.Initialize(_actorConfig.flatResistances, _actorConfig.resistanceMultipliers); //
            _currentSpeed = _actorConfig.baseSpeed;

            gameObject.name = _actorConfig.actorID; // Update GameObject name for clarity in hierarchy
            Debug.Log($"Initialized {gameObject.name} with config: {_actorConfig.actorName}");
        }
    }
}