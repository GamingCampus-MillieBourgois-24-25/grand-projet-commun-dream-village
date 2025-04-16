using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

[CreateAssetMenu(fileName = "Dialogues", menuName = "ScriptableObjects/Dialogues", order = 1)]
public class Dialogues : ScriptableObject
{
    public enum DialogueType
    {
        Introduction,
        TutoHouse,
        TutoActivity,
        Reaction,
        Dream,
        Information
    }
    
    [System.Serializable]
    public struct Stats
    {
        public int Mood;
        public int Serenity;
        public int Energy;
        public int Hearts;
    }

    [System.Serializable]
    public class AttributeEffect
    { 
        public enum BonusType
        {
            Multiple,
            Add
        }
        
        public InterestCategory attribute;
        public float bonus;
        public BonusType bonusType;
    }
    
    [SerializeField] private string ID;
    [SerializeField] private string relatedElementID;
    [SerializeField] private LocalizedString dialogueText;
    [SerializeField] private string[] requiredArguments;
    
    [SerializeField] private DialogueType dialogueType;
    [SerializeField] private Stats stats;
    
    public string GetID()
    {
        return ID;
    }
    
    public string GetRelatedElementID()
    {
        return relatedElementID;
    }
    
    public DialogueType GetDialogueType()
    {
        return dialogueType;
    }
    public LocalizedString GetLocalizedString()
    {
        return dialogueText;
    }
    
    public string GetDialogueText()
    {
        return dialogueText.GetLocalizedString();
    }
    public Stats GetStats()
    {
        return stats;
    }

    public string[] GetRequiredArguments()
    {
        return requiredArguments;
    }
}
