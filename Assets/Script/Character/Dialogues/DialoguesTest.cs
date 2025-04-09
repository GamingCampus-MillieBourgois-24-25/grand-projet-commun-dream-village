using UnityEngine;

public class DialoguesTest : MonoBehaviour
{
    public bool showDialogue;
    [Range(1, 3)] public int type;
    public DialoguesInhabitant dialInhabitant;

    private void Update()
    {
        if (showDialogue)
        {
            dialInhabitant.ShowDialogue(type);
            
            showDialogue = false;
        }
    }
}
