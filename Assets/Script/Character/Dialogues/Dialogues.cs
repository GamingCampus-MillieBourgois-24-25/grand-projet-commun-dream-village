using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Dialogues", menuName = "ScriptableObjects/Dialogues", order = 1)]
public class Dialogues : ScriptableObject
{
    public enum DialogueType
    {
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
    
    public string ID;
    [SerializeField] private LocalizedString dialogueText;
    
    [SerializeField] private DialogueType dialogueType;
    [SerializeField] private Stats stats;
    
    public DialogueType GetDialogueType()
    {
        return dialogueType;
    }
    
    public string GetDialogueText()
    {
        return dialogueText.GetLocalizedString();
    }
    
    public Stats GetStats()
    {
        return stats;
    }
}
