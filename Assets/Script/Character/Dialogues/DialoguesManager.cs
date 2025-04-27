using System;
using System.Collections.Generic;
using System.Linq;
using LitMotion;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using LitMotion.Extensions;
using Unity.Collections;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public enum NosphyPosition
{
    Position1,
    Position2,
    Position3,
}

public class DialoguesManager : MonoBehaviour
{

    [SerializeField] private List<Dialogues> dialogues = new();
    

    
    [Serializable]
    public struct DictStrings
    {
        public string name;
        public string value;
        public LocalizedString localizedValue;
    }

    [SerializeField] private List<DictStrings> localizedArguments;
    
    [Header("Text Animation")]
    private MotionHandle textAnimationHandle;
    public bool isTextAnimationActive;
    [SerializeField] private SerializableMotionSettings<FixedString512Bytes, StringOptions> textAnimationSettings;

    [Header("UI Elements")] 
    public GameObject dialogueCanvas;
    public GameObject dialogueBox;
    public GameObject buttonContainer;
    public GameObject dialogueSkip;
    public GameObject dialogueHide;
    public GameObject topDialogueBoxPosition;
    public GameObject bottomDialogueBoxPosition;
    public GameObject hiddenTopDialogueBoxPosition;
    public GameObject hiddenBottomDialogueBoxPosition;
    public GameObject topButtonPosition;
    public GameObject bottomButtonPosition;
    public GameObject hiddenTopButtonPosition;
    public GameObject hiddenBottomButtonPosition;
    
    public bool isDialogueBoxHidden = false;
    public TMP_Text dialogueText;
    public NosphyPosition nosphyPosition;
    [SerializeField] private GameObject nosphy;
    public bool showNosphy = false;
    public bool isTop = false;
    [SerializeField] private Sprite nosphySprite1;
    [SerializeField] private Sprite nosphySprite2;
    [SerializeField] private LocalizedString showString;
    [SerializeField] private LocalizedString hideString;

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
        var dictString = localizedArguments.FirstOrDefault(x => x.name.ToLower() == key.ToLower());
        if (dictString.value == "" && dictString.localizedValue != null)
        {
            return dictString.localizedValue.GetLocalizedString();
        }
        return dictString.value ?? "ERROR";
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
    
    public void HideDialogue()
    {
        dialogueCanvas.SetActive(false);
        dialogueBox.SetActive(false);
        textAnimationHandle.TryComplete();
    }

    public void DisplayDialogue(Dialogues dialogue)
    {
        isTop = dialogue.IsDialogueBoxTop();

        switch (dialogue.GetNosphyPosition())
        {
            case NosphyPosition.Position1:
                showNosphy = true;
                nosphy.GetComponent<Image>().sprite = nosphySprite1;
                break;
            case NosphyPosition.Position2:
                showNosphy = true;
                nosphy.GetComponent<Image>().sprite = nosphySprite2;
                break;
            case NosphyPosition.Position3:
                showNosphy = false;
                break;
            default:
                showNosphy = true;
                break;
        }
        
        nosphy.SetActive(showNosphy);
        
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
        dialogueBox.transform.localPosition = isTop ? topDialogueBoxPosition.transform.localPosition : bottomDialogueBoxPosition.transform.localPosition;
        buttonContainer.transform.localPosition = isTop ? topButtonPosition.transform.localPosition : bottomButtonPosition.transform.localPosition;
        
        float charactersPerSecond = 10f;
        float textSpeed = text.Length / GM.Ao.CurrentTextSpeedStruct.TextSpeed;

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
    
    public void HideDialogueBox()
    {
        isDialogueBoxHidden = !isDialogueBoxHidden;
        
        nosphy.SetActive(showNosphy);

        if (isTop)
        {
            dialogueBox.GetComponent<RectTransform>().anchoredPosition = isDialogueBoxHidden
                ? hiddenTopDialogueBoxPosition.GetComponent<RectTransform>().anchoredPosition
                : topDialogueBoxPosition.GetComponent<RectTransform>().anchoredPosition;
            
            dialogueBox.transform.localPosition = isDialogueBoxHidden
                ? hiddenTopDialogueBoxPosition.transform.localPosition
                : topDialogueBoxPosition.transform.localPosition;
            
            buttonContainer.transform.localPosition = isDialogueBoxHidden ? hiddenTopButtonPosition.transform.localPosition : topButtonPosition.transform.localPosition;
        }
        else
        {
            dialogueBox.GetComponent<RectTransform>().anchoredPosition = isDialogueBoxHidden
                ? hiddenBottomDialogueBoxPosition.GetComponent<RectTransform>().anchoredPosition
                : bottomDialogueBoxPosition.GetComponent<RectTransform>().anchoredPosition;
            
            dialogueBox.transform.localPosition = isDialogueBoxHidden
                ? hiddenBottomDialogueBoxPosition.transform.localPosition
                : bottomDialogueBoxPosition.transform.localPosition;
            
            buttonContainer.transform.localPosition = isDialogueBoxHidden ? hiddenBottomButtonPosition.transform.localPosition : bottomButtonPosition.transform.localPosition;
        }
    }
}
