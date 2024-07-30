using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


/// <summary>
/// Throw a given prefab into the scene
/// </summary>
public class PlayerThrow : MonoBehaviour
{
    // model which will be thrown
    public GameObject projectilePrefab;
    private Vector3 _prefabRotation;
    public Camera mainCamera;

    private bool _hasObject = false;

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
        if (_throw.triggered && _hasObject)
        {
            // Calculate a point in front of the camera
            Vector3 position = mainCamera.transform.position + transform.forward * 0.5f;

            // Create a new Instance of the prefab
            GameObject newProjectile = Instantiate(projectilePrefab, position, Quaternion.Euler(_prefabRotation));

            newProjectile.GetComponent<PickUpObject>().objectToThrow = projectilePrefab;
            // Add a little bit of force
            newProjectile.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * shootStrength);

            // unset projectile variable
            projectilePrefab = null; // only throw once

            _hasObject = false;
        }
    }

    public void SetProjectileFromPrefab(GameObject prefab, Vector3 rotation, GameObject destroyObject)
    {
        projectilePrefab = prefab;
        _prefabRotation = rotation;

        _hasObject = true;

        Destroy(destroyObject);
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

    public bool ObjectIsEquipped()
    {
        return _hasObject;
    }
}
