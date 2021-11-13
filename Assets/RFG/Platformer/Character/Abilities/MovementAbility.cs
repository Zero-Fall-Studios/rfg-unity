using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Movement")]
  public class MovementAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private InputActionReference _movement;
    private SettingsPack _settings;
    private InputActionReference _runInput;
    private bool _isRunning = false;
    private float _walkToRunTimeElapsed = 0f;
    private float _horizontalSpeed = 0f;

    #region Unity Methods
    private void Awake()
    {
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _movement = _character.InputPack.Movement;
      _runInput = _character.InputPack.RunInput;
      _settings = _character.SettingsPack;

      if (_settings.AlwaysRun)
      {
        _isRunning = true;
      }
    }

    private void Start()
    {
      _state = _controller.State;
    }

    private void Update()
    {
      HandleMovement();
      DetectFallingMovement();
    }
    #endregion

    #region Handlers
    private void HandleMovement()
    {
      if (!CanMove())
      {
        return;
      }

      // Read in horizontal input
      _horizontalSpeed = _movement.action.ReadValue<Vector2>().x;

      HandleFacing();
      DetectMovementState();
      HandleChangeToRunState();
      MoveCharacter();
    }

    private bool CanMove()
    {
      if (!_character.IsAlive() || _character.IsDashing())
      {
        ResetMovement();
        return false;
      }
      return true;
    }

    private void HandleFacing()
    {
      if (_horizontalSpeed > 0f)
      {
        if (!_state.IsFacingRight && !_controller.rotateOnMouseCursor)
        {
          _controller.Flip();
        }
      }
      else if (_horizontalSpeed < 0f)
      {
        if (_state.IsFacingRight && !_controller.rotateOnMouseCursor)
        {
          _controller.Flip();
        }
      }
    }

    private void DetectMovementState()
    {
      if (_character.IsInGroundMovementState())
      {
        if (_horizontalSpeed == 0)
        {
          if (!_character.IsDangling())
          {
            _character.MovementState.ChangeState(typeof(IdleState));
          }
          ResetMovement();
        }
        else
        {
          DetectSlopeMovement();
          if (_state.IsMovingUpSlope)
          {
            _character.MovementState.ChangeState(_isRunning ? typeof(RunningUpSlopeState) : typeof(WalkingUpSlopeState));
          }
          else if (_state.IsMovingDownSlope)
          {
            _character.MovementState.ChangeState(_isRunning ? typeof(RunningDownSlopeState) : typeof(WalkingDownSlopeState));
          }
          else
          {
            _character.MovementState.ChangeState(_isRunning ? typeof(RunningState) : typeof(WalkingState));
          }
        }
      }
    }

    private void DetectSlopeMovement()
    {
      _state.IsMovingUpSlope = false;
      _state.IsMovingDownSlope = false;
      if (
        (_state.IsFacingRight && _state.BelowSlopeAngle > 0 && _horizontalSpeed > 0) ||
        (!_state.IsFacingRight && _state.BelowSlopeAngle < 0 && _horizontalSpeed < 0)
      )
      {
        _state.IsMovingUpSlope = true;
      }
      else if (
        (_state.IsFacingRight && _state.BelowSlopeAngle < 0 && _horizontalSpeed > 0) ||
        (!_state.IsFacingRight && _state.BelowSlopeAngle > 0 && _horizontalSpeed < 0)
      )
      {
        _state.IsMovingDownSlope = true;
      }
    }

    private void HandleChangeToRunState()
    {
      // If there is WalkToRunTime, movement and not running
      if (
        _settings.WalkToRunTime > 0 &&
        _horizontalSpeed != 0f &&
         !_isRunning)
      {
        // Now check to see if WalkToRunTime has elapsed enough to start running
        if (_walkToRunTimeElapsed > _settings.WalkToRunTime)
        {
          _isRunning = true;
          _walkToRunTimeElapsed = 0;
        }
        else
        {
          _walkToRunTimeElapsed += Time.deltaTime;
        }
      }
    }

    private void MoveCharacter()
    {
      float speed = _isRunning ? _settings.RunningSpeed : _settings.WalkingSpeed;
      float movementFactor = _state.IsGrounded ? _controller.Parameters.GroundSpeedFactor : _controller.Parameters.AirSpeedFactor;
      float movementSpeed = _horizontalSpeed * speed * _controller.Parameters.SpeedFactor;
      float horizontalMovementForce = Mathf.Lerp(_controller.Speed.x, movementSpeed, Time.deltaTime * movementFactor);

      // add any external forces that may be active right now
      if (Mathf.Abs(_controller.ExternalForce.x) > 0)
      {
        horizontalMovementForce += _controller.ExternalForce.x;
      }

      // we handle friction
      horizontalMovementForce = HandleFriction(horizontalMovementForce);

      // Debug.Log(horizontalMovementForce);

      _controller.SetHorizontalForce(horizontalMovementForce);
    }

    private float HandleFriction(float force)
    {
      // if we have a friction above 1 (mud, water, stuff like that), we divide our speed by that friction
      if (_controller.Friction > 1)
      {
        force = force / _controller.Friction;
      }

      // if we have a low friction (ice, marbles...) we lerp the speed accordingly
      if (_controller.Friction < 1 && _controller.Friction > 0)
      {
        force = Mathf.Lerp(_controller.Speed.x, force, Time.deltaTime * _controller.Friction * 10);
      }

      return force;
    }

    private void ResetMovement()
    {
      _walkToRunTimeElapsed = 0;
      _isRunning = false;
    }

    private void DetectFallingMovement()
    {
      if (
        _controller.Speed.y < 0 &&
        !_character.IsIdle() &&
        !_character.IsInSlopeMovementState() &&
        !_character.IsWallClinging() &&
        !_character.IsLedgeGrabbing()
      )
      {
        _character.MovementState.ChangeState(typeof(FallingState));
      }
    }
    #endregion

    #region Events
    private void OnRunStarted(InputAction.CallbackContext obj)
    {
      _isRunning = true;
    }

    private void OnRunCanceled(InputAction.CallbackContext obj)
    {
      if (_settings.AlwaysRun)
      {
        return;
      }
      _isRunning = false;
    }

    private void OnStateTypeChange(Type prevType, Type currentType)
    {
      bool beganWalking = prevType != typeof(WalkingState) && currentType == typeof(WalkingState);
      bool isntWalking = currentType != typeof(WalkingState) && currentType != typeof(WalkingDownSlopeState);
      if (beganWalking || isntWalking)
      {
        _walkToRunTimeElapsed = 0;
      }
    }

    private void OnEnable()
    {
      // Make sure to setup new events
      OnDisable();

      if (_runInput != null)
      {
        _runInput.action.started += OnRunStarted;
        _runInput.action.canceled += OnRunCanceled;
      }
      _character.MovementState.OnStateTypeChange += OnStateTypeChange;
    }

    private void OnDisable()
    {
      if (_runInput != null)
      {
        _runInput.action.started -= OnRunStarted;
        _runInput.action.canceled -= OnRunCanceled;
      }
      _character.MovementState.OnStateTypeChange -= OnStateTypeChange;
    }
    #endregion
  }
}