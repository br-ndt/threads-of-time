using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Assets.Scripts.Events;
using Assets.Scripts.Configs;
using UnityEngine.Experimental.GlobalIllumination;

namespace Assets.Scripts.States
{
    public class GameStateMachine : MonoBehaviour
    {
        public static GameStateMachine Instance { get; private set; }

        [Header("Event Channels")]
        [SerializeField]
        private GameStateChangeEvent gameStateRequestedEvent; // Raised by others, to request a state change
        [SerializeField]
        private GameStateChangeEvent gameStateChangedEvent; // Raised here, to notify others that the state change has occurred

        [Header("Scene References")]
        [SerializeField] string overworld = "Overworld";
        [SerializeField] string battle = "Battle";
        [SerializeField] string cutscene = "Cutscene";
        [SerializeField] string[] dynamicSceneKeys = new string[] { "Battle", "Cutscene" };

        // Current state and its associated configuration
        public GameState CurrentState { get; private set; } = GameState.None;
        public GameConfig CurrentConfig { get; private set; }

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
            }
        }

        private void OnEnable()
        {
            if (gameStateRequestedEvent != null)
            {
                gameStateRequestedEvent.OnEventRaised += HandleStateChangeRequest;
            }
        }

        private void OnDisable()
        {
            if (gameStateRequestedEvent != null)
            {
                gameStateRequestedEvent.OnEventRaised -= HandleStateChangeRequest;
            }
        }

        private void Start()
        {
            // Initial state setup (e.g., load the overworld scene at start)
            StartCoroutine(PerformTransition(GameState.TitleScreen, null));
        }

        // Listener for the GameStateChangeEvent.
        private void HandleStateChangeRequest((GameState, GameConfig) stateAndConfig)
        {
            Debug.Log($"Game State Change Requested to: {stateAndConfig.Item1}. Config provided: {stateAndConfig.Item2 != null}");
            StartCoroutine(PerformTransition(stateAndConfig.Item1, stateAndConfig.Item2));
            // We call TransitionToState directly, not via the event in this manager.
            // This prevents an infinite loop if this manager also raised the event.
        }

        private IEnumerator PerformTransition(GameState newState, GameConfig config)
        {
            // 1. Set current state to Loading and show loading screen
            CurrentState = GameState.Loading;
            CurrentConfig = config; // Store the config for the new state to access

            // 2. Unload previous dynamic scene (Battle, Cutscene) if active
            yield return UnloadDynamicScenes();

            // 3. Handle scene loading/unloading based on new state
            yield return LoadNewSceneForState(newState);

            // 4. Update and announce the new state
            CurrentState = newState;
            Debug.Log($"Transitioned to new state: {CurrentState}");

            // Let other systems know the state has changed (optional, can be done by listeners directly)
            // gameStateChangeEvent.Raise(CurrentState, CurrentConfig); // This could cause an infinite loop if listened to by this manager.
            // Instead, rely on direct scene/system listeners to pick up the state.
            gameStateChangedEvent.Raise((CurrentState, CurrentConfig));
        }

        private IEnumerator UnloadDynamicScenes()
        {
            foreach (string sceneToUnload in dynamicSceneKeys)
            {
                if (SceneManager.GetSceneByName(sceneToUnload).isLoaded)
                    yield return SceneManager.UnloadSceneAsync(sceneToUnload);
            }
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
                    // sceneToLoad = "MenuScene"; loadMode = LoadSceneMode.Additive;
                    yield break; // For now, no scene loading for Menu state
                case GameState.GameOver:
                    // sceneToLoad = "GameOver"; loadMode = LoadSceneMode.Additive;
                    yield break;
                // case GameState.TitleScreen:
                //     sceneToLoad = "Title";
                //     loadMode = LoadSceneMode.Single;
                //     break;
                default:
                    Debug.LogWarning($"Unhandled GameState: {newState}. No scene loaded.");
                    yield break;
            }

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                Debug.Log($"Loading scene: {sceneToLoad} ({loadMode})");
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, loadMode);

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
    }
}