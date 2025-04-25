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
    [SerializeField] private GameObject disabledObj;
    [SerializeField] private GameObject lockedImage;
    [SerializeField] private TextMeshProUGUI unlockedLvlText;  

    [Header("Building Option")]
    [SerializeField] private GameObject rotatedCard;
    [SerializeField] private GameObject buildingInfos;
    [SerializeField] private Button infoButton;
    [SerializeField] private GameObject buildingStatsContainer;
    [SerializeField] private GameObject buildingBonusContainer;
    [SerializeField] private GameObject buildingStatsPrefab;
    [SerializeField] private GameObject buildingBonusPrefab;
    [SerializeField] private Sprite moodEffectSprite;
    [SerializeField] private Sprite serenityEffectSprite;
    [SerializeField] private Sprite energyEffectSprite;

    public void SetItemContent(Player.ItemCategory _category, Sprite _icon, string _name, int _price)
    {
        itemCategory = _category;
        itemIcon.sprite = _icon;
        itemName.text = _name;
        itemPrice.text = _price.ToString();
        RefreshInfo();

        if(itemCategory == Player.ItemCategory.BuildingCategory)
        {
            infoButton.gameObject.SetActive(true);
            Building building = GM.Instance.GetBuildingByName(itemName.text.ToString());
            if(building.Mood != 0)
            {
                GameObject moodObj = Instantiate(buildingStatsPrefab, buildingStatsContainer.transform);
                moodObj.GetComponentInChildren<TextMeshProUGUI>().text = building.Mood > 0 ? "+" + building.Mood.ToString() : building.Mood.ToString();
                moodObj.GetComponentInChildren<Image>().sprite = moodEffectSprite;
            }
            if (building.Serenity != 0)
            {
                GameObject serenityObj = Instantiate(buildingStatsPrefab, buildingStatsContainer.transform);
                serenityObj.GetComponentInChildren<TextMeshProUGUI>().text = building.Serenity > 0 ? "+" + building.Serenity.ToString() : building.Serenity.ToString();
                serenityObj.GetComponentInChildren<Image>().sprite = serenityEffectSprite;
            }
            if (building.Energy != 0)
            {
                GameObject energyObj = Instantiate(buildingStatsPrefab, buildingStatsContainer.transform);
                energyObj.GetComponentInChildren<TextMeshProUGUI>().text = building.Energy > 0 ? "+" + building.Energy.ToString() : building.Energy.ToString();
                energyObj.GetComponentInChildren<Image>().sprite = energyEffectSprite;
            }
            foreach (var bonus in building.AttributeEffects)
            {
                GameObject bonusObj = Instantiate(buildingBonusPrefab, buildingBonusContainer.transform);
                if(bonus.attribute != null && bonus.attribute.icon != null)
                {
                    bonusObj.GetComponent<Image>().sprite = bonus.attribute.icon;
                }
            }
        }
    }

    public void RefreshInfo()
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
        CanBuyItem(GetItem<IScriptableElement>());  
    }

    public void CanBuyItem<T>(T _item) where T : IScriptableElement
    {
        if (GM.Instance.player.CanSpendGold(_item.InitialPrice) && ownedQuantity < _item.MaxOwned && _item.UnlockedAtLvl <= GM.Instance.player.Level)
        {
            buyButton.interactable = true;
            infoButton.interactable = true;
            disabledObj.SetActive(false);
            itemIcon.color = Color.white;
            lockedImage.SetActive(false);
        }
        else
        {
            buyButton.interactable = false;
            infoButton.interactable = false;
            disabledObj.SetActive(true);
            if(_item.UnlockedAtLvl > GM.Instance.player.Level)
            {
                itemIcon.color = Color.black;
                lockedImage.SetActive(true);
                unlockedLvlText.text = "LVL " + _item.UnlockedAtLvl.ToString();
            }
            if(itemCategory == Player.ItemCategory.BuildingCategory && isInfos)
            {
                BS_Info();
            }
        }
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
            CanBuyItem(item);
        }
    }

    public void BS_Buy()
    {
        BuyItem<IScriptableElement>();

        GM.Tm.UnHold(49);
    }

    public void BS_Info()
    {
        isInfos = !isInfos;
        if (isInfos) 
        {
            buyButton.interactable = false;
            infoButton.interactable = false;

            LMotion.Create(0.0f, 180.0f, 1.0f)
                .WithEase(Ease.InOutSine)
                .WithOnComplete(() =>
                {
                    infoButton.transform.GetChild(0).gameObject.SetActive(false);
                    infoButton.transform.GetChild(1).gameObject.SetActive(true);
                    infoButton.interactable = true;
                })
                .Bind((float rotation) =>
                {
                    rotatedCard.transform.rotation = Quaternion.Euler(0, rotation, 0);

                    if (rotation >= 90)
                    {
                        notBuildingInfos.transform.SetAsFirstSibling();
                    }
                }
            );   
        }
        else
        {
            buyButton.interactable = true;
            infoButton.interactable = false;

            LMotion.Create(180.0f, 0.0f, 1.0f)
                .WithEase(Ease.InOutSine)
                .WithOnComplete(() =>
                {
                    infoButton.transform.GetChild(0).gameObject.SetActive(true);
                    infoButton.transform.GetChild(1).gameObject.SetActive(false);
                    infoButton.interactable = true;
                })
                .Bind((float rotation) =>
                {
                    rotatedCard.transform.rotation = Quaternion.Euler(0, rotation, 0);

                    if (rotation <= 90)
                    {
                        buildingInfos.transform.SetAsFirstSibling();
                    }
                }
            );
        }
    }
}
