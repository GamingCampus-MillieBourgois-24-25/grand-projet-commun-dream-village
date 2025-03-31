using UnityEngine;

public class GridRayDebugger : MonoBehaviour
{
    private Camera mainCamera;
    private Grid grid;

    void Start()
    {
        mainCamera = Camera.main;
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        if (mainCamera == null || grid == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Dessine un Debug Ray dans la sc�ne
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);

            // Convertit la position du hit en coordonn�es Grid
            Vector3Int gridPosition = grid.WorldToCell(hit.point);

            Debug.Log($"Cellule touch�e : {gridPosition}");
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow);
        }
    }
}
