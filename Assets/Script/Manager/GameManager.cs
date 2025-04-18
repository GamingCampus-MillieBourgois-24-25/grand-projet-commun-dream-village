using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, ISaveable<GameManager.SavePartData>
{
    #region Variables

    public static GameManager instance { get; private set; } //Singleton 

    [Header("Managers")]
    public VillageManager villageManager;
    public IsoManager isoManager;
    public CharacterJournalManager characterJournalManager;
    public DialoguesManager dialoguesManager;
    public TutorialsManager tutorialsManager;
    public AccessibilityOptions accessibilityOptions;

    public List<Inhabitant> inhabitants = new List<Inhabitant>();
    public List<Building> buildings = new List<Building>();

    [Header("Player")]
    public bool isPlayerCreated = false;
    public Player player;   
    [SerializeField] private GameObject playerFormCanvas;
    public GameObject mainUiCanvas;



    DateTime lastTimeSaved;


    #region save Data
    [System.Serializable]
    public class SavePartData : ISaveData
    {
        public DateTime lastTimeConnected;
    }
    #endregion

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
        // if (!isPlayerCreated)
        // {
        //     playerFormCanvas.SetActive(true);
        // }
    }

    // Load all resources for shop from the Resources folder
    private void LoadAllResources()
    {
        this.Load("GameManager");


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


        villageManager.Load("VillageManager");
    }



    public Inhabitant GetInhabitantByName(string name)
    {
        foreach (Inhabitant inhabitant in inhabitants)
        {
            if (inhabitant.Name == name)
            {
                return inhabitant;
            }
        }
        Debug.LogError("Inhabitant not found: " + name);
        return null;
    }

    public Building GetBuildingByName(string name)
    {
        foreach (Building building in buildings)
        {
            if (building.Name == name)
            {
                return building;
            }
        }
        Debug.LogError("Building not found: " + name);
        return null;
    }


    public void SetActualTime()
    {
        lastTimeSaved = DateTime.Now;
    }

    public DateTime GetLastTimeSaved()
    {
        return lastTimeSaved;
    }




    #region Check Game closed
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveGame();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            SaveGame();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
    #endregion

    #region Save Functions
    public SavePartData Serialize()
    {
        SavePartData data = new SavePartData();
        data.lastTimeConnected = lastTimeSaved;
        return data;
    }

    public void Deserialize(SavePartData data)
    {
        lastTimeSaved = data.lastTimeConnected;
    }


    public void SaveGame()
    {
        SetActualTime();
        this.Save("GameManager");
        villageManager.Save("VillageManager");
    }
    #endregion
}

public static class GM
{
    public static GameManager Instance => GameManager.instance;
    public static IsoManager IM => GameManager.instance.isoManager;
    public static VillageManager VM => GameManager.instance.villageManager;

    public static CharacterJournalManager Cjm => GameManager.instance.characterJournalManager;
    
    public static DialoguesManager Dm => GameManager.instance.dialoguesManager;
    
    public static TutorialsManager Tm => GameManager.instance.tutorialsManager;
    
    
    public static AccessibilityOptions Ao => GameManager.instance.accessibilityOptions;
}
