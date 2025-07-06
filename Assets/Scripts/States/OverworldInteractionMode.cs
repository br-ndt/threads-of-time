namespace Assets.Scripts.States
{
    public enum OverworldInteractionMode
    {
        Exploration, // Default movement and general interaction
        Dialogue,    // Player is in a dialogue sequence, input goes to dialogue system
        MenuOpen,    // Pause menu, inventory, etc.
        Puzzle       // Actively solving an environmental puzzle
    }
}