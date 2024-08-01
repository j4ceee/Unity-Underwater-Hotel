using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGazeController : MonoBehaviour
{
    [Tooltip("Max distance for the raycast in meters")]
    public float maxDistance = 2;

    [Tooltip("HUD text element to show action name on")]
    public TMP_Text actionLabel;

    public Camera mainCamera;
    public InputActionAsset playerActions;
    private InputAction _interact;

    // store the last interactable object that was hit
    private InteractableObject _currentInteractable;
    private Vector3 _interactableObjectRotation;

    public LayerMask ignoreTriggerLayer;

    private bool _doRaycast = true;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Action Map
        var playerControls = playerActions.FindActionMap("PlayerControls");

        // Get the actions
        _interact = playerControls.FindAction("Interact");

        // Enable the actions
        _interact.Enable();
    }

    public void ToggleInteractRaycasts(bool enable)
    {
        _doRaycast = enable;

        if (_doRaycast)
        {
            _interact.Enable();
        }
        else
        {
            _interact.Disable();
        }
    }

    private void FixedUpdate()
    {
        if (!_doRaycast) return;

        Vector3 forward = mainCamera.transform.forward;
        Vector3 origin = mainCamera.transform.position;
        RaycastHit hit;

        if (Physics.Raycast(origin, forward, out hit, maxDistance, ~ignoreTriggerLayer) && hit.collider.gameObject.GetComponent<InteractableObject>())
        {
            InteractableObject hitInteractable = hit.collider.gameObject.GetComponent<InteractableObject>();

            if (_currentInteractable == hitInteractable) return;
            // show action label if object is hit & interactable
            actionLabel.text = hitInteractable.GetInteractionName();
            _currentInteractable = hitInteractable;
            _interactableObjectRotation = hit.collider.transform.rotation.eulerAngles;

            Debug.DrawRay(origin, forward * hit.distance, Color.green);
        }
        else
        {
            // clear action label if no object is hit / not interactable
            if (actionLabel.text != "")
            {
                actionLabel.text = "";
            }
            _currentInteractable = null;
            _interactableObjectRotation = Vector3.zero;

            Debug.DrawRay(origin, forward * maxDistance, Color.red);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_doRaycast) return;

        if (_interact.triggered && _currentInteractable)
        {
            _currentInteractable.TriggerAction();
            actionLabel.text = _currentInteractable.GetInteractionName();
        }
    }

    public Vector3 GetInteractableObjectRotation()
    {
        return _interactableObjectRotation;
    }
}
