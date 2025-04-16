using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables

    public static GameManager instance { get; private set; } //Singleton 

    [Header("Managers")]
    public VillageManager villageManager;
    public IsoManager isoManager;

    public List<Inhabitant> inhabitants = new List<Inhabitant>();
    public List<Building> buildings = new List<Building>();

    [Header("Player")]
    public bool isPlayerCreated = false;
    public Player player;   
    [SerializeField] private GameObject playerFormCanvas;

    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllResources();
    }

    private void Start()
    {
        if (!isPlayerCreated)
        {
            playerFormCanvas.SetActive(true);
        }
    }

    // Load all resources for shop from the Resources folder
    private void LoadAllResources()
    {
        // Load all inhabitants
        Inhabitant[] allInhabitants = Resources.LoadAll<Inhabitant>("ScriptableObject/Inhabitants");
        foreach (Inhabitant inhabitant in allInhabitants)
        {
            inhabitants.Add(inhabitant);
        }
        // Load all buildings
        Building[] allBuildings = Resources.LoadAll<Building>("ScriptableObject/Buildings");
        foreach (Building building in allBuildings)
        {
            buildings.Add(building);
        }
    }
}

public static class GM
{
    public static GameManager Instance => GameManager.instance;
    public static IsoManager IM => GameManager.instance.isoManager;
    public static VillageManager VM => GameManager.instance.villageManager;
}
