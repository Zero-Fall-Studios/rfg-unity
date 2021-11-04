using UnityEngine;

namespace RFG
{
  public static class Physics2D
  {

#if UNITY_EDITOR
    private static bool _debugDrawEnabledSet = false;
#endif
    private static bool _debugDrawEnabled = false;

    private const string _editorPrefsDebugDraws = "DebugDrawsEnabled";

    public static bool DebugDrawEnabled
    {
      get
      {
#if UNITY_EDITOR
        if (_debugDrawEnabledSet)
        {
          return _debugDrawEnabled;
        }

        if (PlayerPrefs.HasKey(_editorPrefsDebugDraws))
        {
          _debugDrawEnabled = (PlayerPrefs.GetInt(_editorPrefsDebugDraws) == 0) ? false : true;
        }
        else
        {
          _debugDrawEnabled = true;
        }
        _debugDrawEnabledSet = true;
        return _debugDrawEnabled;
#else
        return _debugDrawEnabled;
#endif
      }
      private set { }
    }

    public static RaycastHit2D RayCast(Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false)
    {
      if (drawGizmo && DebugDrawEnabled)
      {
        Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
      }
      return UnityEngine.Physics2D.Raycast(rayOriginPoint, rayDirection, rayDistance, mask);
    }

    public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float length, LayerMask mask, Color color, bool drawGizmo = false)
    {
      if (drawGizmo && DebugDrawEnabled)
      {
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        Vector3[] points = new Vector3[8];

        float halfSizeX = size.x / 2f;
        float halfSizeY = size.y / 2f;

        points[0] = rotation * (origin + (Vector2.left * halfSizeX) + (Vector2.up * halfSizeY)); // top left
        points[1] = rotation * (origin + (Vector2.right * halfSizeX) + (Vector2.up * halfSizeY)); // top right
        points[2] = rotation * (origin + (Vector2.right * halfSizeX) - (Vector2.up * halfSizeY)); // bottom right
        points[3] = rotation * (origin + (Vector2.left * halfSizeX) - (Vector2.up * halfSizeY)); // bottom left

        points[4] = rotation * ((origin + Vector2.left * halfSizeX + Vector2.up * halfSizeY) + length * direction); // top left
        points[5] = rotation * ((origin + Vector2.right * halfSizeX + Vector2.up * halfSizeY) + length * direction); // top right
        points[6] = rotation * ((origin + Vector2.right * halfSizeX - Vector2.up * halfSizeY) + length * direction); // bottom right
        points[7] = rotation * ((origin + Vector2.left * halfSizeX - Vector2.up * halfSizeY) + length * direction); // bottom left

        Debug.DrawLine(points[0], points[1], color);
        Debug.DrawLine(points[1], points[2], color);
        Debug.DrawLine(points[2], points[3], color);
        Debug.DrawLine(points[3], points[0], color);

        Debug.DrawLine(points[4], points[5], color);
        Debug.DrawLine(points[5], points[6], color);
        Debug.DrawLine(points[6], points[7], color);
        Debug.DrawLine(points[7], points[4], color);

        Debug.DrawLine(points[0], points[4], color);
        Debug.DrawLine(points[1], points[5], color);
        Debug.DrawLine(points[2], points[6], color);
        Debug.DrawLine(points[3], points[7], color);

      }
      return UnityEngine.Physics2D.BoxCast(origin, size, angle, direction, length, mask);
    }
  }
}