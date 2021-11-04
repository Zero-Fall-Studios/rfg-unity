using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace RFG.Dialogue
{
  public class SingleChoiceNode : DialogueNode
  {
    public override void Initialize(string nodeName, DialogueGraphView DialogueGraphView, Vector2 position)
    {
      base.Initialize(nodeName, DialogueGraphView, position);
      DialogueType = DialogueType.SingleChoice;

      ChoiceSaveData choiceData = new ChoiceSaveData()
      {
        Text = "Next Dialogue"
      };

      Choices.Add(choiceData);
    }

    public override void Draw()
    {
      base.Draw();

      foreach (ChoiceSaveData choice in Choices)
      {
        Port choicePort = this.CreatePort(choice.Text);
        choicePort.userData = choice;
        outputContainer.Add(choicePort);
      }

      RefreshExpandedState();
    }
  }
}
