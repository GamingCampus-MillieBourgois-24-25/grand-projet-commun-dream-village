using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceBtn : MonoBehaviour
{
    private Button placeButton;
    

    void Start()
    {
        placeButton = this.GetComponent<Button>();
        placeButton.interactable = IM.Instance.selectedObject != null;
    }
    public void BS_PlaceActivePlaceableObject()
    {
        PlaceableObject selectedObject = IM.Instance.selectedObject;
        if (selectedObject != null)
        {
            float objectHeight = selectedObject.GetComponent<Renderer>().bounds.size.y;
            float newYPosition = IM.Instance.transform.position.y + (objectHeight / 2f);

            selectedObject.transform.position = new Vector3(selectedObject.transform.position.x, newYPosition, selectedObject.transform.position.z);

            // Désactive le mode de déplacement
            selectedObject.IsMoving = false;

            // Réinitialiser
            IM.Instance.selectedObject = null;
            placeButton.interactable = false;
        }
    }

}
