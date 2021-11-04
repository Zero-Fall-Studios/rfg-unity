using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFG.Dialogue
{
  public class GraphSaveData : ScriptableObject
  {
    [field: SerializeField] public string FileName { get; set; }
    [field: SerializeField] public List<GroupSaveData> Groups { get; set; }
    [field: SerializeField] public List<NodeSaveData> Nodes { get; set; }
    [field: SerializeField] public List<string> OldGroupNames { get; set; }
    [field: SerializeField] public List<string> OldUngroupedNames { get; set; }
    [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupedNodeNames { get; set; }

    public void Initialize(string fileName)
    {
      FileName = fileName;

      Groups = new List<GroupSaveData>();
      Nodes = new List<NodeSaveData>();
    }
  }
}