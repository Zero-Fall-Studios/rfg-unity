using UnityEngine;
using RFG.BehaviourTree;

namespace ContextTest
{
  public class ContextActionNode : ActionNode
  {
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      Context context = this.context as Context;

      if (context != null)
      {
        Debug.Log(context.contextData.transform.position.x);
      }

      return State.Success;
    }
  }
}