using System;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.Configs;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised when a battle actor receives any condition(s).
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Condition Apply Event")]
    public class ConditionApplyEvent : GameEvent<(IBattleActor, ConditionStatsDictionary)> { }
}