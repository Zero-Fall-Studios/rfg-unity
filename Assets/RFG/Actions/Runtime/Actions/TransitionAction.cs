using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace RFG.Actions
{
  using Transitions;

  public enum TransitionActionType
  {
    Play,
    TogglePause,
    Cancel
  }

  [Serializable]
  [ActionMenu("Transitions/Transition")]
  public class TransitionAction : Action
  {
    public Transition transition;
    public TransitionActionType TransitionActionType;
    public bool waitToComplete = true;

    private State _currentState = State.Init;

    public override void Init()
    {
      transition.onPlay.RemoveListener(onPlay);
      transition.onPlay.AddListener(onPlay);
      transition.onPause.RemoveListener(onPause);
      transition.onPause.AddListener(onPause);
      transition.onResume.RemoveListener(onResume);
      transition.onResume.AddListener(onResume);
      transition.onComplete.RemoveListener(onComplete);
      transition.onComplete.AddListener(onComplete);
    }

    public override State Run()
    {
      if (_currentState == State.Init)
      {
        switch (TransitionActionType)
        {
          case TransitionActionType.Play:
            transition.Play();
            if (!waitToComplete)
            {
              _currentState = State.Success;
            }
            else
            {
              _currentState = State.Running;
            }
            break;
          case TransitionActionType.TogglePause:
            transition.TogglePause();
            return State.Success;
          case TransitionActionType.Cancel:
            transition.Cancel();
            return State.Success;
        }
      }
      return _currentState;
    }

    private void onPlay()
    {
    }
    private void onPause()
    {
    }
    private void onResume()
    {
    }
    private void onComplete()
    {
      _currentState = State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");

      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        transition = (Transition)EditorGUILayout.ObjectField("Transition:", transition, typeof(Transition), true);
        TransitionActionType = (TransitionActionType)EditorGUILayout.EnumPopup("Transition Action:", TransitionActionType);
        waitToComplete = EditorGUILayout.Toggle("Wait To Complete:", waitToComplete);
      });
      container.Add(guiContainer);
    }
#endif

  }
}