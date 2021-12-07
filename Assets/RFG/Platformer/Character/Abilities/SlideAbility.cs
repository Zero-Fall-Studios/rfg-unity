using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Slide")]
  public class SlideAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private PlayerInput _playerInput;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private InputAction _movement;
    private SettingsPack _settings;
    private InputAction _slideInput;
    private float _horizontalSpeed = 0f;
    private bool _isSliding;
    private bool _isSlidingCooldown;
    private float _slideTimeElapsed;
    private float _slideCooldownTimeElapsed;

    #region Unity Methods
    private void Awake()
    {
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _playerInput = GetComponent<PlayerInput>();
      _movement = _playerInput.actions["Movement"];
      _slideInput = _playerInput.actions["Slide"];
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
      if (_isSlidingCooldown)
      {
        if (_slideCooldownTimeElapsed > _settings.SlideCooldownTime)
        {
          _isSlidingCooldown = false;
          _slideCooldownTimeElapsed = 0;
        }
        _slideCooldownTimeElapsed += Time.deltaTime;
        return;
      }
      if (_isSliding)
      {
        if (_slideTimeElapsed > _settings.SlideTime)
        {
          _slideTimeElapsed = 0;
          _isSliding = false;
          _isSlidingCooldown = true;
          _character.EnableAllInput(true);
          _character.MovementState.ChangeState(typeof(IdleState));
          return;
        }
        _slideTimeElapsed += Time.deltaTime;
        MoveCharacter();
      }
    }

    private void OnEnable()
    {
      if (_slideInput != null)
      {
        _slideInput.started += OnSlideStarted;
      }
    }

    private void OnDisable()
    {
      if (_slideInput != null)
      {
        _slideInput.started -= OnSlideStarted;
      }
    }
    #endregion

    #region Handlers
    private void HandleSlide()
    {
      if (!CanSlide())
      {
        return;
      }
      _isSliding = true;
      _horizontalSpeed = _movement.ReadValue<Vector2>().x;
      // _character.EnableAllInput(false);
      _character.MovementState.ChangeState(typeof(SlidingState));
    }

    private bool CanSlide()
    {
      return _character.IsAlive && _character.IsInGroundMovementState && !_character.IsIdle && !_isSliding && !_isSlidingCooldown && !_character.IsSwimming;
    }

    private void MoveCharacter()
    {


      float speed = _settings.SlideSpeed;
      // float speed = _settings.WalkingSpeed;
      float movementFactor = _controller.Parameters.GroundSpeedFactor;
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
    #endregion

    #region Events
    private void OnSlideStarted(InputAction.CallbackContext obj)
    {
      HandleSlide();
    }
    #endregion
  }
}