using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Interactions/Checkpoint")]
  public class Checkpoint : MonoBehaviour
  {
    public static Checkpoint currentCheckpoint;

    [field: SerializeField] private string[] tags;
    public bool spawnOnAwake;

    [Header("Events")]
    [field: SerializeField] private GameEvent OnCheckpointEnter;

    private void Awake()
    {
      currentCheckpoint = null;
    }

    private void Start()
    {
      if (spawnOnAwake)
      {
        currentCheckpoint = this;
      }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
      if (col.gameObject.CompareTags(tags))
      {
        currentCheckpoint = this;
        OnCheckpointEnter?.Raise();
      }
    }
  }
}