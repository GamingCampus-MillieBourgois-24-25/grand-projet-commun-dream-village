using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

public class DialoguesTest : MonoBehaviour
{
    public bool showDialogue;
    [Range(1, 3)] public int type;
    public DialoguesInhabitantTest dialInhabitant;

    private void Update()
    {
        if (showDialogue)
        {
            dialInhabitant.ShowDialogue(type);
            
            showDialogue = false;
        }
    }
}
