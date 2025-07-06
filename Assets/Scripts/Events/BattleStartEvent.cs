using UnityEngine;
using Assets.Scripts.Combat;
using System.Collections.Generic; // For IBattleActor

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised when a specific actor's turn starts.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Battle Start Event")]
    public class BattleStartEvent : ScriptableObject
    {
        public event System.Action<List<IBattleActor>> OnBattleStarted;

        public void Raise(List<IBattleActor> actors)
        {
            OnBattleStarted?.Invoke(actors);
        }
    }
}