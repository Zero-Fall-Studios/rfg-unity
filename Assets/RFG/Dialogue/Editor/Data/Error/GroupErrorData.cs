using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace RFG.Dialogue
{
  public class GroupErrorData
  {
    public ErrorData ErrorData { get; set; }
    public List<DialogueGroup> Groups { get; set; }

    public GroupErrorData()
    {
      ErrorData = new ErrorData();
      Groups = new List<DialogueGroup>();
    }
  }
}