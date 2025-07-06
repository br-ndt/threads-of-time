namespace Assets.Scripts.States
{
    // Levels of interactivity during a cutscene
    public enum CutsceneInteractivity
    {
        None,             // Full cinematic
        LimitedMovement,  // Game-in-cutscene
        DialogueOnly      // Player controls cutscene flow
    }
}