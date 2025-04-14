using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class DialoguesManager : MonoBehaviour
{
    public static DialoguesManager Instance;
    
    [SerializeField] private List<Dialogues> dialogues = new List<Dialogues>();
    
    [Serializable]
    public struct DictStrings
    {
        public string varName;
        public string ID => varName;
        public string variable;
    }
    
    public List<DictStrings> localizedStrings;
    
    [Header("UI Elements")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        LoadAllDialogues();
        CompleteDialogueArguments();
    }
    
    private void LoadAllDialogues()
    {
        var dialoguesObj = Resources.LoadAll("Dialogues");
        
        foreach (var obj in dialoguesObj)
        {
            if (obj is Dialogues dialogue)
            {
                dialogues.Add(dialogue);
            }
        }
    }
    
    public List<Dialogues> GetDialogues()
    {
        return dialogues;
    }

    [ContextMenu("CompleteDialogueArguments")]
    public void CompleteDialogueArguments()
    {
        foreach (var dial in dialogues)
        {
            Dictionary<string, string> smartArgs = new Dictionary<string, string>();

            foreach (var truc in localizedStrings)
            {
                smartArgs[truc.varName] = truc.variable;
            }

            dial.GetLocalizedString().Arguments = new object[] { smartArgs };
            
            dial.GetLocalizedString().StringChanged += (localizedText) =>
            {
                Debug.Log(localizedText);
            };
        }
    }

    [ContextMenu("ShowIntroDialogue")]
    public void ShowIntroDialogue()
    {
        foreach (var dial in dialogues)
        {
            if (dial.GetDialogueType() == Dialogues.DialogueType.Introduction)
            {
                Debug.Log(dial.GetDialogueText());
            }
        }
    }


}
