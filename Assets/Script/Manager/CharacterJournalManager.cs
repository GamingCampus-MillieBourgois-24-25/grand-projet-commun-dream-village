using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterJournalManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] public GameObject journalCanvas;

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
    [SerializeField] private Sprite heartGoldSprite;
    [SerializeField] private Sprite unknownIcon;

    [Header("UI - Prefabs")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject preferencePrefab;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip switchPageSFX;
    [SerializeField] private AudioClip noneButtonSFX;

    private List<InhabitantInstance> inhabitants;
    private int currentIndex = 0;
    
    private Vector2 startTouchPosition;
    private float swipeThreshold = 50f;


    private void Start()
    {
        inhabitants = GM.VM.inhabitants;

        DisplayInhabitant();
    }

    /*private void Update()
    {
        if (Input.touchCount > 0) 
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    float swipeDistance = touch.position.x - startTouchPosition.x;

                    if (Mathf.Abs(swipeDistance) > swipeThreshold)
                    {
                        if (swipeDistance > 0)
                        {
                            // Swipe � droite -> personnage pr�c�dentt
                            BS_ShowPrevious();
                        }
                        else
                        {
                            // Swipe � gauche -> personnage suivant
                            BS_ShowNext();
                        }
                    }
                    break;
            }
        }
    }*/

    public void DisplayInhabitant()
    {
        if (inhabitants.Count == 0) return;

        InhabitantInstance currentInhabitant = inhabitants[currentIndex];

        iconImage.sprite = currentInhabitant.Icon;
        nameText.text = currentInhabitant.Name;
        pronounsText.text = currentInhabitant.baseData.GetPronouns()[0] + "/" + currentInhabitant.baseData.GetPronouns()[1] ;
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
        for (int i = 1; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        foreach (var interest in interests)
        {
            //GameObject iconGO = new GameObject("InterestIcon", typeof(RectTransform), typeof(Image));
            GameObject iconGO = Instantiate(preferencePrefab);
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
        bool isLocked = !inhabitants[currentIndex].baseData.CanLeave;

        if (isLocked)
        {
            GameObject heartGold = Instantiate(heartPrefab);
            heartGold.transform.SetParent(heartsContainer, false);
            Image img = heartGold.GetComponent<Image>();
            img.sprite = heartGoldSprite;
        }
        else
        {
            for (int i = 0; i < heartMax; i++)
            {
                //GameObject heartGO = new GameObject("Heart", typeof(RectTransform), typeof(Image));
                GameObject heartGO = Instantiate(heartPrefab);
                heartGO.transform.SetParent(heartsContainer, false);
                
                Image img = heartGO.GetComponent<Image>();
                img.sprite = i < currentHearts ? heartFullSprite : heartEmptySprite;
            }
        }
    }

    public void ShowInhabitantByData(InhabitantInstance target)
    {
        int index = inhabitants.FindIndex(i => i == target);
        //Debug.Log(currentIndex);
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
                    Debug.Log($"{inhabitant.Name} quitte le village !");
                    toRemove.Add(inhabitant);
                }
                else
                {
                    Debug.Log($"{inhabitant.Name} a perdu un cœur. Reste {inhabitant.Hearts}");
                }
            }
        }

        foreach (var leaver in toRemove)
        {
            GM.VM.RemoveInstance(leaver.houseObject.gameObject);
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
                Debug.Log($"💖 {inhabitant.Name} a gagné un cœur ! ({inhabitant.Hearts}/{inhabitant.baseData.HeartsBeforeLeaving})");
            }
        }

        // Actualise l'affichage
        DisplayInhabitant();
    }


    public void BS_ShowNext()
    {
        if (inhabitants.Count > 1)
        {
            GM.SM.PlaySFX(switchPageSFX);
        } 
        else
        {
            GM.SM.PlaySFX(noneButtonSFX);
        }

        currentIndex = (currentIndex + 1) % inhabitants.Count;
        DisplayInhabitant();
    }

    public void BS_ShowPrevious()
    {
        if (inhabitants.Count > 1)
        {
            GM.SM.PlaySFX(switchPageSFX);
        }
        else
        {
            GM.SM.PlaySFX(noneButtonSFX);
        }

        currentIndex = (currentIndex - 1 + inhabitants.Count) % inhabitants.Count;
        DisplayInhabitant();
    }

    public void OpenJournal()
    {
        if (journalCanvas != null)
        {
            journalCanvas.SetActive(true);       
            currentIndex = 0;
            GM.JournalPanel.SetActive(false);
            GM.ShopPanel.SetActive(false);
            GM.InventoryPanel.SetActive(false);
            GM.DayNightPanel.SetActive(false);

            nextButton.gameObject.SetActive(true);
            previousButton.gameObject.SetActive(true);
            DisplayInhabitant();                 
        }
        else
        {
            Debug.LogWarning("Journal Canvas n'est pas assigné !");
        }
    }

    public void CloseJournal()
    {
        if (journalCanvas != null)
        {
            journalCanvas.SetActive(false);
            currentIndex = 0;
            GM.JournalPanel.SetActive(true);
            GM.ShopPanel.SetActive(true);
            GM.InventoryPanel.SetActive(true);
            GM.DayNightPanel.SetActive(true);
            DisplayInhabitant();
        }
        else
        {
            Debug.LogWarning("Journal Canvas n'est pas assigné !");
        }
    }

}
