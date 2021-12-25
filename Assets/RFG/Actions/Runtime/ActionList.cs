using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RFG.Actions
{
  [AddComponentMenu("RFG/Actions/Action List")]
  public class ActionList : MonoBehaviour
  {
    [field: SerializeReference] public List<Action> Actions { get; set; }
    [field: SerializeReference] private bool PlayOnAwake { get; set; } = false;

    private Action _currentAction;
    private IEnumerator _currentActionCoroutine;

    private void Awake()
    {
      if (PlayOnAwake)
      {
        Play();
      }
    }

    #region Play Methods
    public void Play()
    {
      ProcessNextAction();
    }

    private void ProcessNextAction()
    {
      if (_currentAction == null)
      {
        _currentAction = Actions[0];
      }
      _currentActionCoroutine = RunAction(_currentAction);
      StartCoroutine(_currentActionCoroutine);
    }

    private IEnumerator RunAction(Action action)
    {
      State currentState = State.Running;
      action.Init();
      while (currentState == State.Init || currentState == State.Running)
      {
        currentState = action.Run();
        yield return new WaitForSeconds(action.waitTimeToComplete);
      }
      if (currentState == State.Break)
      {
        StopCoroutine(_currentActionCoroutine);
        yield break;
      }
      Action nextAction = action.GetNextAction();
      if (nextAction != null)
      {
        _currentAction = nextAction;
        ProcessNextAction();
        yield return null;
      }
    }
    #endregion

    #region Editor Methods

#if UNITY_EDITOR
    public Action CreateAction(Type type, Vector2 position)
    {
      Action action = (Action)Activator.CreateInstance(type);
      action.title = action.GetType().ToString();
      action.position = position;
      action.guid = GUID.Generate().ToString();
      action.type = action.GetType().ToString();
      Actions.Add(action);
      return action;
    }

    public void DeleteAction(Action action)
    {
      Actions.Remove(action);
    }

    public void AddConnection(Action parent, Action child)
    {
      parent.children.Add(child);
    }

    public void RemoveConnection(Action parent, Action child)
    {
      parent.children.Remove(child);
    }
#endif

    #endregion
  }
}