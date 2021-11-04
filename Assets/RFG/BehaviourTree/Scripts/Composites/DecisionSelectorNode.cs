using UnityEngine;

namespace RFG.BehaviourTree
{
  public class DecisionSelectorNode : CompositeNode
  {
    public float DecisionTime = 3f;

    [Range(0, 1)]
    public float DecisionWeight = 0.5f;
    protected int current = 0;
    protected int previous = 0;
    private float _decisionTimeElapsed = 0f;

    protected override void OnStart()
    {
      _decisionTimeElapsed = Time.time;
      current = 0;
      previous = current;
    }

    protected override void OnStop()
    {
      children.ForEach(node => node.Abort());
    }

    protected override State OnUpdate()
    {
      if (Time.time - _decisionTimeElapsed > DecisionTime)
      {
        MakeDecision();
      }

      var child = children[current];
      switch (child.Update())
      {
        case State.Success:
          DefaultDecision();
          break;
        case State.Failure:
          DefaultDecision();
          break;
      }

      return State.Running;
    }

    private void MakeDecision()
    {
      _decisionTimeElapsed = Time.time;
      if (Random.value > DecisionWeight)
      {
        children[current].Abort();
        previous = current;
        int newCurrent = Random.Range(0, children.Count);
        if (previous != newCurrent)
        {
          current = newCurrent;
        }
      }
    }

    private void DefaultDecision()
    {
      _decisionTimeElapsed = Time.time;
      children[current].Abort();
      previous = current;
      current = 0;
    }
  }
}