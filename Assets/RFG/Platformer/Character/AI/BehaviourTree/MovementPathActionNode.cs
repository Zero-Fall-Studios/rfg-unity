namespace RFG
{
  using BehaviourTree;

  public class MovementPathActionNode : ActionNode
  {
    protected override void OnStart()
    {
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      brain.Context.characterContext.character.MovementState.ChangeState(typeof(WalkingState));
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      if (!brain.Context.movementPath.IsPlaying)
      {
        brain.Context.movementPath.Play();
        return State.Running;
      }
      else if (!brain.Context.movementPath.IsCompleted)
      {
        return State.Running;
      }
      else
      {
        brain.Context.characterContext.character.MovementState.ChangeState(typeof(IdleState));
        return State.Success;
      }
    }
  }
}