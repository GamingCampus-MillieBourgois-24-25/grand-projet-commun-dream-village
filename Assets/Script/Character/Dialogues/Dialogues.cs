using UnityEngine;
using UnityEngine.Localization;

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
        NotUsed,
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
        Shop,
    }
    
    [Header("General")]
    [SerializeField] private string ID;
    [SerializeField] private int tutorialID;
    [SerializeField] private bool shouldBePlayed = true;
    [SerializeField] private LocalizedString dialogueText;
    [SerializeField] private string[] requiredArguments;
    
    [Header("Dialogue Types")]
    [SerializeField] private DialogueType dialogueType;
    [SerializeField] private DialogueCondition dialogueCondition;
    [SerializeField] private TutorialType tutorialType;
    
    [Header("Dialogue Options")]
    [SerializeField] private bool shouldShowSkipButton = true;
    [SerializeField] private bool shouldBlockInteractions = false;
    [SerializeField] private bool shouldHoldDialogues = false;
    [SerializeField] private bool shouldGiveRareMoney = false;
    [SerializeField] private bool isDialogueBoxTop = false;
    [SerializeField] private NosphyPosition nosphyPosition;
    
    [Header("UI Highlight")]
    public bool highlightJournalRightPage = false;
    public bool highlightJournalStats = false;
    public bool highlightQuitJournalButton = false;
    public bool highlightNightButton = false;
    public bool highlightDreamButton = false;
    public bool highlightShopButton = false;
    public bool highlightQuitShopButton = false;
    public bool highlightEditButton = false;
    public bool highlightQuitEditButton = false;
    
    public string GetID()
    {
        return ID;
    }
    
    public int GetTutorialID()
    {
        return tutorialID;
    }
    
    public bool ShouldBePlayed()
    {
        return shouldBePlayed;
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
    
    public bool IsDialogueBoxTop()
    {
        return isDialogueBoxTop;
    }
    
    public NosphyPosition GetNosphyPosition()
    {
        return nosphyPosition;
    }
    
    public bool ShouldBlockInteractions()
    {
        return shouldBlockInteractions;
    }
    
    public bool ShouldShowSkipButton()
    {
        return shouldShowSkipButton;
    }
    
}
