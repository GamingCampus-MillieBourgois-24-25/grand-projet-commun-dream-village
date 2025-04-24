using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TutorialsManager : MonoBehaviour
{
    public enum NosphyPosition
    {
        Position1,
        Position2,
        Position3,
    }
    
    private Player player;
    private DialoguesManager dialoguesManager;
    
    [Header("Tutorials UI")]
    [SerializeField] private GameObject playerFormCanvas;
    [SerializeField] private GameObject mainUi;
    [SerializeField] private GameObject nosphy;
    
    [Header("Tutorials variables")]
    public bool skipDialogue = false;
    public float dialoguesTargetDisplayTime;
    public float dialoguesDisplayTime;
    public bool isPlayerCreated = false;
    public bool holdDialogues = false;
    public bool isHouseTutorialAlreadyPlayed = false;
    public NosphyPosition nosphyPosition;
    
    [Header("Tutorials State")]
    public bool inIntroductionTutorial = false;
    public bool inActivityTutorial = false;
    public bool inDreamTutorial = false;
    public bool inShopTutorial = false;
    public bool inEditTutorial = false;
    public bool inHeartTutorial = false;
    public bool inHouseTutorial = false;
    private int currentTutorialID = 0;

    private void Start()
    {
        player = GM.Instance.player;
        dialoguesManager = GM.Dm;
        player.OnPlayerInfoAssigned += PlayerFormCompleted;
        GM.Instance.OnHouseTuto += UnHoldDialogues;
        PlayAllTutorials();
    }

    private void PlayAllTutorials()
    {
        if (GM.Instance.isPlayerCreated) return;
        
        GM.Instance.mainUiCanvas.SetActive(false);
        playerFormCanvas.SetActive(false);

        List<Dialogues> introDialogues = dialoguesManager.GetDialogues()
            .Where(dialogue => dialogue.GetDialogueType() == Dialogues.DialogueType.Introduction)
            .ToList();
        
        List<Dialogues> tutoDialogues = dialoguesManager.GetDialogues()
            .Where(dialogue => dialogue.GetDialogueType() == Dialogues.DialogueType.Tutorial)
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
            
            skipDialogue = false;
            
            dialoguesManager.DisplayDialogue(dialogue);
            float dif = dialoguesTargetDisplayTime - dialoguesDisplayTime - GM.Ao.CurrentTextSpeedStruct.TextSpeed;
            float textSpeed = dialoguesDisplayTime + GM.Ao.CurrentTextSpeedStruct.TextSpeed + dif;
            
            currentTutorialID = dialogue.GetTutorialID();

            if (dialogue.ShouldHideNosphy())
            {
                nosphy.SetActive(false);
            }
            else
            {
                nosphy.SetActive(true);
            }

            if (dialogue.ShouldHoldDialogues())
            {
                holdDialogues = true;

                if (dialogue.GetTutorialType() == Dialogues.TutorialType.None)
                {
                    playerFormCanvas.SetActive(true);
                }
                
                if (dialogue.GetTutorialType() != Dialogues.TutorialType.None) mainUi.SetActive(true);

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
        playerFormCanvas.SetActive(false);
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
        StopCoroutine(DisplayTutorialDialogues(null));
        
        inActivityTutorial = false;
        inDreamTutorial = false;
        inShopTutorial = false;
        inEditTutorial = false;
        inHeartTutorial = false;
        inHouseTutorial = false;
    }
    
    // ------------------------------------------ DEBUG ------------------------------------------
    
    public void GetTutoDialogues(Dialogues.TutorialType type)
    {
        List<Dialogues> introDialogues = dialoguesManager.GetDialogues()
            .Where(dialogue => dialogue.GetTutorialType() == type)
            .ToList();
        
        introDialogues.Sort((x, y) => string.Compare(x.GetID(), y.GetID(), StringComparison.Ordinal));
        
        StartCoroutine(DisplayTutorialDialogues(introDialogues));
    }
    
    [MenuItem("Tools/Tutorials/PlayAllTuto")]
    public static void PlayAllTuto()
    {
        TutorialsManager tutorialsManager = FindObjectOfType<TutorialsManager>();
        if (tutorialsManager != null)
        {
            tutorialsManager.PlayAllTutorials();
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }
    
    [MenuItem("Tools/Tutorials/PlayHouseTuto")]
    public static void PlayHouseTuto()
    {
        TutorialsManager tutorialsManager = FindObjectOfType<TutorialsManager>();
        if (tutorialsManager != null)
        {
            tutorialsManager.GetTutoDialogues(Dialogues.TutorialType.House);
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }
    
    [MenuItem("Tools/Tutorials/PlayActivityTuto")]
    public static void PlayActivityTuto()
    {
        TutorialsManager tutorialsManager = FindObjectOfType<TutorialsManager>();
        if (tutorialsManager != null)
        {
            tutorialsManager.GetTutoDialogues(Dialogues.TutorialType.Activity);
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }
    
    [MenuItem("Tools/Tutorials/PlayDreamTuto")]
    public static void PlayDreamTuto()
    {
        TutorialsManager tutorialsManager = FindObjectOfType<TutorialsManager>();
        if (tutorialsManager != null)
        {
            tutorialsManager.GetTutoDialogues(Dialogues.TutorialType.Dream);
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }
    
    [MenuItem("Tools/Tutorials/PlayShopTuto")]
    public static void PlayShopTuto()
    {
        TutorialsManager tutorialsManager = FindObjectOfType<TutorialsManager>();
        if (tutorialsManager != null)
        {
            tutorialsManager.GetTutoDialogues(Dialogues.TutorialType.Shop);
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }
    
    [MenuItem("Tools/Tutorials/PlayEditTuto")]
    public static void PlayEditTuto()
    {
        TutorialsManager tutorialsManager = FindObjectOfType<TutorialsManager>();
        if (tutorialsManager != null)
        {
            tutorialsManager.GetTutoDialogues(Dialogues.TutorialType.Edit);
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }
    
    [MenuItem("Tools/Tutorials/PlayHeartTuto")]
    public static void PlayHeartTuto()
    {
        TutorialsManager tutorialsManager = FindObjectOfType<TutorialsManager>();
        if (tutorialsManager != null)
        {
            tutorialsManager.GetTutoDialogues(Dialogues.TutorialType.Heart);
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }
}
