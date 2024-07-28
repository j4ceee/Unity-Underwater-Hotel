using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public PlayerMovement playerMovementController;
    public PlayerLook playerLookController;
    public PlayerGazeController playerGazeController;
    public PlayerThrow playerThrowController;

    public InputActionAsset playerActions;

    public GameObject pauseUi;
    public GameObject gameUi;

    private InputAction _pause;

    private bool _isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Action Map
        var playerControls = playerActions.FindActionMap("PlayerControls");

        // Get the actions
        _pause = playerControls.FindAction("Pause");

        // Enable the actions
        _pause.Enable();

        PauseGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (_pause.triggered)
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        _isPaused = true;
        ChangeGameState();
        Cursor.lockState = CursorLockMode.None;
    }

    private void ResumeGame()
    {
        _isPaused = false;
        ChangeGameState();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ChangeGameState()
    {
        playerMovementController.PauseMovement(_isPaused); // enable / disable movement
        playerLookController.PauseLook(_isPaused); // enable / disable look

        playerGazeController.ToggleInteractRaycasts(!_isPaused); // enable / disable raycasts & interaction (opposite of pause -> disable when paused)
        playerThrowController.ToggleThrow(!_isPaused); // enable / disable throwing (opposite of pause -> disable when paused)

        pauseUi.SetActive(_isPaused);
        gameUi.SetActive(!_isPaused);
    }

    [HideInInspector]
    public List<string> triggerList = new List<string>();

    public void AddTrigger(string trigger)
    {
        triggerList.Add(trigger);
    }

    private int CountTriggerOccurences(string trigger)
    {
        int count = 0;
        foreach (string t in triggerList)
        {
            if (t == trigger)
            {
                count++;
            }
        }
        return count;
    }

    public int DeleteTrigger(string trigger)
    {
        // delete the first occurence of the trigger
        if (triggerList.Contains(trigger))
        {
            triggerList.Remove(trigger);
        }

        return CountTriggerOccurences(trigger);
    }

}
