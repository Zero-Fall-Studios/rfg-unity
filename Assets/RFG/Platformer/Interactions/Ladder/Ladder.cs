using UnityEngine;

namespace RFG
{
  public enum LadderType { Simple, BiDirectional }

  [AddComponentMenu("RFG/Interactions/Ladder")]
  public class Ladder : MonoBehaviour
  {
    [field: SerializeField] public LadderType LadderType { get; set; } = LadderType.Simple;
    [field: SerializeField] public GameObject LadderPlatform { get; set; }
    public bool CenterCharacterOnLadder = true;
    [field: SerializeField] private bool AutoPositionLadderPlatform { get; set; } = false;
    [field: SerializeField] public BoxCollider2D LadderPlatformBoxCollider2D { get; set; }
    [field: SerializeField] public EdgeCollider2D LadderPlatformEdgeCollider2D { get; set; }

    private Collider2D _collider2D;
    private Vector3 _newLadderPlatformPosition;

    private void Start()
    {
      _collider2D = GetComponent<Collider2D>();

      if (LadderPlatform == null)
      {
        return;
      }

      LadderPlatformBoxCollider2D = LadderPlatform.GetComponent<BoxCollider2D>();
      LadderPlatformEdgeCollider2D = LadderPlatform.GetComponent<EdgeCollider2D>();

      if (LadderPlatformBoxCollider2D == null && LadderPlatformEdgeCollider2D == null)
      {
        LogExt.Warn<Ladder>(this.name + " : this ladder's LadderPlatform is missing a BoxCollider2D or an EdgeCollider2D.");
      }

      if (AutoPositionLadderPlatform)
      {
        RepositionLadderPlatform();
      }
    }

    private void RepositionLadderPlatform()
    {
      if (_collider2D == null)
      {
        return;
      }
      if (LadderPlatformBoxCollider2D == null && LadderPlatformEdgeCollider2D == null)
      {
        return;
      }

      if (LadderPlatformBoxCollider2D != null)
      {
        _newLadderPlatformPosition = LadderPlatformBoxCollider2D.transform.localPosition;
        _newLadderPlatformPosition.x = 0;
        _newLadderPlatformPosition.y = _collider2D.bounds.size.y / 2 - LadderPlatformBoxCollider2D.bounds.size.y / 2;
        _newLadderPlatformPosition.z = this.transform.position.z;
        LadderPlatformBoxCollider2D.transform.localPosition = _newLadderPlatformPosition;
      }

      if (LadderPlatformEdgeCollider2D != null)
      {
        _newLadderPlatformPosition = LadderPlatformEdgeCollider2D.transform.localPosition;
        _newLadderPlatformPosition.x = 0;
        _newLadderPlatformPosition.y = _collider2D.bounds.size.y / 2 - LadderPlatformBoxCollider2D.bounds.size.y / 2;
        _newLadderPlatformPosition.z = this.transform.position.z;
        LadderPlatformEdgeCollider2D.transform.localPosition = _newLadderPlatformPosition;
      }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
      LadderClimbingAbility ladderAbility = collider.gameObject.GetComponent<LadderClimbingAbility>();
      if (ladderAbility == null)
      {
        return;
      }
      ladderAbility.AddCollidingLadder(_collider2D);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
      LadderClimbingAbility ladderAbility = collider.gameObject.GetComponent<LadderClimbingAbility>();
      if (ladderAbility == null)
      {
        return;
      }
      ladderAbility.RemoveCollidingLadder(_collider2D);
    }
  }
}