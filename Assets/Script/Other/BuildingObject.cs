using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.UI;

public class BuildingObject : MonoBehaviour, ISaveable<BuildingObject.SavePartData>
{
    GameManager gameManager;
    [SerializeField] public Building building;
    private InhabitantInstance inhabitantUsing;

    public GameObject canvasBuilding;


    #region Waiting
    bool isUsed = false;

    float timeRemaining = 0f;

    Coroutine waitingCoroutine = null;
    TextMeshProUGUI timeText;



    IEnumerator WaitingCoroutine()
    {
            while (isUsed)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0f)
                {
                    isUsed = false;
                    timeRemaining = 0f;
                    isUsed = false;
                }
                timeText.text = timeRemaining.ToString() + "s";

                yield return null;
            }
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

        if(isUsed && waitingCoroutine == null)
        {
            TimeSpan elapsedTime = System.DateTime.Now - gameManager.GetLastTimeSaved();
            timeRemaining -= (float)elapsedTime.TotalSeconds;

            waitingCoroutine = StartCoroutine(WaitingCoroutine());
        }
    }

    private void Awake()
    {
        SetupCanvas();
    }

    private void OnMouseDown()
    {
        if (!GM.IM.isEditMode)
        {
            if (!canvasBuilding.activeSelf)
            {
                canvasBuilding.SetActive(true);
            }
        }
    }



    public void StartActivity(InhabitantInstance _inhabitant)
    {
        if (waitingCoroutine == null)
        {
            isUsed = true;
            timeRemaining = building.EffectDuration;
            inhabitantUsing = _inhabitant;
            waitingCoroutine = StartCoroutine(WaitingCoroutine());

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
            inhabitantUsing.FinishActivity(building.AttributeEffects, building.Energy, building.Mood, building.Serenity);
            inhabitantUsing = null;
        }
    }


    private void SetupCanvas()
    {
        TextMeshProUGUI name = canvasBuilding.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI description = canvasBuilding.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        timeText = canvasBuilding.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        name.text = building.Name;
        description.text = building.Description;
        timeText.text = building.EffectDuration.ToString() + "s";



        Button button = canvasBuilding.transform.GetChild(3).GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { DebugSetFirstInhabitant(); });
    }
    private void DebugSetFirstInhabitant()
    {
        StartActivity(GM.VM.inhabitants[0]);
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

        GetComponent<PlaceableObject>().OriginalPosition = data.originalPosition;
        GetComponent<PlaceableObject>().ResetPosition();
    }

    public SavePartData Serialize()
    {
        SavePartData data = new SavePartData();
        data.baseBuildingName = building.Name;
        data.inhabitantUsingName = (inhabitantUsing != null) ? inhabitantUsing.FirstName : null;

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
