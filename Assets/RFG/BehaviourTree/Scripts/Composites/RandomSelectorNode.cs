using UnityEngine;

namespace RFG.BehaviourTree
{
  public class RandomSelectorNode : CompositeNode
  {
    protected int current;

    protected override void OnStart()
    {
      current = Random.Range(0, children.Count);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      var child = children[current];
      return child.Update();
    }
  }
}