using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class InteriorLightTrigger : MonoBehaviour
{
    public GameObject roomLightGroup;
    [Tooltip("Reflection Probes to enable / disable")]
    public HDProbe[] reflectionProbes;

    private InteriorLightController _interiorLightController;
    private PlayerController _playerController;

    private void Start()
    {
        _interiorLightController = FindObjectOfType<InteriorLightController>();
        _playerController = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerController.AddTrigger(name);

            _interiorLightController.ToggleRoomLights(roomLightGroup, true);
            _interiorLightController.ToggleReflectionProbes(reflectionProbes, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int resultCount = _playerController.DeleteTrigger(name);
            if (resultCount > 0) return;

            _interiorLightController.ToggleRoomLights(roomLightGroup, false);
            _interiorLightController.ToggleReflectionProbes(reflectionProbes, false);
        }
    }
}
