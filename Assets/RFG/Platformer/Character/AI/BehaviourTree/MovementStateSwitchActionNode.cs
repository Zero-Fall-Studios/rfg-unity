namespace RFG
{
  using BehaviourTree;

  public class MovementStateSwitchActionNode : CompositeNode
  {
    public string typeSwitch;
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

      if (brain.Context.character.MovementState.CurrentStateType == null)
        return State.Running;

      bool isCurrentState = brain.Context.character.MovementState.CurrentStateType.ToString().Last().Equals(typeSwitch);
      current = isCurrentState ? 1 : 0;
      if (current != previous)
      {
        children[previous].Abort();
        previous = current;
      }
      return isCurrentState ? children[1].Update() : children[0].Update();
    }
  }
}
