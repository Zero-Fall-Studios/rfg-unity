namespace RFG.BehaviourTree
{
  public class InterruptSelectorNode : SelectorNode
  {
    protected override State OnUpdate()
    {
      int previous = current;
      base.OnStart();
      var status = base.OnUpdate();
      if (previous != current)
      {
        if (children[previous].state == State.Running)
        {
          children[previous].Abort();
        }
      }

      return status;
    }
  }
}