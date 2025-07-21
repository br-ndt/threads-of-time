using UnityEngine;
using Assets.Scripts.Events;
using Assets.Scripts.Configs;

namespace Assets.Scripts.Input
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance { get; private set; }

        [SerializeField] private PauseEvent pauseEvent;

        private DefaultInput input;

        private bool isPaused = false;

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

        public void SetPaused(bool paused)
        {
            isPaused = paused;
        }

        private void Start()
        {
            input = new DefaultInput();
            input.Enable();


            input.Player.Pause.performed += _ =>
            {
                isPaused = !isPaused;
                pauseEvent.Raise(isPaused);
            };
        }



    }
}