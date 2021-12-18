using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;

namespace RFG.SceneGraph
{
  public class SceneGraphView : GraphView
  {
    public new class UxmlFactory : UxmlFactory<SceneGraphView, GraphView.UxmlTraits> { }

    public Action<SceneGroup> OnSceneSelected;
    public Action<SceneTransitionNode> OnSceneTransitionSelected;
    private SceneGraph graph;

    public SceneGraphView()
    {
      Insert(0, new GridBackground());

      this.AddManipulator(new ContentZoomer());
      this.AddManipulator(new ContentDragger());
      this.AddManipulator(new SelectionDragger());
      this.AddManipulator(new RectangleSelector());
      this.AddManipulator(CreateSceneContextualMenu());

      this.AddStyleSheets("SceneGraphEditor.uss");
    }

    internal void PopulateView(SceneGraph graph)
    {
      this.graph = graph;

      graphViewChanged -= OnGraphViewChanged;
      DeleteElements(graphElements.ToList());
      graphViewChanged += OnGraphViewChanged;

      // Create Scene Groups
      graph.scenes.ForEach(n => CreateSceneGroup(n));

      // Create Edges
      graph.scenes.ForEach(scene =>
      {
        scene.sceneTransitions.ForEach(sceneTransition =>
        {
          SceneTransitionNode fromView = FindSceneTransitionNode(sceneTransition);
          if (sceneTransition.to != null)
          {
            SceneTransitionNode toView = FindSceneTransitionNode(sceneTransition.to);
            Edge edge = fromView.output.ConnectTo(toView.input);
            AddElement(edge);
          }
        });
      });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
      return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
      if (graphViewChange.elementsToRemove != null)
      {
        graphViewChange.elementsToRemove.ForEach(elem =>
        {
          SceneGroup SceneGroup = elem as SceneGroup;
          if (SceneGroup != null)
          {
            graph.DeleteScene(SceneGroup.scene);
          }

          SceneTransitionNode SceneTransitionNode = elem as SceneTransitionNode;
          if (SceneTransitionNode != null)
          {
            graph.DeleteSceneTransition(SceneTransitionNode.sceneTransition.parent, SceneTransitionNode.sceneTransition);
          }

          Edge edge = elem as Edge;
          if (edge != null)
          {
            SceneTransitionNode fromView = edge.output.node as SceneTransitionNode;
            SceneTransitionNode toView = edge.input.node as SceneTransitionNode;
            graph.RemoveConnection(fromView.sceneTransition, toView.sceneTransition);
          }
        });
      }
      if (graphViewChange.edgesToCreate != null)
      {
        graphViewChange.edgesToCreate.ForEach(edge =>
        {
          SceneTransitionNode from = edge.output.node as SceneTransitionNode;
          SceneTransitionNode to = edge.input.node as SceneTransitionNode;
          from.sceneTransition.to = to.sceneTransition;
          to.sceneTransition.from = from.sceneTransition;
        });
      }

      return graphViewChange;
    }

    #region Scene

    private IManipulator CreateSceneContextualMenu()
    {

      ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
          menuEvent => menuEvent.menu.AppendAction("Add Scene", actionEvent => CreateScene("Scene", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
      );
      return contextualMenuManipulator;
    }

    public Scene CreateScene(string title, Vector2 position)
    {
      if (graph == null)
      {
        LogExt.Warn<SceneGraphView>("Graph is null");
        return null;
      }
      Scene scene = graph.CreateScene(title, position);
      CreateSceneGroup(scene);
      return scene;
    }

    private GraphElement CreateSceneGroup(Scene scene)
    {
      SceneGroup sceneGroup = new SceneGroup(scene, graph, this);

      sceneGroup.OnSceneSelected = OnSceneSelected;
      sceneGroup.OnSceneTransitionSelected = OnSceneTransitionSelected;
      AddElement(sceneGroup);

      sceneGroup.CreateSceneTransitions();

      return sceneGroup;
    }
    #endregion

    #region Utilities
    public Vector2 GetLocalMousePosition(Vector2 position)
    {
      return contentViewContainer.WorldToLocal(position);
    }

    public SceneTransitionNode FindSceneTransitionNode(SceneTransition sceneTransition)
    {
      return GetNodeByGuid(sceneTransition.guid) as SceneTransitionNode;
    }
    #endregion
  }
}