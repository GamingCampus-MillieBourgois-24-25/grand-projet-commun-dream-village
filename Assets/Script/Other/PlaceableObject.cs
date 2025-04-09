using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField]
    private Vector2Int sizeInTiles;
    public Vector2Int SizeInTiles => sizeInTiles;

    private Vector3Int originalPosition;
    public Vector3Int OriginalPosition
    {
        get => originalPosition;
        set => originalPosition = value;
    }

    public Renderer cachedRenderer;

    private void Awake()
    {
        cachedRenderer = GetComponent<Renderer>();
    }

    public void Start()
    {
        sizeInTiles = GetCellSizeWithBounds();
        CenterObject(IM.Instance.tilemapObjects);
        OriginalPosition = IM.Instance.tilemapObjects.WorldToCell(transform.position);
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(OriginalPosition.x, transform.position.y, OriginalPosition.z);
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

        foreach (var cellPos in IsoManager.GetCoveredCells(renderer, IM.Instance.tilemapObjects))
        {
            occupiedTiles.Add(cellPos);
        }

        Debug.Log("OccupiedTiles: "+String.Join(",", occupiedTiles));
        return occupiedTiles;
    }


    private Vector2Int GetCellSizeWithBounds()
    {
        Vector3 size = cachedRenderer.bounds.size;
        Vector3 cellSize = IM.Instance.tilemapObjects.cellSize;
        return new Vector2Int(
            Mathf.CeilToInt(size.x / cellSize.x),
            Mathf.CeilToInt(size.z / cellSize.y)
        );
    }

    public Vector3 GetCenterObject(Tilemap tilemap)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || tilemap == null) return Vector3.zero;

        Vector3 boundsCenter = renderer.bounds.center;
        Vector3 adjustedCenter = new Vector3(boundsCenter.x - 0.01f, boundsCenter.y, boundsCenter.z);

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
        transform.position = GetCenterObject(tilemap);
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
