using UnityEngine;

namespace RFG
{
  using BehaviourTree;

  public class MoveBackActionNode : ActionNode
  {
    public float MinMoveTime = 1f;
    public float MaxMoveTime = 3f;
    [Range(0, 1)]
    public float MoveWeight = 0.5f;

    private float _timeElapsed = 0f;

    protected override void OnStart()
    {
      _timeElapsed = Time.time;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      var elapsed = Time.time - _timeElapsed;
      if (elapsed > MaxMoveTime)
      {
        return State.Success;
      }

      if (elapsed > MinMoveTime && UnityEngine.Random.value >= MoveWeight)
      {
        return State.Success;
      }

      AIBrainBehaviour brain = context as AIBrainBehaviour;
      if (brain.Context.JustRotated())
      {
        Debug.Log("Got here 123");
        return State.Running;
      }
      if (brain.Context.RotateTowards())
      {
        Debug.Log("Got here 345");
        return State.Running;
      }
      if (brain.Context.PauseOnDangle())
      {
        Debug.Log("Got here 678");
        return State.Running;
      }
      float speed = brain.Context.characterContext.settingsPack.WalkingSpeed;
      brain.Context.MoveHorizontally(-speed);
      return State.Running;
    }
  }
}