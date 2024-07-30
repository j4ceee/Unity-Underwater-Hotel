using System;
using UnityEngine;

public enum InteractableType
{
    PickUp,
    Toggle,
    OpenDoor,
}

/// <summary>
/// Base class for all interactable objects in the scene
/// </summary>
public abstract class InteractableObject : MonoBehaviour
{
    [Tooltip("Text to show on HUD, describes the action that can be performed")]
    public InteractableType interactionName;

    [Tooltip("Text to show on HUD, which object is being interacted with")]
    public string objectName;

    public abstract void TriggerAction();

    public string GetInteractionName()
    {
        if (interactionName == InteractableType.PickUp)
        {
            return "Pick Up " + objectName;
        }
        else if (interactionName == InteractableType.Toggle)
        {
            return "Toggle " + objectName;
        }
        else
        {
            return "Interact with " + objectName;
        }
    }
}
