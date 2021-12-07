using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Dash")]
  public class DashAbility : MonoBehaviour, IAbility
  {
    public Aim Aim;
    private StateCharacterContext _context;
    private Character _character;
    private PlayerInput _playerInput;
    private Transform _transform;
    private CharacterController2D _controller;
    private Animator _animator;
    private CharacterControllerState2D _state;
    private InputAction _movement;
    private InputAction _dashInput;
    private SettingsPack _settings;
    private Vector2 _dashDirection;
    private float _slopeAngleSave = 0f;
    private float _distanceTraveled = 0f;
    private bool _shouldKeepDashing = true;
    private Vector3 _initialPosition;
    private float _cooldownTimestamp;
    private IEnumerator _dashCoroutine;
    private float _lastDashAt = 0f;
    private int _numberOfDashesLeft = 2;
    private bool _hasDashing;

    #region Unity Methods
    private void Awake()
    {
      _transform = transform;
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
      _context = _character.Context;
      _animator = _context.animator;
      _controller = _context.controller;
      _state = _context.controller.State;
      _movement = _playerInput.actions["Movement"];
      _dashInput = _playerInput.actions["Dash"];
      _settings = _context.settingsPack;
      _hasDashing = _character.MovementState.HasState(typeof(DashingState));

      // Setup events
      OnEnable();

      // Setup ability
      _cooldownTimestamp = 0;
      _numberOfDashesLeft = _settings.TotalDashes;
      Aim.Init();
    }

    private void LateUpdate()
    {
      if (Time.timeScale == 0f)
      {
        return;
      }
      if (_character.MovementState.CurrentStateType == typeof(DashingState))
      {
        _controller.GravityActive(false);
      }
      HandleAmountOfDashesLeft();
    }

    private void OnEnable()
    {
      if (_dashInput != null)
      {
        _dashInput.started += OnDashStarted;
      }
    }

    private void OnDisable()
    {
      if (_dashInput != null)
      {
        _dashInput.started -= OnDashStarted;
      }
    }
    #endregion

    #region Handlers
    private void StartDash()
    {
      if (_cooldownTimestamp > Time.time)
      {
        return;
      }

      if (_numberOfDashesLeft <= 0)
      {
        return;
      }

      _character.MovementState.ChangeState(typeof(DashingState));

      _cooldownTimestamp = Time.time + _settings.Cooldown;
      _distanceTraveled = 0f;
      _shouldKeepDashing = true;
      _initialPosition = _transform.position;
      _lastDashAt = Time.time;

      _numberOfDashesLeft--;

      _slopeAngleSave = _controller.Parameters.MaxSlopeAngle;
      _controller.Parameters.MaxSlopeAngle = 0;

      ComputerDashDirection();
      CheckFlipCharacter();

      _dashCoroutine = Dash();
      StartCoroutine(_dashCoroutine);
    }

    private void ComputerDashDirection()
    {
      Aim.PrimaryMovement = _movement.ReadValue<Vector2>();
      Aim.CurrentPosition = _transform.position;
      _dashDirection = Aim.GetCurrentAim();

      CheckAutoCorrectTrajectory();

      if (_dashDirection.magnitude < _settings.MinInputThreshold)
      {
        _dashDirection = _state.IsFacingRight ? Vector2.right : Vector2.left;
      }
      else
      {
        _dashDirection = _dashDirection.normalized;
      }
    }

    private void CheckAutoCorrectTrajectory()
    {
      if (_state.IsCollidingBelow && _dashDirection.y < 0f)
      {
        _dashDirection.y = 0f;
      }
    }

    private void CheckFlipCharacter()
    {
      if (Mathf.Abs(_dashDirection.x) > 0.05f)
      {
        if (_state.IsFacingRight != _dashDirection.x > 0f)
        {
          _controller.Flip();
        }
      }
    }

    private IEnumerator Dash()
    {
      yield return new WaitForEndOfFrame();
      bool effectSwitch = false;
      while (_distanceTraveled < _settings.DashDistance && _shouldKeepDashing && !_state.TouchingLevelBounds && _character.MovementState.CurrentStateType == typeof(DashingState))
      {
        _distanceTraveled = Vector3.Distance(_initialPosition, _transform.position);

        if (effectSwitch)
        {
          _transform.SpawnFromPool(_settings.DashEffects, _transform);
        }
        effectSwitch = !effectSwitch;

        if ((_state.IsCollidingLeft && _dashDirection.x < 0f)
          || (_state.IsCollidingRight && _dashDirection.x > 0f)
          || (_state.IsCollidingAbove && _dashDirection.y > 0f)
          || (_state.IsCollidingBelow && _dashDirection.y < 0f))
        {
          _shouldKeepDashing = false;
          _controller.SetForce(Vector2.zero);
        }
        else
        {
          _controller.GravityActive(false);
          _controller.SetForce(_dashDirection * _settings.DashForce);
        }
        yield return null;
      }
      StopDash();
    }

    private void StopDash()
    {
      if (_dashCoroutine != null)
      {
        StopCoroutine(_dashCoroutine);
      }

      _controller.DefaultParameters.MaxSlopeAngle = _slopeAngleSave;
      _controller.Parameters.MaxSlopeAngle = _slopeAngleSave;
      _controller.GravityActive(true);

      _controller.SetForce(Vector2.zero);

      if (_character.MovementState.CurrentStateType == typeof(DashingState))
      {
        if (_state.IsGrounded)
        {
          _character.MovementState.ChangeState(typeof(IdleState), true);
        }
        else
        {
          _character.MovementState.RestorePreviousState(true);
        }
      }
    }

    public void SetNumberOfDashesLeft(int numberLeft)
    {
      _numberOfDashesLeft = numberLeft;
    }

    private void HandleAmountOfDashesLeft()
    {
      if (Time.time - _lastDashAt < _settings.Cooldown)
      {
        return;
      }

      if (_state.IsGrounded)
      {
        SetNumberOfDashesLeft(_settings.TotalDashes);
      }
    }
    #endregion

    #region Events
    public void OnDashStarted(InputAction.CallbackContext ctx)
    {
      if (!_hasDashing || (!_settings.SwimCanDash && _character.IsSwimming))
        return;

      StartDash();
    }
    #endregion

  }
}