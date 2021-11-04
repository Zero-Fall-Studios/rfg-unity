using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace RFG.Actions
{
  [AttributeUsage(AttributeTargets.Class)]
  public class ActionMenu : Attribute
  {
    private string path;
    public ActionMenu(string path)
    {
      this.path = path;
    }
    public string Path
    {
      get
      {
        return path;
      }
    }
  }

  public enum State
  {
    Init,
    Running,
    Break,
    Success
  }
  [Serializable]
  public class Action
  {
    public string guid;
    public string title;
    public Vector2 position;
    public State state = State.Success;
    public string type;
    public float waitTimeToComplete = 0f;
    [field: SerializeReference] public List<Action> children = new List<Action>();

    public virtual void Init()
    {
    }

    public virtual State Run()
    {
      return State.Success;
    }

    public virtual Action GetNextAction()
    {
      if (children.Count > 0)
      {
        return children[0];
      }
      return null;
    }

#if UNITY_EDITOR
    public virtual void Draw(ActionNode node)
    {
    }

    public virtual void DrawBase(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("base-options");
      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        waitTimeToComplete = EditorGUILayout.FloatField("Wait Time To Complete:", waitTimeToComplete);
      });
      container.Add(guiContainer);

      node.input = new NodePort(Direction.Input, Port.Capacity.Single);
      node.input.portName = "";
      node.input.style.flexDirection = FlexDirection.Column;
      node.inputContainer.Add(node.input);

      node.output = new NodePort(Direction.Output, Port.Capacity.Single);
      node.output.portName = "";
      node.output.style.flexDirection = FlexDirection.ColumnReverse;
      node.outputContainer.Add(node.output);
    }
#endif
  }
}