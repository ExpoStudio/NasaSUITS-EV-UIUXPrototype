using UnityEngine;

public class WarningUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _WarningCanvasGroup;
    [SerializeField] private TMPro.TMP_Text _promptText;

    private bool PromptActive => UIPromptManager.Instance.promptDisplaying != null && UIPromptManager.Instance.displayTimer > 0 && UIPromptManager.Instance.CanPrompt;
    void Update()
    {
        if (PromptActive)
        {
            _WarningCanvasGroup.alpha = 1;
            _WarningCanvasGroup.blocksRaycasts = true;
            _WarningCanvasGroup.interactable = true;
        }
        else
        {
            _WarningCanvasGroup.alpha = 0;
            _WarningCanvasGroup.blocksRaycasts = false;
            _WarningCanvasGroup.interactable = false;
        }
    }
}
