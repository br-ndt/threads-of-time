using UnityEngine;

namespace Assets.Scripts.UI.UIScreens
{
    public class OverworldScreen : UIScreen
    {
        public override void OnPausePressed(bool paused)
        {
            Debug.Log("pressed Pause in Overworld Screen");
        }
    }

}
