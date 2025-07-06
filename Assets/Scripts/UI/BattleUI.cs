using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Events;
using Assets.Scripts.Combat;
using Assets.Scripts.States;
using TMPro;
using Assets.Scripts.Configs;
using System.Collections.Generic;

namespace Assets.Scripts.UI
{
    public class BattleUI : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private GameStateChangeEvent gameStateChangeEvent;
        [SerializeField] private ActorTurnEvent actorTurnEvent; // Listen for actor's turn
        [SerializeField] private PlayerActionChosenEvent playerActionChosenEvent; // Raise player's action
        [SerializeField] private BattleStartEvent battleStartEvent; // Listen for battle start
        [SerializeField] private BattleEndEvent battleEndEvent; // Listen for battle end
        [SerializeField] private BattleEndEvent battleLeaveEvent; // Invoke when player leaves battle
        [SerializeField] private AvailableTargetsEvent availableTargetsEvent; // Listen for available targets
        [SerializeField] private HealthChangeEvent healthChangeEvent;

        [Header("UI Elements")]
        [SerializeField] private GameObject playerActionPanel; // Panel with Attack, Defend, Run buttons
        [SerializeField] private Text turnInfoText; // Displays whose turn it is
        [SerializeField] private GameObject actionsPanel;
        [SerializeField] private Button attackButton;
        [SerializeField] private Button defendButton;
        [SerializeField] private Button runButton;

        [Header("Target Selection")]
        [SerializeField] private GameObject targetSelectionPanel; // Parent panel for target selection
        [SerializeField] private GameObject dynamicTargetAreas; // Parent for target buttons
        [SerializeField] private GameObject targetButtonPrefab; // Prefab for enemy target buttons
        [SerializeField] private Button cancelButton; // To cancel target selection

        [Header("Player Actors")] // UI Elements for Player Actors
        [SerializeField] private GameObject heroAreaPanel;
        [SerializeField] private GameObject heroInfoPanelPrefab; // Prefab to display a Player Actor's info

        [Header("Non-Player Actors")] // UI Elements for Non Player Actors
        [SerializeField] private GameObject enemyAreaPanel;
        [SerializeField] private GameObject enemyInfoPanelPrefab; // Prefab to display a Non-Player Actor's info

        [Header("Battle Result")]
        [SerializeField] private Text battleResultText; // For win/loss message
        [SerializeField] private Button returnToOverworldButton; // For example: one button for one enemy target

        // References to current battle actors for target selection (e.g., enemy buttons)
        private IBattleActor currentActorOnTurn;
        private List<GameObject> spawnedTargetButtons = new(); // To manage spawned buttons
        private AttackDefinition selectedAttackDefinition; // Store attack definition if player selects "Attack"

        void Awake()
        {
            // Initialize UI state
            playerActionPanel.SetActive(false);
            heroAreaPanel.SetActive(true);
            targetSelectionPanel.SetActive(false);
            battleResultText.gameObject.SetActive(false);

            // Hook up button listeners
            attackButton.onClick.AddListener(OnAttackButtonClicked);
            defendButton.onClick.AddListener(OnDefendButtonClicked);
            runButton.onClick.AddListener(OnRunButtonClicked);
            cancelButton.onClick.AddListener(OnCancelTargetSelection);
            returnToOverworldButton.onClick.AddListener(OnReturnToOverworldClicked);
            ClearTargetButtons(); // Ensure no old buttons are present
        }

        void OnEnable()
        {
            if (gameStateChangeEvent != null)
            {
                gameStateChangeEvent.OnStateChangeRequested += HandleGameStateChange;
            }
            if (actorTurnEvent != null)
            {
                actorTurnEvent.OnTurnStarted += HandleActorTurnStarted;
            }
            if (battleStartEvent != null)
            {
                battleStartEvent.OnBattleStarted += HandleBattleStarted;
            }
            if (battleEndEvent != null)
            {
                battleEndEvent.OnBattleEnded += HandleBattleEnded;
            }
            if (availableTargetsEvent != null)
            {
                availableTargetsEvent.OnTargetsAvailable += HandleAvailableTargets;
            }
            if (healthChangeEvent != null)
            {
                healthChangeEvent.OnHealthChanged += HandleHealthChanged;
            }
        }

        void OnDisable()
        {
            if (gameStateChangeEvent != null)
            {
                gameStateChangeEvent.OnStateChangeRequested -= HandleGameStateChange;
            }
            if (actorTurnEvent != null)
            {
                actorTurnEvent.OnTurnStarted -= HandleActorTurnStarted;
            }
            if (battleStartEvent != null)
            {
                battleStartEvent.OnBattleStarted -= HandleBattleStarted;
            }
            if (battleEndEvent != null)
            {
                battleEndEvent.OnBattleEnded -= HandleBattleEnded;
            }
            if (availableTargetsEvent != null)
            {
                availableTargetsEvent.OnTargetsAvailable -= HandleAvailableTargets;
            }
            if (healthChangeEvent != null)
            {
                healthChangeEvent.OnHealthChanged -= HandleHealthChanged;
            }
            ClearTargetButtons(); // Clean up buttons on disable
        }

        private void HandleGameStateChange(GameState state, GameConfig config)
        {
            if (state != GameState.Battle)
            {
                playerActionPanel.SetActive(false);
                targetSelectionPanel.SetActive(false);
                battleResultText.gameObject.SetActive(false);
                returnToOverworldButton.gameObject.SetActive(false);
            }
        }

        private void HandleActorTurnStarted(IBattleActor actor)
        {
            currentActorOnTurn = actor;
            turnInfoText.text = $"{actor.DisplayName}'s Turn!";

            if (actor.IsPlayerControlled)
            {
                actionsPanel.SetActive(true); // Show player action buttons
            }
            else
            {
                actionsPanel.SetActive(false); // Hide for enemy turn
            }
        }

        private void HandleHealthChanged(IBattleActor actor, float currentHealth, float maxHealth)
        {
            Debug.Log($"UI received Health Change for {actor.ActorName}: {currentHealth}/{maxHealth}");

            if (actor.IsPlayerControlled)
            {
                Transform playerPanel = heroAreaPanel.transform.Find(actor.ActorName);
                if (playerPanel != null)
                {
                    playerPanel.Find("Health").GetComponent<Image>().fillAmount = currentHealth / maxHealth;
                    playerPanel.Find("Health").GetComponentInChildren<TMP_Text>().text = $"{currentHealth:F0}/{maxHealth:F0}";
                }
                else
                {
                    Debug.LogWarning($"Could not find hero info panel for hero: {actor.ActorName}");
                }
            }
            else
            {
                Debug.Log(actor.GameObject);
                Transform enemyPanel = enemyAreaPanel.transform.Find(actor.GameObject.name);
                if (enemyPanel != null)
                {
                    enemyPanel.Find("Health").GetComponent<Image>().fillAmount = currentHealth / maxHealth;
                    enemyPanel.Find("Health").GetComponentInChildren<TMP_Text>().text = $"{currentHealth:F0}/{maxHealth:F0}";
                }
                else
                {
                    Debug.LogWarning($"Could not find enemy info panel for enemy: {actor.GameObject.name}");
                }

            }
        }

        private void HandleBattleStarted(List<IBattleActor> actors)
        {
            Debug.Log($"BattleUI received Battle Started: {actors}");
            playerActionPanel.SetActive(true); // Hide action panel
            targetSelectionPanel.SetActive(false); // Hide target panel

            foreach (IBattleActor actor in actors)
            {
                if (actor.IsPlayerControlled)
                {
                    //TODO (make this a method for DRY)
                    GameObject infoGO = Instantiate(heroInfoPanelPrefab, heroAreaPanel.transform);
                    TMP_Text title = infoGO.transform.Find("Title").GetComponent<TMP_Text>();
                    Image avatarImg = infoGO.transform.Find("Avatar").GetComponent<Image>();
                    Image healthImg = infoGO.transform.Find("Health").GetComponent<Image>();
                    Health actorHealth = actor.GameObject.GetComponent<Health>();
                    TMP_Text healthText = healthImg.GetComponentInChildren<TMP_Text>();

                    infoGO.name = actor.ActorName;
                    title.text = actor.DisplayName;
                    avatarImg.sprite = actor.Avatar;
                    healthImg.fillAmount = actorHealth.CurrentHealth / actorHealth.MaxHealth;
                    healthText.text = $"{actorHealth.CurrentHealth:F0}/{actorHealth.MaxHealth:F0}";
                }
                else
                {
                    GameObject infoGO = Instantiate(enemyInfoPanelPrefab, enemyAreaPanel.transform);
                    TMP_Text title = infoGO.transform.Find("Title").GetComponent<TMP_Text>();
                    Image avatarImg = infoGO.transform.Find("Avatar").GetComponent<Image>();
                    Image healthImg = infoGO.transform.Find("Health").GetComponent<Image>();
                    Health actorHealth = actor.GameObject.GetComponent<Health>();
                    TMP_Text healthText = healthImg.GetComponentInChildren<TMP_Text>();

                    infoGO.name = actor.ActorName;
                    title.text = actor.DisplayName;
                    avatarImg.sprite = actor.Avatar;
                    healthImg.fillAmount = actorHealth.CurrentHealth / actorHealth.MaxHealth;
                    healthText.text = $"{actorHealth.CurrentHealth:F0}/{actorHealth.MaxHealth:F0}";
                }
            }
        }

        private void HandleBattleEnded(bool playerWon)
        {
            Debug.Log($"BattleUI received Battle Ended: {(playerWon ? "Win" : "Loss")}");
            playerActionPanel.SetActive(false); // Hide action panel
            enemyAreaPanel.SetActive(false); // Hide enemy info
            targetSelectionPanel.SetActive(false); // Hide target panel

            battleResultText.text = playerWon ? "VICTORY!" : "DEFEAT!";
            battleResultText.gameObject.SetActive(true);
            returnToOverworldButton.gameObject.SetActive(true);
        }

        private void HandleAvailableTargets(List<IBattleActor> targets)
        {
            ClearTargetButtons(); // Clear any existing buttons first

            foreach (IBattleActor targetActor in targets)
            {
                // Instantiate a new button from prefab
                GameObject buttonGO = Instantiate(targetButtonPrefab, dynamicTargetAreas.transform);
                Button targetButton = buttonGO.GetComponent<Button>();
                TMP_Text buttonText = buttonGO.GetComponentInChildren<TMP_Text>();
                Image buttonImg = buttonGO.transform.Find("Border").Find("Avatar").GetComponent<Image>();

                if (targetButton != null && buttonText != null && buttonImg != null)
                {
                    buttonText.text = targetActor.DisplayName; // Set button text to enemy name
                    buttonImg.sprite = targetActor.Avatar;
                    // Pass the actual GameObject reference to the button's listener
                    GameObject targetGameObject = targetActor.GameObject;
                    targetButton.onClick.AddListener(() => OnTargetButtonClicked(targetGameObject));
                    spawnedTargetButtons.Add(buttonGO); // Keep track for cleaning up
                }
                else
                {
                    Debug.LogError("Target button prefab missing Button or Text component!");
                }
            }
            Debug.Log($"Generated {targets.Count} target buttons.");
        }

        private void ClearTargetButtons()
        {
            foreach (GameObject buttonGO in spawnedTargetButtons)
            {
                Destroy(buttonGO);
            }
            spawnedTargetButtons.Clear();
        }

        // --- Player Action Button Handlers ---

        private void OnAttackButtonClicked()

        {
            Debug.Log("Attack button clicked. Awaiting target selection.");
            actionsPanel.SetActive(false); // Hide action panel
            targetSelectionPanel.SetActive(true); // Show target selection (in a real game, list enemies)
                                                  // For now, let's just assume MonsterZ is the target (you'd have a list of targets here)

            selectedAttackDefinition = FindFirstObjectByType<CombatManager>().demoAttackDefinition;
            if (selectedAttackDefinition == null) Debug.LogError("Demo Attack not assigned/found on CombatManager!");
        }

        private void OnDefendButtonClicked()
        {
            playerActionChosenEvent.Raise(new PlayerAction(PlayerAction.PlayerActionType.Defend));
            playerActionPanel.SetActive(false);
            targetSelectionPanel.SetActive(false); // Ensure hidden if somehow active
            ClearTargetButtons(); // Clean up
        }

        private void OnRunButtonClicked()
        {
            playerActionChosenEvent.Raise(new PlayerAction(PlayerAction.PlayerActionType.Run));
            playerActionPanel.SetActive(false);
            targetSelectionPanel.SetActive(false); // Ensure hidden if somehow active
            ClearTargetButtons(); // Clean up
        }

        private void OnCancelTargetSelection()
        {
            Debug.Log("Target selection cancelled.");
            targetSelectionPanel.SetActive(false); // Hide target selection panel
            actionsPanel.SetActive(true); // Show action panel again
            ClearTargetButtons(); // Clean up generated buttons
        }

        private void OnTargetButtonClicked(GameObject targetGameObject)
        {
            if (selectedAttackDefinition != null && targetGameObject != null)
            {
                playerActionChosenEvent.Raise(new PlayerAction(selectedAttackDefinition, targetGameObject));
                targetSelectionPanel.SetActive(false); // Hide target panel
                ClearTargetButtons(); // Clean up buttons after selection
            }
            else
            {
                Debug.LogWarning("Selected attack or target is null. Cannot perform action.");
            }
        }

        private void OnReturnToOverworldClicked()
        {
            battleLeaveEvent.Raise(true);
        }
    }
}