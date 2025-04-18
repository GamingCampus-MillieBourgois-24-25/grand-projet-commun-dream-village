using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] public Canvas canvasBuilding;
    [SerializeField] public Canvas remainingTime;

    [SerializeField] public GameObject nameInCanvas;
    [SerializeField] public GameObject preferenceContainer;
    [SerializeField] public GameObject timeInCanvas;


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
}
