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
        OriginalPosition = transform.position;
        //CenterObject(IM.Instance.tilemapObjects);
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

        // Récupérer les dimensions en X et Z
        Vector3 size = renderer.bounds.size;

        // Calculer combien de tiles l'objet occupe
        int tilesX = Mathf.CeilToInt(size.x / tilemap.cellSize.x);
        int tilesZ = Mathf.CeilToInt(size.z / tilemap.cellSize.y); // y dans Tilemap = z en 3D

        // Convertir minWorld en coordonnées de tilemap
        Vector3 minWorld = renderer.bounds.min;
        Vector3Int minCell = tilemap.WorldToCell(minWorld);

        // Calcul de l'offset pour centrer correctement
        float offsetX = (tilesX % 2 == 0) ? 0.5f : 0f; // 0.5 si pair, 0 si impair
        float offsetZ = (tilesZ % 2 == 0) ? 0.5f : 0f;

        // Trouver la position centrale des tiles occupées
        Vector3 centerCell = new Vector3(
            minCell.x + (tilesX / 2f) - offsetX,
            minCell.y + (tilesZ / 2f) - offsetZ,
            0
        );

        // Convertir en position monde et ajuster
        Vector3 centeredWorldPosition = tilemap.GetCellCenterWorld(Vector3Int.FloorToInt(centerCell));
        transform.position = new Vector3(centeredWorldPosition.x, transform.position.y, centeredWorldPosition.z);

        Debug.Log($"Objet centré à : {transform.position}, Taille : {tilesX}x{tilesZ} tiles, CenterCell {centerCell}");
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
