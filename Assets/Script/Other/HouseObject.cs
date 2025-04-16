using UnityEngine;

public class HouseObject : MonoBehaviour
{
    [SerializeField] private Inhabitant inhabitant;
    [SerializeField] private GameObject canvasHabitant;
    private Transform spawnPoint;

    private GameObject instantiatedPrefab;

    private void Start()
    {
        spawnPoint = transform.Find("SpawnPoint");
    }

    private void OnMouseDown()
    {
        if (!GM.IM.isEditMode)
        {
            if (!canvasHabitant.activeSelf)
            {
                canvasHabitant.SetActive(true);
            }

            CharacterJournalManager journalManager = FindObjectOfType<CharacterJournalManager>();
            if (journalManager != null && inhabitant != null)
            {
                journalManager.ShowInhabitantByData(inhabitant);
            }

            if (inhabitant != null && inhabitant.InhabitantPrefab != null && instantiatedPrefab == null)
            {
                if (spawnPoint != null)
                {
                    instantiatedPrefab = Instantiate(
                        inhabitant.InhabitantPrefab,
                        spawnPoint.position,
                        spawnPoint.rotation,
                        spawnPoint
                    );
                    Debug.Log("Hi I just spawned!");
                }
                else
                {
                    Debug.LogWarning($"SpawnPoint not found on {gameObject.name}");
                }
            }
        }
    }

}
