using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace RFG.Dialogue
{

  public class MultipleChoiceNode : DialogueNode
  {
    public override void Initialize(string nodeName, DialogueGraphView DialogueGraphView, Vector2 position)
    {
      base.Initialize(nodeName, DialogueGraphView, position);
      DialogueType = DialogueType.MultipleChoice;

      ChoiceSaveData choiceData = new ChoiceSaveData()
      {
        Text = "Next Choice"
      };

      Choices.Add(choiceData);
    }

    public override void Draw()
    {
      base.Draw();

      Button addChoiceButton = ElementUtility.CreateButton("Add Choice", () =>
      {
        ChoiceSaveData choiceData = new ChoiceSaveData()
        {
          Text = "Next Choice"
        };

        Choices.Add(choiceData);

        Port choicePort = CreateChoicePort(choiceData);
      });

      addChoiceButton.AddToClassList("ds-node__button");

      mainContainer.Insert(1, addChoiceButton);

      foreach (ChoiceSaveData choice in Choices)
      {
        Port choicePort = CreateChoicePort(choice);
      }

      RefreshExpandedState();
    }

    private Port CreateChoicePort(object userData)
    {
      Port choicePort = this.CreatePort();
      choicePort.portName = "";
      choicePort.userData = userData;

      ChoiceSaveData choiceData = (ChoiceSaveData)userData;

      Button deleteChoiceButton = ElementUtility.CreateButton("X", () =>
      {
        if (Choices.Count == 1)
        {
          return;
        }

        if (choicePort.connected)
        {
          graphView.DeleteElements(choicePort.connections);
        }

        Choices.Remove(choiceData);
        graphView.RemoveElement(choicePort);

      });
      deleteChoiceButton.AddToClassList("ds-node__button");

      TextField choiceTextField = ElementUtility.CreateTextField(choiceData.Text, null, callback =>
      {
        choiceData.Text = callback.newValue;
      });
      choiceTextField.AddClasses("ds-node__textfield", "ds-node__choice-textfield", "ds-node__textfield_hidden");

      choicePort.Add(choiceTextField);
      choicePort.Add(deleteChoiceButton);

      outputContainer.Add(choicePort);
      return choicePort;
    }
  }
}
