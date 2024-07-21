using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Tooltip("Text to show on HUD, describes the action that can be performed")]
    public string actionName;

    public void TriggerAction()
    {
        Debug.Log("Triggered action: " + actionName);
    }
}
