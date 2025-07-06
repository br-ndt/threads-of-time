namespace Assets.Scripts.States
{
    // States of a battle scene
    public enum BattleState
    {
        CalculatingTurnOrder,
        PlayerTurn,
        EnemyTurn,
        PerformingAction, // When an actor is animating an attack, playing an effect
        BattleEnd // When battle is over (win/loss)
    }
}