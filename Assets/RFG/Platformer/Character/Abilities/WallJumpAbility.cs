using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Wall Jump")]
  public class WallJumpAbility : MonoBehaviour, IAbility
  {
    private Transform _transform;
    private Character _character;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private Animator _animator;
    private PlayerInput _playerInput;
    private InputAction _wallJumpInput;
    private InputAction _movement;
    private SettingsPack _settings;

    private void Awake()
    {
      _transform = transform;
      _character = GetComponent<Character>();
      _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
      _animator = _character.Context.animator;
      _controller = _character.Context.controller;
      _state = _character.Context.controller.State;
      _movement = _playerInput.actions["Movement"];
      _wallJumpInput = _playerInput.actions["Jump"];
      _settings = _character.Context.settingsPack;

      // Setup events
      OnEnable();
    }

    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
      if (
           _character.MovementState.HasState(typeof(WallJumpingState))
        && _character.MovementState.CurrentStateType == typeof(WallClingingState)
      )
      {
        WallJump();
      }
    }

    private void WallJump()
    {
      _character.MovementState.ChangeState(typeof(WallJumpingState));
      _controller.SlowFall(0f);

      Vector2 _movementVector = _movement.ReadValue<Vector2>();
      float _horizontalInput = _movementVector.x;
      bool isClingingLeft = _state.IsCollidingLeft && _horizontalInput <= -_settings.WallJumpInputThreshold;
      bool isClingingRight = _state.IsCollidingRight && _horizontalInput >= _settings.WallJumpInputThreshold;

      float wallJumpDirection;
      if (isClingingRight)
      {
        wallJumpDirection = -1f;
      }
      else
      {
        wallJumpDirection = 1f;
      }

      Vector2 wallJumpVector = new Vector2(wallJumpDirection * _settings.WallJumpForce.x, Mathf.Sqrt(2f * _settings.WallJumpForce.y * Mathf.Abs(_controller.Parameters.Gravity)));

      _controller.AddForce(wallJumpVector);
    }

    private void OnEnable()
    {
      if (_wallJumpInput != null)
      {
        _wallJumpInput.started += OnJumpStarted;
      }
    }

    private void OnDisable()
    {
      if (_wallJumpInput != null)
      {
        _wallJumpInput.started -= OnJumpStarted;
      }
    }

  }
}