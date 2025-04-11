using System;
using TMPro;

using UnityEngine;

public class DialoguesTest : MonoBehaviour
{
    public delegate void DialogueDelegate(string id);
    private DialogueDelegate selectDialogueByIDDelegate;

    public DialoguesInhabitant dialInhabitant;
    public string dialogueID;

    // =============== PROTOTYPE
    public static event Action<Building> OnBuildingPlaced;
    public Building buildingTest;

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
    
    
    // =============== PROTOTYPE
    [ContextMenu("PlaceBuildingTest")]
    public void PlaceBuildingTest()
    {
        OnBuildingPlaced?.Invoke(buildingTest);
    }
}