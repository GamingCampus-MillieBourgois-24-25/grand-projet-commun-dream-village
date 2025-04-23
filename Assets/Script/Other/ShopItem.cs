using LitMotion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    private Player.ItemCategory itemCategory;
    private int ownedQuantity = 0;
    private bool isInfos = false;

    [Header("Main UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemOwnedQuantityText;
    [SerializeField] private GameObject notBuildingInfos;
    [SerializeField] private Button buyButton;

    [Header("Building Option")]
    [SerializeField] private GameObject rotatedCard;
    [SerializeField] private GameObject buildingInfos;
    [SerializeField] private Button infoButton;
    [SerializeField] private GameObject buildingStatsPrefab;
    [SerializeField] private GameObject buildingSpritePrefab;

    public void SetItemContent(Player.ItemCategory _category, Sprite _icon, string _name, int _price)
    {
        itemCategory = _category;
        itemIcon.sprite = _icon;
        itemName.text = _name;
        itemPrice.text = _price.ToString();
        UpdateOwnedQuantity();

        if(itemCategory == Player.ItemCategory.BuildingCategory)
        {
            infoButton.gameObject.SetActive(true);

        }
    }

    public void UpdateOwnedQuantity()
    {
        switch (itemCategory)
        {
            case Player.ItemCategory.InhabitantCategory:
                Inhabitant inhabitantItem = GetItem<Inhabitant>();
                ownedQuantity = GM.Instance.player.GetItemQuantity(inhabitantItem);
                foreach (var inhabitant in GM.VM.inhabitants)
                {
                    if (inhabitant.baseData == inhabitantItem)
                    {
                        ownedQuantity++;
                    }
                }

                break;
            case Player.ItemCategory.BuildingCategory:
                Building buildingItem = GetItem<Building>();
                ownedQuantity = GM.Instance.player.GetItemQuantity(buildingItem);
                foreach (var building in GM.VM.buildings)
                {
                    if (building.baseData == buildingItem)
                    {
                        ownedQuantity++;
                    }
                }
                break;
            case Player.ItemCategory.DecorationCategory:
                Debug.LogWarning("Not Implemented");
                break;
            default:
                Debug.LogError("Invalid category");
                break;
        }
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

    private void BuyItem<T>() where T : IScriptableElement
    {
        T item = GetItem<T>();
        if (GM.Instance.player.CanSpendGold(item.InitialPrice))
        {
            GM.Instance.player.SpendGold(item.InitialPrice);
            GM.Instance.player.AddToInventory(item, 1);
            ownedQuantity++;
            itemOwnedQuantityText.text = ownedQuantity.ToString() + " OWNED";
        }
    }

    public void BS_Buy()
    {
        BuyItem<IScriptableElement>();
    }

    public void BS_Info()
    {
        isInfos = !isInfos;
        if (isInfos) 
        {
            LMotion.Create(0.0f, 180.0f, 1.0f)
                .WithEase(Ease.InOutSine)
                .Bind((float rotation) =>
                {
                    rotatedCard.transform.rotation = Quaternion.Euler(0, rotation, 0);

                    if (rotation >= 90)
                    {
                        notBuildingInfos.transform.SetAsFirstSibling();
                    }
                });   

            buyButton.interactable = false;
        }
        else
        {
            LMotion.Create(180.0f, 0.0f, 1.0f)
                .WithEase(Ease.InOutSine)
                .Bind((float rotation) =>
                {
                    rotatedCard.transform.rotation = Quaternion.Euler(0, rotation, 0);

                    if (rotation <= 90)
                    {
                        buildingInfos.transform.SetAsFirstSibling();
                    }
                });

            buyButton.interactable = true;
        }
    }
}
