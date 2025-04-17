using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotItem : MonoBehaviour
{
    
    private IScriptableElement inventoryItem;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI quantityText;

    public void SetItemContent<T>(T _item, int _quantity) where T : IScriptableElement
    {
        inventoryItem = _item;
        icon.sprite = inventoryItem.Icon;
        itemName.text = inventoryItem.Name;
        quantityText.text = _quantity.ToString();
    }

    public void UpdateItemContent(int _quantity)
    {
        quantityText.text = _quantity.ToString();
    }
}
