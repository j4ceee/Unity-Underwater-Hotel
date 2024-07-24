using UnityEngine;

/// <summary>
/// Base class for all interactable objects in the scene
/// </summary>
public abstract class InteractableObject : MonoBehaviour
{
    [Tooltip("Text to show on HUD, describes the action that can be performed")]
    public string actionName;

    public abstract void TriggerAction();

}
