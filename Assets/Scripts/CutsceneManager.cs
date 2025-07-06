using UnityEngine;
using Assets.Scripts.States;
using Assets.Scripts.Events;
using Assets.Scripts.Configs;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField]
    private GameStateChangeEvent gameStateChangeEvent; // Listen for GameState changes

    private CutsceneConfig currentCutsceneConfig;
    private Coroutine cutsceneRoutine;

    private void OnEnable()
    {
        if (GameStateMachine.Instance != null) // Check if GameStateMachine is initialized
        {
            GameStateMachine.Instance.OnStateChanged += HandleGameStateChange;
        }
    }

    private void OnDisable()
    {
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.OnStateChanged -= HandleGameStateChange;
        }
        if (cutsceneRoutine != null)
        {
            StopCoroutine(cutsceneRoutine);
        }
    }

    private void HandleGameStateChange(GameState newState, GameConfig config)
    {
        if (newState == GameState.Cutscene)
        {
            currentCutsceneConfig = config as CutsceneConfig;
            if (currentCutsceneConfig != null)
            {
                Debug.Log($"<color=magenta>CutsceneManager received Cutscene State! Setting up cutscene: {currentCutsceneConfig.name}</color>");
                cutsceneRoutine = StartCoroutine(PlayCutscene());
            }
            else
            {
                Debug.LogError("CutsceneState received without a valid CutsceneConfig!");
            }
        }
    }

    private IEnumerator PlayCutscene()
    {
        Debug.Log("--- Playing Cutscene ---");

        // Example of programmatic cutscene logic
        Debug.Log($"Cutscene ID: {currentCutsceneConfig.cutsceneID}");

        for (int i = 0; i < currentCutsceneConfig.dialogueLines.Count; i++)
        {
            Debug.Log($"Dialogue ({i + 1}/{currentCutsceneConfig.dialogueLines.Count}): {currentCutsceneConfig.dialogueLines[i]}");
            // Here you'd update UI, animate characters, etc.
            yield return new WaitForSeconds(currentCutsceneConfig.duration / currentCutsceneConfig.dialogueLines.Count); // Simple timing
        }

        Debug.Log("--- Cutscene Complete! ---");

        // Transition back after cutscene finishes
        if (gameStateChangeEvent != null)
        {
            if (!string.IsNullOrEmpty(currentCutsceneConfig.nextSceneOnEnd))
            {
                // If the cutscene leads to another specific scene (e.g., dungeon)
                // You'd need to create a new GameConfig if parameters are needed
                Debug.Log($"Loading next scene: {currentCutsceneConfig.nextSceneOnEnd}");
                // In this simplified example, we'll just go back to overworld,
                // but in a real game, you'd likely transition to the specific scene via the GameStateMachine
            }
            gameStateChangeEvent.Raise(GameState.Overworld); // Default back to overworld
        }
    }
}