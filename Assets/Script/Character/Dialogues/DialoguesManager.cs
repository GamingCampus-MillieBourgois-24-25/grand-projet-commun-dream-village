using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialoguesManager : MonoBehaviour
{
    public static DialoguesManager Instance;
    
    private List<Dialogues> dialogues = new List<Dialogues>();
    
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
}
