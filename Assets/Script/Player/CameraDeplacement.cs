using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class CameraDeplacement : MonoBehaviour
{
    [SerializeField] float maxZoom = 7f;
    [SerializeField] float minZoom = 1f;
    [SerializeField] float zoomSpeed = 0.1f;
    [SerializeField] float moveSpeed = 0.1f;   

    [Header("Input Actions")]
    [SerializeField] private InputActionReference zoom1Action; 
    [SerializeField] private InputActionReference zoom2Action;
    [SerializeField] private InputActionReference touch1Action;
    [SerializeField] private InputActionReference touch2Action;

    [SerializeField] private InputActionReference MoveAction;


    [Header("Coords")]
    [SerializeField] private Vector2 centerCamera;
    [SerializeField] private Vector2 limCam;
    [SerializeField] private Vector2 outsideSpeedDeplacementMax;
    [SerializeField] private Vector2 lambdaDeplacement;
    private Vector2 distanceBords;
    Vector2 screenSize;

    private float prevMagnitude = 0f;
    private int touchCount = 0;
    IsoManager isoManager;
    Coroutine cameraMovementCoroutine = null;
    float actualZoom;

    void Start()
    {
        isoManager = IsoManager.Instance;
        screenSize = new Vector2(Screen.width, Screen.height);
        distanceBords = new Vector2(screenSize.x * 0.15f, screenSize.y * 0.15f);

        // Mouse scroll wheel input
        InputAction scrollAction = new InputAction("Scroll", binding: "<Mouse>/scroll");
        scrollAction.Enable();
        scrollAction.performed += ctx => CameraZoom(ctx.ReadValue<Vector2>().y * zoomSpeed);

        // Zoom
        touch1Action.action.Enable();
        touch2Action.action.Enable();
        touch1Action.action.performed += ctx => touchCount++;
        touch2Action.action.performed += ctx => touchCount++;
        touch1Action.action.canceled += ctx => RemoveTouch();
        touch2Action.action.canceled += ctx => RemoveTouch();

        zoom1Action.action.Enable();
        zoom2Action.action.Enable();
        zoom2Action.action.performed += ctx => OnPinch();

        // Deplacement
        MoveAction.action.Enable();
        touch1Action.action.performed += ctx => MoveAction.action.performed += ctx => CameraMovement(ctx.ReadValue<Vector2>() * moveSpeed);
        touch1Action.action.canceled += ctx => MoveAction.action.performed -= ctx => CameraMovement(ctx.ReadValue<Vector2>() * moveSpeed);

        // Deplacement in edit 
        touch1Action.action.performed +=  ctx => CameraMovementEdit();
        touch1Action.action.canceled += ctx => CameraMovementEdit();

        actualZoom = Camera.main.orthographicSize;
    }




    private void OnPinch()
    {
        Vector3 finger1 = zoom1Action.action.ReadValue<Vector2>();
        Vector3 finger2 = zoom2Action.action.ReadValue<Vector2>(); 
        float magnitude = (finger1 - finger2).magnitude;
        if (prevMagnitude == 0f)
        {
            prevMagnitude = magnitude;
        }

        float dif = magnitude - prevMagnitude;
        prevMagnitude = magnitude;

        CameraZoom(-dif * zoomSpeed);
    }

    void RemoveTouch()
    {
        if (touchCount > 2)
            return;

        touchCount--;
        prevMagnitude = 0f;
        if (touchCount <= 0)
        {
            touchCount = 0;
        }
    }

    private void CameraZoom(float incr)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + incr, minZoom, maxZoom);
        actualZoom = Camera.main.orthographicSize;
    }

    private void CameraMovement(Vector2 movement, bool isEdit = false)
    {
        if (isoManager.IsEditMode() && !isEdit)
            return;

        Vector3 newPosition = Camera.main.transform.localPosition;

        newPosition.x -= movement.x / lambdaDeplacement.x / (6/actualZoom);
        //newPosition.y += movement.x/* / lambdaDeplacement.x*/;


        //newPosition.x -= movement.y/* * lambdaDeplacement.y*/;
        newPosition.y -= movement.y * lambdaDeplacement.y / (6/actualZoom);


        if(newPosition.x < centerCamera.x - limCam.x)
            newPosition.x = centerCamera.x - limCam.x;
        else if (newPosition.x > centerCamera.x + limCam.x)
            newPosition.x = centerCamera.x + limCam.x;

        if (newPosition.y < centerCamera.y - limCam.y)
            newPosition.y = centerCamera.y - limCam.y;
        else if (newPosition.y > centerCamera.y + limCam.y)
            newPosition.y = centerCamera.y + limCam.y;


        Camera.main.transform.localPosition = newPosition;
    }

    void CameraMovementEdit()
    {
        if(cameraMovementCoroutine != null)
        {
            StopCoroutine(cameraMovementCoroutine);
            cameraMovementCoroutine = null;
        }
        else
        {
            cameraMovementCoroutine = StartCoroutine(CameraDeplacementEditCorout());
        }
    }

    IEnumerator CameraDeplacementEditCorout()
    {
        if (isoManager.IsEditMode())
        {

            while (touch1Action.action.IsPressed())
            {
                Vector2 movement = Vector2.zero;
                // Vérifier si le toucher est sur l'UI

                Vector2 posTouch = zoom1Action.action.ReadValue<Vector2>();

                if (posTouch.x < distanceBords.x)
                {
                    // Normaliser posTouch.x entre 0 et distanceBords.x
                    float normalizedValue = Mathf.InverseLerp(0, distanceBords.x, posTouch.x);

                    // Interpoler la vitesse entre 0 et outsideSpeedDeplacementMax.x
                    movement.x = Mathf.Lerp(outsideSpeedDeplacementMax.x, 0, normalizedValue);
                }

                else if (posTouch.x > screenSize.x - distanceBords.x)
                {
                    // Normaliser posTouch.x entre 0 et distanceBords.x
                    float normalizedValue = Mathf.InverseLerp(screenSize.x - distanceBords.x, screenSize.x, posTouch.x);
                    // Interpoler la vitesse entre 0 et outsideSpeedDeplacementMax.x
                    movement.x = Mathf.Lerp(0, -outsideSpeedDeplacementMax.x, normalizedValue);
                }

                if (posTouch.y < distanceBords.y)
                {
                    // Normaliser posTouch.y entre 0 et distanceBords.y
                    float normalizedValue = Mathf.InverseLerp(0, distanceBords.y, posTouch.y);
                    // Interpoler la vitesse entre 0 et outsideSpeedDeplacementMax.y
                    movement.y = Mathf.Lerp(outsideSpeedDeplacementMax.y, 0, normalizedValue);
                }

                else if (posTouch.y > screenSize.y - distanceBords.y)
                {
                    // Normaliser posTouch.y entre 0 et distanceBords.y
                    float normalizedValue = Mathf.InverseLerp(screenSize.y - distanceBords.y, screenSize.y, posTouch.y);
                    // Interpoler la vitesse entre 0 et outsideSpeedDeplacementMax.y
                    movement.y = Mathf.Lerp(0, -outsideSpeedDeplacementMax.y, normalizedValue);
                }

                if (movement == Vector2.zero)
                    yield return null;

                Debug.Log(movement);
                CameraMovement(movement, true);
                yield return null;
            }
        }
        else
        {
            cameraMovementCoroutine = null;
            yield break;
        }
        cameraMovementCoroutine = null;
        yield break;
    }
}
