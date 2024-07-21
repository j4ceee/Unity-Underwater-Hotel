using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerMovement playerMovementController;
    public PlayerLook playerLookController;
    public GazeController gazeController;

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

        ResumeGame();
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
        gazeController.ToggleRaycasts(!_isPaused); // enable / disable raycasts (opposite of pause -> disable when paused)

        pauseUi.SetActive(_isPaused);
        gameUi.SetActive(!_isPaused);
    }
}
