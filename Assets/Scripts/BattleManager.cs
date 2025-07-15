using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // For OrderBy
using Assets.Scripts.States;
using Assets.Scripts.Events;
using Assets.Scripts.Configs;
using Assets.Scripts.Combat;
using Assets.Scripts.Audio;

public class BattleManager : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField]
    private GameStateChangeEvent gameStateChanged; // Listens for GameState changes
    [SerializeField]
    private GameStateChangeEvent requestGameStateChange;
    [SerializeField]
    private BattleStartEvent battleStartEvent; // Raises when battle starts
    [SerializeField]
    private ActorTurnEvent actorTurnEvent; // Raises when an actor's turn begins
    [SerializeField]
    private AvailableAttacksEvent availableAttacksEvent; // Raises on a player's turn, to inform the UI of available player attacks
    [SerializeField]
    private AvailableTargetsEvent availableTargetsEvent; // Raises on a player's turn, to inform the UI of available enemy targets
    [SerializeField]
    private PlayerActionChosenEvent playerActionChosenEvent; // Listens for player's chosen action
    [SerializeField]
    private BattleEndEvent battleEndEvent; // Raises when battle ends
    [SerializeField]
    private BattleEndEvent battleLeaveEvent; // Raises when player leaves the battle

    [Header("Battle Elements (Programmatic)")]
    public GameObject enemyPrefab; // Assign an enemy prefab for instantiation
    public GameObject playerCharacterPrefab; // Assign a player character prefab for instantiation
    public Transform[] playerSpawnPoints; // Assign empty transforms in your scene
    public Transform[] enemySpawnPoints; // Assign empty transforms in your scene
    // [SerializeField] private HeroConfig[] demoHeroConfigs;

    [SerializeField] private BattleConfig currentBattleConfig;

    // Core Battle State
    public BattleState CurrentBattleState { get; private set; } = BattleState.CalculatingTurnOrder;
    private List<IBattleActor> activeActors = new List<IBattleActor>(); // All participants in battle
    private List<IBattleActor> playerActors = new List<IBattleActor>(); // Actors controlled by player
    private List<IBattleActor> enemyActors = new List<IBattleActor>(); // Actors controlled by computer
    private Queue<IBattleActor> turnOrderQueue = new Queue<IBattleActor>(); // Current turn sequence
    private IBattleActor currentActor; // The actor whose turn it is
    private PlayerAction playerChosenAction; // Stores the action chosen by the player

    // Reference to the CombatManager for performing attacks
    private CombatManager combatCalculator;

    private void Awake()
    {
        // Get reference to the CombatManager (assuming it's also a component in the BattleScene, or globally accessible)
        combatCalculator = FindAnyObjectByType<CombatManager>();
        if (combatCalculator == null)
        {
            Debug.LogError("CombatManager not found in scene! Battle system will not function.");
        }
    }

    private void OnEnable()
    {
        // Subscribe to GameState changes from the main GameStateMachine
        if (gameStateChanged != null)
        {
            gameStateChanged.OnEventRaised += HandleGameStateChange;
        }

        // Subscribe to player action choices
        if (playerActionChosenEvent != null)
        {
            playerActionChosenEvent.OnEventRaised += HandlePlayerActionChosen;
        }

        if (battleLeaveEvent != null)
        {
            battleLeaveEvent.OnEventRaised += HandleLeaveBattle;
        }

    }

    private void OnDisable()
    {
        if (gameStateChanged != null)
        {
            gameStateChanged.OnEventRaised -= HandleGameStateChange;
        }

        if (playerActionChosenEvent != null)
        {
            playerActionChosenEvent.OnEventRaised -= HandlePlayerActionChosen;
        }

        if (battleLeaveEvent != null)
        {
            battleLeaveEvent.OnEventRaised -= HandleLeaveBattle;
        }
    }

    private void HandlePlayerActionChosen(PlayerAction action)
    {
        playerChosenAction = action; // Reset action
    }

    private void HandleGameStateChange((GameState state, GameConfig config) payload)
    {
        if (payload.state == GameState.Battle)
        {
            currentBattleConfig = payload.config as BattleConfig;
            if (currentBattleConfig != null)
            {
                Debug.Log($"<color=blue>BattleManager received Battle State! Setting up battle with config: {currentBattleConfig.name}</color>");
                SetupBattle();
            }
            else
            {
                Debug.LogError("BattleState received without a valid BattleConfigSO!");
            }
        }
    }

    private void SetupBattle()
    {
        Debug.Log("--- Starting Battle Setup ---");

        // Clear previous actors if any
        activeActors.Clear();
        playerActors.Clear();
        enemyActors.Clear();
        turnOrderQueue.Clear();

        // 1. Instantiate Player Characters (from BattleConfig or a default party)
        // For simplicity, let's just add one predefined player actor for now
        for (int i = 0; i < currentBattleConfig.heroes.Count; i++)
        {
            var heroConfig = currentBattleConfig.heroes[i];
            GameObject playerGO = Instantiate(playerCharacterPrefab);
            playerGO.transform.position = playerSpawnPoints[i].position;

            var playerActor = playerGO.GetComponent<PlayerBattleActor>();
            if (playerActor != null)
            {
                playerActor.Initialize(heroConfig);
                activeActors.Add(playerActor);
                playerActors.Add(playerActor);
                Debug.Log($"Spawned Player: {playerActor.ActorName}");
            }
            else
            {
                Debug.LogError("PlayerCharacterPrefab does not have a PlayerBattleActor component!");
            }
        }

        // 2. Instantiate Enemies based on BattleConfig
        for (int i = 0; i < currentBattleConfig.enemies.Count && i < enemySpawnPoints.Length; i++)
        {
            var enemyConfig = currentBattleConfig.enemies[i];
            GameObject enemyGO = Instantiate(enemyPrefab);
            enemyGO.transform.position = enemySpawnPoints[i].position;

            var enemyActor = enemyGO.GetComponent<EnemyBattleActor>();
            if (enemyActor != null)
            {
                enemyActor.Initialize(enemyConfig);
                activeActors.Add(enemyActor);
                enemyActors.Add(enemyActor);
                Debug.Log($"Spawned Enemy: {enemyActor.ActorName}");
            }
            else
            {
                Debug.LogError("EnemyPrefab does not have an EnemyBattleActor component!");
            }
        }

        Debug.Log($"Total active actors: {activeActors.Count}");

        // Start the battle loop!
        battleStartEvent.Raise(activeActors);
        StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        Debug.Log("<color=orange>--- Battle Loop Started ---</color>");

        AudioManager.Instance.PlayBGM(AudioManager.AudioContext.Battle);

        while (CheckBattleEndConditions() == BattleEndResult.None)
        {
            CurrentBattleState = BattleState.CalculatingTurnOrder;
            yield return StartCoroutine(CalculateTurnOrder());

            while (turnOrderQueue.Count > 0 && CheckBattleEndConditions() == BattleEndResult.None)
            {
                currentActor = turnOrderQueue.Dequeue();

                // If actor is defeated, skip their turn
                if (!currentActor.IsAlive)
                {
                    Debug.Log($"{currentActor.ActorName} is defeated, skipping turn.");
                    continue;
                }

                if (CurrentBattleState != BattleState.BattleEnd)
                {
                    actorTurnEvent.Raise((currentActor, true)); // Notify UI and other systems
                }

                if (currentActor.IsPlayerControlled)
                {
                    CurrentBattleState = BattleState.PlayerTurn;
                    Debug.Log($"<color=green>Waiting for {currentActor.ActorName}'s input...</color>");
                    availableAttacksEvent.Raise((currentActor as PlayerBattleActor).Attacks);
                    availableTargetsEvent.Raise(enemyActors.Where(e => e.IsAlive).ToList());
                    playerChosenAction = new PlayerAction(PlayerAction.PlayerActionType.None); // Reset action

                    // Wait until player action is chosen (event handler will set playerChosenAction)
                    yield return new WaitUntil(() => playerChosenAction.ActionType != PlayerAction.PlayerActionType.None);

                    yield return StartCoroutine(PerformPlayerAction(currentActor, playerChosenAction));
                }
                else // Enemy turn
                {
                    CurrentBattleState = BattleState.EnemyTurn;
                    yield return StartCoroutine(PerformEnemyAction(currentActor as EnemyBattleActor)); // Cast to EnemyBattleActor for AI logic
                }

                if (CurrentBattleState != BattleState.BattleEnd)
                {
                    actorTurnEvent.Raise((currentActor, false));
                }

                // Remove defeated actors from active list
                activeActors.RemoveAll(actor => !actor.IsAlive);
                enemyActors.RemoveAll(actor => !actor.IsAlive || actor.IsPlayerControlled);
                playerActors.RemoveAll(actor => !actor.IsPlayerControlled);
            }
        }

        // Battle has ended
        BattleEndResult result = CheckBattleEndConditions();
        bool playerWon = result == BattleEndResult.PlayersWin;

        CurrentBattleState = BattleState.BattleEnd;
        battleEndEvent.Raise(playerWon); // Announce battle outcome
        Debug.Log($"<color=orange>--- Battle Ended: {(playerWon ? "VICTORY" : "DEFEAT")} ---</color>");
    }

    private IEnumerator CalculateTurnOrder()
    {
        Debug.Log("Calculating turn order...");
        CurrentBattleState = BattleState.CalculatingTurnOrder;

        // Simple turn order: sort by speed (highest first)
        List<IBattleActor> sortedActors = activeActors
            .Where(a => a.IsAlive) // Only include living actors
            .OrderByDescending(a => a.CurrentSpeed)
            .ToList();

        turnOrderQueue.Clear();
        foreach (var actor in sortedActors)
        {
            turnOrderQueue.Enqueue(actor);
            Debug.Log($"  - {actor.DisplayName} (Speed: {actor.CurrentSpeed})");
        }
        yield return null; // Allow a frame to pass if needed
    }

    private IEnumerator PerformPlayerAction(IBattleActor playerActor, PlayerAction action)
    {
        CurrentBattleState = BattleState.PerformingAction;
        Debug.Log($"<color=green>{playerActor.ActorName} chose to {action.ActionType}!</color>");

        switch (action.ActionType)
        {
            case PlayerAction.PlayerActionType.Attack:
                if (action.AttackDefinition != null && action.TargetActor != null)
                {
                    // Find the IBattleActor component on the target GameObject
                    IBattleActor targetActor = action.TargetActor.GetComponent<IBattleActor>();
                    if (targetActor != null && targetActor.IsAlive)
                    {
                        Debug.Log($"{playerActor.DisplayName} attacking {targetActor.DisplayName} with {action.AttackDefinition.attackName}...");
                        combatCalculator.PerformAttack(playerActor.GameObject, targetActor.GameObject, action.AttackDefinition);

                        // Await animation or effects if desired
                        yield return new WaitForSeconds(1.5f); // Simulate action time
                        if (!targetActor.IsAlive)
                        {
                            Debug.Log($"{targetActor.DisplayName} was defeated!");
                            // Trigger death animation/effects
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid or defeated target for attack: {action.TargetActor?.name}");
                    }
                }
                else
                {
                    Debug.LogWarning("Attack action missing definition or target.");
                }
                break;
            case PlayerAction.PlayerActionType.Defend:
                Debug.Log($"{playerActor.ActorName} is defending!");
                // Apply defense buff
                yield return new WaitForSeconds(1f);
                break;
            case PlayerAction.PlayerActionType.Run:
                Debug.Log($"{playerActor.ActorName} attempts to run!");
                // Implement escape logic
                SpriteCharacter2D sprite = playerActor.GameObject.GetComponentInChildren<SpriteCharacter2D>();
                sprite.isFlipped = !sprite.isFlipped;
                sprite.Play(BattleSpriteState.Run);
                CurrentBattleState = BattleState.BattleEnd;
                battleEndEvent.Raise(false); // Assume running is a "loss" for now
                yield break; // Exit coroutine
            default:
                Debug.LogWarning($"Unhandled player action type: {action.ActionType}");
                break;
        }
        playerChosenAction = new PlayerAction(PlayerAction.PlayerActionType.None); // Reset for next turn
        yield return null;
    }

    private IEnumerator PerformEnemyAction(EnemyBattleActor enemyActor)
    {
        Debug.Log(enemyActor);
        CurrentBattleState = BattleState.PerformingAction;
        Debug.Log($"<color=red>{enemyActor.ActorName} is performing its action...</color>");

        // Simple AI: Find a living player target
        IBattleActor playerTarget = playerActors
            .Where(a => a.IsAlive)
            .OrderBy(_ => Random.value) // shuffle with LINQ
            .FirstOrDefault();

        if (playerTarget != null)
        {
            PlayerAction aiAction = enemyActor.ChooseAIAction(playerTarget); // Enemy AI chooses
            Debug.Log($"{enemyActor.ActorName} attacking {playerTarget.ActorName} with {aiAction.AttackDefinition.attackName}...");
            combatCalculator.PerformAttack(enemyActor.GameObject, playerTarget.GameObject, aiAction.AttackDefinition);

            yield return new WaitForSeconds(1.5f); // Simulate action time
            if (!playerTarget.IsAlive)
            {
                Debug.Log($"{playerTarget.ActorName} was defeated!");
                // Trigger death animation/effects
            }
        }
        else
        {
            Debug.Log($"{enemyActor.ActorName} has no valid targets to attack.");
        }
        yield return null;
    }

    // --- Battle End Conditions ---
    private enum BattleEndResult { None, PlayersWin, PlayersLose }

    private BattleEndResult CheckBattleEndConditions()
    {
        bool allPlayersDefeated = playerActors.All(a => !a.IsAlive);
        bool allEnemiesDefeated = enemyActors.All(a => !a.IsAlive);

        if (allPlayersDefeated)
        {
            AudioManager.Instance.PlayBGM(AudioManager.AudioContext.Defeat);
            return BattleEndResult.PlayersLose;
        }
        if (allEnemiesDefeated)
        {
            AudioManager.Instance.PlayBGM(AudioManager.AudioContext.Victory);
            return BattleEndResult.PlayersWin;
        }
        return BattleEndResult.None;
    }

    public void HandleLeaveBattle(bool didWin)
    {
        Debug.Log("<color=blue>BattleManager: Battle sequence ending. Transitioning back to Overworld.</color>");
        // Grant rewards, update player state, clear battle specific objects
        activeActors.Clear();
        playerActors.Clear();
        enemyActors.Clear();
        turnOrderQueue.Clear();
        currentActor = null;

        AudioManager.Instance.PlayBGM(AudioManager.AudioContext.Overworld);

        if (requestGameStateChange != null)
        {
            requestGameStateChange.Raise((GameState.Overworld, null)); // Request transition back to overworld
        }
        else
        {
            Debug.LogError("requestGameStateChange not assigned to BattleManager! Cannot transition back to overworld.");
        }
    }

}