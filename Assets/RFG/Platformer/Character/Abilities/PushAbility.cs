using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Push")]
  public class PushAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private SettingsPack _settings;
    private bool _collidingWithPushable = false;
    private Vector3 _raycastDirection;
    private Vector3 _raycastOrigin;


    #region Unity Methods
    private void Awake()
    {
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _settings = _character.SettingsPack;
    }

    private void Start()
    {
      _state = _controller.State;
    }

    private void Update()
    {
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

      if (_character.IsGrounded && _collidingWithPushable && !_character.IsPushing && !_character.IsInAirMovementState && !_character.IsInCrouchMovementState)
      {
        _character.MovementState.ChangeState(typeof(PushingState));
      }
      else if (!_collidingWithPushable && _character.IsPushing)
      {
        _character.MovementState.ChangeState(typeof(IdleState));
      }
    }
  }
}