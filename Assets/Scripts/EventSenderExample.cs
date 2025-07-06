using UnityEngine;
using Assets.Scripts.Events;

public class EventSenderExample : MonoBehaviour
{
    [Header("Void Event")]
    [SerializeField]
    private GameEvent playerDiedEvent; // Assign your "Void Game Event" asset here

    [Header("Int Event")]
    [SerializeField]
    private IntGameEvent playerHealthChangedEvent; // Assign your "Int Game Event" asset here

    [SerializeField]
    private int dummyHealthValue = 100;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerHealthChangedEvent != null)
            {
                if (dummyHealthValue > 0)
                {
                    dummyHealthValue -= 10;
                    Debug.Log($"Sending Player Health Changed Event: {dummyHealthValue}");
                    playerHealthChangedEvent.Raise(dummyHealthValue);
                }
                else
                {
                    if (playerDiedEvent != null)
                    {
                        Debug.Log("Sending Player Died Event!");
                        playerDiedEvent.Raise();
                    }
                    else
                    {
                        Debug.LogWarning("Player Died Event not assigned in Inspector!");
                    }
                }
            }
            else
            {
                Debug.LogWarning("Player Health Changed Event not assigned in Inspector!");
            }
        }
    }
}