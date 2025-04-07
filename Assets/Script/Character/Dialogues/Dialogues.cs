using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogues", menuName = "ScriptableObjects/Dialogues", order = 1)]
public class Dialogues : ScriptableObject
{
    public string ID;
    
    // Type of dialogue: either a reaction of an event, information about a dream or the character
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
    
    
    public DialogueType dialogueType;
    public string dialogueText;
    public List<AttributeEffect> attributeEffects;
    public Stats stats;
    
    
    public DialogueType GetDialogueType()
    {
        return dialogueType;
    }
    
    public string GetDialogueText()
    {
        return dialogueText;
    }
}
