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

    public InterestDatabase interestDatabase;

    private List<Inhabitant> inhabitants;
    private int currentIndex = 0;
    private Dictionary<Inhabitant, List<DisplayableDream>> dreamsByInhabitant = new Dictionary<Inhabitant, List<DisplayableDream>>();
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
        }
        else
        {
            Debug.Log("No inhabitants found in VillageManager!");
        }
    }

    private void Update()
    {
        // Gestion du swipe pour changer de personnage
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // On prend le premier touch

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Enregistrer la position de d�part du swipe
                    startTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    // Calculer la distance du swipe
                    float swipeDistance = touch.position.x - startTouchPosition.x;

                    if (Mathf.Abs(swipeDistance) > swipeThreshold)
                    {
                        // Si le swipe est assez long, changer de personnage
                        if (swipeDistance > 0)
                        {
                            // Swipe � droite -> personnage pr�c�dent
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

    private void DisplayCurrentInhabitant()
    {
        Inhabitant currentInhabitant = inhabitants[currentIndex];

        characterImage.sprite = currentInhabitant.Icon;
        characterNameText.text = $"{currentInhabitant.FirstName} {currentInhabitant.LastName}";

        moodSlider.value = currentInhabitant.Mood;
        serenitySlider.value = currentInhabitant.Serenity;
        energySlider.value = currentInhabitant.Energy;

        index.text = $"{currentIndex + 1}/{inhabitants.Count}";
    }

    private void DisplayDreams(List<DisplayableDream> dreams)
    {
        foreach (Transform child in dreamsContainer)
            Destroy(child.gameObject);

        foreach (var displayable in dreams)
        {
            // Cr�er le bouton pour chaque r�ve
            GameObject dreamButton = Instantiate(dreamButtonPrefab, dreamsContainer);

            // R�cup�rer les images dans le bouton
            Image[] images = dreamButton.GetComponentsInChildren<Image>();

            // Assigner les bonnes images (Positive, Negative, Random)
            images[1].sprite = option.positiveElement.icon;  // Image pour l'�l�ment positif
            images[2].sprite = option.negativeElement.icon;  // Image pour l'�l�ment n�gatif
            images[3].sprite = option.randomElement.icon;    // Image pour l'�l�ment al�atoire

            Debug.Log($"Dream Order: {ordered[0].interestName}, {ordered[1].interestName}, {ordered[2].interestName}");
        }
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
            InterestCategory pos = liked[Random.Range(0, liked.Count)];
            InterestCategory neg = disliked[Random.Range(0, disliked.Count)];

            InterestCategory rand;
            int r = Random.Range(0, 3);
            if (r == 0 && liked.Count > 0)
                rand = liked[Random.Range(0, liked.Count)];
            else if (r == 1 && disliked.Count > 0)
                rand = disliked[Random.Range(0, disliked.Count)];
            else
                rand = neutral.Count > 0 ? neutral[Random.Range(0, neutral.Count)] : interestDatabase.allInterests[Random.Range(0, interestDatabase.allInterests.Count)];

            var option = new DreamOption(pos, neg, rand);
            var ordered = permutations[i](option);
            displayableDreams.Add(new DisplayableDream(option, ordered));
        }

        return displayableDreams;
    }


}
