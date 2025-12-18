using UnityEngine;

/// <summary>
/// The purpose of this class is to make the modification of several UI components and settings that are used by all components easy and Enumerable.
/// Enumerability means will allow me to iterate through each game object with this component and enable/disable or tweak settings as needed.
/// To every game object or parent of a game object that is a UI component, add this script and set the parameters accordingly.
/// </summary>
public class UISettings : MonoBehaviour
{
    public enum UIComponentType
    {
        Vitals,
        Map,
        EmergencyPrompt,
        Other
    }
    public enum UIComponentSize
    {
        Normal,
        Enlarged,
        Hidden
    }
    public UIComponentType ComponentType = UIComponentType.Other;
    public UIComponentSize ComponentSize = UIComponentSize.Normal;
    //Identifier. Just t
    public string Identifier;
    private bool _isHidden = true;
    public bool IsHidden
    {
        get => _isHidden;
        set
        {
            _isHidden = value;
            gameObject.SetActive(value);

            if (value)
            {

            }
        }
    }
    public bool isFaded = false;

}
