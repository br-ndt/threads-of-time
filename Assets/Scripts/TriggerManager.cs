using System.Collections.Generic;
using Assets.Scripts.Events;
using Assets.Scripts.Triggers;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private TriggerCheckEvent triggerCheckEvent;
    [SerializeField] private RecordTriggerEvent recordTriggerEvent;
    [SerializeField] private Dictionary<string, bool> activatedTriggers;

    private void OnEnable()
    {
        activatedTriggers = new Dictionary<string, bool>();
        triggerCheckEvent.OnEventRaised += HandleTriggerCheck;
        recordTriggerEvent.OnEventRaised += HandleRecordTrigger;
    }

    private void OnDisable()
    {
        triggerCheckEvent.OnEventRaised -= HandleTriggerCheck;
        recordTriggerEvent.OnEventRaised -= HandleRecordTrigger;
    }

    private void HandleTriggerCheck(TriggerCheckContext context)
    {
        // context starts with IsValid = false...
        if (context.TriggersToCheck.Count == 0)
        {
            Debug.Log("No triggers required. Auto-completing...");
        }
        else
        {
            foreach (TriggerEvent trigger in context.TriggersToCheck)
            {
                if (!activatedTriggers.ContainsKey(trigger.name))
                {
                    return;
                }
                // todo: make this handle different types like int
                if (activatedTriggers[trigger.name] == false)
                {
                    return;
                }
            }
        }
        context.IsValid = true;
    }

    private void HandleRecordTrigger((List<TriggerEvent> triggers, bool value) payload)
    {
        foreach (TriggerEvent trigger in payload.triggers)
        {
            activatedTriggers[trigger.name] = payload.value;
        }
    }
}