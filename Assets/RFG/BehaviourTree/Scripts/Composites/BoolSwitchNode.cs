namespace RFG.BehaviourTree
{
  public class BoolSwitchNode : CompositeNode
  {
    public string BoolProperty;

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

      System.Reflection.PropertyInfo info = context.GetType().GetProperty(BoolProperty);

      if (info == null)
        return children[0].Update();

      if (info.PropertyType == typeof(bool))
      {
        bool value = (bool)info.GetValue(context, null);

        if (value)
        {
          return children[1].Update();
        }
      }

      return children[0].Update();
    }
  }
}