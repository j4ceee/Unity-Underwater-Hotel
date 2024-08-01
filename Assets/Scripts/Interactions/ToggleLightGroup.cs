using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLightGroup : InteractableObject
{
    public GameObject[] lightGroups;
    private InteriorLightController _interiorLightController;

    void Start()
    {
        _interiorLightController = FindObjectOfType<InteriorLightController>();
    }

    public override void TriggerAction()
    {
        if (!_interiorLightController || lightGroups.Length == 0) return;
        foreach (GameObject lightGroup in lightGroups)
        {
            _interiorLightController.ToggleSpecificLightGroup(lightGroup);
        }
    }

    public override string GetInteractionName()
    {
        return "Toggle " + objectName;
    }
}
