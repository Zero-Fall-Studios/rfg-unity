using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [Serializable]
  public class Aim
  {
    public enum AimControls { Off, PrimaryMovement, Mouse }

    public enum RotationModes { Free, Strict4Directions, Strict8Directions }

    [Header("Control Mode")]
    public AimControls aimControl = AimControls.PrimaryMovement;
    public RotationModes rotationMode = RotationModes.Free;

    [Header("Limits")]
    [Range(-180, 180)]
    public float minAngle = -180f;
    [Range(-180, 180)]
    public float maxAngle = 180f;
    public float currentAngle;

    public Vector3 CurrentPosition { get; set; }
    public Vector3 PrimaryMovement { get; set; }

    private float[] _possibleAngleValues;
    private Vector3 _currentAim = Vector3.zero;
    private Vector3 _direction;
    private Vector3 _mousePosition;
    private Camera _mainCamera;

    public void Init()
    {
      if (rotationMode == RotationModes.Strict4Directions)
      {
        _possibleAngleValues = new float[5];
        _possibleAngleValues[0] = -180f;
        _possibleAngleValues[1] = -90f;
        _possibleAngleValues[2] = 0f;
        _possibleAngleValues[3] = 90f;
        _possibleAngleValues[4] = 180f;
      }
      else if (rotationMode == RotationModes.Strict8Directions)
      {
        _possibleAngleValues = new float[9];
        _possibleAngleValues[0] = -180f;
        _possibleAngleValues[1] = -135f;
        _possibleAngleValues[2] = -90f;
        _possibleAngleValues[3] = -45f;
        _possibleAngleValues[4] = 0f;
        _possibleAngleValues[5] = 45f;
        _possibleAngleValues[6] = 90f;
        _possibleAngleValues[7] = 135f;
        _possibleAngleValues[8] = 180f;
      }
      _mainCamera = Camera.main;
    }

    public Vector2 GetCurrentAim()
    {
      switch (aimControl)
      {
        case AimControls.PrimaryMovement:
          _currentAim = PrimaryMovement;
          break;
        case AimControls.Mouse:
          _mousePosition = Mouse.current.position.ReadValue();
          _mousePosition.z = 10;
          _direction = _mainCamera.ScreenToWorldPoint(_mousePosition);
          _direction.z = CurrentPosition.z;
          _currentAim = _direction - CurrentPosition;
          break;
        case AimControls.Off:
        default:
          _currentAim = Vector2.zero;
          break;
      }

      currentAngle = Mathf.Atan2(_currentAim.y, _currentAim.x) * Mathf.Rad2Deg;

      if (currentAngle < minAngle || currentAngle > maxAngle)
      {
        float minAngleDiff = Mathf.DeltaAngle(currentAngle, minAngle);
        float maxAngleDiff = Mathf.DeltaAngle(currentAngle, maxAngle);
        currentAngle = Mathf.Abs(minAngleDiff) < Mathf.Abs(maxAngleDiff) ? minAngle : maxAngle;
      }

      if (rotationMode == RotationModes.Strict4Directions || rotationMode == RotationModes.Strict8Directions)
      {
        currentAngle = RFG.Math.RoundToClosest(currentAngle, _possibleAngleValues);
      }

      currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

      _currentAim = _currentAim.magnitude == 0f ? Vector2.zero : RFG.Math.RotateVector2(Vector2.right, currentAngle);

      return _currentAim;
    }

    public void SetAim(Vector2 newAim)
    {
      _currentAim = newAim;
    }

  }
}