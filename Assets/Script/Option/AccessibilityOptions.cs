using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccessibilityOptions : MonoBehaviour
{
    public enum LanguageParameter
    {
        English,
        French,
        Spanish,
        Deutsch
    }
    
    public LanguageParameter currentLanguage = LanguageParameter.English;
    
    public enum TextSpeedParameter
    {
        Slow,
        Normal,
        Fast
    }
    
    public TextSpeedParameter currentTextSpeed = TextSpeedParameter.Normal;
    
    [Header("Interface")]
    
    #region Language Dropdown
    public TMP_Dropdown languageDropdown;
    public RectTransform languageDropdownTemplate;
    public RectTransform languageDropdownViewport;
    public float languageDropdownHeight = 150f;
    #endregion
    
    #region Text Speed
    public List<GameObject> textSpeedButtons;
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        InitLanguageDropdown();

        SetTextSpeedParameter((int) currentTextSpeed);
    }
    
    private void InitLanguageDropdown()
    {
        languageDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (var lang in Enum.GetValues(typeof(LanguageParameter)))
        {
            options.Add(lang.ToString());
        }
        languageDropdown.AddOptions(options);
        languageDropdownTemplate.sizeDelta = new Vector2(0, languageDropdownHeight * options.Count);
        languageDropdownViewport.sizeDelta = new Vector2(languageDropdownViewport.rect.width, languageDropdownHeight * options.Count);
    }

    public void SetLanguageParameter(int value)
    {
        currentLanguage = (LanguageParameter) value;
    }
    
    public void SetTextSpeedParameter(int value)
    {
        currentTextSpeed = (TextSpeedParameter) value;
        ChangeTextSpeedButtonColor(value);
    }

    private void ChangeTextSpeedButtonColor(int value)
    {
        for (int i = 0; i < textSpeedButtons.Count; i++)
        {
            textSpeedButtons[i].GetComponent<Image>().color = i == value ? Color.yellow : Color.white;
        }
    }
}
