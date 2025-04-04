using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceableObject : MonoBehaviour
{
    private Vector3 originalPosition;
    public Vector3 OriginalPosition
    {
        get => originalPosition;
        set => originalPosition = value;
    }

    public void Start()
    {
        CenterObject(IM.Instance.tilemapObjects);
        OriginalPosition = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = OriginalPosition;
    }

    public Vector3 GetSize()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size;
        }
        return Vector3.one; // Valeur par défaut si pas de Renderer
    }

    public void CenterObject(Tilemap tilemap)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || tilemap == null) return;

        Vector3 boundsCenter = renderer.bounds.center;
        Vector3Int cell = tilemap.WorldToCell(boundsCenter);
        Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cell);
        //cellCenterWorld.y = transform.position.y;

        // Calculer combien de tiles l'objet occupe
        Vector3 size = renderer.bounds.size;
        int tilesX = Mathf.CeilToInt(size.x / tilemap.cellSize.x);
        int tilesZ = Mathf.CeilToInt(size.z / tilemap.cellSize.y);

        // Calcul de l'offset pour centrer correctement
        float offsetX = (tilesX % 2 == 0) ? 0.5f * tilemap.cellSize.x : 0f; // 0.5 si pair, 0 si impair
        float offsetZ = (tilesZ % 2 == 0) ? 0.5f * tilemap.cellSize.y : 0f;

        transform.position = new Vector3(cellCenterWorld.x + offsetX, transform.position.y, cellCenterWorld.z + offsetZ);

        Debug.Log($"Object centered on tile: Cell {cell}, WorldPos {cellCenterWorld}");
    }

    //public void CenterObject(Tilemap tilemap)
    //{
    //Vector3Int minCellCoords = tilemap.WorldToCell(renderer.bounds.min);

    //// Calculer combien de tiles l'objet occupe
    //Vector3 size = renderer.bounds.size;
    //int tilesX = Mathf.CeilToInt(size.x / tilemap.cellSize.x);
    //int tilesZ = Mathf.CeilToInt(size.z / tilemap.cellSize.y);

    //// Tile en bas à gauche
    //Vector3Int minCell = tilemap.WorldToCell(renderer.bounds.min);
    //Vector3 minCellWorld = tilemap.CellToWorld(minCell);

    //// Calcul de l'offset pour centrer correctement
    //float offsetX = (tilesX % 2 == 0) ? 0f : 0.5f; // 0 si pair, 0.5 si impair
    //float offsetZ = (tilesZ % 2 == 0) ? 0f : 0.5f;

    //// Trouver la position centrale des tiles occupées
    //Vector3 centeredWorldPosition = new Vector3(
    //    minCellWorld.x + (tilesX / 2f - offsetX) * tilemap.cellSize.x,
    //    transform.position.y,
    //    minCellWorld.y + (tilesZ / 2f - offsetZ) * tilemap.cellSize.y
    //);

    //// Convertir en position monde et ajuster
    ////Vector3 centeredWorldPosition = tilemap.GetCellCenterWorld(Vector3Int.FloorToInt(centerCell));
    //transform.position = centeredWorldPosition;

    //Debug.Log($"minCellWorld : {minCellWorld.x}, TilesX/2 : {(tilesX / 2f)}, OffestX: {offsetX} Taille : {tilesX}x{tilesZ} tiles, CenterCell {centeredWorldPosition}");
//}



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
