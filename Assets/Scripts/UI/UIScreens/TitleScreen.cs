using Assets.Scripts.Events;
using Assets.Scripts.Input;
using Assets.Scripts.States;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.UIScreens
{
    public class TitleScreen : UIScreen
    {
        [SerializeField] private GameStateChangeEvent gameStateRequestedEvent;


        public void OnStartPressed()
        {
            gameStateRequestedEvent?.Raise((GameState.Overworld, null));
            InputController.Instance.SetPaused(false);
        }

        public void OnQuitPressed()
        {
            Application.Quit();
        }

        public override void OnPausePressed(bool paused)
        {
            Debug.Log("pressed Pause in Title Screen");
        }
    }
}
