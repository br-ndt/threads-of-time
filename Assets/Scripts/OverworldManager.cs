using UnityEngine;
using Assets.Scripts.States;
using Assets.Scripts.Events;
using Assets.Scripts.Configs;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.TextCore.Text;
using Assets.Scripts.Audio;

public class OverworldManager : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private GameStateChangeEvent requestGameStateChange;
    [SerializeField] private GameStateChangeEvent gameStateChanged;
    [SerializeField] private ConversationStartEvent conversationStartEvent;
    [SerializeField] private ConversationEndEvent conversationEndEvent;

    [Header("Battle & Cutscene Configurations")]
    [SerializeField] private List<BattleConfig> possibleBattles;
    [SerializeField] private CutsceneConfig specificCutsceneConfig;

    [Header("Player Settings")]
    [Tooltip("How fast the player moves from one tile to the next.")]
    public float moveSpeed = 5f;

    [Tooltip("The size of one tile in the grid. Should match your tilemap grid size.")]
    public float tileSize = 1f;

    // Player state variables
    [SerializeField] private GameObject player;
    private bool canMove = false;
    private bool isMoving = false;
    private Vector3 targetPosition;
    private SpriteCharacter2D spriteCharacter;

    void OnEnable()
    {
        if (gameStateChanged != null)
        {
            gameStateChanged.OnEventRaised += HandleGameStateChange;
        }
        if (requestGameStateChange != null)
        {
            requestGameStateChange.OnEventRaised += HandleGameStateRequested;
        }
        if (conversationStartEvent != null)
        {
            conversationStartEvent.OnEventRaised += HandleConversationStart;
        }
        if (conversationEndEvent != null)
        {
            conversationEndEvent.OnEventRaised += HandleConversationEnd;
        }
    }

    void OnDisable()
    {
        if (gameStateChanged != null)
        {
            gameStateChanged.OnEventRaised -= HandleGameStateChange;
        }
        if (requestGameStateChange != null)
        {
            requestGameStateChange.OnEventRaised -= HandleGameStateRequested;
        }
        if (conversationStartEvent != null)
        {
            conversationStartEvent.OnEventRaised -= HandleConversationStart;
        }
        if (conversationEndEvent != null)
        {
            conversationEndEvent.OnEventRaised -= HandleConversationEnd;
        }
    }

    void Awake()
    {
        spriteCharacter = player.GetComponentInChildren<SpriteCharacter2D>();
    }

    void Start()
    {
        player.transform.position = RoundToNearestTile(player.transform.position);
    }

    void Update()
    {
        if (!isMoving && canMove)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            if (horizontalInput != 0)
            {
                targetPosition = player.transform.position + new Vector3(horizontalInput * tileSize, 0, 0);
                spriteCharacter.isFlipped = horizontalInput < 0;
                StartCoroutine(MovePlayer());
            }
            else if (verticalInput != 0)
            {
                targetPosition = player.transform.position + new Vector3(0, verticalInput * tileSize, 0);
                StartCoroutine(MovePlayer());
            }
        }
    }

    /// <summary>
    /// Event listener for game state change being requested. We listen to this so we can quickly stop any interactions if we're leaving the scene
    /// </summary>
    private void HandleGameStateRequested((GameState state, GameConfig config) payload)
    {
        if (payload.state != GameState.Overworld)
        {
            StopMovingImmediately();
        }
    }

    /// <summary>
    /// Event listener for game state change
    /// </summary>
    private void HandleGameStateChange((GameState state, GameConfig config) payload)
    {
        if (payload.state == GameState.Overworld)
        {
            canMove = true;
        }
    }

    private void HandleConversationStart(ConversationConfig config)
    {
        StopMovingImmediately();
    }

    private void HandleConversationEnd()
    {
        canMove = true;
    }

    private void StopMovingImmediately()
    {
        canMove = false;
        isMoving = false;
        spriteCharacter.Play(BattleSpriteState.Idle);
        StopAllCoroutines();
    }

    /// <summary>
    /// A coroutine that smoothly moves the player from its current position to the target position.
    /// </summary>
    private IEnumerator MovePlayer()
    {
        isMoving = true;
        spriteCharacter.Play(BattleSpriteState.Walk);

        // TODO check for collision

        while (Vector3.Distance(player.transform.position, targetPosition) > 0.01f)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        player.transform.position = targetPosition;

        // check if there's more input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput == 0 && verticalInput == 0)
        {
            isMoving = false;
            spriteCharacter.Play(BattleSpriteState.Idle);
        }
        else
        {
            // Immediately queue next move
            if (horizontalInput != 0)
            {
                targetPosition = player.transform.position + new Vector3(horizontalInput * tileSize, 0, 0);
                spriteCharacter.isFlipped = horizontalInput < 0;
                StartCoroutine(MovePlayer());
            }
            else if (verticalInput != 0)
            {
                targetPosition = player.transform.position + new Vector3(0, verticalInput * tileSize, 0);
                StartCoroutine(MovePlayer());
            }
        }
    }

    /// <summary>
    /// Helper function to snap a position to the center of the nearest tile.
    /// </summary>
    private Vector3 RoundToNearestTile(Vector3 pos)
    {
        float x = Mathf.Round(pos.x / tileSize) * tileSize;
        float y = Mathf.Round(pos.y / tileSize) * tileSize;
        return new Vector3(x, y, pos.z);
    }
}