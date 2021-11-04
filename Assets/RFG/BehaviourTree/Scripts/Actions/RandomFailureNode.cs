using UnityEngine;

namespace RFG.BehaviourTree
{
  public class RandomFailureNode : ActionNode
  {
    [Range(0, 1)]
    public float ChanceOfFailure = 0.5f;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      float value = Random.value;
      if (value < ChanceOfFailure)
      {
        return State.Failure;
      }
      return State.Success;
    }
  }
}