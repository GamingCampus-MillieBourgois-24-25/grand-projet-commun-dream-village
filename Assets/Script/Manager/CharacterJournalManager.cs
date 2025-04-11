using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CharacterJournalManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text pronounsText;
    public TMP_Text mbtiText;
    public TMP_Text personalitiesText;
    public Transform likesContainer;
    public Transform dislikesContainer;
    public TMP_Text GoldMultiplierText;

    [Header("UI Elements - Statistiques")]
    public Slider moodSlider;
    public Slider serenitySlider;
    public Slider energySlider;
    public Transform heartsContainer;

    [Header("UI Elements - Navigation")]
    public TMP_Text indexText;
    public Button nextButton;
    public Button previousButton;

    private List<InhabitantInstance> inhabitants;
    private int currentIndex = 0;

    private void Start()
    {
        inhabitants = VillageManager.instance.inhabitants;

        nextButton.onClick.AddListener(ShowNext);
        previousButton.onClick.AddListener(ShowPrevious);

        DisplayInhabitant();
    }

    private void DisplayInhabitant()
    {
        if (inhabitants.Count == 0) return;

        InhabitantInstance currentInhabitant = inhabitants[currentIndex];

        iconImage.sprite = currentInhabitant.Icon;
        nameText.text = $"{currentInhabitant.FirstName} {currentInhabitant.LastName}";
        pronounsText.text = currentInhabitant.baseData.Pronouns.ToString();
        mbtiText.text = currentInhabitant.baseData.MBTI.ToString();
        personalitiesText.text = string.Join(" / ", currentInhabitant.baseData.Personnality.ToString());
        GoldMultiplierText.text = currentInhabitant.GoldMultiplier.ToString();

        moodSlider.value = currentInhabitant.Mood;
        serenitySlider.value = currentInhabitant.Serenity;
        energySlider.value = currentInhabitant.Energy;

        indexText.text = $"{currentIndex + 1}/{inhabitants.Count}";
    }

    private void ShowNext()
    {
        currentIndex = (currentIndex + 1) % inhabitants.Count;
        DisplayInhabitant();
    }

    private void ShowPrevious()
    {
        currentIndex = (currentIndex - 1 + inhabitants.Count) % inhabitants.Count;
        DisplayInhabitant();
    }

}
