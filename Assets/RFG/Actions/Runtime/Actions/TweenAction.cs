using System;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.Actions
{
  public enum TweenActionType
  {
    Play,
    TogglePause,
    Cancel,
    PlayReverse
  }

  [Serializable]
  [ActionMenu("Tween/Tween")]
  public class TweenAction : Action
  {
    public Tween tween;
    public TweenActionType TweenActionType;
    public bool waitToComplete = true;

    private State _currentState = State.Init;

    public override State Run()
    {
      if (_currentState == State.Init)
      {
        switch (TweenActionType)
        {
          case TweenActionType.Play:
            RemoveListeners();
            AddListeners();
            tween.Play();
            if (!waitToComplete)
            {
              _currentState = State.Success;
            }
            else
            {
              _currentState = State.Running;
            }
            break;
          case TweenActionType.TogglePause:
            tween.TogglePause();
            return State.Success;
          case TweenActionType.Cancel:
            tween.Cancel();
            return State.Success;
          case TweenActionType.PlayReverse:
            RemoveListeners();
            AddListeners();
            tween.Play(true);
            if (!waitToComplete)
            {
              _currentState = State.Success;
            }
            else
            {
              _currentState = State.Running;
            }
            break;
        }
      }
      return _currentState;
    }

    private void AddListeners()
    {
      tween.onPlay.AddListener(onPlay);
      tween.onPause.AddListener(onPause);
      tween.onResume.AddListener(onResume);
      tween.onComplete.AddListener(onComplete);
    }

    private void RemoveListeners()
    {
      tween.onPlay.RemoveListener(onPlay);
      tween.onPause.RemoveListener(onPause);
      tween.onResume.RemoveListener(onResume);
      tween.onComplete.RemoveListener(onComplete);
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
      tween.onPlay.RemoveListener(onPlay);
      tween.onPause.RemoveListener(onPause);
      tween.onResume.RemoveListener(onResume);
      tween.onComplete.RemoveListener(onComplete);
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");
      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        tween = (Tween)EditorGUILayout.ObjectField("Tween:", tween, typeof(Tween), true);
        TweenActionType = (TweenActionType)EditorGUILayout.EnumPopup("Tween Action:", TweenActionType);
        waitToComplete = EditorGUILayout.Toggle("Wait To Complete:", waitToComplete);
      });
      container.Add(guiContainer);
    }
#endif

  }
}