using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Throw a given prefab into the scene
/// </summary>
public class PlayerThrow : MonoBehaviour
{
    // model which will be thrown
    public GameObject projectile;
    public Camera mainCamera;

    public InputActionAsset playerActions;
    private InputAction _throw;

    // Strength of throw
    public float shootStrength = 100;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Action Map
        var playerControls = playerActions.FindActionMap("PlayerControls");

        // Get the actions
        _throw = playerControls.FindAction("Throw");

        // Enable the actions
        _throw.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (_throw.triggered)
        {
            // Calculate a point in front of the camera
            Vector3 position = mainCamera.transform.position + transform.forward * 0.5f;

            // Create a new Instance of the prefab
            GameObject newProjectile = Instantiate(projectile, position, Quaternion.identity);

            // Add a little bit of force
            newProjectile.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * shootStrength);
        }
    }

    public void ToggleThrow(bool enable)
    {
        if (enable)
        {
            _throw.Enable();
        }
        else
        {
            _throw.Disable();
        }
    }
}
