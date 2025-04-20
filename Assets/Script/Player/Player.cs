using LitMotion;
using LitMotion.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum InventoryCategory
    {
        InhabitantCategory,
        BuildingCategory,
        DecorationCategory,
    }

    #region Variables

    private Dictionary<Inhabitant, InventoryItem> inhabitantsInventory = new();
    private Dictionary<Building, InventoryItem> buildingsInventory = new();
    private Dictionary<Decoration, InventoryItem> decorationsInventory = new();

    public string PlayerName { get; private set; }
    public string CityName { get; private set; }

    public int Level { get; private set; } = 1;
    public int CurrentXP { get; private set; } = 0;

    private int baseExpPerLevel = 100;
    private float multExp = 1.9f;
    private int expLevel;

    // Currency
    private int gold = 100;
    private int star = 100;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI starText;

    [Header("Progression")]
    public LevelProgression levelProgression;


    [Header("Player UI")]
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField cityNameInputField;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private GameObject levelUpCanvas;
    [SerializeField] private RectTransform levelUpItemContainer;
    [SerializeField] private GameObject unlockedItemPrefab;

    [Header("Inventory Menu UI")]
    [SerializeField] private InventoryCategory inventoryCategory = InventoryCategory.InhabitantCategory;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private List<Button> categoryButtons;
    [SerializeField] private List<GameObject> categoryContainers;

    #endregion


    private void Start()
    {
        expLevel = baseExpPerLevel;
        UpdateGoldText();
        UpdateStarText();
    }

    public void SetPlayerInfo()
    {
        PlayerName = playerNameInputField.text;
        CityName = cityNameInputField.text;
        if (string.IsNullOrEmpty(PlayerName))
        {
            Debug.LogError("Player name cannot be empty.");
            return;
        }
        if (string.IsNullOrEmpty(CityName))
        {
            Debug.LogError("City name cannot be empty.");
            return;
        }
        playerNameText.text = PlayerName;
        levelText.text = Level.ToString();
        Debug.Log($"Player created: {PlayerName}, City: {CityName}");
    }

    #region Currency

    // GOLD
    public int GetGold() => gold;
    public void SetGold(int value) {
        gold = Mathf.Max(0, value); // ne jamais avoir un solde négatif
        UpdateGoldText();
    } 

    public void AddGold(int amount) {
        gold += Mathf.Max(0, amount); //Ajoute des nombres positifs seulement
        UpdateGoldText();
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
            gold -= amount;
            UpdateGoldText();
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
    } 

    public void AddStar(int amount)
    {
        star += Mathf.Max(0, amount); //Ajoute des nombres positifs seulement
        UpdateStarText();
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
            star -= amount;
            UpdateStarText();
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
    }

    private void CheckLevelUp()
    {
        if (CurrentXP >= expLevel) { 
            CurrentXP -= expLevel;
            Level++;
            expLevel = Mathf.RoundToInt(expLevel * multExp);
            CheckUnlockedItem();
            levelText.text = Level.ToString();
            Debug.Log($"Level Up! New level: {Level}");
        }
    }

    private void CheckUnlockedItem()
    {
        LevelProgression.Level levelUnlockItem = levelProgression.GetLevel(Level);
        if(levelUnlockItem.unlockable.Count != 0)
        {
            Vector2 size = levelUpItemContainer.sizeDelta;
            size.x = levelUnlockItem.unlockable.Count * unlockedItemPrefab.gameObject.GetComponent<RectTransform>().rect.width +
                levelUpItemContainer.gameObject.GetComponent<HorizontalLayoutGroup>().spacing * (levelUnlockItem.unlockable.Count - 1);
            levelUpItemContainer.sizeDelta = size;

            for (int i = 0; i < levelUnlockItem.unlockable.Count; i++)
            {
                GameObject unlockedItem = Instantiate(unlockedItemPrefab, levelUpItemContainer.transform);
                unlockedItem.GetComponent<UnlockedItem>().SetItemContent(levelUnlockItem.unlockable[i].Icon, levelUnlockItem.unlockable[i].Name);
            }
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

    public Dictionary<T, InventoryItem> GetInventory<T>(T item)
    {
        if (item is Inhabitant inhabitant)
        {
            return inhabitantsInventory as Dictionary<T, InventoryItem>;
        }
        else if (item is Building building)
        {
            return buildingsInventory as Dictionary<T, InventoryItem>;
        }
        else if (item is Decoration decoration)
        {
            return decorationsInventory as Dictionary<T, InventoryItem>;
        }
        return null;
    }

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

    public void AddToInventory<T>(T item, int amount) where T : IScriptableElement
    {
        Dictionary<T, InventoryItem> inventory = GetInventory(item);
        if (inventory.TryGetValue(item, out var existing))
        {
            existing.quantity += amount;
            existing.inventorySlotItem.UpdateItemContent(existing.quantity);
        }
        else
        {
            inventory[item] = new InventoryItem(amount);
            GameObject inventorySlot = null;
            switch (item)
            {
                case Inhabitant inhabitant:
                    inventorySlot = Instantiate(itemPrefab, categoryContainers[0].transform);
                    break;
                case Building building:
                    inventorySlot = Instantiate(itemPrefab, categoryContainers[1].transform);
                    break;
                case Decoration decoration:
                    inventorySlot = Instantiate(itemPrefab, categoryContainers[2].transform);
                    break;
            }
            Debug.Log($"Added {amount} of {item} to inventory.");
            if (inventorySlot != null)
            {
                InventorySlotItem slotComponent = inventorySlot.GetComponent<InventorySlotItem>();
                inventory[item].SetInventorySlotItem(slotComponent);
                slotComponent.SetItemContent(item, amount);
            }
        }
    }

    public bool RemoveFromInventory<T>(T item, int amount) where T : IScriptableElement
    {
        if (GetItemInInventory(item, out var existing))
        {
            if (existing.quantity >= amount)
            {
                existing.quantity -= amount;
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
                }
                Debug.Log($"Removed {amount} of {item.name} from inventory.");
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

        inventoryCategory = (InventoryCategory)_category;

        categoryButtons[_category].interactable = false;
        categoryContainers[_category].SetActive(true);
        scrollView.content = categoryContainers[_category].GetComponent<RectTransform>();
    }



    #endregion
}