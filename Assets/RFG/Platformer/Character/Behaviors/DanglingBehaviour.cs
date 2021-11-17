using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Behaviours/Dangling")]
  public class DanglingBehaviour : MonoBehaviour
  {
    private Vector3 _leftOne = new Vector3(-1, 1, 1);
    private Character _character;
    private CharacterControllerState2D _state;
    private CharacterController2D _controller;
    private Transform _transform;
    private SettingsPack _settings;

    #region Unity Methods
    private void Awake()
    {
      _transform = transform;
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
      HandleDangle();
    }
    #endregion

    #region Handlers
    private void HandleDangle()
    {
      if (!CanDangle())
      {
        return;
      }

      // we determine the ray's origin (our character's position + an offset defined in the inspector)
      Vector3 raycastOrigin = Vector3.zero;
      if (_state.IsFacingRight)
      {
        raycastOrigin = _transform.position + _settings.DanglingRaycastOrigin.x * Vector3.right + _settings.DanglingRaycastOrigin.y * _transform.up;
      }
      else
      {
        raycastOrigin = _transform.position - _settings.DanglingRaycastOrigin.x * Vector3.right + _settings.DanglingRaycastOrigin.y * _transform.up;
      }

      // we cast our ray downwards
      RaycastHit2D hit = RFG.Physics2D.RayCast(raycastOrigin, -_transform.up, _settings.DanglingRaycastLength, _controller.PlatformMask | _controller.OneWayPlatformMask | _controller.OneWayMovingPlatformMask, Color.white, true);

      // if the ray didn't hit something, we're dangling
      if (!hit)
      {
        _character.MovementState.ChangeState(typeof(DanglingState));
      }

      // if the ray hit something and we were dangling previously, we go back to Idle
      if (hit && _character.MovementState.IsInState(typeof(DanglingState)))
      {
        _character.MovementState.ChangeState(typeof(IdleState));
      }
    }

    private bool CanDangle()
    {
      return _settings.CanDangle && _character.IsIdle && _character.IsGrounded;
    }
    #endregion
  }
}
