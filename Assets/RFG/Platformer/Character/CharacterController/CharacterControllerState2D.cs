namespace RFG
{
  public class CharacterControllerState2D
  {
    public bool IsCollidingRight { get; set; }
    public bool IsCollidingLeft { get; set; }
    public bool IsCollidingAbove { get; set; }
    public bool IsCollidingBelow { get; set; }
    public bool IsMovingDownSlope { get; set; }
    public bool IsMovingUpSlope { get; set; }
    public bool IsGrounded { get { return IsCollidingBelow; } }
    public bool JustGotGrounded { get; set; }
    public bool WasGroundedLastFrame { get; set; }
    public bool WasTouchingTheCeilingLastFrame { get; set; }
    /// returns the slope angle met horizontally
    public float LateralSlopeAngle { get; set; }
    /// returns the slope the character is moving on angle
    public float BelowSlopeAngle { get; set; }
    /// returns true if the slope angle is ok to walk on
    public bool SlopeAngleOK { get; set; }
    public bool HasCollisions { get { return IsCollidingRight || IsCollidingLeft || IsCollidingAbove || IsCollidingBelow; } }
    /// returns the distance to the left collider, equals -1 if not colliding left
    public float DistanceToLeftCollider;
    /// returns the distance to the right collider, equals -1 if not colliding right
    public float DistanceToRightCollider;
    public bool IsFacingRight { get; set; }
    public bool IsFalling { get; set; }
    public bool IsDangling { get; set; }
    public bool IsJumping { get; set; }
    public bool IsStandingOnStairs { get; set; }
    public bool IsWalking { get; set; }
    public bool IsRunning { get; set; }
    public bool IsWallClinging { get; set; }
    public bool IsWallJumping { get; set; }
    public bool OnAMovingPlatform { get; set; }
    public bool ColliderResized { get; set; }
    public bool TouchingLevelBounds { get; set; }

    public void Reset()
    {
      IsCollidingLeft = false;
      IsCollidingRight = false;
      IsCollidingAbove = false;
      DistanceToLeftCollider = -1;
      DistanceToRightCollider = -1;
      SlopeAngleOK = false;
      JustGotGrounded = false;
      IsFalling = true;
      LateralSlopeAngle = 0;
    }

  }
}
