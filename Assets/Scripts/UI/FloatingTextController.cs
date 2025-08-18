using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.Events;
using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    [Tooltip("The prefab for the floating text UI element.")]
    private GameObject floatingTextPrefab;

    [SerializeField]
    [Tooltip("The world-space canvas that will hold the text objects.")]
    private Transform textContainer;

    [Header("Settings")]
    [SerializeField]
    [Tooltip("The number of text objects to create initially.")]
    private int initialPoolSize = 20;

    [SerializeField]
    [Tooltip("The default duration the text stays on screen.")]
    private float defaultDuration = 1.0f;

    [SerializeField]
    [Tooltip("The vertical offset to apply to the actor's position.")]
    private float verticalOffset = 1.0f;

    [SerializeField]
    [Tooltip("The default color for damage text.")]
    private Color defaultDamageColor = Color.red;

    [SerializeField]
    [Tooltip("The default color for healing text.")]
    private Color defaultHealColor = Color.green;

    [SerializeField] private DamageTakenEvent damageTakenEvent;

    private Queue<FloatingText> textPool = new Queue<FloatingText>();

    public static FloatingTextController Instance { get; private set; }
    private Vector3 offset;

    void OnEnable()
    {
        if (damageTakenEvent != null)
        {
            damageTakenEvent.OnEventRaised += HandleDamageTaken;
        }
    }

    void OnDisable()
    {
        if (damageTakenEvent != null)
        {
            damageTakenEvent.OnEventRaised -= HandleDamageTaken;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        offset = new Vector3(0, verticalOffset, 0);
    }

    void Start()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateAndPoolTextObject();
        }
    }

    private void HandleDamageTaken((IBattleActor actor, float damage) payload)
    {
        FloatingText floatingText = GetPooledText();

        floatingText.transform.position = payload.actor.GameObject.transform.position + offset;
        Color color = (payload.damage >= 0) ? defaultDamageColor : defaultHealColor;

        floatingText.Init(Mathf.Floor(payload.damage).ToString(), color, defaultDuration);
    }

    /// <summary>
    /// Creates a new floating text object and adds it to the pool.
    /// </summary>
    private void CreateAndPoolTextObject()
    {
        GameObject textObject = Instantiate(floatingTextPrefab, textContainer);
        FloatingText floatingText = textObject.GetComponent<FloatingText>();
        textObject.SetActive(false); // Start inactive
        textPool.Enqueue(floatingText);
    }

    /// <summary>
    /// Gets an available text object from the pool.
    /// </summary>
    private FloatingText GetPooledText()
    {
        if (textPool.Count == 0)
        {
            CreateAndPoolTextObject();
        }

        return textPool.Dequeue();
    }

    public enum TextType { Damage, Heal }
}