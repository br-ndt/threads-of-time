using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Assets.Scripts.Events;
using Assets.Scripts.Configs;
using System.Collections.Generic;
using Assets.Scripts.States; // Required for TextMeshPro

/// <summary>
/// Manages the Conversation UI, including displaying dialogue, speaker names, and handling user input.
/// </summary>
public class ConversationUI : MonoBehaviour
{
    [SerializeField] ConversationStartEvent conversationStartEvent;
    [SerializeField] ConversationProgressEvent conversationProgressEvent;
    [SerializeField] ConversationEndEvent conversationEndEvent;
    [SerializeField] private GameStateChangeEvent requestGameStateChange;
    // --- UI ELEMENT REFERENCES ---
    // Assign these in the Unity Inspector
    [Header("UI Elements")]
    [Tooltip("The main container for the entire dialogue UI.")]
    public GameObject dialogueContainer;

    [Tooltip("The Image element for displaying the speaker's avatar.")]
    public Image avatar;

    [Tooltip("The TextMeshProUGUI element for displaying the speaker's name.")]
    public TextMeshProUGUI speakerNameText;

    [Tooltip("The TextMeshProUGUI element for displaying the dialogue content.")]
    public TextMeshProUGUI dialogueText;

    [Tooltip("The Button the player clicks to advance the conversation.")]
    public Button continueButton;

    [Header("Typing Effect Settings")]
    [Tooltip("The speed at which the text types out, in characters per second.")]
    [Range(1f, 100f)]
    public float typingSpeed = 50f;

    [SerializeField] private Sprite fallbackAvatar;

    private ConversationConfig currentConfig;
    private int currentSpeakerIndex = 0;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private void OnEnable()
    {
        conversationStartEvent.OnEventRaised += HandleConversationStart;
    }

    private void OnDisable()
    {
        conversationStartEvent.OnEventRaised -= HandleConversationStart;
    }

    private void Start()
    {
        // Ensure the dialogue UI is hidden at the start of the game.
        if (dialogueContainer != null)
        {
            dialogueContainer.SetActive(false);
        }

        // Add a listener to the continue button to call the DisplayNextLine method when clicked.
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(DisplayNextLine);
        }
    }

    /// <summary>
    /// Starts a new conversation.
    /// </summary>
    /// <param name="speakerName">The name of the character speaking.</param>
    /// <param name="dialogueLines">An array of strings, where each string is a line of dialogue.</param>
    public void HandleConversationStart(ConversationConfig config)
    {
        if (config.speakers == null || config.speakers.Count == 0)
        {
            Debug.LogError("Cannot start conversation with no dialogue lines.");
            return;
        }

        // Set the conversation data
        currentConfig = config;

        // Update the speaker name
        if (speakerNameText != null)
        {
            speakerNameText.text = config.speakers[currentSpeakerIndex].Name;
        }

        avatar.sprite = currentConfig.speakers[currentSpeakerIndex].Sprite != null ? currentConfig.speakers[currentSpeakerIndex].Sprite : fallbackAvatar;

        // Show the dialogue UI
        if (dialogueContainer != null)
        {
            dialogueContainer.SetActive(true);
        }

        // Display the first line of dialogue
        DisplayNextLine();
    }

    /// <summary>
    /// Displays the next line in the conversation or ends the conversation if it's the last line.
    /// </summary>
    public void DisplayNextLine()
    {
        // If text is currently typing out, finish it instantly.
        if (isTyping)
        {
            CompleteLine();
            return;
        }

        // If we have more lines to display
        if (currentLineIndex < currentConfig.speakers[currentSpeakerIndex].Messages.Count)
        {
            // Start the typing effect for the next line
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeLine(currentConfig.speakers[currentSpeakerIndex].Messages[currentLineIndex]));
            currentLineIndex++;
        }
        else if (currentSpeakerIndex < currentConfig.speakers.Count - 1)
        {
            // If there are no more lines, go to the next speaker.
            currentSpeakerIndex++;
            avatar.sprite = currentConfig.speakers[currentSpeakerIndex].Sprite != null ? currentConfig.speakers[currentSpeakerIndex].Sprite : fallbackAvatar;
            currentLineIndex = 0;
            // Start the typing effect for the next line
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeLine(currentConfig.speakers[currentSpeakerIndex].Messages[currentLineIndex]));
            currentLineIndex++;
        }
        else
        {
            // If there are no more speakers, end the conversation.
            EndConversation();
        }
    }

    /// <summary>
    /// Coroutine that displays the dialogue text with a typing effect.
    /// </summary>
    /// <param name="line">The line of text to display.</param>
    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = ""; // Clear previous text
        continueButton.interactable = false; // Disable button while typing

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / typingSpeed);
        }

        isTyping = false;
        continueButton.interactable = true; // Re-enable button
    }

    /// <summary>
    /// Instantly completes the typing of the current line.
    /// </summary>
    private void CompleteLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        // The currentLineIndex was already incremented, so we access the previous line.
        dialogueText.text = currentConfig.speakers[currentSpeakerIndex].Messages[currentLineIndex - 1];
        isTyping = false;
        continueButton.interactable = true;
    }

    /// <summary>
    /// Hides the dialogue UI and resets the state.
    /// </summary>
    public void EndConversation()
    {
        if (dialogueContainer != null)
        {
            dialogueContainer.SetActive(false);
        }
        if (currentConfig.gameStateOnEnd == GameState.Battle && currentConfig.nextSceneBattle != null && requestGameStateChange != null)
        {
            requestGameStateChange.Raise((GameState.Battle, currentConfig.nextSceneBattle));
        }
        else
        {
            requestGameStateChange.Raise((currentConfig.gameStateOnEnd, null));
        }
        conversationEndEvent.Raise();
        currentConfig = null;
        currentSpeakerIndex = 0;
        currentLineIndex = 0;
        isTyping = false;
    }
}