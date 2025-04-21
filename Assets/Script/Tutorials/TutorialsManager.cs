using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TutorialsManager : MonoBehaviour
{
    private Player player;
    private DialoguesManager dialoguesManager;
    
    [Header("Tutorials UI")]
    [SerializeField] private GameObject playerFormCanvas;
    
    [Header("Tutorials variables")]
    public float dialoguesTargetDisplayTime;
    public float dialoguesDisplayTime;
    public bool isPlayerCreated = false;
    public bool holdDialogues = false;
    public bool isHouseTutorialAlreadyPlayed = false;


    private void Start()
    {
        player = GM.Instance.player;
        dialoguesManager = GM.Dm;
        player.OnPlayerInfoAssigned += PlayerFormCompleted;
        GM.Instance.OnHouseTuto += UnHoldDialogues;
        IntroductionTutorial();
    }

    private void IntroductionTutorial()
    {
        if (GM.Instance.isPlayerCreated) return;
        
        GM.Instance.mainUiCanvas.SetActive(false);
        playerFormCanvas.SetActive(false);
        
        List<Dialogues> introDialogues = dialoguesManager.GetDialogues()
            .Where(dialogue => dialogue.GetDialogueType() == Dialogues.DialogueType.Introduction)
            .ToList();
        
        introDialogues.Sort((x, y) => string.Compare(x.GetID(), y.GetID(), StringComparison.Ordinal));
        
        StartCoroutine(DisplayTutorialDialogues(introDialogues));
    }
    
    private IEnumerator DisplayTutorialDialogues(List<Dialogues> dialogues)
    {
        foreach (Dialogues dialogue in dialogues)
        {
            if (dialogue.ShouldHoldDialogues())
            {
                holdDialogues = true;

                switch (dialogue.GetTutorialType())
                {
                    case Dialogues.TutorialType.None:
                        playerFormCanvas.SetActive(true);
                        break;
                    case Dialogues.TutorialType.House:
                        Debug.Log(dialogue.GetID());
                        break;
                }
                
                
                yield return new WaitUntil(() => !holdDialogues);
            }
            
            dialoguesManager.DisplayDialogue(dialogue);
            float dif = dialoguesTargetDisplayTime - dialoguesDisplayTime - GM.Ao.CurrentTextSpeedStruct.TextSpeed;
            float textSpeed = dialoguesDisplayTime + GM.Ao.CurrentTextSpeedStruct.TextSpeed + dif;
            yield return new WaitForSeconds(textSpeed);
        }
        
        dialoguesManager.HideDialogue();
        GM.Instance.mainUiCanvas.SetActive(true);
    }

    private void UnHoldDialogues()
    {
        Debug.Log("Unhold dialogues");
        holdDialogues = false;
        isHouseTutorialAlreadyPlayed = true;
    }

    public void PlayerFormCompleted()
    {
        isPlayerCreated = true;
        playerFormCanvas.SetActive(false);
    }

    public void HouseTutorial()
    {
        List<Dialogues> introDialogues = dialoguesManager.GetDialogues()
            .Where(dialogue => dialogue.GetTutorialType() == Dialogues.TutorialType.House)
            .ToList();
        
        introDialogues.Sort((x, y) => string.Compare(x.GetID(), y.GetID(), StringComparison.Ordinal));
        
        StartCoroutine(DisplayTutorialDialogues(introDialogues));
    }
    
    [MenuItem("Tools/PlayHouseTuto")]
    public static void PlayHouseTuto()
    {
        TutorialsManager tutorialsManager = FindObjectOfType<TutorialsManager>();
        if (tutorialsManager != null)
        {
            tutorialsManager.HouseTutorial();
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }
}
