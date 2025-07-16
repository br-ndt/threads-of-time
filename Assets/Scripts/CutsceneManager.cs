using UnityEngine;
using Assets.Scripts.States;
using Assets.Scripts.Events;
using Assets.Scripts.Configs;
using System.Collections;
using System;

public class CutsceneManager : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private GameStateChangeEvent gameStateChanged; // Listen for GameState changes
    [SerializeField] private GameStateChangeEvent requestGameStateChange;
    private CutsceneConfig currentCutsceneConfig;
    private Coroutine cutsceneRoutine;

    private void OnEnable()
    {
        if (gameStateChanged != null)
        {
            gameStateChanged.OnEventRaised += HandleGameStateChange;
        }
    }

    private void OnDisable()
    {
        if (gameStateChanged != null)
        {
            gameStateChanged.OnEventRaised -= HandleGameStateChange;
        }
        if (cutsceneRoutine != null)
        {
            StopCoroutine(cutsceneRoutine);
        }
    }

    private void HandleGameStateChange((GameState, GameConfig) stateAndConfig)
    {
        if (stateAndConfig.Item1 == GameState.Cutscene)
        {
            currentCutsceneConfig = stateAndConfig.Item2 as CutsceneConfig;
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
        Debug.Log($"Cutscene ID: {currentCutsceneConfig.sceneActionID}");

        for (int i = 0; i < currentCutsceneConfig.dialogueLines.Count; i++)
        {
            Debug.Log($"Dialogue ({i + 1}/{currentCutsceneConfig.dialogueLines.Count}): {currentCutsceneConfig.dialogueLines[i]}");
            // Here you'd update UI, animate characters, etc.
            yield return new WaitForSeconds(currentCutsceneConfig.duration / currentCutsceneConfig.dialogueLines.Count); // Simple timing
        }

        Debug.Log("--- Cutscene Complete! ---");

        // Transition back after cutscene finishes
        if (requestGameStateChange != null)
        {
            Debug.Log($"Loading next scene: {currentCutsceneConfig.sceneChange.newState}...");
            requestGameStateChange.Raise((currentCutsceneConfig.sceneChange.newState, currentCutsceneConfig.sceneChange.newStateConfig));
        }
    }
}