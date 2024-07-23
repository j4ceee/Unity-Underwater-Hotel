#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 10f; // Sensitivity for mouse
    public float controllerSensitivity = 100f; // Sensitivity for controller

    public Transform playerBody;

    public InputActionAsset playerActions;

    private float _xRotation;

    private Camera _camera;

    private InputAction _look;
    private InputAction _zoom;

    // Start is called before the first frame update
    private void Start()
    {
        // get the camera component
        _camera = GetComponent<Camera>();

        // get the Action Map
        var playerControls = playerActions.FindActionMap("PlayerControls");

        // get the actions
        _look = playerControls.FindAction("Look");
        _zoom = playerControls.FindAction("Zoom");

        // enable the actions
        PauseLook(false);
    }

// Update is called once per frame
    private void Update()
    {
        float mouseX = 0, mouseY = 0;
        var delta = _look.ReadValue<Vector2>();
        mouseX += delta.x;
        mouseY += delta.y;

        // Check if the input is from a gamepad or a mouse
        if (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0) // mouse is moving
        {
            mouseX *= mouseSensitivity * Time.deltaTime;
            mouseY *= mouseSensitivity * Time.deltaTime;
        }
        else if (Gamepad.current != null && Gamepad.current.rightStick.ReadValue().sqrMagnitude > 0) // right stick is moving
        {
            mouseX *= controllerSensitivity * Time.deltaTime;
            mouseY *= controllerSensitivity * Time.deltaTime;
        }

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);

        if (_zoom.triggered)
        {
            _camera.fieldOfView = Mathf.Approximately(_camera.fieldOfView, 60) ? 40 : 60;
        }
    }

    public void PauseLook(bool pause)
    {
        if (pause)
        {
            _look.Disable();
            _zoom.Disable();
        }
        else
        {
            _look.Enable();
            _zoom.Enable();
        }
    }
}