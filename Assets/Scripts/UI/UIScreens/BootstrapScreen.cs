using UnityEngine;

namespace Assets.Scripts.UI.UIScreens
{
    public class BootstrapScreen: UIScreen
    {

        public override void OnPausePressed(bool paused)
        {
            Debug.Log("pressed Pause in Bootstrap Screen");
        }
    }

}
