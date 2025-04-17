using System;
using UnityEngine;

[System.Serializable]
public class InventoryItem<T> where T : IScriptableElement
{
    public T item;
    public int quantity;
    public InventorySlotItem inventorySlotItem;

    // Constructeur 
    public InventoryItem(T _item, int _quantity)
    {
        this.item = _item;
        this.quantity = _quantity;
    }

    public void SetInventorySlotItem(InventorySlotItem _inventorySlotItem)
    {
        this.inventorySlotItem = _inventorySlotItem;
    }
}

