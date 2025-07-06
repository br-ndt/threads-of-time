using Assets.Scripts.Configs;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Component representing a player-controlled actor in battle.
    /// </summary>
    [RequireComponent(typeof(Health))]
    // Add CharacterCombatStats and other relevant components here
    public class PlayerBattleActor : MonoBehaviour, IBattleActor
    {
        public GameObject GameObject => this.gameObject;
        public string ActorName => gameObject.name; // Use config name, fallback to GO name
        public string DisplayName => _heroConfig.heroName ?? gameObject.name; // Use config name, fallback to GO name

        public int _currentSpeed = 5; // For turn order
        [Header("Battle Stats")]
        private HeroConfig _heroConfig;
        public Health Health { get; private set; }

        public int CurrentSpeed => _currentSpeed;
        public bool IsPlayerControlled => true;
        public bool IsAlive => Health.IsAlive;

        public Sprite Avatar => _heroConfig.avatar;

        void Awake()
        {
            Health = GetComponent<Health>();
        }

        /// <summary>
        /// Initializes the enemy actor with data from an EnemyConfigSO.
        /// This should be called immediately after instantiation.
        /// </summary>
        /// <param name="config">The EnemyConfigSO to use for this actor.</param>
        public void Initialize(HeroConfig config)
        {
            _heroConfig = config;


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

            gameObject.name = config.heroName; // Update GameObject name for clarity in hierarchy
            Debug.Log($"Initialized {gameObject.name} with config: {config.heroName}");
        }

        public void OnTurnStart()
        {
            Debug.Log($"<color=cyan>{ActorName}'s Turn!</color> Awaiting player input...");
            // Enable UI elements specific to player character, highlight character, etc.
        }

        public void OnTurnEnd()
        {
            Debug.Log($"{ActorName}'s Turn Ended.");
            // Disable UI elements, remove highlight, etc.
        }

        // Methods to interact with the HealthComponent
        public void TakeDamage(float damage) => Health.TakeDamage(damage);
        public void Heal(float amount) => Health.Heal(amount);
    }
}