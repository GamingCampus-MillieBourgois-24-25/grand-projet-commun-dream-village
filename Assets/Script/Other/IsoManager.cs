using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    }
    #endregion

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

        Debug.Log("OnDragPerformed");
        Vector2 pointerPos = context.ReadValue<Vector2>();
        CheckUnderPointerMove(pointerPos);
    }

    private Vector2 GetPointerPosition(InputAction.CallbackContext context)
    {
        // Si besoin de récupérer la position à partir du device
        if (Pointer.current != null)
            return Pointer.current.position.ReadValue();
        return Vector2.zero;
    }

    private void CheckUnderPointerTouch(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            PlaceableObject obj = hit.collider.GetComponent<PlaceableObject>();
            if (obj != null && obj != selectedObject)
            {
                // Si c'est un objet on change d'objet
                OnObjectSelected(obj);
            }
            else if (selectedObject != null)
            {
                // Sinon juste on le move
                MoveSelectedObject(hit);
            }
        }
    }

    private void CheckUnderPointerMove(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, GridLayer))
        {
            // Move selected object
            if (selectedObject != null)
            {
                MoveSelectedObject(hit);
            }
        }
    }

    private void OnObjectSelected(PlaceableObject obj)
    {
        if (obj == null) return; // Sécurité

        // Si un objet est déjà sélectionné, il revient à sa position initiale
        if (selectedObject != null)
        {
            ChangeTileUnderObject(selectedObject, null);
            selectedObject.ResetPosition();
        }

        selectedObject = obj;
        selectedObject.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y + yMovingObject, selectedObject.transform.position.z);

        placeBtn.interactable = true;

        Debug.Log("Objet sélectionné : " + obj.name);
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

    private void ChangeTileUnderObject(PlaceableObject obj, TileBase tileType)
    {
        if (obj != null && obj.GetComponent<Renderer>())
        {
            Vector3 minWorld = obj.GetComponent<Renderer>().bounds.min;
            Vector3Int bottomLeftCell = tilemapObjects.WorldToCell(minWorld);
            Vector3Int topRightCell = tilemapObjects.WorldToCell(obj.GetComponent<Renderer>().bounds.max);

            for (int x = bottomLeftCell.x; x <= topRightCell.x; x++)
            {
                for (int y = topRightCell.y; y <= bottomLeftCell.y; y++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                    TileBase tile = tilemapBase.GetTile(cellPos);

                    // Si sur une case valide, on peint
                    if (tile == whiteTile)
                    {
                        tilemapObjects.SetTile(cellPos, tileType);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Can't clean under object because obj is null or obj does not have a renderer component!");
        }
    }

    // Fonction qui vérifie si l'objet peut être placé sur la tilemap
    private bool CanPlaceObjectOnTilemap(PlaceableObject obj)
    {
        if (obj == null) return false; // Sécurité

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return false; // Sécurité

        // Récupère les bornes de l'objet dans l'espace 3D
        Vector3 bottomLeftWorldPos = renderer.bounds.min;
        Vector3 topRightWorldPos = renderer.bounds.max;

        // Convertit les positions du monde en coordonnées de la tilemap
        Vector3Int bottomLeftCell = tilemapBase.WorldToCell(bottomLeftWorldPos);
        Vector3Int topRightCell = tilemapBase.WorldToCell(topRightWorldPos);

        bool isValidPos = true;

        for (int x = bottomLeftCell.x; x <= topRightCell.x; x++)
        {
            for (int y = topRightCell.y; y <= bottomLeftCell.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                TileBase tile = tilemapBase.GetTile(cellPos);

                // Si la case est invalide
                if (tile != whiteTile)
                {
                    isValidPos = false;
                    break;
                }
            }

            if (!isValidPos) break;
        }

        return isValidPos;
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

    private void PlacePlaceableObject(PlaceableObject obj)
    {
        if (obj == null) return; // Sécurité

        float objectHeight = obj.GetComponent<Renderer>().bounds.size.y;
        // TODO: à changer plus tard si toutes les origines des batiments sont en bas !
        float newYPosition = IM.Instance.transform.position.y + (objectHeight / 2f);

        obj.transform.position = new Vector3(obj.transform.position.x, newYPosition, obj.transform.position.z);

        // SET NEW POSITION
        obj.OriginalPosition = obj.transform.position;

        // Réinitialiser
        ChangeTileUnderObject(selectedObject, null);
        selectedObject = null;
        placeBtn.interactable = false;
    }




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

