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
    public bool skipDialogue = false;
    public float dialoguesTargetDisplayTime;
    public float dialoguesDisplayTime;
    public bool isPlayerCreated = false;
    public bool holdDialogues = false;
    public bool isHouseTutorialAlreadyPlayed = false;
    
    [Header("Tutorials State")]
    public bool inActivityTutorial = false;
    public bool inDreamTutorial = false;
    public bool inShopTutorial = false;
    public bool inEditTutorial = false;
    public bool inHeartTutorial = false;
    public bool inHouseTutorial = false;


    private void Start()
    {
        player = GM.Instance.player;
        dialoguesManager = GM.Dm;
        player.OnPlayerInfoAssigned += PlayerFormCompleted;
        GM.Instance.OnHouseTuto += UnHoldHouseTuto;
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
            skipDialogue = false;
            
            dialoguesManager.DisplayDialogue(dialogue);
            float dif = dialoguesTargetDisplayTime - dialoguesDisplayTime - GM.Ao.CurrentTextSpeedStruct.TextSpeed;
            float textSpeed = dialoguesDisplayTime + GM.Ao.CurrentTextSpeedStruct.TextSpeed + dif;

            if (dialogue.ShouldHoldDialogues())
            {
                holdDialogues = true;

                switch (dialogue.GetTutorialType())
                {
                    case Dialogues.TutorialType.None:
                        playerFormCanvas.SetActive(true);
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

    public void UnHoldDialogues()
    {
        Debug.Log("Unhold dialogues");
        holdDialogues = false;
    }
    
    public void PlayerFormCompleted()
    {
        isPlayerCreated = true;
        playerFormCanvas.SetActive(false);
    }

    public void GetTutoDialogues(Dialogues.TutorialType type)
    {
        List<Dialogues> introDialogues = dialoguesManager.GetDialogues()
            .Where(dialogue => dialogue.GetTutorialType() == type)
            .ToList();
        
        introDialogues.Sort((x, y) => string.Compare(x.GetID(), y.GetID(), StringComparison.Ordinal));
        
        StartCoroutine(DisplayTutorialDialogues(introDialogues));
    }

    public void SkipDialogue()
    {
        if (GM.Dm.isTextAnimationActive)
        {
            return;
        }
        
        skipDialogue = true;
    }

    public void UnHoldHouseTuto()
    {
        if (!inHouseTutorial) return;
        
        UnHoldDialogues();
    }
    
    public void UnHoldDreamTuto()
    {
        if (!inDreamTutorial) return;
        
        UnHoldDialogues();
    }
    
    public void UnHoldActivityTuto()
    {
        if (!inActivityTutorial) return;
        
        UnHoldDialogues();
    }
    
    public void UnHoldShopTuto()
    {
        if (!inShopTutorial) return;
        
        UnHoldDialogues();
    }
    
    public void UnHoldEditTuto()
    {
        if (!inEditTutorial) return;
        
        UnHoldDialogues();
    }
    
    public void UnHoldHeartTuto()
    {
        if (!inHeartTutorial) return;
        
        UnHoldDialogues();
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
