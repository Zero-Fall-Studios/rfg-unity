using UnityEngine;
using MyBox;

namespace RFG
{
  [AddComponentMenu("RFG/Utils/Bounds/Bounds Resizer")]
  public class BoundsResizer : MonoBehaviour
  {
    public Bounds Bounds;

#if UNITY_EDITOR
    [ButtonMethod]
    private void ResizeToLevelBounds()
    {
      transform.localScale = Bounds.size;
      transform.position = Bounds.center;
    }
#endif
  }
}