using UnityEngine;

public class HideInGameplay : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
}
