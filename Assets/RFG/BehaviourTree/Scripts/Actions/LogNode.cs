using UnityEngine;

namespace RFG.BehaviourTree
{
  public class LogNode : ActionNode
  {
    public string Message;
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      Debug.Log(Message);
      return State.Success;
    }
  }
}