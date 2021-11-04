using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace RFG.Dialogue
{
  public static class IOUtility
  {
    public static string graphFileName;
    private static string containerFolderPath;
    private static DialogueGraphView graphView;
    private static List<DialogueGroup> groups;
    private static List<DialogueNode> nodes;
    private static Dictionary<string, DialogueGroupData> createdDialogueGroups;
    private static Dictionary<string, Dialogue> createdDialogues;
    private static Dictionary<string, DialogueGroup> loadedGroups;
    private static Dictionary<string, DialogueNode> loadedNodes;

    public static void Initialize(DialogueGraphView DialogueGraphView, string graphName)
    {
      graphView = DialogueGraphView;
      graphFileName = graphName;
      containerFolderPath = $"Assets/DialogueSystem/Dialogues/{graphFileName}";

      groups = new List<DialogueGroup>();
      nodes = new List<DialogueNode>();
      createdDialogueGroups = new Dictionary<string, DialogueGroupData>();
      createdDialogues = new Dictionary<string, Dialogue>();
      loadedGroups = new Dictionary<string, DialogueGroup>();
      loadedNodes = new Dictionary<string, DialogueNode>();
    }

    #region Save Methods
    public static void Save()
    {
      CreateStaticFolders();
      GetElementsFromGraphView();

      GraphSaveData graphData = CreateAsset<GraphSaveData>("Assets/Editor/DialogueSystem/Graphs", $"{graphFileName}Graph");
      graphData.Initialize(graphFileName);

      DialogueContainer dialogueContainer = CreateAsset<DialogueContainer>(containerFolderPath, graphFileName);
      dialogueContainer.Initialize(graphFileName);

      SaveGroups(graphData, dialogueContainer);
      SaveNodes(graphData, dialogueContainer);

      SaveAsset(graphData);
      SaveAsset(dialogueContainer);
    }

    private static void SaveGroups(GraphSaveData graphData, DialogueContainer dialogueContainer)
    {
      List<string> groupNames = new List<string>();
      foreach (DialogueGroup group in groups)
      {
        SaveGroupToGraph(group, graphData);
        SaveGroupToScriptableObject(group, dialogueContainer);
        groupNames.Add(group.title);
      }

      UpdateOldGroups(groupNames, graphData);
    }

    private static void SaveNodes(GraphSaveData graphData, DialogueContainer dialogueContainer)
    {
      SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
      List<string> ungroupedNodeName = new List<string>();
      foreach (DialogueNode node in nodes)
      {
        SaveNodeToGraph(node, graphData);
        SaveNodeToScriptableObject(node, dialogueContainer);

        if (node.Group != null)
        {
          groupedNodeNames.AddItem(node.Group.title, node.DialogueName);
          continue;
        }

        ungroupedNodeName.Add(node.DialogueName);
      }

      UpdateDialogueChoicesConnections();
      UpdateOldGroupedNodes(groupedNodeNames, graphData);
      UpdateOldUngroupedNodes(ungroupedNodeName, graphData);
    }

    private static void UpdateDialogueChoicesConnections()
    {
      foreach (DialogueNode node in nodes)
      {
        Dialogue dialogue = createdDialogues[node.ID];
        for (int i = 0; i < node.Choices.Count; i++)
        {
          ChoiceSaveData nodeChoice = node.Choices[i];
          if (string.IsNullOrEmpty(nodeChoice.NodeID))
          {
            continue;
          }
          dialogue.Choices[i].NextDialogue = createdDialogues[nodeChoice.NodeID];
          SaveAsset(dialogue);
        }
      }
    }

    private static void SaveGroupToGraph(DialogueGroup group, GraphSaveData graphData)
    {
      GroupSaveData groupData = new GroupSaveData()
      {
        ID = group.ID,
        Name = group.title,
        Position = group.GetPosition().position
      };

      graphData.Groups.Add(groupData);
    }

    private static void SaveGroupToScriptableObject(DialogueGroup group, DialogueContainer dialogueContainer)
    {
      string groupName = group.title;
      CreateFolder($"{containerFolderPath}/Groups", groupName);
      CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");

      DialogueGroupData dialogueGroup = CreateAsset<DialogueGroupData>($"{containerFolderPath}/Groups/{groupName}", groupName);
      dialogueGroup.Initialize(groupName);

      createdDialogueGroups.Add(group.ID, dialogueGroup);

      dialogueContainer.DialogueGroups.Add(dialogueGroup, new List<Dialogue>());

      SaveAsset(dialogueGroup);
    }

    private static void SaveNodeToGraph(DialogueNode node, GraphSaveData graphData)
    {
      List<ChoiceSaveData> choices = CloneNodeChoices(node.Choices);

      NodeSaveData nodeData = new NodeSaveData()
      {
        ID = node.ID,
        Name = node.DialogueName,
        Choices = choices,
        Text = node.Text,
        GroupID = node.Group?.ID,
        DialogueType = node.DialogueType,
        Position = node.GetPosition().position
      };

      graphData.Nodes.Add(nodeData);
    }

    private static void SaveNodeToScriptableObject(DialogueNode node, DialogueContainer dialogueContainer)
    {
      Dialogue dialogue;
      if (node.Group != null)
      {
        dialogue = CreateAsset<Dialogue>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);
        dialogueContainer.DialogueGroups.AddItem(createdDialogueGroups[node.Group.ID], dialogue);
      }
      else
      {
        dialogue = CreateAsset<Dialogue>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);
        dialogueContainer.UngroupedDialogues.Add(dialogue);
      }
      dialogue.Initialize(node.DialogueName, node.Text, ConvertNodeChoicesToDialogChoices(node.Choices), node.DialogueType, node.IsStartingNode());

      createdDialogues.Add(node.ID, dialogue);

      SaveAsset(dialogue);
    }

    private static List<DialogueChoiceData> ConvertNodeChoicesToDialogChoices(List<ChoiceSaveData> nodeChoices)
    {
      List<DialogueChoiceData> dialogueChoices = new List<DialogueChoiceData>();
      foreach (ChoiceSaveData nodeChoice in nodeChoices)
      {
        DialogueChoiceData choiceData = new DialogueChoiceData()
        {
          Text = nodeChoice.Text
        };
        dialogueChoices.Add(choiceData);
      }
      return dialogueChoices;
    }

    private static void UpdateOldGroups(List<string> currentGroupNames, GraphSaveData graphData)
    {
      if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
      {
        List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

        foreach (string groupToRemove in groupsToRemove)
        {
          RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
        }
      }
      graphData.OldGroupNames = new List<string>(currentGroupNames);
    }

    private static void UpdateOldUngroupedNodes(List<string> ungroupedNodeNames, GraphSaveData graphData)
    {
      if (graphData.OldUngroupedNames != null && graphData.OldUngroupedNames.Count != 0)
      {
        List<string> nodesToRemove = graphData.OldUngroupedNames.Except(ungroupedNodeNames).ToList();

        foreach (string nodeToRemove in nodesToRemove)
        {
          RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
        }
      }
      graphData.OldUngroupedNames = new List<string>(ungroupedNodeNames);
    }

    private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> groupedNodeNames, GraphSaveData graphData)
    {
      if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
      {
        foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
        {
          List<string> nodesToRemove = new List<string>();
          if (groupedNodeNames.ContainsKey(oldGroupedNode.Key))
          {
            nodesToRemove = oldGroupedNode.Value.Except(groupedNodeNames[oldGroupedNode.Key]).ToList();
          }

          foreach (string nodeToRemove in nodesToRemove)
          {
            RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
          }
        }
      }

      graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(groupedNodeNames);
    }
    #endregion

    #region Load Methods
    public static void Load()
    {
      GraphSaveData graphData = LoadAsset<GraphSaveData>("Assets/Editor/DialogueSystem/Graphs", graphFileName);
      if (graphData == null)
      {
        EditorUtility.DisplayDialog("Couldn't load the file", "The file at the following path could not be found: \n\n" + $"Assets/Editor/DialogueSystem/Graphs/{graphFileName}\n\n" + " Make sure you chose the right file and it's placed at the folder path mentioned above.", "Thanks!");
        return;
      }

      DialogueEditorWindow.UpdateFileName(graphData.FileName);

      LoadGroups(graphData.Groups);
      LoadNodes(graphData.Nodes);
      LoadNodesConnections();
    }

    private static void LoadGroups(List<GroupSaveData> groups)
    {
      foreach (GroupSaveData groupData in groups)
      {
        DialogueGroup group = (DialogueGroup)graphView.CreateGroup(groupData.Name, groupData.Position);
        group.ID = groupData.ID;
        loadedGroups.Add(group.ID, group);
      }
    }
    private static void LoadNodes(List<NodeSaveData> nodes)
    {
      foreach (NodeSaveData nodeData in nodes)
      {
        List<ChoiceSaveData> choices = CloneNodeChoices(nodeData.Choices);
        DialogueNode node = (DialogueNode)graphView.CreateNode(nodeData.Name, nodeData.DialogueType, nodeData.Position, false);
        node.ID = nodeData.ID;
        node.Choices = choices;
        node.Text = nodeData.Text;
        node.Draw();
        graphView.AddElement(node);

        loadedNodes.Add(node.ID, node);

        if (string.IsNullOrEmpty(nodeData.GroupID))
        {
          continue;
        }

        DialogueGroup group = loadedGroups[nodeData.GroupID];
        node.Group = group;

        group.AddElement(node);
      }
    }

    private static void LoadNodesConnections()
    {
      foreach (KeyValuePair<string, DialogueNode> loadedNode in loadedNodes)
      {
        foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
        {
          ChoiceSaveData choiceData = (ChoiceSaveData)choicePort.userData;
          if (string.IsNullOrEmpty(choiceData.NodeID))
          {
            continue;
          }

          DialogueNode nextNode = loadedNodes[choiceData.NodeID];

          Port nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();
          Edge edge = choicePort.ConnectTo(nextNodeInputPort);
          graphView.AddElement(edge);

          loadedNode.Value.RefreshPorts();
        }
      }
    }

    #endregion

    #region Creation Methods
    private static void CreateStaticFolders()
    {
      CreateFolder("Assets/Editor/DialogueSystem", "Graphs");
      CreateFolder("Assets", "DialogueSystem");
      CreateFolder("Assets/DialogueSystem", "Dialogues");
      CreateFolder("Assets/DialogueSystem/Dialogues", graphFileName);
      CreateFolder(containerFolderPath, "Global");
      CreateFolder(containerFolderPath, "Groups");
      CreateFolder($"{containerFolderPath}/Global", "Dialogues");
    }
    #endregion

    #region Fetch Methods
    private static void GetElementsFromGraphView()
    {
      Type groupType = typeof(DialogueGroup);
      graphView.graphElements.ForEach(graphElement =>
      {
        if (graphElement is DialogueNode node)
        {
          nodes.Add(node);
          return;
        }
        if (graphElement.GetType() == groupType)
        {
          DialogueGroup group = (DialogueGroup)graphElement;
          groups.Add(group);
          return;
        }
      });
    }
    #endregion

    #region Utility Methods
    private static void CreateFolder(string path, string folderName)
    {
      if (AssetDatabase.IsValidFolder($"{path}/{folderName}/"))
      {
        return;
      }

      AssetDatabase.CreateFolder(path, folderName);
    }

    private static void RemoveFolder(string fullPath)
    {
      FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
      FileUtil.DeleteFileOrDirectory($"{fullPath}/");
    }

    private static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
    {
      string fullPath = $"{path}/{assetName}.asset";

      T asset = LoadAsset<T>(path, assetName);

      if (asset == null)
      {
        asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, fullPath);
      }

      return asset;
    }

    private static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
    {
      string fullPath = $"{path}/{assetName}.asset";
      return AssetDatabase.LoadAssetAtPath<T>(fullPath);
    }

    private static void RemoveAsset(string path, string assetName)
    {
      AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
    }

    private static void SaveAsset(UnityEngine.Object asset)
    {
      EditorUtility.SetDirty(asset);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }

    private static List<ChoiceSaveData> CloneNodeChoices(List<ChoiceSaveData> nodeChoices)
    {
      List<ChoiceSaveData> choices = new List<ChoiceSaveData>();

      foreach (ChoiceSaveData choice in nodeChoices)
      {
        ChoiceSaveData choiceData = new ChoiceSaveData()
        {
          Text = choice.Text,
          NodeID = choice.NodeID
        };
        choices.Add(choiceData);
      }

      return choices;
    }
    #endregion


  }
}