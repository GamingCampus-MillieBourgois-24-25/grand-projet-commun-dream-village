using System;
using System.Collections.Generic;
using System.Linq;
using LitMotion;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Serialization;
using LitMotion;
using LitMotion.Extensions;
using Unity.Collections;

public class DialoguesManager : MonoBehaviour
{

    [SerializeField] private List<Dialogues> dialogues = new();
    
    [Serializable]
    public struct DictStrings
    {
        public string name;
        public string value;
    }

    [SerializeField] private List<DictStrings> localizedArguments;
    
    [Header("Text Animation")]
    private MotionHandle textAnimationHandle;
    private bool isTextAnimationActive;
    [SerializeField] private SerializableMotionSettings<FixedString512Bytes, StringOptions> textAnimationSettings;

    [Header("UI Elements")] public GameObject dialogueCanvas;
    public GameObject dialogueBox;
    public TMP_Text dialogueText;

    private void Awake()
    {
        LoadAllDialogues();
    }

    private void LoadAllDialogues()
    {
        var dialoguesObj = Resources.LoadAll("ScriptableObject/Dialogues", typeof(Dialogues));
        foreach (var obj in dialoguesObj)
        {
            if (obj is Dialogues dialogue)
                dialogues.Add(dialogue);
        }
    }

    public List<Dialogues> GetDialogues() => dialogues;

    private string GetVariable(string key)
    {
        return localizedArguments.FirstOrDefault(x => x.name.ToLower() == key.ToLower()).value ?? "ERROR";
    }

    [ContextMenu("ShowIntroDialogue")]
    public void ShowIntroDialogue()
    {
        var introDialogue = dialogues.FirstOrDefault(d => d.GetDialogueType() == Dialogues.DialogueType.Introduction);
        if (introDialogue != null)
        {
            DisplayDialogue(introDialogue);
        }
    }

    public Dialogues debugDialogue;
    
    [ContextMenu("DebugShowDialogue")]
    public void DebugShowDialogue()
    {
        if (debugDialogue != null)
        {
            DisplayDialogue(debugDialogue);
        }
    }
    
    public void HideDialogue()
    {
        dialogueCanvas.SetActive(false);
        dialogueBox.SetActive(false);
        textAnimationHandle.TryComplete();
    }

    public void DisplayDialogue(Dialogues dialogue)
    {
        LocalizedString localized = dialogue.GetLocalizedString();
        localized.Arguments = null;

        var args = new List<object>();
        foreach (var argKey in dialogue.GetRequiredArguments())
        {
            args.Add(GetVariable(argKey));
        }

        localized.Arguments = args.ToArray();

        DisplayInBox(localized.GetLocalizedString());
    }

    private void DisplayInBox(string text)
    {
        dialogueCanvas.SetActive(true);
        dialogueBox.SetActive(true);
        
        float textSpeed = GM.Ao.CurrentTextSpeedStruct.TextSpeed;
        
        textAnimationSettings = textAnimationSettings with
        {
            StartValue = "",
            EndValue = text,
            Duration = textSpeed,
        };
        isTextAnimationActive = true;
        textAnimationHandle = LMotion.String.Create512Bytes(textAnimationSettings.StartValue, textAnimationSettings.EndValue, textAnimationSettings.Duration)
            .BindToText(dialogueText);
    }

    public void UpdateArgument(Dialogues dialogues, string variableName, string variableValue)
    {
        for (int i = 0; i < dialogues.GetLocalizedString().Count; i++)
        {
            if (!string.Equals(localizedArguments[i].name, variableName, StringComparison.OrdinalIgnoreCase)) continue;
            
            var temp = localizedArguments[i];
            temp.value = variableValue;
            localizedArguments[i] = temp;
            break;
        }
    }

    public void UpdateLocalizedArguments(string variableName, string variableValue)
    {
        for (int i = 0; i < localizedArguments.Count; i++)
        {
            if (!string.Equals(localizedArguments[i].name, variableName, StringComparison.OrdinalIgnoreCase)) continue;
            
            var temp = localizedArguments[i];
            temp.name = variableName;
            temp.value = variableValue;
            localizedArguments[i] = temp;
            break;
        }
    }
    
    public void StopTextAnimation()
    {
        textAnimationHandle.TryComplete();
        isTextAnimationActive = false;
    }
}
