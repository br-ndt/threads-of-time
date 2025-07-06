using UnityEngine;
using Assets.Scripts.Events;

public class EventListenerExample : MonoBehaviour
{
    [Header("Void Event Listener")]
    [SerializeField]
    private GameEvent playerDiedEvent; // Assign the SAME "Void Game Event" asset here

    [Header("Int Event Listener")]
    [SerializeField]
    private IntGameEvent playerHealthChangedEvent; // Assign the SAME "Int Game Event" asset here

    // IMPORTANT: Subscribe to events when enabled, unsubscribe when disabled
    void OnEnable()
    {
        if (playerDiedEvent != null)
        {
            playerDiedEvent.OnEventRaised += HandlePlayerDied;
        }
        if (playerHealthChangedEvent != null)
        {
            playerHealthChangedEvent.OnEventRaised += HandlePlayerHealthChanged;
        }
    }

    void OnDisable()
    {
        if (playerDiedEvent != null)
        {
            playerDiedEvent.OnEventRaised -= HandlePlayerDied;
        }
        if (playerHealthChangedEvent != null)
        {
            playerHealthChangedEvent.OnEventRaised -= HandlePlayerHealthChanged;
        }
    }

    private void HandlePlayerDied()
    {
        Debug.Log("<color=red>Listener received: Player has died! Display Game Over Screen!</color>");
        // Example: Trigger a game over state, show UI, etc.
    }

    private void HandlePlayerHealthChanged(int currentHealth)
    {
        Debug.Log($"<color=green>Listener received: Player health is now {currentHealth}. Update UI!</color>");
        // Example: Update a health bar UI element
    }
}