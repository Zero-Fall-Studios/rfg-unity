using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  public enum LedgeGrabDirection { Left, Right }

  [AddComponentMenu("RFG/Platformer/Interactions/Ledge")]
  public class Ledge : MonoBehaviour
  {
    public LedgeGrabDirection LedgeGrabDirection = LedgeGrabDirection.Left;
    public Vector3 HangOffset;
    public Vector3 ClimbOffset;

    [field: SerializeField] private List<string> Tags { get; set; }

    private void OnTriggerEnter2D(Collider2D collider)
    {
      if (collider.gameObject.CompareTags(Tags))
      {
        LedgeGrabAbility ledgeGrabAbility = collider.GetComponent<LedgeGrabAbility>();
        if (ledgeGrabAbility != null)
        {
          ledgeGrabAbility.StartGrabbingLedge(this);
        }
      }
    }

    private void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position + HangOffset, 0.1f);
      Gizmos.color = Color.magenta;
      Gizmos.DrawWireSphere(transform.position + ClimbOffset, 0.1f);
    }
  }
}
