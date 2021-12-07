using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Push")]
  public class PushAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private PlayerInput _playerInput;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private SettingsPack _settings;
    private InputAction _movement;
    private bool _collidingWithPushable = false;
    private Vector3 _raycastDirection;
    private Vector3 _raycastOrigin;
    private float _horizontalSpeed = 0f;
    private bool _isPushing = false;


    #region Unity Methods
    private void Awake()
    {
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _playerInput = GetComponent<PlayerInput>();
      _settings = _character.SettingsPack;
      _movement = _playerInput.actions["Movement"];
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
      HandlePush();
    }
    #endregion

    private void HandlePush()
    {
      _collidingWithPushable = false;
      _raycastDirection = transform.right;
      _raycastOrigin = _controller.ColliderCenterPosition + _raycastDirection * (_controller.Width() / 2);

      RaycastHit2D hit = RFG.Physics2D.RayCast(_raycastOrigin, _raycastDirection, _settings.PushDetectionRaycastLength, _controller.PlatformMask, Color.white, true);

      if (hit)
      {
        if (hit.collider.gameObject.GetComponentNoAlloc<Pushable>() != null)
        {
          _collidingWithPushable = true;
        }
      }

      if (_character.IsGrounded && _collidingWithPushable && !_character.IsInAirMovementState && !_character.IsInCrouchMovementState)
      {
        _isPushing = true;
      }
      else if (!_collidingWithPushable && _isPushing)
      {
        _isPushing = false;
        _character.MovementState.ChangeState(typeof(IdleState));
      }

      if (_isPushing)
      {
        _horizontalSpeed = _movement.ReadValue<Vector2>().x;
        if (_horizontalSpeed == 0)
        {
          _character.MovementState.ChangeState(typeof(PushingIdleState));
        }
        else
        {
          _character.MovementState.ChangeState(typeof(PushingState));
        }
      }

    }
  }
}