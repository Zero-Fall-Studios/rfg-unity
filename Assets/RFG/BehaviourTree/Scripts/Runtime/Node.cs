using UnityEngine;

namespace RFG.BehaviourTree
{
  public abstract class Node : ScriptableObject
  {
    public enum State
    {
      Running,
      Failure,
      Success
    }

    [HideInInspector] public State state = State.Running;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public INodeContext context;
    [TextArea] public string description;
    public bool drawGizmos = false;

    public State Update()
    {
      if (!started)
      {
        OnStart();
        started = true;
      }
      state = OnUpdate();
      if (state == State.Failure || state == State.Success)
      {
        OnStop();
        started = false;
      }
      return state;
    }

    public virtual Node Clone()
    {
      return Instantiate(this);
    }

    public void Abort()
    {
      started = false;
      state = State.Running;
      OnStop();
      BehaviourTree.Traverse(this, (node) =>
      {
        node.started = false;
        node.state = State.Running;
        node.OnStop();
      });
    }

    public virtual void OnDrawGizmos() { }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();
  }
}