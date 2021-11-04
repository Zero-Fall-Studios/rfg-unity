using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace RFG.Dialogue
{
  public class DialogueSearchWindow : ScriptableObject, ISearchWindowProvider
  {
    private DialogueGraphView graphView;
    private Texture2D indentationIcon;
    public void Initialize(DialogueGraphView DialogueGraphView)
    {
      graphView = DialogueGraphView;
      indentationIcon = new Texture2D(1, 1);
      indentationIcon.SetPixel(0, 0, Color.clear);
      indentationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
      List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
      {
        new SearchTreeGroupEntry(new GUIContent("Create Element")),
        new SearchTreeGroupEntry(new GUIContent("Dialague Node"), 1),
        new SearchTreeEntry(new GUIContent("Single Choice", indentationIcon))
        {
            level = 2,
            userData = DialogueType.SingleChoice
        },
        new SearchTreeEntry(new GUIContent("Multiple Choice", indentationIcon))
        {
            level = 2,
            userData = DialogueType.MultipleChoice
        },
        new SearchTreeGroupEntry(new GUIContent("Dialague Group"), 1),
        new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
        {
            level = 2,
            userData = new Group()
        },
      };

      return searchTreeEntries;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
      Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);
      switch (SearchTreeEntry.userData)
      {
        case DialogueType.SingleChoice:
          {
            SingleChoiceNode singleChoiceNode = (SingleChoiceNode)graphView.CreateNode("DialogueName", DialogueType.SingleChoice, localMousePosition);
            graphView.AddElement(singleChoiceNode);
            return true;
          }
        case DialogueType.MultipleChoice:
          {
            MultipleChoiceNode multipleChoiceNode = (MultipleChoiceNode)graphView.CreateNode("DialogueName", DialogueType.MultipleChoice, localMousePosition);
            graphView.AddElement(multipleChoiceNode);
            return true;
          }
        case Group _:
          {
            graphView.CreateGroup("DialogueGroup", localMousePosition);
            return true;
          }
        default:
          {
            return false;
          }
      }
    }
  }
}