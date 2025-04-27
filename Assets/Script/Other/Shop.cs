using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private enum ShopCategory
    {
        InhabitantCategory,
        BuildingCategory,
        DecorationCategory,
        MoneyCategory
    }

    #region Variables

    [SerializeField] private ShopCategory shopCategory = ShopCategory.InhabitantCategory;
    [SerializeField] private GameObject itemPrefab;
    private LevelProgression levelProgression;


    [Header("UI")]
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private List<Button> categoryButtons;
    [SerializeField] private List<GameObject> categoryContainers;

    #endregion

    public void InitShop()
    {
        levelProgression = GM.Instance.gameObject.GetComponent<LevelProgression>();

        InitCategory(categoryContainers[0], GameManager.instance.inhabitants.OrderBy(x => x.InitialPrice).ToList());
        InitCategory(categoryContainers[1], GameManager.instance.buildings.OrderBy(x => x.InitialPrice).ToList());
        InitCategory(categoryContainers[2], GameManager.instance.decorations.OrderBy(x => x.InitialPrice).ToList());
    }


    private void InitCategory<T>(GameObject _container, List<T> _contents) where T : IScriptableElement
    {
        foreach (IScriptableElement item in _contents)
        {
            if (item.InitialPrice > 0)
            {
                GameObject obj = Instantiate(itemPrefab, _container.transform);
                obj.GetComponent<ShopItem>().SetItemContent(item.Category, item.Icon, item.Name, item.InitialPrice);
                levelProgression.AddItemOnLevel(item.UnlockedAtLvl, item);
            }
        }
    }

    public void BS_SwitchCategory(int _category)
    {
        if (_category < 0 || _category > 3)
        {
            Debug.LogError("Invalid shop category index");
            return;
        }

        categoryButtons[(int)shopCategory].interactable = true;
        categoryContainers[(int)shopCategory].SetActive(false);

        shopCategory = (ShopCategory)_category;

        categoryButtons[_category].interactable = false;
        categoryContainers[_category].SetActive(true);
        scrollView.content = categoryContainers[_category].GetComponent<RectTransform>();
    }

    public void BS_RefreshShop()
    {
        foreach (Transform item in categoryContainers[0].transform)
        {
            if (item.TryGetComponent<ShopItem>(out ShopItem shopItem))
            {
                shopItem.RefreshInfo();
            }
        }
        foreach (Transform item in categoryContainers[1].transform)
        {
            if (item.TryGetComponent<ShopItem>(out ShopItem shopItem))
            {
                shopItem.RefreshInfo();
            }
        }
        foreach (Transform item in categoryContainers[2].transform)
        {
            if (item.TryGetComponent<ShopItem>(out ShopItem shopItem))
            {
                shopItem.RefreshInfo();
            }
        }
    }
}
