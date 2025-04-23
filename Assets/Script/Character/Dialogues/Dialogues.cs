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
        Dialogue,
        Dream,
        Information,
        Introduction,
        Reaction,
        Tutorial,
    }
    
    public enum DialogueCondition
    {
        None,
        FirstGreeting,
        GreetingRandom,
        MorningNegative,
        MorningPositive,
        PostDreamNegative,
        PostDreamPositive,
        TutorialSpecific,
    }
    
    public enum TutorialType
    {
        None,
        Activity,
        Dream,
        Edit,
        Heart,
        House,
        Shop
    }
    
    [SerializeField] private string ID;
    [SerializeField] private LocalizedString dialogueText;
    [SerializeField] private string[] requiredArguments;
    
    [SerializeField] private DialogueType dialogueType;
    [SerializeField] private DialogueCondition dialogueCondition;
    [SerializeField] private TutorialType tutorialType;
    
    [SerializeField] private bool shouldHoldDialogues = false;
    [SerializeField] private bool shouldGiveRareMoney = false;
    
    
    public string GetID()
    {
        return ID;
    }
    
    public DialogueType GetDialogueType()
    {
        return dialogueType;
    }
    
    public DialogueCondition GetDialogueCondition()
    {
        return dialogueCondition;
    }
    
    public TutorialType GetTutorialType()
    {
        return tutorialType;
    }
    
    public LocalizedString GetLocalizedString()
    {
        return dialogueText;
    }
    
    public string GetDialogueText()
    {
        return dialogueText.GetLocalizedString();
    }

    public string[] GetRequiredArguments()
    {
        return requiredArguments;
    }
    
    public bool ShouldHoldDialogues()
    {
        return shouldHoldDialogues;
    }
    
    public bool ShouldGiveRareMoney()
    {
        return shouldGiveRareMoney;
    }
}
