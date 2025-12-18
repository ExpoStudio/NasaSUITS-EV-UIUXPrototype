using UnityEngine;
using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using System.Runtime.InteropServices.WindowsRuntime;

public class Vitals : MonoBehaviour
{
    private UIPromptManager PromptManager => UIPromptManager.Instance;
    public const double lowOxygenThreshold = 19.5; // Percentage of oxygen considered low
    [SerializeField] private Vector2 _oxygenLevel = new(100, 100); // Percentage of oxygen level
    [SerializeField] private UISettings _vitalsSettings;

    [SerializeField] private CanvasGroup _normalVitalsUI;
    [SerializeField] private CanvasGroup _MaximizedVitalsUI;


    [SerializeField] private float _heartRate = 100; // Percentage of health

    public float PrimaryO2
    {
        get { return _oxygenLevel.x; }
        set
        {
            _oxygenLevel.x = Mathf.Clamp(value, 0, 100);
        }
    }
    public float SecondaryO2
    {
        get { return _oxygenLevel.y; }
        set
        {
            _oxygenLevel.y = Mathf.Clamp(value, 0, 100);
        }
    }

    public enum OxygenTankState
    {
        Primary,
        Secondary,
        RemainingSupply
    }
    public OxygenTankState oxygenTankUse = OxygenTankState.Primary;
    public static Vitals Instance;
    public Vector2 heartRate = new(100, 100); // Percentage of health
    public event Action<OxygenTankState> OnOxygenLow;
    public bool UseEmergencyOxygen => PrimaryO2 <= lowOxygenThreshold && SecondaryO2 <= lowOxygenThreshold;
    void Awake()
    {
        AcceptEmergencyInput = false;
        OnOxygenLow += (state) =>
        {
            switch (state)
            {
                case OxygenTankState.Primary:
                    PromptKeys.TestPrompt.QueuePrompt();
                    break;
                case OxygenTankState.Secondary:
                    PromptKeys.PrimaryO2Depleted.QueuePrompt();
                    break;
                case OxygenTankState.RemainingSupply:
                    PromptKeys.EmergencyO2Supply.QueuePrompt();
                    break;
                default:
                    break;
            }
        };
    }

    [SerializeField] private List<Interactable> emergencyButtons = new();

    private bool _emergencyInputsRegistered = false;
    private bool _acceptEmergencyInput = false;
    public bool AcceptEmergencyInput
    {
        get => _acceptEmergencyInput;
        set
        {
            if (_acceptEmergencyInput == value) return;
            _acceptEmergencyInput = value;

            if (value)
            {
                UnregisterMenuVisibilitySpeech();
                RegisterEmergencyInput();
                return;
            }
            else
            {
                RegisterMenuVisibilitySpeech();
                UnregisterEmergencyInput();
            }
        }
    }

    private void UnregisterEmergencyInput()
    {
        if (!_emergencyInputsRegistered) return;
        UnregisterButtonInput();
        UnregisterPromptSpeechInput();
        _emergencyInputsRegistered = false;
    }

    private void RegisterEmergencyInput()
    {
        if (_emergencyInputsRegistered) return;
        ListenToPromptSpeechInput();
        ListenToButtonInput();
        _emergencyInputsRegistered = true;
    }

    private void ListenToButtonInput()
    {
        foreach (var button in emergencyButtons)
        {
            var buttonRef = button;
            buttonRef.OnClick.AddListener(() =>
            {
                if (buttonRef.name.Contains("ConfirmButton"))
                {
                    Ok();
                }
                else if (buttonRef.name.Contains("OverrideButton"))
                {
                    Override();
                }
            });
        }
    }

    void ListenToPromptSpeechInput()
    {
        //Subscribe to Speech Events
        SpeechEventRouter.Instance.OnConfirmRecongized += Ok;
        SpeechEventRouter.Instance.OnOverrideRecognized += Override;
    }
    void UnregisterButtonInput()
    {
        foreach (var button in emergencyButtons)
        {
            button.OnClick.RemoveAllListeners();
        }
    }

    void RegisterMenuVisibilitySpeech()
    {
        SpeechEventRouter.Instance.OnShowVitalsRecognized += ToggleVitalsEnlarge;
        SpeechEventRouter.Instance.OnShowO2CommandRecognized += ToggleVitalsEnlarge;
    }

    private void ToggleVitalsEnlarge()
    {
        _vitalsSettings.ComponentSize = UISettings.UIComponentSize.Enlarged;
    }
    private void ToggleVitalsNormal()
    {
        _vitalsSettings.ComponentSize = UISettings.UIComponentSize.Normal;
    }
    private void ToggleVitalsSmall()
    {
        _vitalsSettings.ComponentSize = UISettings.UIComponentSize.Hidden;
    }


    void UnregisterMenuVisibilitySpeech()
    {
        SpeechEventRouter.Instance.OnShowVitalsRecognized -= ToggleVitalsEnlarge;
    }

    void Update()
    {
        float depletionPerSecond = .9f * Time.deltaTime;
        switch (oxygenTankUse)
        {
            case OxygenTankState.Primary:
                PrimaryO2 -= depletionPerSecond;

                if (PrimaryO2 <= lowOxygenThreshold)
                {
                    oxygenTankUse = OxygenTankState.Secondary;
                    OnOxygenLow?.Invoke(OxygenTankState.Secondary);
                }
                break;
            case OxygenTankState.Secondary:
                SecondaryO2 -= depletionPerSecond;

                if (SecondaryO2 <= lowOxygenThreshold)
                {
                    oxygenTankUse = OxygenTankState.RemainingSupply;
                    OnOxygenLow?.Invoke(OxygenTankState.RemainingSupply);
                }
                break;
            case OxygenTankState.RemainingSupply:
                if (UseEmergencyOxygen)
                {
                    PrimaryO2 -= depletionPerSecond / 2;
                    SecondaryO2 -= depletionPerSecond / 2;
                }

                if (PromptManager.emergencyPromptType == UIPromptManager.EmergencyUIPrompt.EmergencyOxygen && UseEmergencyOxygen)
                {
                    if(!AcceptEmergencyInput) AcceptEmergencyInput = true;
                    // Await Responses through UI - Either through voice command or button press
                }
                break;
        }

        //Toggle Vitals UI
        switch (_vitalsSettings.ComponentSize)
        {
            case UISettings.UIComponentSize.Enlarged:
                DetermineVisibility(true, false);
                break;
            case UISettings.UIComponentSize.Normal:
                DetermineVisibility(false, true);
                break;
            case UISettings.UIComponentSize.Hidden:
                DetermineVisibility(false, false);
                break;
        }
    }
    private int AlphaValue(bool arg) =>  arg ? 1 : 0;
    private void DetermineVisibility(bool showMaximizedUI, bool showNormalUI)
    {  
        _MaximizedVitalsUI.alpha = AlphaValue(showMaximizedUI);
        _MaximizedVitalsUI.blocksRaycasts = showMaximizedUI;
        _MaximizedVitalsUI.interactable = showMaximizedUI;

        _normalVitalsUI.alpha = AlphaValue(showNormalUI);
        _normalVitalsUI.blocksRaycasts = showNormalUI;
        _normalVitalsUI.interactable = showNormalUI;
    }

    void OnDestroy()
    {
        UnregisterPromptSpeechInput();
        UnregisterButtonInput();
    }

    private void UnregisterPromptSpeechInput()
    {
        SpeechEventRouter.Instance.OnConfirmRecongized -= Ok;
        SpeechEventRouter.Instance.OnOverrideRecognized -= Override;
    }
    
    //For connecting the events to the buttons on the Emergency Prompt UI
    public void Override()
    {
        Debug.Log("Override");
        PromptManager.responseID = UIPromptManager.ResponseID.No;
    }
    public void Ok()
    {
        Debug.Log("Ok");
        PromptManager.responseID = UIPromptManager.ResponseID.Ok;
    }
}
