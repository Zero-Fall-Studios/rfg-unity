using UnityEngine;
using UnityEngine.InputSystem;
using RFG;

public class PlayerControllerInputSystem : MonoBehaviour
{
  [field: SerializeField] private float MovementSpeed { get; set; } = 3f;
  [field: SerializeField] private float MovementSmoothingSpeed { get; set; } = 1f;
  [field: SerializeField] private Aim Aim { get; set; }
  [field: SerializeField] private float DashSpeed { get; set; } = 100f;

  private PlayerInputActions _inputActions;
  private InputAction _movement;

  private Vector2 _velocity;
  private Vector2 _rawInputMovement;
  private Vector2 _dashDirection;

  private void Awake()
  {
    _inputActions = new PlayerInputActions();
    Aim.Init();
  }

  private void Update()
  {
    CalculateMovementInputSmoothing();
    UpdatePlayerMovement();
  }

  private void FixedUpdate()
  {
    Vector2 inputMovement = _movement.ReadValue<Vector2>();
    _rawInputMovement = inputMovement;
  }

  private void CalculateMovementInputSmoothing()
  {
    Vector2 speed = _rawInputMovement * MovementSpeed;
    _velocity = Vector2.Lerp(_velocity, speed, Time.deltaTime * MovementSmoothingSpeed);
  }

  private void UpdatePlayerMovement()
  {
    transform.Translate(_velocity * Time.deltaTime, Space.World);
  }

  private void ComputerDashDirection()
  {
    Aim.PrimaryMovement = _movement.ReadValue<Vector2>();
    Aim.CurrentPosition = transform.position;
    _dashDirection = Aim.GetCurrentAim();
    _dashDirection = _dashDirection.normalized;
  }

  private void OnAttack(InputAction.CallbackContext ctx)
  {
    // if (ctx.started)
    Debug.Log("Attack!");
    ComputerDashDirection();
    transform.Translate((_dashDirection * DashSpeed) * Time.deltaTime, Space.World);
  }

  private void OnAttackCanceled(InputAction.CallbackContext ctx)
  {
    // if (ctx.started)
    Debug.Log("Attack Canceled!");
  }

  private void OnAttackStarted(InputAction.CallbackContext ctx)
  {
    // if (ctx.started)
    Debug.Log("Attack Started!");
  }

  private void OnPause(InputAction.CallbackContext ctx)
  {
    Debug.Log("Pause");
  }

  private void OnEnable()
  {
    //inputActions.PlayerControls
    _movement = _inputActions.PlayerControls.Movement;
    _movement.Enable();

    _inputActions.PlayerControls.PrimaryAttack.performed += OnAttack;
    _inputActions.PlayerControls.PrimaryAttack.started += OnAttackStarted;
    _inputActions.PlayerControls.PrimaryAttack.canceled += OnAttackCanceled;
    _inputActions.PlayerControls.Pause.performed += OnPause;

    _inputActions.PlayerControls.PrimaryAttack.Enable();
    _inputActions.PlayerControls.Pause.Enable();
  }

  private void OnDisable()
  {
    _movement.Disable();
    _inputActions.PlayerControls.PrimaryAttack.Disable();
    _inputActions.PlayerControls.Pause.Disable();
  }

}
