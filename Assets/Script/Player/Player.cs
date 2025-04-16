using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Player
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

    public Player(string name, string city)
    {
        PlayerName = name;
        CityName = city;
        expLevel = baseExpPerLevel;
    }
    public void SetPlayerInfo(string name, string city)
    {
        PlayerName = name;
        CityName = city;
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
