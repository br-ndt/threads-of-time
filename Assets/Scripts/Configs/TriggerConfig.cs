using System;
using System.Collections.Generic;
using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    /// <summary>
    /// Two lists of triggers for any trigger event: those required for emission, and those emitted.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Configs/Trigger Config")]
    public class TriggerConfig : GameConfig
    {
        [SerializeField] public List<TriggerEvent> requiredTriggers;
        [SerializeField] public List<TriggerEvent> emittedTriggers;
    }
}
