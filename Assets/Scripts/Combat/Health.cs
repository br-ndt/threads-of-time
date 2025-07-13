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

        private bool initialized = false;
        private IBattleActor _actor; // Reference to the IBattleActor interface on this GameObject
        private SpriteCharacter2D _sprite;

        void Awake()
        {
            CurrentHealth = MaxHealth;
            _actor = GetComponent<IBattleActor>();
            _sprite = GetComponentInChildren<SpriteCharacter2D>();
        }

        void Start()
        {
            // Raise event initially to set up UI for current health
            if (healthChangeEvent != null && _actor != null)
            {
                healthChangeEvent.Raise(_actor, CurrentHealth, MaxHealth);
            }
        }

        public void Initialize(int maxHealth)
        {
            if (!initialized)
            {
                MaxHealth = maxHealth;
                CurrentHealth = maxHealth;
                initialized = true;
            }
        }

        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Max(CurrentHealth, 0); // Health can't go below 0
            Debug.Log($"{gameObject.name} took {damage:F2} damage. Health: {CurrentHealth:F2}");

            // Raise the event after health changes
            if (healthChangeEvent != null && _actor != null)
            {
                healthChangeEvent.Raise(_actor, CurrentHealth, MaxHealth);
            }

            if (!IsAlive)
            {
                Debug.Log($"{gameObject.name} has been defeated!");
                // You'd trigger death animations, effects, disable actor, etc. 
                if (_sprite != null) DieAndFade(_sprite);
                else gameObject.SetActive(false);
            }
        }

        private void DieAndFade(SpriteCharacter2D sprite)
        {
            StartCoroutine(DieAndFadeRoutine(sprite));
        }

        private IEnumerator DieAndFadeRoutine(SpriteCharacter2D sprite)
        {
            sprite.Play(States.BattleSpriteState.Die);

            yield return new WaitForSeconds(0.3f); // Let death animation start a moment

            Renderer rend = sprite.GetComponent<Renderer>();
            Material mat = rend != null ? rend.material : null;

            if (mat != null && mat.HasProperty("_Color"))
            {
                Color originalColor = mat.color;
                float fadeDuration = 3f;
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


        public void Heal(float amount)
        {
            CurrentHealth += amount;
            CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
            Debug.Log($"{gameObject.name} healed {amount:F2}. Health: {CurrentHealth:F2}");

            // Raise the event after health changes
            if (healthChangeEvent != null && _actor != null)
            {
                healthChangeEvent.Raise(_actor, CurrentHealth, MaxHealth);
            }
        }
    }
}