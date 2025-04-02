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
            // Logique lorsqu'il bouge
            TC.Instance.CheckObjectOnTilemap(this);
        }
    }
}
