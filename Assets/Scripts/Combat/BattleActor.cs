using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.Configs;
using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Attack))]
    [RequireComponent(typeof(Resistance))]
    public abstract class BattleActor<T> : MonoBehaviour, IBattleActor where T : ActorConfig
    {
        protected string _actorID;
        protected string _displayName;
        protected int _currentSpeed;
        protected T _actorConfig;
        protected Health _health;
        protected Resistance _resistance;
        protected SpriteCharacter2D _spriteCharacter;
        protected Sprite _avatar;
        protected bool _isPlayerControlled;
        protected bool _isAlive = true;
        protected List<AttackDefinition> _attacks;

        [Header("Event Channels")]
        [SerializeField] private ActorTurnEvent actorTurnEvent;
        [SerializeField] private DeathEvent deathEvent;

        public GameObject GameObject => this.gameObject;
        public List<AttackDefinition> Attacks => _attacks;
        public Sprite Avatar => _avatar;
        public string ActorID => _actorID; // used for ID 
        public string DisplayName => _displayName; // used for UI, but falls back to GO name
        public int CurrentSpeed => _currentSpeed;
        public bool IsAlive => _isAlive;
        public bool IsPlayerControlled => _isPlayerControlled;

        protected void OnEnable()
        {
            actorTurnEvent.OnEventRaised += HandleTurnEvent;
            deathEvent.OnEventRaised += HandleDeath;
        }

        protected void OnDisable()
        {
            actorTurnEvent.OnEventRaised -= HandleTurnEvent;
            deathEvent.OnEventRaised -= HandleDeath;
        }

        private void Update()
        {
            FaceCamera();
        }

        private void FaceCamera()
        {
            if (Camera.main == null)
                return;

            Vector3 direction = Camera.main.transform.position - transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = rotation;
            }
        }

        private void HandleDeath(IBattleActor actor)
        {
            if ((Object)actor == this)
            {
                Debug.Log("Match");
                _isAlive = false;
                StartCoroutine(DieAndFade());
            }
        }

        private IEnumerator DieAndFade()
        {
            _spriteCharacter.Play(States.BattleSpriteState.Die);

            yield return new WaitForSeconds(0.3f); // Let death animation start a moment

            Renderer rend = _spriteCharacter.GetComponent<Renderer>();
            Material mat = rend != null ? rend.material : null;

            if (mat != null && mat.HasProperty("_Color"))
            {
                Color originalColor = mat.color;
                float fadeDuration = 1.2f;
                float elapsed = 0f;

                while (elapsed < fadeDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / fadeDuration);
                    float smoothAlpha = Mathf.SmoothStep(1f, 0f, t);
                    mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, smoothAlpha);
                    yield return null;
                }

                mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            }

            gameObject.SetActive(false);
            Debug.Log($"{gameObject.name} has faded out smoothly and been disabled.");
        }

        public void HandleTurnEvent((IBattleActor actor, bool isStarting) payload)
        {
            if ((Object)payload.actor == this)
            {
                if (payload.isStarting)
                {
                    string color = _isPlayerControlled ? "cyan" : "red";
                    Debug.Log($"<color={color}>{DisplayName}'s Turn!</color> Awaiting player input...");
                }
                else
                {
                    Debug.Log($"{DisplayName}'s Turn Ended.");
                }
            }
        }

        /// <summary>
        /// Initializes the actor with data from an ActorConfigSO.
        /// This should be called immediately after instantiation.
        /// </summary>
        /// <param name="config">The ActorConfigSO to use for this actor.</param>
        public void Initialize(T config)
        {
            _actorConfig = config;
            _health = GetComponent<Health>();
            _resistance = GetComponent<Resistance>();
            _spriteCharacter = GetComponentInChildren<SpriteCharacter2D>();

            if (_spriteCharacter != null)
            {
                _spriteCharacter.LoadFromConfig(config);
            }

            // Set base stats from config
            _actorID = config.actorID;
            _displayName = config.actorName;
            _avatar = config.avatar;
            _attacks = config.attacks;
            _health.Initialize(config.baseHealth);
            _resistance.Initialize(config.flatResistances, config.resistanceMultipliers);
            _currentSpeed = config.baseSpeed;

            gameObject.name = config.actorID; // Update GameObject name for clarity in hierarchy
            Debug.Log($"Initialized {gameObject.name} with config: {config.actorName}");
        }
    }
}