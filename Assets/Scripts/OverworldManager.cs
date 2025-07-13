using UnityEngine;
using Assets.Scripts.States;
using Assets.Scripts.Events;
using Assets.Scripts.Configs;
using NUnit.Framework;
using System.Collections.Generic;

public class OverworldManager : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField]
    private GameStateChangeEvent gameStateChangeEvent; // Assign Game State Change Event

    [Header("Battle & Cutscene Configurations")]
    [SerializeField]
    private List<BattleConfig> possibleBattles; // Assign a pre-configured BattleConfig asset
    [SerializeField]
    private CutsceneConfig specificCutsceneConfig; // Assign a pre-configured CutsceneConfig asset

    void Update()
    {
        // Example: Trigger battle when 'B' is pressed
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (gameStateChangeEvent != null && possibleBattles.Count > 0)
            {
                BattleConfig selectedBattle = possibleBattles[Random.Range(0, possibleBattles.Count)];
                // Request a transition to Battle state with a specific battle config
                gameStateChangeEvent.Raise(GameState.Battle, selectedBattle);
            }
            else
            {
                Debug.LogWarning("Battle Trigger: GameStateChangeEvent or BattleConfig is not assigned!");
            }
        }

        // Example: Trigger cutscene when 'C' is pressed
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (gameStateChangeEvent != null && specificCutsceneConfig != null)
            {
                // Request a transition to Cutscene state with a specific cutscene config
                gameStateChangeEvent.Raise(GameState.Cutscene, specificCutsceneConfig);
            }
            else
            {
                Debug.LogWarning("Cutscene Trigger: GameStateChangeEvent or CutsceneConfig is not assigned!");
            }
        }
    }

    // Example: When player hits an invisible trigger for a battle
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyTrigger") && gameStateChangeEvent != null && possibleBattles.Count > 0)
        {
            Debug.Log("Player entered enemy trigger! Starting battle...");
            BattleConfig selectedBattle = possibleBattles[Random.Range(0, possibleBattles.Count)];
            gameStateChangeEvent.Raise(GameState.Battle, selectedBattle);
        }
    }
}