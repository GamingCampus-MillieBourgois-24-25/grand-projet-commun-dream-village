using LitMotion;
using LitMotion.Extensions;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Player : MonoBehaviour
{
    public Dictionary<Inhabitant, InventoryItem<Inhabitant>> InhabitantInventory { get; private set; } = new();
    public Dictionary<Deco, InventoryItem<Deco>> DecoInventory { get; private set; } = new();
    public Dictionary<Building, InventoryItem<Building>> BuildingInventory { get; private set; } = new();


    public string PlayerName { get; private set; }
    public string CityName { get; private set; }

    public int Level { get; private set; } = 1;
    public int CurrentXP { get; private set; } = 0;

    private int baseExpPerLevel = 300;
    private float multExp = 1.3f;
    private int expLevel;

    [Header("Progression")]
    public LevelProgression levelProgression;


    [Header("UI")]
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField cityNameInputField;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private GameObject levelUpCanvas;
    [SerializeField] private RectTransform levelUpItemContainer;
    [SerializeField] private GameObject unlockedItemPrefab;


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
                switch (levelUnlockItem.unlockable[i])
                {
                    case Inhabitant inhabitant:
                        unlockedItem.GetComponent<UnlockedItem>().SetItemContent(inhabitant.Icon, inhabitant.FirstName + " " + inhabitant.LastName); ;
                        break;
                    case Building building:
                        unlockedItem.GetComponent<UnlockedItem>().SetItemContent(building.Icon, building.Name);
                        break;
                    //case Decoration decoration:
                    //    SetItemContent(decoration, obj);
                    //    break;
                    default:
                        break;
                }
            }

            levelUpCanvas.SetActive(true);
            RectTransform target = levelUpCanvas.transform.GetChild(0).GetComponent<RectTransform>();
            target.localScale = Vector3.zero;

            LMotion.Create(Vector3.zero, Vector3.one, 0.5f)
             .WithEase(Ease.InCubic) 
             .BindToLocalScale(target);
        }
    }

    #endregion

    #region Inventory
    public void AddToInventory<T>(T item, int amount, Dictionary<T, InventoryItem<T>> inventory, GameObject prefab = null) where T : ScriptableObject
    {
        if (inventory.TryGetValue(item, out var existing))
        {
            existing.quantity += amount;
        }
        else
        {
            inventory[item] = new InventoryItem<T>(item, amount, prefab);
        }
    }

    public bool RemoveFromInventory<T>(T item, int amount, Dictionary<T, InventoryItem<T>> inventory) where T : ScriptableObject
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

    public interface IPlaceable
    {
        GameObject GetPrefab();
    }

    #endregion
}