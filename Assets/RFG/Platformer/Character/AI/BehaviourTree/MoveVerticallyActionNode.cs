namespace RFG
{
  using BehaviourTree;

  public class MoveVerticallyActionNode : ActionNode
  {
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      if (brain.Context.JustRotated())
      {
        return State.Running;
      }
      if (brain.Context.RotateTowards())
      {
        return State.Running;
      }
      if (brain.Context.PauseOnDangle())
      {
        return State.Running;
      }
      float speed = brain.Context.WalkOrRun();
      brain.Context.MoveVertically(speed);
      return State.Running;
    }
  }
}