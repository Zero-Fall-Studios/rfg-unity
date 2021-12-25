namespace RFG
{
  using BehaviourTree;

  public class AttackActionNode : ActionNode
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
      brain.Context.Attack();
      return State.Running;
    }
  }
}