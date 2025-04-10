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

    private void Start()
    {
        inhabitants = VillageManager.instance.inhabitants;

        if (inhabitants.Count > 0)
        {
            var currentDreams = GenerateDreamOptions(inhabitants[currentIndex]);
            DisplayCurrentInhabitant();
            DisplayDreams(currentDreams);
        }
        else
        {
            Debug.Log("No inhabitants found in VillageManager!");
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

    private void DisplayDreams(List<DreamOption> dreamOptions)
    {
        foreach (Transform child in dreamsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var option in dreamOptions)
        {
            // Créer le bouton pour chaque rêve
            GameObject dreamButton = Instantiate(dreamButtonPrefab, dreamsContainer);

            // Récupérer les images dans le bouton
            Image[] images = dreamButton.GetComponentsInChildren<Image>();

            // Assigner les bonnes images (Positive, Negative, Random)
            images[1].sprite = option.positiveElement.icon;  // Image pour l'élément positif
            images[2].sprite = option.negativeElement.icon;  // Image pour l'élément négatif
            images[3].sprite = option.randomElement.icon;    // Image pour l'élément aléatoire

        }
    }

    public void NextInhabitant()
    {
        currentIndex = (currentIndex + 1) % inhabitants.Count;
        DisplayCurrentInhabitant();
    }

    public void PreviousInhabitant()
    {
        currentIndex = ( currentIndex - 1 + inhabitants.Count ) % inhabitants.Count;
        DisplayCurrentInhabitant();
    }

    private List<DreamOption> GenerateDreamOptions(Inhabitant inhabitant)
    {
        List<DreamOption> dreamOptions = new List<DreamOption>();

        List<InterestCategory> liked = new List<InterestCategory>(inhabitant.Likes);
        List<InterestCategory> disliked = new List<InterestCategory>(inhabitant.Dislikes);

        List<InterestCategory> neutral = new List<InterestCategory>(interestDatabase.allInterests);
        neutral.RemoveAll(i => liked.Contains(i) || disliked.Contains(i));

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

            DreamOption option = new DreamOption(pos, neg, rand);
            dreamOptions.Add(option);

            Debug.Log($"Dream {i + 1}:");
            Debug.Log($"Positive: {option.positiveElement.interestName}");
            Debug.Log($"Negative: {option.negativeElement.interestName}");
            Debug.Log($"Random: {option.randomElement.interestName}");
        }

        return dreamOptions;
    }

}
