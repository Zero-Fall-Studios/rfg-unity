using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFG.Dialogue
{
  public class DialogueContainer : ScriptableObject
  {
    public string FileName { get; set; }
    public SerializableDictionary<DialogueGroupData, List<Dialogue>> DialogueGroups { get; set; }
    public List<Dialogue> UngroupedDialogues { get; set; }

    public void Initialize(string fileName)
    {
      FileName = fileName;
      DialogueGroups = new SerializableDictionary<DialogueGroupData, List<Dialogue>>();
      UngroupedDialogues = new List<Dialogue>();
    }
  }
}