using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
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

    private void SpawnPrefab(Vector2 _mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GM.IM.BS_TakeInventoryItem(inventoryItem, hit.point);
        }
        else
        {
            Tilemap tilemap = GM.IM.tilemapBase;
            Plane gridPlane = new Plane(Vector3.up, tilemap.transform.position);

            if (gridPlane.Raycast(ray, out float distance))
            {
                Vector3 projectedPoint = ray.GetPoint(distance);
                Vector3Int cell = tilemap.WorldToCell(projectedPoint);

                Debug.Log("Spawn position: " + cell);
                BoundsInt bounds = tilemap.cellBounds;

                if(cell.x < bounds.min.x)
                {
                    cell.x = bounds.min.x;
                }
                else if(cell.x > bounds.max.x)
                {
                    cell.x = bounds.max.x;
                }

                if (cell.y < bounds.min.y)
                {
                    cell.y = bounds.min.y;
                }
                else if (cell.y > bounds.max.y)
                {
                    cell.y = bounds.max.y;
                }

                Vector3 spawnPos = tilemap.GetCellCenterWorld(cell);

                GM.IM.BS_TakeInventoryItem(inventoryItem, spawnPos);
            }
            else
            {
                Debug.LogWarning("Impossible de projeter la souris sur la grille.");
            }
        }

        Destroy(gameObject);
    }

}

