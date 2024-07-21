using TMPro;
using UnityEngine;

public class GazeController : MonoBehaviour
{
    [Tooltip("Max distance for the raycast in meters")]
    public float maxDistance = 2;

    [Tooltip("HUD text element to show action name on")]
    public TMP_Text actionLabel;

    public Camera mainCamera;

    // store the last interactable object that was hit
    private InteractableObject _currentInteractable;

    private bool _doRaycast = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (!_doRaycast) return;

        Vector3 forward = mainCamera.transform.forward;
        Vector3 origin = mainCamera.transform.position;
        RaycastHit hit;

        if (Physics.Raycast(origin, forward, out hit, maxDistance) && hit.collider.gameObject.GetComponent<InteractableObject>() != null)
        {
            InteractableObject hitInteractable = hit.collider.gameObject.GetComponent<InteractableObject>();

            if (_currentInteractable == hitInteractable) return;
            // show action label if object is hit & interactable
            actionLabel.text = hitInteractable.actionName;
            _currentInteractable = hitInteractable;
        }
        else
        {
            // clear action label if no object is hit / not interactable
            if (actionLabel.text != "")
            {
                actionLabel.text = "";
            }
            _currentInteractable = null;
        }
    }

    public void ToggleRaycasts(bool enable)
    {
        _doRaycast = enable;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
