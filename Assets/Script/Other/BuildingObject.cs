using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingObject : MonoBehaviour, ISaveable<BuildingObject.SavePartData>
{
    GameManager gameManager;
    [SerializeField] public Building building;
    private InhabitantInstance inhabitantUsing;

    [SerializeField] private GameObject canvasBuilding;


    #region Waiting
    bool isUsed = false;

    float timeRemaining = 0f;

    Coroutine waitingCoroutine = null;
    TextMeshProUGUI timeText;



    IEnumerator WaitingCoroutine()
    {
        while (true)
        {
            if (isUsed)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0f)
                {
                    isUsed = false;
                    timeRemaining = 0f;
                    isUsed = false;
                }
                timeText.text = timeRemaining.ToString() + "s";
            }
            yield return null;
        }
    }

    IEnumerator RestartCoroutine()
    {
        yield return null;

        if (isUsed)
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

        if(isUsed)
        {
            TimeSpan elapsedTime = System.DateTime.Now - gameManager.GetLastTimeSaved();
            timeRemaining -= (float)elapsedTime.TotalSeconds;

            waitingCoroutine = StartCoroutine(WaitingCoroutine());
        }
    }

    private void OnMouseDown()
    {
        if (!GM.IM.isEditMode)
        {
            if (!canvasBuilding.activeSelf)
            {
                canvasBuilding.SetActive(true);
                SetupCanvas();
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
        building = data.baseBuilding;
        inhabitantUsing = data.inhabitantUsing;
        isUsed = data.isUsed;
        timeRemaining = data.timeRemaining;

        Debug.Log("Deserialized data: " + building.Name);
    }

    public SavePartData Serialize()
    {
        SavePartData data = new SavePartData();
        inhabitantUsing = data.inhabitantUsing;

        data.baseBuilding = building;
        data.isUsed = isUsed;
        data.timeRemaining = timeRemaining;

        Debug.Log("Serialized data: " + data.baseBuilding.Name);
        return data;
    }

    public class SavePartData : ISaveData
    {
        public Building baseBuilding;
        public InhabitantInstance inhabitantUsing;

        public bool isUsed = false;
        public float timeRemaining = 0f;
    }
    #endregion
}
