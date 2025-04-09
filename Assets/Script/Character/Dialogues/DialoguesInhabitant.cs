using System;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Editor;
using LitMotion.Extensions;
using TMPro;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;

public class DialoguesInhabitant : MonoBehaviour
{
    [SerializeField] private Inhabitant inhabitant;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialogueBox;
    
    public List<Dialogues> dialoguesDream = new List<Dialogues>();
    public List<Dialogues> dialoguesInformation = new List<Dialogues>();
    private List<Dialogues> dialoguesReaction = new List<Dialogues>();

    [SerializeField] private Dialogues.Stats stats;
    
    [SerializeField] private InputActionReference touchAction;
    [SerializeField] private InputActionReference positionAction;

    [Header("Animation Settings")]
    private MotionHandle textAnimationHandle;
    [SerializeField] private SerializableMotionSettings<FixedString128Bytes, StringOptions> textAnimationSettings;
    [SerializeField] private float textAnimationSpeed;
    
    private void Start()
    {
        touchAction.action.Enable();
        positionAction.action.Enable();
        touchAction.action.performed += OnTouch;
        GetUIElements();
        GetAllDialogues();
    }

    private void OnEnable()
    {
        touchAction.action.Enable();
        positionAction.action.Enable();
    }
    
    private void OnDisable()
    {
        touchAction.action.Disable();
        positionAction.action.Disable();
    }

    private void OnTouch(InputAction.CallbackContext ctx)
    {
        if (ctx.action.triggered && !dialogueBox.activeSelf)
        {
            Ray ray = Camera.main.ScreenPointToRay(positionAction.action.ReadValue<Vector2>());
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("NPC"))
                {
                    ShowDialogue(Random.Range(1, 3));
                }
            }
        }
    }

    public void ShowDialogue(int type)
    {
        Dialogues dial = null;
        switch (type)
        {
            case 1:
                dial = dialoguesDream[Random.Range(0, dialoguesDream.Count)];
                ShowDialogue(dial);
                UpdateStats(dial);
                ShowStats();
                break;
            case 2:
                dial = dialoguesInformation[Random.Range(0, dialoguesInformation.Count)];
                ShowDialogue(dial);
                UpdateStats(dial);
                ShowStats();
                break;
            case 3:
                dial = dialoguesReaction[Random.Range(0, dialoguesReaction.Count)];
                ShowDialogue(dial);
                UpdateStats(dial);
                ShowStats();
                break;
            default:
                break;
        }
    }

    private void UpdateStats(Dialogues dialogue)
    {
        stats.Mood += dialogue.GetStats().Mood;
        stats.Serenity += dialogue.GetStats().Serenity;
        stats.Energy += dialogue.GetStats().Energy;
        stats.Hearts += dialogue.GetStats().Hearts;
    }
    
    private void ShowDialogue(Dialogues dialogue)
    {
        dialogueBox.SetActive(true);

        textAnimationSettings = textAnimationSettings with
        {
            StartValue = "",
            EndValue = dialogue.GetDialogueText(),
            Duration = textAnimationSpeed
        };
        
        textAnimationHandle = LMotion.String.Create128Bytes(textAnimationSettings.StartValue, textAnimationSettings.EndValue, textAnimationSettings.Duration)
            .BindToText(dialogueText);
    }
    
    public void SetTextAnimationSpeed(float speed)
    {
        textAnimationSpeed = speed;
    }

    public void StopTextAnimation()
    {
        textAnimationHandle.TryComplete();
    }

    private void ShowStats()
    {
        Debug.Log($"Mood : {stats.Mood}");
        Debug.Log($"Serenity : {stats.Serenity}");
        Debug.Log($"Energy : {stats.Energy}");
        Debug.Log($"Hearts : {stats.Hearts}");
    }
    
    
    
    
    
    
    
    // ----------------- Called at START -----------------

    private void GetUIElements()
    {
        dialogueText = GameObject.Find("Dialogue Text").GetComponent<TMP_Text>();
        dialogueBox = GameObject.Find("Dialogue Canvas");
    }
    
    private void GetAllDialogues()
    {
        var dialoguesObj = Resources.LoadAll("Dialogues");
        
        foreach (var obj in dialoguesObj)
        {
            if (obj is Dialogues dialogue)
            {
                switch (dialogue.GetDialogueType())
                {
                    case Dialogues.DialogueType.Reaction:
                        dialoguesReaction.Add(dialogue);
                        break;
                    case Dialogues.DialogueType.Dream:
                        dialoguesDream.Add(dialogue);
                        break;
                    case Dialogues.DialogueType.Information:
                        dialoguesInformation.Add(dialogue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
