using UnityEngine; // For GameObject

namespace Assets.Scripts.Combat
{
    public interface IBattleActor
    {
        GameObject GameObject { get; } // Reference to the Unity GameObject
        Sprite Avatar { get; }
        string ActorName { get; }
        string DisplayName { get; }
        int CurrentSpeed { get; } // Used for turn order calculation
        bool IsPlayerControlled { get; }
        bool IsAlive { get; }

        void OnTurnStart(); // Called by BattleManager when it's this actor's turn
        void OnTurnEnd();   // Called by BattleManager when this actor's turn concludes

        // Add more methods as needed, e.g., TakeDamage(), Heal(), ApplyStatusEffect()
    }
}