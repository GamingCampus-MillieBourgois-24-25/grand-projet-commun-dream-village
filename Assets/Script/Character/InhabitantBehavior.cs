using UnityEngine;
using UnityEngine.AI;

public class InhabitantBehavior : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Unity.AI.Navigation.NavMeshSurface surface;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void GoTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GoTo(hit.point);
            }
        }
    }
}
