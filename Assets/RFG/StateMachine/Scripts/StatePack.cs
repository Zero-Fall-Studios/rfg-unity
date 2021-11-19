
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

    public State Find(State state)
    {
      State foundState = States.Find(s => s == state);
      if (foundState == null)
      {
        LogExt.Warn<StatePack>($"State {foundState.ToString()} not found in state pack");
      }
      return state;
    }

    public bool HasState(State state)
    {
      return Find(state) != null;
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

#if UNITY_EDITOR
    public T AddToPack<T>(string enterClip = null, bool freezeState = false, float waitToUnfreezeTime = 0f, bool defaultState = false, params State[] statesCanUnFreeze) where T : State
    {
      T state = null;
      if (States != null && States.Count > 0)
      {
        state = States.Find(s => s.GetType().Equals(typeof(T))) as T;
      }

      if (state == null)
      {
        state = ScriptableObject.CreateInstance<T>();
        Add(state);
        AssetDatabase.AddObjectToAsset(state, this);
      }

      state.name = state.GetType().ToString().Last();

      if (!string.IsNullOrEmpty(enterClip))
      {
        state.EnterClip = enterClip;
      }

      if (freezeState)
      {
        state.FreezeState = freezeState;
      }

      if (waitToUnfreezeTime > 0)
      {
        state.WaitToUnfreezeTime = waitToUnfreezeTime;
      }

      if (statesCanUnFreeze != null && statesCanUnFreeze.Length > 0)
      {
        state.StatesCanUnfreeze = statesCanUnFreeze;
      }

      if (defaultState)
      {
        DefaultState = state;
      }
      AssetDatabase.SaveAssets();
      EditorUtility.SetDirty(this);
      return state;
    }

    public T AddToPack<T>(bool defaultState = false) where T : State
    {
      return AddToPack<T>(null, false, 0, defaultState, null);
    }
#endif
  }
}