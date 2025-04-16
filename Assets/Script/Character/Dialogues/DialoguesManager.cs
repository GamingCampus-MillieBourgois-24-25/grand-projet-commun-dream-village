using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;

public class DialoguesManager : MonoBehaviour
{
    public static DialoguesManager Instance;

    [SerializeField] private List<Dialogues> dialogues = new();
    
    [Serializable]
    public struct DictStrings
    {
        public string varName;
        public string variable;
    }

    [SerializeField] private List<DictStrings> localizedStrings;

    [Header("UI Elements")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadAllDialogues();
    }

    private void LoadAllDialogues()
    {
        var dialoguesObj = Resources.LoadAll("Dialogues", typeof(Dialogues));
        foreach (var obj in dialoguesObj)
        {
            if (obj is Dialogues dialogue)
                dialogues.Add(dialogue);
        }
    }

    public List<Dialogues> GetDialogues() => dialogues;

    private string GetVariable(string key)
    {
        return localizedStrings.FirstOrDefault(x => x.varName == key).variable ?? "???";
    }

    [ContextMenu("ShowIntroDialogue")]
    public void ShowIntroDialogue()
    {
        var introDialogue = dialogues.FirstOrDefault(d => d.GetDialogueType() == Dialogues.DialogueType.Introduction);
        if (introDialogue != null)
        {
            DisplayDialogue(introDialogue);
        }
    }

    public Dialogues debugDialogue;
    
    [ContextMenu("DebugShowIntroDialogue")]
    public void DebugShowIntroDialogue()
    {
        if (debugDialogue != null)
        {
            DisplayDialogue(debugDialogue);
        }
    }

    private void DisplayDialogue(Dialogues dialogue)
    {
        LocalizedString localized = dialogue.GetLocalizedString();
        localized.Arguments = null; // reset

        var args = new List<object>();
        foreach (var argKey in dialogue.GetRequiredArguments())
        {
            args.Add(GetVariable(argKey));
        }

        localized.Arguments = args.ToArray();

        DisplayInBox(localized.GetLocalizedString());
    }

    private void DisplayInBox(string text)
    {
        dialogueBox.SetActive(true);
        dialogueText.text = text;
    }
}
