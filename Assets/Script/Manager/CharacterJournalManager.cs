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
    [SerializeField] private Sprite unknownIcon;

    private List<InhabitantInstance> inhabitants;
    private int currentIndex = 0;

    private void Start()
    {
        inhabitants = GM.VM.inhabitants;

        nextButton.onClick.AddListener(ShowNext);
        previousButton.onClick.AddListener(ShowPrevious);

        DisplayInhabitant();
    }

    public void DisplayInhabitant()
    {
        if (inhabitants.Count == 0) return;

        InhabitantInstance currentInhabitant = inhabitants[currentIndex];

        iconImage.sprite = currentInhabitant.Icon;
        nameText.text = currentInhabitant.Name;
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

        DisplayInterestIcons(currentInhabitant.baseData.Likes, likesContainer, currentInhabitant.DiscoveredLikes);
        DisplayInterestIcons(currentInhabitant.baseData.Dislikes, dislikesContainer, currentInhabitant.DiscoveredDislikes);

        RefreshHearts(currentInhabitant.Hearts);

        indexText.text = $"{currentIndex + 1}/{inhabitants.Count}";
    }

    private void DisplayInterestIcons(List<InterestCategory> interests, Transform container, HashSet<InterestCategory> discovered)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        foreach (var interest in interests)
        {
            GameObject iconGO = new GameObject("InterestIcon", typeof(RectTransform), typeof(Image));
            iconGO.transform.SetParent(container, false);

            Image img = iconGO.GetComponent<Image>();
            if (discovered.Contains(interest))
            {
                img.sprite = interest.icon; // Dévoilé
            }
            else
            {
                img.sprite = unknownIcon; // Sprite de "?"
            }
            img.preserveAspect = true;

            RectTransform rt = iconGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(48, 48);
        }
    }


    private void RefreshHearts(int currentHearts)
    {
        foreach (Transform child in heartsContainer)
            Destroy(child.gameObject);

        int heartMax = inhabitants[currentIndex].baseData.HeartsBeforeLeaving;

        for (int i = 0; i < heartMax; i++)
        {
            GameObject heartGO = new GameObject("Heart", typeof(RectTransform), typeof(Image));
            heartGO.transform.SetParent(heartsContainer, false);

            Image img = heartGO.GetComponent<Image>();
            img.sprite = i < currentHearts ? heartFullSprite : heartEmptySprite;
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

    public void CheckStatsAndHandleDeparture()
    {
        List<InhabitantInstance> toRemove = new();

        foreach (var inhabitant in inhabitants)
        {
            bool allNegative = inhabitant.Mood < 0 && inhabitant.Serenity < 0 && inhabitant.Energy < 0;

            if (allNegative && inhabitant.baseData.CanLeave)
            {
                inhabitant.Hearts--;
                Debug.Log(inhabitant.Hearts); // Correction effectuée ici

                if (inhabitant.Hearts <= 0)
                {
                    Debug.Log($"{inhabitant.FirstName} quitte le village !");
                    toRemove.Add(inhabitant);
                }
                else
                {
                    Debug.Log($"{inhabitant.FirstName} a perdu un cœur. Reste {inhabitant.Hearts}");
                }
            }
        }

        foreach (var leaver in toRemove)
        {
            GM.VM.RemoveInhabitant(leaver);
        }

        // Remise à jour du journal
        if (inhabitants.Count > 0)
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, inhabitants.Count - 1);
            DisplayInhabitant();
        }
    }

    public void CheckForHeartBonus()
    {
        foreach (var inhabitant in GM.VM.inhabitants)
        {
            if (!inhabitant.baseData.CanLeave) continue; // Juste au cas où on veut restreindre ça aussi

            int maxLimit = inhabitant.baseData.Limit;

            int maxStats = 0;
            if (inhabitant.Mood >= maxLimit) maxStats++;
            if (inhabitant.Serenity >= maxLimit) maxStats++;
            if (inhabitant.Energy >= maxLimit) maxStats++;

            bool onePositive = inhabitant.Mood > 0 || inhabitant.Serenity > 0 || inhabitant.Energy > 0;

            if (maxStats >= 2 && onePositive && inhabitant.Hearts < inhabitant.baseData.HeartsBeforeLeaving)
            {
                inhabitant.Hearts += 1;
                Debug.Log($"💖 {inhabitant.FirstName} {inhabitant.LastName} a gagné un cœur ! ({inhabitant.Hearts}/{inhabitant.baseData.HeartsBeforeLeaving})");
            }
        }

        // Actualise l'affichage
        DisplayInhabitant();
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
