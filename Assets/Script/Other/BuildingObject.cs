using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BuildingObject : MonoBehaviour, ISaveable<BuildingObject.SavePartData>
{
    GameManager gameManager;
    [SerializeField] public Building baseData;

    private InhabitantInstance inhabitantUsing;
    private GameObject canvasBuilding;

    #region Waiting
    private bool isUsed = false;
    public bool IsUsed => isUsed;

    public float timeRemaining = 0.0f;
    int notificationID = -1;

    private GameObject remainingTimeUI;
    Coroutine waitingCoroutine = null;
    TextMeshProUGUI timeText;
    private TextMeshProUGUI starText;


    IEnumerator WaitingCoroutine()
    {
        CheckAndInstanciateRemainingTime();

        int lastWholeMinutes = Mathf.CeilToInt(timeRemaining / 60f);
        UpdateSkipText(lastWholeMinutes);
        AddBSSkipFunctions();


        if (notificationID == -1 && inhabitantUsing != null && inhabitantUsing.baseData.Name != null)
        {
            string title = inhabitantUsing.baseData.Name + " has finished " + inhabitantUsing.baseData.GetPronouns()[5] + " activity!";
            string text = "Come back to see what " + inhabitantUsing.baseData.GetPronouns()[3] + (inhabitantUsing.baseData.isPlural() ? " are" : " is") + " doing!";
            notificationID = NotificationManager.CreateNotification(title, text, timeRemaining);
        }

        while (isUsed)
        {
            timeRemaining -= Time.deltaTime;

            int currentWholeMinutes = Mathf.CeilToInt(timeRemaining / 60f);
            if (GM.Instance.skipWithStarButton.gameObject.activeSelf)
            {
                if (currentWholeMinutes != lastWholeMinutes)
                {
                    lastWholeMinutes = currentWholeMinutes;
                    UpdateSkipText(currentWholeMinutes);
                }
            }

            if (timeRemaining <= 0f)
            {
                isUsed = false;
                timeRemaining = 0f;
            }

            timeText.text = GM.Instance.DisplayFormattedTime(timeRemaining);
            yield return null;
        }

        FinishActivity();
    }


    IEnumerator RestartCoroutine()
    {
        yield return null;

        if (isUsed && waitingCoroutine == null)
        {
            TimeSpan elapsedTime = System.DateTime.Now - gameManager.GetLastTimeSaved();
            timeRemaining -= (float)elapsedTime.TotalSeconds;

            waitingCoroutine = StartCoroutine(WaitingCoroutine());
        }
    }

    private IEnumerator WaitingForInhabitantInDestination()
    {
        while (inhabitantUsing.agent.pathPending)
        {
            yield return null;
        }

        while (inhabitantUsing.agent.remainingDistance > 0.1 || inhabitantUsing.agent.velocity.sqrMagnitude > 0f)
        {
            yield return null;
        }
        Destroy(inhabitantUsing.inhabitantObject);
    }

    #endregion

    private void Start()
    {
        gameManager = GameManager.instance;

        canvasBuilding = GM.BM.canvasBuilding.gameObject;

        if (isUsed && waitingCoroutine == null)
        {
            TimeSpan elapsedTime = System.DateTime.Now - gameManager.GetLastTimeSaved();
            timeRemaining -= (float)elapsedTime.TotalSeconds;

            waitingCoroutine = StartCoroutine(WaitingCoroutine());
        }
    }

    public void ClickOnBuiding()
    {
        //Debug.Log("JE CLICK SUR LE BUILDING");
        if (!GM.IM.isEditMode)
        {
            if ((!canvasBuilding.transform.IsChildOf(transform) || !canvasBuilding.activeSelf) && !isUsed)
            {
                SetupCanvas();
                canvasBuilding.SetActive(true);
            }
            
            if (/*GM.Tm.inActivityTutorial*/ GM.Tm.currentTutorialType == Dialogues.TutorialType.Activity)
            {
                GM.Tm.UnHold(31);
            }
        }
    }

    private void CheckAndInstanciateRemainingTime()
    {
        // S'assurer que l'UI est pr�sente
        Transform existing = transform.Find("remainingTime");
        if (existing != null)
        {
            remainingTimeUI = existing.gameObject;
            remainingTimeUI.SetActive(true);
        }
        else
        {
            remainingTimeUI = Instantiate(GM.BM.remainingTime.gameObject, transform); // GM.BM.remainingTime est le prefab
            remainingTimeUI.name = "remainingTime";
            remainingTimeUI.SetActive(true);
        }

        remainingTimeUI.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = inhabitantUsing.baseData.Icon;
        timeText = remainingTimeUI.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        Debug.Log(timeText);
        canvasBuilding.SetActive(false);
    }

    private void UpdateSkipText(int remainingMinutes)
    {
        if (starText == null)
        {
            starText = GM.Instance.skipWithStarButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        }
        if (starText == null) return;

        starText.text = remainingMinutes.ToString();

        UpdateStarBTNInteractable(remainingMinutes);
    }

    private void UpdateStarBTNInteractable(int stars)
    {
        Button starButton = GM.Instance.skipWithStarButton;
        if (GM.Instance.player.CanSpendStar(stars))
        {
            starButton.interactable = true;
        }
        else
        {
            starButton.interactable = false;
        }
    }

    //private void UpdateSkipText(int remainingMinutes)
    //{
    //    if (starText == null && remainingTimeUI != null)
    //    {
    //        Transform starTransform = remainingTimeUI.transform.GetChild(0).GetChild(2).GetChild(2).GetChild(0);

    //        if (starTransform == null) return;

    //        starText = starTransform.GetComponent<TextMeshProUGUI>();

    //    }
    //    if (starText == null) return;

    //    starText.text = remainingMinutes.ToString();
    //}
    private void AddBSSkipFunctions()
    {
        Button starButton = GM.Instance.skipWithStarButton;

        if (starButton != null && starText != null)
        {
            starButton.onClick.RemoveAllListeners();
            starButton.onClick.AddListener(() =>
            {
                GM.Instance.TrySkipActivityWithStars(starText, this, true);
            });
        }

        Button adButton = GM.Instance.skipWithAdButton;

        if (adButton != null)
        {
            adButton.onClick.RemoveAllListeners();
            adButton.onClick.AddListener(() =>
            {
                GM.Instance.SkipActivityWithADS(this, true);
            });
        }
    }

    public void StartActivityInBuilding(InhabitantInstance _inhabitant)
    {
        if (waitingCoroutine == null && _inhabitant.isInActivity == false)
        {
            isUsed = true;
            timeRemaining = baseData.EffectDuration;
            inhabitantUsing = _inhabitant;
            waitingCoroutine = StartCoroutine(WaitingCoroutine());

            if (inhabitantUsing.inhabitantObject == null)
            {
                Transform spawnPoint = inhabitantUsing.houseObject.spawnPoint;
                inhabitantUsing.inhabitantObject = Instantiate(inhabitantUsing.baseData.InhabitantPrefab, spawnPoint.transform.position, spawnPoint.rotation, GM.Instance.playerIslandObject);
                inhabitantUsing.agent = inhabitantUsing.inhabitantObject.GetComponent<NavMeshAgent>();
            }
            inhabitantUsing.agent.SetDestination(gameObject.transform.Find("ArrivalPoint").position);
            StartCoroutine(WaitingForInhabitantInDestination());

            _inhabitant.isInActivity = true;

            gameManager.SaveGame();
        }
    }

    public void FinishActivity()
    {
        if (waitingCoroutine != null)
        {
            StopCoroutine(waitingCoroutine);
            timeRemaining = 0f;
        }

        if (remainingTimeUI != null)
        {
            Destroy(remainingTimeUI.gameObject);
        }

        isUsed = false;
        waitingCoroutine = null;

        if (inhabitantUsing != null)
        {
            Debug.Log("inhabitant just finished an activity! "+ inhabitantUsing.baseData.Name);
            inhabitantUsing.FinishActivity(baseData.AttributeEffects, baseData.Energy, baseData.Mood, baseData.Serenity);

            Transform spawnPoint = gameObject.transform.Find("ArrivalPoint");
            inhabitantUsing.inhabitantObject = Instantiate(inhabitantUsing.baseData.InhabitantPrefab, spawnPoint.position, spawnPoint.rotation, GM.Instance.playerIslandObject);
            inhabitantUsing.agent = inhabitantUsing.inhabitantObject.GetComponent<NavMeshAgent>();

            inhabitantUsing = null;
        }

        // TODO : changer l'exp en fonction du building
        GM.Instance.player.AddXP(baseData.Experience);
        Destroy(remainingTimeUI);

        GM.VM.Save("VillageManager");

        if (notificationID != -1)
        {
            NotificationManager.CancelNotification(notificationID);
            notificationID = -1;
        }
    }


    private void SetupCanvas()
    {
        TextMeshProUGUI name = GM.BM.nameInCanvas.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI timeText = GM.BM.timeInCanvas.GetComponent<TextMeshProUGUI>();
        GameObject preferencesContainer = GM.BM.preferenceContainer;

        name.text = baseData.Name;
        timeText.text = GM.Instance.DisplayFormattedTime(baseData.EffectDuration);

        foreach (Transform child in preferencesContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var attributeEffect in baseData.AttributeEffects)
        {
            GameObject iconGO = new GameObject("AttributeIcon");
            iconGO.transform.SetParent(preferencesContainer.transform, false);

            Image image = iconGO.AddComponent<Image>();
            image.sprite = attributeEffect.attribute.icon;
        }

        Button button = canvasBuilding.transform.GetChild(4).GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            if (/*GM.Tm.inActivityTutorial*/ GM.Tm.currentTutorialType == Dialogues.TutorialType.Activity)
            {
                GM.Tm.UnHold(26);
            }
        });

        canvasBuilding.transform.SetParent(transform, true);
        canvasBuilding.transform.position = this.transform.position + new Vector3(0, canvasBuilding.transform.position.y, 0);

    }

    #region Check Game closed
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            if(waitingCoroutine != null)
            {
                StopCoroutine(waitingCoroutine);
                waitingCoroutine = null;
            }
        }
        else
        {
            StartCoroutine(RestartCoroutine());
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (waitingCoroutine != null)
            {
                StopCoroutine(waitingCoroutine);
                waitingCoroutine = null;
            }
        }
        else
        {
            StartCoroutine(RestartCoroutine());
        }
    }

    private void OnApplicationQuit()
    {
        if (waitingCoroutine != null)
        {
            StopCoroutine(waitingCoroutine);
            waitingCoroutine = null;
        }
    }
    #endregion

    #region Save
    public void Deserialize(SavePartData data)
    {
        if (data.inhabitantUsingName != null) inhabitantUsing = GM.VM.GetInhabitant(GM.Instance.GetInhabitantByName(data.inhabitantUsingName));
        isUsed = data.isUsed;
        timeRemaining = data.timeRemaining;

        if(inhabitantUsing != null) 
            inhabitantUsing.isInActivity = true;

        GetComponent<PlaceableObject>().OriginalPosition = data.originalPosition;
        GetComponent<PlaceableObject>().ResetPosition();
    }

    public SavePartData Serialize()
    {
        SavePartData data = new SavePartData();
        data.baseBuildingName = baseData.Name;
        data.inhabitantUsingName = (inhabitantUsing != null) ? inhabitantUsing.Name : null;

        data.isUsed = isUsed;
        data.timeRemaining = timeRemaining;

        data.originalPosition = GetComponent<PlaceableObject>().OriginalPosition;
        return data;
    }

    [System.Serializable]
    public class SavePartData : ISaveData
    {
        public string baseBuildingName;
        public string inhabitantUsingName;

        public bool isUsed = false;
        public float timeRemaining = 0f;

        public Vector3Int originalPosition;
    }
    #endregion
}
