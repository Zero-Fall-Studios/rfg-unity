using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Jump")]
  public class JumpAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private PlayerInput _playerInput;
    private Transform _transform;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private InputAction _movement;
    private InputAction _jumpInput;
    private SettingsPack _settings;
    private int _numberOfJumpsLeft = 0;
    private float _lastJumpTime = 0f;
    private float _jumpButtonPressTime = 0f;
    private bool _jumpButtonPressed = false;
    private bool _jumpButtonReleased = false;

    #region Unity Methods
    private void Awake()
    {
      _transform = transform;
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _playerInput = GetComponent<PlayerInput>();
      _movement = _playerInput.actions["Movement"];
      _jumpInput = _playerInput.actions["Jump"];
      _settings = _character.SettingsPack;
    }

    private void Start()
    {
      _state = _controller.State;
      SetNumberOfJumpsLeft();
    }

    private void Update()
    {
      if (Time.timeScale == 0f)
      {
        return;
      }
      if (_jumpButtonPressTime != 0f)
      {
        JumpStop();
      }
    }

    private void LateUpdate()
    {
      if (Time.timeScale == 0f)
      {
        return;
      }
      if (!_character.IsIdle && _state.JustGotGrounded && !_character.IsSwimming)
      {
        _character.MovementState.ChangeState(typeof(LandedState));
        SetNumberOfJumpsLeft();
      }
    }

    private void OnEnable()
    {
      if (_jumpInput != null)
      {
        _jumpInput.started += OnJumpStarted;
        _jumpInput.canceled += OnJumpCanceled;
      }
    }

    private void OnDisable()
    {
      if (_jumpInput != null)
      {
        _jumpInput.started -= OnJumpStarted;
        _jumpInput.canceled -= OnJumpCanceled;
      }
    }
    #endregion

    #region Handlers
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

      if (!_character.IsLadderCliming && _settings.Restrictions == JumpRestrictions.CanJumpOnGround && _numberOfJumpsLeft <= 0)
      {
        return false;
      }

      if (_character.MovementState.IsInState(typeof(WallClingingState), typeof(SwimmingState)))
      {
        return false;
      }

      float _verticalInput = _movement.ReadValue<Vector2>().y;

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
      if (!EvaluateJumpConditions())
      {
        return;
      }

      float _horizontalInput = _movement.ReadValue<Vector2>().x;

      if ((!_settings.CanJumpFlip) || (_horizontalInput > -_settings.JumpThreshold.x && _horizontalInput < _settings.JumpThreshold.x))
      {
        _character.MovementState.ChangeState(typeof(JumpingState));
      }
      else
      {
        _character.MovementState.ChangeState(typeof(JumpingFlipState));
      }

      _numberOfJumpsLeft--;

      _controller.GravityActive(true);
      _controller.CollisionsOn();

      _lastJumpTime = Time.time;
      _jumpButtonPressTime = Time.time;
      _jumpButtonReleased = false;
      _jumpButtonPressed = true;
      _state.IsJumping = true;

      _controller.SetVerticalForce(Mathf.Sqrt(2f * _settings.JumpHeight * Mathf.Abs(_controller.Parameters.Gravity)));

    }

    private void JumpStop()
    {
      bool hasMinAirTime = Time.time - _lastJumpTime >= _settings.JumpMinAirTime;
      bool speedGreaterThanGravity = _controller.Speed.y > Mathf.Sqrt(Mathf.Abs(_controller.Parameters.Gravity));
      if (hasMinAirTime && speedGreaterThanGravity && _jumpButtonReleased && !_jumpButtonPressed)
      {
        _jumpButtonReleased = false;
        if (_settings.JumpIsProportionalToThePressTime)
        {
          _jumpButtonPressTime = 0f;
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
      _state.IsJumping = false;
    }

    private bool JumpDownFromOneWayPlatform()
    {
      if (
        _settings.CanJumpDownOneWayPlatforms
        &&
        (
             _controller.OneWayPlatformMask.Contains(_controller.StandingOn.layer)
          || _controller.OneWayMovingPlatformMask.Contains(_controller.StandingOn.layer)
          || _controller.StairsMask.Contains(_controller.StandingOn.layer)
        )
      )
      {
        _state.IsJumping = true;
        // we turn the boxcollider off for a few milliseconds, so the character doesn't get stuck mid platform
        StartCoroutine(_controller.DisableCollisionsWithOneWayPlatforms(_settings.OneWayPlatformsJumpCollisionOffDuration));
        return true;
      }
      return false;
    }

    private void JumpFromMovingPlatform()
    {
      if (_controller.MovingPlatformMask.Contains(_controller.StandingOn.layer)
        || _controller.OneWayMovingPlatformMask.Contains(_controller.StandingOn.layer))
      {
        // we turn the boxcollider off for a few milliseconds, so the character doesn't get stuck mid air
        StartCoroutine(_controller.DisableCollisionsWithMovingPlatforms(_settings.MovingPlatformsJumpCollisionOffDuration));
      }
    }
    #endregion

    #region Events
    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
      JumpStart();
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
      _jumpButtonPressed = false;
      _jumpButtonReleased = true;
    }
    #endregion

  }
}