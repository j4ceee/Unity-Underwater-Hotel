using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Picks up the object interacted with & enables the player to throw it
/// </summary>
public class PickUpObject : InteractableObject
{
    [Tooltip("Prefab to create thrown object from")]
    public GameObject objectToThrow;

    private PlayerThrow _playerThrowController;
    private PlayerGazeController _playerGazeController;

    private void Start()
    {
        _playerThrowController = FindObjectOfType<PlayerThrow>();
        _playerGazeController = FindObjectOfType<PlayerGazeController>();
    }

    public override void TriggerAction()
    {
        if (_playerThrowController && _playerGazeController)
        {
            bool occupied = _playerThrowController.ObjectIsEquipped();

            if (occupied) return;
            Vector3 rotation = _playerGazeController.GetInteractableObjectRotation();
            _playerThrowController.SetProjectileFromPrefab(objectToThrow, rotation, gameObject);
        }
    }

    public override string GetInteractionName()
    {
         return "Pick Up " + objectName;
    }
}
