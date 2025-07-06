using UnityEngine;

namespace Assets.Scripts.CameraManagement
{
    /// <summary>
    /// Attaches to a Camera GameObject in a scene to register it with the CameraManager.
    /// </summary>
    [RequireComponent(typeof(Camera))] // Ensure there's a Camera component on this GameObject
    [RequireComponent(typeof(AudioListener))] // Ensure there's an AudioListener as well
    public class SceneCamera : MonoBehaviour
    {
        [Tooltip("Unique ID for this camera (e.g., 'OverworldCamera', 'BattleCamera')")]
        public string cameraID;

        private Camera _camera;
        private AudioListener _audioListener;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _audioListener = GetComponent<AudioListener>();

            // Ensure they are initially disabled to prevent conflicts before CameraManager takes over
            _camera.enabled = false;
            _audioListener.enabled = false;
            gameObject.tag = "Untagged"; // Ensure it's not "MainCamera" initially
        }

        void OnEnable()
        {
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.RegisterCamera(cameraID, _camera);
            }
            else
            {
                Debug.LogError("CameraManager not found! Camera will not be registered correctly.", this);
            }
        }

        void OnDisable()
        {
            // When this GameObject or its scene is disabled/unloaded, unregister the camera
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.UnregisterCamera(cameraID);
            }
        }
    }
}