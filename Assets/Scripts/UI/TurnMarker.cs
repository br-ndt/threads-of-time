using Assets.Scripts.Combat;
using Assets.Scripts.Configs;
using Assets.Scripts.Events;
using UnityEngine;

public class TurnMarker : MonoBehaviour
{
    public float amplitude = 0.2f;
    public float frequency = 5f;

    public float rotateAmplitude = 60f;
    public float rotateFrequency = 1.5f;

    private Vector3 startPos;
    private Quaternion startRot;
    [SerializeField] private ActorTurnEvent actorTurnEvent; // Raises when an actor's turn begins
    [SerializeField] private BattleEndEvent battleEndEvent; // Raises when an actor's turn begins
    private IBattleActor actor;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        actor = GetComponentInParent<IBattleActor>();
    }

    private void OnEnable()
    {
        if (actorTurnEvent != null)
        {
            actorTurnEvent.OnEventRaised += HandleTurnStart;
        }
        if (battleEndEvent != null)
        {
            battleEndEvent.OnEventRaised += HandleBattleEnd;
        }
    }

    private void OnDisable()
    {
        if (actorTurnEvent != null)
        {
            actorTurnEvent.OnEventRaised -= HandleTurnStart;
        }
        if (battleEndEvent != null)
        {
            battleEndEvent.OnEventRaised -= HandleBattleEnd;
        }
    }

    private void HandleTurnStart((IBattleActor actor, bool starting) payload)
    {
        spriteRenderer.enabled = payload.actor == actor;
    }

    private void HandleBattleEnd(bool playerWon)
    {
        spriteRenderer.enabled = false;
    }

    private void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    private void Update()
    {
        // Bob up and down
        float offsetY = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = startPos + new Vector3(0f, offsetY, 0f);

        // Sway rotation around local Y
        float sway = Mathf.Sin(Time.time * rotateFrequency) * rotateAmplitude;
        transform.localRotation = startRot * Quaternion.Euler(0f, sway, 0f);
    }
}
