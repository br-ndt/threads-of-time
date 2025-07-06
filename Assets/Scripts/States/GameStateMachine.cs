using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Assets.Scripts.Events;
using Assets.Scripts.Configs;

namespace Assets.Scripts.States
{
    public class GameStateMachine : MonoBehaviour
    {
        public static GameStateMachine Instance { get; private set; }

        [Header("Event Channels")]
        [SerializeField]
        private GameStateChangeEvent gameStateChangeEvent; // Event to request state changes
        public event System.Action<GameState, GameConfig> OnStateChanged;

        [Header("Scene References")]
        [SerializeField] string overworld = "Overworld";
        [SerializeField] string battle = "Battle";
        [SerializeField] string cutscene = "Cutscene";

        // Current state and its associated configuration
        public GameState CurrentState { get; private set; } = GameState.None;
        public GameConfig CurrentConfig { get; private set; }

        // Fading & Loading Screen
        [Header("Loading Screen")]
        [SerializeField] private GameObject loadingScreenPanel;
        [SerializeField] private float fadeDuration = 0.5f;

        private CanvasGroup loadingScreenCanvasGroup;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Ensure only one instance exists
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Keep this manager alive across scenes

                if (loadingScreenPanel != null)
                {
                    loadingScreenCanvasGroup = loadingScreenPanel.GetComponent<CanvasGroup>();
                    if (loadingScreenCanvasGroup == null)
                    {
                        loadingScreenCanvasGroup = loadingScreenPanel.AddComponent<CanvasGroup>();
                    }
                    loadingScreenPanel.SetActive(false); // Start with loading screen hidden
                }
                else
                {
                    Debug.LogWarning("Loading Screen Panel not assigned to GameStateMachine.");
                }
            }
        }

        private void OnEnable()
        {
            if (gameStateChangeEvent != null)
            {
                gameStateChangeEvent.OnStateChangeRequested += HandleStateChangeRequest;
            }
        }

        private void OnDisable()
        {
            if (gameStateChangeEvent != null)
            {
                gameStateChangeEvent.OnStateChangeRequested -= HandleStateChangeRequest;
            }
        }

        private void Start()
        {
            // Initial state setup (e.g., load the overworld scene at start)
            if (CurrentState == GameState.None)
            {
                TransitionToState(GameState.Overworld); // Or GameState.TitleScreen
            }
        }

        // --- Public method to request a state transition ---
        public void TransitionToState(GameState newState, GameConfig config = null)
        {
            if (CurrentState == newState && config == CurrentConfig)
            {
                Debug.LogWarning($"Already in {newState} with same config. Ignoring transition request.");
                return;
            }

            Debug.Log($"Requesting transition from {CurrentState} to {newState}");
            StartCoroutine(PerformTransition(newState, config));
        }

        private IEnumerator PerformTransition(GameState newState, GameConfig config)
        {
            // 1. Set current state to Loading and show loading screen
            CurrentState = GameState.Loading;
            CurrentConfig = config; // Store the config for the new state to access

            yield return FadeLoadingScreen(true); // Fade In loading screen

            // 2. Unload previous dynamic scene (Battle, Cutscene) if active
            yield return UnloadCurrentDynamicScene(newState);

            // 3. Handle scene loading/unloading based on new state
            yield return LoadNewSceneForState(newState);

            // 4. Update and announce the new state
            CurrentState = newState;
            Debug.Log($"Transitioned to new state: {CurrentState}");

            // Let other systems know the state has changed (optional, can be done by listeners directly)
            // gameStateChangeEvent.Raise(CurrentState, CurrentConfig); // This could cause an infinite loop if listened to by this manager.
            // Instead, rely on direct scene/system listeners to pick up the state.
            OnStateChanged?.Invoke(CurrentState, CurrentConfig);

            yield return FadeLoadingScreen(false); // Fade Out loading screen
        }

        private IEnumerator FadeLoadingScreen(bool fadeIn)
        {
            if (loadingScreenPanel == null || loadingScreenCanvasGroup == null)
            {
                yield break; // No loading screen to fade
            }

            loadingScreenPanel.SetActive(true);
            float startAlpha = fadeIn ? 0f : 1f;
            float endAlpha = fadeIn ? 1f : 0f;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                loadingScreenCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
                yield return null;
            }
            loadingScreenCanvasGroup.alpha = endAlpha;

            if (!fadeIn)
            {
                loadingScreenPanel.SetActive(false);
            }
        }

        private IEnumerator UnloadCurrentDynamicScene(GameState newState)
        {
            Debug.Log($"Unloading previous {newState} scenes...");
            // Unload the previous additive scene if it was a battle or cutscene
            if (newState == GameState.Battle && SceneManager.GetSceneByName(battle).isLoaded)
            {
                Debug.Log($"Unloading {battle}...");
                yield return SceneManager.UnloadSceneAsync(battle);
            }
            else if (newState == GameState.Cutscene && SceneManager.GetSceneByName(cutscene).isLoaded)
            {
                Debug.Log($"Unloading {cutscene}...");
                yield return SceneManager.UnloadSceneAsync(cutscene);
            }
            // Add other additive scenes to unload here if necessary
        }

        private IEnumerator LoadNewSceneForState(GameState newState)
        {
            string sceneToLoad;
            LoadSceneMode loadMode;
            switch (newState)
            {
                case GameState.Overworld:
                    // If moving *back* to overworld from a dynamic scene, overworld should already be loaded.
                    // If starting from scratch, it would be loaded here.
                    // We might need to ensure only the overworld is active if coming from a full unload.
                    sceneToLoad = overworld;
                    loadMode = LoadSceneMode.Single; // Load overworld as the only scene initially
                    if (SceneManager.GetSceneByName(sceneToLoad).isLoaded)
                    {
                        Debug.Log("Overworld scene already loaded. Activating it.");
                        SceneManager.SetActiveScene(SceneManager.GetSceneByName(overworld));
                        yield break; // No need to reload
                    }
                    break;
                case GameState.Battle:
                    sceneToLoad = battle;
                    loadMode = LoadSceneMode.Additive;
                    break;
                case GameState.Cutscene:
                    sceneToLoad = cutscene;
                    loadMode = LoadSceneMode.Additive;
                    break;
                case GameState.Menu:
                    // Load a menu scene additively if not already loaded
                    // You might have multiple menu scenes or a single one with different panels
                    // sceneToLoad = "MenuScene"; loadMode = LoadSceneMode.Additive;
                    yield break; // For now, no scene loading for Menu state
                case GameState.GameOver:
                    // sceneToLoad = gameOver; loadMode = LoadSceneMode.Additive;
                    yield break;
                // case GameState.TitleScreen:
                //     sceneToLoad = title; // Assume this is a single scene load
                //     loadMode = LoadSceneMode.Single;
                //     break;
                default:
                    Debug.LogWarning($"Unhandled GameState: {newState}. No scene loaded.");
                    yield break;
            }
            Debug.Log(sceneToLoad);

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                Debug.Log($"Loading scene: {sceneToLoad} ({loadMode})");
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, loadMode);

                // Wait until the asynchronous scene fully loads
                while (!asyncLoad.isDone)
                {
                    // Update progress bar here if you have one
                    // Debug.Log($"Loading progress: {asyncLoad.progress * 100}%");
                    yield return null;
                }

                // If loading additively, set the newly loaded scene as active.
                // This is important for objects created in that scene to receive events properly.
                if (loadMode == LoadSceneMode.Additive)
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
                }
                // If Single, the new scene is already active by default.
            }
            else
            {
                Debug.LogWarning($"Unhandled GameState: {newState}. No scene loaded.");
            }
        }

        // Listener for the GameStateChangeEvent.
        // This is primarily for debugging or systems that *react* to state changes.
        // The core transition logic is handled by the private PerformTransition coroutine.
        private void HandleStateChangeRequest(GameState newState, GameConfig config)
        {
            // The TransitionToState method already handles the logic.
            // This listener could be used by other managers (e.g., UI Manager)
            // to show/hide specific UI elements based on the state.
            Debug.Log($"Game State Change Requested to: {newState}. Config provided: {config != null}");
            TransitionToState(newState, config);
            // We call TransitionToState directly, not via the event in this manager.
            // This prevents an infinite loop if this manager also raised the event.
        }
    }
}