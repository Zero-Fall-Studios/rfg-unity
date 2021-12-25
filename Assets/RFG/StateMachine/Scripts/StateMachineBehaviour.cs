using System;
using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/State Machine/State Machine Behaviour")]
  public class StateMachineBehaviour : MonoBehaviour
  {
    public StateMachine State;

    protected virtual void Awake()
    {
      Bind(null);
    }

    private void Update()
    {
      State.Update();
    }

    public void Bind(IStateContext context)
    {
      if (context == null)
      {
        StateAnimatorContext animatorContext = new StateAnimatorContext();
        animatorContext.transform = transform;
        animatorContext.animator = GetComponent<Animator>();
        context = animatorContext;
      }
      State.Bind(context);
    }

    public void ChangeState(Type newStateType)
    {
      State.ChangeState(newStateType);
    }

    public void ResetToDefaultState()
    {
      State.ResetToDefaultState();
    }

    public void RestorePreviousState()
    {
      State.RestorePreviousState();
    }

    public bool HasState(Type type)
    {
      return State.HasState(type);
    }

  }
}