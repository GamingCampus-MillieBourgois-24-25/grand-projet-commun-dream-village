using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] public Canvas canvasBuilding;
    [SerializeField] public Canvas remainingTime;

    [SerializeField] public GameObject nameInCanvas;
    [SerializeField] public GameObject preferenceContainer;
    [SerializeField] public GameObject timeInCanvas;

    [SerializeField] private InputActionReference clickAction;

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

    public void BS_DebugSetFirstInhabitant()
    {
        StartActivity(GM.VM.inhabitants[0]);
    }

    private void OnEnable()
    {
        clickAction.action.performed += OnClickPerformed;
        clickAction.action.Enable();
    }

    private void OnDisable()
    {
        clickAction.action.performed -= OnClickPerformed;
        clickAction.action.actionMap.Disable();
    }

    public void OnClickPerformed(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = GM.Instance.GetPointerPosition(context);

        if (GM.Instance.IsPointerOverUIElement(screenPosition) && GM.IM.isEditMode)
        {
            return;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                BuildingObject obj = hit.collider.GetComponent<BuildingObject>();
                if (obj != null)
                {
                    //obj.ClickOnBuiding();
                }
            }
        }
    }
}
