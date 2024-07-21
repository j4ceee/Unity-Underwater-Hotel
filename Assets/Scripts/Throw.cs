using UnityEngine;


/// <summary>
/// Throw a given prefab into the scene
/// </summary>
public class Throw : MonoBehaviour
{
    // Prefab which will thrown
    public GameObject projectile;

    // Strength of throw
    public float shootStrength = 100;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Calculate a point in front of the camera
            Vector3 position = Camera.main.transform.position + transform.forward * 0.5f;

            // Create a new Instance of the prefab
            GameObject newProjectile = Instantiate(projectile, position, Quaternion.identity);

            // Add a little bit of force
            newProjectile.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * shootStrength);    
        }
    }
}
