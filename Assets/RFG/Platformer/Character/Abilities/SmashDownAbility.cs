
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Smash Down")]
  public class SmashDownAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private CharacterController2D _controller;
    private InputActionReference _smashDownInput;
    private bool _pointerOverUi = false;
    private bool _smashingInAir = false;

    #region Unity Methods
    private void Awake()
    {
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _smashDownInput = _character.InputPack.SmashDownInput;
    }

    private void Update()
    {
      _pointerOverUi = EventSystem.current.IsPointerOverGameObject();
    }

    private void LateUpdate()
    {
      if (_smashingInAir && _character.IsGrounded)
      {
        HandleSmashDownCollision();
      }
    }
    #endregion

    public void ChangeToSmashDownPerform()
    {
      bool changed = _character.MovementState.ChangeState(typeof(SmashDownPerformedState));
      if (_smashingInAir)
      {
        _controller.SetVerticalForce(-_character.SettingsPack.SmashDownInAirSpeed);
      }
    }

    public void HandleSmashDownCollision()
    {
      _controller.SetForce(Vector2.zero);
      _controller.GravityActive(true);
      _character.EnableAllInput(true);
      _character.MovementState.ChangeState(typeof(SmashDownCollidedState));
      _smashingInAir = false;
    }

    private void OnSmashDownStarted(InputAction.CallbackContext ctx)
    {
      if (!_pointerOverUi)
      {
        if (_character.IsInAirMovementState && !_character.SettingsPack.CanSmashDownInAir)
        {
          return;
        }
        _smashingInAir = _character.IsInAirMovementState && _character.SettingsPack.CanSmashDownInAir;
        bool changed = _character.MovementState.ChangeState(typeof(SmashDownStartedState));
        if (changed)
        {
          _controller.SetForce(Vector2.zero);
          _controller.GravityActive(false);
          _character.EnableAllInput(false);
        }
      }
    }

    private void OnStateTypeChange(Type prevType, Type currentType)
    {
      bool wasPerforming = prevType == typeof(SmashDownPerformedState) && currentType != typeof(SmashDownCollidedState);
      if (wasPerforming)
      {
        _controller.GravityActive(true);
        _character.EnableAllInput(true);
        _smashingInAir = false;
      }
    }

    private void OnEnable()
    {
      if (_smashDownInput != null)
      {
        _smashDownInput.action.started += OnSmashDownStarted;
      }
      _character.MovementState.OnStateTypeChange += OnStateTypeChange;
    }

    private void OnDisable()
    {
      if (_smashDownInput != null)
      {
        _smashDownInput.action.started -= OnSmashDownStarted;
      }
      _character.MovementState.OnStateTypeChange -= OnStateTypeChange;
    }

  }
}