using System;
using UnityEngine;

/// <summary>
/// Base class for all interactable objects in the scene
/// </summary>
public abstract class InteractableObject : MonoBehaviour
{
    [Tooltip("Text to show on HUD, which object is being interacted with")]
    public string objectName;

    public abstract void TriggerAction();

    public abstract string GetInteractionName();
}
