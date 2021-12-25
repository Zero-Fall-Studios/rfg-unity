using System;
using UnityEngine;

namespace RFG
{
  [Serializable]
  public class StateMachine
  {
    public StatePack StatePack;
    public State PreviousState;
    public State CurrentState;
    public Type PreviousStateType { get; private set; }
    public Type CurrentStateType { get; private set; }
    public bool Enabled { get; set; } = true;
    public bool Frozen { get; set; } = false;
    public IStateContext Context { get { return _context; } set { _context = value; } }
    public Action<State, State> OnStateChange;
    public Action<Type, Type> OnStateTypeChange;
    private IStateContext _context;
    private StatePack _defaultStatePack;
    private float _frozenTimeElapsed = 0f;
    private float _nextStateWaitTimeElapsed = 0f;

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
      if (!Enabled)
        return;

      if (CurrentState == null)
        ResetToDefaultState();

      Type newStateType = CurrentState.Execute(_context);

      if (newStateType == null && CurrentState.NextState != null)
      {
        if (CurrentState.NextStateAfterTime > 0f)
        {
          _nextStateWaitTimeElapsed += Time.deltaTime;
          if (_nextStateWaitTimeElapsed > CurrentState.NextStateAfterTime)
          {
            newStateType = CurrentState.NextState.GetType();
          }
        }
        else if (CurrentState.GoToNextStateAfterCompletion)
        {
          newStateType = CurrentState.NextState.GetType();
        }
      }

      if (newStateType != null)
      {
        ChangeState(newStateType);
      }
      if (CurrentState.WaitToUnfreezeTime > 0f && Frozen)
      {
        _frozenTimeElapsed += Time.deltaTime;
        if (_frozenTimeElapsed > CurrentState.WaitToUnfreezeTime)
        {
          Frozen = false;
          _frozenTimeElapsed = 0;
        }
      }
    }

    public bool ChangeState(Type newStateType, bool force = false)
    {
      if (!force)
      {
        // Really frozen means that the new state type cant unfreeze and the state is currently frozen
        bool reallyFrozen = Frozen && !CanStateUnfreeze(newStateType);

        // Dont change if current state or if frozen
        if ((CurrentStateType != null && CurrentStateType.Equals(newStateType)) || reallyFrozen)
        {
          return false;
        }
      }

      // Exit the previous state if there was one
      if (CurrentState != null)
      {
        PreviousState = CurrentState;
        PreviousStateType = CurrentState.GetType();
        CurrentState.Exit(_context);
      }

      // Enter the new state
      CurrentState = Find(newStateType);

      // Debug.Log("Current State is null: " + CurrentState == null);

      // TODO - check if has sub states
      // Example - there might be many death states. Need to find a way to pull that substate

      Frozen = CurrentState.FreezeState;
      CurrentStateType = newStateType;
      CurrentState.Enter(_context);

      // Call the action for state change
      OnStateChange?.Invoke(PreviousState, CurrentState);
      OnStateTypeChange?.Invoke(PreviousStateType, newStateType);

      return true;
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

    public void RestorePreviousState(bool force = false)
    {
      ChangeState(PreviousStateType, force);
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

    private bool CanStateUnfreeze(Type stateType)
    {
      if (CurrentState != null && CurrentState.StatesCanUnfreeze != null)
      {
        foreach (State state in CurrentState.StatesCanUnfreeze)
        {
          if (stateType == state.GetType())
          {
            return true;
          }
        }
      }
      return false;
    }

    public bool IsInState(params Type[] stateTypes)
    {
      if (CurrentStateType == null)
        return false;

      foreach (Type stateType in stateTypes)
      {
        if (CurrentStateType == stateType)
        {
          return true;
        }
      }
      return false;
    }

    public bool IsntInState(params Type[] stateTypes)
    {
      return !IsInState(stateTypes);
    }

    public void GoToNextState()
    {
      if (CurrentState.NextState != null)
      {
        ChangeState(CurrentState.NextState.GetType());
      }
    }
  }
}