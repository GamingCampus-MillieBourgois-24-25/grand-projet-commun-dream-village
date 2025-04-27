using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class DreamMachineManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] public GameObject dreamMachineCanvas;
    
    [Header("UI Elements")]
    public Image characterImage;
    public TextMeshProUGUI characterNameText;
    public Slider moodSlider;
    public Slider serenitySlider;
    public Slider energySlider;
    public TextMeshProUGUI indexUI;
    public GameObject dreamButtonPrefab;
    public Transform dreamsContainer;
    public Button validateButton;

    [Header("Selection")] 
    [SerializeField] public Canvas selectInhabitant;
    [SerializeField] public GameObject inhabitantsContainer;
    [SerializeField] public GameObject selectInhabitantPrefab;
    [SerializeField] private TextMeshProUGUI selectionCountText;
    [SerializeField] private TextMeshProUGUI goldPreviewText;
    [SerializeField] private TextMeshProUGUI expPreviewText;
    [SerializeField] private TextMeshProUGUI timePreviewText;

    [Header("Sounds")]
    [SerializeField] private AudioClip applyDreamsSFX;
    [SerializeField] private AudioClip clickSFX;
    [SerializeField] private AudioClip noneButtonSFX;

    [Header("Visuals")]
    [SerializeField] private Color likeColor;
    [SerializeField] private Color dislikeColor;


    private List<InhabitantInstance> selectedInhabitants = new();
    private GameObject selectedButton;
    
    public InterestDatabase interestDatabase;

    private int currentIndex = 0;
    private int numberDreamSelected = 0;

    private Dictionary<InhabitantInstance, List<DisplayableDream>> dreamsByInhabitant = new();
    public Dictionary<InhabitantInstance, DisplayableDream> selectedDreamByInhabitant = new();

    private Vector2 startTouchPosition;
    private float swipeThreshold = 50f;

    [SerializeField]
    private int baseGoldPerDream = 200;
    [SerializeField]
    private int baseEXPPerDream = 150;

    private float totalDreamMinute;

    private void Start()
    {
        UpdateSelectionCanvas();

        if (selectedInhabitants.Count > 0)
        {
            var current = selectedInhabitants[currentIndex];

            if (!dreamsByInhabitant.ContainsKey(current))
            {
                dreamsByInhabitant[current] = GenerateDreamOptions(current);
            }

            DisplayCurrentInhabitant();
            DisplayDreams(dreamsByInhabitant[current]);
            
            CheckIfAllDreamsSelected();
        }
        else
        {
            Debug.Log("No inhabitants found in VillageManager!");
        }
    }

    /*private void Update()
    {
        if (dreamMachineCanvas.activeSelf)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        // Enregistrer la position de d�part du swipe
                        startTouchPosition = touch.position;
                        break;

                    case TouchPhase.Ended:
                        float swipeDistance = touch.position.x - startTouchPosition.x;

                        if (Mathf.Abs(swipeDistance) > swipeThreshold)
                        {
                            if (swipeDistance > 0)
                            {
                                // Swipe � droite -> personnage pr�c�dentt
                                PreviousInhabitant();
                            }
                            else
                            {
                                // Swipe � gauche -> personnage suivant
                                NextInhabitant();
                            }
                        }
                        break;
                }
            }
        }
    }*/

    private void DisplayCurrentInhabitant()
    {
        if (selectedInhabitants.Count == 0)
            return;

        InhabitantInstance currentInhabitant = selectedInhabitants[currentIndex];

        characterImage.sprite = currentInhabitant.Icon;
        characterNameText.text = $"{currentInhabitant.Name}";

        moodSlider.value = currentInhabitant.Mood;
        serenitySlider.value = currentInhabitant.Serenity;
        energySlider.value = currentInhabitant.Energy;

        indexUI.text = $"{numberDreamSelected}/{selectedInhabitants.Count}";

        // Reset slider colors to default (white)
        ResetSliderColors();
    }

    
    private void UpdateDreamButtonVisual(Button button, bool isActive)
    {
        ColorBlock colors = button.colors;
        if (isActive)
        {
            colors.normalColor = new Color(0.70f, 0.70f, 0.70f); // sélectionné
            colors.selectedColor = new Color(0.70f, 0.70f, 0.70f);
        }
        else
        {
            colors.normalColor = Color.white; // non sélectionné
            colors.selectedColor = Color.white;
        }
        button.colors = colors;
    }


    private void DisplayDreams(List<DisplayableDream> dreams)
    {
        foreach (Transform child in dreamsContainer)
            Destroy(child.gameObject);

        List<Button> buttons = new();
        InhabitantInstance currentInhabitant = selectedInhabitants[currentIndex];

        for (int i = 0; i < dreams.Count; i++)
        {
            var displayable = dreams[i];
            // Cr�er le bouton pour chaque r�ve
            GameObject dreamButton = Instantiate(dreamButtonPrefab, dreamsContainer);

            // R�cup�rer les images dans le bouton
            List<Image> images = new();
            foreach (Transform child in dreamButton.transform)
            {
                images.Add(child.GetComponent<Image>());
            }

            var ordered = displayable.orderedElements;

            for (int j = 0; j < ordered.Count; j++)
            {
                images[j].sprite = ordered[j].icon;

                switch (currentInhabitant.IsInterestLiked(ordered[j]))
                {
                    case 1:
                        images[j].transform.GetChild(0).GetComponent<Image>().color = likeColor;
                        break;
                    case -1:
                        images[j].transform.GetChild(0).GetComponent<Image>().color = dislikeColor;
                        images[j].transform.GetChild(0).GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
                        images[j].transform.GetChild(0).GetComponent<RectTransform>().pivot = new Vector2(0.8f, 0.2f);
                        break;
                    default:
                        images[j].transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
                        break;
                }
            }




            Button button = dreamButton.GetComponent<Button>();
            buttons.Add(button);

            // Check if this is the one previously selected for this inhabitant
            bool isSelected = selectedDreamByInhabitant.TryGetValue(currentInhabitant, out var selected) && selected == displayable;
            displayable.isSelected = isSelected;
            UpdateDreamButtonVisual(button, isSelected);

            if (isSelected)
                PreviewStats(displayable);

            int index = i;
            button.onClick.AddListener(() =>
            {
                // Deselect all
                foreach (var d in dreams)
                    d.isSelected = false;
                foreach (var b in buttons)
                    UpdateDreamButtonVisual(b, false);

                // Select clicked one
                displayable.isSelected = true;
                if (!selectedDreamByInhabitant.ContainsKey(currentInhabitant))
                {
                    numberDreamSelected++;
                }
                UpdateDreamButtonVisual(button, true);

                // Save the selection
                selectedDreamByInhabitant[currentInhabitant] = displayable;

                PreviewStats(displayable);
                
                CheckIfAllDreamsSelected();
                indexUI.text = $"{numberDreamSelected}/{selectedInhabitants.Count}";
            });

            Debug.Log($"Dream Order: {ordered[0].interestName}, {ordered[1].interestName}, {ordered[2].interestName}");
        }

        // Si aucun n’est sélectionné, reset sliders
        if (!selectedDreamByInhabitant.ContainsKey(currentInhabitant))
            DisplayCurrentInhabitant();
    }


    
    public void NextInhabitant()
    {
        if (selectedInhabitants.Count > 1)
        {
            GM.SM.PlaySFX(clickSFX);
        } 
        else
        {
            GM.SM.PlaySFX(noneButtonSFX);
        }
        
        currentIndex = (currentIndex + 1) % selectedInhabitants.Count;
        DisplayCurrentInhabitant();

        var current = selectedInhabitants[currentIndex];

        if (!dreamsByInhabitant.ContainsKey(current))
        {
            dreamsByInhabitant[current] = GenerateDreamOptions(current);
        }

        DisplayDreams(dreamsByInhabitant[current]);
    }

    public void PreviousInhabitant()
    {
        if (selectedInhabitants.Count > 1)
        {
            GM.SM.PlaySFX(clickSFX);
        } 
        else
        {
            GM.SM.PlaySFX(noneButtonSFX);
        }
        
        currentIndex = (currentIndex - 1 + selectedInhabitants.Count) % selectedInhabitants.Count;
        DisplayCurrentInhabitant();

        var current = selectedInhabitants[currentIndex];

        if (!dreamsByInhabitant.ContainsKey(current))
        {
            dreamsByInhabitant[current] = GenerateDreamOptions(current);
        }

        DisplayDreams(dreamsByInhabitant[current]);
    }


    private List<DisplayableDream> GenerateDreamOptions(InhabitantInstance inhabitant)
    {
        List<DisplayableDream> displayableDreams = new();

        List<InterestCategory> liked = new(inhabitant.Likes);
        List<InterestCategory> disliked = new(inhabitant.Dislikes);
        List<InterestCategory> neutral = new(interestDatabase.allInterests);
        neutral.RemoveAll(i => liked.Contains(i) || disliked.Contains(i));

        List<System.Func<DreamOption, List<InterestCategory>>> permutations = new()
    {
        (option) => new List<InterestCategory> { option.positiveElement, option.negativeElement, option.randomElement },
        (option) => new List<InterestCategory> { option.randomElement, option.positiveElement, option.negativeElement },
        (option) => new List<InterestCategory> { option.negativeElement, option.randomElement, option.positiveElement }
    };

        // M�lange les permutations pour cette s�rie
        for (int i = 0; i < permutations.Count; i++)
        {
            int r = Random.Range(i, permutations.Count);
            (permutations[i], permutations[r]) = (permutations[r], permutations[i]);
        }

        for (int i = 0; i < 3; i++)
        {
            InterestCategory pos = GetUniqueRandom(liked, null);
            InterestCategory neg = GetUniqueRandom(disliked, new List<InterestCategory> { pos });

            List<InterestCategory> used = new List<InterestCategory> { pos, neg };

            InterestCategory rand;
            int r = Random.Range(0, 3);
            if (r == 0 && liked.Count > 0)
                rand = GetUniqueRandom(liked, used);
            else if (r == 1 && disliked.Count > 0)
                rand = GetUniqueRandom(disliked, used);
            else
                rand = GetUniqueRandom(neutral.Count > 0 ? neutral : interestDatabase.allInterests, used);

            var option = new DreamOption(pos, neg, rand);
            var ordered = permutations[i](option);
            displayableDreams.Add(new DisplayableDream(option, ordered));
        }

        return displayableDreams;
    }
    
    private InterestCategory GetUniqueRandom(List<InterestCategory> source, List<InterestCategory> exclude)
    {
        List<InterestCategory> filtered = exclude == null ? new List<InterestCategory>(source) : new List<InterestCategory>(source.FindAll(ic => !exclude.Contains(ic)));

        if (filtered.Count == 0)
        {
            Debug.LogWarning("No unique element found, falling back to full list.");
            return source[Random.Range(0, source.Count)];
        }

        return filtered[Random.Range(0, filtered.Count)];
    }

    
    private int GetStatChange(InterestCategory element, InhabitantInstance inhabitant)
    {
        if (inhabitant.Likes.Contains(element)) return 10;
        if (inhabitant.Dislikes.Contains(element)) return -10;
        return 0;
    }
    
    private void PreviewStats(DisplayableDream displayable)
    {
        var currentInhabitant = selectedInhabitants[currentIndex];

        // Calculer les changements
        int moodChange = GetStatChange(displayable.orderedElements[0], currentInhabitant);
        int serenityChange = GetStatChange(displayable.orderedElements[1], currentInhabitant);
        int energyChange = GetStatChange(displayable.orderedElements[2], currentInhabitant);

        // Appliquer les valeurs
        moodSlider.value = currentInhabitant.Mood + moodChange;
        serenitySlider.value = currentInhabitant.Serenity + serenityChange;
        energySlider.value = currentInhabitant.Energy + energyChange;
    }
    
    private void ResetSliderColors()
    {
        moodSlider.fillRect.GetComponent<Image>().color = Color.white;
        serenitySlider.fillRect.GetComponent<Image>().color = Color.white;
        energySlider.fillRect.GetComponent<Image>().color = Color.white;
    }
    
    private void CheckIfAllDreamsSelected()
    {
        bool allSelected = true;

        foreach (var inhabitant in selectedInhabitants)
        {
            if (!selectedDreamByInhabitant.ContainsKey(inhabitant))
            {
                allSelected = false;
                break;
            }
        }

        validateButton.interactable = allSelected;
    }

    public void BS_ValidateSelectedDream()
    {
        GM.SM.PlaySFX(applyDreamsSFX);

        GM.DreamPanel.SetActive(false);
        GM.SkipDreamPanel.SetActive(true);

        GM.DN.TimeRemaining = totalDreamMinute * 60; //minutes to seconds
        GM.DN.nightDreamTimeCoroutine = GM.DN.StartCoroutine(GM.DN.StartWaitingTime());
    }

    public void ApplySelectedDreams(int notificationID)
    {
        List<InhabitantInstance> allInhabitants = new List<InhabitantInstance>(GM.VM.inhabitants);
        
        Debug.Log("Apply Selected Dreams! " + selectedDreamByInhabitant.First());
        foreach (var pair in selectedDreamByInhabitant)
        {
            var inhabitant = pair.Key;
            var dream = pair.Value;
            var ordered = dream.orderedElements;

            // 🔍 Stats avant
            Debug.Log($"[Before] {inhabitant.Name} | Mood: {inhabitant.Mood}, Serenity: {inhabitant.Serenity}, Energy: {inhabitant.Energy}");

            // ✨ Application des effets
            inhabitant.Mood += GetStatChange(ordered[0], inhabitant);
            inhabitant.Serenity += GetStatChange(ordered[1], inhabitant);
            inhabitant.Energy += GetStatChange(ordered[2], inhabitant);

            // 🎁 Découverte progressive
            foreach (var element in ordered)
            {
                if (inhabitant.Likes.Contains(element))
                    inhabitant.DiscoveredLikes.Add(element);

                if (inhabitant.Dislikes.Contains(element))
                    inhabitant.DiscoveredDislikes.Add(element);
            }

            inhabitant.Mood = Mathf.FloorToInt(inhabitant.Mood / 2f);
            inhabitant.Serenity = Mathf.FloorToInt(inhabitant.Serenity / 2f);
            inhabitant.Energy = Mathf.FloorToInt(inhabitant.Energy / 2f);

            // 🔄 Désélection
            dream.isSelected = false;

            GM.Instance.player.AddGold(Mathf.Max(0,Mathf.FloorToInt((baseGoldPerDream + inhabitant.Mood + inhabitant.Serenity + inhabitant.Energy)*inhabitant.GoldMultiplier)));
            GM.Instance.player.AddXP(Mathf.Max(0,Mathf.FloorToInt((baseEXPPerDream + inhabitant.Mood + inhabitant.Serenity + inhabitant.Energy)*inhabitant.GoldMultiplier)));

        }
        
        foreach (var inhabitant in allInhabitants)
        {
            if (!selectedInhabitants.Contains(inhabitant))
            {
                int randomStat = UnityEngine.Random.Range(0, 3);

                switch (randomStat)
                {
                    case 0:
                        inhabitant.Mood -= 15;
                        Debug.Log($"[Penalty] {inhabitant.Name} loses 20 Mood.");
                        break;
                    case 1:
                        inhabitant.Serenity -= 15;
                        Debug.Log($"[Penalty] {inhabitant.Name} loses 20 Serenity.");
                        break;
                    case 2:
                        inhabitant.Energy -= 15;
                        Debug.Log($"[Penalty] {inhabitant.Name} loses 20 Energy.");
                        break;
                }
            }
        }
        
        Debug.Log("Player just gained " + GM.Instance.player.GetGold() + " gold and " + GM.Instance.player.CurrentXP+ " exp");


        // ♻️ Reset
        selectedDreamByInhabitant.Clear();
        validateButton.interactable = false;
        numberDreamSelected = 0;
        dreamsByInhabitant.Clear();
        foreach (var inhabitant in selectedInhabitants)
        {
            dreamsByInhabitant[inhabitant] = GenerateDreamOptions(inhabitant);
        }

        // 🔁 Rafraîchissement UI
        DisplayCurrentInhabitant();
        if(selectedInhabitants.Count > 0)
            DisplayDreams(dreamsByInhabitant[selectedInhabitants[currentIndex]]);
        
        GM.Cjm.DisplayInhabitant();

        selectedInhabitants.Clear();

        if (notificationID != -1)
        {
            NotificationManager.CancelNotification(notificationID);
            notificationID = -1;
        }

        GM.SkipDreamPanel.SetActive(false);
    }
    
    public void UpdateSelectionCanvas()
    {
        foreach (Transform child in inhabitantsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var inhabitant in GM.VM.inhabitants)
        {
            GameObject go = Instantiate(selectInhabitantPrefab, inhabitantsContainer.transform);

            Image image = go.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            image.sprite = inhabitant.Icon;

            GameObject statsContainer = go.transform.GetChild(1).gameObject;

            Slider mood = statsContainer.transform.GetChild(0).GetComponent<Slider>();
            Slider serenity = statsContainer.transform.GetChild(1).GetComponent<Slider>();
            Slider energy = statsContainer.transform.GetChild(2).GetComponent<Slider>();

            mood.value = inhabitant.Mood;
            serenity.value = inhabitant.Serenity;
            energy.value = inhabitant.Energy;

            Button btn = go.GetComponent<Button>();
            if (btn != null)
            {
                InhabitantInstance capturedInhabitant = inhabitant;
                btn.onClick.AddListener(() => BS_SelectInhabitant(btn, capturedInhabitant));
            }
        }
        
        UpdateInformationsSelectionCanvas();
    }

    public void UpdateInformationsSelectionCanvas()
    {
        int selectedCount = selectedInhabitants.Count;
        int totalCount = GM.VM.inhabitants.Count;

        selectionCountText.text = $"{selectedCount} / {totalCount}";

        int totalGold = 0;
        int totalXP = 0;
        totalDreamMinute = 0f;

        foreach (var inhabitant in selectedInhabitants)
        {
            float multiplier = inhabitant.GoldMultiplier;
            int mood = inhabitant.Mood;
            int serenity = inhabitant.Serenity;
            int energy = inhabitant.Energy;

            int gold = Mathf.Max(0, Mathf.FloorToInt((baseGoldPerDream + mood + serenity + energy) * multiplier));
            int xp = Mathf.Max(0, Mathf.FloorToInt((baseEXPPerDream + mood + serenity + energy) * multiplier));

            totalGold += gold;
            totalXP += xp;
            totalDreamMinute += 0.1f; // TODO : A changer 30 min
        }

        goldPreviewText.text = $"{totalGold} gold";
        expPreviewText.text = $"{totalXP} XP";
        timePreviewText.text = $"{(int)(totalDreamMinute / 60)}h {((int)totalDreamMinute % 60)}min";
    }

    
    public void BS_OpenSelectionCanvas()
    {
        UpdateSelectionCanvas();
        DisableButton(selectedButton, true);
        selectInhabitant.gameObject.SetActive(true);
    }
    
    public void BS_SelectInhabitant(Button button, InhabitantInstance inhabitant)
    {
        if (selectedInhabitants.Contains(inhabitant))
        {
            selectedInhabitants.Remove(inhabitant);
            DisableButton(button.gameObject, true);
        }
        else
        {
            selectedInhabitants.Add(inhabitant);
            DisableButton(button.gameObject, false);
        }
        
        UpdateInformationsSelectionCanvas();
    }

    public void BS_CloseSelectionCanvas()
    {
        selectedInhabitants.Clear();
        selectInhabitant.gameObject.SetActive(false);
    }
    
    public void BS_SendSelectedInhabitant()
    {
        if (selectedInhabitants.Count > 0)
        {
            currentIndex = 0;
            var current = selectedInhabitants[currentIndex];
            
            selectInhabitant.gameObject.SetActive(false);
            
            if (!dreamsByInhabitant.ContainsKey(current))
            {
                dreamsByInhabitant[current] = GenerateDreamOptions(current);
            }
            
            DisplayCurrentInhabitant();
            DisplayDreams(dreamsByInhabitant[current]);
            CheckIfAllDreamsSelected();
            
            dreamMachineCanvas.SetActive(true);
            UpdateSelectionCanvas();

            if (GM.Tm.inDreamTutorial)
            {
                GM.Tm.UnHold(38);
            }
        }
    }
    
    private void DisableButton(GameObject button, bool disable)
    {
        if (button != null) button.transform.GetChild(3).gameObject.SetActive(disable);
    }
}

