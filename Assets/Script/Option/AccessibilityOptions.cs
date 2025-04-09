using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class AccessibilityOptions : MonoBehaviour
{
    public static AccessibilityOptions Instance;
    
    #region Language
    
    public enum LanguageParameter
    {
        English,
        French
    }
    
    public LanguageParameter currentLanguage = LanguageParameter.English;
    
    public TMP_Dropdown languageDropdown;
    public RectTransform languageDropdownTemplate;
    public RectTransform languageDropdownViewport;
    public float languageDropdownHeight = 150f;
    
    private bool _localizationActive = false;
    #endregion
    
    #region Text Speed
    
    public List<GameObject> textSpeedButtons;
    
    public enum TextSpeedParameter
    {
        Slow,
        Normal,
        Fast
    }
    
    public struct TextSpeedStruct
    {
        public TextSpeedParameter TextSpeedParameter;
        public float TextSpeed;
    }

    private TextSpeedStruct[] _textSpeedParameters = new TextSpeedStruct[3]
    {
        new TextSpeedStruct {TextSpeedParameter = TextSpeedParameter.Slow, TextSpeed = 3f},
        new TextSpeedStruct {TextSpeedParameter = TextSpeedParameter.Normal, TextSpeed = 2f},
        new TextSpeedStruct {TextSpeedParameter = TextSpeedParameter.Fast, TextSpeed = 1f}
    };
    
    public TextSpeedStruct CurrentTextSpeedStruct;
    
    #endregion

    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitLanguageDropdown();

        SetTextSpeedParameter(CurrentTextSpeedStruct.TextSpeedParameter == TextSpeedParameter.Slow ? 0 : CurrentTextSpeedStruct.TextSpeedParameter == TextSpeedParameter.Normal ? 1 : 2);
    }
    
    private void InitLanguageDropdown()
    {
        languageDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (var lang in LocalizationSettings.AvailableLocales.Locales)
        {
            options.Add(lang.ToString());
        }
        languageDropdown.AddOptions(options);
        languageDropdownTemplate.sizeDelta = new Vector2(0, languageDropdownHeight * options.Count);
        languageDropdownViewport.sizeDelta = new Vector2(languageDropdownViewport.rect.width, languageDropdownHeight * options.Count);
        languageDropdown.value = (int) currentLanguage;
    }

    public void SetLanguageParameter(int value)
    {
        if (_localizationActive) return;
        currentLanguage = (LanguageParameter) value;
        StartCoroutine(SetLocale(value));
    }
    
    public void SetTextSpeedParameter(int value)
    {
        CurrentTextSpeedStruct = _textSpeedParameters[value];
        ChangeTextSpeedButtonColor(value);
    }

    private void ChangeTextSpeedButtonColor(int value)
    {
        for (int i = 0; i < textSpeedButtons.Count; i++)
        {
            textSpeedButtons[i].GetComponent<Image>().color = i == value ? Color.yellow : Color.white;
        }
    }
    
    IEnumerator SetLocale(int localeID)
    {
        _localizationActive = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale=LocalizationSettings.AvailableLocales.Locales[localeID];
        PlayerPrefs.SetInt("LocaleKey", localeID);
        _localizationActive = false;
    }
}
