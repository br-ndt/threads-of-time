using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Animation Settings")]
    public float moveSpeed = 1.0f;
    public float fadeSpeed = 1.0f;
    public float randomHorizontalIntensity = 0.2f;

    private TextMeshProUGUI textMesh;
    private Vector3 moveDirection;
    private Color textColor;
    private float initialLifetime;
    private float lifetime;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {
            Debug.LogError("FloatingText requires a TextMeshProUGUI component!", this);
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * moveDirection;

        lifetime -= Time.deltaTime;

        if (lifetime < initialLifetime)
        {
            float alpha = Mathf.Clamp01(lifetime / initialLifetime);
            textColor.a = alpha;
            textMesh.color = textColor;
        }

        // if (lifetime <= 0)
        // {
        //     gameObject.SetActive(false);
        // }
    }

    /// <summary>
    /// Initializes the floating text with its value, color, and duration.
    /// </summary>
    public void Init(string text, Color color, float duration)
    {
        textMesh.text = text;
        textColor = color;
        textMesh.color = textColor;

        initialLifetime = duration;
        lifetime = duration;

        float randomX = Random.Range(-randomHorizontalIntensity, randomHorizontalIntensity);
        moveDirection = new Vector3(randomX, 1, 0).normalized;

        gameObject.SetActive(true);
    }
}