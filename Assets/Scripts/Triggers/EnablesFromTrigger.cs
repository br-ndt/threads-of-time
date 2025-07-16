using Assets.Scripts.Events;
using UnityEngine;

public class EnablesFromTrigger : MonoBehaviour
{
    [SerializeField] private TriggerEvent spawnTrigger;
    [SerializeField] private GameObject objectToEnable;

    void OnEnable()
    {
        spawnTrigger.OnEventRaised += HandleSpawnTrigger;
    }

    void OnDisable()
    {
        spawnTrigger.OnEventRaised -= HandleSpawnTrigger;
    }

    void HandleSpawnTrigger(bool value)
    {
        objectToEnable.SetActive(value);
    }
}
