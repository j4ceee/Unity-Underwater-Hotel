using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLightGroup : InteractableObject
{
    public GameObject lightGroup;
    private InteriorLightController _interiorLightController;

    void Start()
    {
        _interiorLightController = FindObjectOfType<InteriorLightController>();
    }

    public override void TriggerAction()
    {
        _interiorLightController.ToggleSpecificLightGroup(lightGroup);
    }
}
