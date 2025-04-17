using LitMotion;
using LitMotion.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private enum InventoryCategory
    {
        InhabitantCategory,
        BuildingCategory,
        DecorationCategory,
    }

    #region Variables

    public Dictionary<Inhabitant, InventoryItem<Inhabitant>> InhabitantInventory { get; private set; } = new();
    public Dictionary<Building, InventoryItem<Building>> BuildingInventory { get; private set; } = new();
    public Dictionary<Decoration, InventoryItem<Decoration>> DecorationInventory { get; private set; } = new();


    public string PlayerName { get; private set; }
    public string CityName { get; private set; }

    public int Level { get; private set; } = 1;
    public int CurrentXP { get; private set; } = 0;

    private int baseExpPerLevel = 300;
    private float multExp = 1.3f;
    private int expLevel;

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

    public void AddToInventory<T>(T item, int amount, Dictionary<T, InventoryItem<T>> inventory) where T : IScriptableElement
    {
        if (inventory.TryGetValue(item, out var existing))
        {
            existing.quantity += amount;
            existing.inventorySlotItem.UpdateItemContent(existing.quantity);
        }
        else
        {
            inventory[item] = new InventoryItem<T>(item, amount);
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
            if (inventorySlot != null)
            {
                InventorySlotItem slotComponent = inventorySlot.GetComponent<InventorySlotItem>();
                inventory[item].SetInventorySlotItem(slotComponent);
                slotComponent.SetItemContent(item, amount);
            }
        }
    }

    public bool RemoveFromInventory<T>(T item, int amount, Dictionary<T, InventoryItem<T>> inventory) where T : IScriptableElement
    {
        if (inventory.TryGetValue(item, out var existing))
        {
            if (existing.quantity >= amount)
            {
                existing.quantity -= amount;
                if (existing.quantity == 0)
                {
                    inventory.Remove(item);
                }
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


    public interface IPlaceable
    {
        GameObject GetPrefab();
    }

    #endregion
}