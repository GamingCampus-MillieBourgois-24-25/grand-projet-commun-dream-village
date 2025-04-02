using System.Collections;
using System.Collections.Generic;
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
            // Supprime les doublons
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CheckObjectOnTilemap(PlacableObject obj)
    {
        if (obj == null) return; // Sécurité

        Vector3Int cellPosition = tilemapBase.WorldToCell(obj.transform.position);
        TileBase tile = tilemapBase.GetTile(cellPosition);

        if (tile == whiteTile)
        {
            tilemapObjects.SetTile(cellPosition, greenTile);
        }
        else if (tile != greenTile)
        {
            Destroy(obj.gameObject);
        }
    }
}

public static class TC
{
    public static TilemapChecker Instance => TilemapChecker.Instance;
}

