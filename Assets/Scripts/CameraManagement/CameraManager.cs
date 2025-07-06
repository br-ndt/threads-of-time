using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.States;
using Assets.Scripts.Configs;

namespace Assets.Scripts.CameraManagement
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        // A dictionary to hold all registered cameras by a unique ID (e.g., scene name)
        private readonly Dictionary<string, Camera> registeredCameras = new();

        [Header("Default Camera for Scene Activation")]
        [SerializeField] private string overworldCameraID = "OverworldCamera"; // ID of the camera to activate when overworld scene is active
        [SerializeField] private string battleCameraID = "BattleCamera";       // ID of the camera to activate when battle scene is active
        [SerializeField] private string cutsceneCameraID = "CutsceneCamera";   // ID of the camera to activate when cutscene scene is active
        [SerializeField] private GameStateMachine gameBrain;
        public Camera ActiveCamera { get; private set; }

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

        // --- Public Methods to Register and Control Cameras ---

        /// <summary>
        /// Registers a camera with the manager.
        /// </summary>
        /// <param name="cameraID">A unique identifier for this camera (e.g., "OverworldCamera", "BattleCamera").</param>
        /// <param name="cam">The Camera component to register.</param>
        public void RegisterCamera(string cameraID, Camera cam)
        {
            if (registeredCameras.ContainsKey(cameraID))
            {
                Debug.LogWarning($"Camera with ID '{cameraID}' already registered. Overwriting.");
                registeredCameras[cameraID] = cam;
            }
            else
            {
                registeredCameras.Add(cameraID, cam);
            }
            Debug.Log($"Camera '{cameraID}' registered.");

            // Immediately disable newly registered cameras to prevent conflicts until explicitly activated
            SetCameraActiveState(cam, false);
        }

        /// <summary>
        /// Unregisters a camera. Call this when a scene containing the camera is unloaded.
        /// </summary>
        /// <param name="cameraID">The unique identifier of the camera to unregister.</param>
        public void UnregisterCamera(string cameraID)
        {
            if (registeredCameras.ContainsKey(cameraID))
            {
                // If the camera being unregistered is currently active, clear the active camera reference
                if (ActiveCamera != null && ActiveCamera == registeredCameras[cameraID])
                {
                    ActiveCamera = null;
                }
                registeredCameras.Remove(cameraID);
                Debug.Log($"Camera '{cameraID}' unregistered.");
            }
        }

        /// <summary>
        /// Activates a specific camera by its ID, deactivating all others.
        /// </summary>
        /// <param name="cameraID">The ID of the camera to activate.</param>
        public void ActivateCamera(string cameraID)
        {
            if (!registeredCameras.ContainsKey(cameraID))
            {
                Debug.LogWarning($"Camera with ID '{cameraID}' not found in registered cameras.");
                return;
            }

            Camera targetCam = registeredCameras[cameraID];

            if (ActiveCamera == targetCam)
            {
                // Already active, no need to do anything
                return;
            }

            Debug.Log($"Activating camera: {cameraID}");

            // Deactivate the currently active camera, if any
            if (ActiveCamera != null)
            {
                SetCameraActiveState(ActiveCamera, false);
            }

            // Activate the target camera
            SetCameraActiveState(targetCam, true);
            ActiveCamera = targetCam;

            // Optional: Smooth camera transition (e.g., using Cinemachine blend) could go here
            // For now, it's an instant switch.
        }

        /// <summary>
        /// Helper to enable/disable camera component, its GameObject, and AudioListener.
        /// Also manages the "MainCamera" tag.
        /// </summary>
        private void SetCameraActiveState(Camera cam, bool isActive)
        {
            if (cam == null) return;

            // Deactivate/activate the Camera component
            cam.enabled = isActive;

            // Manage the AudioListener (should be on the same GameObject as the active camera)
            if (cam.TryGetComponent<AudioListener>(out var listener))
            {
                listener.enabled = isActive;
            }

            // Set/remove the "MainCamera" tag
            if (isActive)
            {
                cam.gameObject.tag = "MainCamera";
            }
            else if (cam.gameObject.CompareTag("MainCamera"))
            {
                // Only remove the tag if it was set by us. Avoid removing other main cameras if they exist.
                cam.gameObject.tag = "Untagged";
            }

            // Optionally, deactivate/activate the entire GameObject if the camera is the only component
            // cam.gameObject.SetActive(isActive); // This might break other components on the same GO, so be careful.
            // Best to only enable/disable the Camera and AudioListener components.
        }

        // --- Listening to GameState Changes to Activate Correct Camera ---
        private void OnEnable()
        {
            // Subscribe to the GameStateChangeEvent from your events system
            // You'll need to assign your GameStateChangeEvent ScriptableObject here in the Inspector
            if (gameBrain != null) // Check if GameStateMachine is initialized
            {
                gameBrain.OnStateChanged += HandleGameStateChange;
            }
            // else, you might want to find the event channel via Resources.Load or assign it in inspector
        }

        private void OnDisable()
        {
            if (gameBrain != null)
            {
                gameBrain.OnStateChanged -= HandleGameStateChange;
            }
        }

        private void HandleGameStateChange(GameState newState, GameConfig config)
        {
            switch (newState)
            {
                case GameState.Overworld:
                    ActivateCamera(overworldCameraID);
                    break;
                case GameState.Battle:
                    ActivateCamera(battleCameraID);
                    break;
                case GameState.Cutscene:
                    ActivateCamera(cutsceneCameraID);
                    break;
                // Could add cases for other states (e.g., Menu, TitleScreen)
                // Or existing cases may handle for multiple cameras, etc
                case GameState.Loading:
                // During loading, keep the previous camera active
                default:
                    break;
            }
        }
    }
}