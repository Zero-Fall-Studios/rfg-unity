namespace RFG
{
  using BehaviourTree;

  public class WanderActionNode : ActionNode
  {
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      brain.Context.MoveHorizontally(0);
      brain.Context.characterContext.character.MovementState.ChangeState(typeof(IdleState));
    }

    protected override State OnUpdate()
    {
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      if (brain.Context.characterContext.settingsPack == null)
        return State.Failure;

      brain.Context.FlipOnCollision();
      brain.Context.FlipOnDangle();
      if (!brain.Context.JustRotated())
      {
        brain.Context.FlipOnLevelBoundsCollision();
      }
      brain.Context.controller.State.IsWalking = true;
      brain.Context.characterContext.character.MovementState.ChangeState(typeof(WalkingState));
      brain.Context.MoveHorizontally(brain.Context.characterContext.settingsPack.WalkingSpeed);
      brain.Context.TouchingWalls();
      return State.Running;
    }
  }
}
