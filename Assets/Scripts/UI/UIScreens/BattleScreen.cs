using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.UIScreens
{
    public class BattleScreen : UIScreen
    {
        [SerializeField] private GameObject battleUI;
        [SerializeField] private GameObject pausedUI;
        [SerializeField] private TextMeshProUGUI pauseText;

        private void Awake()
        {
            pausedUI.SetActive(false);        }

        public override void OnPausePressed(bool paused)
        {
            pausedUI.SetActive(paused);
            Time.timeScale = paused ? 0f : 1f;
        }
    }
}
