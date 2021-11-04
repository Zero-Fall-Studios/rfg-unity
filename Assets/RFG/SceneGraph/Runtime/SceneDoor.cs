using UnityEngine;
using UnityEngine.Events;

namespace RFG.SceneGraph
{
  [AddComponentMenu("RFG/Scene Graph/Scene Door")]
  public class SceneDoor : MonoBehaviour
  {
    [field: SerializeField] private string[] tags;
    [field: SerializeField] private UnityEvent onTriggerEnter;
    public Vector3 spawnOffset = Vector3.zero;

    [HideInInspector] public SceneTransition sceneTransition;
    [HideInInspector] public int fromTo;

    private bool _triggered = false;

    public void LoadScene()
    {
      if (fromTo == 0)
      {
        SceneGraphManager.Instance.LoadSceneFrom(sceneTransition);
      }
      else
      {
        SceneGraphManager.Instance.LoadSceneTo(sceneTransition);
      }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
      if (col.gameObject.CompareTags(tags))
      {
        if (!_triggered)
        {
          _triggered = true;
          onTriggerEnter?.Invoke();
        }
      }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
      UnityEditor.Handles.color = Color.yellow;
      UnityEditor.Handles.Label(transform.position + spawnOffset, "Spawn At");
    }
#endif

  }
}