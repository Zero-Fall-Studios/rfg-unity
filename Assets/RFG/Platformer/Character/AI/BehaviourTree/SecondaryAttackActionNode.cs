namespace RFG
{
  using BehaviourTree;

  public class SecondaryAttackActionNode : ActionNode
  {
    protected override void OnStart()
    {
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      brain.Context.SecondaryAttack();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      if (brain.Context.character.MovementState.IsInState(typeof(PrimaryAttackStartedState)))
      {
        return State.Running;
      }
      return State.Success;
    }
  }
}