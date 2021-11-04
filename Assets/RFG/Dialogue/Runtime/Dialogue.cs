using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFG.Dialogue
{
  public class Dialogue : ScriptableObject
  {
    [field: SerializeField] public string DialogueName { get; set; }
    [field: SerializeField] [field: TextArea()] public string Text { get; set; }
    [field: SerializeField] public List<DialogueChoiceData> Choices { get; set; }
    [field: SerializeField] public DialogueType DialogueType { get; set; }
    [field: SerializeField] public bool IsStartingDialogue { get; set; }

    public void Initialize(string dialogueName, string text, List<DialogueChoiceData> choices, DialogueType dialogueType, bool isStartingDialogue)
    {
      DialogueName = dialogueName;
      Text = text;
      Choices = choices;
      DialogueType = dialogueType;
      IsStartingDialogue = isStartingDialogue;
    }
  }
}