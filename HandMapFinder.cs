using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;


public class HandMapFinder : MonoBehaviour
{
    public static HandMapFinder Instance;
    // Moves based on position.
    // Control the visibility of the hand map UI. This follows the hand in the scene. If you tap on the minimap on the hand, it will enlarge the map. Toggle visibility based on found hand parented hand rotation.
    [SerializeField] private CanvasGroup _handMapCanvasGroup;
    [SerializeField] private Transform _userHead;
    [SerializeField] private RectTransform _handMapUserLocatorArrow;
    [SerializeField] private RectTransform _handMapTransform;
    bool HandMapVisible => _userHead.rotation.eulerAngles.x > 30f && _userHead.rotation.eulerAngles.x < 80f; // User is looking down at their hand and the hand is found.

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
        if (HandMapVisible)
        {

            _handMapUserLocatorArrow.localRotation = Quaternion.Euler(0, 0, -_userHead.eulerAngles.y);
            Vector3 targetPosition = new(-_userHead.localPosition.x * 2f, -_userHead.localPosition.z * 2f, 0);
            _handMapTransform.localPosition = targetPosition;

            _handMapCanvasGroup.alpha = 1;
            _handMapCanvasGroup.blocksRaycasts = true;
            _handMapCanvasGroup.interactable = true;
        }
        else
        {
            _handMapCanvasGroup.alpha = 0;
            _handMapCanvasGroup.blocksRaycasts = false;
            _handMapCanvasGroup.interactable = false;
        }
    }
}
