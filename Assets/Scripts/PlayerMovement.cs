#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;
    public GameObject player;

    public float speed = 4f;
    private float _speedStart;
    public float gravity = -10f;
    public float jumpHeight = 0.6f;
    private float _jumpHeightStart;

    [Tooltip("Time in seconds before the player can jump again")]
    public float jumpCooldown = .2f;
    private float _lastJumpTime = -1f; // time when the player last jumped

    public Transform groundCheck;
    public Transform roofCheck;
    public float groundDistance = 0.25f;
    //public LayerMask groundMask;

    public InputActionAsset playerActions;

    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _isCrouching = false;

    private InputAction _movement;
    private InputAction _jump;
    private InputAction _crouch;

    private void Start()
    {
        // Get the Action Map
        var playerControls = playerActions.FindActionMap("PlayerControls");

        // Get the actions
        _movement = playerControls.FindAction("Movement");
        _jump = playerControls.FindAction("Jump");
        _crouch = playerControls.FindAction("Crouch");

        // Enable the actions
        PauseMovement(false);

        _jumpHeightStart = jumpHeight;
        _speedStart = speed;
    }

    // Update is called once per frame
    private void Update()
    {
        float x;
        float z;

        var delta = _movement.ReadValue<Vector2>();
        x = delta.x;
        z = delta.y;

        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * (speed * Time.deltaTime));

        if(_jump.triggered && _isGrounded && Time.time >= _lastJumpTime + jumpCooldown)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _lastJumpTime = Time.time; // update the last jump time
        }

        // check if the player is hitting an obstacle above
        if (_velocity.y > 0 && Physics.CheckSphere(roofCheck.position, groundDistance))
        {
            _velocity.y = 0;
        }

        _velocity.y += gravity * Time.deltaTime;

        controller.Move(_velocity * Time.deltaTime);

        // Crouch logic
        if (_crouch.triggered)
        {
            _isCrouching = !_isCrouching;
            player.transform.localScale = _isCrouching ? new Vector3(1, 0.5f, 1) : new Vector3(1, 1, 1);

            jumpHeight = _isCrouching ? (.5f*_jumpHeightStart) : _jumpHeightStart;
            speed = _isCrouching ? (.5f*_speedStart) : _speedStart;
        }
    }

    public void PauseMovement(bool pause)
    {
        if (pause)
        {
            _movement.Disable();
            _jump.Disable();
            _crouch.Disable();
        }
        else
        {
            _movement.Enable();
            _jump.Enable();
            _crouch.Enable();
        }
    }
}
