using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Player;
using static UnityEditor.Progress;

public class ShopItem : MonoBehaviour
{
    private Player.ItemCategory itemCategory;
    private int ownedQuantity = 0;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemOwnedQuantityText;

    public void SetItemContent(Player.ItemCategory _category, Sprite _icon, string _name, int _price)
    {
        itemCategory = _category;
        itemIcon.sprite = _icon;
        itemName.text = _name;
        itemPrice.text = _price.ToString();
        ownedQuantity = GM.Instance.player.GetItemQuantity(GetItem<IScriptableElement>());
        itemOwnedQuantityText.text = ownedQuantity.ToString() + " OWNED";
    }

    public T GetItem<T>() where T : IScriptableElement
    {
        switch (itemCategory)
        {
            case Player.ItemCategory.InhabitantCategory:
                return GM.Instance.GetInhabitantByName(itemName.text.ToString()) as T;
            case Player.ItemCategory.BuildingCategory:
                return GM.Instance.GetBuildingByName(itemName.text.ToString()) as T;
            case Player.ItemCategory.DecorationCategory:
                return GM.Instance.GetDecorationByName(itemName.text.ToString()) as T;
            default:
                Debug.LogError("Invalid category");
                return default;
        }
    }

    public void BS_Buy()
    {
        BuyItem<IScriptableElement>();
    }

    private void BuyItem<T>() where T : IScriptableElement
    {
        T item = GetItem<T>();
        if (GM.Instance.player.SpendGold(item.InitialPrice))
        {
            GM.Instance.player.AddToInventory(item, 1);
            ownedQuantity++;
            itemOwnedQuantityText.text = ownedQuantity.ToString() + " OWNED";
        }
    }
}
