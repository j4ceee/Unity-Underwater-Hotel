using UnityEngine;

/**
 * Changes the Crest Underwater Renderer's mode to "Portal" when the player enters the water volume (trigger) and changes it back to "Fullscreen" when the player exits the water volume.
 *
 * Requirements:
 * [1] Put on a trigger GameObject
 * [2] Assign player camera
 */
public class SwitchWaterVolume : MonoBehaviour
{
    public Camera playerCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // change Crest Underwater Renderer's mode to "Portal"
            playerCamera.GetComponent<Crest.UnderwaterRenderer>()._mode = Crest.UnderwaterRenderer.Mode.Portal;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // change Crest Underwater Renderer's mode to "Fullscreen"
            playerCamera.GetComponent<Crest.UnderwaterRenderer>()._mode = Crest.UnderwaterRenderer.Mode.FullScreen;
        }
    }
}
