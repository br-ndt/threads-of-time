using Assets.Scripts.Events;
using Assets.Scripts.States;
using Assets.Scripts.Configs;
using Assets.Scripts.UI.UIScreens;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using System.Collections.Generic;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameStateChangeEvent _gameStateRequestedEvent;
        [SerializeField] private GameStateChangeEvent _gameStateChangedEvent;

        [SerializeField] private UIScreen titleScreen;
        [SerializeField] private UIScreen bootstrapScreen;
        [SerializeField] private UIScreen battleScreen;
        [SerializeField] private UIScreen overworldScreen;
        [SerializeField] private UIScreen loadingScreen;

        private List<UIScreen> allScreens = new();

        private void Awake()
        {
            loadingScreen.gameObject.SetActive(true);

            allScreens.Add(titleScreen);
            allScreens.Add(bootstrapScreen);
            allScreens.Add(battleScreen);
            allScreens.Add(overworldScreen);
            allScreens.Add(loadingScreen);
        }

        private void Start()
        {
            foreach (var s in allScreens)
            {
                s.Hide();
            }

            SwitchTo(loadingScreen);
        }

        private void OnEnable()
        {
            _gameStateRequestedEvent.OnEventRaised += HandleGameStateRequested;
            _gameStateChangedEvent.OnEventRaised += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            _gameStateRequestedEvent.OnEventRaised -= HandleGameStateRequested;
            _gameStateChangedEvent.OnEventRaised -= HandleGameStateChanged;

        }

        private void HandleGameStateRequested((GameState newState, GameConfig config) paylod)
        {
            foreach (var s in allScreens)
            {
                s.Hide();
            }

            SwitchTo(loadingScreen);
        }

        private void HandleGameStateChanged((GameState newState, GameConfig config) payload)
        {
            UIScreen nextScreen;

            switch (payload.newState)
            {
                case (GameState.TitleScreen):
                    nextScreen = titleScreen;
                    break;
                case (GameState.Menu):
                    nextScreen = bootstrapScreen;
                    break;
                case (GameState.Cutscene):
                    nextScreen = loadingScreen;
                    break;
                case (GameState.Battle):
                    nextScreen = battleScreen;
                    break;
                case GameState.Overworld:
                    nextScreen = overworldScreen;
                    break;
                case GameState.GameOver:
                case GameState.None:
                default:
                    nextScreen = null;
                    break;
            }

            SwitchTo(nextScreen);
        }

        public void SwitchTo(UIScreen screen)
        {
            foreach (var s in allScreens)
            {
                s.Hide();
            }

            var next = screen;

            if (next != null)
            {
                Debug.Log($"Switching to UIScreen: {screen}");
                next.Show();
            }
            else
            {
                Debug.LogWarning($"No UIScreen of type {screen} found.");
            }
        }

    }


}

