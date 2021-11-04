using System.Collections.Generic;

namespace RFG.Dialogue
{
  public class NodeErrorData
  {
    public ErrorData ErrorData { get; set; }
    public List<DialogueNode> Nodes { get; set; }

    public NodeErrorData()
    {
      ErrorData = new ErrorData();
      Nodes = new List<DialogueNode>();
    }

  }
}