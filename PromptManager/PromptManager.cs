using System;
using System.Collections.Generic;
using UnityEngine;

    public class UIPromptManager : MonoBehaviour
    {
        public static UIPromptManager Instance;
        public bool CanPrompt => emergencyPromptType is EmergencyUIPrompt.None;

        public enum UIPromptType
        {
            None,
            Emergency,
            Info,
            Warning
        }

        public enum EmergencyUIPrompt
        {
            None,
            EmergencyOxygen,
        }

    [Serializable]
    /// <summary>
    /// A single prompt to be displayed on the UI. A choice to prompt choices or just a message with a time limit.
    /// </summary>
    public class Prompt
    {
        public UIPromptType promptType;
        /// <summary>
        /// Select the emergency prompt type if it is an emergency. Otherwise set to none.
        /// </summary>
        public EmergencyUIPrompt emergencyPrompt;
        public string message;
        public bool awaitResponse;

        /// <summary>
        /// Set the display time for the prompt. This is ignored if awaitResponse is true.
        /// </summary>
        public float displayTime = 5f; // Time in seconds the prompt should be displayed
            
        public bool minimized = false;

    }


    /// <summary>
    /// As to prevent prompt spamming, we will queue prompts and display them one at a time.
    /// </summary>
    public Queue<Prompt> promptQueue = new();

    /// <summary>
    /// Configuration for each UI component. Keeps track of the Main Component Objects for iteration
    /// </summary>
    public List<UISettings> uiComponents = new();

    public EmergencyUIPrompt emergencyPromptType = EmergencyUIPrompt.None;
    private enum PromptState
    {
        Idle,
        Displaying,
        AwaitingResponse,
        Cooldown
    }
    [SerializeField] private PromptState currentState = PromptState.Idle;

    [SerializeField] private float _actionTimer = 0f;
    public float ActionTimer
    {
        get { return _actionTimer; }
        set { _actionTimer = Mathf.Clamp(value, 0, 10f); }
    }
    private bool isPromptMinimized = false;

    public float displayTimer = 0f;
    /// <summary>
    /// Time in seconds between prompts to avoid spamming the user with too many prompts.
    /// </summary>
    [SerializeField] private const float PromptCooldown = 2f; // Time in seconds between prompts
    public Prompt promptDisplaying = null;

    public ResponseID responseID = ResponseID.None;

    public enum ResponseID
    {
        None = 0,
        Ok = 1,
        No = 2
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        float deltaTime = Time.deltaTime;
        
        switch (currentState)
        {
            case PromptState.Idle:
                if (ActionTimer >= 0) ActionTimer -= deltaTime; 

                if (promptQueue.Count > 0 && ActionTimer <= 0)
                {
                    promptDisplaying = promptQueue.Dequeue();
                    // Display the prompt
                    Debug.Log($"Displaying Prompt: {promptDisplaying.message}");
                    displayTimer = promptDisplaying.displayTime;
                    currentState = promptDisplaying.awaitResponse ? PromptState.AwaitingResponse : PromptState.Displaying;
                }
                else
                {
                    promptDisplaying = null;
                }
                break;
            case PromptState.Displaying:
                displayTimer -= deltaTime;

                SpeechEventRouter.Instance.OnConfirmRecongized += DismissInfoWarningPrompt;

                if (displayTimer <= 0)
                {
                    DismissInfoWarningPrompt();
                    SpeechEventRouter.Instance.OnConfirmRecongized -= DismissInfoWarningPrompt;
                }
                break;
            case PromptState.AwaitingResponse:
                /*if (!promptDisplaying.minimized && isPromptMinimized)
                {
                    promptDisplaying.minimized = true;
                }*/

                if (responseID == ResponseID.Ok) // Simulate user response
                {
                    Debug.Log("User responded to prompt.");
                    responseID = 0;
                    currentState = PromptState.Idle;
                }
                break;
        }
    }

    private void DismissInfoWarningPrompt()
    {
        displayTimer = 0;

        Debug.Log("Prompt display time ended.");
        currentState = PromptState.Idle;
        ActionTimer = PromptCooldown;
        promptDisplaying = null;
    }

    public void SetEmergencyPrompt(EmergencyUIPrompt prompt)
    {
        emergencyPromptType = prompt;
    }

    public void AddPromptToQueue(Prompt prompt)
    {
        promptQueue.Enqueue(prompt);
    }
}