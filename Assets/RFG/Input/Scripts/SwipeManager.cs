using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  public enum SwipeDirection
  {
    None, // Basically means an invalid swipe
    Up,
    UpRight,
    Right,
    DownRight,
    Down,
    DownLeft,
    Left,
    UpLeft
  }

  public class SwipeManager : MonoBehaviour
  {
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;
    public delegate void EndTouch(Vector2 position, float time);
    public event EndTouch OnEndTouch;
    public delegate void SwipeEvent(SwipeDirection swipeDirection);
    public event SwipeEvent OnSwipe;
    public delegate void TapEvent(Vector2 position, float time);
    public event TapEvent OnTap;

    [field: SerializeField] private float MinimumDistance { get; set; } = .2f;
    [field: SerializeField] private float MaximumTime { get; set; } = 1f;
    [field: SerializeField, Range(0f, 1f)] private float DirectionThreshold { get; set; } = .9f;
    [field: SerializeField] private InputActionReference PrimaryContact { get; set; }
    [field: SerializeField] private InputActionReference PrimaryPosition { get; set; }
    [field: SerializeField] private InputActionReference PrimaryTap { get; set; }
    [field: SerializeField] private GameObject SwipeTrail { get; set; }

    private Camera _mainCam;
    private Vector2 _startPosition;
    private float _startTime;
    private Vector2 _endPosition;
    private float _endTime;
    private Coroutine _coroutine;

    #region Unity Methods
    private void Awake()
    {
      _mainCam = Camera.main;
    }

    private void OnEnable()
    {
      PrimaryContact.action.Enable();
      PrimaryPosition.action.Enable();
      PrimaryTap.action.Enable();
      PrimaryContact.action.started += OnStartTouchPrimary;
      PrimaryContact.action.canceled += OnEndTouchPrimary;
      PrimaryTap.action.performed += HandleTap;
    }

    private void OnDisable()
    {
      PrimaryContact.action.Disable();
      PrimaryPosition.action.Disable();
      PrimaryTap.action.Disable();
      PrimaryContact.action.started -= OnStartTouchPrimary;
      PrimaryContact.action.canceled -= OnEndTouchPrimary;
      PrimaryTap.action.performed -= HandleTap;
    }
    #endregion

    public Vector2 GetPrimaryPosition()
    {
      Vector3 position = PrimaryPosition.action.ReadValue<Vector2>();
      position.z = _mainCam.nearClipPlane;
      Vector3 worldPosition = _mainCam.ScreenToWorldPoint(position);
      return worldPosition;
    }

    private void OnStartTouchPrimary(InputAction.CallbackContext ctx)
    {
      _startPosition = GetPrimaryPosition();
      _startTime = (float)ctx.startTime;
      if (OnStartTouch != null)
      {
        OnStartTouch(_startPosition, _startTime);
      }
      if (SwipeTrail != null)
      {
        SwipeTrail.SetActive(true);
        SwipeTrail.transform.position = _startPosition;
        _coroutine = StartCoroutine(UpdateTrail());
      }
    }

    private void OnEndTouchPrimary(InputAction.CallbackContext ctx)
    {
      _endPosition = GetPrimaryPosition();
      _endTime = (float)ctx.time;
      if (OnEndTouch != null)
      {
        OnStartTouch(_endPosition, _endTime);
      }
      DetectSwipe();
      if (SwipeTrail != null)
      {
        SwipeTrail.SetActive(false);
        StopCoroutine(_coroutine);
      }
    }

    private IEnumerator UpdateTrail()
    {
      while (true)
      {
        SwipeTrail.transform.position = GetPrimaryPosition();
        yield return null;
      }
    }

    private void DetectSwipe()
    {
      if (Vector3.Distance(_startPosition, _endPosition) >= MinimumDistance && _endTime - _startTime <= MaximumTime)
      {
        HandleSwipe();
      }
    }

    private void HandleSwipe()
    {
      // Debug.DrawLine(_startPosition, _endPosition, Color.red, 5f);
      Vector3 direction = _endPosition - _startPosition;
      Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
      SwipeDirection swipeDirection = GetSwipeDirection(direction2D);
      if (OnSwipe != null)
      {
        OnSwipe(swipeDirection);
      }
    }

    public SwipeDirection GetSwipeDirection(Vector2 direction)
    {
      var angle = Vector2.Angle(Vector2.up, direction.normalized); // Degrees
      var swipeDirection = SwipeDirection.None;

      if (direction.x > 0) // Right
      {
        if (angle < 22.5f) // 0.0 - 22.5
        {
          swipeDirection = SwipeDirection.Up;
        }
        else if (angle < 67.5f) // 22.5 - 67.5
        {
          swipeDirection = SwipeDirection.UpRight;
        }
        else if (angle < 112.5f) // 67.5 - 112.5
        {
          swipeDirection = SwipeDirection.Right;
        }
        else if (angle < 157.5f) // 112.5 - 157.5
        {
          swipeDirection = SwipeDirection.DownRight;
        }
        else if (angle < 180.0f) // 157.5 - 180.0
        {
          swipeDirection = SwipeDirection.Down;
        }
      }
      else // Left
      {
        if (angle < 22.5f) // 0.0 - 22.5
        {
          swipeDirection = SwipeDirection.Up;
        }
        else if (angle < 67.5f) // 22.5 - 67.5
        {
          swipeDirection = SwipeDirection.UpLeft;
        }
        else if (angle < 112.5f) // 67.5 - 112.5
        {
          swipeDirection = SwipeDirection.Left;
        }
        else if (angle < 157.5f) // 112.5 - 157.5
        {
          swipeDirection = SwipeDirection.DownLeft;
        }
        else if (angle < 180.0f) // 157.5 - 180.0
        {
          swipeDirection = SwipeDirection.Down;
        }
      }

      return swipeDirection;
    }

    private void HandleTap(InputAction.CallbackContext ctx)
    {
      if (OnTap != null)
      {
        OnTap(GetPrimaryPosition(), (float)ctx.time);
      }
    }
  }
}