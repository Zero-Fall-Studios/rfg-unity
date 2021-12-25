using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;

namespace RFG.Actions
{
  public class ActionListView : GraphView
  {
    public new class UxmlFactory : UxmlFactory<ActionListView, GraphView.UxmlTraits> { }

    private ActionList actionList;

    public ActionListView()
    {
      Insert(0, new GridBackground());

      this.AddManipulator(new ContentZoomer());
      this.AddManipulator(new ContentDragger());
      this.AddManipulator(new SelectionDragger());
      this.AddManipulator(new RectangleSelector());

      this.AddStyleSheets("ActionListEditor.uss");
    }

    internal void PopulateView(ActionList actionList)
    {

      this.actionList = actionList;

      graphViewChanged -= OnGraphViewChanged;
      DeleteElements(graphElements.ToList());
      graphViewChanged += OnGraphViewChanged;

      if (actionList == null)
      {
        return;
      }

      if (actionList.Actions == null)
      {
        actionList.Actions = new List<Action>();
      }

      // Create Scene Groups
      actionList.Actions.ForEach(n => CreateActionNode(n));

      // Create Edges
      actionList.Actions.ForEach(action =>
      {
        var children = action.children;
        children.ForEach(c =>
        {
          ActionNode parentView = FindActionNode(action);
          ActionNode childView = FindActionNode(c);

          Edge edge = parentView.output.ConnectTo(childView.input);
          AddElement(edge);
        });
      });
    }

    public ActionNode FindActionNode(Action action)
    {
      return GetNodeByGuid(action.guid) as ActionNode;
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
          ActionNode actionNode = elem as ActionNode;
          if (actionNode != null)
          {
            actionList.DeleteAction(actionNode.action);
          }

          Edge edge = elem as Edge;
          if (edge != null)
          {
            ActionNode parent = edge.output.node as ActionNode;
            ActionNode child = edge.input.node as ActionNode;
            actionList.RemoveConnection(parent.action, child.action);
          }
        });
      }

      if (graphViewChange.edgesToCreate != null)
      {
        graphViewChange.edgesToCreate.ForEach(edge =>
        {
          ActionNode parent = edge.output.node as ActionNode;
          ActionNode child = edge.input.node as ActionNode;
          actionList.AddConnection(parent.action, child.action);
        });
      }

      actionList.Actions.Sort(SortByVerticalPosition);

      return graphViewChange;
    }

    private int SortByVerticalPosition(Action left, Action right)
    {
      return left.position.y < right.position.y ? -1 : 1;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
      Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
      {
        var types = TypeCache.GetTypesDerivedFrom<Action>();
        foreach (var type in types)
        {
          ActionMenu menu = Attribute.GetCustomAttribute(type, typeof(ActionMenu)) as ActionMenu;
          if (menu != null)
          {
            evt.menu.AppendAction(menu.Path, (a) => CreateAction(type, nodePosition));
          }
          else
          {
            evt.menu.AppendAction($"Custom/{type.Name}", (a) => CreateAction(type, nodePosition));
          }
        }
      }
    }

    public Action CreateAction(System.Type type, Vector2 position)
    {
      if (actionList == null)
      {
        LogExt.Warn<ActionListView>("ActionList is null");
        return null;
      }
      Action action = actionList.CreateAction(type, position);
      CreateActionNode(action);
      return action;
    }

    public ActionNode CreateActionNode(Action action)
    {
      if (actionList == null)
      {
        LogExt.Warn<ActionListView>("ActionList is null");
        return null;
      }
      ActionNode actionNode = new ActionNode(actionList, action);
      actionNode.Draw();
      AddElement(actionNode);
      return actionNode;
    }

  }
}