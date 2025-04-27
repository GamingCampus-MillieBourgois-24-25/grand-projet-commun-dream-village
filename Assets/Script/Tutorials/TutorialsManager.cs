using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitMotion;
using UnityEngine;

public class TutorialsManager : MonoBehaviour
{
    private Player player;
    private DialoguesManager dialoguesManager;
    private TutorialsUI tutorialsUI;
    

    [Header("Tutorials variables")] 
    public bool skipDialogue = false;
    public float dialoguesTargetDisplayTime;
    public float dialoguesDisplayTime;
    public bool isPlayerCreated = false;
    public bool holdDialogues = false;
    public bool isHouseTutorialAlreadyPlayed = false;
    
    private MotionHandle highlightAnimationHandle;
    
    [Header("Tutorials State")]
    public bool inIntroductionTutorial = false;
    public bool inActivityTutorial = false;
    public bool inDreamTutorial = false;
    public bool inShopTutorial = false;
    public bool inEditTutorial = false;
    public bool inHeartTutorial = false;
    public bool inHouseTutorial = false;
    public int currentTutorialID = 0;

    private void Start()
    {
        tutorialsUI = GetComponent<TutorialsUI>();
        player = GM.Instance.player;
        dialoguesManager = GM.Dm;
        player.OnPlayerInfoAssigned += PlayerFormCompleted;
        GM.Instance.OnHouseTuto += UnHoldDialogues;
        PlayAllTutorials();
    }

    public void PlayAllTutorials()
    {
        if (GM.Instance.isPlayerCreated) return;
        
        GM.Instance.mainUiCanvas.SetActive(false);
        tutorialsUI.playerFormCanvas.SetActive(false);

        List<Dialogues> introDialogues = dialoguesManager.GetDialogues()
            .Where(dialogue => dialogue.GetDialogueType() == Dialogues.DialogueType.Introduction)
            .ToList();
        
        List<Dialogues> tutoDialogues = dialoguesManager.GetDialogues()
            .Where(dialogue => dialogue.GetDialogueType() == Dialogues.DialogueType.Tutorial && dialogue.ShouldBePlayed())
            .ToList();

        List<Dialogues> dialogues = introDialogues.ToList();

        dialogues.AddRange(tutoDialogues);

        dialogues.Sort((x, y) => x.GetTutorialID().CompareTo(y.GetTutorialID()));
        
        StartCoroutine(DisplayTutorialDialogues(dialogues));
    }
    
    private IEnumerator DisplayTutorialDialogues(List<Dialogues> dialogues)
    {
        foreach (Dialogues dialogue in dialogues)
        {
            SetCurrentTutorial(dialogue.GetTutorialType());
            
            if (highlightAnimationHandle.IsPlaying())
            {
                highlightAnimationHandle.TryComplete();
                highlightAnimationHandle.Cancel();
            }

            if (dialogue.ShouldBlockInteractions())
            {
                tutorialsUI.blockPanel.SetActive(true);
            }
            else
            {
                tutorialsUI.blockPanel.SetActive(false);
            }
            
            skipDialogue = false;
            
            dialoguesManager.DisplayDialogue(dialogue);
            float dif = dialoguesTargetDisplayTime - dialoguesDisplayTime - GM.Ao.CurrentTextSpeedStruct.TextSpeed;
            float textSpeed = dialoguesDisplayTime + GM.Ao.CurrentTextSpeedStruct.TextSpeed + dif;

            TutorialsButtonFeedback(dialogue);
            
            currentTutorialID = dialogue.GetTutorialID();

            if (dialogue.ShouldHoldDialogues())
            {
                holdDialogues = true;

                if (dialogue.GetTutorialType() == Dialogues.TutorialType.None)
                {
                    tutorialsUI.playerFormCanvas.SetActive(true);
                }
                
                if (dialogue.GetTutorialType() != Dialogues.TutorialType.None) tutorialsUI.mainUi.SetActive(true);

                yield return new WaitUntil(() => !holdDialogues);
            }

            float elapsedTime = 0f;
            while (elapsedTime < textSpeed && (!skipDialogue && !dialogue.ShouldHoldDialogues()))
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
        }

        dialoguesManager.HideDialogue();
        GM.Instance.mainUiCanvas.SetActive(true);
    }

    private void SetCurrentTutorial(Dialogues.TutorialType type)
    {
        inIntroductionTutorial = false;
        inHouseTutorial = false;
        inDreamTutorial = false;
        inActivityTutorial = false;
        inShopTutorial = false;
        inEditTutorial = false;
        inHeartTutorial = false;

        switch (type)
        {
            case Dialogues.TutorialType.None:
                inIntroductionTutorial = true;
                break;
            case Dialogues.TutorialType.House:
                inHouseTutorial = true;
                break;
            case Dialogues.TutorialType.Dream:
                inDreamTutorial = true;
                break;
            case Dialogues.TutorialType.Activity:
                inActivityTutorial = true;
                break;
            case Dialogues.TutorialType.Shop:
                inShopTutorial = true;
                break;
            case Dialogues.TutorialType.Edit:
                inEditTutorial = true;
                break;
            case Dialogues.TutorialType.Heart:
                inHeartTutorial = true;
                break;
            default:
                break;
        }
    }

    private void TutorialsButtonFeedback(Dialogues dialogue)
    {
        void HighlightButton(GameObject button, bool shouldHighlight)
        {
            if (shouldHighlight)
            {
                highlightAnimationHandle = LMotion.Create(1, 1.1f, 0.5f)
                    .WithLoops(-1, LoopType.Yoyo)
                    .WithOnCancel(() => button.transform.localScale = Vector3.one)
                    .Bind(x => button.transform.localScale = new Vector3(x, x, 1));
            }
        }

        HighlightButton(tutorialsUI.journalRightPage, dialogue.highlightJournalRightPage);
        HighlightButton(tutorialsUI.journalStats, dialogue.highlightJournalStats);
        HighlightButton(tutorialsUI.quitJournalButton, dialogue.highlightQuitJournalButton);
        HighlightButton(tutorialsUI.nightButton, dialogue.highlightNightButton);
        HighlightButton(tutorialsUI.dreamButton, dialogue.highlightDreamButton);
        HighlightButton(tutorialsUI.shopButton, dialogue.highlightShopButton);
        HighlightButton(tutorialsUI.quitShopButton, dialogue.highlightQuitShopButton);
        HighlightButton(tutorialsUI.editButton, dialogue.highlightEditButton);
        HighlightButton(tutorialsUI.quitEditButton, dialogue.highlightQuitEditButton);
    }

    public void UnHold(int value)
    {
        if (value != currentTutorialID) return;

        holdDialogues = false;
    }

    public void UnHoldDialogues()
    {
        holdDialogues = false;
    }
    
    public void PlayerFormCompleted()
    {
        isPlayerCreated = true;
        tutorialsUI.playerFormCanvas.SetActive(false);
    }

    public void SkipDialogue()
    {
        if (GM.Dm.isTextAnimationActive)
        {
            return;
        }
        
        skipDialogue = true;
    }
    
    public void SkipTutorial()
    {
        if (inIntroductionTutorial) return;
        
        dialoguesManager.HideDialogue();
        StopAllCoroutines();
        
        inActivityTutorial = false;
        inDreamTutorial = false;
        inShopTutorial = false;
        inEditTutorial = false;
        inHeartTutorial = false;
        inHouseTutorial = false;
    }

    public void GetTutoDialogues(Dialogues.TutorialType type)
    {
        List<Dialogues> introDialogues = dialoguesManager.GetDialogues()
            .Where(dialogue => dialogue.GetTutorialType() == type)
            .ToList();

        introDialogues.Sort((x, y) => string.Compare(x.GetID(), y.GetID(), StringComparison.Ordinal));

        StartCoroutine(DisplayTutorialDialogues(introDialogues));
    }
}
