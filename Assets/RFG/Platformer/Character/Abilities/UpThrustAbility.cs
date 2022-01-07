using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Up Thrust")]
  public class UpThrustAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private PlayerInput _playerInput;
    private Transform _transform;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private InputAction _movement;
    private InputAction _jumpInput;
    private SettingsPack _settings;

    #region Unity Methods
    private void Awake()
    {
      _transform = transform;
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
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
      HandleUpThrust();
    }
    #endregion

    private void HandleUpThrust()
    {
      if (!CanUpThrust())
      {
        return;
      }

      // Read in vertical input
      float verticalInput = _movement.ReadValue<Vector2>().y;

      if (verticalInput > _settings.Threshold.y)
      {
        Debug.Log("Up Thrust");
        _character.MovementState.ChangeState(typeof(UpThrustState));
      }
      else if (
        verticalInput > -_settings.Threshold.y &&
        verticalInput <= _settings.Threshold.y &&
        _character.MovementState.CurrentStateType == typeof(UpThrustState)
      )
      {
        _character.MovementState.RestorePreviousState();
      }
    }

    private bool CanUpThrust()
    {
      return _character.IsInAirMovementState || _character.MovementState.CurrentStateType == typeof(UpThrustState);
    }
  }
}
