using Assets.Scripts.Combat;
using Assets.Scripts.Events;
using UnityEngine;

/// <summary>
/// ScriptableObject event for character death.
/// Carries the battle actor corresponding to the character who died.
/// </summary>
[CreateAssetMenu(menuName = "Game Events/Death Event")]
public class DeathEvent : GameEvent<IBattleActor> { }
