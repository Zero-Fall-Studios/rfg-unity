using System;
using UnityEngine;
using MyBox;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.SceneGraph
{
  [AddComponentMenu("RFG/Scene Graph/Scene Bounds")]
  public class SceneBounds : MonoBehaviour
  {
    public event Action<Vector2, Transform> OnBoundsTop;
    public event Action<Vector2, Transform> OnBoundsBottom;
    public event Action<Vector2, Transform> OnBoundsLeft;
    public event Action<Vector2, Transform> OnBoundsRight;

    public Bounds bounds;
    private Scene _scene;
    private Vector2 _constrainedPosition = Vector2.zero;
    private Transform _transform;

    private void Awake()
    {
      _transform = transform;
      _scene = SceneGraphManager.Instance.CurrentScene;
    }

    public void HandleSceneBounds(Transform _transform, Vector2 size, float minX, float maxX, float minY, float maxY)
    {
      if (_scene.bounds.size == Vector3.zero)
        return;

      if (OnBoundsTop != null && maxY > _scene.bounds.max.y)
      {
        _constrainedPosition.x = transform.position.x;
        _constrainedPosition.y = _scene.bounds.max.y - Mathf.Abs(size.y) / 2;
        ApplyBoundsBehaviour(OnBoundsTop, _constrainedPosition, _transform);
      }

      if (OnBoundsBottom != null && minY < _scene.bounds.min.y)
      {
        _constrainedPosition.x = _transform.position.x;
        _constrainedPosition.y = _scene.bounds.min.y + Mathf.Abs(size.y) / 2;
        ApplyBoundsBehaviour(OnBoundsBottom, _constrainedPosition, _transform);
      }

      if (OnBoundsRight != null && maxX > _scene.bounds.max.x)
      {
        _constrainedPosition.x = _scene.bounds.max.x - Mathf.Abs(size.x) / 2;
        _constrainedPosition.y = _transform.position.y;
        ApplyBoundsBehaviour(OnBoundsRight, _constrainedPosition, _transform);
      }

      if (OnBoundsLeft != null && minX < _scene.bounds.min.x)
      {
        _constrainedPosition.x = _scene.bounds.min.x + Mathf.Abs(size.x) / 2;
        _constrainedPosition.y = _transform.position.y;
        ApplyBoundsBehaviour(OnBoundsLeft, _constrainedPosition, _transform);
      }

    }

    private void ApplyBoundsBehaviour(Action<Vector2, Transform> action, Vector2 constrainedPosition, Transform _transform)
    {
      action?.Invoke(constrainedPosition, _transform);
    }

#if UNITY_EDITOR
    [ButtonMethod]
    private void CopyFromSelection()
    {
      if (_scene == null)
      {
        _scene = SceneGraphManager.Instance.CurrentScene;
      }
      PolygonCollider2D collider = Selection.activeGameObject.GetComponent<PolygonCollider2D>();
      if (collider != null)
      {
        _scene.bounds.min = new Vector3(collider.points[0].x, collider.points[0].y, 0);
        _scene.bounds.max = new Vector3(collider.points[2].x, collider.points[2].y, 0);
      }
      EditorUtility.SetDirty(gameObject);
    }

    [ButtonMethod]
    private void GeneratePolygonCollider2DToSelection()
    {
      if (_scene == null)
      {
        _scene = SceneGraphManager.Instance.CurrentScene;
      }
      PolygonCollider2D collider = Selection.activeGameObject.AddComponent<PolygonCollider2D>();
      Vector2[] points = new Vector2[]
      {
        new Vector2(_scene.bounds.min.x, _scene.bounds.min.y),
        new Vector2(_scene.bounds.min.x, _scene.bounds.max.y),
        new Vector2(_scene.bounds.max.x, _scene.bounds.max.y),
        new Vector2(_scene.bounds.max.x, _scene.bounds.min.y),
      };
      collider.SetPath(0, points);
      EditorUtility.SetDirty(Selection.activeGameObject);
    }

    private void OnDrawGizmos()
    {
      if (_scene == null)
      {
        _scene = SceneGraphManager.Instance.CurrentScene;
      }
      var b = bounds;
      var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
      var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
      var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
      var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

      Gizmos.DrawLine(p1, p2);
      Gizmos.DrawLine(p2, p3);
      Gizmos.DrawLine(p3, p4);
      Gizmos.DrawLine(p4, p1);

      // top
      var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
      var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
      var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
      var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

      Gizmos.DrawLine(p5, p6);
      Gizmos.DrawLine(p6, p7);
      Gizmos.DrawLine(p7, p8);
      Gizmos.DrawLine(p8, p5);

      // sides
      Gizmos.DrawLine(p1, p5);
      Gizmos.DrawLine(p2, p6);
      Gizmos.DrawLine(p3, p7);
      Gizmos.DrawLine(p4, p8);
    }
#endif

  }
}