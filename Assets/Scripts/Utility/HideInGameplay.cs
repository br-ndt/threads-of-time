using UnityEngine;


namespace Assets.Scripts.Utility
{
    public class HideInGameplay : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}
