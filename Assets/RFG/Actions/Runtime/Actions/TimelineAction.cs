using System;
using UnityEngine.UIElements;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.Actions
{
  [Serializable]
  [ActionMenu("Timeline/Timeline")]
  public class TimelineAction : Action
  {
    public enum TimelineActionType { Play, Stop, Pause, Resume }
    public PlayableDirector director;
    public TimelineActionType actionType = TimelineActionType.Play;

    public override State Run()
    {
      switch (actionType)
      {
        case TimelineActionType.Play:
          director.Play();
          break;
        case TimelineActionType.Stop:
          director.Stop();
          break;
        case TimelineActionType.Pause:
          director.Pause();
          break;
        case TimelineActionType.Resume:
          director.Resume();
          break;
      }
      return State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");
      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        director = (PlayableDirector)EditorGUILayout.ObjectField("Director:", director, typeof(PlayableDirector), true);
        actionType = (TimelineActionType)EditorGUILayout.EnumPopup("Action: ", actionType);
      });
      container.Add(guiContainer);
    }
#endif

  }
}