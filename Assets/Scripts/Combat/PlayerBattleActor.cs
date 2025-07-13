using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Configs;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Component representing a player-controlled actor in battle.
    /// </summary>
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Resistance))]
    // Add CharacterCombatStats and other relevant components here
    public class PlayerBattleActor : MonoBehaviour, IBattleActor
    {
        public GameObject GameObject => this.gameObject;
        public string ActorName => gameObject.name; // Use config name, fallback to GO name
        public string DisplayName => _heroConfig.heroName ?? gameObject.name; // Use config name, fallback to GO name

        public int _currentSpeed = 5; // For turn order
        [Header("Battle Stats")]
        private HeroConfig _heroConfig;
        public Resistance Resistance { get; private set; }
        public Health Health { get; private set; }

        public int CurrentSpeed => _currentSpeed;
        public bool IsPlayerControlled => true;
        public bool IsAlive => Health.IsAlive;

        public Sprite Avatar => _heroConfig.avatar;
        public List<AttackDefinition> Attacks => _heroConfig.attacks;

        public SpriteCharacter2D spriteCharacter;

        private GameObject marker;

        void Awake()
        {
            Health = GetComponent<Health>();
            Resistance = GetComponent<Resistance>();

            spriteCharacter = GetComponentInChildren<SpriteCharacter2D>();
            marker = transform.Find("TurnMarker").gameObject;
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
        /// Initializes the hero actor with data from an HeroConfigSO.
        /// This should be called immediately after instantiation.
        /// </summary>
        /// <param name="config">The HeroConfigSO to use for this actor.</param>
        public void Initialize(HeroConfig config)
        {
            _heroConfig = config;

            if (spriteCharacter != null)
            {
                spriteCharacter.LoadFromConfig(config);
            }

            // Set base stats from config
            Health.Initialize(config.maxHealth);
            Resistance.Initialize(config.flatResistances, config.resistanceMultipliers);
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
            marker.SetActive(true);
            // Enable UI elements specific to player character, highlight character, etc.
        }

        public void OnTurnEnd()
        {
            Debug.Log($"{ActorName}'s Turn Ended.");
            marker.SetActive(false);
            // Disable UI elements, remove highlight, etc.
        }


        // Methods to interact with the HealthComponent
        public void TakeDamage(float damage) => Health.TakeDamage(damage);
        public void Heal(float amount) => Health.Heal(amount);
    }
}