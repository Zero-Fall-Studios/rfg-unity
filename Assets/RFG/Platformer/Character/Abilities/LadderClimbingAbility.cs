using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Ladder Climbing")]
  public class LadderClimbingAbility : MonoBehaviour
  {
    public bool LadderColliding
    {
      get
      {
        return (_colliders.Count > 0);
      }
    }

    [field: SerializeField] private bool ForceAnchorToGroundOnExit { get; set; } = false;
    [field: SerializeField] private bool ForceRightFacing { get; set; } = false;

    private Ladder CurrentLadder { get; set; }
    private Ladder HighestLadder { get; set; }
    private Ladder LowestLadder { get; set; }
    private Vector2 CurrentLadderClimbingSpeed { get; set; }

    private Character _character;
    private PlayerInput _playerInput;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private SettingsPack _settings;
    private List<Collider2D> _colliders;
    private BoxCollider2D _boxCollider;
    private InputAction _movement;
    private float _horizontalSpeed;
    private float _verticalSpeed;
    // private CharacterGravity _characterGravity;

    private float _climbingDownInitialYTranslation = 0.1f;
    private float _ladderTopSkinHeight = 0.01f;

    #region Unity Methods
    private void Awake()
    {
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _playerInput = GetComponent<PlayerInput>();
      _movement = _playerInput.actions["Movement"];
      _settings = _character.SettingsPack;
      _colliders = new List<Collider2D>();
      _boxCollider = GetComponent<BoxCollider2D>();
      CurrentLadderClimbingSpeed = Vector2.zero;
      // _characterGravity = GetComponent<CharacterGravity>();
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
      ComputeClosestLadder();
      HandleLadderClimbing();
    }

    private void OnEnable()
    {
      _character.MovementState.OnStateTypeChange += OnStateTypeChange;
    }

    private void OnDisable()
    {
      _character.MovementState.OnStateTypeChange -= OnStateTypeChange;
    }
    #endregion

    #region Handlers
    private void ComputeClosestLadder()
    {
      CurrentLadder = null;
      HighestLadder = null;
      LowestLadder = null;

      if (_colliders.Count > 0)
      {
        float closestHorizontalDistance = Mathf.Infinity;
        int closestHorizontalIndex = 0;

        float highestPosition = -Mathf.Infinity;
        int highestLadderIndex = 0;

        float lowestPosition = Mathf.Infinity;
        int lowestLadderIndex = 0;

        for (int i = 0; i < _colliders.Count; i++)
        {
          float distance = Mathf.Abs(_colliders[i].bounds.center.x - _controller.BoundsCenter.x);
          float yPosition = _colliders[i].bounds.center.y;

          if (distance < closestHorizontalDistance)
          {
            closestHorizontalIndex = i;
            closestHorizontalDistance = distance;
          }

          if (yPosition > highestPosition)
          {
            highestPosition = yPosition;
            highestLadderIndex = i;
          }

          if (yPosition < lowestPosition)
          {
            lowestPosition = yPosition;
            lowestLadderIndex = i;
          }

        }
        CurrentLadder = _colliders[closestHorizontalIndex].gameObject.GetComponentNoAlloc<Ladder>();
        LowestLadder = _colliders[lowestLadderIndex].gameObject.GetComponentNoAlloc<Ladder>();
        HighestLadder = _colliders[highestLadderIndex].gameObject.GetComponentNoAlloc<Ladder>();
      }
    }

    private void HandleLadderClimbing()
    {
      if (LadderColliding)
      {
        if (_character.IsLadderCliming && _controller.State.IsGrounded)
        {
          GetOffTheLadder();
          return;
        }

        var speed = _movement.ReadValue<Vector2>();
        _verticalSpeed = speed.y;
        _horizontalSpeed = speed.x;

        if (_verticalSpeed > _settings.Threshold.y && !_character.IsLadderCliming && !_character.IsJumping)
        {
          StartClimbing();
        }

        if (_character.IsLadderCliming)
        {
          Climbing();
        }

        if (CurrentLadder == null)
        {
          return;
        }

        if (LowestLadder.LadderPlatform != null)
        {
          if (_character.IsLadderCliming && AboveLadderPlatform() && (LowestLadder == HighestLadder))
          {
            GetOffTheLadder();
            if (ForceAnchorToGroundOnExit)
            {
              _controller.AnchorToGround();
            }
          }

          if (!_character.IsLadderCliming && _verticalSpeed < -_settings.Threshold.y && AboveLadderPlatform() && _controller.State.IsGrounded)
          {
            StartClimbingDown();
          }
        }

      }
      else if (_character.IsLadderCliming)
      {
        GetOffTheLadder();
      }
    }

    private void Climbing()
    {
      if (CurrentLadder.LadderPlatform != null)
      {
        if (!AboveLadderPlatform())
        {
          _controller.CollisionsOn();
        }
      }
      else
      {
        _controller.CollisionsOn();
      }

      if (_verticalSpeed == 0)
      {
        _character.MovementState.ChangeState(typeof(LadderIdleState));
      }
      else
      {
        _character.MovementState.ChangeState(typeof(LadderClimbingState));
      }

      if (CurrentLadder.LadderType == LadderType.Simple)
      {
        _controller.SetVerticalForce(_verticalSpeed * _settings.LadderClimbingSpeed);
        // we set the climbing speed state.
        CurrentLadderClimbingSpeed = Mathf.Abs(_verticalSpeed) * transform.up;
      }

      if (CurrentLadder.LadderType == LadderType.BiDirectional)
      {
        _controller.SetHorizontalForce(_horizontalSpeed * _settings.LadderClimbingSpeed);
        _controller.SetVerticalForce(_verticalSpeed * _settings.LadderClimbingSpeed);
        CurrentLadderClimbingSpeed = Mathf.Abs(_horizontalSpeed) * transform.right;
        CurrentLadderClimbingSpeed += Mathf.Abs(_verticalSpeed) * (Vector2)transform.up;
      }

    }

    private void GetOffTheLadder()
    {
      _character.MovementState.ChangeState(typeof(IdleState));
      CurrentLadderClimbingSpeed = Vector2.zero;
      _controller.GravityActive(true);
      _controller.CollisionsOn();
    }

    public virtual void AddCollidingLadder(Collider2D newCollider)
    {
      _colliders.Add(newCollider);
    }

    public virtual void RemoveCollidingLadder(Collider2D newCollider)
    {
      _colliders.Remove(newCollider);
    }

    private void StartClimbing()
    {
      if (CurrentLadder.LadderPlatform != null)
      {
        if (AboveLadderPlatform() && (LowestLadder == HighestLadder))
        {
          return;
        }
      }

      // we rotate our character if requested
      //   if (ForceRightFacing)
      //   {
      //     _character.Face(Character.FacingDirections.Right);
      //   }

      SetClimbingState();
      _controller.CollisionsOn();

      //   if ((_characterHandleWeapon != null) && (!_characterHandleWeapon.CanShootFromLadders))
      //   {
      //     _characterHandleWeapon.ForceStop();
      //   }

      if (CurrentLadder.CenterCharacterOnLadder)
      {
        _controller.SetTransformPosition(new Vector2(CurrentLadder.transform.position.x, _controller.transform.position.y));
      }
    }

    protected virtual void StartClimbingDown()
    {
      SetClimbingState();
      _controller.CollisionsOff();

      // we rotate our character if requested
      //   if (ForceRightFacing)
      //   {
      //     _character.Face(Character.FacingDirections.Right);
      //   }

      //   if (_characterGravity != null)
      //   {
      //     if (_characterGravity.ShouldReverseInput())
      //     {
      //       if (LowestLadder.CenterCharacterOnLadder)
      //       {
      //         _controller.SetTransformPosition(new Vector2(LowestLadder.transform.position.x, transform.position.y + _climbingDownInitialYTranslation));
      //       }
      //       else
      //       {
      //         _controller.SetTransformPosition(new Vector2(transform.position.x, transform.position.y + _climbingDownInitialYTranslation));
      //       }
      //       return;
      //     }
      //   }

      if (LowestLadder.CenterCharacterOnLadder)
      {
        _controller.SetTransformPosition(new Vector2(LowestLadder.transform.position.x, transform.position.y - _climbingDownInitialYTranslation));
      }
      else
      {
        _controller.SetTransformPosition(new Vector2(transform.position.x, transform.position.y - _climbingDownInitialYTranslation));
      }
    }

    private bool AboveLadderPlatform()
    {
      // we make sure we have a current ladder and that it has a ladder platform associated to it
      if (LowestLadder == null)
      {
        return false;
      }
      if (LowestLadder.LadderPlatform == null)
      {
        return false;
      }

      float ladderColliderY = 0;

      if (LowestLadder.LadderPlatformBoxCollider2D != null)
      {
        ladderColliderY = LowestLadder.LadderPlatformBoxCollider2D.bounds.center.y + LowestLadder.LadderPlatformBoxCollider2D.bounds.extents.y;
      }
      if (LowestLadder.LadderPlatformEdgeCollider2D != null)
      {
        ladderColliderY = LowestLadder.LadderPlatform.transform.position.y + LowestLadder.LadderPlatformEdgeCollider2D.offset.y;
      }

      bool conditionAboveLadderPlatform = (ladderColliderY < _controller.ColliderBottomPosition.y + _ladderTopSkinHeight);

      //   if (_characterGravity != null)
      //   {
      //     if (_characterGravity.ShouldReverseInput())
      //     {
      //       if (LowestLadder.LadderPlatformBoxCollider2D != null)
      //       {
      //         ladderColliderY = LowestLadder.LadderPlatformBoxCollider2D.bounds.center.y - LowestLadder.LadderPlatformBoxCollider2D.bounds.extents.y;
      //       }

      //       if (LowestLadder.LadderPlatformEdgeCollider2D != null)
      //       {
      //         ladderColliderY = LowestLadder.LadderPlatform.transform.position.y
      //           - LowestLadder.LadderPlatformEdgeCollider2D.offset.y;
      //       }
      //       conditionAboveLadderPlatform = (ladderColliderY > _controller.ColliderTopPosition.y - _ladderTopSkinHeight);
      //     }
      //   }

      // if the bottom of the player's collider is above the ladder platform, we return true
      if (conditionAboveLadderPlatform)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    private void SetClimbingState()
    {
      _character.MovementState.ChangeState(typeof(LadderClimbingState));
      CurrentLadderClimbingSpeed = Vector2.zero;
      _controller.SetHorizontalForce(0);
      _controller.SetVerticalForce(0);
      _controller.GravityActive(false);
    }

    private void OnStateTypeChange(Type prevType, Type currentType)
    {
      bool wasLadderIdle = prevType == typeof(LadderIdleState);
      bool isLadderIdle = currentType == typeof(LadderIdleState);

      bool wasLadderClimbing = prevType == typeof(LadderClimbingState);
      bool isLadderClimbing = currentType == typeof(LadderClimbingState);

      if ((wasLadderIdle && !isLadderIdle && !isLadderClimbing) || (wasLadderClimbing && !isLadderIdle && !isLadderClimbing))
      {
        GetOffTheLadder();
      }
    }
    #endregion

  }
}