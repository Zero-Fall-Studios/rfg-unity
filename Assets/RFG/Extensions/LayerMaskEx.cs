using UnityEngine;

namespace RFG
{
  public static class LayerMaskEx
  {
    public static bool Contains(this LayerMask mask, int layer)
    {
      return ((mask.value & (1 << layer)) > 0);
    }

    public static bool Contains(this LayerMask mask, GameObject gameObject)
    {
      return ((mask.value & (1 << gameObject.layer)) > 0);
    }
  }
}