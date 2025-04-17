using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;

public class DialoguesManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private List<Dialogues> dialogues = new();
    
    [Serializable]
    public struct DictStrings
    {
        public string name;
        public string value;
    }

    [SerializeField] private List<DictStrings> localizedStrings;

    [Header("UI Elements")] public GameObject dialogueCanvas;
    public GameObject dialogueBox;
    public TMP_Text dialogueText;

    private void Awake()
    {
        gameManager = GameManager.instance;

        LoadAllDialogues();
    }

    private void Start()
    {
        
    }

    private void LoadAllDialogues()
    {
        var dialoguesObj = Resources.LoadAll("ScriptableObject/Dialogues", typeof(Dialogues));
        foreach (var obj in dialoguesObj)
        {
            if (obj is Dialogues dialogue)
                dialogues.Add(dialogue);
        }
    }

    public List<Dialogues> GetDialogues() => dialogues;

    private string GetVariable(string key)
    {
        return localizedStrings.FirstOrDefault(x => x.name.ToLower() == key.ToLower()).value ?? "ERROR";
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
    
    [ContextMenu("DebugShowDialogue")]
    public void DebugShowDialogue()
    {
        if (debugDialogue != null)
        {
            DisplayDialogue(debugDialogue);
        }
    }

    private void DisplayDialogue(Dialogues dialogue)
    {
        LocalizedString localized = dialogue.GetLocalizedString();
        localized.Arguments = null;

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
        dialogueCanvas.SetActive(true);
        dialogueBox.SetActive(true);
        dialogueText.text = text;
    }

    public string debugVariable;
    
    [ContextMenu("DebugUpdateArguments")]
    public void DebugUpdateArguments()
    {
        UpdateArgument("PLAYER_NAME", debugVariable);
    }

    public void UpdateArgument(string variableName, string variableValue)
    {
        for (int i = 0; i < localizedStrings.Count; i++)
        {
            if (!string.Equals(localizedStrings[i].name, variableName, StringComparison.OrdinalIgnoreCase)) continue;
            
            var temp = localizedStrings[i];
            temp.value = variableValue;
            localizedStrings[i] = temp;
            break;
        }
    }
}
