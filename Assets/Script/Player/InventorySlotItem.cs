using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    
    private IScriptableElement inventoryItem;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI quantityText;

    [Header("Drag To Build")]
    private LayoutElement layoutElement;
    private float initialHeight;
    private RectTransform rectTransform;
    bool isInside = true;

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

    public void OnBeginDrag(PointerEventData eventData)
    {
        layoutElement = gameObject.AddComponent<LayoutElement>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        initialHeight = rectTransform.sizeDelta.y;
        layoutElement.minHeight = initialHeight;
    }

    public void OnDrag(PointerEventData eventData)
    {
        layoutElement.minHeight += eventData.delta.y;

        if (isInside)
        {
            isInside = RectTransformUtility.RectangleContainsScreenPoint(
            gameObject.GetComponentInParent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera);
        }
        else
        {
            SpawnPrefab(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(layoutElement);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, initialHeight);
    }

    private void SpawnPrefab(Vector2 _mousPos)
    {
        GM.Instance.player.RemoveFromInventory(inventoryItem, 1);
        Ray ray = Camera.main.ScreenPointToRay(_mousPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GM.IM.BS_TakeInventoryItem(inventoryItem, hit.point);
            Destroy(gameObject);
        }
    }

}

