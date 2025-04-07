using System;
using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DialoguesInhabitantTest : MonoBehaviour
{
    public Inhabitant inhabitant;
    public TMP_Text dialogueText;
    public List<Dialogues> dialogues;
    public List<Dialogues> dialoguesDream;
    public List<Dialogues> dialoguesInformation;
    public List<Dialogues> dialoguesReaction;
    

    [System.Serializable]
    public struct Stats
    {
        public int Mood;
        public int Serenity;
        public int Energy;
        public int Hearts;
    }

    public Stats stats;
    
    private void Start()
    {
        GetAllDialogues();
    }

    public void ShowDialogue(int type)
    {
        Dialogues dial = null;
        switch (type)
        {
            case 1:
                dial = dialoguesDream[Random.Range(0, dialoguesDream.Count)];
                ShowDialogue(dial);
                UpdateStats(dial);
                ShowStats();
                break;
            case 2:
                dial = dialoguesInformation[Random.Range(0, dialoguesInformation.Count)];
                ShowDialogue(dial);
                UpdateStats(dial);
                ShowStats();
                break;
            case 3:
                dial = dialoguesReaction[Random.Range(0, dialoguesReaction.Count)];
                ShowDialogue(dial);
                UpdateStats(dial);
                ShowStats();
                break;
        }
    }

    private void UpdateStats(Dialogues dialogue)
    {
        stats.Mood += dialogue.stats.Mood;
        stats.Serenity += dialogue.stats.Serenity;
        stats.Energy += dialogue.stats.Energy;
        stats.Hearts += dialogue.stats.Hearts;
    }
    
    private void ShowDialogue(Dialogues dialogue)
    {
        LMotion.String.Create128Bytes("", dialogue.GetDialogueText(), 2f)
            .WithRichText()
            .WithScrambleChars(ScrambleMode.Lowercase)
            .BindToText(dialogueText);
    }

    private void ShowStats()
    {
        Debug.Log($"Mood : {stats.Mood}");
        Debug.Log($"Serenity : {stats.Serenity}");
        Debug.Log($"Energy : {stats.Energy}");
        Debug.Log($"Hearts : {stats.Hearts}");
    }
    
    private void GetAllDialogues()
    {
        var dialoguesObj = Resources.LoadAll("Dialogues");
        foreach (var obj in dialoguesObj)
        {
            if (obj is Dialogues dialogue)
            {
                dialogues.Add(dialogue);
            }
        }
        
        foreach (var obj in dialoguesObj)
        {
            if (obj is Dialogues dialogue)
            {
                switch (dialogue.GetDialogueType())
                {
                    case Dialogues.DialogueType.Reaction:
                        dialoguesReaction.Add(dialogue);
                        break;
                    case Dialogues.DialogueType.Dream:
                        dialoguesDream.Add(dialogue);
                        break;
                    case Dialogues.DialogueType.Information:
                        dialoguesInformation.Add(dialogue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
