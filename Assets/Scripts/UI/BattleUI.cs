using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Events;
using Assets.Scripts.Combat;
using Assets.Scripts.States;
using TMPro;
using Assets.Scripts.Configs;
using System.Collections.Generic;
using System.Collections;

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
        [SerializeField] private AvailableAttacksEvent availableAttacksEvent; // Listen for available targets
        [SerializeField] private AvailableTargetsEvent availableTargetsEvent; // Listen for available targets
        [SerializeField] private HealthChangeEvent healthChangeEvent;

        [Header("UI Elements")]
        [SerializeField] private GameObject bottomPanel; // Panel with Attack, Defend, Run buttons
        [SerializeField] private Text turnInfoText; // Displays whose turn it is
        [SerializeField] private GameObject actionsPanel;
        [SerializeField] private Button attackButton;
        [SerializeField] private Button defendButton;
        [SerializeField] private Button runButton;

        [Header("Attack Selection")]
        [SerializeField] private GameObject attackMenu; // Panel to show available attacks
        [SerializeField] private Transform attackButtonAreas; // Parent for dynamically spawned attack buttons
        [SerializeField] private GameObject attackButtonPrefab; // Prefab for individual attack buttons
        [SerializeField] private Button attackSelectionCancelButton; // Button to go back to main actions

        [Header("Target Selection")]
        [SerializeField] private GameObject targetMenu; // Parent panel for target selection
        [SerializeField] private GameObject targetButtonArea; // Parent for target buttons
        [SerializeField] private GameObject targetButtonPrefab; // Prefab for enemy target buttons
        [SerializeField] private Button targetSelectionCancelButton; // To cancel target selection

        [Header("Player Actors")] // UI Elements for Player Actors
        [SerializeField] private GameObject heroInfoArea;
        [SerializeField] private GameObject heroInfoPrefab; // Prefab to display a Player Actor's info
        private Dictionary<IBattleActor, GameObject> spawnedHeroInfos = new();

        [Header("Non-Player Actors")] // UI Elements for Non Player Actors
        [SerializeField] private GameObject enemyAreaPanel;
        [SerializeField] private GameObject enemyInfoPanelPrefab; // Prefab to display a Non-Player Actor's info
        private Dictionary<IBattleActor, GameObject> spawnedEnemyInfos = new();

        [Header("Battle Result")]
        [SerializeField] private Text battleResultText; // For win/loss message
        [SerializeField] private Button returnToOverworldButton;

        // References to current battle actors for target selection (e.g., enemy buttons)
        private IBattleActor currentActorOnTurn;
        private List<GameObject> spawnedTargetButtons = new(); // To manage spawned target buttons
        private List<GameObject> spawnedAttackButtons = new(); // To manage spawned attack buttons
        private AttackDefinition selectedAttackDefinition; // Store attack definition if player selects "Attack"

        void Awake()
        {
            // Initialize UI state
            bottomPanel.SetActive(false);
            attackMenu.SetActive(false);
            targetMenu.SetActive(false);
            battleResultText.gameObject.SetActive(false);

            heroInfoArea.SetActive(false);
            enemyAreaPanel.SetActive(false);

            // Hook up button listeners
            attackButton.onClick.AddListener(OnAttackButtonClicked);
            defendButton.onClick.AddListener(OnDefendButtonClicked);
            runButton.onClick.AddListener(OnRunButtonClicked);
            attackSelectionCancelButton.onClick.AddListener(OnCancelAttackSelection);
            targetSelectionCancelButton.onClick.AddListener(OnCancelTargetSelection);
            returnToOverworldButton.onClick.AddListener(OnReturnToOverworldClicked);

            ClearAttackButtons(); // Ensure no old buttons are present
            ClearTargetButtons();
        }

        void OnEnable()
        {
            if (gameStateChangeEvent != null)
            {
                gameStateChangeEvent.OnEventRaised += HandleGameStateChange;
            }
            if (actorTurnEvent != null)
            {
                actorTurnEvent.OnEventRaised += HandleActorTurnStarted;
            }
            if (battleStartEvent != null)
            {
                battleStartEvent.OnEventRaised += HandleBattleStarted;
            }
            if (battleEndEvent != null)
            {
                battleEndEvent.OnEventRaised += HandleBattleEnded;
            }
            if (availableAttacksEvent != null)
            {
                availableAttacksEvent.OnEventRaised += HandleAvailableAttacks;
            }
            if (availableTargetsEvent != null)
            {
                availableTargetsEvent.OnEventRaised += HandleAvailableTargets;
            }
            if (healthChangeEvent != null)
            {
                healthChangeEvent.OnEventRaised += HandleHealthChanged;
            }
        }

        void OnDisable()
        {
            if (gameStateChangeEvent != null)
            {
                gameStateChangeEvent.OnEventRaised -= HandleGameStateChange;
            }
            if (actorTurnEvent != null)
            {
                actorTurnEvent.OnEventRaised -= HandleActorTurnStarted;
            }
            if (battleStartEvent != null)
            {
                battleStartEvent.OnEventRaised -= HandleBattleStarted;
            }
            if (battleEndEvent != null)
            {
                battleEndEvent.OnEventRaised -= HandleBattleEnded;
            }
            if (availableAttacksEvent != null)
            {
                availableAttacksEvent.OnEventRaised -= HandleAvailableAttacks;
            }
            if (availableTargetsEvent != null)
            {
                availableTargetsEvent.OnEventRaised -= HandleAvailableTargets;
            }
            if (healthChangeEvent != null)
            {
                healthChangeEvent.OnEventRaised -= HandleHealthChanged;
            }
            ClearAttackButtons(); // Clean up buttons on disable
            ClearTargetButtons();
            ClearAllActorInfoPanels(); // Clean up actor info panels
        }

        private void HandleGameStateChange((GameState state, GameConfig config) payload)
        {
            if (payload.state != GameState.Battle)
            {
                // This will happen when leaving battle
                bottomPanel.SetActive(false);
                actionsPanel.SetActive(false); // Also hide the actions panel
                attackMenu.SetActive(false);
                targetMenu.SetActive(false);
                battleResultText.gameObject.SetActive(false);
                returnToOverworldButton.gameObject.SetActive(false);
                ClearAllActorInfoPanels(); // Clear all actor panels when leaving battle state
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
            attackMenu.SetActive(false);
            targetMenu.SetActive(false);
            ClearTargetButtons();
            ClearAttackButtons();
        }

        private void HandleHealthChanged((IBattleActor actor, float currentHealth, float maxHealth) payload)
        {
            Debug.Log($"UI received Health Change for {payload.actor.ActorName}: {payload.currentHealth}/{payload.maxHealth}");

            GameObject infoPanelGO = payload.actor.IsPlayerControlled
                ? spawnedHeroInfos.GetValueOrDefault(payload.actor)
                : spawnedEnemyInfos.GetValueOrDefault(payload.actor);
            if (infoPanelGO != null)
            {
                // Assuming "Health" is a child GameObject with an Image (fill) and a Text (text)
                Image healthImg = infoPanelGO.transform.Find("Health").GetComponent<Image>();
                TMP_Text healthText = healthImg.GetComponentInChildren<TMP_Text>();

                float targetFill = payload.currentHealth / payload.maxHealth;

                if (healthImg != null) StartCoroutine(SmoothFill(healthImg, targetFill, 5f));

                if (healthText != null) healthText.text = $"{payload.currentHealth:F0}/{payload.maxHealth:F0}";

                // Hide panel if actor is defeated
                infoPanelGO.SetActive(payload.actor.IsAlive);
            }
            else
            {
                Debug.LogWarning($"Could not find info panel for {payload.actor.DisplayName}");
            }
        }

        private IEnumerator SmoothFill(Image img, float targetFill, float speed)
        {
            while (!Mathf.Approximately(img.fillAmount, targetFill))
            {
                img.fillAmount = Mathf.MoveTowards(img.fillAmount, targetFill, speed * Time.deltaTime);
                yield return null;
            }
            img.fillAmount = targetFill;
        }

        private void HandleBattleStarted(List<IBattleActor> actors)
        {
            Debug.Log($"BattleUI received Battle Started: {actors}");
            ClearAllActorInfoPanels(); // Clear any existing from previous battles

            heroInfoArea.SetActive(true); // Show hero info area
            enemyAreaPanel.SetActive(true); // Show enemy info area

            foreach (IBattleActor actor in actors)
            {
                if (actor.IsPlayerControlled)
                {
                    InstantiateActorInfoPanel(actor, heroInfoPrefab, heroInfoArea.transform, spawnedHeroInfos);
                }
                else
                {
                    InstantiateActorInfoPanel(actor, enemyInfoPanelPrefab, enemyAreaPanel.transform, spawnedEnemyInfos);
                }
            }
            // Initial UI state setup after panels are generated
            bottomPanel.SetActive(true); // Show main action panel at the start of battle
            actionsPanel.SetActive(true);
            targetMenu.SetActive(false);
            attackMenu.SetActive(false);
            battleResultText.gameObject.SetActive(false);
            returnToOverworldButton.gameObject.SetActive(false);
        }

        private void InstantiateActorInfoPanel(IBattleActor actor, GameObject prefab, Transform parent, Dictionary<IBattleActor, GameObject> spawnedPanelsDict)
        {
            GameObject infoGO = Instantiate(prefab, parent);
            TMP_Text title = infoGO.transform.Find("Title").GetComponent<TMP_Text>();
            Image avatarImg = infoGO.transform.Find("Avatar").GetComponent<Image>();

            infoGO.name = actor.ActorName; // Use ActorName for internal lookup consistency
            title.text = actor.DisplayName;
            avatarImg.sprite = actor.Avatar;

            spawnedPanelsDict.Add(actor, infoGO); 
        }

        private void ClearAllActorInfoPanels()
        {
            foreach (var kvp in spawnedHeroInfos) { if (kvp.Value != null) Destroy(kvp.Value); }
            spawnedHeroInfos.Clear();
            foreach (var kvp in spawnedEnemyInfos) { if (kvp.Value != null) Destroy(kvp.Value); }
            spawnedEnemyInfos.Clear();
            heroInfoArea.SetActive(false);
            enemyAreaPanel.SetActive(false);
        }

        private void HandleBattleEnded(bool playerWon)
        {
            Debug.Log($"BattleUI received Battle Ended: {(playerWon ? "Win" : "Loss")}");
            attackMenu.SetActive(false);
            targetMenu.SetActive(false);
            ClearTargetButtons();
            ClearAttackButtons();
            ClearAllActorInfoPanels(); // Clear all actor info panels
            bottomPanel.SetActive(false);

            battleResultText.text = playerWon ? "VICTORY!" : "DEFEAT!";
            battleResultText.gameObject.SetActive(true);
            returnToOverworldButton.gameObject.SetActive(true);
        }

        private void HandleAvailableAttacks(List<AttackDefinition> attacks)
        {
            ClearAttackButtons(); // Clear existing attack buttons first

            if (attackButtonAreas == null || attackButtonPrefab == null)
            {
                Debug.LogError("Dynamic Attack Buttons Container or Attack Button Prefab not assigned!");
                return;
            }

            foreach (AttackDefinition attackDef in attacks)
            {
                if (attackDef == null) continue;

                GameObject buttonGO = Instantiate(attackButtonPrefab, attackButtonAreas);
                Button attackButton = buttonGO.GetComponent<Button>();
                TMP_Text buttonText = buttonGO.GetComponentInChildren<TMP_Text>();
                // Image buttonIcon = buttonGO.transform.Find("Icon").GetComponent<Image>(); // Assuming an "Icon" child

                if (attackButton != null && buttonText != null)
                {
                    buttonText.text = attackDef.attackName;
                    // buttonIcon.sprite = attackDef.attackIcon; // Use the attack's icon
                    attackButton.onClick.AddListener(() => OnAttackSelectionChosen(attackDef));
                    spawnedAttackButtons.Add(buttonGO);
                }
                else
                {
                    Debug.LogError("Attack button prefab missing Button, Text, or Icon Image component!");
                }
            }
        }

        private void HandleAvailableTargets(List<IBattleActor> targets)
        {
            ClearTargetButtons(); // Clear any existing buttons first

            foreach (IBattleActor targetActor in targets)
            {
                // Instantiate a new button from prefab
                GameObject buttonGO = Instantiate(targetButtonPrefab, targetButtonArea.transform);
                Button targetButton = buttonGO.GetComponent<Button>();
                TMP_Text buttonText = buttonGO.GetComponentInChildren<TMP_Text>();
                Image buttonImg = buttonGO.transform.Find("Border").Find("Avatar").GetComponent<Image>();

                if (targetButton != null && buttonText != null && buttonImg != null)
                {
                    buttonText.text = targetActor.DisplayName; // Set button text to enemy name
                    buttonImg.sprite = targetActor.Avatar;
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

        private void ClearAttackButtons()
        {
            foreach (GameObject buttonGO in spawnedAttackButtons)
            {
                if (buttonGO != null) Destroy(buttonGO);
            }
            spawnedAttackButtons.Clear();
        }
        private void ClearTargetButtons()
        {
            foreach (GameObject buttonGO in spawnedTargetButtons)
            {
                if (buttonGO != null) Destroy(buttonGO);
            }
            spawnedTargetButtons.Clear();
        }

        // --- Player Action Button Handlers ---

        private void OnAttackButtonClicked()

        {
            Debug.Log("Attack button clicked. Awaiting attack selection.");
            actionsPanel.SetActive(false);
            attackMenu.SetActive(true);
        }

        private void OnAttackSelectionChosen(AttackDefinition attackDef)
        {
            selectedAttackDefinition = attackDef; // Store the chosen attack
            Debug.Log($"Selected attack: {attackDef.attackName}. Now select target.");

            attackMenu.SetActive(false);
            targetMenu.SetActive(true);
            ClearAttackButtons();
        }

        private void OnCancelAttackSelection()
        {
            Debug.Log("Attack selection cancelled. Returning to main actions.");
            attackMenu.SetActive(false); 
            actionsPanel.SetActive(true); 
            ClearAttackButtons(); 
        }

        private void OnDefendButtonClicked()
        {
            playerActionChosenEvent.Raise(new PlayerAction(PlayerAction.PlayerActionType.Defend));
            actionsPanel.SetActive(false);
            targetMenu.SetActive(false); 
            ClearTargetButtons();
        }

        private void OnRunButtonClicked()
        {
            playerActionChosenEvent.Raise(new PlayerAction(PlayerAction.PlayerActionType.Run));
            actionsPanel.SetActive(false);
            targetMenu.SetActive(false); 
            ClearTargetButtons();
        }

        private void OnCancelTargetSelection()
        {
            Debug.Log("Target selection cancelled.");
            targetMenu.SetActive(false);
            actionsPanel.SetActive(true); 
            ClearTargetButtons();
        }

        private void OnTargetButtonClicked(GameObject targetGameObject)
        {
            if (selectedAttackDefinition != null && targetGameObject != null)
            {
                playerActionChosenEvent.Raise(new PlayerAction(selectedAttackDefinition, targetGameObject));
                targetMenu.SetActive(false); 
                ClearTargetButtons(); 
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