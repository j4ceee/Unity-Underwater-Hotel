using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorLightTrigger : MonoBehaviour
{
    public GameObject roomLightGroup;

    private InteriorLightController _interiorLightController;

    private void Start()
    {
        _interiorLightController = FindObjectOfType<InteriorLightController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _interiorLightController.ToggleRoomLights(roomLightGroup, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _interiorLightController.ToggleRoomLights(roomLightGroup, false);
        }
    }
}
