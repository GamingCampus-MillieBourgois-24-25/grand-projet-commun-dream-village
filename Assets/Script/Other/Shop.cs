using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

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

    [SerializeField] private GameObject inhabitantContainer;
    [SerializeField] private GameObject buildingContainer;
    [SerializeField] private GameObject decorationContainer;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
       InitCatagory(inhabitantContainer, GameManager.instance.inhabitants);
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
                    SetItemContent(inhabitant, obj);
                    break;
                case Building building:
                    SetItemContent(building, obj);
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

    private void SetItemContent(Inhabitant _item, GameObject _obj)
    {
        _obj.GetComponentInChildren<Image>().sprite = _item.Icon;
        _obj.GetComponentInChildren<TextMeshProUGUI>().text = _item.FirstName + " " + _item.LastName;
        _obj.GetComponentInChildren<TextMeshProUGUI>().text = 
    }

    private void SetItemContent(Building _item, GameObject _obj)
    {

    }

    private void SwitchCategory(int _category)
    {
        if(_category < 0 || _category > 3)
        {
            Debug.LogError("Invalid shop category index");
            return;
        }

        shopCategory = (ShopCategory)_category;


    }
}
