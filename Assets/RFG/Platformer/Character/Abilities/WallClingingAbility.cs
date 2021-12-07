using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Wall Clinging")]
  public class WallClingingAbility : MonoBehaviour, IAbility
  {
    private Transform _transform;
    private Character _character;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private PlayerInput _playerInput;
    private InputAction _movement;
    private SettingsPack _settings;
    private bool _hasWallCling = false;

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
      _hasWallCling = _character.MovementState.HasState(typeof(WallClingingState));
    }

    private void Update()
    {
      if (Time.timeScale == 0f)
      {
        return;
      }
      if (!_hasWallCling || (!_settings.SwimCanWallCling && _character.IsSwimming))
        return;

      WallCling();
    }

    private void WallCling()
    {
      if (_state.IsGrounded || _controller.Speed.y >= 0)
      {
        _controller.SlowFall(0f);
        return;
      }

      Vector2 _movementVector = _movement.ReadValue<Vector2>();

      float _horizontalInput = _movementVector.x;
      float _verticalInput = _movementVector.y;

      bool isClingingLeft = _state.IsCollidingLeft && _horizontalInput <= -_settings.WallClingingInputThreshold;
      bool isClingingRight = _state.IsCollidingRight && _horizontalInput >= _settings.WallClingingInputThreshold;

      // If we are wall clinging, then change the state
      if (isClingingLeft || isClingingRight)
      {
        // Slow the fall speed
        _controller.SlowFall(_settings.WallClingingSlowFactor);
        _state.IsWallClinging = true;
      }
      else
      {
        _state.IsWallClinging = false;
      }

      // If we are in a wall clinging state then make sure we are still wall clinging
      // if not then go back to idle
      if (_state.IsWallClinging)
      {
        bool shouldExit = false;
        if (_state.IsGrounded || _controller.Speed.y >= 0)
        {
          // If the character is grounded or moving up
          shouldExit = true;
        }

        Vector3 raycastOrigin = _transform.position;
        Vector3 raycastDirection;
        Vector3 right = _transform.right;

        if (isClingingRight && !_state.IsFacingRight)
        {
          right = -right;
        }
        else if (isClingingLeft && _state.IsFacingRight)
        {
          right = -right;
        }

        raycastOrigin = raycastOrigin + right * _controller.Width() / 2 + _transform.up * _settings.RaycastVerticalOffset;
        raycastDirection = right - _transform.up;

        LayerMask mask = _controller.PlatformMask & (~_controller.OneWayPlatformMask | ~_controller.OneWayMovingPlatformMask);

        RaycastHit2D hit = RFG.Physics2D.RayCast(raycastOrigin, raycastDirection, _settings.WallClingingTolerance, mask, Color.red, true);

        if (isClingingRight)
        {
          if (!hit || _horizontalInput <= _settings.WallClingingInputThreshold)
          {
            shouldExit = true;
          }
        }
        else
        {
          if (!hit || _horizontalInput >= -_settings.WallClingingInputThreshold)
          {
            shouldExit = true;
          }
        }
        if (shouldExit)
        {
          _controller.SlowFall(0f);
          _character.MovementState.ChangeState(typeof(IdleState));
        }
        else
        {
          _character.MovementState.ChangeState(typeof(WallClingingState));
        }
      }

      if (!_state.IsWallClinging)
      {
        _controller.SlowFall(0f);
      }
    }
  }
}