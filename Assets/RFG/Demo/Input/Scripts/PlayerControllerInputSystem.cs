using UnityEngine;
using UnityEngine.InputSystem;
using RFG;

public class PlayerControllerInputSystem : MonoBehaviour
{
  [field: SerializeField] private float MovementSpeed { get; set; } = 3f;
  [field: SerializeField] private float MovementSmoothingSpeed { get; set; } = 1f;
  [field: SerializeField] private Aim Aim { get; set; }
  [field: SerializeField] private float DashSpeed { get; set; } = 100f;
  [field: SerializeField] private GameEvent PauseEvent { get; set; }
  [field: SerializeField] private GameEvent UnPauseEvent { get; set; }

  private PlayerInput _playerInput;
  private Vector2 _velocity;
  private Vector2 _rawInputMovement;
  private Vector2 _dashDirection;

  private void Awake()
  {
    _playerInput = GetComponent<PlayerInput>();
    Aim.Init();
  }

  private void Update()
  {
    _rawInputMovement = _playerInput.actions["Movement"].ReadValue<Vector2>();
    Vector2 speed = _rawInputMovement * MovementSpeed;
    _velocity = Vector2.Lerp(_velocity, speed, Time.deltaTime * MovementSmoothingSpeed);
    transform.Translate(_velocity * Time.deltaTime, Space.World);
  }


  private void ComputerDashDirection()
  {
    Aim.PrimaryMovement = _playerInput.actions["Movement"].ReadValue<Vector2>();
    Aim.CurrentPosition = transform.position;
    _dashDirection = Aim.GetCurrentAim();
    _dashDirection = _dashDirection.normalized;
  }

  private void OnAttack(InputAction.CallbackContext ctx)
  {
    Debug.Log("Attack!");
    ComputerDashDirection();
    transform.Translate((_dashDirection * DashSpeed) * Time.deltaTime, Space.World);
  }

  private void OnAttackCanceled(InputAction.CallbackContext ctx)
  {
    Debug.Log("Attack Canceled!");
  }

  private void OnAttackStarted(InputAction.CallbackContext ctx)
  {
    Debug.Log("Attack Started!");
  }

  public void OnPause(InputValue value)
  {
    Debug.Log("Pause");
    if (GameManager.Instance.IsPaused)
    {
      UnPauseEvent?.Raise();
    }
    else
    {
      PauseEvent?.Raise();
    }
  }

  private void OnEnable()
  {
    _playerInput.actions["Movement"].Enable();
    _playerInput.actions["PrimaryAttack"].Enable();
    _playerInput.actions["Pause"].Enable();

    _playerInput.actions["PrimaryAttack"].performed += OnAttack;
    _playerInput.actions["PrimaryAttack"].started += OnAttackStarted;
    _playerInput.actions["PrimaryAttack"].canceled += OnAttackCanceled;
    // _playerInput.actions["Pause"].performed += OnPause;
  }

  private void OnDisable()
  {
    _playerInput.actions["Movement"].Disable();
    _playerInput.actions["PrimaryAttack"].Disable();
    _playerInput.actions["Pause"].Disable();

    _playerInput.actions["PrimaryAttack"].performed -= OnAttack;
    _playerInput.actions["PrimaryAttack"].started -= OnAttackStarted;
    _playerInput.actions["PrimaryAttack"].canceled -= OnAttackCanceled;
    // _playerInput.actions["Pause"].performed -= OnPause;
  }

}
