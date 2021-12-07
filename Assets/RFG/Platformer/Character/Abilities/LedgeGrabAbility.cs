using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Ledge Grab")]
  public class LedgeGrabAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private PlayerInput _playerInput;
    private InputAction _movement;
    private SettingsPack _settings;
    private float _ledgeHangingStartedTimestamp;
    private Ledge _ledge;
    private MovementAbility _movementAbility;
    private JumpAbility _jumpAbility;

    #region Unity Methods
    private void Awake()
    {
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _movementAbility = GetComponent<MovementAbility>();
      _jumpAbility = GetComponent<JumpAbility>();
      _playerInput = GetComponent<PlayerInput>();
      _movement = _playerInput.actions["Movement"];
      _settings = _character.SettingsPack;
    }

    private void Start()
    {
      _state = _controller.State;
    }

    private void Update()
    {
      if (Time.timeScale == 0f)
      {
        return;
      }
      HandleLedgeGrab();
    }
    #endregion

    #region Handlers
    private void HandleLedgeGrab()
    {
      if (!_character.IsLedgeGrabbing)
      {
        return;
      }

      if (Time.time - _ledgeHangingStartedTimestamp < _settings.MinimumHangingTime)
      {
        return;
      }

      float verticalSpeed = _movement.ReadValue<Vector2>().y;

      if (verticalSpeed > _settings.Threshold.y)
      {
        StartCoroutine(Climb());
      }
      else if (verticalSpeed < -_settings.Threshold.y)
      {
        DetachFromLedge();
      }
    }

    private IEnumerator Climb()
    {
      _character.MovementState.ChangeState(typeof(LedgeClimbingState));

      _character.EnableAllInput(false);

      yield return new WaitForSeconds(_settings.ClimbingAnimationDuration);

      _character.EnableAllInput(true);

      // Teleport to ledge
      _character.transform.position = _ledge.transform.position + _ledge.ClimbOffset;

      _character.MovementState.ChangeState(typeof(IdleState));

      DetachFromLedge();
    }

    private void DetachFromLedge()
    {
      _ledge = null;
      // TODO _character.CanFlip = true;
      _character.EnableAllAbilities(true);
      _controller.CollisionsOn();
      _controller.GravityActive(true);
      _character.MovementState.ChangeState(typeof(FallingState));
    }
    #endregion

    #region Events
    public void StartGrabbingLedge(Ledge ledge)
    {
      if (
          (_state.IsFacingRight && ledge.LedgeGrabDirection == LedgeGrabDirection.Left) ||
          (!_state.IsFacingRight && ledge.LedgeGrabDirection == LedgeGrabDirection.Right)
        )
      {
        return;
      }

      _ledgeHangingStartedTimestamp = Time.time;
      _ledge = ledge;
      _controller.CollisionsOff();
      _character.MovementState.ChangeState(typeof(LedgeGrabState));

      _controller.SetForce(Vector2.zero);
      _controller.GravityActive(false);
      if (_jumpAbility != null)
      {
        _jumpAbility.SetNumberOfJumpsLeft();
      }
      _character.EnableAllAbilities(false, this);
      // TODO _character.CanFlip = false;
      _controller.transform.position = _ledge.transform.position + _ledge.HangOffset;
    }
    #endregion

  }
}