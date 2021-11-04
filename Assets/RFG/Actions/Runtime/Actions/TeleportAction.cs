using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace RFG.Actions
{
  [Serializable]
  [ActionMenu("Transform/Teleport")]
  public class TeleportAction : Action
  {
    public Transform teleport;
    public Transform marker;

    public override State Run()
    {
      teleport.position = marker.position;
      return State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");

      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        teleport = (Transform)EditorGUILayout.ObjectField("Transform:", teleport, typeof(Transform), true);
        marker = (Transform)EditorGUILayout.ObjectField("Marker:", marker, typeof(Transform), true);
      });
      container.Add(guiContainer);
    }
#endif

  }
}