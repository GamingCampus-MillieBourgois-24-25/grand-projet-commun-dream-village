using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogues", menuName = "ScriptableObjects/Dialogues", order = 1)]
public class Dialogues : ScriptableObject
{
    
    // Type of dialogue: either a reaction of an event, information about a dream or the character
    public enum DialogueType
    {
        Reaction,
        Dream,
        Information
    }
    
    [SerializeField] private DialogueType dialogueType;
    [SerializeField] private string dialogueText;
    
    public DialogueType GetDialogueType()
    {
        return dialogueType;
    }
    
    public string GetDialogueText()
    {
        return dialogueText;
    }
}
