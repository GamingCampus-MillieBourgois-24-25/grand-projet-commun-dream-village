using UnityEngine;

public class PlacableObject : MonoBehaviour
{
    [SerializeField] private bool isMoving = false;

    public bool IsMoving
    {
        get => isMoving;
        set => isMoving = value;
    }

    private void Update()
    {
        if (isMoving)
        {
            TC.Instance.CheckObjectOnTilemap(this);
        }
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
