using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private TextMeshProUGUI itemName;

    public void SetItemContent(Sprite _icon, string _name, int _price)
    {
        itemIcon.sprite = _icon;
        itemName.text = _name;
        itemPrice.text = _price.ToString();
    }
}
