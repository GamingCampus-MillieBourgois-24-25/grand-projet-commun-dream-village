using UnityEngine;
using UnityEngine.InputSystem;

public class CameraDeplacement : MonoBehaviour
{
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

    private float prevMagnitude = 0f;
    private int touchCount = 0;
    IsoManager isoManager;

    void Start()
    {
        isoManager = IsoManager.Instance;

        // Mouse scroll wheel input
        InputAction scrollAction = new InputAction("Scroll", binding: "<Mouse>/scroll");
        scrollAction.Enable();
        scrollAction.performed += ctx => CameraZoom(ctx.ReadValue<Vector2>().y * zoomSpeed);

        touch1Action.action.Enable();
        touch2Action.action.Enable();
        touch1Action.action.performed += ctx => touchCount++;
        touch2Action.action.performed += ctx => touchCount++;
        touch1Action.action.canceled += ctx => RemoveTouch();
        touch2Action.action.canceled += ctx => RemoveTouch();

        zoom1Action.action.Enable();
        zoom2Action.action.Enable();
        zoom2Action.action.performed += ctx => OnPinch();

        MoveAction.action.Enable();
        touch1Action.action.performed += ctx => MoveAction.action.performed += ctx => CameraMovement(ctx.ReadValue<Vector2>() * moveSpeed);
        touch1Action.action.canceled += ctx => MoveAction.action.performed -= ctx => CameraMovement(ctx.ReadValue<Vector2>() * moveSpeed);

    }




    private void OnPinch()
    {
        float magnitude = (zoom1Action.action.ReadValue<Vector2>() - zoom2Action.action.ReadValue<Vector2>()).magnitude;
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
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + incr, 1, 7);
    }

    private void CameraMovement(Vector2 movement)
    {

        if (isoManager.IsEditMode())
            return;

        Debug.Log("CameraMovement: " + movement);

        Vector3 newPosition = Camera.main.transform.position;

        newPosition.x -= movement.x / 1.7f;
        newPosition.z += movement.x / 1.7f;


        newPosition.x -= movement.y * 1.7f;
        newPosition.z -= movement.y * 1.7f;


        if(newPosition.x < centerCamera.x - limCam.x)
            newPosition.x = centerCamera.x - limCam.x;
        if (newPosition.x > centerCamera.x + limCam.x)
            newPosition.x = centerCamera.x + limCam.x;

        if (newPosition.z < centerCamera.y - limCam.y)
            newPosition.z = centerCamera.y - limCam.y;
        if (newPosition.z > centerCamera.y + limCam.y)
            newPosition.z = centerCamera.y + limCam.y;


        Camera.main.transform.position = newPosition;
    }
}
