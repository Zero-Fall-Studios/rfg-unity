using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFG.Dialogue
{
  public class DialogueGroupData : ScriptableObject
  {
    [field: SerializeField] public string GroupName { get; set; }

    public void Initialize(string groupName)
    {
      GroupName = groupName;
    }
  }
}