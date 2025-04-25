using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class AccessibilityOptions : MonoBehaviour
{
    
    #region Language
    
    
    public TMP_Dropdown languageDropdown;
    public RectTransform languageDropdownTemplate;
    public RectTransform languageDropdownViewport;
    public float languageDropdownHeight = 150f;
    
    private bool _localizationActive = false;
    #endregion
    
    #region Text Speed
    
    public List<GameObject> textSpeedButtons;
    [SerializeField] private Sprite unselectedButtonSpeed;
    [SerializeField] private Sprite selectedButtonSpeed;
    
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
    
    private void Awake()
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
        
        int savedLocaleKey = PlayerPrefs.GetInt("LocaleKey", -1);
        if (savedLocaleKey == -1)
        {
            var systemLanguage = Application.systemLanguage.ToString();
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                if (LocalizationSettings.AvailableLocales.Locales[i].Identifier.Code.Equals(systemLanguage, System.StringComparison.OrdinalIgnoreCase))
                {
                    languageDropdown.value = i;
                    SetLocale(i);
                    PlayerPrefs.SetInt("LocaleKey", i);
                    break;
                }
            }
        }
        else
        {
            languageDropdown.value = savedLocaleKey;
            SetLocale(savedLocaleKey);
        }
    }

    public void SetLanguageParameter(int value)
    {
        if (_localizationActive) return;
        SetLocale(value);
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
            if (i == value)
            {
                textSpeedButtons[i].GetComponent<Image>().sprite = selectedButtonSpeed;
            }
            else
            {
                textSpeedButtons[i].GetComponent<Image>().sprite = unselectedButtonSpeed;
            }
        }
    }
    
    private void SetLocale(int localeID)
    {
        _localizationActive = true;
        LocalizationSettings.SelectedLocale=LocalizationSettings.AvailableLocales.Locales[localeID];
        PlayerPrefs.SetInt("LocaleKey", localeID);
        _localizationActive = false;
    }
}
