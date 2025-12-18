using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Actual UI controller for the emergency prompt.
/// </summary>
public class EmergencyPrompt : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject _uiParent;
    [SerializeField] private List<GameObject> _promptButtons = new();

    [SerializeField] private TMP_Text _messageText;
    private bool UiActive => UIPromptManager.Instance.promptDisplaying != null && UIPromptManager.Instance.emergencyPromptType != UIPromptManager.EmergencyUIPrompt.None && !UIPromptManager.Instance.promptDisplaying.minimized;

    void Update()
    {
        if (UiActive)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            foreach (var button in _promptButtons)
            {
                button.SetActive(true);
            }

            if (UIPromptManager.Instance.promptDisplaying is not null)
            {
                _messageText.text = UIPromptManager.Instance.promptDisplaying.message;
            }
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            foreach (var button in _promptButtons)
            {
                button.SetActive(false);
            }
        }
    }
}
