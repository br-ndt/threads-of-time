using System.Collections.Generic;
using UnityEngine; // For GameObject

namespace Assets.Scripts.Combat
{
    public interface IBattleActor
    {
        GameObject GameObject { get; } // Reference to the Unity GameObject
        Sprite Avatar { get; }
        List<AttackDefinition> Attacks { get; }
        string ActorName { get; }
        string DisplayName { get; }
        int CurrentSpeed { get; } // Used for turn order calculation
        bool IsPlayerControlled { get; }
        bool IsAlive { get; }

        void HandleTurnEvent((IBattleActor actor, bool isStarting) payload); // Event Listener to ActorTurnEvent raised by BattleManager when it's this actor's turn

        // Add more methods as needed, e.g., TakeDamage(), Heal(), ApplyStatusEffect()
    }
}