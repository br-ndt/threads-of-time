using UnityEngine;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Data structure to define a player's chosen action.
    /// </summary>
    public struct PlayerAction
    {
        public PlayerActionType ActionType;
        public AttackDefinition AttackDefinition; // For attack actions
        public GameObject TargetActor; // For single-target actions
        // public List<GameObject> TargetActors; // For multi-target actions
        // public ItemDefinitionSO ItemUsed; // For item usage
        // public AbilityDefinitionSO AbilityUsed; // For ability usage

        public enum PlayerActionType
        {
            None,
            Attack,
            UseItem,
            UseAbility,
            Defend,
            Run
        }

        // Example constructor for an attack action
        public PlayerAction(AttackDefinition attackDef, GameObject target)
        {
            ActionType = PlayerActionType.Attack;
            AttackDefinition = attackDef;
            TargetActor = target;
            // Initialize other fields as needed
        }

        // Example constructor for other actions
        public PlayerAction(PlayerActionType type)
        {
            ActionType = type;
            AttackDefinition = null;
            TargetActor = null;
        }
    }
}