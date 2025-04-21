using System;
using System.Collections;
using TMPro;
using UnityEngine;
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

    float timeRemaining = 0f;

    private GameObject remainingTimeUI;
    Coroutine waitingCoroutine = null;
    TextMeshProUGUI timeText;



    IEnumerator WaitingCoroutine()
    {
        // S'assurer que l'UI est présente
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

        TextMeshProUGUI timeText = remainingTimeUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        canvasBuilding.SetActive(false);

        while (isUsed)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0f)
                {
                    isUsed = false;
                    timeRemaining = 0f;
                    isUsed = false;
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

    //private void Awake()
    //{
    //    SetupCanvas();
    //}

    //private void OnMouseDown()
    //{
    //    ClickOnBuiding();
    //}

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

        isUsed = false;
        waitingCoroutine = null;

        if (inhabitantUsing != null)
        {
            inhabitantUsing.FinishActivity(baseData.AttributeEffects, baseData.Energy, baseData.Mood, baseData.Serenity);
            inhabitantUsing = null;
        }

        // TODO : changer l'exp en fonction du building
        GM.Instance.player.AddXP(100);
        Destroy(remainingTimeUI);
    }


    private void SetupCanvas()
    {
        TextMeshProUGUI name = GM.BM.nameInCanvas.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI timeText = GM.BM.timeInCanvas.GetComponent<TextMeshProUGUI>();
        GameObject preferencesContainer = GM.BM.preferenceContainer;

        name.text = baseData.Name;
        timeText.text = baseData.EffectDuration.ToString() + "s";

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

        //Button button = canvasBuilding.transform.GetChild(3).GetComponent<Button>();
        //button.onClick.RemoveAllListeners();
        //button.onClick.AddListener(() => { DebugSetFirstInhabitant(); });

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
        inhabitantUsing = GM.VM.GetInhabitant(GM.Instance.GetInhabitantByName(data.inhabitantUsingName));
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
