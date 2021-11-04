using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG.Actions
{
  [Serializable]
  [ActionMenu("Debug/Log")]
  public class DebugAction : Action
  {
    public string message;
    public override State Run()
    {
      Debug.Log(message);
      return State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");

      TextField text = new TextField()
      {
        label = "Message",
        value = message
      };
      text.multiline = true;
      text.RegisterValueChangedCallback(callback =>
      {
        TextField target = (TextField)callback.target;
        message = callback.newValue;
      });
      container.Add(text);
    }
#endif
  }
}