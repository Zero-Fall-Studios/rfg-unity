using UnityEngine;

namespace RFG
{
  using BehaviourTree;

  public class IdleActionNode : ActionNode
  {
    protected override void OnStart()
    {
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      brain.Context.controller.SetForce(Vector2.zero);
      brain.Context.characterContext.character.MovementState.ChangeState(typeof(IdleState));
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      return State.Running;
    }
  }
}
