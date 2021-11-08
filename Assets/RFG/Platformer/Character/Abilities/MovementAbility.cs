using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Movement")]
  public class MovementAbility : MonoBehaviour, IAbility
  {
    [HideInInspector]
    private Character _character;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private InputActionReference _movement;
    private SettingsPack _settings;
    private InputActionReference _runInput;
    private bool _isRunning = false;
    private float _walkToRunTimeElapsed = 0f;

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
      if (_character.CharacterState.CurrentStateType != typeof(AliveState) || Time.timeScale == 0f)
      {
        _walkToRunTimeElapsed = 0;
        _isRunning = false;
        return;
      }

      float horizontalSpeed = _movement.action.ReadValue<Vector2>().x;

      if (horizontalSpeed > 0f)
      {
        if (!_state.IsFacingRight && !_controller.rotateOnMouseCursor)
        {
          _controller.Flip();
        }
      }
      else if (horizontalSpeed < 0f)
      {
        if (_state.IsFacingRight && !_controller.rotateOnMouseCursor)
        {
          _controller.Flip();
        }
      }

      // If the movement state is dashing return so it wont get set back to idle
      if (_character.MovementState.CurrentStateType == typeof(DashingState))
      {
        _walkToRunTimeElapsed = 0;
        _isRunning = false;
        return;
      }

      if ((_state.IsGrounded || _state.JustGotGrounded)
        && _character.MovementState.CurrentStateType != typeof(JumpingState)
        && _character.MovementState.CurrentStateType != typeof(LandedState))
      {
        if (horizontalSpeed == 0)
        {
          if (_character.MovementState.CurrentStateType != typeof(DanglingState))
          {
            _character.MovementState.ChangeState(typeof(IdleState));
            _walkToRunTimeElapsed = 0;
            _isRunning = false;
          }
        }
        else
        {
          _state.IsMovingUpSlope = false;
          _state.IsMovingDownSlope = false;
          if (
            (_state.IsFacingRight && _state.BelowSlopeAngle > 0 && horizontalSpeed > 0) ||
            (!_state.IsFacingRight && _state.BelowSlopeAngle < 0 && horizontalSpeed < 0)
          )
          {
            _state.IsMovingUpSlope = true;
          }

          if (
            (_state.IsFacingRight && _state.BelowSlopeAngle > 0 && horizontalSpeed < 0) ||
            (!_state.IsFacingRight && _state.BelowSlopeAngle < 0 && horizontalSpeed > 0)
          )
          {
            _state.IsMovingDownSlope = true;
          }

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


          if (_settings.WalkToRunTime > 0 && !_isRunning)
          {
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
      }

      float speed = _isRunning ? _settings.RunningSpeed : _settings.WalkingSpeed;
      float movementFactor = _state.IsGrounded ? _controller.Parameters.GroundSpeedFactor : _controller.Parameters.AirSpeedFactor;
      float movementSpeed = horizontalSpeed * speed * _controller.Parameters.SpeedFactor;
      float horizontalMovementForce = Mathf.Lerp(_controller.Speed.x, movementSpeed, Time.deltaTime * movementFactor);

      // add any external forces that may be active right now
      if (Mathf.Abs(_controller.ExternalForce.x) > 0)
      {
        horizontalMovementForce += _controller.ExternalForce.x;
      }

      // we handle friction
      horizontalMovementForce = HandleFriction(horizontalMovementForce);

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

    private void OnEnable()
    {
      // Make sure to setup new events
      OnDisable();

      if (_runInput != null)
      {
        _runInput.action.started += OnRunStarted;
        _runInput.action.canceled += OnRunCanceled;
      }
    }

    private void OnDisable()
    {
      if (_runInput != null)
      {
        _runInput.action.started -= OnRunStarted;
        _runInput.action.canceled -= OnRunCanceled;
      }
    }
  }
}