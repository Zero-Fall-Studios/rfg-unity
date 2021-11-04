using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG
{
  using Actions;

  public enum AudioManagerActionType
  {
    StartAll,
    StopAll
  }

  [Serializable]
  [ActionMenu("Audio/Audio Manager")]
  public class AudioManagerAction : Action
  {
    public AudioManager AudioManager;
    public AudioManagerActionType AudioManagerActionType;

    public override RFG.Actions.State Run()
    {
      switch (AudioManagerActionType)
      {
        case AudioManagerActionType.StartAll:
          AudioManager.StartAll();
          break;
        case AudioManagerActionType.StopAll:
          AudioManager.StopAll();
          break;
      }
      return RFG.Actions.State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");
      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        AudioManager = (AudioManager)EditorGUILayout.ObjectField("Audio Manager:", AudioManager, typeof(AudioManager), true);
        AudioManagerActionType = (AudioManagerActionType)EditorGUILayout.EnumPopup("Action:", AudioManagerActionType);
      });
      container.Add(guiContainer);
    }
#endif

  }
}