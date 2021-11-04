namespace RFG.BehaviourTree
{
  public class SequencerNode : CompositeNode
  {
    private int _currentIndex = 0;
    protected override void OnStart()
    {
      _currentIndex = 0;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      var child = children[_currentIndex];
      switch (child.Update())
      {
        case State.Running:
          return State.Running;
        case State.Failure:
          return State.Failure;
        case State.Success:
          _currentIndex++;
          break;
      }
      return _currentIndex == children.Count ? State.Success : State.Running;
    }
  }
}