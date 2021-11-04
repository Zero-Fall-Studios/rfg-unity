using UnityEngine;

public static class TransformEx
{
  public static Transform DestroyChildren(this Transform transform)
  {
    foreach (Transform child in transform)
    {
      GameObject.Destroy(child.gameObject);
    }
    return transform;
  }
}