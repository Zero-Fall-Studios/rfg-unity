namespace RFG
{
  using BehaviourTree;

  public class HasAggroNode : CompositeNode
  {
    protected int current = 0;
    protected int previous = 0;
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      if (children.Count < 2)
        return State.Failure;

      AIBrainBehaviour brain = context as AIBrainBehaviour;
      current = brain.HasAggro ? 1 : 0;
      if (current != previous)
      {
        children[previous].Abort();
        previous = current;
      }
      return brain.HasAggro ? children[1].Update() : children[0].Update();
    }
  }
}