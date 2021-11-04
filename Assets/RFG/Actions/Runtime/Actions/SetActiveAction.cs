using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace RFG.Actions
{
  [Serializable]
  [ActionMenu("Game Object/Set Active")]
  public class SetActiveAction : Action
  {
    public GameObject gameObject;
    public bool active = false;

    public override State Run()
    {
      gameObject.SetActive(active);
      return State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");

      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        gameObject = (GameObject)EditorGUILayout.ObjectField("Game Object:", gameObject, typeof(GameObject), true);
        active = EditorGUILayout.Toggle("Active:", active);
      });
      container.Add(guiContainer);
    }
#endif

  }
}