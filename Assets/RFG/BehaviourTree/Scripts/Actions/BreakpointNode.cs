using UnityEngine;

namespace RFG.BehaviourTree
{
  public class Breakpoint : ActionNode
  {
    protected override void OnStart()
    {
      Debug.Log("Triggering Breakpoint");
      Debug.Break();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      return State.Success;
    }
  }
}