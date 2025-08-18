using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ConditionIconUI : MonoBehaviour
{
    private Image _iconImage;

    private void Awake()
    {
        _iconImage = GetComponent<Image>();
    }

    public void SetCondition(Sprite iconSprite)
    {
        if (iconSprite != null)
        {
            _iconImage.sprite = iconSprite;
            _iconImage.enabled = true;
        }
        else
        {
            _iconImage.enabled = false;
        }
    }
}