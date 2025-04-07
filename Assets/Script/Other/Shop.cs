using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    
    private void InitCatagory(GameObject _container, List<Inhabitant> _contents)
    {
        foreach(Inhabitant item in _contents)
        {
            GameObject obj = Instantiate(itemPrefab, _container.transform);
            SetItemContent(item, obj);
        }
    }

    private void SetItemContent(Inhabitant _item, GameObject _obj)
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
