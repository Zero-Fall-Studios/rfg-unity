using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;

namespace RFG.SceneGraph
{
  public class SceneGroup : Group
  {
    public Scene scene;
    public Action<SceneGroup> OnSceneSelected;
    public Action<SceneTransitionNode> OnSceneTransitionSelected;
    private SceneGraph graph;
    private SceneGraphView graphView;

    public SceneGroup(Scene scene, SceneGraph graph, SceneGraphView graphView)
    {
      this.scene = scene;
      this.title = System.IO.Path.GetFileNameWithoutExtension(scene.scenePath);
      SetPosition(new Rect(scene.position, Vector2.zero));

      this.graph = graph;
      this.graphView = graphView;

      this.AddManipulator(CreateSceneTransitionViewContextualMenu());
    }

    public override void SetPosition(Rect newPos)
    {
      base.SetPosition(newPos);
      Undo.RecordObject(scene, "Scene Graph (Set Position)");
      scene.position.x = newPos.xMin;
      scene.position.y = newPos.yMin;
      EditorUtility.SetDirty(scene);
    }

    public override void OnSelected()
    {
      base.OnSelected();
      OnSceneSelected?.Invoke(this);
    }

    #region Scene Transition

    private IManipulator CreateSceneTransitionViewContextualMenu()
    {

      ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
          menuEvent => menuEvent.menu.AppendAction("Add Scene Transition", actionEvent => CreateSceneTransition("Scene Transition", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
      );
      return contextualMenuManipulator;
    }

    public SceneTransition CreateSceneTransition(string title, Vector2 position)
    {
      if (graph == null)
      {
        LogExt.Warn<SceneGraphView>("Graph is null");
        return null;
      }
      SceneTransition sceneTransition = graph.CreateSceneTransition(scene, title, position);
      CreateSceneTransitionNode(sceneTransition);
      return sceneTransition;
    }

    private GraphElement CreateSceneTransitionNode(SceneTransition sceneTransition)
    {
      SceneTransitionNode sceneTransitionNode = new SceneTransitionNode(sceneTransition);
      sceneTransitionNode.Draw();
      sceneTransitionNode.OnSceneTransitionSelected = OnSceneTransitionSelected;
      graphView.AddElement(sceneTransitionNode);
      AddElement(sceneTransitionNode);
      return sceneTransitionNode;
    }

    public void CreateSceneTransitions()
    {
      scene.sceneTransitions.ForEach(sceneTransition => CreateSceneTransitionNode(sceneTransition));
    }
    #endregion

    #region Utilities
    public Vector2 GetLocalMousePosition(Vector2 position)
    {
      return contentContainer.WorldToLocal(position);
    }
    #endregion
  }
}