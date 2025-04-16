using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem<T> where T : IScriptableElement
{
    public T item;
    public int quantity;
    public GameObject prefab;

    // Constructeur 
    public InventoryItem(T item, int quantity, GameObject prefab = null)
    {
        this.item = item;
        this.quantity = quantity;
        this.prefab = prefab;
    }
}

