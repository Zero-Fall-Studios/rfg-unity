
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New State Pack", menuName = "RFG/State/State Pack")]
  public class StatePack : ScriptableObject
  {
    public List<State> States;

    public State DefaultState;

    public void Add(State state)
    {
      if (States == null)
      {
        States = new List<State>();
      }
      States.Add(state);
    }

    public void Remove(State state)
    {
      States.Remove(state);
    }

    public State Find(Type type)
    {
      State state = States.Find(state => state.GetType().Equals(type));
      if (state == null)
      {
        LogExt.Warn<StatePack>($"State {type.ToString()} not found in state pack");
      }
      return state;
    }

    public bool HasState(Type type)
    {
      return Find(type) != null;
    }
  }
}