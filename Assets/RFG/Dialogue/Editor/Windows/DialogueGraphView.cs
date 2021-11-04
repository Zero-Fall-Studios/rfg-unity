using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace RFG.Dialogue
{
  public class DialogueGraphView : GraphView
  {
    private DialogueEditorWindow editorWindow;
    private DialogueSearchWindow searchWindow;
    private MiniMap miniMap;

    private SerializableDictionary<string, NodeErrorData> ungroupedNodes;
    private SerializableDictionary<Group, SerializableDictionary<string, NodeErrorData>> groupedNodes;
    private SerializableDictionary<string, GroupErrorData> groups;

    private int repeatedNamesAmount;
    public int RepeatedNamesAmount
    {
      get
      {
        return repeatedNamesAmount;
      }
      set
      {
        repeatedNamesAmount = value;
        if (repeatedNamesAmount == 0)
        {
          editorWindow.EnableSaving(true);
        }
        else
        {
          editorWindow.EnableSaving(false);
        }
      }
    }

    public DialogueGraphView(DialogueEditorWindow dsEditorWindow)
    {
      editorWindow = dsEditorWindow;
      ungroupedNodes = new SerializableDictionary<string, NodeErrorData>();
      groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, NodeErrorData>>();
      groups = new SerializableDictionary<string, GroupErrorData>();

      AddManipulators();
      AddGridBackground();
      AddSearchWindow();
      AddMiniMap();
      AddStyles();
      AddMiniMapStyles();

      OnElementsDeleted();
      OnGroupElementsAdded();
      OnGroupElementsRemoved();
      OnGroupRenamed();
      OnGraphViewChanged();
    }

    #region Elements

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
      List<Port> compatiblePorts = new List<Port>();

      ports.ForEach(port =>
      {
        if (startPort == port)
        {
          return;
        }
        if (startPort.node == port.node)
        {
          return;
        }
        if (startPort.direction == port.direction)
        {
          return;
        }
        compatiblePorts.Add(port);
      });

      return compatiblePorts;
    }

    private void AddManipulators()
    {
      SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

      this.AddManipulator(new ContentDragger());
      this.AddManipulator(new SelectionDragger());
      this.AddManipulator(new RectangleSelector());

      this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DialogueType.SingleChoice));
      this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DialogueType.MultipleChoice));

      this.AddManipulator(CreateGroupContextualMenu());
    }

    private void AddGridBackground()
    {
      GridBackground gridBackground = new GridBackground();
      gridBackground.StretchToParentSize();
      Insert(0, gridBackground);
    }

    private void AddStyles()
    {
      this.AddStyleSheets("GraphViewStyles.uss", "NodeStyles.uss");
    }

    private void AddMiniMapStyles()
    {
      StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
      StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

      miniMap.style.backgroundColor = backgroundColor;
      miniMap.style.borderTopColor = borderColor;
      miniMap.style.borderRightColor = borderColor;
      miniMap.style.borderBottomColor = borderColor;
      miniMap.style.borderLeftColor = borderColor;
    }

    private void AddSearchWindow()
    {
      if (searchWindow == null)
      {
        searchWindow = ScriptableObject.CreateInstance<DialogueSearchWindow>();
        searchWindow.Initialize(this);
      }
      nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }

    private void AddMiniMap()
    {
      miniMap = new MiniMap()
      {
        anchored = true
      };
      miniMap.SetPosition(new Rect(15, 50, 200, 180));
      Add(miniMap);
      miniMap.visible = false;
    }

    #endregion

    #region Groups

    private IManipulator CreateGroupContextualMenu()
    {
      ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
          menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => CreateGroup("DialogueGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
      );
      return contextualMenuManipulator;
    }

    public GraphElement CreateGroup(string title, Vector2 position)
    {
      DialogueGroup group = new DialogueGroup(title, position);
      AddGroup(group);

      AddElement(group);

      foreach (GraphElement seletedElement in selection)
      {
        if (!(seletedElement is DialogueNode))
        {
          continue;
        }

        DialogueNode node = (DialogueNode)seletedElement;

        group.AddElement(node);
      }
      return group;
    }

    public void AddGroup(DialogueGroup group)
    {
      string groupName = group.title.ToLower();

      if (!groups.ContainsKey(groupName))
      {
        GroupErrorData groupErrorData = new GroupErrorData();
        groupErrorData.Groups.Add(group);
        groups.Add(groupName, groupErrorData);
        return;
      }

      List<DialogueGroup> groupsList = groups[groupName].Groups;

      groupsList.Add(group);

      Color errorColor = groups[groupName].ErrorData.Color;

      group.SetErrorStyle(errorColor);

      if (groupsList.Count == 2)
      {
        ++RepeatedNamesAmount;
        groupsList[0].SetErrorStyle(errorColor);
      }
    }

    private void RemoveGroup(DialogueGroup group)
    {
      string groupName = group.oldTitle.ToLower();

      List<DialogueGroup> groupsList = groups[groupName].Groups;

      groupsList.Remove(group);

      group.ResetStyle();

      if (groupsList.Count == 1)
      {
        --RepeatedNamesAmount;
        groupsList[0].ResetStyle();
      }

      if (groupsList.Count == 0)
      {
        groups.Remove(groupName);
      }
    }
    #endregion

    #region Nodes

    private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueType dialogueType)
    {
      ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
          menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode("DialogueName", dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
      );
      return contextualMenuManipulator;
    }

    public DialogueNode CreateNode(string nodeName, DialogueType dialogueType, Vector2 position, bool shouldDraw = true)
    {
      Type nodeType = Type.GetType($"RFG.Dialogue.{dialogueType}Node");

      DialogueNode node = (DialogueNode)Activator.CreateInstance(nodeType);
      node.Initialize(nodeName, this, position);

      if (shouldDraw)
      {
        node.Draw();
      }

      AddUngroupedNode(node);
      return node;
    }

    public void AddUngroupedNode(DialogueNode node)
    {
      string nodeName = node.DialogueName.ToLower();

      if (!ungroupedNodes.ContainsKey(nodeName))
      {
        NodeErrorData nodeErrorData = new NodeErrorData();
        nodeErrorData.Nodes.Add(node);
        ungroupedNodes.Add(nodeName, nodeErrorData);
        return;
      }

      List<DialogueNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

      ungroupedNodesList.Add(node);

      Color errorColor = ungroupedNodes[nodeName].ErrorData.Color;

      node.SetErrorStyle(errorColor);

      if (ungroupedNodesList.Count == 2)
      {
        ++RepeatedNamesAmount;
        ungroupedNodesList[0].SetErrorStyle(errorColor);
      }
    }

    public void RemoveUngroupedNode(DialogueNode node)
    {
      string nodeName = node.DialogueName.ToLower();

      List<DialogueNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

      ungroupedNodesList.Remove(node);
      node.ResetStyle();

      if (ungroupedNodesList.Count == 1)
      {
        --RepeatedNamesAmount;
        ungroupedNodesList[0].ResetStyle();
        return;
      }

      if (ungroupedNodesList.Count == 0)
      {
        ungroupedNodes.Remove(nodeName);
      }
    }

    public void AddGroupedNode(DialogueNode node, DialogueGroup group)
    {
      string nodeName = node.DialogueName.ToLower();

      node.Group = group;

      if (!groupedNodes.ContainsKey(group))
      {
        groupedNodes.Add(group, new SerializableDictionary<string, NodeErrorData>());
      }

      if (!groupedNodes[group].ContainsKey(nodeName))
      {
        NodeErrorData nodeErrorData = new NodeErrorData();
        nodeErrorData.Nodes.Add(node);
        groupedNodes[group].Add(nodeName, nodeErrorData);
        return;
      }

      List<DialogueNode> groupedNodesLists = groupedNodes[group][nodeName].Nodes;

      groupedNodesLists.Add(node);

      Color errorColor = groupedNodes[group][nodeName].ErrorData.Color;

      node.SetErrorStyle(errorColor);

      if (groupedNodesLists.Count == 2)
      {
        ++RepeatedNamesAmount;
        groupedNodesLists[0].SetErrorStyle(errorColor);
      }
    }

    public void RemoveGroupedNode(DialogueNode node, Group group)
    {
      string nodeName = node.DialogueName.ToLower();

      node.Group = null;

      List<DialogueNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

      groupedNodesList.Remove(node);

      node.ResetStyle();

      if (groupedNodesList.Count == 1)
      {
        --RepeatedNamesAmount;
        groupedNodesList[0].ResetStyle();
        return;
      }

      if (groupedNodesList.Count == 0)
      {
        groupedNodes[group].Remove(nodeName);

        if (groupedNodes[group].Count == 0)
        {
          groupedNodes.Remove(group);
        }
      }
    }
    #endregion

    #region Events
    private void OnElementsDeleted()
    {
      deleteSelection = (operationName, askUser) =>
      {
        Type groupType = typeof(DialogueGroup);
        Type edgeType = typeof(Edge);

        List<DialogueGroup> groupsToDelete = new List<DialogueGroup>();
        List<DialogueNode> nodesToDelete = new List<DialogueNode>();
        List<Edge> edgesToDelete = new List<Edge>();
        foreach (GraphElement element in selection)
        {
          if (element is DialogueNode node)
          {
            nodesToDelete.Add(node);
            continue;
          }

          if (element.GetType() == edgeType)
          {
            Edge edge = (Edge)element;
            edgesToDelete.Add(edge);
            continue;
          }

          if (element.GetType() != groupType)
          {
            continue;
          }

          DialogueGroup group = (DialogueGroup)element;

          groupsToDelete.Add(group);
        }

        foreach (DialogueGroup group in groupsToDelete)
        {
          List<DialogueNode> groupNodes = new List<DialogueNode>();
          foreach (GraphElement groupElement in group.containedElements)
          {
            if (!(groupElement is DialogueNode))
            {
              continue;
            }
            DialogueNode groupNode = (DialogueNode)groupElement;
            groupNodes.Add(groupNode);
          }

          group.RemoveElements(groupNodes);

          RemoveGroup(group);

          RemoveElement(group);
        }

        DeleteElements(edgesToDelete);

        foreach (DialogueNode node in nodesToDelete)
        {
          if (node.Group != null)
          {
            node.Group.RemoveElement(node);
          }
          RemoveUngroupedNode(node);
          node.DisconnectAllPorts();
          RemoveElement(node);
        }
      };
    }

    private void OnGroupElementsAdded()
    {
      elementsAddedToGroup = (group, elements) =>
      {
        foreach (GraphElement element in elements)
        {
          if (!(element is DialogueNode))
          {
            continue;
          }
          DialogueGroup nodeGroup = (DialogueGroup)group;
          DialogueNode node = (DialogueNode)element;

          RemoveUngroupedNode(node);
          AddGroupedNode(node, nodeGroup);
        }
      };
    }

    private void OnGroupElementsRemoved()
    {
      elementsRemovedFromGroup = (group, elements) =>
      {
        foreach (GraphElement element in elements)
        {
          if (!(element is DialogueNode))
          {
            continue;
          }

          DialogueNode node = (DialogueNode)element;

          RemoveGroupedNode(node, group);
          AddUngroupedNode(node);
        }
      };
    }

    private void OnGroupRenamed()
    {
      groupTitleChanged = (group, newTitle) =>
      {
        DialogueGroup DialogueGroup = (DialogueGroup)group;
        DialogueGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

        if (string.IsNullOrEmpty(DialogueGroup.title))
        {
          if (!string.IsNullOrEmpty(DialogueGroup.oldTitle))
          {
            ++RepeatedNamesAmount;
          }
        }
        else
        {
          if (string.IsNullOrEmpty(DialogueGroup.oldTitle))
          {
            --RepeatedNamesAmount;
          }
        }

        RemoveGroup(DialogueGroup);
        DialogueGroup.oldTitle = DialogueGroup.title;
        AddGroup(DialogueGroup);
      };
    }

    private void OnGraphViewChanged()
    {
      graphViewChanged = (changes) =>
      {
        if (changes.edgesToCreate != null)
        {
          foreach (Edge edge in changes.edgesToCreate)
          {
            DialogueNode nextNode = (DialogueNode)edge.input.node;
            ChoiceSaveData choiceData = (ChoiceSaveData)edge.output.userData;

            choiceData.NodeID = nextNode.ID;
          }
        }

        if (changes.elementsToRemove != null)
        {
          Type edgeType = typeof(Edge);

          foreach (GraphElement element in changes.elementsToRemove)
          {
            if (element.GetType() != edgeType)
            {
              continue;
            }

            Edge edge = (Edge)element;

            ChoiceSaveData choiceData = (ChoiceSaveData)edge.output.userData;

            choiceData.NodeID = "";
          }
        }
        return changes;
      };
    }
    #endregion

    #region Utilities

    public Vector2 GetLocalMousePosition(Vector2 position, bool isSearchWindow = false)
    {
      Vector2 worldMousePosition = position;

      if (isSearchWindow)
      {
        worldMousePosition -= editorWindow.position.position;
      }

      Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
      return localMousePosition;
    }

    public void ClearGraph()
    {
      graphElements.ForEach(graphElement => RemoveElement(graphElement));

      groups.Clear();
      groupedNodes.Clear();
      ungroupedNodes.Clear();

      RepeatedNamesAmount = 0;
    }

    public void ToggleMiniMap()
    {
      miniMap.visible = !miniMap.visible;
    }
    #endregion

  }
}