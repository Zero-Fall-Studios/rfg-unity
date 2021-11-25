using UnityEngine;
using MyBox;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG
{
  using SceneGraph;

  [AddComponentMenu("RFG/Platformer/Character/Behaviours/Scene Bounds")]
  public class SceneBoundsBehaviour : MonoBehaviour
  {
    public enum BoundsBehaviour { Nothing, Constrain, Kill }

    [Header("Bounds Behaviour")]
    public BoundsBehaviour Top = BoundsBehaviour.Constrain;
    public BoundsBehaviour Bottom = BoundsBehaviour.Kill;
    public BoundsBehaviour Left = BoundsBehaviour.Constrain;
    public BoundsBehaviour Right = BoundsBehaviour.Constrain;
    public Bounds Bounds = new Bounds(Vector3.zero, Vector3.one * 10);

    private Vector2 _constrainedPosition = Vector2.zero;
    private Character _character;

    private void Awake()
    {
      _character = GetComponent<Character>();
    }

    private void LateUpdate()
    {
      if (_character.CharacterState.CurrentStateType != typeof(AliveState))
        return;

      HandleLevelBounds(_character);
    }

    public void HandleLevelBounds(Character _character)
    {
      _character.Controller.State.TouchingLevelBounds = false;
      if (Bounds.size != Vector3.zero)
      {
        if (Top != BoundsBehaviour.Nothing && _character.Controller.ColliderTopPosition.y > Bounds.max.y)
        {
          _constrainedPosition.x = _character.transform.position.x;
          _constrainedPosition.y = Bounds.max.y - Mathf.Abs(_character.Controller.ColliderSize.y) / 2;
          ApplyBoundsBehaviour(Top, _constrainedPosition, _character);
        }

        if (Bottom != BoundsBehaviour.Nothing && _character.Controller.ColliderBottomPosition.y < Bounds.min.y)
        {
          _constrainedPosition.x = _character.transform.position.x;
          _constrainedPosition.y = Bounds.min.y + Mathf.Abs(_character.Controller.ColliderSize.y) / 2;
          ApplyBoundsBehaviour(Bottom, _constrainedPosition, _character);
        }

        if (Right != BoundsBehaviour.Nothing && _character.Controller.ColliderRightPosition.x > Bounds.max.x)
        {
          _constrainedPosition.x = Bounds.max.x - Mathf.Abs(_character.Controller.ColliderSize.x) / 2;
          _constrainedPosition.y = _character.transform.position.y;
          ApplyBoundsBehaviour(Right, _constrainedPosition, _character);
        }

        if (Left != BoundsBehaviour.Nothing && _character.Controller.ColliderLeftPosition.x < Bounds.min.x)
        {
          _constrainedPosition.x = Bounds.min.x + Mathf.Abs(_character.Controller.ColliderSize.x) / 2;
          _constrainedPosition.y = _character.transform.position.y;
          ApplyBoundsBehaviour(Left, _constrainedPosition, _character);
        }
      }
    }

    private void ApplyBoundsBehaviour(BoundsBehaviour Behaviour, Vector2 constrainedPosition, Character _character)
    {
      _character.Controller.State.TouchingLevelBounds = true;
      if (Behaviour == BoundsBehaviour.Kill)
      {
        _character.transform.position = constrainedPosition;
        _character.Kill();
      }
      else if (Behaviour == BoundsBehaviour.Constrain)
      {
        _character.transform.position = constrainedPosition;
      }
    }

#if UNITY_EDITOR
    [ButtonMethod]
    private void CopyBoundsFromScene()
    {
      Scene _scene = SceneGraphManager.Instance.CurrentScene;
      Bounds = _scene.bounds;
    }

    [ButtonMethod]
    private void GeneratePolygonCollider2DToSelection()
    {
      Scene _scene = SceneGraphManager.Instance.CurrentScene;
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
      Scene _scene = SceneGraphManager.Instance.CurrentScene;
      var b = Bounds;
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
