using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapChecker : MonoBehaviour
{
    public static TilemapChecker Instance { get; private set; } // Singleton

    [SerializeField] private Tilemap tilemapBase;
    [SerializeField] private Tilemap tilemapObjects;
    [SerializeField] private TileBase whiteTile;
    [SerializeField] private TileBase greenTile;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Supprime les doublons
            return;
        }

        Instance = this;
    }

    public void CheckObjectOnTilemap(PlacableObject obj)
    {
        if (obj == null) return; // Sécurité

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return; // Sécurité

        // Récupère les bornes de l'objet dans l'espace 3D
        Vector3 bottomLeftWorldPos = renderer.bounds.min;
        Vector3 topRightWorldPos = renderer.bounds.max;

        Debug.Log("BottomLEft: "+ bottomLeftWorldPos + " and TOPRIght: "+ topRightWorldPos);

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




}

public static class TC
{
    public static TilemapChecker Instance => TilemapChecker.Instance;
}

