using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised when a battle actor receives any condition(s).
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Condition Clear Event")]
    public class ConditionClearEvent : GameEvent<(IBattleActor, List<Condition>)> { }
}