namespace RFG
{
  using BehaviourTree;
  public class ChangeDirectionActionNode : ActionNode
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
      brain.Context.controller.Flip();
      return State.Success;
    }
  }
}