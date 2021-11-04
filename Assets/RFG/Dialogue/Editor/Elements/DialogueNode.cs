using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace RFG.Dialogue
{
  public class DialogueNode : Node
  {
    public string ID { get; set; }
    public string DialogueName { get; set; }
    public List<ChoiceSaveData> Choices { get; set; }
    public string Text { get; set; }
    public DialogueType DialogueType { get; set; }
    public DialogueGroup Group { get; set; }
    private Color defaultBackgroundColor;
    protected DialogueGraphView graphView;

    public virtual void Initialize(string nodeName, DialogueGraphView DialogueGraphView, Vector2 position)
    {
      ID = Guid.NewGuid().ToString();
      DialogueName = nodeName;
      Choices = new List<ChoiceSaveData>();
      Text = "Dialogue Text";

      graphView = DialogueGraphView;
      defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

      SetPosition(new Rect(position, Vector2.zero));

      mainContainer.AddToClassList("ds-node__main-container");
      extensionContainer.AddToClassList("ds-node__extension-container");
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
      evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectPorts(inputContainer));
      evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectPorts(outputContainer));
      base.BuildContextualMenu(evt);
    }

    public virtual void Draw()
    {
      TextField dialogueNameTextField = ElementUtility.CreateTextField(DialogueName, null, callback =>
      {
        TextField target = (TextField)callback.target;
        target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

        if (string.IsNullOrEmpty(target.value))
        {
          if (!string.IsNullOrEmpty(DialogueName))
          {
            ++graphView.RepeatedNamesAmount;
          }
        }
        else
        {
          if (string.IsNullOrEmpty(DialogueName))
          {
            --graphView.RepeatedNamesAmount;
          }
        }

        if (Group == null)
        {
          graphView.RemoveUngroupedNode(this);
          DialogueName = target.value;
          graphView.AddUngroupedNode(this);
          return;
        }

        DialogueGroup currentGroup = Group;

        graphView.RemoveGroupedNode(this, currentGroup);
        DialogueName = callback.newValue;
        graphView.AddGroupedNode(this, currentGroup);
      });
      dialogueNameTextField.AddClasses("ds-node__textfield", "ds-node__filename-textfield", "ds-node__textfield__hidden");
      titleContainer.Insert(0, dialogueNameTextField);

      Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
      inputContainer.Add(inputPort);

      VisualElement customDataContainer = new VisualElement();

      customDataContainer.AddToClassList("ds-node__custom-data-container");

      Foldout textFoldout = ElementUtility.CreateFoldout("Dialogue Text");

      TextField textTextField = ElementUtility.CreateTextArea(Text, null, callback =>
      {
        Text = callback.newValue;
      });
      textTextField.AddClasses("ds-node__textfield", "ds-node__quote-textfield");
      textFoldout.Add(textTextField);

      customDataContainer.Add(textFoldout);

      extensionContainer.Add(customDataContainer);

    }

    public void DisconnectAllPorts()
    {
      DisconnectPorts(inputContainer);
      DisconnectPorts(outputContainer);
    }

    public void DisconnectPorts(VisualElement container)
    {
      foreach (Port port in container.Children())
      {
        if (!port.connected)
        {
          continue;
        }

        graphView.DeleteElements(port.connections);
      }
    }

    #region Utility Methods
    public bool IsStartingNode()
    {
      Port inputPort = (Port)inputContainer.Children().First();
      return !inputPort.connected;
    }

    public void SetErrorStyle(Color color)
    {
      mainContainer.style.backgroundColor = color;
    }

    public void ResetStyle()
    {
      mainContainer.style.backgroundColor = defaultBackgroundColor;
    }
    #endregion
  }
}