using Assets.Scripts.Events;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Assets.Scripts.Combat
{
    public class Health : MonoBehaviour
    {
        public float MaxHealth = 100f;
        public float CurrentHealth { get; private set; }

        public bool IsAlive => CurrentHealth > 0;
        [SerializeField] private HealthChangeEvent healthChangeEvent;
        [SerializeField] private DamageTakenEvent damageTakenEvent;
        [SerializeField] private DeathEvent deathEvent;

        private bool initialized = false;
        private IBattleActor _actor; // Reference to the IBattleActor interface on this GameObject

        void OnEnable()
        {
            if (damageTakenEvent != null)
            {
                damageTakenEvent.OnEventRaised += HandleDamageTaken;
            }
        }

        void OnDisable()
        {
            if (damageTakenEvent != null)
            {
                damageTakenEvent.OnEventRaised -= HandleDamageTaken;
            }
        }

        void Awake()
        {
            CurrentHealth = MaxHealth;
            _actor = GetComponent<IBattleActor>();
        }

        void Start()
        {
            // Raise event initially to set up UI for current health
            if (healthChangeEvent != null && _actor != null)
            {
                healthChangeEvent.Raise((_actor, CurrentHealth, MaxHealth));
            }
        }

        private void HandleDamageTaken((IBattleActor target, float damage) payload)
        {
            if (payload.target != _actor)
            {
                return;
            }
            CurrentHealth -= payload.damage;
            CurrentHealth = Mathf.Max(CurrentHealth, 0); // Health can't go below 0
            Debug.Log($"{gameObject.name} took {payload.damage:F2} damage. Health: {CurrentHealth:F2}");

            // Raise the event after health changes
            if (healthChangeEvent != null && _actor != null)
            {
                healthChangeEvent.Raise((_actor, CurrentHealth, MaxHealth));
            }

            if (!IsAlive)
            {
                Debug.Log($"{gameObject.name} has been defeated!");
                deathEvent.Raise(_actor);
            }
        }

        public void Initialize(int maxHealth)
        {
            Initialize(maxHealth, maxHealth);
        }

        public void Initialize(int currentHealth, int maxHealth)
        {
            if (!initialized)
            {
                MaxHealth = maxHealth;
                CurrentHealth = currentHealth;
                initialized = true;
                if (!IsAlive)
                {
                    Debug.Log($"{gameObject.name} was dead as the battle began...");
                    deathEvent.Raise(_actor);
                }
            }
        }
    }
}