using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.Input;


public class SpeechEventRouter : MonoBehaviour, IMixedRealitySpeechHandler
{
    public static SpeechEventRouter Instance;
    void OnEnable()
    {
        Microsoft.MixedReality.Toolkit.CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnDisable()
    {
        Microsoft.MixedReality.Toolkit.CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
    }


    public event Action OnConfirmRecongized;
    public event Action OnOverrideRecognized;
    public event Action OnShowVitalsRecognized;
    public event Action OnShowMapRecognized;
    public event Action OnHideMapRecognized;
    public event Action OnShowO2CommandRecognized;

    void IMixedRealitySpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        Debug.Log($"Speech command recognized: {eventData.Command.Keyword}");

        string keyword = eventData.Command.Keyword.Trim().ToLowerInvariant();

        switch (keyword)
        {
            case "confirm":
                OnConfirmRecongized?.Invoke();
                break;
            case "override":
                OnOverrideRecognized?.Invoke();
                break;
            case "dismiss":
                OnConfirmRecongized?.Invoke();
                break;
            case "show vitals":
            case "check vitals":
                OnShowVitalsRecognized?.Invoke();
                Debug.Log("Show Vitals Invoked");
                break;
            case "show map":
                OnShowMapRecognized?.Invoke();
                Debug.Log("Show Map Invoked");
                break;
            case "hide map":
                OnHideMapRecognized?.Invoke();
                break;
            case "tell me my oxygen levels":
            case "tell me my oh two levels":
                OnShowO2CommandRecognized?.Invoke();
                break;
            default:
                Debug.LogWarning($"Unhandled speech command: {keyword}");
                break;
        }
    }
}
