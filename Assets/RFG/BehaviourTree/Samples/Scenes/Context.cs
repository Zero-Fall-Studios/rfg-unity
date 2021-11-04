using System;
using UnityEngine;
using RFG.BehaviourTree;

namespace ContextTest
{
  [Serializable]
  public class ContextData
  {
    public Transform transform;
  }

  public class Context : MonoBehaviour, INodeContext
  {
    public ContextData contextData;
    [field: SerializeField] public bool IsPlaying { get; set; }

    private void Awake()
    {
      contextData = new ContextData();
      contextData.transform = transform;
    }
  }
}
