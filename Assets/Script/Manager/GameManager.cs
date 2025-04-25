using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour, ISaveable<GameManager.SavePartData>
{
    #region Variables

    public static GameManager instance { get; private set; } //Singleton 

    public Transform playerIslandObject;

    [Header("Managers")]
    public VillageManager villageManager;
    public IsoManager isoManager;
    public CharacterJournalManager characterJournalManager;
    public DialoguesManager dialoguesManager;
    public TutorialsManager tutorialsManager;
    public AccessibilityOptions accessibilityOptions;
    public BuildingManager buildingManager;
    public DayNight dayNight;
    public DreamMachineManager dreamMachineManager;
    public SoundManager soundManager;

    public List<Inhabitant> inhabitants = new List<Inhabitant>();
    public List<Building> buildings = new List<Building>();
    public List<Decoration> decorations = new List<Decoration>();

    [Header("Player")]
    public bool isPlayerCreated = false;
    public Player player;   
    [SerializeField] private GameObject playerFormCanvas;
    public GameObject mainUiCanvas;

    
    public delegate void HouseTutoDelegate();
    public HouseTutoDelegate OnHouseTuto;
    [Header("UI Buttons")]
    public GameObject dreamPanel;
    public GameObject dayNightPanel;
    public GameObject journalPanel;
    public GameObject inventoryPanel;
    public GameObject shopPanel;

    DateTime lastTimeSaved;
    Dictionary<string, DisplayableDream> selectedDreamByInhabitantTemp;

    #region save Data
    [System.Serializable]
    public class SavePartData : ISaveData
    {
        public DateTime lastTimeConnected;

        public bool isDay;
        public float timeRemainingNight;

        public Dictionary<string, DisplayableDream.SavePartData> selectedDreamByInhabitant;
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
        inhabitants.Sort((x, y) => x.UnlockedAtLvl.CompareTo(y.UnlockedAtLvl)); // Sort inhabitants by name
        // Load all buildings
        Building[] allBuildings = Resources.LoadAll<Building>("ScriptableObject/Buildings");
        foreach (Building building in allBuildings)
        {
            buildings.Add(building);
        }
        buildings.Sort((x, y) => x.UnlockedAtLvl.CompareTo(y.UnlockedAtLvl)); // Sort buildings by name
        // Load all decorations
        Decoration[] allDecorations = Resources.LoadAll<Decoration>("ScriptableObject/Decorations");
        foreach (Decoration decoration in allDecorations)
        {
            decorations.Add(decoration);
        }
        decorations.Sort((x, y) => x.UnlockedAtLvl.CompareTo(y.UnlockedAtLvl)); // Sort decorations by name


        villageManager.Load("VillageManager");
        player.Load("PlayerData");

        // Load all dream
        dreamMachineManager.selectedDreamByInhabitant = new Dictionary<InhabitantInstance, DisplayableDream>();
        if(selectedDreamByInhabitantTemp == null)
            selectedDreamByInhabitantTemp = new Dictionary<string, DisplayableDream>();
        foreach (var kvp in selectedDreamByInhabitantTemp)
        {
            InhabitantInstance inhabitant = villageManager.GetInhabitant(GetInhabitantByName(kvp.Key));
            if (inhabitant != null)
            {
                DisplayableDream displayableDream = kvp.Value;
                dreamMachineManager.selectedDreamByInhabitant.Add(inhabitant, displayableDream);
            }
        }

        NotificationManager.SetupNotifications();
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
    public Decoration GetDecorationByName(string name)
    {
        foreach (Decoration decoration in decorations)
        {
            if (decoration.Name == name)
            {
                return decoration;
            }
        }
        Debug.LogError("Decoration not found: " + name);
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

    private List<int> FormatTimeFromSeconds(float totalSeconds)
    {
        int seconds = Mathf.FloorToInt(totalSeconds);

        int days = seconds / 86400;
        seconds %= 86400;

        int hours = seconds / 3600;
        seconds %= 3600;

        int minutes = seconds / 60;
        seconds %= 60;

        return new List<int> { days, hours, minutes, seconds };
    }

    public string DisplayFormattedTime(float totalSeconds)
    {
        List<int> timeList = FormatTimeFromSeconds(totalSeconds);

        int days = timeList[0];
        int hours = timeList[1];
        int minutes = timeList[2];
        int seconds = timeList[3];

        if (days > 0)
        {
            return $"{days}d {hours}h";
        }
        else if (hours > 0)
        {
            return $"{hours}h {minutes}m";
        }
        else if (minutes > 0)
        {
            return $"{minutes}m {seconds}s";
        }
        else
        {
            return $"{seconds}s";
        }
    }

    public void TrySkipActivityWithStars(TextMeshProUGUI starText, BuildingObject buildingObject)
    {
        int timeStars = int.Parse(starText.text);
        if (player.CanSpendStar(timeStars)) {
            player.SpendStar(timeStars);
            buildingObject.FinishActivity();
        }
    }

    public bool IsPointerOverUIElement(Vector2 screenPosition)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        //Debug.Log("results count ui: " + results.Count);

        return results.Count > 0; // If there's any UI element under the pointer, return true
    }

    public Vector2 GetPointerPosition(InputAction.CallbackContext context)
    {
        if (Pointer.current != null)
            return Pointer.current.position.ReadValue();
        return Vector2.zero;
    }

    #region Check Game closed
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveGame();
            NotificationManager.CreateInactivityNotification();
            NotificationManager.LaunchNotifications();
        }
        else 
            NotificationManager.CancelAllNotifications();
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            SaveGame();
            NotificationManager.CreateInactivityNotification();
            NotificationManager.LaunchNotifications();
        }
        else
            NotificationManager.CancelAllNotifications();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        NotificationManager.CreateInactivityNotification();
        NotificationManager.LaunchNotifications();
    }
    #endregion

    #region Save Functions
    public SavePartData Serialize()
    {
        SavePartData data = new SavePartData();
        data.lastTimeConnected = lastTimeSaved;

        data.isDay = dayNight.isDay;
        data.timeRemainingNight = dayNight.TimeRemaining;


        data.selectedDreamByInhabitant = new ();
        foreach (var kpd in dreamMachineManager.selectedDreamByInhabitant)
        {
            data.selectedDreamByInhabitant.Add(kpd.Key.Name, kpd.Value.Serialize());
        }


        return data;
    }

    public void Deserialize(SavePartData data)
    {
        lastTimeSaved = data.lastTimeConnected;

        //dayNight.isDay = data.isDay;


        dayNight.TimeRemaining = data.timeRemainingNight;
        if (dayNight.TimeRemaining > 0f)
        {
            dayNight.isDay = false;
        }
        else
        {
            dayNight.isDay = true;
        }


        selectedDreamByInhabitantTemp = new Dictionary<string, DisplayableDream>();
        foreach (var kvp in data.selectedDreamByInhabitant)
        {
            DisplayableDream displayableDream = new DisplayableDream();
            displayableDream.Deserialize(kvp.Value);
            selectedDreamByInhabitantTemp.Add(kvp.Key, displayableDream);
        }
    }


    public void SaveGame()
    {
        SetActualTime();
        this.Save("GameManager");
        villageManager.Save("VillageManager");
        player.Save("PlayerData");
    }
    #endregion
}

public static class GM
{
    public static GameManager Instance => GameManager.instance;
    public static IsoManager IM => GameManager.instance.isoManager;
    public static VillageManager VM => GameManager.instance.villageManager;
    public static DayNight DN => GameManager.instance.dayNight;
    public static CharacterJournalManager Cjm => GameManager.instance.characterJournalManager;
    public static DialoguesManager Dm => GameManager.instance.dialoguesManager;
    public static TutorialsManager Tm => GameManager.instance.tutorialsManager;
    public static AccessibilityOptions Ao => GameManager.instance.accessibilityOptions;
    public static DreamMachineManager DMM => GameManager.instance.dreamMachineManager;
    public static BuildingManager BM => GameManager.instance.buildingManager;
    public static SoundManager SM => GameManager.instance.soundManager;

    public static GameObject DreamPanel => Instance.dreamPanel;
    public static GameObject DayNightPanel => Instance.dayNightPanel;
    public static GameObject JournalPanel => Instance.journalPanel;
    public static GameObject InventoryPanel => Instance.inventoryPanel;
    public static GameObject ShopPanel => Instance.shopPanel;

}
