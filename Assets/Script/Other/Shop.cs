using System.Collections.Generic;
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


    [Header("UI")]
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private List<Button> categoryButtons;
    [SerializeField] private List<GameObject> categoryContainers;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        InitCatagory(categoryContainers[0], GameManager.instance.inhabitants);
        InitCatagory(categoryContainers[1], GameManager.instance.buildings);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void InitCatagory<T>(GameObject _container, List<T> _contents)
    {
        foreach(T item in _contents)
        {
            GameObject obj = Instantiate(itemPrefab, _container.transform);
            switch (item)
            {
                case Inhabitant inhabitant:
                    obj.GetComponent<ShopItem>().SetItemContent(inhabitant.Icon, inhabitant.FirstName + " " + inhabitant.LastName, inhabitant.InitialPrice);
                    break;
                case Building building:
                    obj.GetComponent<ShopItem>().SetItemContent(building.effect.icon, building.effect.name, building.price);
                    break;
                //case Decoration decoration:
                //    SetItemContent(decoration, obj);
                //    break;
                default:
                    Debug.LogWarning($"Type {typeof(T)} non pris en charge dans SetItemContent.");
                    break;
            }
        }
    }

    public void SwitchCategory(int _category)
    {
        if(_category < 0 || _category > 3)
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
}
