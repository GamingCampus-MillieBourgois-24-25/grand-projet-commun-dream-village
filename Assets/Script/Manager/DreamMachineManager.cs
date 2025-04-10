using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DreamMachineManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image characterImage;
    public TextMeshProUGUI characterNameText;
    public Slider moodSlider;
    public Slider serenitySlider;
    public Slider energySlider;
    public TextMeshProUGUI index;
    public GameObject dreamButtonPrefab;
    public Transform dreamsContainer;
    public Button validateButton;

    public InterestDatabase interestDatabase;

    private List<Inhabitant> inhabitants;
    private int currentIndex = 0;
    
    private Dictionary<Inhabitant, List<DisplayableDream>> dreamsByInhabitant = new Dictionary<Inhabitant, List<DisplayableDream>>();
    private Dictionary<Inhabitant, DisplayableDream> selectedDreamByInhabitant = new();
    
    private Vector2 startTouchPosition;
    private float swipeThreshold = 50f;

    private void Start()
    {
        inhabitants = VillageManager.instance.inhabitants;

        if (inhabitants.Count > 0)
        {
            var current = inhabitants[currentIndex];

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

    private void Update()
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
                            PreviousInhabitant();
                        }
                        else
                        {
                            NextInhabitant();
                        }
                    }
                    break;
            }
        }
    }

    private void DisplayCurrentInhabitant()
    {
        Inhabitant currentInhabitant = inhabitants[currentIndex];

        characterImage.sprite = currentInhabitant.Icon;
        characterNameText.text = $"{currentInhabitant.FirstName} {currentInhabitant.LastName}";

        moodSlider.value = currentInhabitant.Mood;
        serenitySlider.value = currentInhabitant.Serenity;
        energySlider.value = currentInhabitant.Energy;

        index.text = $"{currentIndex + 1}/{inhabitants.Count}";

        // 🔁 Reset slider colors to default (white)
        ResetSliderColors();
    }

    
    private void UpdateDreamButtonVisual(Button button, bool isActive)
    {
        ColorBlock colors = button.colors;
        if (isActive)
        {
            colors.normalColor = Color.green; // sélectionné
            colors.selectedColor = Color.green;
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
        Inhabitant currentInhabitant = inhabitants[currentIndex];

        for (int i = 0; i < dreams.Count; i++)
        {
            var displayable = dreams[i];
            GameObject dreamButton = Instantiate(dreamButtonPrefab, dreamsContainer);
            Image[] images = dreamButton.GetComponentsInChildren<Image>();

            var ordered = displayable.orderedElements;
            images[1].sprite = ordered[0].icon;
            images[2].sprite = ordered[1].icon;
            images[3].sprite = ordered[2].icon;

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
                UpdateDreamButtonVisual(button, true);

                // Save the selection
                selectedDreamByInhabitant[currentInhabitant] = displayable;

                PreviewStats(displayable);
                
                CheckIfAllDreamsSelected();
            });

            Debug.Log($"Dream Order: {ordered[0].interestName}, {ordered[1].interestName}, {ordered[2].interestName}");
        }

        // Si aucun n’est sélectionné, reset sliders
        if (!selectedDreamByInhabitant.ContainsKey(currentInhabitant))
            DisplayCurrentInhabitant();
    }


    
    public void NextInhabitant()
    {
        currentIndex = (currentIndex + 1) % inhabitants.Count;
        DisplayCurrentInhabitant();

        var current = inhabitants[currentIndex];

        if (!dreamsByInhabitant.ContainsKey(current))
        {
            dreamsByInhabitant[current] = GenerateDreamOptions(current);
        }

        DisplayDreams(dreamsByInhabitant[current]);

        /* Si un rêve est sélectionné, applique la prévisualisation
        if (selectedDream != null)
        {
            PreviewStats(selectedDream);
        }*/
    }

    public void PreviousInhabitant()
    {
        currentIndex = (currentIndex - 1 + inhabitants.Count) % inhabitants.Count;
        DisplayCurrentInhabitant();

        var current = inhabitants[currentIndex];

        if (!dreamsByInhabitant.ContainsKey(current))
        {
            dreamsByInhabitant[current] = GenerateDreamOptions(current);
        }

        DisplayDreams(dreamsByInhabitant[current]);

        /* Si un rêve est sélectionné, applique la prévisualisation
        if (selectedDream != null)
        {
            PreviewStats(selectedDream);
        }*/
    }


    private List<DisplayableDream> GenerateDreamOptions(Inhabitant inhabitant)
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

    
    private int GetStatChange(InterestCategory element, Inhabitant inhabitant)
    {
        if (inhabitant.Likes.Contains(element)) return 10;
        if (inhabitant.Dislikes.Contains(element)) return -10;
        return 0;
    }
    
    private void PreviewStats(DisplayableDream displayable)
    {
        var currentInhabitant = inhabitants[currentIndex];

        // Calculer les changements
        int moodChange = GetStatChange(displayable.orderedElements[0], currentInhabitant);
        int serenityChange = GetStatChange(displayable.orderedElements[1], currentInhabitant);
        int energyChange = GetStatChange(displayable.orderedElements[2], currentInhabitant);

        // Appliquer les valeurs
        moodSlider.value = currentInhabitant.Mood + moodChange;
        serenitySlider.value = currentInhabitant.Serenity + serenityChange;
        energySlider.value = currentInhabitant.Energy + energyChange;

        // Appliquer la couleur selon les changements
        UpdateSliderColor(moodSlider, moodChange);
        UpdateSliderColor(serenitySlider, serenityChange);
        UpdateSliderColor(energySlider, energyChange);
    }

    private void UpdateSliderColor(Slider slider, int change)
    {
        if (change > 0)
        {
            slider.fillRect.GetComponent<Image>().color = Color.green;  // Couleur verte pour positif
        }
        else if (change < 0)
        {
            slider.fillRect.GetComponent<Image>().color = Color.red;  // Couleur rouge pour négatif
        }
        else
        {
            slider.fillRect.GetComponent<Image>().color = Color.white;  // Couleur neutre pour aucun changement
        }
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

        foreach (var inhabitant in inhabitants)
        {
            if (!selectedDreamByInhabitant.ContainsKey(inhabitant))
            {
                allSelected = false;
                break;
            }
        }

        validateButton.interactable = allSelected;
    }

    
    public void ApplySelectedDreams()
    {
        foreach (var pair in selectedDreamByInhabitant)
        {
            Inhabitant inhabitant = pair.Key;
            DisplayableDream dream = pair.Value;

            var ordered = dream.orderedElements;

            inhabitant.Mood += GetStatChange(ordered[0], inhabitant);
            inhabitant.Serenity += GetStatChange(ordered[1], inhabitant);
            inhabitant.Energy += GetStatChange(ordered[2], inhabitant);

            // 🔄 Désélectionner le rêve
            dream.isSelected = false;
        }

        // ♻️ Vider le dictionnaire de sélection
        selectedDreamByInhabitant.Clear();

        // 🧼 Désactiver le bouton Validate
        validateButton.interactable = false;

        // 🔁 Rafraîchir l'affichage du personnage courant
        DisplayCurrentInhabitant();

        // 🔁 Recharger les rêves pour ce personnage (en réinitialisant visuellement)
        DisplayDreams(dreamsByInhabitant[inhabitants[currentIndex]]);
    }

}

