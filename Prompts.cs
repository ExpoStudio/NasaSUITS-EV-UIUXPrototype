using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A ScriptableObject that holds a list of various prompts for the UI for easier reference.
/// </summary>
[CreateAssetMenu(fileName = "Prompts", menuName = "Scriptable Objects/Prompts")]
public class PromptList : ScriptableObject
{

    [System.Serializable]
    public class PromptEntry
    {
        public string key;
        public UIPromptManager.Prompt prompt;
    }
    
    public List<PromptEntry> PromptEntries = new();
}

/// <summary>
/// This static class provides global access to the prompt keys, method to convert them to prompt entries and a method to queue them in the prompt manager.
/// 
/// <para>Below is list of the Key Constants to access the prompts</para>
/// 
/// <para><paramref name="PrimaryO2Depleted">PrimaryOxygenDepleted prompt</paramref></para> 
/// <para><paramref name="EmergencyO2Supply"> - prompt </paramref></para> 
/// <para><paramref name="TestPrompt"> - Test prompt</paramref></para> 
/// </summary>
public static class PromptKeys
{
    public const string PrimaryO2Depleted = "PrimaryO2Depleted";
    public const string EmergencyO2Supply = "EmergencyO2Supply";
    public const string TestPrompt = "TestPrompt";

    public static UIPromptManager.Prompt ToPromptEntry(this string key)
    {
        var prompts = Resources.Load<PromptList>("Prompts");
        var entry = prompts.PromptEntries.FirstOrDefault(prompt => prompt.key == key);
        if (entry != null)
        {
            return entry.prompt;
        }
        else
        {
            Debug.LogWarning($"Prompt with key '{key}' not found.");
            return null;
        }
    }

    public static void QueuePrompt(this string key)
    {
        var keyPrompt = key.ToPromptEntry();
        if (key.ToPromptEntry() != null)
        {
            UIPromptManager.Instance.AddPromptToQueue(keyPrompt);
            UIPromptManager.Instance.SetEmergencyPrompt(keyPrompt.emergencyPrompt);
        }
    }
}