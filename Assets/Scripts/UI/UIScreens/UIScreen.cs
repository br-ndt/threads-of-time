using Assets.Scripts.Events;
using Assets.Scripts.States;
using System;
using UnityEngine;

namespace Assets.Scripts.UI.UIScreens
{
    [Serializable]
    public abstract class UIScreen : MonoBehaviour
    {
        protected CanvasGroup _canvasGroup;

        [SerializeField] private PauseEvent pauseEvent;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            pauseEvent.OnEventRaised += OnPausePressed;
        }

        private void OnDisable()
        {
            pauseEvent.OnEventRaised -= OnPausePressed;
        }

        public abstract void OnPausePressed(bool paused);

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }

}