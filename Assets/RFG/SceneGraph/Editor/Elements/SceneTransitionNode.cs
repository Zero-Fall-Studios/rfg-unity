using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace RFG.SceneGraph
{
  public class SceneTransitionNode : Node
  {
    public Action<SceneTransitionNode> OnSceneTransitionSelected;
    public SceneTransition sceneTransition;

    public Port input;
    public Port output;

    public SceneTransitionNode(SceneTransition sceneTransition)
    {
      this.sceneTransition = sceneTransition;
      this.sceneTransition.name = sceneTransition.GetType().Name;
      this.title = "Scene Transition";
      this.viewDataKey = sceneTransition.guid;

      SetPosition(new Rect(sceneTransition.position, Vector2.zero));
    }

    public override void SetPosition(Rect newPos)
    {
      base.SetPosition(newPos);
      sceneTransition.position.x = newPos.xMin;
      sceneTransition.position.y = newPos.yMin;
      EditorUtility.SetDirty(sceneTransition);
    }

    public override void OnSelected()
    {
      base.OnSelected();
      OnSceneTransitionSelected?.Invoke(this);
    }

    private void CreateConnections()
    {
      // Port p = new Port();
      //   if (port != null)
      //   {
      //     port.portName = "";
      //     inputContainer.Add(port);
      //   }
    }

    public void UpdateState()
    {
      //   Label sceneName = this.Q<Label>("scene-name-label");
      //   sceneName.text = sceneTransition.sceneName;
    }

    public virtual void Draw()
    {
      input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
      input.portName = "From Scene";
      inputContainer.Add(input);

      output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
      output.portName = "To Scene";
      outputContainer.Add(output);
    }

  }
}