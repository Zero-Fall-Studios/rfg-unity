using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Jump")]
  public class JumpAbility : MonoBehaviour, IAbility
  {
    [HideInInspector]
    private StateCharacterContext _context;
    private Character _character;
    private Transform _transform;
    private CharacterController2D _controller;
    private Animator _animator;
    private CharacterControllerState2D _state;
    private InputActionReference _movement;
    private InputActionReference _jumpInput;
    private SettingsPack _settings;
    private int _numberOfJumpsLeft = 0;
    private float _lastJumpTime = 0f;

    private void Start()
    {
      _transform = transform;
      _character = GetComponent<Character>();
      _context = _character.Context;
      _animator = _context.animator;
      _controller = _context.controller;
      _state = _context.controller.State;
      _movement = _context.inputPack.Movement;
      _jumpInput = _context.inputPack.JumpInput;
      _settings = _context.settingsPack;

      // Setup events
      OnEnable();

      SetNumberOfJumpsLeft();
    }

    private void LateUpdate()
    {
      if (_state.JustGotGrounded)
      {
        _character.MovementState.ChangeState(typeof(LandedState));
        SetNumberOfJumpsLeft();
      }
    }

    public void SetNumberOfJumpsLeft()
    {
      _numberOfJumpsLeft = _settings.NumberOfJumps;
      if (_character.MovementState.HasState(typeof(DoubleJumpState)))
      {
        if (_numberOfJumpsLeft == 1)
        {
          _numberOfJumpsLeft = 2;
        }
      }
    }

    private bool EvaluateJumpConditions()
    {
      if (_settings.Restrictions == JumpRestrictions.CanJumpAnywhere)
      {
        return true;
      }

      if (_settings.Restrictions == JumpRestrictions.CanJumpOnGround && _numberOfJumpsLeft <= 0)
      {
        return false;
      }

      if (_character.MovementState.CurrentStateType == typeof(WallClingingState))
      {
        return false;
      }

      float _verticalInput = _movement.action.ReadValue<Vector2>().y;

      // if the character is standing on a one way platform and is also pressing the down button,
      if (_verticalInput < -_settings.JumpThreshold.y && _state.IsGrounded)
      {
        if (JumpDownFromOneWayPlatform())
        {
          return false;
        }
      }

      // if the character is standing on a moving platform and not pressing the down button,
      if (_state.IsGrounded)
      {
        JumpFromMovingPlatform();
      }

      return true;
    }

    private void JumpStart()
    {
      if (EvaluateJumpConditions())
      {
        _character.MovementState.ChangeState(typeof(JumpingState));
        _numberOfJumpsLeft--;

        _controller.GravityActive(true);
        _controller.CollisionsOn();

        _lastJumpTime = Time.time;
        _state.IsJumping = true;

        _controller.SetVerticalForce(Mathf.Sqrt(2f * _settings.JumpHeight * Mathf.Abs(_controller.Parameters.Gravity)));
      }
    }

    private void JumpStop()
    {
      if (_settings.JumpIsProportionalToThePressTime)
      {
        bool hasMinAirTime = Time.time - _lastJumpTime >= _settings.JumpMinAirTime;
        bool speedGreaterThanGravity = _controller.Speed.y > Mathf.Sqrt(Mathf.Abs(_controller.Parameters.Gravity));
        if (hasMinAirTime && speedGreaterThanGravity)
        {
          _lastJumpTime = 0f;
          if (_settings.JumpReleaseForceFactor == 0f)
          {
            _controller.SetVerticalForce(0f);
          }
          else
          {
            _controller.AddVerticalForce(-_controller.Speed.y / _settings.JumpReleaseForceFactor);
          }
        }
      }
      _character.MovementState.ChangeState(typeof(FallingState));
      _state.IsJumping = false;
    }

    /// <summary>
    /// Handles jumping down from a one way platform.
    /// </summary>
    protected virtual bool JumpDownFromOneWayPlatform()
    {
      if (!_settings.CanJumpDownOneWayPlatforms)
      {
        return false;
      }
      if (_controller.OneWayPlatformMask.Contains(_controller.StandingOn.layer)
        || _controller.OneWayMovingPlatformMask.Contains(_controller.StandingOn.layer)
        || _controller.StairsMask.Contains(_controller.StandingOn.layer))
      {
        _state.IsJumping = true;
        _character.MovementState.ChangeState(typeof(FallingState));
        // we turn the boxcollider off for a few milliseconds, so the character doesn't get stuck mid platform
        StartCoroutine(_controller.DisableCollisionsWithOneWayPlatforms(_settings.OneWayPlatformsJumpCollisionOffDuration));
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Handles jumping from a moving platform.
    /// </summary>
    protected virtual void JumpFromMovingPlatform()
    {
      if (_controller.MovingPlatformMask.Contains(_controller.StandingOn.layer)
        || _controller.OneWayMovingPlatformMask.Contains(_controller.StandingOn.layer))
      {
        // we turn the boxcollider off for a few milliseconds, so the character doesn't get stuck mid air
        StartCoroutine(_controller.DisableCollisionsWithMovingPlatforms(_settings.MovingPlatformsJumpCollisionOffDuration));
      }
    }

    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
      JumpStart();
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
      JumpStop();
    }

    private void OnEnable()
    {
      // Make sure to setup new events
      OnDisable();

      if (_jumpInput != null)
      {
        _jumpInput.action.Enable();
        _jumpInput.action.started += OnJumpStarted;
        _jumpInput.action.canceled += OnJumpCanceled;
      }
    }

    private void OnDisable()
    {
      if (_jumpInput != null)
      {
        _jumpInput.action.Disable();
        _jumpInput.action.started -= OnJumpStarted;
        _jumpInput.action.canceled -= OnJumpCanceled;
      }
    }

  }
}