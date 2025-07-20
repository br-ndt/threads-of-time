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
        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
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