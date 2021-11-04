using UnityEngine;
#if UNITY_EDITOR
using MyBox;
using UnityEditor;
#endif

namespace RFG
{
  public enum JumpRestrictions
  {
    CanJumpOnGround,
    CanJumpAnywhere,
    CantJump
  }

  [CreateAssetMenu(fileName = "New Settings Pack", menuName = "RFG/Platformer/Character/Packs/Settings")]
  public class SettingsPack : ScriptableObject
  {
    [Header("Attack Settings")]
    public float AttackSpeed = 5f;

    [Header("Dangling Settings")]

    /// the origin of the raycast used to detect pits. This is relative to the transform.position of our character
    [Tooltip("the origin of the raycast used to detect pits. This is relative to the transform.position of our character")]
    public Vector3 DanglingRaycastOrigin = new Vector3(0.7f, -0.25f, 0f);

    /// the length of the raycast used to detect pits
    [Tooltip("the length of the raycast used to detect pits")]
    public float DanglingRaycastLength = 2f;
    public bool CanDangle = true;

    [Header("Dash")]
    public float DashDistance = 3f;
    public float DashForce = 40f;
    public int TotalDashes = 2;
    public float MinInputThreshold = 0.1f;
    public float Cooldown = 1f;

    [Header("Jump Settings")]

    /// <summary>How high can the controller jump</summary>
    [Tooltip("How high can the controller jump")]
    public float JumpHeight = 12f;
    public float OneWayPlatformFallVelocity = -10f;

    /// the minimum horizontal and vertical value you need to reach to trigger movement on an analog controller (joystick for example)
    [Tooltip("the minimum horizontal and vertical value you need to reach to trigger movement on an analog controller (joystick for example)")]
    public Vector2 JumpThreshold = new Vector2(0.1f, 0.4f);

    /// duration (in seconds) we need to disable collisions when jumping down a 1 way platform
    [Tooltip("duration (in seconds) we need to disable collisions when jumping down a 1 way platform")]
    public float OneWayPlatformsJumpCollisionOffDuration = 0.3f;

    /// duration (in seconds) we need to disable collisions when jumping off a moving platform
    [Tooltip("duration (in seconds) we need to disable collisions when jumping off a moving platform")]
    public float MovingPlatformsJumpCollisionOffDuration = 0.05f;

    public JumpRestrictions Restrictions;
    public int NumberOfJumps = 1;
    public bool CanJumpDownOneWayPlatforms = true;
    public bool JumpIsProportionalToThePressTime = true;
    public float JumpMinAirTime = 0.1f;
    public float JumpReleaseForceFactor = 2f;

    [Header("Pause Settings")]
    public GameEvent PauseEvent;

    [Header("Running Settings")]
    public float RunningSpeed = 5f;
    public float RunningPower = 5f;
    public float PowerGainPerFrame = .01f;
    public float CooldownTimer = 5f;

    [Header("Input Setting")]
    /// the minimum horizontal and vertical value you need to reach to trigger movement on an analog controller (joystick for example)
    [Tooltip("the minimum horizontal and vertical value you need to reach to trigger movement on an analog controller (joystick for example)")]
    public Vector2 Threshold = new Vector2(0.1f, 0.4f);

    /// the offset to apply when raycasting for stairs
    [Tooltip("the offset to apply when raycasting for stairs")]
    public Vector3 StairsAheadDetectionRaycastOrigin = new Vector3(-2f, 0f, 0f);

    /// the length of the raycast looking for stairs
    [Tooltip("the length of the raycast looking for stairs")]
    public float StairsAheadDetectionRaycastLength = 4f;

    /// the offset to apply when raycasting for stairs
    [Tooltip("the offset to apply when raycasting for stairs")]
    public Vector3 StairsBelowDetectionRaycastOrigin = new Vector3(-0.2f, 0f, 0f);

    /// the length of the raycast looking for stairs
    [Tooltip("the length of the raycast looking for stairs")]
    public float StairsBelowDetectionRaycastLength = 0.5f;

    /// the duration, in seconds, during which collisions with one way platforms should be ignored when starting to get down a stair
    [Tooltip("the duration, in seconds, during which collisions with one way platforms should be ignored when starting to get down a stair")]
    public float StairsBelowLockTime = 0.2f;

    [Header("Walking Settings")]
    public float WalkingSpeed = 5f;

    [Header("Wall Clinging Settings")]
    [Range(0.01f, 1f)]
    public float WallClingingSlowFactor = 0.6f;
    public float RaycastVerticalOffset = 0f;
    public float WallClingingTolerance = 0.3f;
    public float WallClingingInputThreshold = 0.1f;

    [Header("Wall Jump Settings")]
    public float WallJumpInputThreshold = 0.01f;
    public Vector2 WallJumpForce = new Vector2(10f, 4f);

    [Header("AI Settings")]
    public bool CanFollowVertically = false;

#if UNITY_EDITOR
    [ButtonMethod]
    private void CopyFromSelection()
    {
      SettingsPack from = Selection.activeObject as SettingsPack;

      AttackSpeed = from.AttackSpeed;
      DanglingRaycastOrigin = from.DanglingRaycastOrigin;
      DanglingRaycastLength = from.DanglingRaycastLength;
      DashDistance = from.DashDistance;
      DashForce = from.DashForce;
      TotalDashes = from.TotalDashes;
      MinInputThreshold = from.MinInputThreshold;
      Cooldown = from.Cooldown;
      JumpHeight = from.JumpHeight;
      OneWayPlatformFallVelocity = from.OneWayPlatformFallVelocity;
      JumpThreshold = from.JumpThreshold;
      OneWayPlatformsJumpCollisionOffDuration = from.OneWayPlatformsJumpCollisionOffDuration;
      MovingPlatformsJumpCollisionOffDuration = from.MovingPlatformsJumpCollisionOffDuration;
      Restrictions = from.Restrictions;
      NumberOfJumps = from.NumberOfJumps;
      CanJumpDownOneWayPlatforms = from.CanJumpDownOneWayPlatforms;
      JumpIsProportionalToThePressTime = from.JumpIsProportionalToThePressTime;
      JumpMinAirTime = from.JumpMinAirTime;
      JumpReleaseForceFactor = from.JumpReleaseForceFactor;
      PauseEvent = from.PauseEvent;
      RunningSpeed = from.RunningSpeed;
      RunningPower = from.RunningPower;
      PowerGainPerFrame = from.PowerGainPerFrame;
      CooldownTimer = from.CooldownTimer;
      Threshold = from.Threshold;
      StairsAheadDetectionRaycastOrigin = from.StairsAheadDetectionRaycastOrigin;
      StairsAheadDetectionRaycastLength = from.StairsAheadDetectionRaycastLength;
      StairsBelowDetectionRaycastOrigin = from.StairsBelowDetectionRaycastOrigin;
      StairsBelowDetectionRaycastLength = from.StairsBelowDetectionRaycastLength;
      StairsBelowLockTime = from.StairsBelowLockTime;
      WalkingSpeed = from.WalkingSpeed;
      WallClingingSlowFactor = from.WallClingingSlowFactor;
      RaycastVerticalOffset = from.RaycastVerticalOffset;
      WallClingingTolerance = from.WallClingingTolerance;
      WallClingingInputThreshold = from.WallClingingInputThreshold;
      WallJumpInputThreshold = from.WallJumpInputThreshold;
      WallJumpForce = from.WallJumpForce;

      EditorUtility.SetDirty(this);
    }
#endif

  }

}