using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // For OrderBy
using Assets.Scripts.States;
using Assets.Scripts.Events;
using Assets.Scripts.Configs;
using Assets.Scripts.Combat;
using System;
using Unity.Collections;
using Unity.VisualScripting;

public class BattleManager : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField]
    private GameStateChangeEvent gameStateChanged; // Listens for GameState changes
    [SerializeField]
    private GameStateChangeEvent requestGameStateChange;
    [SerializeField]
    private SetupBattleEvent setupBattleEvent;
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
    [SerializeField]
    private RecordTriggerEvent recordTriggerEvent; // Raises to record gameplay triggers
    [SerializeField]
    private IntegerGameEvent experienceGainEvent; // Raises when battle is won to assign experience

    [Header("Battle Elements")]
    public GameObject enemyPrefab; // Assign an enemy prefab for instantiation
    public GameObject playerCharacterPrefab; // Assign a player character prefab for instantiation
    public Transform[] playerSpawnPoints; // Assign empty transforms in your scene
    public Transform[] enemySpawnPoints; // Assign empty transforms in your scene
    // [SerializeField] private HeroConfig[] demoHeroConfigs;

    [SerializeField] private BattleConfig currentBattleConfig;

    // Core Battle State
    public BattleState CurrentBattleState { get; private set; } = BattleState.CalculatingTurnOrder;
    private List<IBattleActor> activeActors = new List<IBattleActor>(); // All participants in battle
    private List<HeroBattleActor> playerActors = new List<HeroBattleActor>(); // Actors controlled by player
    private List<EnemyBattleActor> enemyActors = new List<EnemyBattleActor>(); // Actors controlled by computer
    private Queue<IBattleActor> turnOrderQueue = new Queue<IBattleActor>(); // Current turn sequence
    private IBattleActor currentActor; // The actor whose turn it is
    private PlayerAction playerChosenAction; // Stores the action chosen by the player
    private int experienceValue;

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
        experienceValue = 0;
        activeActors.Clear();
        playerActors.Clear();
        enemyActors.Clear();
        turnOrderQueue.Clear();

        SetupBattleContext context = new(currentBattleConfig.staticHeroes, currentBattleConfig.enemies);

        setupBattleEvent.Raise(context);
        experienceValue = context.Enemies.Select(enemy => enemy.experienceValue).Sum();

        // context now holds the heroes and enemies
        for (int i = 0; i < Math.Min(3, context.Heroes.Count); i++)
        {
            var hero = context.Heroes[i];
            if (hero.CurrentHealth <= 0)
            {
                continue;
            }
            GameObject playerGO = Instantiate(playerCharacterPrefab);
            playerGO.transform.position = playerSpawnPoints[i].position;

            var playerActor = playerGO.GetComponent<HeroBattleActor>();
            if (playerActor != null)
            {
                playerActor.Initialize(hero);
                activeActors.Add(playerActor);
                playerActors.Add(playerActor);
                Debug.Log($"Spawned Player: {playerActor.DisplayName}");
            }
            else
            {
                Debug.LogError("PlayerCharacterPrefab does not have a HeroBattleActor component!");
            }
        }

        for (int i = 0; i < context.Enemies.Count && i < enemySpawnPoints.Length; i++)
        {
            var enemyConfig = context.Enemies[i];
            GameObject enemyGO = Instantiate(enemyPrefab);
            enemyGO.transform.position = enemySpawnPoints[i].position;

            var enemyActor = enemyGO.GetComponent<EnemyBattleActor>();
            if (enemyActor != null)
            {
                enemyActor.Initialize(enemyConfig);
                activeActors.Add(enemyActor);
                enemyActors.Add(enemyActor);
                Debug.Log($"Spawned Enemy: {enemyActor.DisplayName}");
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

        while (CheckBattleEndConditions() == BattleEndResult.None)
        {
            CurrentBattleState = BattleState.CalculatingTurnOrder;
            yield return StartCoroutine(CalculateTurnOrder());

            while (CurrentBattleState != BattleState.BattleEnd && turnOrderQueue.Count > 0 && CheckBattleEndConditions() == BattleEndResult.None)
            {
                bool skipTurn = false;
                currentActor = turnOrderQueue.Dequeue();

                // If actor is stunned, skip their turn before ticking down conditions
                if (currentActor.ActiveConditions.Keys.Contains(Condition.Stunned))
                {
                    Debug.Log($"{currentActor.DisplayName} is incapacitated, skipping turn.");
                    skipTurn = true;
                }
                List<Condition> conditionsToClear = new();
                foreach (KeyValuePair<Condition, ActiveCondition> kvp in currentActor.ActiveConditions)
                {
                    // PLACEHOLDER
                    if (kvp.Key == Condition.Burning)
                    {
                        Debug.Log($"<color=red>{currentActor.DisplayName} takes fire damage!</color>");
                        AttackDefinition burnDamage = new();
                        burnDamage.baseDamageModifierByType[DamageType.ADDO] = currentActor.ActiveConditions[kvp.Key].Damage;
                        combatCalculator.PerformAttack(null, currentActor, burnDamage, true);
                    }


                    kvp.Value.TickTurn();
                    if (kvp.Value.Turns <= 0)
                    {
                        conditionsToClear.Add(kvp.Key);
                    }
                }
                foreach (Condition condition in conditionsToClear)
                {
                    currentActor.ActiveConditions.Remove(condition);
                }
                // After ticking conditions, check if actor is alive. If not, skip their turn
                if (!currentActor.IsAlive)
                {
                    Debug.Log($"{currentActor.DisplayName} is dead, skipping turn.");
                    skipTurn = true;
                }

                if (skipTurn)
                {
                    continue;
                }

                actorTurnEvent.Raise((currentActor, true)); // Notify UI and other systems

                if (currentActor.IsPlayerControlled)
                {
                    CurrentBattleState = BattleState.PlayerTurn;
                    Debug.Log($"<color=green>Waiting for {currentActor.DisplayName}'s input...</color>");
                    availableAttacksEvent.Raise((currentActor as HeroBattleActor).Attacks);
                    availableTargetsEvent.Raise(enemyActors.Where(e => e.IsAlive).Select(actor => (IBattleActor)actor).ToList());
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

                actorTurnEvent.Raise((currentActor, false));

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
        if (playerWon && currentBattleConfig.onCompleteTrigger != null)
        {
            currentBattleConfig.onCompleteTrigger.Raise(true);
            recordTriggerEvent.Raise((new List<TriggerEvent>() { currentBattleConfig.onCompleteTrigger }, true));
        }
        else if (!playerWon && currentBattleConfig.onFailTrigger != null)
        {
            currentBattleConfig.onFailTrigger.Raise(true);
            recordTriggerEvent.Raise((new List<TriggerEvent>() { currentBattleConfig.onFailTrigger }, true));
        }
        battleEndEvent.Raise(playerWon); // Announce battle outcome
        experienceGainEvent.Raise(experienceValue);
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
        Debug.Log($"<color=green>{playerActor.DisplayName} chose to {action.ActionType}!</color>");

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
                        combatCalculator.PerformAttack(playerActor, targetActor, action.AttackDefinition);

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
                Debug.Log($"{playerActor.DisplayName} is defending!");
                // Apply defense buff
                yield return new WaitForSeconds(1f);
                break;
            case PlayerAction.PlayerActionType.Run:
                Debug.Log($"{playerActor.DisplayName} attempts to run!");
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
        Debug.Log($"<color=red>{enemyActor.DisplayName} is performing its action...</color>");

        // Simple AI: Find a living player target
        IBattleActor playerTarget = playerActors
            .Where(a => a.IsAlive)
            .OrderBy(_ => UnityEngine.Random.value) // shuffle with LINQ
            .FirstOrDefault();

        if (playerTarget != null)
        {
            PlayerAction aiAction = enemyActor.ChooseAIAction(playerTarget); // Enemy AI chooses
            Debug.Log($"{enemyActor.DisplayName} attacking {playerTarget.DisplayName} with {aiAction.AttackDefinition.attackName}...");
            combatCalculator.PerformAttack(enemyActor, playerTarget, aiAction.AttackDefinition, false);

            yield return new WaitForSeconds(1.5f); // Simulate action time
            if (!playerTarget.IsAlive)
            {
                Debug.Log($"{playerTarget.DisplayName} was defeated!");
                // Trigger death animation/effects
            }
        }
        else
        {
            Debug.Log($"{enemyActor.DisplayName} has no valid targets to attack.");
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
            return BattleEndResult.PlayersLose;
        }
        if (allEnemiesDefeated)
        {
            return BattleEndResult.PlayersWin;
        }
        return BattleEndResult.None;
    }

    public void HandleLeaveBattle(bool didWin)
    {
        Debug.Log("<color=blue>BattleManager: Battle sequence ending. Transitioning back to Overworld.</color>");
        // Grant rewards, update player state, clear battle specific objects
        experienceValue = 0;
        activeActors.Clear();
        playerActors.Clear();
        enemyActors.Clear();
        turnOrderQueue.Clear();
        currentActor = null;
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