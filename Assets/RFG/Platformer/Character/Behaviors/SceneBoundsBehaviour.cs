using UnityEngine;

namespace RFG
{
  using SceneGraph;

  public enum CharacterBoundsBehaviour { Nothing, Constrain, Kill }
  [AddComponentMenu("RFG/Platformer/Character/Behaviours/Scene Bounds")]
  public class SceneBoundsBehaviour : MonoBehaviour
  {
    [Header("Bounds Behaviour")]
    public CharacterBoundsBehaviour Top = CharacterBoundsBehaviour.Constrain;
    public CharacterBoundsBehaviour Bottom = CharacterBoundsBehaviour.Kill;
    public CharacterBoundsBehaviour Left = CharacterBoundsBehaviour.Constrain;
    public CharacterBoundsBehaviour Right = CharacterBoundsBehaviour.Constrain;

    private Character _character;
    private SceneBounds _sceneBounds;

    private void Awake()
    {
      _sceneBounds = FindObjectOfType<SceneBounds>();
      _character = GetComponent<Character>();
    }

    private void LateUpdate()
    {
      if (_character.CharacterState.CurrentStateType != typeof(AliveState))
        return;

      if (_sceneBounds != null)
      {
        _sceneBounds.HandleSceneBounds(
          transform,
          _character.Controller.ColliderSize,
          _character.Controller.ColliderLeftPosition.x,
          _character.Controller.ColliderRightPosition.x,
          _character.Controller.ColliderBottomPosition.y,
          _character.Controller.ColliderTopPosition.y
        );
      }
    }

    private void OnBoundsLeft(Vector2 constrainedPosition, Transform constrainedTransform)
    {
      if (Left == CharacterBoundsBehaviour.Kill)
      {
        _character.transform.position = constrainedPosition;
        _character.Kill();
      }
      else if (Left == CharacterBoundsBehaviour.Constrain)
      {
        _character.transform.position = constrainedPosition;
      }
    }

    private void OnBoundsBottom(Vector2 constrainedPosition, Transform constrainedTransform)
    {
      if (Bottom == CharacterBoundsBehaviour.Kill)
      {
        _character.transform.position = constrainedPosition;
        _character.Kill();
      }
      else if (Bottom == CharacterBoundsBehaviour.Constrain)
      {
        _character.transform.position = constrainedPosition;
      }
    }

    private void OnBoundsRight(Vector2 constrainedPosition, Transform constrainedTransform)
    {
      if (Right == CharacterBoundsBehaviour.Kill)
      {
        _character.transform.position = constrainedPosition;
        _character.Kill();
      }
      else if (Right == CharacterBoundsBehaviour.Constrain)
      {
        _character.transform.position = constrainedPosition;
      }
    }

    private void OnBoundsTop(Vector2 constrainedPosition, Transform constrainedTransform)
    {
      if (Top == CharacterBoundsBehaviour.Kill)
      {
        _character.transform.position = constrainedPosition;
        _character.Kill();
      }
      else if (Top == CharacterBoundsBehaviour.Constrain)
      {
        _character.transform.position = constrainedPosition;
      }
    }

    private void OnEnable()
    {
      if (_sceneBounds != null)
      {
        _sceneBounds.OnBoundsTop += OnBoundsTop;
        _sceneBounds.OnBoundsRight += OnBoundsRight;
        _sceneBounds.OnBoundsBottom += OnBoundsBottom;
        _sceneBounds.OnBoundsLeft += OnBoundsLeft;
      }
    }

    private void OnDisable()
    {
      if (_sceneBounds != null)
      {
        _sceneBounds.OnBoundsTop -= OnBoundsTop;
        _sceneBounds.OnBoundsRight -= OnBoundsRight;
        _sceneBounds.OnBoundsBottom -= OnBoundsBottom;
        _sceneBounds.OnBoundsLeft -= OnBoundsLeft;
      }
    }

  }
}
