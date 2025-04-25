using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField]
    private bool canBeStocked = true;

    public bool CanBeStocked
    {
        get { return canBeStocked; }
    }

    private Vector2Int sizeInTiles;
    [HideInInspector]
    public Vector2Int SizeInTiles => sizeInTiles;

    private Vector3Int originalPosition;
    [HideInInspector]
    public Vector3Int OriginalPosition
    {
        get => originalPosition;
        set {
            originalPosition = value;
            //Debug.Log("New Original position: "+originalPosition);
        }
    }

    [HideInInspector]
    public Renderer cachedRenderer;

    private void Awake()
    {
        cachedRenderer = GetComponent<Renderer>();
    }

    public void Start()
    {
        sizeInTiles = GetCellSizeWithBounds();
        CenterObject(GM.IM.tilemapObjects);
        OriginalPosition = GM.IM.tilemapObjects.WorldToCell(transform.position);
    }

    public void ResetPosition()
    {
        Vector3 centerWorld;
        if (sizeInTiles.x % 2 == 0)
        {
            centerWorld = GM.IM.tilemapObjects.CellToWorld(OriginalPosition);
        }
        else
        {
            centerWorld = GM.IM.tilemapObjects.GetCellCenterWorld(OriginalPosition);
        }

        transform.position = GetCenterObject(GM.IM.tilemapObjects, centerWorld);
        Debug.Log("Reset Position: original "+ OriginalPosition +" centerWorld: "+ centerWorld+ " getCenterObject: " + GetCenterObject(GM.IM.tilemapObjects, centerWorld));
    }

    public Vector3 GetSize()
    {
        if (cachedRenderer != null)
        {
            return cachedRenderer.bounds.size;
        }
        return Vector3.one; // Valeur par défaut si pas de Renderer
    }

    public HashSet<Vector3Int> GetOccupiedTiles()
    {
        HashSet<Vector3Int> occupiedTiles = new HashSet<Vector3Int>();

        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) return occupiedTiles;

        foreach (var cellPos in IsoManager.GetCoveredCells(renderer, GM.IM.tilemapObjects))
        {
            occupiedTiles.Add(cellPos);
        }

        //Debug.Log("OccupiedTiles de: "+this.gameObject.name+""+String.Join(",", occupiedTiles));
        return occupiedTiles;
    }


    private Vector2Int GetCellSizeWithBounds()
    {
        Vector3 size = cachedRenderer.bounds.size;
        Vector3 cellSize = GM.IM.tilemapObjects.cellSize;
        return new Vector2Int(
            Mathf.CeilToInt(size.x / cellSize.x),
            Mathf.CeilToInt(size.z / cellSize.y)
        );
    }

    public Vector3 GetCenterObject(Tilemap tilemap, Vector3 centerPoint)
    {
        Vector3 adjustedCenter = new Vector3(centerPoint.x - 0.01f, centerPoint.y, centerPoint.z - 0.01f);
        Vector3Int cell = tilemap.WorldToCell(adjustedCenter);
        Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cell);
        //cellCenterWorld.y = transform.position.y;

        int tilesX = sizeInTiles.x;
        int tilesZ = sizeInTiles.y;

        // Calcul de l'offset pour centrer correctement
        float offsetX = (tilesX % 2 == 0) ? 0.5f * tilemap.cellSize.x : 0f; // 0.5 si pair, 0 si impair
        float offsetZ = (tilesZ % 2 == 0) ? 0.5f * tilemap.cellSize.y : 0f;

        //Debug.Log($"Object centered on tile: Cell {cell}, WorldPos {cellCenterWorld}");

        return new Vector3(cellCenterWorld.x + offsetX, transform.position.y, cellCenterWorld.z + offsetZ);
    }

    public void CenterObject(Tilemap tilemap)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || tilemap == null) return;

        Vector3 boundsCenter = renderer.bounds.center;
        transform.position = GetCenterObject(tilemap, boundsCenter);
    }

    void OnDrawGizmos()
    {
        if (TryGetComponent<Renderer>(out Renderer renderer))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(renderer.bounds.center, renderer.bounds.size);

            Gizmos.DrawSphere(renderer.bounds.min, 0.1f);
            Gizmos.DrawSphere(renderer.bounds.max, 0.1f);
        }
    }
}
