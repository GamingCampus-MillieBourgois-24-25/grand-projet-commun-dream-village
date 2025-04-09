using UnityEngine;
using UnityEngine.AI;

public class InhabitantsBehavior : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] NavMeshSurface surface;

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

        if (Input.GetMouseButtonDown(1))
        {

        }
    }
}
