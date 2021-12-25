using UnityEngine;

namespace RFG
{
  using BehaviourTree;

  public class IdleActionNode : ActionNode
  {
    public float MinIdleTime = 1f;
    public float MaxIdleTime = 3f;
    [Range(0, 1)]
    public float IdleWeight = 0.5f;

    private float _timeElapsed = 0f;

    protected override void OnStart()
    {
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      brain.Context.controller.SetForce(Vector2.zero);
      brain.Context.characterContext.character.MovementState.ChangeState(typeof(IdleState));
      _timeElapsed = Time.time;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      var elapsed = Time.time - _timeElapsed;
      if (elapsed > MaxIdleTime)
      {
        return State.Success;
      }

      if (elapsed > MinIdleTime && UnityEngine.Random.value >= IdleWeight)
      {
        return State.Success;
      }

      return State.Running;
    }
  }
}
