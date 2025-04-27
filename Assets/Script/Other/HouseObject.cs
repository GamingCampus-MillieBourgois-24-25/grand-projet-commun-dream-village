using UnityEngine;
using UnityEngine.AI;

public class HouseObject : MonoBehaviour
{
    private GameObject instantiatedPrefab;

    public InhabitantInstance inhabitantInstance;
    public Transform spawnPoint;

    public float wanderRadius = 2f;
    public float wanderTimer = 5f;

    private float timer;

    private void Start()
    {
        spawnPoint = transform.Find("SpawnPoint");
        if (!inhabitantInstance.isInActivity)
        {
            inhabitantInstance.inhabitantObject = Instantiate(inhabitantInstance.baseData.InhabitantPrefab, spawnPoint.position, spawnPoint.rotation, GM.Instance.playerIslandObject);
            inhabitantInstance.agent = inhabitantInstance.inhabitantObject.GetComponent<NavMeshAgent>();
            timer = wanderTimer;
        }
    }

    void Update()
    {
        if (inhabitantInstance.inhabitantObject != null && !inhabitantInstance.isInActivity)
        {
            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(inhabitantInstance.inhabitantObject.transform.position, wanderRadius, -1);
                Debug.Log(inhabitantInstance.inhabitantObject);
                Debug.Log(inhabitantInstance.Name);
                inhabitantInstance.agent.SetDestination(newPos);
                timer = 0;
            }
        }
    }

    //private void OnMouseDown()
    //{
    //    OpenInhabitantJournal();
    //}

    public void OpenInhabitantJournal()
    {
        if (!GM.IM.isEditMode)
        {

            if (!GM.Tm.isHouseTutorialAlreadyPlayed)
            {
                GM.Instance.OnHouseTuto?.Invoke();
                Debug.Log("House Clicked");
                GM.Instance.tutorialsManager.skipDialogue = true;
            }

            if (/*GM.Tm.inHeartTutorial*/ GM.Tm.currentTutorialType == Dialogues.TutorialType.Heart)
            {
                GM.Tm.UnHold(55);
            }
            
            if (!GM.Cjm.journalCanvas.activeSelf)
            {
                GM.Cjm.journalCanvas.SetActive(true);
            }

            if (GM.Cjm != null && inhabitantInstance != null)
            {
                GM.JournalPanel.SetActive(false);
                GM.ShopPanel.SetActive(false);
                GM.InventoryPanel.SetActive(false);
                GM.DayNightPanel.SetActive(false);
                
                GM.Cjm.nextButton.gameObject.SetActive(false);
                GM.Cjm.previousButton.gameObject.SetActive(false);

                GM.Cjm.ShowInhabitantByData(inhabitantInstance);
            }

            if (inhabitantInstance != null && inhabitantInstance.baseData.InhabitantPrefab != null && instantiatedPrefab == null)
            {
                if (spawnPoint != null)
                {
                    //instantiatedPrefab = Instantiate(
                    //    inhabitant.InhabitantPrefab,
                    //    spawnPoint.position,
                    //    spawnPoint.rotation,
                    //    spawnPoint
                    //);
                    Debug.Log("Hi I just spawned!");
                }
                else
                {
                    Debug.LogWarning($"SpawnPoint not found on {gameObject.name}");
                }
            }
        }
    }

    #region InhabitantFreeMovement

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 finalPosition = origin;
        for (int i = 0; i < 30; i++) // On essaie plusieurs fois
        {
            Vector3 randomDirection = Random.insideUnitSphere * dist;
            randomDirection += origin;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, dist, layermask))
            {
                finalPosition = hit.position;
                break; 
            }
        }
        return finalPosition;
    }



    #endregion

}
