using System;
using UnityEngine;

public class DialoguesTest : MonoBehaviour
{
    public delegate void DialogueDelegate(string id);
    private DialogueDelegate selectDialogueByIDDelegate;

    public DialoguesInhabitant dialInhabitant;
    public string dialogueID;
    
    

    private void Start()
    {
        if (dialInhabitant != null)
        {
            selectDialogueByIDDelegate = dialInhabitant.SelectDialogueByID;
        }
    }

    [ContextMenu("CallTest")]
    public void CallTest()
    {
        selectDialogueByIDDelegate.Invoke(dialogueID);
    }
}