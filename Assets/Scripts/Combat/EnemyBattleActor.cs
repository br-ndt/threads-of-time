using UnityEngine;
using Assets.Scripts.Combat;
using Assets.Scripts.Configs;
using System.Collections; // For IBattleActor

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Component representing an AI-controlled enemy actor in battle.
    /// </summary>
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Attack))]
    [RequireComponent(typeof(Resistance))]
    // Add MonsterResistance and other relevant components here
    public class EnemyBattleActor : MonoBehaviour, IBattleActor
    {
        public GameObject GameObject => this.gameObject;
        public string ActorName => gameObject.name; // Use config name, fallback to GO name
        public string DisplayName => _enemyConfig.enemyName ?? gameObject.name; // Use config name, fallback to GO name
        public int _currentSpeed = 5; // For turn order
        [Header("Battle Stats")]
        private EnemyConfig _enemyConfig;

        [Header("AI Attack")]
        public GameObject targetOverride; // For testing specific target

        public int CurrentSpeed => _currentSpeed;
        public bool IsPlayerControlled => false;
        public bool IsAlive => Health.IsAlive;

        public Health Health { get; private set; }
        public Resistance _resistance;
        public Sprite Avatar => _enemyConfig.avatar;

        public SpriteCharacter2D spriteCharacter;

        private GameObject marker;

        void Awake()
        {
            Health = GetComponent<Health>();
            _resistance = GetComponent<Resistance>();
            spriteCharacter = GetComponentInChildren<SpriteCharacter2D>();
            marker = transform.Find("TurnMarker").gameObject;

            if (_enemyConfig != null)
            {
                Initialize(_enemyConfig, 0);
            }
        }
        private void Start()
        {
            StartCoroutine(OrientToCamera());
        }

        private IEnumerator OrientToCamera()
        {
            // Wait one frame to ensure Camera.main is available
            yield return null;

            Vector3 direction = Camera.main.transform.position - transform.position;
            direction.y = 0; // eliminate vertical difference

            if (direction != Vector3.zero) // avoid zero length
            {
                Quaternion rotation = Quaternion.LookRotation(-direction, Vector3.up);
                transform.rotation = rotation;
            }
        }


        /// <summary>
        /// Initializes the enemy actor with data from an EnemyConfigSO.
        /// This should be called immediately after instantiation.
        /// </summary>
        /// <param name="config">The EnemyConfigSO to use for this actor.</param>
        public void Initialize(EnemyConfig config, int index)
        {
            _enemyConfig = config;

            if (spriteCharacter != null)
            {
                spriteCharacter.LoadFromConfig(config);
            }

            // Set base stats from config
            Health.Initialize(config.maxHealth);
            _currentSpeed = config.baseSpeed;

            // Initialize resistance component with data from config
            // if (_resistance != null)
            // {
            //     _resistance.Initialize(config.percentDamageResistances);
            // }
            // else
            // {
            //     Debug.LogWarning($"Resistance component missing on {gameObject.name}. Cannot apply resistances from config.");
            // }

            gameObject.name = $"{config.enemyName}-{index}"; // Update GameObject name for clarity in hierarchy
            Debug.Log($"Initialized {gameObject.name} with config: {config.enemyName}");
        }

        public void OnTurnStart()
        {
            Debug.Log($"<color=red>{ActorName}'s Turn!</color> AI is thinking...");
            marker.SetActive(true);
            // Trigger AI logic, play thinking animation, etc.
            // This is where you would start a coroutine for enemy AI action
        }

        public void OnTurnEnd()
        {
            Debug.Log($"{ActorName}'s Turn Ended.");
            marker.SetActive(false);
        }

        // Methods to interact with the HealthComponent
        public void TakeDamage(float damage) => Health.TakeDamage(damage);
        public void Heal(float amount) => Health.Heal(amount);

        // This method will be called by BattleManager to get AI's chosen action
        public PlayerAction ChooseAIAction(IBattleActor playerTarget)
        {
            // Simple AI: always attack the provided player target with its default attack
            if (_enemyConfig != null && _enemyConfig.attacks[0] != null)
            {
                // if (spriteCharacter != null) spriteCharacter.Play(States.BattleSpriteState.Attack);
                return new PlayerAction(_enemyConfig.attacks[0], playerTarget.GameObject);
            }
            else
            {
                Debug.LogWarning($"{ActorName} has no default attack defined in its config! Cannot choose action.");
                return new PlayerAction(PlayerAction.PlayerActionType.Defend); // Fallback
            }
        }

    }
}