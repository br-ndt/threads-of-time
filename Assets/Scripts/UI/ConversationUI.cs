using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Assets.Scripts.Events;
using Assets.Scripts.Configs;
using System.Collections.Generic;

/// <summary>
/// Manages the Conversation UI, including displaying dialogue, speaker names, and handling user input.
/// </summary>
public class ConversationUI : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] ConversationStartEvent conversationStartEvent;
    [SerializeField] ConversationProgressEvent conversationProgressEvent;
    [SerializeField] ConversationEndEvent conversationEndEvent;
    [SerializeField] GameStateChangeEvent requestGameStateChange;
    [SerializeField] RecordTriggerEvent recordTriggerEvent;
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
    [Tooltip("The area where responses buttons will occupy")]
    public GameObject responsesContainer;
    [Tooltip("The prefab for the response buttons")]
    public GameObject responseButtonPrefab;

    [Header("Typing Effect Settings")]
    [Tooltip("The speed at which the text types out, in characters per second.")]
    [Range(1f, 100f)]
    public float typingSpeed = 50f;

    [SerializeField] private Sprite fallbackAvatar;

    public ConversationConfig currentConfig;
    private int currentSpeakerIndex = 0;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private List<GameObject> spawnedResponseButtons = new(); // To manage spawned target buttons


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
            continueButton.onClick.AddListener(HandleButtonPress);
        }
    }

    private void ClearResponseButtons()
    {
        foreach (GameObject buttonGO in spawnedResponseButtons)
        {
            if (buttonGO != null) Destroy(buttonGO);
        }
        spawnedResponseButtons.Clear();
        continueButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Starts a new conversation.
    /// </summary>
    /// <param name="speakerName">The name of the character speaking.</param>
    /// <param name="dialogueLines">An array of strings, where each string is a line of dialogue.</param>
    public void HandleConversationStart(ConversationConfig config)
    {
        ClearResponseButtons();
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
        CheckIfResponsesType();

        // Show the dialogue UI
        if (dialogueContainer != null)
        {
            dialogueContainer.SetActive(true);
        }

        // Display the first line of dialogue
        DisplayNextLine();
    }

    /// <summary>
    /// Event handler for pressing the "A" button.
    /// </summary>
    public void HandleButtonPress()
    {
        ClearResponseButtons();
        if (isTyping)
        {
            CompleteLine();
        }
        else
        {
            currentLineIndex++;
        }
        if (currentLineIndex >= currentConfig.speakers[currentSpeakerIndex].Messages.Count)
        {
            DisplayNextSpeaker();
        }
        else
        {
            DisplayNextLine();
        }
    }

    /// <summary>
    /// Displays the next speaker in the conversation.
    /// </summary>
    public void DisplayNextSpeaker()
    {
        currentSpeakerIndex++;
        if (currentSpeakerIndex >= currentConfig.speakers.Count)
        {
            EndConversation();
            return;
        }
        avatar.sprite = currentConfig.speakers[currentSpeakerIndex].Sprite != null ? currentConfig.speakers[currentSpeakerIndex].Sprite : fallbackAvatar;
        speakerNameText.text = currentConfig.speakers[currentSpeakerIndex].Name;
        currentLineIndex = 0;
        DisplayNextLine();
    }

    /// <summary>
    /// Displays the next line in the conversation or ends the conversation if it's the last line.
    /// </summary>
    public void DisplayNextLine()
    {
        CheckIfResponsesType();
        typingCoroutine = StartCoroutine(TypeLine(currentConfig.speakers[currentSpeakerIndex].Messages[currentLineIndex].Content));
    }

    private void CheckIfResponsesType()
    {
        if (currentConfig.speakers[currentSpeakerIndex].Messages[currentLineIndex] is ConversationResponses)
        {
            foreach (ConversationResponse response in (currentConfig.speakers[currentSpeakerIndex].Messages[currentLineIndex] as ConversationResponses).Responses)
            {
                // TODO: use a pool or something
                GameObject buttonGO = Instantiate(responseButtonPrefab, responsesContainer.transform);
                Button responseButton = buttonGO.GetComponent<Button>();
                TMP_Text buttonText = buttonGO.GetComponentInChildren<TMP_Text>();

                responseButton.onClick.AddListener(() => OnResponseButtonClicked(response.ConvoIfChosen, response.TriggerIfChosen));
                buttonText.text = response.Content;
                spawnedResponseButtons.Add(buttonGO); // Keep track for cleaning up
            }
            responsesContainer.SetActive(true);
        }
    }

    private void OnResponseButtonClicked(ConversationConfig convoIfChosen, TriggerEvent triggerIfChosen)
    {
        if (triggerIfChosen != null)
        {
            recordTriggerEvent.Raise((new List<TriggerEvent>() { triggerIfChosen }, true));
            triggerIfChosen.Raise(true);
        }
        if (convoIfChosen != null)
        {
            currentConfig = convoIfChosen;
            currentLineIndex = 0;
            currentSpeakerIndex = 0;
            HandleConversationStart(convoIfChosen);
        }
    }

    /// <summary>
    /// Coroutine that displays the dialogue text with a typing effect.
    /// </summary>
    /// <param name="line">The line of text to display.</param>
    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / typingSpeed);
        }

        isTyping = false;
        RenderButtons();
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

        dialogueText.text = currentConfig.speakers[currentSpeakerIndex].Messages[currentLineIndex].Content;
        isTyping = false;
        RenderButtons();
    }

    /// <summary>
    /// Shows the appropriate buttons based on the current line.
    /// </summary>
    public void RenderButtons()
    {
        if (currentConfig.speakers[currentSpeakerIndex].Messages[currentLineIndex] is ConversationResponses)
        {
            continueButton.gameObject.SetActive(false);
            responsesContainer.SetActive(true);
        }
        else
        {
            continueButton.gameObject.SetActive(true);
            responsesContainer.SetActive(false);
        }
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
        if (requestGameStateChange != null)
        {
            requestGameStateChange.Raise((currentConfig.sceneChange.newState, currentConfig.sceneChange.newStateConfig));
        }
        else
        {
            requestGameStateChange.Raise((currentConfig.sceneChange.newState, null));
        }
        conversationEndEvent.Raise();
        currentConfig = null;
        currentSpeakerIndex = 0;
        currentLineIndex = 0;
        isTyping = false;
    }
}