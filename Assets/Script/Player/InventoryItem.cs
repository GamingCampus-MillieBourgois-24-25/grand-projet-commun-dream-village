using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem<T> where T : ScriptableObject
{
    public T item;
    public int quantity;

    // Constructeur 
    public InventoryItem(T item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

