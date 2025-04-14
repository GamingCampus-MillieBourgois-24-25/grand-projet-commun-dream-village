using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Header("UI")]
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField cityNameInputField;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;


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
            Debug.Log($"Level Up! New level: {Level}");
        }
    }
    #endregion

    #region Inventory
    public void AddToInventory<T>(T item, int amount, Dictionary<T, InventoryItem<T>> inventory) where T : ScriptableObject
    {
        if (inventory.TryGetValue(item, out var existing))
        {
            existing.quantity += amount;
        }
        else
        {
            inventory[item] = new InventoryItem<T>(item, amount);
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
    #endregion
}