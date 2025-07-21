using Assets.Scripts.States;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI.UIScreens
{
    public class LoadingScreen : UIScreen
    {
        [SerializeField] private float fadeDuration = 0.5f;

        public override void Show()
        {
            if (_canvasGroup == null)
                return;

            _canvasGroup.alpha = 1f;
            base.Show();
        }

        public override void Hide()
        {
            if (gameObject.activeSelf)
                StartCoroutine(FadeOutAndHide());
        }

        private IEnumerator FadeOutAndHide()
        {
            if (_canvasGroup == null)
                yield break;

            float timer = 0f;
            float start = 1f;
            float end = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(start, end, timer / fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = end;
            base.Hide();
        }

        public override void OnPausePressed(bool paused)
        {
            Debug.Log("pressed Pause in Loading Screen");
        }
    }
}
