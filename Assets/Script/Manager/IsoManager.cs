using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class IsoManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    [SerializeField] public Tilemap tilemapBase;
    [SerializeField] public Tilemap tilemapObjects;
    [SerializeField] private TileBase whiteTile;
    [SerializeField] private TileBase greenTile;
    [SerializeField] private TileBase redTile;
    [SerializeField] private LayerMask IslandLayer;

    [Header("UI")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas editModeCanvas;
    [SerializeField] private Canvas stockCanvas;
    [SerializeField] private float yStockCanvas;
    //[SerializeField] private Button placeBtn;

    [Header("Sounds")]
    [SerializeField] private AudioClip placeBuildingSFX;

    private GameObject canvasBottomLeft;
    private GameObject canvasBottomRight;

    [SerializeField] private float yMovingObject;

    private HashSet<Vector3Int> validWhiteTilePositions;
    private HashSet<Vector3Int> occupiedTilePositions;
    private PlaceableObject selectedObject;
    public bool isEditMode = false;

    private bool isClicking = false;
    private bool isDragging = false;

    [SerializeField] private InputActionReference clickAction;
    [SerializeField] private InputActionReference dragAction;

    private TilemapRenderer tileRenderer;

    private Coroutine scaleAnimationCoroutine;
    private Coroutine inventoryMoveCoroutine;

    #region Unity Functions

    private void Start()
    {
        tileRenderer = transform.GetChild(0).GetComponent<TilemapRenderer>();
        tileRenderer.enabled = isEditMode;

        editModeCanvas.gameObject.SetActive(isEditMode);

        canvasBottomLeft = mainCanvas.transform.Find("BottomLeft").gameObject;
        canvasBottomRight = mainCanvas.transform.Find("BottomRight").gameObject;

        CacheWhiteTilePositions();
        AddExistingObjectsToOccupiedPositions();
    }
    #endregion

    #region Input

    private void OnEnable()
    {


        clickAction.action.performed += OnClickPerformed;
        clickAction.action.canceled += OnClickCancelled;
        dragAction.action.performed += OnDragPerformed;

        dragAction.action.Enable();
        clickAction.action.Enable();
    }

    private void OnDisable()
    {
        clickAction.action.performed -= OnClickPerformed;
        dragAction.action.performed -= OnDragPerformed;
        clickAction.action.canceled -= OnClickCancelled;

        clickAction.action.actionMap.Disable();
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (!isEditMode) return;

        isClicking = true;

        //Debug.Log("OnClickPerformed");
        Vector2 pointerPos = GM.Instance.GetPointerPosition(context);
        CheckUnderPointerTouch(pointerPos);
    }

    private void OnDragPerformed(InputAction.CallbackContext context)
    {
        if (!isEditMode || !isClicking) return;

        isDragging = true;

        //Debug.Log("OnDragPerformed");
        Vector2 pointerPos = context.ReadValue<Vector2>();
        CheckUnderPointerMove(pointerPos);
    }

    private void OnClickCancelled(InputAction.CallbackContext context)
    {
        if (!isEditMode) return;

        if ((selectedObject && isDragging))
        {
            scaleAnimationCoroutine = StartCoroutine(AnimateScalePop(selectedObject.transform));

            ChangeTileUnderObject(selectedObject, null);
            if (CanPlaceObjectOnTilemap(selectedObject))
            {
                PlacePlaceableObject(selectedObject);
            } 
            else
            {
                selectedObject.ResetPosition();
                ToggleInventorySmooth(true);
            }
            UnSelectObject();
        }

        isClicking = false;
        isDragging = false;
    }
    #endregion

    #region Touch/Move
    //private Vector2 GetPointerPosition(InputAction.CallbackContext context)
    //{
    //    // Si besoin de récupérer la position à partir du device
    //    if (Pointer.current != null)
    //        return Pointer.current.position.ReadValue();
    //    return Vector2.zero;
    //}
    //private bool IsPointerOverUIElement(Vector2 screenPosition)
    //{
    //    PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
    //    {
    //        position = screenPosition
    //    };

    //    List<RaycastResult> results = new List<RaycastResult>();
    //    EventSystem.current.RaycastAll(pointerEventData, results);

    //    return results.Count > 0; // If there's any UI element under the pointer, return true
    //}

    private void CheckUnderPointerTouch(Vector2 screenPosition)
    {
        // Cancel si click sur un bouton de l'UI
        if (GM.Instance.IsPointerOverUIElement(screenPosition))
        {
            return;
        }

        // Au toucher on peut changer d'objet ou move celui qu'on a
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            PlaceableObject obj = hit.collider.GetComponent<PlaceableObject>();
            BuildingObject bat = hit.collider.GetComponent<BuildingObject>();
            if (obj != null && obj != selectedObject)
            {
                if (!(bat && bat.IsUsed))
                {
                    OnObjectSelected(obj);
                }
            }
            else if (selectedObject != null)
            {
                MoveSelectedObject(hit);
            }
        }
    }

    private void CheckUnderPointerMove(Vector2 screenPosition)
    {
        // Cancel si click sur un bouton de l'UI
        if (GM.Instance.IsPointerOverUIElement(screenPosition))
        {
            return;
        }

        // Au move on ne peut que move le selectedobject
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, IslandLayer))
        {
            if (selectedObject != null)
            {
                MoveSelectedObject(hit);
            }
        }
    }

    private void MoveSelectedObject(RaycastHit hit)
    {
        if (selectedObject == null) return; // Sécurité

        ChangeTileUnderObject(selectedObject, null);

        //Vector3Int gridPosition = tilemapBase.WorldToCell(hit.point);
        //Vector3 newPosition = tilemapBase.CellToWorld(gridPosition);
        Vector3 newPosition = selectedObject.GetCenterObject(tilemapObjects, hit.point);
        newPosition.y += yMovingObject;
        selectedObject.transform.position = newPosition;

        CheckObjectOnTilemap(selectedObject);
    }
    #endregion

    #region Tiles

    // Récupère toutes les cases valides au départ
    private void CacheWhiteTilePositions()
    {
        validWhiteTilePositions = new HashSet<Vector3Int>();

        BoundsInt bounds = tilemapBase.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemapBase.GetTile(pos) == whiteTile)
            {
                validWhiteTilePositions.Add(pos);
                //Debug.Log(pos);
            }
        }
    }

    private void AddExistingObjectsToOccupiedPositions()
    {
        occupiedTilePositions = new HashSet<Vector3Int>();

        foreach (var placeableObject in FindObjectsOfType<PlaceableObject>())
        {
            // Ajouter les positions de chaque objet déjà présent sur la scène
            Renderer renderer = placeableObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                foreach (var cellPos in GetCoveredCells(renderer, tilemapBase))
                {
                    if (!occupiedTilePositions.Contains(cellPos))
                        occupiedTilePositions.Add(cellPos);
                }
            }
        }
    }

    public static IEnumerable<Vector3Int> GetCoveredCells(Renderer renderer, Tilemap tilemap)
    {
        Vector3Int bottomLeftCell = tilemap.WorldToCell(renderer.bounds.min);
        Vector3Int topRightCell = tilemap.WorldToCell(renderer.bounds.max);

        for (int x = bottomLeftCell.x; x <= topRightCell.x; x++)
        {
            for (int y = topRightCell.y; y <= bottomLeftCell.y; y++)
            {
                yield return new Vector3Int(x, y, 0);
            }
        }
    }

    private void ChangeTileUnderObject(PlaceableObject obj, TileBase tileType)
    {
        // Pour changer les Tiles sous un objet du type voulu
        Renderer renderer = obj.GetComponent<Renderer>();
        if (obj != null && renderer != null)
        {
            foreach (var cellPos in GetCoveredCells(renderer, tilemapBase))
            {
                if (validWhiteTilePositions.Contains(cellPos))
                {
                    tilemapObjects.SetTile(cellPos, tileType);
                }
            }
        }
        else
        {
            Debug.LogError("Can't clean under object because obj is null or obj does not have a renderer component!");
        }
    }
    #endregion

    #region Object

    private void OnObjectSelected(PlaceableObject obj)
    {
        if (obj == null) return; // Sécurité

        //Debug.Log("OnObjectSelected");

        // Si un objet est déjà sélectionné, il revient à sa position initiale
        if (selectedObject != null)
        {
            ChangeTileUnderObject(selectedObject, null);
            if (CanPlaceObjectOnTilemap(selectedObject))
            {
                PlacePlaceableObject(selectedObject);
            }
            else
            {
                selectedObject.ResetPosition();
            }
        }
        else
        {
            ToggleInventorySmooth(false);
        }

        occupiedTilePositions.ExceptWith(obj.GetOccupiedTiles());

        selectedObject = obj;

        if (stockCanvas != null)
        {
            stockCanvas.transform.position = new Vector3(obj.transform.position.x, (obj.cachedRenderer.bounds.size.y ) + yStockCanvas, obj.transform.position.z);
            //Debug.Log(obj.cachedRenderer.bounds.size.y + " " + obj.cachedRenderer.bounds.size.y / obj.transform.localScale.y + " " + ((obj.cachedRenderer.bounds.size.y / obj.transform.localScale.y) + yStockCanvas));
            stockCanvas.transform.SetParent(selectedObject.transform, worldPositionStays: true);
            stockCanvas.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
            stockCanvas.gameObject.SetActive(true);
        }

        selectedObject.transform.position = new Vector3(selectedObject.transform.position.x, transform.position.y + yMovingObject, selectedObject.transform.position.z);
        //if (placeBtn) placeBtn.interactable = true;
        CheckObjectOnTilemap(selectedObject);

        Debug.Log("Objet sélectionné : " + obj.name);
    }

    private void UnSelectObject()
    {
        if (selectedObject == null) return; // Sécurité

        stockCanvas.gameObject.SetActive(false);
        selectedObject = null;
    }

    public bool HasSelectedObject()
    {
        if (isEditMode && selectedObject != null)
        {
            return true;
        }
        return false;
    }

    public void CheckObjectOnTilemap(PlaceableObject obj)
    {
        if (obj == null) return; // Sécurité

        bool canPlace = CanPlaceObjectOnTilemap(obj); // Vérifie si l'objet peut être placé

        if (canPlace)
        {
            //if (placeBtn) placeBtn.interactable = true;
            ChangeTileUnderObject(obj, greenTile);
        }
        else
        {
            //if (placeBtn) placeBtn.interactable = false;
            ChangeTileUnderObject(obj, redTile); 
        }
    }

    // Fonction qui vérifie si l'objet peut être placé sur la tilemap
    private bool CanPlaceObjectOnTilemap(PlaceableObject obj)
    {
        if (obj == null) return false; // Sécurité

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return false; // Sécurité

        foreach (var cellPos in GetCoveredCells(renderer, tilemapBase))
        {
            // Si la case est invalide
            if (!validWhiteTilePositions.Contains(cellPos) || occupiedTilePositions.Contains(cellPos))
                return false;
        }

        return true;
    }

    private void PlacePlaceableObject(PlaceableObject obj)
    {
        if (obj == null || !CanPlaceObjectOnTilemap(obj)) return; // Sécurité

        GM.SM.PlaySFX(placeBuildingSFX);

        float objectHeight = obj.GetComponent<Renderer>().bounds.size.y;
        // TODO: à changer plus tard si toutes les origines des batiments sont en bas !
        //float newYPosition = GM.IM.transform.position.y + (objectHeight / 2f);
        float newYPosition = GM.IM.transform.position.y;

        obj.transform.position = new Vector3(obj.transform.position.x, newYPosition, obj.transform.position.z);

        occupiedTilePositions.UnionWith((obj.GetOccupiedTiles()));
        ToggleInventorySmooth(true);

        // SET NEW POSITION
        obj.OriginalPosition = tilemapObjects.WorldToCell(obj.transform.position);

        // Réinitialiser
        ChangeTileUnderObject(selectedObject, null);
        UnSelectObject();
        //if (placeBtn) placeBtn.interactable = false;
    }
    #endregion

    #region UI
    private void HideMainUI(bool hide)
    {
        if (hide)
        {
            canvasBottomLeft.SetActive(false);
            canvasBottomRight.SetActive(false);
        } else
        {
            canvasBottomLeft.SetActive(true);
            canvasBottomRight.SetActive(true);
        }
    }

    #endregion

    #region Anims
    private IEnumerator AnimateScalePop(Transform target, float offset = 2f, float duration = 0.1f)
    {
        if (target == null) yield break;

        Vector3 originalScale = target.localScale;
        Vector3 targetScale = originalScale + new Vector3(offset, offset, offset);

        float time = 0f;
        while (time < duration)
        {
            target.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        target.localScale = targetScale;

        // Retour au scale original
        time = 0f;
        while (time < duration)
        {
            target.localScale = Vector3.Lerp(targetScale, originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        target.localScale = originalScale;
    }

    public void ToggleInventorySmooth(bool open)
    {
        if (inventoryMoveCoroutine != null)
            StopCoroutine(inventoryMoveCoroutine);

        inventoryMoveCoroutine = StartCoroutine(SmoothMoveInventory(open));
    }

    private IEnumerator SmoothMoveInventory(bool open)
    {
        RectTransform bottom = editModeCanvas.transform.Find("Bottom").GetComponent<RectTransform>();
        RectTransform under = bottom.Find("Under").GetComponent<RectTransform>();

        float duration = 0.25f;
        float time = 0f;

        Vector2 startPos = bottom.anchoredPosition;
        float targetY = open ? 0f : -under.rect.height;
        Vector2 targetPos = new Vector2(startPos.x, targetY);

        while (time < duration)
        {
            float t = time / duration;
            t = t * t * (3f - 2f * t); // Smoothstep
            bottom.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        bottom.anchoredPosition = targetPos;
    }



    #endregion

    #region Btns Functions
    public void BS_PlaceSelectedObject()
    {
        PlacePlaceableObject(selectedObject);
    }
    public void BS_ToggleEditMode()
    {
        isEditMode = !isEditMode;

        GM.BM.canvasBuilding.gameObject.SetActive(false);

        if (isEditMode) {
            HideMainUI(true);
            GM.JournalPanel.SetActive(false);
        }
        else
        {
            HideMainUI(false);
            GM.JournalPanel.SetActive(true);
        }

        tileRenderer.enabled = isEditMode;

        editModeCanvas.gameObject.SetActive(isEditMode);
        tilemapObjects.ClearAllTiles();
        //if (selectedObject != null) selectedObject.ResetPosition();
        if (selectedObject != null)
        {
            if (CanPlaceObjectOnTilemap(selectedObject)) {
                PlacePlaceableObject(selectedObject);
            }
            else
            {
                selectedObject.ResetPosition();
            }
        }
        UnSelectObject();
    }
    public void BS_StockSelectedObject()
    {
        if(selectedObject.TryGetComponent<BuildingObject>(out BuildingObject buildingObj))
        {
            GM.VM.RemoveInstance(buildingObj.gameObject);
            GM.Instance.player.AddToInventory(buildingObj.baseData, 1);
        }
        else if (selectedObject.TryGetComponent<HouseObject>(out HouseObject houseObj))
        {
            GM.VM.RemoveInstance(houseObj.gameObject);
            GM.Instance.player.AddToInventory(houseObj.inhabitantInstance.baseData, 1);
        }
        else
        {
            Debug.LogWarning("Selected object is not a BuildingObject or HouseObject.");
            return;
        }

        GameObject objToDestroy = selectedObject.gameObject;

        tilemapObjects.ClearAllTiles();
        UnSelectObject();
        stockCanvas.transform.parent = null;
        GM.BM.canvasBuilding.transform.parent = null;
        Destroy(objToDestroy);

        ToggleInventorySmooth(true);
    }

    #endregion

    #region SpawnInventoryItem

    public GameObject SpawnInventoryItem<T>(T item, Vector3 _spawnPoint) where T : IScriptableElement
    {
        if (!GM.Instance.player.GetItemInInventory(item, out var entry))
        {
            Debug.LogWarning("Item not in inventory or prefab is missing.");
            return null;
        }

        Vector3 centerPos = tilemapBase.WorldToCell(_spawnPoint);
        centerPos.y += yMovingObject;

        GameObject newObj = Instantiate(item.InstantiatePrefab, centerPos, item.InstantiatePrefab.transform.rotation, GM.Instance.playerIslandObject);
        PlaceableObject placeable = newObj.GetComponent<PlaceableObject>();

        if (placeable != null)
        {
            OnObjectSelected(placeable);
            GM.Instance.player.RemoveFromInventory(item, 1);
            return newObj;
        }
        else
        {
            Debug.LogError("Prefab does not contain a PlaceableObject.");
        }
        return null;
    }

    #endregion

}

