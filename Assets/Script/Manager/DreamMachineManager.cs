using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Build.Content;
using Unity.VisualScripting;

public class DreamMachineManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image characterImage;
    public TextMeshProUGUI characterNameText;
    public Slider moodSlider;
    public Slider serenitySlider;
    public Slider energySlider;

    public List<Inhabitant> inhabitants;
    private int currentIndex = 0;

    private void Start()
    {
        //inhabitants = VillageManager.instance.inhabitants;

        if (inhabitants.Count > 0)
        {
            DisplayCurrentInhabitant();
        }
        else
        {
            Debug.Log("No inhabitants found in VillageManager!");
        }
    }

    private void DisplayCurrentInhabitant()
    {
        Inhabitant currentInhabitant = inhabitants[currentIndex];

        characterImage = currentInhabitant.Icon;
        characterNameText.text = $"{currentInhabitant.FirstName} {currentInhabitant.LastName}";

        moodSlider.value = currentInhabitant.Mood;
        serenitySlider.value = currentInhabitant.Serenity;
        energySlider.value = currentInhabitant.Energy;
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
}
