using TMPro;
using UnityEngine;

public class DialoguesTest : MonoBehaviour
{
    public Dialogues dialogues;
    
    public TMP_Text dialogueText;
    
    private void Start()
    {
        dialogueText.text = dialogues.GetDialogueText();
    }
}
