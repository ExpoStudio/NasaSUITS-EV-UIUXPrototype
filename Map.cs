using System;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map Instance;

    [SerializeField] private RectTransform _userLocatorArrow;
    [SerializeField] private Transform _userHead;

    [SerializeField] private GameObject _EnlargedMapUI;
    [SerializeField] private GameObject _MinimapUI;

    [SerializeField] private CanvasGroup _mapCanvasGroup;
    [SerializeField] private CanvasGroup _minimapCanvasGroup;
    public UISettings mapSettings;

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

    void Start()
    {
        RegisterMenuVisibilitySpeech();

        
    }

    bool isInitialized = false;
    bool EmergencyPromptDisplaying =>
    (UIPromptManager.Instance.promptDisplaying?.emergencyPrompt ?? UIPromptManager.EmergencyUIPrompt.None)
        != UIPromptManager.EmergencyUIPrompt.None;
    void Update()
    {
        if (mapSettings == null) return;
        if (!isInitialized)
        {
            RegisterMenuVisibilitySpeech();
        }
        if (EmergencyPromptDisplaying && !mapSettings.isFaded)
        {
            UnregisterMenuVisibilitySpeech();
            mapSettings.isFaded = true;
        }
        else if (!EmergencyPromptDisplaying && mapSettings.isFaded)
        {
            RegisterMenuVisibilitySpeech();
            mapSettings.isFaded = false;
        }



        CanvasVisibility(mapSettings.isFaded);
        if (mapSettings.isFaded) return;

        switch (mapSettings.ComponentSize)
        {
            case UISettings.UIComponentSize.Hidden:
                transform.localScale = Vector3.one * 0.5f;
                _EnlargedMapUI.SetActive(false);
                _MinimapUI.SetActive(true);
                break;
            case UISettings.UIComponentSize.Enlarged:
                _EnlargedMapUI.SetActive(true);
                _MinimapUI.SetActive(false);

                _userLocatorArrow.localRotation = Quaternion.Euler(0, 0, -_userHead.eulerAngles.y);
                _userLocatorArrow.localPosition = new Vector3(_userHead.position.x * 2f, _userHead.position.z * 2f, 0);
                break;
            default:
                transform.localScale = Vector3.one * 0.75f;
                break;
        }
    }

    private void CanvasVisibility(bool shouldHide)
    {
        _mapCanvasGroup.alpha = shouldHide ? 0.25f : 1f;
        _mapCanvasGroup.blocksRaycasts = !shouldHide;
        _mapCanvasGroup.interactable = !shouldHide;

        _minimapCanvasGroup.alpha = shouldHide ? 0.25f : 1f;
        _minimapCanvasGroup.blocksRaycasts = !shouldHide;
        _minimapCanvasGroup.interactable = !shouldHide;
    }

    void OnDestroy()
    {
        UnregisterMenuVisibilitySpeech();
    }


    bool _speechRegistered = false;
    void RegisterMenuVisibilitySpeech()
    {
        if (!_speechRegistered)
        {
            isInitialized = true;
            SpeechEventRouter.Instance.OnShowMapRecognized += ShowLargeMapVisibility;
            SpeechEventRouter.Instance.OnHideMapRecognized += HideMapVisibility;

            _speechRegistered = true;
            Debug.Log("Map Speech Registered");
        }
    }
    void UnregisterMenuVisibilitySpeech()
    {

        if (_speechRegistered)
        {
            SpeechEventRouter.Instance.OnShowMapRecognized -= ShowLargeMapVisibility;
            SpeechEventRouter.Instance.OnHideMapRecognized -= HideMapVisibility;

            _speechRegistered = false;
        }
        Debug.Log("Map Speech UnRegistered");
    }

    private void HideMapVisibility()
    {
        mapSettings.ComponentSize = UISettings.UIComponentSize.Hidden;
        Debug.Log("Map hidden and state changed");
    }
    private void ShowLargeMapVisibility()
    {
        mapSettings.ComponentSize = UISettings.UIComponentSize.Enlarged;
        Debug.Log("Large map shown and state changed");
    }
}
