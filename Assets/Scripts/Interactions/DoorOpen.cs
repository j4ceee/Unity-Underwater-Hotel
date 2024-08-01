using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Door
{
    Open,
    Closed
}

public class DoorOpen : InteractableObject
{
    [Tooltip("Animator of the door to open, uses animator on current object if not set")]
    public Animator doorAnimator;

    public Door currentDoorState;
    private static readonly int IsOpen = Animator.StringToHash("IsOpen"); // name of the bool in the animator

    [Tooltip("Occlusion portal to open/close with the door (if any)")]
    public OcclusionPortal occlusionPortal;

    private void Start()
    {
        if (!doorAnimator)
        {
            try
            {
                doorAnimator = GetComponent<Animator>();
            }
            catch
            {
                Debug.LogError("No animator found on object, please add one or set the doorAnimator variable");
            }
        }
    }

    public override void TriggerAction()
    {
        if (doorAnimator)
        {
            if (currentDoorState == Door.Open)
            {
                doorAnimator.SetBool(IsOpen, false);
                currentDoorState = Door.Closed;
            }
            else
            {
                doorAnimator.SetBool(IsOpen, true);
                currentDoorState = Door.Open;
            }
        }
    }

    public override string GetInteractionName()
    {
        if (currentDoorState == Door.Open)
        {
            return "Close " + objectName;
        }
        // else
        return "Open " + objectName;
    }
}
