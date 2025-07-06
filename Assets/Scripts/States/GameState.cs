namespace Assets.Scripts.States
{
    // High-level states of gameplay
    public enum GameState
    {
        None,           // Initial or undefined state
        Overworld,      // Main exploration map
        Battle,         // Combat sequence
        Cutscene,       // Cinematic
        Menu,           // Pause menu, inventory, etc.
        Loading,        // During scene transitions or heavy data loading
        GameOver,       // Game over screen
        TitleScreen     // Main menu
    }
}