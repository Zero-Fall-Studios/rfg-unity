using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Stairs")]
  public class StairsAbility : MonoBehaviour, IAbility
  {
    [Header("Status")]

    /// true if the character is on stairs this frame, false otherwise
    [ReadOnly]
    [Tooltip("true if the character is on stairs this frame, false otherwise")]
    public bool OnStairs = false;

    /// true if there are stairs below our character
    [ReadOnly]
    [Tooltip("true if there are stairs below our character")]
    public bool StairsBelow = false;

    /// true if there are stairs ahead of our character
    [ReadOnly]
    [Tooltip("true if there are stairs ahead of our character")]
    public bool StairsAhead = false;

    private bool _stairsInputUp = false;
    private bool _stairsInputDown = false;
    private float _stairsAheadAngle;
    private float _stairsBelowAngle;
    private Vector3 _raycastOrigin;
    private Vector3 _raycastDirection;
    private Collider2D _goingDownEntryBoundsCollider;
    private float _goingDownEntryAt;
    private Character _character;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private PlayerInput _playerInput;
    private InputAction _movement;
    private SettingsPack _settings;

    private void Awake()
    {
      _character = GetComponent<Character>();
      _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
      _controller = _character.Context.controller;
      _state = _character.Context.controller.State;
      _movement = _playerInput.actions["Movement"];
      _settings = _character.Context.settingsPack;
    }

    private void Update()
    {
      if (Time.timeScale == 0f)
      {
        return;
      }
      float verticalInput = _movement.ReadValue<Vector2>().y;
      _stairsInputUp = (verticalInput > _settings.Threshold.y);
      _stairsInputDown = (verticalInput < -_settings.Threshold.y);
      HandleEntryBounds();
      CheckIfStairsAhead();
      CheckIfStairsBelow();
      CheckIfOnStairways();
      HandleStairsAuthorization();
    }

    /// <summary>
    /// Sets the character in looking up state and asks the camera to look up
    /// </summary>
    private void HandleStairsAuthorization()
    {
      bool authorize = true;
      if (
           _state.IsGrounded
        && _character.MovementState.CurrentStateType != typeof(JumpingState)
        && _character.MovementState.CurrentStateType != typeof(DashingState)
      )
      {
        // If there are stairs ahead and you're not on stairs
        if (StairsAhead && !OnStairs)
        {
          // if you have no input to go up the stairs
          if (!_stairsInputUp)
          {
            authorize = false;
          }
          // or if the stairs ahead angle is too much, then dont turn on collisions with stairs
          if (_stairsAheadAngle < 0 || _stairsAheadAngle >= 90f)
          {
            authorize = false;
          }
        }

        // If there are stairs below and you're not on stairs and the One Way Platform mask is the one you are standing on
        if (StairsBelow && !OnStairs && _controller.StandingOn != null && _controller.OneWayPlatformMask.Contains(_controller.StandingOn.layer))
        {
          // If you have input going down
          if (_stairsInputDown)
          {
            // and the angle is good to go
            if (_stairsBelowAngle > 0 && _stairsBelowAngle <= 90f)
            {
              // Then jump through the one way platform
              // _state.IsJumping = true;

              // Record what collider we were standing on
              _goingDownEntryBoundsCollider = _controller.StandingOnCollider;

              // Remove one way platforms in the platform mask
              _controller.PlatformMask -= _controller.OneWayPlatformMask;
              _controller.PlatformMask -= _controller.OneWayMovingPlatformMask;

              // Add stairs to the platform mask
              _controller.PlatformMask |= _controller.StairsMask;

              // Record the time when you went through the one way platform
              _goingDownEntryAt = Time.time;
            }
          }
        }
      }

      if (authorize)
      {
        _controller.CollisionsOnWithStairs();
      }
      else
      {
        _controller.CollisionsOffWithStairs();
      }
    }

    /// <summary>
    /// Restores collisions once we're out of the stairs and if enough time has passed
    /// </summary>
    private void HandleEntryBounds()
    {
      // If we weren't standing on any collider then return
      if (_goingDownEntryBoundsCollider == null)
      {
        return;
      }

      // If the time hasn't passed yet to exceed the StairsBelow lock time then return
      if (Time.time - _goingDownEntryAt < _settings.StairsBelowLockTime)
      {
        return;
      }

      // Getting here means we have a collider we were standing on, we have passed the lock time
      // and the collider doesn't contain the controllers collider bottom position
      // then turn back on collisions
      if (!_goingDownEntryBoundsCollider.bounds.Contains(_controller.ColliderBottomPosition))
      {
        _controller.CollisionsOn();
        _goingDownEntryBoundsCollider = null;
      }
    }

    /// <summary>
    /// Casts a ray to see if there are stairs in front of the character
    /// </summary>
    private void CheckIfStairsAhead()
    {
      StairsAhead = false;

      if (_state.IsFacingRight)
      {
        _raycastOrigin = transform.position + _settings.StairsAheadDetectionRaycastOrigin.x * Vector3.right + _settings.StairsAheadDetectionRaycastOrigin.y * transform.up;
        _raycastDirection = Vector3.right;
      }
      else
      {
        _raycastOrigin = transform.position - _settings.StairsAheadDetectionRaycastOrigin.x * Vector3.right + _settings.StairsAheadDetectionRaycastOrigin.y * transform.up;
        _raycastDirection = -Vector3.right;
      }

      // we cast our ray in front of us
      RaycastHit2D hit = RFG.Physics2D.RayCast(_raycastOrigin, _raycastDirection, _settings.StairsAheadDetectionRaycastLength, _controller.StairsMask, Color.yellow, true);

      if (hit)
      {
        _stairsAheadAngle = Mathf.Abs(Vector2.Angle(hit.normal, transform.up));
        StairsAhead = true;
      }
    }

    /// <summary>
    /// Casts a ray to see if there are stairs below the character
    /// </summary>
    private void CheckIfStairsBelow()
    {
      StairsBelow = false;

      _raycastOrigin = _controller.BoundsCenter;
      if (_state.IsFacingRight)
      {
        _raycastOrigin = _controller.ColliderBottomPosition + _settings.StairsBelowDetectionRaycastOrigin.x * Vector3.right + _settings.StairsBelowDetectionRaycastOrigin.y * transform.up;
      }
      else
      {
        _raycastOrigin = _controller.ColliderBottomPosition - _settings.StairsBelowDetectionRaycastOrigin.x * Vector3.right + _settings.StairsBelowDetectionRaycastOrigin.y * transform.up;
      }

      RaycastHit2D hit = RFG.Physics2D.RayCast(_raycastOrigin, -transform.up, _settings.StairsBelowDetectionRaycastLength, _controller.StairsMask, Color.yellow, true);

      if (hit)
      {
        if (_state.IsFacingRight)
        {
          _stairsBelowAngle = Mathf.Abs(Vector2.Angle(hit.normal, Vector3.right));
        }
        else
        {
          _stairsBelowAngle = Mathf.Abs(Vector2.Angle(hit.normal, -Vector3.right));
        }

        StairsBelow = true;
      }
    }

    /// <summary>
    /// Checks if the character is currently standing on stairs
    /// </summary>
    private void CheckIfOnStairways()
    {
      OnStairs = false;
      if (_controller.StandingOn != null)
      {
        // Are we actually standing on a layer with the Stairs layer mask?
        if (_controller.StairsMask.Contains(_controller.StandingOn.layer))
        {
          OnStairs = true;
        }
      }
    }
  }
}

