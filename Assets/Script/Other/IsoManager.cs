using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class IsoManager : MonoBehaviour
{
    public static IsoManager Instance { get; private set; } // Singleton

    [SerializeField] private Tilemap tilemapBase;
    [SerializeField] private Tilemap tilemapObjects;
    [SerializeField] private TileBase whiteTile;
    [SerializeField] private TileBase greenTile;
    [SerializeField] private TileBase redTile;

    [SerializeField] private Canvas editModeCanvas;
    [SerializeField] private Button placeBtn;

    [SerializeField] private float yMovingObject;

    [SerializeField] public PlaceableObject selectedObject;
    [SerializeField] public bool isEditMode = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Supprime les doublons
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (isEditMode)
        {
            // Si on a un objet
            if (selectedObject)
            {
                CheckObjectOnTilemap(selectedObject);
            }

            if (Input.GetMouseButtonDown(0)) // PC
            {
                SelectObjectUnderPointer(Input.mousePosition);
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) // mobile
            {
                SelectObjectUnderPointer(Input.GetTouch(0).position);
            }
        }
    }

    public void CheckObjectOnTilemap(PlaceableObject obj)
    {
        if (obj == null) return; // Sécurité

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return; // Sécurité

        // Récupère les bornes de l'objet dans l'espace 3D
        Vector3 bottomLeftWorldPos = renderer.bounds.min;
        Vector3 topRightWorldPos = renderer.bounds.max;

        //Debug.Log("BottomLEft: "+ bottomLeftWorldPos + " and TOPRIght: "+ topRightWorldPos);

        // Convertit les positions du monde en coordonnées de la tilemap
        Vector3Int bottomLeftCell = tilemapBase.WorldToCell(bottomLeftWorldPos);
        Vector3Int topRightCell = tilemapBase.WorldToCell(topRightWorldPos);

        for (int x = bottomLeftCell.x; x <= topRightCell.x; x++)
        {
            // Oui c'est top avant bottom. Left Right, Top Bottom. Pas changer!
            for (int y = topRightCell.y; y <= bottomLeftCell.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                TileBase tile = tilemapBase.GetTile(cellPos);

                // Si sur une case valide, on peint
                if (tile == whiteTile)
                {
                    tilemapObjects.SetTile(cellPos, greenTile);
                }
                else
                {
                    // Si l'objet est sur une case invalide, il peut pas être placé
                    Destroy(obj.gameObject);
                    return;
                }
            }
        }
    }



    private void SelectObjectUnderPointer(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            PlaceableObject obj = hit.collider.GetComponent<PlaceableObject>();
            if (obj != null)
            {
                OnObjectSelected(obj);
            }
        }
    }

    private void OnObjectSelected(PlaceableObject obj)
    {
        if (obj == null) return; // Sécurité

        // Si un objet est déjà sélectionné, il revient à sa position initiale
        if (selectedObject != null)
        {
            selectedObject.transform.position = selectedObject.OriginalPosition;
        }

        selectedObject = obj;
        selectedObject.transform.position = new Vector3(selectedObject.transform.position.x, yMovingObject, selectedObject.transform.position.z);

        placeBtn.interactable = true;

        Debug.Log("Objet sélectionné : " + obj.name);
    }

    private void PlacePlaceableObject(PlaceableObject obj)
    {
        if (obj != null)
        {
            float objectHeight = obj.GetComponent<Renderer>().bounds.size.y;
            // TODO: à changer plus tard si toutes les origines des bâtiments sont en bas !
            float newYPosition = IM.Instance.transform.position.y + (objectHeight / 2f);

            obj.transform.position = new Vector3(obj.transform.position.x, newYPosition, obj.transform.position.z);

            // Réinitialiser
            selectedObject = null;
            placeBtn.interactable = false;
        }
    }

#region Btns Functions
    public void BS_PlaceSelectedObject()
    {
        PlacePlaceableObject(selectedObject);
    }
    public void BS_ToggleEditMode()
    {
        isEditMode = !isEditMode;
        editModeCanvas.gameObject.SetActive(isEditMode);
        selectedObject = null;
    }
#endregion
}

public static class IM
{
    public static IsoManager Instance => IsoManager.Instance;
}

