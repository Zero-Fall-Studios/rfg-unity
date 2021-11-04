using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Character Controller Parameters 2D", menuName = "RFG/Platformer/Character/Character Controller Parameters 2D")]
  public class CharacterControllerParameters2D : ScriptableObject
  {
    [Header("Gravity")]

    /// The force to apply vertically at all times
    [Tooltip("The force to apply vertically at all times")]
    public float Gravity = -25f;

    /// a multiplier applied to the character's gravity when going down
    [Tooltip("a multiplier applied to the character's gravity when going down")]
    public float FallMultiplier = 1f;

    /// a multiplier applied to the character's gravity when going up
    [Tooltip("a multiplier applied to the character's gravity when going up")]
    public float AscentMultiplier = 1f;

    [Header("Speed")]
    public Vector2 MaxVelocity = new Vector2(100f, 100f);
    public float SpeedFactor = 3f;
    public float GroundSpeedFactor = 10f;
    public float AirSpeedFactor = 5f;

    [Header("Slopes")]
    [Range(0, 90)]
    public float MaxSlopeAngle = 30f;

    /// the speed multiplier to apply when walking on a slope
    [Tooltip("the speed multiplier to apply when walking on a slope")]
    public AnimationCurve SlopeAngleSpeedFactor = new AnimationCurve(new Keyframe(-90f, 1f), new Keyframe(0f, 1f), new Keyframe(90f, 1f));

    /// Raycasting

    [Header("Raycasting")]

    [Tooltip("The number of rays cast horizontally")]
    public int NumberOfHorizontalRays = 4;

    [Tooltip("The number of rays cast vertically")]
    public int NumberOfVerticalRays = 4;

    [Tooltip("A small value added to all raycasts to accomodate for edge cases")]
    public float RayOffset = 0.05f;

    [Tooltip("The length of the raycasts used to get back to normal size will be auto generated based on your characters normal standing height, but here you can specify a different value")]
    public float CrouchedRaycastLengthMultiplier = 1f;

    [Tooltip("If this is true rays will cast on both sides, otherwise only in the current movements direction")]
    public bool CastRaysOnBothSides = false;

    [Tooltip("The maximum length of the ray used to detect the distance to the ground")]
    public float DistanceToTheGroundRayMaximumLength = 100f;

    [Tooltip("If this is true, en extra boxcast will be performed to prevent going through a platform")]
    public bool PerformSafetyBoxcast = false;

    /// Stickiness

    [Header("Stickiness")]

    [Tooltip("If this is true, the character will stick to slopes when walking down them")]
    public bool StickToSlopes = false;

    [Tooltip("The length of the raycasts used to stick to downward slopes")]
    public float StickyRaycastLength = 0f;

    [Tooltip("The movements Y offset to evaluate for stickiness")]
    public float StickToSlopesOffsetY = 0.2f;

    /// Safety

    [Header("Safety")]

    [Tooltip("If this is true, gravitiy ability settings will be automatically set. It's recommended to set this to true.")]
    public bool AutomaticGravtiySettings = true;

    [Tooltip("Whether or not to perform additional checks when setting the transform's position. Slightly more expensive in terms of performance, but also safer.")]
    public bool SafeSetTransform = false;

    [Tooltip("If this is true, this controller will set a number of physics settings automatically on init, to ensure they're correct")]
    public bool AutomaticallySetPhysicsSettings = false;

    /// Physics

    [Header("Physics2D Interaction [Experimental]")]

    /// if set to true, the character will transfer its force to all the rigidbodies it collides with horizontally
    [Tooltip("if set to true, the character will transfer its force to all the rigidbodies it collides with horizontally")]
    public bool Physics2DInteraction = true;
    /// the force applied to the objects the character encounters
    [Tooltip("the force applied to the objects the character encounters")]
    public float Physics2DPushForce = 2.0f;
    public float Weight = 1f;

  }
}