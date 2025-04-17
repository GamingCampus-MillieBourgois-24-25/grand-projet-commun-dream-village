using System.Collections.Generic;
using System.Linq;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;

public class DialoguesInhabitant : MonoBehaviour
{
    [Header("Inhabitant Settings")]
    [SerializeField] private Inhabitant inhabitant;
    [SerializeField] private Dialogues.Stats stats;
    
    
    
    private DialoguesManager dialoguesManager;
    
    private List<Dialogues> dialogues = new List<Dialogues>();
    
    private AccessibilityOptions accessibilityOptions;

    [Header("Animation Settings")]
    private MotionHandle textAnimationHandle;
    [SerializeField] private SerializableMotionSettings<FixedString128Bytes, StringOptions> textAnimationSettings;
    [SerializeField] private float textAnimationSpeed;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text dialogueText;
    
    private void Start()
    {
        accessibilityOptions = AccessibilityOptions.Instance;
        dialoguesManager = DialoguesManager.Instance;

        if (dialoguesManager)
        {
            GetUIElements();
            dialogues = dialoguesManager.GetDialogues();
        }
    }
    
    // React to building placement ======= PROTOTYPE
    private void ReactToBuildingPlacement(Building building)
    {
        if (building.name == "Bench")
        {
            SelectDialogueByID("Reac001");
        }
    }

    public void SelectDialogue(int type)
    {
        Dialogues dial = null;
        switch (type)
        {
            case 1:
                dial = dialogues.Where(x => x.GetDialogueType() == Dialogues.DialogueType.Dream)
                    .OrderBy(x => Random.value)
                    .FirstOrDefault();
                ShowDialogue(dial);
                UpdateStats(dial);
                ShowStats();
                break;
            case 2:
                dial = dialogues.Where(x => x.GetDialogueType() == Dialogues.DialogueType.Information)
                    .OrderBy(x => Random.value)
                    .FirstOrDefault();
                ShowDialogue(dial);
                UpdateStats(dial);
                ShowStats();
                break;
            case 3:
                dial = dialogues.Where(x => x.GetDialogueType() == Dialogues.DialogueType.Reaction)
                    .OrderBy(x => Random.value)
                    .FirstOrDefault();
                ShowDialogue(dial);
                UpdateStats(dial);
                ShowStats();
                break;
            default:
                break;
        }
    }
    
    public void SelectDialogueByID(string id)
    {
        Dialogues dial = null;
        
        dial = dialogues.Where(x => x.GetID() == id)
            .OrderBy(x => Random.value)
            .FirstOrDefault();
        
        if (dial == null)
        {
            Debug.LogError($"Dialogue with ID {id} not found.");
            return;
        }
        
        ShowDialogue(dial);
        UpdateStats(dial);
        ShowStats();
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
        
        textAnimationSpeed = accessibilityOptions.CurrentTextSpeedStruct.TextSpeed;
        
        textAnimationSettings = textAnimationSettings with
        {
            StartValue = "",
            EndValue = dialogue.GetDialogueText(),
            Duration = textAnimationSpeed
        };
        
        textAnimationHandle = LMotion.String.Create128Bytes(textAnimationSettings.StartValue, textAnimationSettings.EndValue, textAnimationSettings.Duration)
            .BindToText(dialogueText);
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
        dialogueText = dialoguesManager.dialogueText;
        dialogueBox = dialoguesManager.dialogueBox;
    }
}
