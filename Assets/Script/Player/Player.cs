using LitMotion;
using LitMotion.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, ISaveable<Player.SavePartData>
{
    public enum ItemCategory
    {
        InhabitantCategory,
        BuildingCategory,
        DecorationCategory,
    }

    #region Variables

    private Dictionary<Inhabitant, InventoryItem> inhabitantsInventory = new();
    private Dictionary<Building, InventoryItem> buildingsInventory = new();
    private Dictionary<Decoration, InventoryItem> decorationsInventory = new();
    
    public delegate void PlayerInfoAssignDelegate();
    public PlayerInfoAssignDelegate OnPlayerInfoAssigned;

    public string PlayerName { get; private set; }
    public string CityName { get; private set; }

    public int Level { get; private set; } = 1;
    public int CurrentXP { get; private set; } = 0;

    private int baseExpPerLevel = 100;
    private float multExp = 1.9f;
    private int expLevel;

    // Currency
    private int gold = 100000;
    private int star = 100;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI starText;

    [Header("Progression")]
    public LevelProgression levelProgression;

    [Header("Player UI")]
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField cityNameInputField;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private GameObject levelUpCanvas;
    [SerializeField] private RectTransform levelUpItemContainer;
    [SerializeField] private GameObject unlockedItemPrefab;

    [Header("Inventory Menu UI")]
    [SerializeField] private ItemCategory inventoryCategory = ItemCategory.InhabitantCategory;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private List<Button> categoryButtons;
    [SerializeField] private List<GameObject> categoryContainers;

    [Header("Inventory Menu UI")]
    [SerializeField] private AudioClip levelUpSFX;
    [SerializeField] private AudioClip clickSFX;
    [SerializeField] private AudioClip noneButtonSFX;

    #endregion

    #region Save
    public class SavePartData : ISaveData
    {
        public string playerName;
        public string cityName;
        public int level;
        public int currentXP;
        public int gold;
        public int star;

        public Dictionary<string, int> inhabitantsInventory = new();
        public Dictionary<string, int> buildingsInventory = new();
        public Dictionary<string, int> decorationsInventory = new();
    }

    public SavePartData Serialize()
    {
        SavePartData data = new SavePartData();

        data.playerName = PlayerName;
        data.cityName = CityName;
        data.level = Level;
        data.currentXP = CurrentXP;
        
        data.star = star;
        data.gold = gold;

        foreach (var item in inhabitantsInventory)
            data.inhabitantsInventory[item.Key.Name] = item.Value.quantity;

        foreach (var item in buildingsInventory)
            data.buildingsInventory[item.Key.Name] = item.Value.quantity;

        foreach (var item in decorationsInventory)
            data.decorationsInventory[item.Key.Name] = item.Value.quantity;

        return data;
    }

    public void Deserialize(SavePartData data)
    {
        PlayerName = data.playerName;
        CityName = data.cityName;
        Level = data.level;
        CurrentXP = data.currentXP;

        playerNameText.text = PlayerName;
        levelText.text = Level.ToString();
        
        star = data.star;
        gold = data.gold;

        foreach (var inhabitantDic in data.inhabitantsInventory)
        {
            Inhabitant inhabitant = GM.Instance.GetInhabitantByName(inhabitantDic.Key);
            if (inhabitant != null)
            {
                AddToInventory(inhabitant, inhabitantDic.Value);
            }
        }

        foreach (var buildingDic in data.buildingsInventory)
        {
            Building building = GM.Instance.GetBuildingByName(buildingDic.Key);
            if (building != null)
            {
                AddToInventory(building, buildingDic.Value);
            }
        }

        foreach (var decorationDic in data.decorationsInventory)
        {
            Decoration decoration = GM.Instance.GetDecorationByName(decorationDic.Key);
            if (decoration != null)
            {
                AddToInventory(decoration, decorationDic.Value);
            }
        }

        if (PlayerName != null)
        {
            GM.Instance.isPlayerCreated = true;
        }
    }
    #endregion


    private void Start()
    {
        expLevel = baseExpPerLevel;
        for(int i = 1; i < Level; i++)
        {
            expLevel = Mathf.RoundToInt(expLevel * multExp);
        }

        UpdateGoldText();
        UpdateStarText();
        UpdateLevelText();
    }

    public void SetPlayerInfo()
    {
        GM.SM.PlaySFX(clickSFX);
        PlayerName = playerNameInputField.text;
        CityName = cityNameInputField.text;
        if (string.IsNullOrEmpty(PlayerName))
        {
            GM.SM.PlaySFX(noneButtonSFX);
            Debug.LogError("Player name cannot be empty.");
            return;
        }
        if (string.IsNullOrEmpty(CityName))
        {
            GM.SM.PlaySFX(noneButtonSFX);
            Debug.LogError("City name cannot be empty.");
            return;
        }
        playerNameText.text = PlayerName;
        UpdateLevelText();
        Debug.Log($"Player created: {PlayerName}, City: {CityName}");
        
        GM.Dm.UpdateLocalizedArguments("PLAYER_NAME", PlayerName);
        GM.Dm.UpdateLocalizedArguments("VILLAGE_NAME", CityName);
        
        playerUIPrefab.SetActive(false);
        
        GM.Tm.holdDialogues = false;
        GM.Tm.skipDialogue = true; 
        
        GM.Instance.isPlayerCreated = true;
        
        OnPlayerInfoAssigned?.Invoke();

        this.Save("PlayerData");
        
    }

    #region Currency

    // GOLD
    public int GetGold() => gold;
    public void SetGold(int value) {
        gold = Mathf.Max(0, value); // ne jamais avoir un solde négatif
        UpdateGoldText();

        this.Save("PlayerData");
    } 

    public void AddGold(int amount) {
        SetGold(gold + amount);
    } 
    public bool CanSpendGold(int amount)
    {
        if (gold >= amount)
        {
            return true;
        }
        return false;
    }
    public void SpendGold(int amount)
    {
        if (CanSpendGold(amount))
        {
            SetGold(gold - amount);
        }
    }
    private void UpdateGoldText()
    {
        goldText.text = GetGold().ToString();
    }

    // STAR
    public int GetStar() => star;
    public void SetStar(int value) {
        star = Mathf.Max(0, value);
        UpdateStarText();

        this.Save("PlayerData");
    } 

    public void AddStar(int amount)
    {
        SetStar(star + amount);
    }
    public bool CanSpendStar(int amount)
    {
        if (star >= amount)
        {
            return true;
        }
        return false;
    }
    public void SpendStar(int amount)
    {
        if (CanSpendStar(amount))
        {
            SetStar(star - amount);
        }
    }

    private void UpdateStarText()
    {
        starText.text = GetStar().ToString();
    }

    #endregion

    #region EXP
    public void AddXP(int amount)
    {
        CurrentXP += amount;
        CheckLevelUp();

        this.Save("PlayerData");
    }

    private void CheckLevelUp()
    {
        if (CurrentXP >= expLevel) {
            GM.SM.PlaySFX(levelUpSFX);
            CurrentXP -= expLevel;
            Level++;
            expLevel = Mathf.RoundToInt(expLevel * multExp);
            CheckUnlockedItem();
            levelText.text = Level.ToString();
            Debug.Log($"Level Up! New level: {Level}");
        }
    }

    private void UpdateLevelText()
    {
        levelText.text = Level.ToString();
    }

    private void CheckUnlockedItem()
    {
        LevelProgression.Level levelUnlockItem = levelProgression.GetLevel(Level);
        if(levelUnlockItem != null && levelUnlockItem.unlockable.Count != 0)
        {
            levelUpItemContainer.gameObject.SetActive(true);
            Vector2 size = levelUpItemContainer.sizeDelta;
            size.x = levelUnlockItem.unlockable.Count * unlockedItemPrefab.gameObject.GetComponent<RectTransform>().rect.width +
                levelUpItemContainer.gameObject.GetComponent<HorizontalLayoutGroup>().spacing * (levelUnlockItem.unlockable.Count - 1);
            levelUpItemContainer.sizeDelta = size;

            for (int i = 0; i < levelUnlockItem.unlockable.Count; i++)
            {
                GameObject unlockedItem = Instantiate(unlockedItemPrefab, levelUpItemContainer.transform);
                unlockedItem.GetComponent<UnlockedItem>().SetItemContent(levelUnlockItem.unlockable[i].Icon, levelUnlockItem.unlockable[i].Name);
            }
        } else
        {
            levelUpItemContainer.gameObject.SetActive(false);
        }
        levelUpCanvas.SetActive(true);
        RectTransform target = levelUpCanvas.transform.GetChild(0).GetComponent<RectTransform>();
        target.localScale = Vector3.zero;

        LMotion.Create(Vector3.zero, Vector3.one, 0.5f)
         .WithEase(Ease.InCubic)
         .BindToLocalScale(target);

    }

    public void DisableLvlUpCanvas()
    {
        levelUpCanvas.SetActive(false);
        foreach (Transform child in levelUpItemContainer)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion

    #region Inventory

    public bool GetItemInInventory<T>(T item, out InventoryItem inventoryItem)
    {
        switch (item)
        {
            case Inhabitant inhabitant:
                if (inhabitantsInventory.TryGetValue(inhabitant, out var existing))
                {
                    inventoryItem = existing;
                    return true;
                }
                break;
            case Building building:
                if(buildingsInventory.TryGetValue(building, out var existing2))
                {
                    inventoryItem = existing2;
                    return true;
                }
                break;
            case Decoration decoration:
                if (decorationsInventory.TryGetValue(decoration, out var existing3))
                {
                    inventoryItem = existing3;
                    return true;
                }
                break;
        }
        inventoryItem = null;
        return false;
    }

    public int GetItemQuantity<T>(T item) where T : IScriptableElement
    {
        if (GetItemInInventory(item, out var inventoryItem))
        {
            return inventoryItem.quantity;
        }
        return 0;
    }

    public void AddToInventory<T>(T item, int amount) where T : IScriptableElement
    {
        if (GetItemInInventory(item, out var existing))
        {
            existing.quantity += amount;
            existing.inventorySlotItem.UpdateItemContent(existing.quantity);
        }
        else
        {
            GameObject inventorySlot = null;
            switch (item)
            {
                case Inhabitant inhabitant:
                    inhabitantsInventory[inhabitant] = new InventoryItem(amount);
                    inventorySlot = Instantiate(itemPrefab, categoryContainers[0].transform);
                    CreateInventorySlot(inventorySlot, inhabitantsInventory[inhabitant], inhabitant, amount);
                    break;
                case Building building:
                    buildingsInventory[building] = new InventoryItem(amount);
                    inventorySlot = Instantiate(itemPrefab, categoryContainers[1].transform);
                    CreateInventorySlot(inventorySlot, buildingsInventory[building], building, amount);
                    break;
                case Decoration decoration:
                    decorationsInventory[decoration] = new InventoryItem(amount);
                    inventorySlot = Instantiate(itemPrefab, categoryContainers[2].transform);
                    CreateInventorySlot(inventorySlot, decorationsInventory[decoration], decoration, amount);
                    break;
            }
            Debug.Log($"Added {amount} of {item} to inventory.");
            this.Save("PlayerData");
            
        }
    }

    public void CreateInventorySlot<T>(GameObject inventorySlot, InventoryItem inventory_item, T item, int amount) where T : IScriptableElement
    {
        if (inventorySlot != null)
        {
            InventorySlotItem slotComponent = inventorySlot.GetComponent<InventorySlotItem>();
            inventory_item.SetInventorySlotItem(slotComponent);
            slotComponent.SetItemContent(item, amount);
        }
    }

    public bool RemoveFromInventory<T>(T item, int amount) where T : IScriptableElement
    {
        if (GetItemInInventory(item, out var existing))
        {
            if (existing.quantity >= amount)
            {
                existing.quantity -= amount;
                existing.inventorySlotItem.UpdateItemContent(existing.quantity);
                if (existing.quantity == 0)
                {
                    switch (item)
                    {
                        case Inhabitant inhabitant:
                            inhabitantsInventory.Remove(inhabitant);
                            break;
                        case Building building:
                            buildingsInventory.Remove(building);
                            break;
                        case Decoration decoration:
                            decorationsInventory.Remove(decoration);
                            break;
                    }
                    Destroy(existing.inventorySlotItem.gameObject);
                }
                Debug.Log($"Removed {amount} of {item.name} from inventory.");
                this.Save("PlayerData");
                return true;
            }
        }
        return false;
    }

    public void BS_SwitchInventoryCategory(int _category)
    {
        if (_category < 0 || _category > 2)
        {
            Debug.LogError("Invalid shop category index");
            return;
        }

        categoryButtons[(int)inventoryCategory].interactable = true;
        categoryContainers[(int)inventoryCategory].SetActive(false);

        inventoryCategory = (ItemCategory)_category;

        categoryButtons[_category].interactable = false;
        categoryContainers[_category].SetActive(true);
        scrollView.content = categoryContainers[_category].GetComponent<RectTransform>();
    }



    #endregion
}