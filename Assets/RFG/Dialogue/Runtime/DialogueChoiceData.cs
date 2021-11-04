using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFG.Dialogue
{
  [Serializable]
  public class DialogueChoiceData
  {
    [field: SerializeField] public string Text { get; set; }
    [field: SerializeField] public Dialogue NextDialogue { get; set; }
  }
}