using System;
using UnityEngine;

namespace RFG
{
  [Serializable]
  public class StateMachine
  {
    public StatePack StatePack;
    public State CurrentState;
    public Type PreviousStateType { get; private set; }
    public Type CurrentStateType { get; private set; }
    public IStateContext Context { get { return _context; } set { _context = value; } }
    private IStateContext _context;
    private StatePack _defaultStatePack;

    public void Init()
    {
      if (StatePack == null || StatePack.States == null || StatePack.States.Count == 0)
      {
        Debug.LogWarning("Init: There are no states");
        return;
      }
      _defaultStatePack = StatePack;
    }

    public void SetStatePack(StatePack statePack)
    {
      StatePack = statePack;
    }

    public void RestoreDefaultStatePack()
    {
      StatePack = _defaultStatePack;
    }

    public void Update()
    {
      if (CurrentState == null)
        ResetToDefaultState();

      Type newStateType = CurrentState.Execute(_context);
      if (newStateType != null)
      {
        ChangeState(newStateType);
      }
    }

    public void ChangeState(Type newStateType)
    {
      // Dont change if current state
      if (CurrentStateType != null && CurrentStateType.Equals(newStateType))
      {
        return;
      }

      // Exit the previous state if there was one
      if (CurrentState != null)
      {
        PreviousStateType = CurrentState.GetType();
        CurrentState.Exit(_context);
      }

      // Enter the new state
      CurrentState = Find(newStateType);
      CurrentStateType = newStateType;
      CurrentState.Enter(_context);
    }

    public void ResetToDefaultState()
    {
      CurrentState = null;
      if (StatePack.DefaultState != null)
      {
        ChangeState(StatePack.DefaultState.GetType());
      }
      else
      {
        Debug.LogWarning("No default state defined");
      }
    }

    public void RestorePreviousState()
    {
      ChangeState(PreviousStateType);
    }

    public bool HasState(Type type)
    {
      return StatePack.HasState(type);
    }

    public void Bind(IStateContext context)
    {
      _context = context;
    }

    public void Add(State state)
    {
      StatePack.Add(state);
    }

    public void Remove(State state)
    {
      StatePack.Remove(state);
    }

    public State Find(Type type)
    {
      return StatePack.Find(type);
    }
  }
}