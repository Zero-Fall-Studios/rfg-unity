using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Character Controller 2D")]
  public class CharacterController2D : MonoBehaviour, IPhysics2D
  {
    #region Variables

    /// Enums

    // the possible modes this controller can update on
    public enum UpdateModes { Update, FixedUpdate }

    // the possible ways a character can detach from a oneway or moving platform
    public enum DetachmentMethods { Layer, Object }

    /// State

    // the various states of the character
    public CharacterControllerState2D State { get; private set; }

    /// Parameters

    [Header("Parameters")]

    [Tooltip("The default parameters for the character")]
    public CharacterControllerParameters2D DefaultParameters;

    [Tooltip("Add any overrides to the default parameters")]
    public CharacterControllerParameters2D OverrideParameters;
    public CharacterControllerParameters2D Parameters => OverrideParameters ?? DefaultParameters;

    public bool rotateOnMouseCursor = false;
    [DefinedValues("Face Right", "Face Left")]
    public string facingOnStart = "Face Right";

    /// Layer Masks

    [Header("Layer Masks")]

    [Tooltip("The layer mask the platforms are on")]
    public LayerMask PlatformMask;

    [Tooltip("The layer mask the one way platforms are on")]
    public LayerMask OneWayPlatformMask;

    [Tooltip("The layer mask the moving platforms are on")]
    public LayerMask MovingPlatformMask;

    [Tooltip("The layer mask the moving one way platforms are on")]
    public LayerMask OneWayMovingPlatformMask;

    [Tooltip("The layer mask the stairs are on")]
    public LayerMask StairsMask;

    /// Modes
    [Header("Modes")]

    [Tooltip("Whether this controller should run on Update or FixedUpdate")]
    public UpdateModes UpdateMode = UpdateModes.Update;

    [Tooltip("When a character jumps from a oneway or moving platform, collisions are off for a short moment. You can decide if they should happen a whole moving one way platform layer basis or just with the object the character just left")]
    public DetachmentMethods DetachmentMethod = DetachmentMethods.Layer;

    /// Standing On

    // Gives you the object the character is standing on
    [Tooltip("Gives you the object the character is standing on")]
    public GameObject StandingOn;

    // the object the character was standing on last frame
    public GameObject StandingOnLastFrame { get; private set; }

    /// Colliders

    // the collider the character is standing on
    public Collider2D StandingOnCollider { get; private set; }

    // the wall currently colliding with
    public GameObject CurrentWallCollider { get; private set; }

    /// Speed

    // The current speed of the character
    public Vector2 Speed { get { return _speed; } }

    // The world speed of the character
    public Vector2 WorldSpeed { get { return _worldSpeed; } }

    // The value of forces applied at one point in time
    public Vector2 ForcesApplied { get; private set; }



    /// Collider Information

    public Vector3 ColliderSize { get { return Vector3.Scale(transform.localScale, _boxCollider.size); } }
    public Vector3 ColliderCenterPosition { get { return _boxCollider.bounds.center; } }

    public Vector3 ColliderBottomPosition
    {
      get
      {
        _colliderBottomCenterPosition.x = _boxCollider.bounds.center.x;
        _colliderBottomCenterPosition.y = _boxCollider.bounds.min.y;
        _colliderBottomCenterPosition.z = 0;
        return _colliderBottomCenterPosition;
      }
    }

    public Vector3 ColliderLeftPosition
    {
      get
      {
        _colliderLeftCenterPosition.x = _boxCollider.bounds.min.x;
        _colliderLeftCenterPosition.y = _boxCollider.bounds.center.y;
        _colliderLeftCenterPosition.z = 0;
        return _colliderLeftCenterPosition;
      }
    }

    public Vector3 ColliderTopPosition
    {
      get
      {
        _colliderTopCenterPosition.x = _boxCollider.bounds.center.x;
        _colliderTopCenterPosition.y = _boxCollider.bounds.max.y;
        _colliderTopCenterPosition.z = 0;
        return _colliderTopCenterPosition;
      }
    }

    public Vector3 ColliderRightPosition
    {
      get
      {
        _colliderRightCenterPosition.x = _boxCollider.bounds.max.x;
        _colliderRightCenterPosition.y = _boxCollider.bounds.center.y;
        _colliderRightCenterPosition.z = 0;
        return _colliderRightCenterPosition;
      }
    }

    /// Bounds Information

    public Vector2 Bounds
    {
      get
      {
        _bounds.x = _boundsWidth;
        _bounds.y = _boundsHeight;
        return _bounds;
      }
    }

    public Vector3 BoundsTopLeftCorner { get { return _boundsTopLeftCorner; } }
    public Vector3 BoundsBottomLeftCorner { get { return _boundsBottomLeftCorner; } }
    public Vector3 BoundsTopRightCorner { get { return _boundsTopRightCorner; } }
    public Vector3 BoundsBottomRightCorner { get { return _boundsBottomRightCorner; } }
    public Vector3 BoundsTop { get { return (_boundsTopLeftCorner + _boundsTopRightCorner) / 2; } }
    public Vector3 BoundsBottom { get { return (_boundsBottomLeftCorner + _boundsBottomRightCorner) / 2; } }
    public Vector3 BoundsRight { get { return (_boundsTopRightCorner + _boundsBottomRightCorner) / 2; } }
    public Vector3 BoundsLeft { get { return (_boundsTopLeftCorner + _boundsBottomLeftCorner) / 2; } }
    public Vector3 BoundsCenter { get { return _boundsCenter; } }

    /// <summary>
    /// Returns the character's bounds width
    /// </summary>
    public float Width()
    {
      return _boundsWidth;
    }

    /// <summary>
    /// Returns the character's bounds height
    /// </summary>
    public float Height()
    {
      return _boundsHeight;
    }

    /// Distance

    public float DistanceToTheGround { get { return _distanceToTheGround; } }

    /// Forces

    public Vector2 ExternalForce { get { return _externalForce; } }
    public float Friction { get { return _friction; } }


    // Components References

    private Camera _mainCamera;
    private BoxCollider2D _boxCollider;
    private Transform _transform;
    private Vector2 _newPosition;

    // Layer Mask References

    private LayerMask _platformMaskSave;
    private LayerMask _raysBelowLayerMaskPlatforms;
    private LayerMask _raysBelowLayerMaskPlatformsWithoutOneWay;
    private int _savedBelowLayer;

    // Collider Positions

    private Vector3 _colliderBottomCenterPosition;
    private Vector3 _colliderLeftCenterPosition;
    private Vector3 _colliderRightCenterPosition;
    private Vector3 _colliderTopCenterPosition;

    // Bounds Positions

    private Vector2 _boundsTopLeftCorner;
    private Vector2 _boundsBottomLeftCorner;
    private Vector2 _boundsTopRightCorner;
    private Vector2 _boundsBottomRightCorner;
    private Vector2 _boundsCenter;
    private Vector2 _bounds;
    private float _boundsWidth;
    private float _boundsHeight;

    // Forces

    private Vector2 _speed;
    private Vector2 _worldSpeed;
    private Vector2 _externalForce;
    private float _friction = 0;
    private float _fallSlowFactor;
    private float _currentGravity = 0f;
    private bool _gravityActive = true;

    // Moving Platforms

    private Vector3 _activeLocalPlatformPoint;
    private Vector3 _activeGlobalPlatformPoint;

    // Distances

    protected float _distanceToTheGround;

    // Collider Info

    private Collider2D _ignoredCollider = null;
    private bool _collisionsOnWithStairs = false;

    // Const Values

    private const float _smallValue = 0.0001f;
    private const float _obstacleHeightTolerance = 0.05f;
    private const float _movingPlatformsGravity = -500f;

    // Original Values

    private Vector2 _originalColliderSize;
    private Vector2 _originalColliderOffset;
    private Vector2 _originalSizeRaycastOrigin;

    // Angles

    private Vector3 _crossBelowSlopeAngle;

    // Raycasts

    private RaycastHit2D[] _sideHitsStorage;
    private RaycastHit2D[] _belowHitsStorage;
    private RaycastHit2D[] _aboveHitsStorage;
    private RaycastHit2D _stickRaycastLeft;
    private RaycastHit2D _stickRaycastRight;
    private RaycastHit2D _stickRaycast;
    private RaycastHit2D _distanceToTheGroundRaycast;

    private Vector2 _horizontalRayCastFromBottom = Vector2.zero;
    private Vector2 _horizontalRayCastToTop = Vector2.zero;
    private Vector2 _verticalRayCastFromLeft = Vector2.zero;
    private Vector2 _verticalRayCastToRight = Vector2.zero;
    private Vector2 _aboveRayCastStart = Vector2.zero;
    private Vector2 _aboveRayCastEnd = Vector2.zero;
    private Vector2 _rayCastOrigin = Vector2.zero;

    private RaycastHit2D[] _raycastNonAlloc = new RaycastHit2D[0];

    private List<RaycastHit2D> _contactList;

    // Directions

    private float _movementDirection;
    private float _storedMovementDirection = 1;
    private const float _movementDirectionThreshold = 0.0001f;

    // Update

    private bool _update;

    #endregion

    #region Unity Methods
    private void Awake()
    {
      _mainCamera = Camera.main;
      _transform = transform;

      // Colliders

      _boxCollider = GetComponent<BoxCollider2D>();
      _originalColliderSize = _boxCollider.size;
      _originalColliderOffset = _boxCollider.offset;

      CurrentWallCollider = null;

      // State

      State = new CharacterControllerState2D();
      State.Reset();

      // Raycasts

      _contactList = new List<RaycastHit2D>();
      _sideHitsStorage = new RaycastHit2D[Parameters.NumberOfHorizontalRays];
      _belowHitsStorage = new RaycastHit2D[Parameters.NumberOfVerticalRays];
      _aboveHitsStorage = new RaycastHit2D[Parameters.NumberOfVerticalRays];

      SetRaysParameters();

      // Layer Masks
      _platformMaskSave = PlatformMask;
      PlatformMask |= OneWayPlatformMask;
      PlatformMask |= MovingPlatformMask;
      PlatformMask |= OneWayMovingPlatformMask;

      // Modes

      _update = UpdateMode == UpdateModes.Update;

      // Settings

      ApplyGravitySettings();
      ApplyPhysicsSettings();

      if (facingOnStart.Equals("Face Left"))
      {
        Flip();
      }

      State.IsFacingRight = _transform.right.x > 0;
    }

    private void Update()
    {
      if (_update)
      {
        EveryFrame();
      }
    }

    private void LateUpdate()
    {
      if (rotateOnMouseCursor)
      {
        RotateOnMouseCursor();
      }
    }

    private void FixedUpdate()
    {
      if (!_update)
      {
        EveryFrame();
      }
    }
    #endregion

    #region Set Force
    public void AddForce(Vector2 force)
    {
      _speed += force;
      _externalForce += force;
    }

    public void AddHorizontalForce(float x)
    {
      _speed.x += x;
      _externalForce.x += x;
    }

    public void AddVerticalForce(float y)
    {
      _speed.y += y;
      _externalForce.y += y;
    }

    public void SetForce(Vector2 force)
    {
      _speed = force;
      _externalForce = force;
    }

    public void SetHorizontalForce(float x)
    {
      _speed.x = x;
      _externalForce.x = x;
    }

    public void SetVerticalForce(float y)
    {
      _speed.y = y;
      _externalForce.y = y;
    }
    #endregion

    private void ApplyGravitySettings()
    {
      if (Parameters.AutomaticGravtiySettings)
      {
        // Gravity Ability / Behaviour
      }
    }

    private void ApplyPhysicsSettings()
    {
      if (Parameters.AutomaticallySetPhysicsSettings)
      {
        UnityEngine.Physics2D.queriesHitTriggers = true;
        UnityEngine.Physics2D.queriesStartInColliders = true;
        UnityEngine.Physics2D.callbacksOnDisable = true;
        UnityEngine.Physics2D.reuseCollisionCallbacks = false;
        UnityEngine.Physics2D.autoSyncTransforms = true;
      }
    }

    private void EveryFrame()
    {
      if (Time.timeScale == 0f)
      {
        return;
      }

      ApplyGravity();
      FrameInitializtion();
      SetRaysParameters();
      HandleMovingPlatforms();

      // Store current speed for use in moving platforms
      ForcesApplied = _speed;

      DetermineMovementDirection();

      if (Parameters.CastRaysOnBothSides)
      {
        CastRaysToTheLeft();
        CastRaysToTheRight();
      }
      else
      {
        if (_movementDirection == -1f)
        {
          CastRaysToTheLeft();
        }
        else
        {
          CastRaysToTheRight();
        }
      }

      CastRaysBelow();
      CastRaysAbove();

      MoveTransform();

      SetRaysParameters();
      ComputeNewSpeed();
      SetStates();
      ComputeDistanceToTheGround();

      _externalForce = Vector2.zero;

      FrameExit();

      _worldSpeed = _speed;
    }

    private void ApplyGravity()
    {
      _currentGravity = Parameters.Gravity;
      if (_speed.y > 0)
      {
        _currentGravity = _currentGravity / Parameters.AscentMultiplier;
      }
      if (_speed.y < 0)
      {
        _currentGravity = _currentGravity * Parameters.FallMultiplier;
      }
      if (_gravityActive)
      {
        _speed.y += _currentGravity * Time.deltaTime;
      }
      if (_fallSlowFactor != 0)
      {
        _speed.y *= _fallSlowFactor;
      }
    }

    private void FrameInitializtion()
    {
      _contactList.Clear();
      _newPosition = Speed * Time.deltaTime;
      State.WasGroundedLastFrame = State.IsCollidingBelow;
      StandingOnLastFrame = StandingOn;
      State.WasTouchingTheCeilingLastFrame = State.IsCollidingAbove;
      CurrentWallCollider = null;
      State.Reset();
    }

    private void SetRaysParameters()
    {
      float top = _boxCollider.offset.y + (_boxCollider.size.y / 2f);
      float bottom = _boxCollider.offset.y - (_boxCollider.size.y / 2f);
      float left = _boxCollider.offset.x - (_boxCollider.size.x / 2f);
      float right = _boxCollider.offset.x + (_boxCollider.size.x / 2f);

      _boundsTopLeftCorner.x = left;
      _boundsTopLeftCorner.y = top;

      _boundsTopRightCorner.x = right;
      _boundsTopRightCorner.y = top;

      _boundsBottomLeftCorner.x = left;
      _boundsBottomLeftCorner.y = bottom;

      _boundsBottomRightCorner.x = right;
      _boundsBottomRightCorner.y = bottom;

      _boundsTopLeftCorner = transform.TransformPoint(_boundsTopLeftCorner);
      _boundsTopRightCorner = transform.TransformPoint(_boundsTopRightCorner);
      _boundsBottomLeftCorner = transform.TransformPoint(_boundsBottomLeftCorner);
      _boundsBottomRightCorner = transform.TransformPoint(_boundsBottomRightCorner);
      _boundsCenter = _boxCollider.bounds.center;

      _boundsWidth = Vector2.Distance(_boundsBottomLeftCorner, _boundsBottomRightCorner);
      _boundsHeight = Vector2.Distance(_boundsBottomLeftCorner, _boundsTopLeftCorner);
    }

    private void HandleMovingPlatforms()
    {
      if (StandingOn != null && (MovingPlatformMask.Contains(StandingOn.layer) || OneWayMovingPlatformMask.Contains(StandingOn.layer)))
      {
        var newGlobalPlatformPoint = StandingOn.transform.TransformPoint(_activeLocalPlatformPoint);
        var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;
        if (moveDistance != Vector3.zero)
        {
          transform.Translate(moveDistance, Space.World);
        }
      }
      StandingOn = null;
    }

    private void DetermineMovementDirection()
    {
      _movementDirection = _storedMovementDirection;
      if (_speed.x < -_movementDirectionThreshold)
      {
        _movementDirection = -1;
      }
      else if (_speed.x > _movementDirectionThreshold)
      {
        _movementDirection = 1;
      }
      else if (_externalForce.x < -_movementDirectionThreshold)
      {
        _movementDirection = -1;
      }
      else if (_externalForce.x > _movementDirectionThreshold)
      {
        _movementDirection = 1;
      }

      _storedMovementDirection = _movementDirection;

      State.IsFacingRight = _transform.right.x > 0;
    }

    #region Raycasts
    private void CastRaysToTheLeft()
    {
      CastRaysToTheSides(-1);
    }

    private void CastRaysToTheRight()
    {
      CastRaysToTheSides(1);
    }

    /// <summary>
    /// Casts rays to the sides of the character, from its center axis.
    /// If we hit a wall/slope, we check its angle and move or not according to it.
    /// </summary>
    protected void CastRaysToTheSides(float raysDirection)
    {
      // we determine the origin of our rays
      _horizontalRayCastFromBottom = (_boundsBottomRightCorner + _boundsBottomLeftCorner) / 2;
      _horizontalRayCastToTop = (_boundsTopLeftCorner + _boundsTopRightCorner) / 2;
      _horizontalRayCastFromBottom = _horizontalRayCastFromBottom + (Vector2)transform.up * _obstacleHeightTolerance;
      _horizontalRayCastToTop = _horizontalRayCastToTop - (Vector2)transform.up * _obstacleHeightTolerance;

      // we determine the length of our rays
      float horizontalRayLength = Mathf.Abs(_speed.x * Time.deltaTime) + _boundsWidth / 2 + Parameters.RayOffset * 2;

      // we resize our storage if needed
      if (_sideHitsStorage.Length != Parameters.NumberOfHorizontalRays)
      {
        _sideHitsStorage = new RaycastHit2D[Parameters.NumberOfHorizontalRays];
      }

      // we cast rays to the sides
      for (int i = 0; i < Parameters.NumberOfHorizontalRays; i++)
      {
        Vector2 rayOriginPoint = Vector2.Lerp(_horizontalRayCastFromBottom, _horizontalRayCastToTop, (float)i / (float)(Parameters.NumberOfHorizontalRays - 1));

        // if we were grounded last frame and if this is our first ray, we don't cast against one way platforms
        if (State.WasGroundedLastFrame && i == 0)
        {
          _sideHitsStorage[i] = RFG.Physics2D.RayCast(rayOriginPoint, raysDirection * Vector2.right, horizontalRayLength, PlatformMask, Color.red, true);
        }
        else
        {
          _sideHitsStorage[i] = RFG.Physics2D.RayCast(rayOriginPoint, raysDirection * Vector2.right, horizontalRayLength, PlatformMask & ~OneWayPlatformMask & ~OneWayMovingPlatformMask, Color.red, true);
        }
        // if we've hit something
        if (_sideHitsStorage[i].distance > 0)
        {
          // if this collider is on our ignore list, we break
          if (_sideHitsStorage[i].collider == _ignoredCollider)
          {
            break;
          }

          // we determine and store our current lateral slope angle
          float hitAngle = Mathf.Abs(Vector2.Angle(_sideHitsStorage[i].normal, transform.up));

          if (OneWayPlatformMask.Contains(_sideHitsStorage[i].collider.gameObject))
          {
            if (hitAngle > 90)
            {
              break;
            }
          }

          // we check if this is our movement direction
          if (_movementDirection == raysDirection)
          {
            State.LateralSlopeAngle = hitAngle;
          }

          // if the lateral slope angle is higher than our maximum slope angle, then we've hit a wall, and stop x movement accordingly
          if (hitAngle > Parameters.MaxSlopeAngle)
          {
            if (raysDirection < 0)
            {
              State.IsCollidingLeft = true;
              State.DistanceToLeftCollider = _sideHitsStorage[i].distance;
            }
            else
            {
              State.IsCollidingRight = true;
              State.DistanceToRightCollider = _sideHitsStorage[i].distance;
            }

            if ((_movementDirection == raysDirection) || (Parameters.CastRaysOnBothSides && (_speed.x == 0f)))
            {
              CurrentWallCollider = _sideHitsStorage[i].collider.gameObject;
              State.SlopeAngleOK = false;

              float distance = Math.DistanceBetweenPointAndLine(_sideHitsStorage[i].point, _horizontalRayCastFromBottom, _horizontalRayCastToTop);
              if (raysDirection <= 0)
              {
                _newPosition.x = -distance + _boundsWidth / 2 + Parameters.RayOffset * 2;
              }
              else
              {
                _newPosition.x = distance - _boundsWidth / 2 - Parameters.RayOffset * 2;
              }

              // if we're in the air, we prevent the character from being pushed back.
              if (!State.IsGrounded && (Speed.y != 0) && (!Mathf.Approximately(hitAngle, 90f)))
              {
                _newPosition.x = 0;
              }

              _contactList.Add(_sideHitsStorage[i]);
              _speed.x = 0;
            }

            break;
          }
        }
      }
    }

    private void CastRaysBelow()
    {
      _friction = 0;

      if (_newPosition.y < -_smallValue)
      {
        State.IsFalling = true;
      }
      else
      {
        State.IsFalling = false;
      }

      // if ((Parameters.Gravity > 0) && (!State.IsFalling))
      // {
      //   State.IsCollidingBelow = false;
      //   return;
      // }

      float rayLength = (_boundsHeight / 2) + Parameters.RayOffset;

      if (State.OnAMovingPlatform)
      {
        rayLength *= 2;
      }

      if (_newPosition.y < 0)
      {
        rayLength += Mathf.Abs(_newPosition.y);
      }

      _verticalRayCastFromLeft = (_boundsBottomLeftCorner + _boundsTopLeftCorner) / 2;
      _verticalRayCastToRight = (_boundsBottomRightCorner + _boundsTopRightCorner) / 2;
      _verticalRayCastFromLeft += (Vector2)transform.up * Parameters.RayOffset;
      _verticalRayCastToRight += (Vector2)transform.up * Parameters.RayOffset;
      _verticalRayCastFromLeft += (Vector2)Vector2.right * _newPosition.x;
      _verticalRayCastToRight += (Vector2)Vector2.right * _newPosition.x;

      if (_belowHitsStorage.Length != Parameters.NumberOfVerticalRays)
      {
        _belowHitsStorage = new RaycastHit2D[Parameters.NumberOfVerticalRays];
      }

      _raysBelowLayerMaskPlatforms = PlatformMask;

      _raysBelowLayerMaskPlatformsWithoutOneWay = PlatformMask & ~OneWayPlatformMask & ~OneWayMovingPlatformMask;

      // if what we're standing on is a mid height oneway platform, we turn it into a regular platform for this frame only
      if (StandingOnLastFrame != null)
      {
        _savedBelowLayer = StandingOnLastFrame.layer;
      }

      // stairs management
      if (State.WasGroundedLastFrame)
      {
        if (StandingOnLastFrame != null)
        {
          if (StairsMask.Contains(StandingOnLastFrame.layer))
          {
            // if we're still within the bounds of the stairs
            if (StandingOnCollider.bounds.Contains(_colliderBottomCenterPosition))
            {
              _raysBelowLayerMaskPlatforms = _raysBelowLayerMaskPlatforms & ~OneWayPlatformMask | StairsMask;
            }
          }
        }
      }

      if (State.OnAMovingPlatform && (_newPosition.y > 0))
      {
        _raysBelowLayerMaskPlatforms = _raysBelowLayerMaskPlatforms & ~OneWayPlatformMask;
      }

      float smallestDistance = float.MaxValue;
      int smallestDistanceIndex = 0;
      bool hitConnected = false;

      for (int i = 0; i < Parameters.NumberOfVerticalRays; i++)
      {
        Vector2 rayOriginPoint = Vector2.Lerp(_verticalRayCastFromLeft, _verticalRayCastToRight, (float)i / (float)(Parameters.NumberOfVerticalRays - 1));

        if ((_newPosition.y > 0) && (!State.WasGroundedLastFrame))
        {
          _belowHitsStorage[i] = RFG.Physics2D.RayCast(rayOriginPoint, -transform.up, rayLength, _raysBelowLayerMaskPlatformsWithoutOneWay, Color.magenta, true);
        }
        else
        {
          _belowHitsStorage[i] = RFG.Physics2D.RayCast(rayOriginPoint, -transform.up, rayLength, _raysBelowLayerMaskPlatforms, Color.magenta, true);
        }

        float distance = Math.DistanceBetweenPointAndLine(_belowHitsStorage[smallestDistanceIndex].point, _verticalRayCastFromLeft, _verticalRayCastToRight);

        if (_belowHitsStorage[i])
        {
          if (_belowHitsStorage[i].collider == _ignoredCollider)
          {
            continue;
          }

          hitConnected = true;
          State.BelowSlopeAngle = Vector2.Angle(_belowHitsStorage[i].normal, transform.up);
          _crossBelowSlopeAngle = Vector3.Cross(transform.up, _belowHitsStorage[i].normal);
          if (_crossBelowSlopeAngle.z < 0)
          {
            State.BelowSlopeAngle = -State.BelowSlopeAngle;
          }

          if (_belowHitsStorage[i].distance < smallestDistance)
          {
            smallestDistanceIndex = i;
            smallestDistance = _belowHitsStorage[i].distance;
          }
        }

        if (distance < _smallValue)
        {
          break;
        }
      }
      if (hitConnected)
      {
        StandingOn = _belowHitsStorage[smallestDistanceIndex].collider.gameObject;
        StandingOnCollider = _belowHitsStorage[smallestDistanceIndex].collider;

        // if the character is jumping onto a (1-way) platform but not high enough, we do nothing
        if (
          !State.WasGroundedLastFrame
          && smallestDistance < _boundsHeight / 2
          && (OneWayPlatformMask.Contains(StandingOn.layer) || (OneWayMovingPlatformMask.Contains(StandingOn.layer) && _speed.y > 0))
        )
        {
          State.IsCollidingBelow = false;
          return;
        }

        State.IsFalling = false;
        State.IsCollidingBelow = true;

        // if we're applying an external force (jumping, jetpack...) we only apply that
        if (_externalForce.y > 0 && _speed.y > 0)
        {
          _newPosition.y = _speed.y * Time.deltaTime;
          State.IsCollidingBelow = false;
        }
        // if not, we just adjust the position based on the raycast hit
        else
        {
          float distance = Math.DistanceBetweenPointAndLine(_belowHitsStorage[smallestDistanceIndex].point, _verticalRayCastFromLeft, _verticalRayCastToRight);
          _newPosition.y = -distance + _boundsHeight / 2 + Parameters.RayOffset;
        }

        if (!State.WasGroundedLastFrame && _speed.y > 0)
        {
          _newPosition.y += _speed.y * Time.deltaTime;
        }

        if (Mathf.Abs(_newPosition.y) < _smallValue)
        {
          _newPosition.y = 0;
        }

        // we check if whatever we're standing on applies a friction change
        // _frictionTest = _belowHitsStorage[smallestDistanceIndex].collider.gameObject.MMGetComponentNoAlloc<SurfaceModifier>();
        // if ((_frictionTest != null) && (_frictionTest.enabled))
        // {
        //   _friction = _belowHitsStorage[smallestDistanceIndex].collider.GetComponent<SurfaceModifier>().Friction;
        // }
      }
      else
      {
        State.IsCollidingBelow = false;
      }

      if (Parameters.StickToSlopes)
      {
        StickToSlope();
      }
    }

    private void CastRaysAbove()
    {
      float rayLength = State.IsGrounded ? Parameters.RayOffset : _newPosition.y;
      rayLength += _boundsHeight / 2;

      bool hitConnected = false;

      _aboveRayCastStart = (_boundsBottomLeftCorner + _boundsTopLeftCorner) / 2;
      _aboveRayCastEnd = (_boundsBottomRightCorner + _boundsTopRightCorner) / 2;

      _aboveRayCastStart += (Vector2)Vector2.right * _newPosition.x;
      _aboveRayCastEnd += (Vector2)Vector2.right * _newPosition.x;

      if (_aboveHitsStorage.Length != Parameters.NumberOfVerticalRays)
      {
        _aboveHitsStorage = new RaycastHit2D[Parameters.NumberOfVerticalRays];
      }

      float smallestDistance = float.MaxValue;

      int collidingIndex = 0;
      for (int i = 0; i < Parameters.NumberOfVerticalRays; i++)
      {
        Vector2 rayOriginPoint = Vector2.Lerp(_aboveRayCastStart, _aboveRayCastEnd, (float)i / (float)(Parameters.NumberOfVerticalRays - 1));
        _aboveHitsStorage[i] = RFG.Physics2D.RayCast(rayOriginPoint, (transform.up), rayLength, PlatformMask & ~OneWayPlatformMask & ~OneWayMovingPlatformMask, Color.green, true);

        if (_aboveHitsStorage[i])
        {
          hitConnected = true;
          collidingIndex = i;

          if (_aboveHitsStorage[i].collider == _ignoredCollider)
          {
            break;
          }
          if (_aboveHitsStorage[i].distance < smallestDistance)
          {
            smallestDistance = _aboveHitsStorage[i].distance;
          }
        }
      }

      if (hitConnected)
      {
        _newPosition.y = smallestDistance - _boundsHeight / 2;

        if ((State.IsGrounded) && (_newPosition.y < 0))
        {
          _newPosition.y = 0;
        }

        State.IsCollidingAbove = true;

        if (!State.WasTouchingTheCeilingLastFrame)
        {
          _speed = new Vector2(_speed.x, 0f);
        }

        SetVerticalForce(0);
      }
    }
    #endregion

    #region Slopes
    private void StickToSlope()
    {
      // if we're in the air, don't have to stick to slopes, being pushed up or on a moving platform, we exit
      if (
        _newPosition.y >= Parameters.StickToSlopesOffsetY || _newPosition.y <= -Parameters.StickToSlopesOffsetY
        || State.IsJumping
        || !Parameters.StickToSlopes
        || !State.WasGroundedLastFrame
        || _externalForce.y > 0
      )
      {
        // edge case for stairs
        if (!(!State.WasGroundedLastFrame
          && ((StandingOnLastFrame != null) && StairsMask.Contains(StandingOnLastFrame.layer))
          && !(State.IsJumping)
          ))
        {
          return;
        }
      }

      // if ((_characterGravity != null) && (_characterGravity.InGravityPointRange))
      // {
      //   return;
      // }

      // we determine our ray's length
      float rayLength = 0;
      if (Parameters.StickyRaycastLength == 0)
      {
        rayLength = _boundsWidth * Mathf.Abs(Mathf.Tan(Parameters.MaxSlopeAngle));
        rayLength += _boundsHeight / 2 + Parameters.RayOffset;
      }
      else
      {
        rayLength = Parameters.StickyRaycastLength;
      }

      // we cast rays on both sides to know what we're standing on
      _rayCastOrigin.y = _boundsCenter.y;

      _rayCastOrigin.x = _boundsBottomLeftCorner.x;
      _rayCastOrigin.x += _newPosition.x;
      _stickRaycastLeft = RFG.Physics2D.RayCast(_rayCastOrigin, -transform.up, rayLength, _raysBelowLayerMaskPlatforms, Color.green, true);

      _rayCastOrigin.x = _boundsBottomRightCorner.x;
      _rayCastOrigin.x += _newPosition.x;
      _stickRaycastRight = RFG.Physics2D.RayCast(_rayCastOrigin, -transform.up, rayLength, _raysBelowLayerMaskPlatforms, Color.green, true);

      bool castFromLeft = false;
      float belowSlopeAngleLeft = Vector2.Angle(_stickRaycastLeft.normal, transform.up);
      Vector3 crossBelowSlopeAngleLeft = Vector3.Cross(transform.up, _stickRaycastLeft.normal);
      if (crossBelowSlopeAngleLeft.z < 0)
      {
        belowSlopeAngleLeft = -belowSlopeAngleLeft;
      }

      float belowSlopeAngleRight = Vector2.Angle(_stickRaycastRight.normal, transform.up);
      Vector3 crossBelowSlopeAngleRight = Vector3.Cross(transform.up, _stickRaycastRight.normal);
      if (crossBelowSlopeAngleRight.z < 0)
      {
        belowSlopeAngleRight = -belowSlopeAngleRight;
      }

      float belowSlopeAngle = 0f;

      castFromLeft = (Mathf.Abs(belowSlopeAngleLeft) > Mathf.Abs(belowSlopeAngleRight));

      // if we're on a slope
      if (belowSlopeAngleLeft == belowSlopeAngleRight)
      {
        belowSlopeAngle = belowSlopeAngleLeft;
        castFromLeft = (belowSlopeAngle < 0f);
      }

      // if we have a slope on the right and flat on the left
      if ((belowSlopeAngleLeft == 0f) && (belowSlopeAngleRight != 0f))
      {
        belowSlopeAngle = belowSlopeAngleLeft;
        castFromLeft = (belowSlopeAngleRight < 0f);
      }

      // if we have flat on the right and a slope on the left
      if ((belowSlopeAngleLeft != 0f) && (belowSlopeAngleRight == 0f))
      {
        belowSlopeAngle = belowSlopeAngleRight;
        castFromLeft = (belowSlopeAngleLeft < 0f);
      }

      // if both angles aren't flat
      if ((belowSlopeAngleLeft != 0f) && (belowSlopeAngleRight != 0f))
      {
        castFromLeft = (_stickRaycastLeft.distance < _stickRaycastRight.distance);
        belowSlopeAngle = (castFromLeft) ? belowSlopeAngleLeft : belowSlopeAngleRight;
      }

      // if we're on a damn spike, we handle it and exit
      if (belowSlopeAngleLeft > 0f && belowSlopeAngleRight < 0f)
      {
        _stickRaycast = RFG.Physics2D.BoxCast(_boundsCenter, Bounds, Vector2.Angle(transform.up, Vector2.up), -transform.up, rayLength, _raysBelowLayerMaskPlatforms, Color.yellow, true);
        if (_stickRaycast)
        {
          if (_stickRaycast.collider == _ignoredCollider)
          {
            return;
          }

          _newPosition.y = -Mathf.Abs(_stickRaycast.point.y - _rayCastOrigin.y) + _boundsHeight / 2;

          State.IsCollidingBelow = true;
        }

        return;
      }

      _stickRaycast = castFromLeft ? _stickRaycastLeft : _stickRaycastRight;

      // we cast a ray, if it hits, we move to match its height
      if (_stickRaycast)
      {
        if (_stickRaycast.collider == _ignoredCollider)
        {
          return;
        }

        _newPosition.y = -Mathf.Abs(_stickRaycast.point.y - _rayCastOrigin.y) + _boundsHeight / 2;

        State.IsCollidingBelow = true;
      }
    }
    #endregion

    private void MoveTransform()
    {
      if (Parameters.PerformSafetyBoxcast)
      {
        _stickRaycast = RFG.Physics2D.BoxCast(_boundsCenter, Bounds, Vector2.Angle(transform.up, Vector2.up), _newPosition.normalized, _newPosition.magnitude, PlatformMask, Color.red, true);
        if (_stickRaycast)
        {
          if (Mathf.Abs(_stickRaycast.distance - _newPosition.magnitude) < 0.0002f)
          {
            _newPosition = Vector2.zero;
            return;
          }
        }
      }

      // we move our transform to its next position
      _transform.Translate(_newPosition, Space.World);

      if (StandingOn != null)
      {
        _activeGlobalPlatformPoint = _transform.position;
        _activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(_transform.position);
      }
    }

    private void ComputeNewSpeed()
    {
      // we compute the new speed
      if (Time.deltaTime > 0)
      {
        _speed = _newPosition / Time.deltaTime;
      }

      // we apply our slope speed factor based on the slope's angle
      if (State.IsGrounded)
      {
        _speed.x *= Parameters.SlopeAngleSpeedFactor.Evaluate(Mathf.Abs(State.BelowSlopeAngle) * Mathf.Sign(_speed.y));
      }

      if (!State.OnAMovingPlatform)
      {
        // we make sure the velocity doesn't exceed the MaxVelocity specified in the parameters
        _speed.x = Mathf.Clamp(_speed.x, -Parameters.MaxVelocity.x, Parameters.MaxVelocity.x);
        _speed.y = Mathf.Clamp(_speed.y, -Parameters.MaxVelocity.y, Parameters.MaxVelocity.y);
      }
    }

    private void SetStates()
    {
      // we change states depending on the outcome of the movement
      if (!State.WasGroundedLastFrame && State.IsCollidingBelow)
      {
        State.JustGotGrounded = true;
      }

      if (State.IsCollidingLeft || State.IsCollidingRight || State.IsCollidingBelow || State.IsCollidingAbove)
      {
        OnColliderHit();
      }
    }
    private void ComputeDistanceToTheGround()
    {
      if (Parameters.DistanceToTheGroundRayMaximumLength <= 0)
      {
        return;
      }

      _rayCastOrigin.x = (State.BelowSlopeAngle < 0) ? _boundsBottomLeftCorner.x : _boundsBottomRightCorner.x;
      _rayCastOrigin.y = _boundsCenter.y;

      _distanceToTheGroundRaycast = RFG.Physics2D.RayCast(_rayCastOrigin, -transform.up, Parameters.DistanceToTheGroundRayMaximumLength, _raysBelowLayerMaskPlatforms, Color.cyan, true);

      if (_distanceToTheGroundRaycast)
      {
        if (_distanceToTheGroundRaycast.collider == _ignoredCollider)
        {
          _distanceToTheGround = -1f;
          return;
        }
        _distanceToTheGround = _distanceToTheGroundRaycast.distance - _boundsHeight / 2;
      }
      else
      {
        _distanceToTheGround = -1f;
      }
    }

    private void FrameExit()
    {
      if (StandingOnLastFrame != null)
      {
        StandingOnLastFrame.layer = _savedBelowLayer;
      }
    }

    /// <summary>
    /// Slows the character's fall by the specified factor.
    /// </summary>
    /// <param name="factor">Factor.</param>
    public void SlowFall(float factor)
    {
      _fallSlowFactor = factor;
    }

    /// <summary>
    /// Activates or desactivates the gravity for this character only.
    /// </summary>
    /// <param name="state">If set to <c>true</c>, activates the gravity. If set to <c>false</c>, turns it off.</param>	   
    public void GravityActive(bool state)
    {
      if (state)
      {
        _gravityActive = true;
      }
      else
      {
        _gravityActive = false;
      }
    }

    /// <summary>
    /// Resizes the collider to the new size set in parameters
    /// </summary>
    /// <param name="newSize">New size.</param>
    public void ResizeCollider(Vector2 newSize)
    {
      float newYOffset = _originalColliderOffset.y - (_originalColliderSize.y - newSize.y) / 2;

      _boxCollider.size = newSize;
      _boxCollider.offset = newYOffset * Vector3.up;
      SetRaysParameters();
      State.ColliderResized = true;

    }

    /// <summary>
    /// Returns the collider to its initial size
    /// </summary>
    public void ResetColliderSize()
    {
      _boxCollider.size = _originalColliderSize;
      _boxCollider.offset = _originalColliderOffset;
      SetRaysParameters();
      State.ColliderResized = false;
    }

    /// <summary>
    /// Determines whether this instance can go back to original size.
    /// </summary>
    /// <returns><c>true</c> if this instance can go back to original size; otherwise, <c>false</c>.</returns>
    public bool CanGoBackToOriginalSize()
    {
      // if we're already at original size, we return true
      if (_boxCollider.size == _originalColliderSize)
      {
        return true;
      }
      float headCheckDistance = _originalColliderSize.y * transform.localScale.y * Parameters.CrouchedRaycastLengthMultiplier;

      // we cast two rays above our character to check for obstacles. If we didn't hit anything, we can go back to original size, otherwise we can't
      _originalSizeRaycastOrigin = _boundsTopLeftCorner + (Vector2)transform.up * _smallValue;
      bool headCheckLeft = RFG.Physics2D.RayCast(_originalSizeRaycastOrigin, transform.up, headCheckDistance, PlatformMask - OneWayPlatformMask, Color.gray, true);

      _originalSizeRaycastOrigin = _boundsTopRightCorner + (Vector2)transform.up * _smallValue;
      bool headCheckRight = RFG.Physics2D.RayCast(_originalSizeRaycastOrigin, transform.up, headCheckDistance, PlatformMask - OneWayPlatformMask, Color.gray, true);
      if (headCheckLeft || headCheckRight)
      {
        return false;
      }
      else
      {
        return true;
      }
    }

    public void SetIgnoreCollider(Collider2D newIgnoredCollider)
    {
      _ignoredCollider = newIgnoredCollider;
    }

    /// <summary>
    /// Disables the collisions for the specified duration
    /// </summary>
    /// <param name="duration">the duration for which the collisions must be disabled</param>
    public IEnumerator DisableCollisions(float duration)
    {
      // we turn the collisions off
      CollisionsOff();
      // we wait for a few seconds
      yield return new WaitForSeconds(duration);
      // we turn them on again
      CollisionsOn();
    }

    /// <summary>
    /// Resets the collision mask with the default settings
    /// </summary>
    public void CollisionsOn()
    {
      PlatformMask = _platformMaskSave;
      PlatformMask |= OneWayPlatformMask;
      PlatformMask |= MovingPlatformMask;
      PlatformMask |= OneWayMovingPlatformMask;
    }

    /// <summary>
    /// Turns all collisions off
    /// </summary>
    public void CollisionsOff()
    {
      PlatformMask = 0;
    }

    /// <summary>
    /// Disables the collisions with one way platforms for the specified duration
    /// </summary>
    /// <param name="duration">the duration for which the collisions must be disabled</param>
    public IEnumerator DisableCollisionsWithOneWayPlatforms(float duration)
    {
      switch (DetachmentMethod)
      {
        case DetachmentMethods.Layer:
          // we make it fall down below the platform by moving it just below the platform
          this.transform.position = new Vector2(transform.position.x, transform.position.y - 0.1f);
          // we turn the collisions off
          CollisionsOffWithOneWayPlatformsLayer();
          // we wait for a few seconds
          yield return new WaitForSeconds(duration);
          // we turn them on again
          CollisionsOn();
          break;
        case DetachmentMethods.Object:
          // we set our current platform collider as ignored
          SetIgnoreCollider(StandingOnCollider);
          // we wait for a few seconds
          yield return new WaitForSeconds(duration);
          // we turn clear it
          SetIgnoreCollider(null);
          break;
      }
    }

    /// <summary>
    /// Disables the collisions with moving platforms for the specified duration
    /// </summary>
    /// <param name="duration">the duration for which the collisions must be disabled</param>
    public IEnumerator DisableCollisionsWithMovingPlatforms(float duration)
    {
      if (DetachmentMethod == DetachmentMethods.Layer)
      {
        // we turn the collisions off
        CollisionsOffWithMovingPlatformsLayer();
        // we wait for a few seconds
        yield return new WaitForSeconds(duration);
        // we turn them on again
        CollisionsOn();
      }
      else
      {
        // we set our current platform collider as ignored
        SetIgnoreCollider(StandingOnCollider);
        // we wait for a few seconds
        yield return new WaitForSeconds(duration);
        // we turn clear it
        SetIgnoreCollider(null);
      }
    }

    /// <summary>
    /// Disables collisions only with the one way platform layers
    /// </summary>
    public void CollisionsOffWithOneWayPlatformsLayer()
    {
      PlatformMask -= OneWayPlatformMask;
      PlatformMask -= OneWayMovingPlatformMask;
    }

    /// <summary>
    /// Disables collisions only with moving platform layers
    /// </summary>
    public void CollisionsOffWithMovingPlatformsLayer()
    {
      PlatformMask -= MovingPlatformMask;
      PlatformMask -= OneWayMovingPlatformMask;
    }

    /// <summary>
    /// Enables collisions with the stairs layer
    /// </summary>
    public void CollisionsOnWithStairs()
    {
      if (!_collisionsOnWithStairs)
      {
        PlatformMask = PlatformMask | StairsMask;
        OneWayPlatformMask = OneWayPlatformMask | StairsMask;
        _collisionsOnWithStairs = true;
        CollisionsOn();
      }
    }

    /// <summary>
    /// Disables collisions with the stairs layer
    /// </summary>
    public void CollisionsOffWithStairs()
    {
      if (_collisionsOnWithStairs)
      {
        PlatformMask = PlatformMask - StairsMask;
        OneWayPlatformMask = OneWayPlatformMask - StairsMask;
        _collisionsOnWithStairs = false;
      }
    }

    /// <summary>
    /// Moves the controller's transform to the desired position
    /// </summary>
    /// <param name="position"></param>
    public void SetTransformPosition(Vector2 position)
    {
      if (Parameters.SafeSetTransform)
      {
        this.transform.position = GetClosestSafePosition(position);
      }
      else
      {
        this.transform.position = position;
      }
    }

    /// <summary>
    /// Returns the closest "safe" point (not overlapping any platform) to the destination
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    public Vector2 GetClosestSafePosition(Vector2 destination)
    {
      // we do a first test to see if there's room enough to move to the destination
      Collider2D hit = UnityEngine.Physics2D.OverlapBox(destination, _boxCollider.size, this.transform.rotation.eulerAngles.z, PlatformMask);

      if (hit == null)
      {
        return destination;
      }
      else
      {
        // if the original destination wasn't safe, we find the closest safe point between our controller and the obstacle
        destination -= Parameters.RayOffset * (Vector2)(hit.transform.position - this.transform.position).normalized;
        hit = UnityEngine.Physics2D.OverlapBox(destination, _boxCollider.size, this.transform.rotation.eulerAngles.z, PlatformMask);

        if (hit == null)
        {
          return destination;
        }
        else
        {
          return this.transform.position;
        }
      }
    }

    /// <summary>
    /// Teleports the character to the ground
    /// </summary>
    public void AnchorToGround()
    {
      ComputeDistanceToTheGround();
      if (_distanceToTheGround > 0f)
      {
        Vector2 newPosition;
        newPosition.x = this.transform.position.x;
        newPosition.y = this.transform.position.y - _distanceToTheGround;
        SetTransformPosition(newPosition);

        State.IsFalling = false;
        State.IsCollidingBelow = true;
        _speed = Vector2.zero;
        _externalForce = Vector2.zero;
      }
    }

    public void Flip()
    {
      // transform.localScale = new Vector3(-_transform.localScale.x, _transform.localScale.y, _transform.localScale.z);
      transform.Rotate(0f, 180f, 0f);
      State.IsFacingRight = _transform.right.x > 0;
    }

    public bool RotateTowards(Transform target)
    {
      if (!State.IsFacingRight && target.position.x > _transform.position.x)
      {
        Flip();
        return true;
      }
      else if (State.IsFacingRight && target.position.x < _transform.position.x)
      {
        Flip();
        return true;
      }
      return false;
    }

    private void RotateOnMouseCursor()
    {
      var mousePos = (Vector2)_mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
      if (State.IsFacingRight && mousePos.x < _transform.position.x)
      {
        Flip();
      }
      else if (!State.IsFacingRight && mousePos.x > _transform.position.x)
      {
        Flip();
      }
    }

    public void SetOverrideParameters(CharacterControllerParameters2D parameters)
    {
      OverrideParameters = parameters;
    }

    #region Events
    /// <summary>
    /// triggered when the character's raycasts collide with something 
    /// </summary>
    protected void OnColliderHit()
    {
      foreach (RaycastHit2D hit in _contactList)
      {
        if (Parameters.Physics2DInteraction)
        {
          Rigidbody2D body = hit.collider.attachedRigidbody;
          if (body == null || body.isKinematic || body.bodyType == RigidbodyType2D.Static)
          {
            return;
          }
          Vector3 pushDirection = new Vector3(_externalForce.x, 0, 0);
          body.velocity = pushDirection.normalized * Parameters.Physics2DPushForce;
        }
      }
    }

    /// <summary>
    /// triggered when the character enters a collider
    /// </summary>
    /// <param name="collider">the object we're colliding with.</param> 
    protected void OnTriggerEnter2D(Collider2D collider)
    {
      // CorgiControllerPhysicsVolume2D parameters = collider.gameObject.MMGetComponentNoAlloc<CorgiControllerPhysicsVolume2D>();
      // if (parameters != null)
      // {
      //   // if the object we're colliding with has parameters, we apply them to our character.
      //   _overrideParameters = parameters.ControllerParameters;
      //   if (parameters.ResetForcesOnEntry)
      //   {
      //     SetForce(Vector2.zero);
      //   }
      //   if (parameters.MultiplyForcesOnEntry)
      //   {
      //     SetForce(Vector2.Scale(parameters.ForceMultiplierOnEntry, Speed));
      //   }
      // }
    }

    /// <summary>
    /// triggered while the character stays inside another collider
    /// </summary>
    /// <param name="collider">the object we're colliding with.</param>
    protected void OnTriggerStay2D(Collider2D collider)
    {
    }

    /// <summary>
    /// triggered when the character exits a collider
    /// </summary>
    /// <param name="collider">the object we're colliding with.</param>
    protected void OnTriggerExit2D(Collider2D collider)
    {
      // CorgiControllerPhysicsVolume2D parameters = collider.gameObject.MMGetComponentNoAlloc<CorgiControllerPhysicsVolume2D>();
      // if (parameters != null)
      // {
      //   // if the object we were colliding with had parameters, we reset our character's parameters
      //   _overrideParameters = null;
      // }
    }
    #endregion

  }
}