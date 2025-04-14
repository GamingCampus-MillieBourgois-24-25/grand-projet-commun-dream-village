using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

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

    [Header("UI - Sprites")]
    [SerializeField] private Sprite heartFullSprite;
    [SerializeField] private Sprite heartEmptySprite;

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
        personalitiesText.text = string.Join(" / ", currentInhabitant.baseData.Personnality);
        GoldMultiplierText.text = $"x{currentInhabitant.baseData.GoldMultiplier:F2}";

        int limit = currentInhabitant.baseData.Limit;
        moodSlider.minValue = -limit;
        moodSlider.maxValue = limit;
        serenitySlider.minValue = -limit;
        serenitySlider.maxValue = limit;
        energySlider.minValue = -limit;
        energySlider.maxValue = limit;

        moodSlider.value = currentInhabitant.Mood;
        serenitySlider.value = currentInhabitant.Serenity;
        energySlider.value = currentInhabitant.Energy;

        RefreshIcons(likesContainer, currentInhabitant.Likes);
        RefreshIcons(dislikesContainer, currentInhabitant.Dislikes);

        RefreshHearts(currentInhabitant.Hearts);

        indexText.text = $"{currentIndex + 1}/{inhabitants.Count}";
    }

    private void RefreshIcons(Transform container, List<InterestCategory> interests)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        foreach (var interest in interests)
        {
            GameObject newGO = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            newGO.transform.SetParent(container, false);

            Image img = newGO.GetComponent<Image>();
            img.sprite = interest.icon;
            img.preserveAspect = true;
        }
    }

    private void RefreshHearts(int currentHearts)
    {
        foreach (Transform child in heartsContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentHearts; i++)
        {
            GameObject heartGO = new GameObject("Heart", typeof(RectTransform), typeof(Image));
            heartGO.transform.SetParent(heartsContainer, false);

            Image img = heartGO.GetComponent<Image>();
            img.sprite = heartFullSprite; 
            img.preserveAspect = true;

            RectTransform rt = heartGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(100, 100); 
        }
    }

    public void ShowInhabitantByData(Inhabitant target)
    {
        int index = inhabitants.FindIndex(i => i.baseData == target);
        Debug.Log(currentIndex);
        if (index != -1)
        {
            currentIndex = index;
            DisplayInhabitant();
        }
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
