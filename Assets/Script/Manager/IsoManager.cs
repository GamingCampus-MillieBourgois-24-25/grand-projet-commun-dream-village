using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class IsoManager : MonoBehaviour
{
    public static IsoManager Instance { get; private set; } // Singleton

    [SerializeField] private InputActionAsset inputActions;

    [SerializeField] private Tilemap tilemapBase;
    [SerializeField] public Tilemap tilemapObjects;
    [SerializeField] private TileBase whiteTile;
    [SerializeField] private TileBase greenTile;
    [SerializeField] private TileBase redTile;
    [SerializeField] private LayerMask GridLayer;


    [SerializeField] private Canvas editModeCanvas;
    [SerializeField] private Button placeBtn;

    [SerializeField] private float yMovingObject;

    private HashSet<Vector3Int> validWhiteTilePositions;
    private HashSet<Vector3Int> occupiedTilePositions;
    private PlaceableObject selectedObject;
    private bool isEditMode = false;

    private InputAction clickAction;
    private InputAction dragAction;

    private TilemapRenderer tileRenderer;

    #region Unity Functions
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Supprime les doublons
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        tileRenderer = transform.GetChild(0).GetComponent<TilemapRenderer>();
        tileRenderer.enabled = isEditMode;

        CacheWhiteTilePositions();
        AddExistingObjectsToOccupiedPositions();
    }
    #endregion

    #region Input

    private void OnEnable()
    {
        var baseSceneMap = inputActions.FindActionMap("BaseScene");

        clickAction = baseSceneMap.FindAction("Click");
        dragAction = baseSceneMap.FindAction("Drag");

        clickAction.performed += OnClickPerformed;
        dragAction.performed += OnDragPerformed;

        baseSceneMap.Enable();
    }

    private void OnDisable()
    {
        clickAction.performed -= OnClickPerformed;
        dragAction.performed -= OnDragPerformed;

        clickAction.actionMap.Disable();
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (!isEditMode) return;

        Debug.Log("OnClickPerformed");
        Vector2 pointerPos = GetPointerPosition(context);
        CheckUnderPointerTouch(pointerPos);
    }

    private void OnDragPerformed(InputAction.CallbackContext context)
    {
        if (!isEditMode) return;

        //Debug.Log("OnDragPerformed");
        Vector2 pointerPos = context.ReadValue<Vector2>();
        CheckUnderPointerMove(pointerPos);
    }
    #endregion

    #region Touch/Move
    private Vector2 GetPointerPosition(InputAction.CallbackContext context)
    {
        // Si besoin de récupérer la position à partir du device
        if (Pointer.current != null)
            return Pointer.current.position.ReadValue();
        return Vector2.zero;
    }

    private void CheckUnderPointerTouch(Vector2 screenPosition)
    {
        // Au toucher on peut changer d'objet ou move celui qu'on a
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            PlaceableObject obj = hit.collider.GetComponent<PlaceableObject>();
            if (obj != null && obj != selectedObject)
            {
                OnObjectSelected(obj);
            }
            else if (selectedObject != null)
            {
                MoveSelectedObject(hit);
            }
        }
    }

    private void CheckUnderPointerMove(Vector2 screenPosition)
    {
        // Au move on ne peut que move le selectedobject
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, GridLayer))
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

        Vector3Int gridPosition = tilemapBase.WorldToCell(hit.point);
        Vector3 newPosition = tilemapBase.CellToWorld(gridPosition);
        newPosition.y = selectedObject.transform.position.y + yMovingObject;
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

        // Si un objet est déjà sélectionné, il revient à sa position initiale
        if (selectedObject != null)
        {
            ChangeTileUnderObject(selectedObject, null);
            selectedObject.ResetPosition();
        }

        Debug.Log(String.Join(",", obj.GetOccupiedTiles()));
        occupiedTilePositions.ExceptWith(obj.GetOccupiedTiles());

        selectedObject = obj;
        selectedObject.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y + yMovingObject, selectedObject.transform.position.z);
        placeBtn.interactable = true;

        Debug.Log("Objet sélectionné : " + obj.name);
    }
    public void CheckObjectOnTilemap(PlaceableObject obj)
    {
        if (obj == null) return; // Sécurité

        bool canPlace = CanPlaceObjectOnTilemap(obj); // Vérifie si l'objet peut être placé

        if (canPlace)
        {
            placeBtn.interactable = true;
            ChangeTileUnderObject(obj, greenTile);
        }
        else
        {
            placeBtn.interactable = false;
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

        float objectHeight = obj.GetComponent<Renderer>().bounds.size.y;
        // TODO: à changer plus tard si toutes les origines des batiments sont en bas !
        float newYPosition = IM.Instance.transform.position.y + (objectHeight / 2f);

        obj.transform.position = new Vector3(obj.transform.position.x, newYPosition, obj.transform.position.z);

        occupiedTilePositions.Union((obj.GetOccupiedTiles()));

        // SET NEW POSITION
        obj.OriginalPosition = tilemapObjects.WorldToCell(transform.position);

        // Réinitialiser
        ChangeTileUnderObject(selectedObject, null);
        selectedObject = null;
        placeBtn.interactable = false;
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

        tileRenderer.enabled = isEditMode;

        editModeCanvas.gameObject.SetActive(isEditMode);
        tilemapObjects.ClearAllTiles();
        if (selectedObject != null) selectedObject.ResetPosition();
        selectedObject = null;
    }
#endregion
}

public static class IM
{
    public static IsoManager Instance => IsoManager.Instance;
}

