using UnityEngine;
using Assets.Scripts.Combat;
using System.Collections.Generic;
using Assets.Scripts.Configs; // For IBattleActor

namespace Assets.Scripts.Events
{
    /// <summary>
    /// Event raised to notify game state about baseline hero config.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Events/Hero Config Event")]
    public class HeroConfigEvent : GameEvent<HeroConfig> { }
}