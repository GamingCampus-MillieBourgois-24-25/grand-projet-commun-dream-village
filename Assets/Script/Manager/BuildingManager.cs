using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    [Header("Building")]
    [SerializeField] public Canvas canvasBuilding;
    [SerializeField] public Canvas remainingTime;

    [SerializeField] public GameObject nameInCanvas;
    [SerializeField] public GameObject preferenceContainer;
    [SerializeField] public GameObject timeInCanvas;

    [Header("Selection")]
    [SerializeField] public Canvas selectONEInhabitant;
    [SerializeField] public GameObject inhabitantsContainer;
    [SerializeField] public GameObject selectInhabitantPrefab;
    private InhabitantInstance selectedInhabitant;
    private GameObject selectedButton;

    //[Header("Other")]
    //[SerializeField] private InputActionReference clickAction;


    #region Unity Functions
    //private void OnEnable()
    //{
    //    clickAction.action.performed += OnClickPerformed;
    //    clickAction.action.Enable();
    //}

    //private void OnDisable()
    //{
    //    clickAction.action.performed -= OnClickPerformed;
    //    clickAction.action.actionMap.Disable();
    //}

    #endregion

    public void StartActivity(InhabitantInstance _inhabitant)
    {
        GameObject parent = canvasBuilding.transform.parent.gameObject;
        BuildingObject buildingObject = parent.GetComponent<BuildingObject>();

        if (parent != null && buildingObject != null) { 
            buildingObject.StartActivityInBuilding(_inhabitant);
        } 
        else
        {
            Debug.LogError("Error : Canvas' parent is not a BuildingObject!");
        }
    }

    public void UpdateSelectionCanvas()
    {
        // TODO: optimiser

        foreach (Transform child in inhabitantsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var inhabitant in GM.VM.inhabitants)
        {
            GameObject go = Instantiate(selectInhabitantPrefab, inhabitantsContainer.transform);

            Sprite sprite = go.transform.GetChild(0).GetChild(0).GetComponent<Sprite>();
            sprite = inhabitant.Icon;

            Button btn = go.GetComponent<Button>();
            if (btn != null)
            {
                InhabitantInstance capturedInhabitant = inhabitant;
                btn.onClick.AddListener(() => BS_SelectInhabitant(btn, capturedInhabitant));

                WorkingButton(btn.gameObject, inhabitant.isInActivity);
            }
        }
    }

    #region Buttons
    public void BS_DebugSetFirstInhabitant()
    {
        StartActivity(GM.VM.inhabitants[0]);
    }

    public void BS_OpenSelectionCanvas()
    {
        UpdateSelectionCanvas();

        DisableButton(selectedButton, true);
        selectedInhabitant = null;
        selectedButton = null;

        selectONEInhabitant.gameObject.SetActive(true);
    }
    public void BS_ChangeSelectedButton(Button button)
    {
        if (button != null)
        {
            if (selectedButton != null)
            {
                DisableButton(selectedButton, true);
            }

            selectedButton = button.gameObject;
            DisableButton(selectedButton, false);
        }
    }

    public void BS_SelectInhabitant(Button button, InhabitantInstance inhabitant)
    {
        if (!inhabitant.isInActivity && inhabitant != selectedInhabitant)
        {
            selectedInhabitant = inhabitant;
            BS_ChangeSelectedButton(button);
            Debug.Log("Selected: " + inhabitant.Name);
        }
    }

    public void BS_SendSelectedInhabitant()
    {
        Debug.Log("VA MOUSSAILLON");
        if (selectedInhabitant != null)
        {
            StartActivity(selectedInhabitant);
            selectONEInhabitant.gameObject.SetActive(false);
        }
    }

    private void DisableButton(GameObject button, bool disable)
    {
        if (button != null) button.transform.GetChild(3).gameObject.SetActive(disable);
    }

    private void WorkingButton(GameObject button, bool working)
    {
        if (button != null) button.transform.GetChild(2).gameObject.SetActive(working);
    }
    #endregion




    //public void OnClickPerformed(InputAction.CallbackContext context)
    //{
    //    Vector2 screenPosition = GM.Instance.GetPointerPosition(context);

    //    if (GM.Instance.IsPointerOverUIElement(screenPosition) && GM.IM.isEditMode)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
    //        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
    //        {
    //            BuildingObject obj = hit.collider.GetComponent<BuildingObject>();
    //            if (obj != null)
    //            {
    //                //obj.ClickOnBuiding();
    //            }
    //        }
    //    }
    //}
}
