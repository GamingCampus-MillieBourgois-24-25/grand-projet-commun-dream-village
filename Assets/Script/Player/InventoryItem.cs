using System;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public int quantity;
    public InventorySlotItem inventorySlotItem;

    // Constructeur 
    public InventoryItem(int _quantity)
    {
        this.quantity = _quantity;
    }

    public void SetInventorySlotItem(InventorySlotItem _inventorySlotItem)
    {
        this.inventorySlotItem = _inventorySlotItem;
    }
}

